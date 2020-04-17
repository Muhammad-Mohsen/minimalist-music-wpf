using MinimalistMusicPlayer.Explorer;
using MinimalistMusicPlayer.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WMPLib;

namespace MinimalistMusicPlayer
{
	// the explorer piece of MainWindow
	public partial class MainWindow : Window
	{
		public FileInfo[] DirectoryMediaFiles { get; set; }

		public StackPanel StackPanelExplorer;
		public ScrollViewer ScrollViewerExplorer;

		public async void PopulateMediaExplorer(StackPanel explorer, DirectoryInfo directory)
		{
			AddExplorerItemsAsync(explorer, directory);
			await Task.Delay(TimeSpan.FromTicks(0)); // just to make the method async!
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
				await Task.Delay(TimeSpan.FromMilliseconds(Const.AsyncDelay));
			}

			for (int i = 0; i < DirectoryMediaFiles.Length; i++)
			{
				AddExplorerItem(panel, DirectoryMediaFiles[i].FullName, i);
				await Task.Delay(TimeSpan.FromMilliseconds(Const.AsyncDelay));
			}
		}

		public async void AddExplorerItem(StackPanel panel, string itemPath, int i)
		{
			ExplorerItem item;
			if (i == -1) item = CreateDirectoryItem(itemPath);
			else item = CreateMediaItem(new FileInfo(itemPath));

			item.Opacity = 0;
			panel.Children.Add(item);

			Anim.ShowHideFrameworkElement(item, true, Const.ShowHideDelay);
			await Task.Delay(TimeSpan.FromSeconds(Const.ShowHideDelay));
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
		public MediaItem CreateMediaItem(FileInfo mediaFile)
		{
			IWMPMedia media = Player.InternalPlayer.newMedia(mediaFile.FullName);

			MediaItemStyle mediaItemStyle = GetMediaItemStyle(mediaFile.FullName);

			bool isSelected = false;
			if (CurrentDirectory != null && CurrentDirectory.FullName == Playlist.PlaylistDirectory)
				isSelected = Playlist.Index == GetMediaItemPlaylistIndex(mediaFile.FullName);

			MediaItem mediaItem = new MediaItem(mediaFile, media.durationString, mediaItemStyle, isSelected);
			mediaItem.MouseDoubleClick += MediaItem_MouseDoubleClick;
			mediaItem.MarkedItemCountChange += MediaItem_MarkedItemCountChange;

			return mediaItem;
		}
		private void MediaItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			MediaItem item = (MediaItem)sender;

			MediaItem.Select(item); // set selection styling, deselect the old item while you're at it

			Playlist.Index = GetMediaItemPlaylistIndex(item.FullName);
			// if item is already in the playlist, simply play the item
			if (Playlist.Index != Const.InvalidIndex) Player.Play(Playlist.GetItem(Playlist.Index));

			// else, repopulate the playlist with all the files in the current directory then play the item
			else
			{
				Playlist.ClearPlaylistItems();
				Playlist.AddPlaylistItems(DirectoryMediaFiles.Select(f => f.FullName));

				Playlist.Index = GetMediaItemPlaylistIndex(item.FullName); // update index
				Player.Play(Playlist.GetItem(Playlist.Index)); // start playing the item

				// reset the icons for this particular item
				SetPlaylistMediaItemStyle(Playlist.PlaylistFullNames, false);
				SetMediaItemForeground(Playlist.PlaylistFullNames);

				// reset marking states for all items
				ResetMediaItemMarkState();
				MediaItem.MarkedItemCount = 0;

				// hide the select mode if applicable
				SetPlaylistSelectMode(false);
			}
		}
		// controls whether the Play selected button should be shown
		private void MediaItem_MarkedItemCountChange(object sender, RoutedEventArgs e)
		{
			bool shouldShowSelectMode = MediaItem.MarkedItemCount > 0;
			SetPlaylistSelectMode(shouldShowSelectMode);

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
			string mediaItemFullName = Playlist.PlaylistFullNames[playlistIndex]; // get the playlist media item

			// skip over the directory items
			foreach (MediaItem item in StackPanelExplorer.Children.OfType<MediaItem>())
			{
				if (item.FullName == mediaItemFullName) return item;
			}
			return null;
		}

		public int GetMediaItemPlaylistIndex(string fullName)
		{
			if (Playlist.PlaylistDirectory == CurrentDirectory.FullName)
			{
				int index;
				for (index = 0; index < Playlist.PlaylistFullNames.Count; index++)
				{
					if (Playlist.PlaylistFullNames[index] == fullName) return index;
				}
			}
			// return -1 if the MediaItem is not in the current playlist
			return Const.InvalidIndex;
		}

		// gets a list of marked MediaItems' FullNames
		public List<string> GetMarkedMediaFileFullNames()
		{
			List<string> markedFiles = new List<string>();

			foreach (MediaItem item in StackPanelExplorer.Children.OfType<MediaItem>())
			{
				if (item.IsMarked) markedFiles.Add(item.FullName);
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
				item.MarkMediaIcon(item.MediaIcon, false);
			}
		}

		// returns whether the given media is contained in the currently playing playlist
		public MediaItemStyle GetMediaItemStyle(string fullName)
		{
			if (CurrentDirectory != null && CurrentDirectory.FullName == Playlist.PlaylistDirectory)
			{
				if (DirectoryMediaFiles.Length == Playlist.PlaylistFullNames.Count)
					return MediaItemStyle.Highlighted;

				else if (Playlist.PlaylistFullNames.Contains(fullName))
					return MediaItemStyle.IconHighlighted;
			}

			return MediaItemStyle.Normal;
		}

		// sets the playlist icon for selected media files
		public void SetPlaylistMediaItemStyle(IEnumerable<string> playlistItemFullNames, bool toPlaylist)
		{
			if (CurrentDirectory != null && CurrentDirectory.FullName != Playlist.PlaylistDirectory) return;

			foreach (MediaItem item in StackPanelExplorer.Children.OfType<MediaItem>())
			{
				if (playlistItemFullNames.Contains(item.FullName)) // if we're in the playlist items
				{
					item.SetMediaIcon(toPlaylist); // set the icon
					item.SetTitleLabelForeground(toPlaylist);
				}
			}
		}
		// sets the title label text color
		public void SetMediaItemForeground(IEnumerable<string> playlistItemFullNames)
		{
			if (CurrentDirectory != null && CurrentDirectory.FullName != Playlist.PlaylistDirectory) return;

			foreach (MediaItem item in StackPanelExplorer.Children.OfType<MediaItem>())
			{
				if (playlistItemFullNames.Contains(item.FullName)) item.SetTitleLabelForeground(true);
				else item.SetTitleLabelForeground(false);
			}
		}

		private void SelectMediaItemByIndex(int index)
		{
			MediaItem mediaItem = GetMediaItemByPlaylistIndex(index);
			MediaItem.Select(mediaItem);
		}

		// returns appropriate margin (left/right) for the media explorer stackPanel animation
		private Thickness GetExplorerAnimationMargin(DirectoryInfo fromDirectory, DirectoryInfo currentDirectory)
		{
			if (currentDirectory == null) return Const.ExplorerMargin.RightPage;
			else if (fromDirectory == null) return Const.ExplorerMargin.LeftPage;
			else if (fromDirectory.FullName.Length <= currentDirectory.FullName.Length) return Const.ExplorerMargin.LeftPage;
			else return Const.ExplorerMargin.RightPage;
		}
	}
}