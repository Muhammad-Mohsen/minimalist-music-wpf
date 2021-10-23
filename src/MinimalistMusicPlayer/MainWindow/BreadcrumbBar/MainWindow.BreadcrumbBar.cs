using MinimalistMusicPlayer.Explorer;
using MinimalistMusicPlayer.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MinimalistMusicPlayer
{
	public partial class MainWindow
	{
		public DirectoryInfo CurrentDirectory { get; set; }

		// will be called on directory change (up button click, breadcrumb button click, directory item double click)
		public void DirectoryChange(DirectoryInfo directory)
		{
			// if changing to the same directory (when clicking the track button) don't do anything
			if (directory != null && CurrentDirectory != null && directory.FullName == CurrentDirectory.FullName) return;
			if (directory == null && CurrentDirectory == null) return;

			Thickness toMargin = GetExplorerAnimationMargin(CurrentDirectory, directory);
			Thickness fromMargin = GetExplorerAnimationMargin(directory, CurrentDirectory);

			var toScale = GetExplorerAnimationScale(CurrentDirectory, directory);
			var fromScale = GetExplorerAnimationScale(directory, CurrentDirectory);

			CurrentDirectory = directory;

			// set the setting (will be saved in OnExit event in the app class!!)
			ApplicationSettings.Instance.ExplorerDirectory = directory?.FullName;

			var explorerToHide = ScrollViewerExplorer;

			// get paged media explorer
			ScrollViewerExplorer = GetExplorer(directory);
			StackPanelExplorer = ScrollViewerExplorer.Content as StackPanel;

			// animate the media explorer current -> paged
			// Anim.AnimateMargin(explorerToHide, Const.ExplorerMargin.CurrentPage, toMargin, Const.ShowHideDelay);
			explorerToHide.AnimateRenderScale(Constant.DrillScale.Normal, toScale, Constant.DrillAnimDuration);
			explorerToHide.AnimateOpacity(Constant.OpacityLevel.Transparent, Constant.DrillAnimDuration, (object sender, EventArgs args) => explorerToHide.Visibility = Visibility.Collapsed);
			// repopulate the breadcrumb bar
			PopulateBreadcrumbBar(directory);
			// animate the media explorer paged -> current
			// Anim.AnimateMargin(ScrollViewerExplorer, fromMargin, Const.ExplorerMargin.CurrentPage, Const.ShowHideDelay);
			ScrollViewerExplorer.AnimateRenderScale(fromScale, Constant.DrillScale.Normal, Constant.DrillAnimDuration);
			ScrollViewerExplorer.Visibility = Visibility.Visible;
			ScrollViewerExplorer.AnimateOpacity(Constant.OpacityLevel.Opaque, Constant.DrillAnimDuration);

			// reset item markings
			MediaItem.MarkedItemCount = 0;
			SetToolbarMode(ToolbarMode.Breadcrumb);
		}

		// attaches the Up button click event, and calls PopulateBreadcrumbBar
		public void InitializeBreadcrumbBar(DirectoryInfo directory)
		{
			ButtonUp.Click += UpButton_Click;
			ButtonSearch.Click += SearchButton_Click;
			TextBoxSearch.TextChanged += TextBoxSearch_TextChanged;

			PopulateBreadcrumbBar(directory);
		}

		// reinitializes the breadcrumb bar using already existing breadcrumb buttons
		public void PopulateBreadcrumbBar(DirectoryInfo directory)
		{
			StackPanelDirectory.Children.Clear(); // clear everything and start over

			BreadcrumbButton.AddCrumb(StackPanelDirectory, Constant.Root, ComputerButton_Click); // Add the computer button

			// will be null at the root
			if (directory != null)
			{
				// break up the full path
				string[] breadcrumbs = directory.FullName.Split(Constant.DirectorySeparators);

				foreach (var (crumb, index) in breadcrumbs.WithIndex())
				{
					if (string.IsNullOrWhiteSpace(crumb)) continue; // removes the extra crumb for drive paths
					BreadcrumbButton.AddCrumb(StackPanelDirectory, crumb, BreadcrumbButton_Click, index == breadcrumbs.Length - 1);
				}

				ScrollViewerDirectory.ScrollToRightEnd();
			}
		}

		private async void SetToolbarMode(ToolbarMode mode)
		{
			if (GetToolbarMode() == mode) return;

			TextBoxSearch.Text = string.Empty;

			var upButtonRotation = mode == ToolbarMode.Search ? Constant.UpButtonAngle.Back : Constant.UpButtonAngle.Up;
			(ButtonUp.Content as FrameworkElement).AnimateAngle(upButtonRotation, Constant.ShowHideDelay, false);

			var hide = new List<FrameworkElement>();
			var show = new List<FrameworkElement>();

			switch (mode)
			{
				case ToolbarMode.Breadcrumb:
					hide.Add(GridSelectMode, TextBoxSearch);
					show.Add(StackPanelDirectory, ButtonSearch, ButtonUp);
					break;
				case ToolbarMode.Selection:
					hide.Add(StackPanelDirectory, ButtonSearch, TextBoxSearch, ButtonUp);
					Extensions.Add(show, GridSelectMode);
					break;
				case ToolbarMode.Search:
					hide.Add(ButtonSearch, StackPanelDirectory, GridSelectMode);
					show.Add(TextBoxSearch, ButtonUp);
					break;
			}

			hide.ForEach(h => h.Toggle(false, Constant.ShowHideDelay));
			await Task.Delay(TimeSpan.FromSeconds(Constant.ShowHideDelay)).ConfigureAwait(true);
			show.ForEach(s => s.Toggle(true, Constant.ShowHideDelay));
		}
		private ToolbarMode GetToolbarMode()
		{
			if (GridSelectMode.Visibility == Visibility.Visible) return ToolbarMode.Selection;
			if (TextBoxSearch.Visibility == Visibility.Visible) return ToolbarMode.Search;
			return ToolbarMode.Breadcrumb;
		}

		private void SetAddToSelectionEnableState(string currentDirectory, string playlistDirectory, int playlistCount)
		{
			ButtonAddToSelection.IsEnabled = currentDirectory == playlistDirectory && playlistCount > 0;
			(ButtonAddToSelection.Content as System.Windows.Shapes.Path).Stroke = ButtonAddToSelection.IsEnabled ? Brushes.PrimaryTextBrush : Brushes.AccentBrush;
		}
		//
		// click handlers
		//
		private void UpButton_Click(object sender, RoutedEventArgs e)
		{
			if (GetToolbarMode() == ToolbarMode.Search)
			{
				SetToolbarMode(ToolbarMode.Breadcrumb);
				return;
			}

			if (CurrentDirectory == null) return; // if you're at the computer root, do nothing

			DirectoryInfo parentDirectory = CurrentDirectory.Parent;
			DirectoryChange(parentDirectory);
		}
		private void ComputerButton_Click(object sender, RoutedEventArgs e)
		{
			// set the current directory to null, and repopulate everything
			DirectoryChange(null);
		}
		private void BreadcrumbButton_Click(object sender, RoutedEventArgs e)
		{
			BreadcrumbButton breadcrumbButton = (BreadcrumbButton)sender;

			// get the current directory up to that particular bread crumb button
			DirectoryInfo newDirectory = new DirectoryInfo(GetDirectory(breadcrumbButton));

			// if you click a crumb button that points to the same directory as the current, don't do anything
			if (newDirectory.FullName == CurrentDirectory.FullName)
				return;

			DirectoryChange(newDirectory);
		}

		// activate search mode
		private void SearchButton_Click(object sender, RoutedEventArgs e)
		{
			SetToolbarMode(ToolbarMode.Search);
		}
		private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
		{
			FilterMediaExplorer(TextBoxSearch.Text);
		}


		// gets the directory up to the given crumb
		private string GetDirectory(BreadcrumbButton crumbButton)
		{
			StringBuilder directory = new StringBuilder();

			foreach (BreadcrumbButton button in StackPanelDirectory.Children)
			{
				directory.Append(button.Content.ToString());
				if (button.Equals(crumbButton)) break; // if you get to where you want, break
			}

			directory.Replace("Computer/", ""); // remove the "Computer/"

			// if you get to the root, append a '/'
			if (directory.Length == 2) directory.Append(Constant.BreadcrumbButtonSeparator);

			return directory.ToString();
		}
		//
		// Select mode click handlers
		//
		private void ButtonPlaySelected_Click(object sender, RoutedEventArgs e)
		{
			// get marked media files
			var markedFiles = GetMarkedMediaFiles();

			if (markedFiles.Count == 0) return;

			// reset everything
			SetPlaylistMediaItemStyle(Playlist.Tracks, false);

			// reinitialize playlist
			Playlist.Clear();
			Playlist.AddTracks(markedFiles);

			// reset marking state
			ResetMediaItemMarkState();
			MediaItem.MarkedItemCount = 0;

			SetPlaylistMediaItemStyle(markedFiles, true);

			Playlist.CurrentIndex = 0;
			Player.PlayTrack(Playlist.GetTrack(Playlist.CurrentIndex));

			SetToolbarMode(ToolbarMode.Breadcrumb);
			UpdateUi();
		}
		private void ButtonCancelPlaySelected_Click(object sender, RoutedEventArgs e)
		{
			// reset marking state
			ResetMediaItemMarkState();
			MediaItem.MarkedItemCount = 0;

			SetToolbarMode(ToolbarMode.Breadcrumb);
		}
		private void ButtonAddToSelection_Click(object sender, RoutedEventArgs e)
		{
			// get marked media files
			var markedFiles = GetMarkedMediaFiles();

			// reset everything
			SetPlaylistMediaItemStyle(Playlist.Tracks, false);

			// reset marking state
			ResetMediaItemMarkState();
			MediaItem.MarkedItemCount = 0;

			// add media files to playlist
			Playlist.AddTracks(markedFiles);

			// this time, set the item style for the entire playlist (as opposed to the marked files)
			SetPlaylistMediaItemStyle(Playlist.Tracks, true);

			SetToolbarMode(ToolbarMode.Breadcrumb);
		}
	}

	public enum ToolbarMode
	{
		Breadcrumb,
		Selection,
		Search
	}
}
