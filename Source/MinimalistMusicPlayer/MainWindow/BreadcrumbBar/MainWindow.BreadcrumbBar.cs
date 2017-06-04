using MinimalistMusicPlayer.Explorer;
using MinimalistMusicPlayer.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MinimalistMusicPlayer
{
	public partial class MainWindow : Window
	{
		public DirectoryInfo CurrentDirectory { get; set; }

		// will be called on directory change (up button click, breadcrumb button click, directory item double click)
		public void DirectoryChange(DirectoryInfo directory)
		{
			// if changing to the same directory (when clicking the track button) don't do anything
			if (directory != null && CurrentDirectory != null && directory.FullName == CurrentDirectory.FullName)
				return;

			if (directory == null && CurrentDirectory == null)
				return;

			Thickness toMargin = GetExplorerAnimationMargin(CurrentDirectory, directory);
			Thickness fromMargin = GetExplorerAnimationMargin(directory, CurrentDirectory);

			CurrentDirectory = directory;

			// set the setting (will be saved in OnExit event in the app class!!)
			Properties.Settings.Default[Const.ExplorerDirectorySetting] = directory != null ? directory.FullName : null;

			// get paged media explorer
			ScrollViewerExplorer = GetPagedScrollViewerExplorer();
			StackPanelExplorer = GetPagedStackPanelExplorer();

			// populate paged media explorer (asynchronously)
			PopulateMediaExplorer(directory);

			// animate the media explorer current -> paged
			Anim.AnimateMargin(GetPagedScrollViewerExplorer(), Const.ExplorerMargin.CurrentPage, toMargin, Const.ShowHideDelay);
			// repopulate the breadcrumb bar
			PopulateBreadcrumbBar(directory);
			// animate the media explorer paged -> current
			Anim.AnimateMargin(ScrollViewerExplorer, fromMargin, Const.ExplorerMargin.CurrentPage, Const.ShowHideDelay);

			// reset item markings
			MediaItem.MarkedItemCount = 0;
			SetPlaylistSelectMode(false);
		}

		// attaches the Up button click event, and calls PopulateBreadcrumbBar
		public void InitializeBreadcrumbBar(DirectoryInfo directory)
		{
			ButtonUp.Click += UpButton_Click;
			PopulateBreadcrumbBar(directory);
		}	
		// reinitializes the breadcrumb bar using already existing breadcrumb buttons
		public void PopulateBreadcrumbBar(DirectoryInfo directory)
		{
			// clear everything and start over
			StackPanelDirectory.Children.Clear();

			// Add the computer button
			BreadcrumbButton computerButton = new BreadcrumbButton(Const.Computer);
			computerButton.Click += ComputerButton_Click;
			StackPanelDirectory.Children.Add(computerButton);

			BreadcrumbButton computerSeparatorButton = CreateSeparatorButton();
			StackPanelDirectory.Children.Add(computerSeparatorButton);

			// will be null at the root
			if (directory != null)
			{
				// break up the full path
				string[] breadcrumbs = directory.FullName.Split(Const.DirectorySeparators);

				foreach (string crumb in breadcrumbs)
				{
					if (string.IsNullOrWhiteSpace(crumb)) // removes the extra crumb for drive paths
						continue;

					// add the breadcrumb button to the breadcrumb bar
					BreadcrumbButton button = new BreadcrumbButton(crumb);
					button.Click += BreadcrumbButton_Click;
					StackPanelDirectory.Children.Add(button);

					// click handler isn't wired up to the event
					BreadcrumbButton separatorButton = CreateSeparatorButton();
					StackPanelDirectory.Children.Add(separatorButton);
				}

				ScrollViewerDirectory.ScrollToRightEnd();
			}

		}
		
		private BreadcrumbButton CreateSeparatorButton()
		{
			BreadcrumbButton separatorButton = new BreadcrumbButton(Const.BreadcrumbButtonSeparator);
			separatorButton.IsEnabled = false;

			return separatorButton;
		}

		// gets the directory up to the given crumb
		private string GetDirectory(BreadcrumbButton crumbButton)
		{
			StringBuilder directory = new StringBuilder();

			foreach (BreadcrumbButton button in StackPanelDirectory.Children)
			{
				directory.Append(button.Content.ToString());
				if (button.Equals(crumbButton)) // if you get to where you want, break
					break;
			}

			// remove the "Computer/"
			directory.Replace("Computer/", "");

			// if you get to the root, append a '/'
			if (directory.Length == 2)
				directory.Append(Const.BreadcrumbButtonSeparator);

			return directory.ToString();
		}

		private async void SetPlaylistSelectMode(bool shouldShow)
		{
			FrameworkElement elementToHide = shouldShow ? GridBreadcrumnBar : GridSelectMode;
			FrameworkElement elementToShow = shouldShow ? GridSelectMode : GridBreadcrumnBar;

			Util.ShowHideFrameworkElement(elementToHide, false, Const.ShowHideDelay);
			await Task.Delay(TimeSpan.FromSeconds(Const.ShowHideDelay));
			Util.ShowHideFrameworkElement(elementToShow, true, Const.ShowHideDelay);
		}

		private void SetAddToSelectionEnableState(string currentDirectory, string playlistDirectory, int playlistCount)
		{
			ButtonAddToSelection.IsEnabled = currentDirectory == playlistDirectory && playlistCount > 0;
		}
		//
		// click handlers
		//
		private void UpButton_Click(object sender, RoutedEventArgs e)
		{
			// if you're at the computer root, do nothing
			if (CurrentDirectory == null)
				return;

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
		//
		// Select mode click handlers
		//
		private void ButtonPlaySelected_Click(object sender, RoutedEventArgs e)
		{
			// get marked media files
			List<string> markedFiles = GetMarkedMediaFileFullNames();

			if (markedFiles.Count == 0)
				return;

			// reset everything
			SetPlaylistMediaItemStyle(Player.PlaylistFullNames, false);

			// reinitialize playlist
			Player.ClearPlaylistItems();
			Player.AddPlaylistItems(markedFiles);

			// reset marking state
			ResetMediaItemMarkState();
			MediaItem.MarkedItemCount = 0;

			SetPlaylistMediaItemStyle(markedFiles, true);

			Player.Index = 0;
			Player.Play(Player.Index);

			SetPlaylistSelectMode(false);
		}
		private void ButtonCancelPlaySelected_Click(object sender, RoutedEventArgs e)
		{
			// reset marking state
			ResetMediaItemMarkState();
			MediaItem.MarkedItemCount = 0;

			SetPlaylistSelectMode(false);
		}
		private void ButtonAddToSelection_Click(object sender, RoutedEventArgs e)
		{
			// get marked media files
			List<string> markedFiles = GetMarkedMediaFileFullNames();

			// reset everything
			SetPlaylistMediaItemStyle(Player.PlaylistFullNames, false);

			// reset marking state
			ResetMediaItemMarkState();
			MediaItem.MarkedItemCount = 0;

			// add media files to playlist
			Player.AddPlaylistItems(markedFiles);
			
			// this time, set the item style for the entire playlist (as opposed to the marked files)
			SetPlaylistMediaItemStyle(Player.PlaylistFullNames, true);

			SetPlaylistSelectMode(false);
		}
	}
}