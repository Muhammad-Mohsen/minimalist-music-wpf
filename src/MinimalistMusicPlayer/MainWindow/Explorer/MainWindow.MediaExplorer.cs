using MinimalistMusicPlayer.Explorer;
using MinimalistMusicPlayer.Media;
using MinimalistMusicPlayer.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MinimalistMusicPlayer
{
	// the explorer piece of MainWindow
	public partial class MainWindow
	{
		public MediaFile[] DirectoryMediaFiles { get; set; }

		public StackPanel StackPanelExplorer { get; set; }
		public ScrollViewer ScrollViewerExplorer { get; set; }

		// whether the playlist is visible
		public bool IsPlaylistVisible { get; set; }

		public async void PopulateMediaExplorer(StackPanel explorer, DirectoryInfo directory)
		{
			AddExplorerItemsAsync(explorer, directory);
			await Task.Delay(TimeSpan.FromTicks(0)).ConfigureAwait(false); // just to make the method async!
		}

		// adds explorer items that are not Drives
		private async void AddExplorerItemsAsync(StackPanel panel, DirectoryInfo newDirectory)
		{
			// if at the root of the HDD
			if (newDirectory == null)
			{
				// added synchronously!
				DriveInfo.GetDrives().Where(drive => drive.IsReady).ToList().ForEach(drive =>
				{
					DriveItem driveItem = new DriveItem(drive.RootDirectory.FullName);
					driveItem.MouseDoubleClick += DriveItem_MouseDoubleClick;
					panel.Children.Add(driveItem);
				});

				return;
			}

			var subDirectories = newDirectory.GetDirectories().Where(x => (x.Attributes & FileAttributes.Hidden) == 0).ToArray();
			DirectoryMediaFiles = newDirectory.GetMediaFiles();

			foreach (DirectoryInfo dir in subDirectories)
			{
				AddExplorerItem(panel, dir.FullName, -1);
				await Task.Delay(TimeSpan.FromMilliseconds(Constant.AsyncDelay)).ConfigureAwait(true);
			}

			var files = newDirectory.GetMediaFiles();
			for (int i = 0; i < files.Length; i++)
			{
				AddExplorerItem(panel, files[i].FullName, i);
				await Task.Delay(TimeSpan.FromMilliseconds(Constant.AsyncDelay)).ConfigureAwait(true);
			}
		}

		public async void AddExplorerItem(StackPanel panel, string itemPath, int i)
		{
			ExplorerItem item;
			if (i == -1) item = CreateDirectoryItem(itemPath);
			else item = CreateMediaItem(new MediaFile(itemPath));

			item.Opacity = Constant.OpacityLevel.Transparent;
			panel.Children.Add(item);

			Anim.Toggle(item, true, Constant.ShowHideDelay);
			await Task.Delay(TimeSpan.FromSeconds(Constant.ShowHideDelay)).ConfigureAwait(false);
		}

		// Directory Items
		public DirectoryItem CreateDirectoryItem(string directory)
		{
			DirectoryItem directoryItem = new DirectoryItem(directory);
			directoryItem.MouseDoubleClick += DirectoryItem_MouseDoubleClick;

			return directoryItem;
		}
		private void DirectoryItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			DirectoryItem directoryItem = (DirectoryItem)sender;
			DirectoryChange(new DirectoryInfo(directoryItem.Directory));
		}

		// Media Items
		public MediaItem CreateMediaItem(MediaFile mediaFile)
		{
			MediaItemStyle mediaItemStyle = GetMediaItemStyle(mediaFile.FullName);

			bool isSelected = false;
			if (CurrentDirectory != null && CurrentDirectory.FullName == Playlist.PlaylistDirectory)
				isSelected = Playlist.CurrentIndex == Playlist.IndexOf(mediaFile.FullName, CurrentDirectory);

			MediaItem mediaItem = new MediaItem(mediaFile, mediaItemStyle, isSelected);
			mediaItem.MouseDoubleClick += MediaItem_MouseDoubleClick;
			mediaItem.MarkedItemCountChange += MediaItem_MarkedItemCountChange;

			return mediaItem;
		}
		private void MediaItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			MediaItem item = (MediaItem)sender;

			Playlist.CurrentIndex = Playlist.IndexOf(item.FullName, CurrentDirectory);

			// if the item is not in the current playlist, repopulate the playlist with all the files in the current directory then play the item
			if (Playlist.CurrentIndex == Constant.InvalidIndex)
			{
				Playlist.Clear();
				Playlist.AddTracks(DirectoryMediaFiles);
				Playlist.CurrentIndex = Playlist.IndexOf(item.FullName, CurrentDirectory); // update index

				// reset the icons for this particular item
				SetPlaylistMediaItemStyle(Playlist.Tracks, false);
				SetMediaItemForeground(Playlist.Tracks);

				// reset marking states for all items
				ResetMediaItemMarkState();
				MediaItem.MarkedItemCount = 0;

				// hide the select mode if applicable
				SetToolbarMode(ToolbarMode.Breadcrumb);
			}

			var track = Playlist.GetTrack(Playlist.CurrentIndex);
			Player.PlayTrack(track);
			UpdateUi();
		}
		// controls whether the Play selected button should be shown
		private void MediaItem_MarkedItemCountChange(object sender, RoutedEventArgs e)
		{
			bool shouldShowSelectMode = MediaItem.MarkedItemCount > 0;
			SetToolbarMode(shouldShowSelectMode ? ToolbarMode.Selection : ToolbarMode.Breadcrumb);

			// only enable AddToSelection button if we're in the same directory as the playlist, and the playlist is not empty
			if (shouldShowSelectMode)
				SetAddToSelectionEnableState(CurrentDirectory.FullName, Playlist.PlaylistDirectory, Playlist.Count);
		}

		// Drive Items
		public DriveItem CreateDriveItem(string root)
		{
			DriveItem driveItem = new DriveItem(root);
			driveItem.MouseDoubleClick += DriveItem_MouseDoubleClick;

			return driveItem;
		}
		private void DriveItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			DriveItem driveItem = (DriveItem)sender;
			DirectoryChange(new DirectoryInfo(driveItem.Directory));
		}

		// maps a given playlist index to an actual MediaItem object
		// mapping isn't 1:1 because there are directories and DirectoryItems thrown in the mix!
		public MediaItem GetMediaItemByPlaylistIndex(int playlistIndex)
		{
			string mediaItemFullName = Playlist.Tracks[playlistIndex].FullName; // get the playlist media item

			// skip over the directory items
			foreach (MediaItem item in StackPanelExplorer.Children.OfType<MediaItem>())
			{
				if (item.FullName == mediaItemFullName) return item;
			}
			return null;
		}

		// gets a list of marked MediaItems' FullNames
		public List<MediaFile> GetMarkedMediaFiles()
		{
			List<MediaFile> markedFiles = new List<MediaFile>();

			foreach (MediaItem item in StackPanelExplorer.Children.OfType<MediaItem>())
			{
				if (item.IsMarked) markedFiles.Add(new MediaFile(item.FullName));
			}

			return markedFiles;
		}

		// resets mark state for all media items
		// called when starting to play selected media
		public void ResetMediaItemMarkState()
		{
			foreach (MediaItem item in StackPanelExplorer.Children.OfType<MediaItem>())
			{
				item.IsMarked = false;
				MediaItem.MarkMediaIcon(item.MediaIcon, false);
			}
		}

		// returns whether the given media is contained in the currently playing playlist
		public MediaItemStyle GetMediaItemStyle(string fullName)
		{
			if (CurrentDirectory != null && CurrentDirectory.FullName == Playlist.PlaylistDirectory)
			{
				if (DirectoryMediaFiles.Length == Playlist.Tracks.Count) return MediaItemStyle.Highlighted;
				else if (Playlist.Contains(fullName)) return MediaItemStyle.IconHighlighted;
			}

			return MediaItemStyle.Normal;
		}

		// sets the playlist icon for selected media files
		public void SetPlaylistMediaItemStyle(List<MediaFile> tracks, bool toPlaylist)
		{
			if (CurrentDirectory != null && CurrentDirectory.FullName != Playlist.PlaylistDirectory) return;

			foreach (MediaItem item in StackPanelExplorer.Children.OfType<MediaItem>())
			{
				if (tracks.Exists(track => track.FullName == item.FullName)) // if we're in the playlist items
				{
					item.SetMediaIcon(toPlaylist); // set the icon
					item.SetTitleLabelForeground(toPlaylist);
				}
			}
		}
		// sets the title label text color
		public void SetMediaItemForeground(List<MediaFile> tracks)
		{
			if (CurrentDirectory != null && CurrentDirectory.FullName != Playlist.PlaylistDirectory) return;

			foreach (MediaItem item in StackPanelExplorer.Children.OfType<MediaItem>())
			{
				if (tracks.Exists(track => track.FullName == item.FullName)) item.SetTitleLabelForeground(true);
				else item.SetTitleLabelForeground(false);
			}
		}

		private void SelectMediaItemByIndex(int index)
		{
			MediaItem mediaItem = GetMediaItemByPlaylistIndex(index);
			MediaItem.Select(mediaItem);
		}

		// returns appropriate margin (left/right) for the media explorer stackPanel animation
		private static Thickness GetExplorerAnimationMargin(DirectoryInfo fromDirectory, DirectoryInfo currentDirectory)
		{
			if (currentDirectory == null) return Constant.ExplorerMargin.RightPage;
			else if (fromDirectory == null) return Constant.ExplorerMargin.LeftPage;
			else if (fromDirectory.FullName.Length <= currentDirectory.FullName.Length) return Constant.ExplorerMargin.LeftPage;
			else return Constant.ExplorerMargin.RightPage;
		}
		private static double GetExplorerAnimationScale(DirectoryInfo fromDirectory, DirectoryInfo currentDirectory)
		{
			if (currentDirectory == null) return Constant.DrillScale.In;
			else if (fromDirectory == null) return Constant.DrillScale.Out;
			else if (fromDirectory.FullName.Length <= currentDirectory.FullName.Length) return Constant.DrillScale.Out;
			else return Constant.DrillScale.In;
		}

		private void FilterMediaExplorer(string filter)
		{
			var explorer = ExplorerCache[CurrentDirectory.FullName];
			var items = (explorer.Content as StackPanel).Children.OfType<MediaItem>();

			if (filter.Length == 0)
			{
				items.ToList().ForEach(i => i.Toggle(true));
				return;
			}

			items.ToList().ForEach(i => i.Toggle(i.File.Name.FuzzyMatch(filter)));
		}
	}
}