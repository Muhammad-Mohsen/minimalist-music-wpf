using MinimalistMusicPlayer.Explorer;
using MinimalistMusicPlayer.Player;
using MinimalistMusicPlayer.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
		public DirectoryInfo CurrentDirectory { get; set; }
		public DirectoryInfo[] SubDirectories { get; set; }
		public FileInfo[] MediaFiles { get; set; }

		public StackPanel StackPanelExplorer;
		public ScrollViewer ScrollViewerExplorer;

		public void InitializeMediaExplorer(DirectoryInfo directory)
		{
			StackPanelExplorer.Children.Clear();

			if (directory == null)
			{
				StackPanelExplorer.Children.Clear();
				foreach (DriveInfo drive in DriveInfo.GetDrives())
					if (drive.IsReady)
						AddDriveItem(drive.RootDirectory.FullName);
			}
			else
			{
				// ignore hidden folders
				SubDirectories = directory.GetDirectories().Where(x => (x.Attributes & FileAttributes.Hidden) == 0).ToArray();
				foreach (DirectoryInfo subDirectory in SubDirectories)
					AddDirectoryItem(subDirectory.FullName);

				MediaFiles = directory.GetMediaFiles();
				for (int i = 0; i < MediaFiles.Count(); i++)
					AddMediaItem(MediaFiles[i], i);
			}
		}

		public void AddMediaItem(FileInfo mediaFile, int index)
		{
			IWMPMedia media = Player.Player.newMedia(mediaFile.FullName);

			MediaItemStyle mediaItemStyle = GetMediaItemStyle(mediaFile.FullName);

			bool isSelected = false;
			if (CurrentDirectory != null && CurrentDirectory.FullName == Player.PlaylistDirectory)
				isSelected = Player.Index == GetMediaItemPlaylistIndex(mediaFile.FullName);

			MediaItem mediaItem = new MediaItem(mediaFile, media.durationString, mediaItemStyle, isSelected);
			mediaItem.MouseDoubleClick += MediaItem_MouseDoubleClick;
			mediaItem.MarkedItemCountChange += MediaItem_MarkedItemCountChange;

			StackPanelExplorer.Children.Add(mediaItem);
		}
		private void MediaItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			MediaItem item = (MediaItem)sender;

			MediaItem.Select(item); // set selection styling, deselect the old item while you're at it
			
			Player.Index = GetMediaItemPlaylistIndex(item.FullName);
			// if item is already in the playlist, simply play the item
			if (Player.Index != Const.InvalidIndex)
				Player.Play(Player.Index);

			// else, repopulate the playlist with all the files in the current directory then play the item
			else
			{
				Player.ClearPlaylistItems();
				Player.AddPlaylistItems(MediaFiles.Select(f => f.FullName));

				Player.Index = GetMediaItemPlaylistIndex(item.FullName); // update index
				Player.Play(Player.Index); // start playing the item

				// reset the icons
				SetPlaylistMediaItemStyle(Player.PlaylistFullNames, false);
				SetMediaItemForeground(Player.PlaylistFullNames);
			}
		}
		
		// controls whether the Play selected button should be shown
		private void MediaItem_MarkedItemCountChange(object sender, RoutedEventArgs e)
		{
			bool shouldShowSelectMode = MediaItem.MarkedItemCount > 0;
			TogglePlaylistSelectMode(shouldShowSelectMode);

			// only enable AddToSelection button if we're in the same directory as the playlist, and the playlist is not empty
			if (shouldShowSelectMode)
				SetAddToSelectionEnableState(CurrentDirectory.FullName, Player.PlaylistDirectory, Player.Count);
		}

		// pretty much a duplicate of DirectoryItem code, but I'm alright with that
		public void AddDriveItem(string root)
		{
			DriveItem driveItem = new DriveItem(root);
			driveItem.MouseDoubleClick += DriveItem_MouseDoubleClick;

			StackPanelExplorer.Children.Add(driveItem);
		}
		private void DriveItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			DriveItem driveItem = (DriveItem)sender;
			DirectoryChange(new DirectoryInfo(driveItem.Directory));
		}

		public void AddDirectoryItem(string directory)
		{
			DirectoryItem directoryItem = new DirectoryItem(directory);
			directoryItem.MouseDoubleClick += DirectoryItem_MouseDoubleClick;

			StackPanelExplorer.Children.Add(directoryItem);
		}
		private void DirectoryItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			DirectoryItem directoryItem = (DirectoryItem)sender;
			DirectoryChange(new DirectoryInfo(directoryItem.Directory));
		}

		// maps a given playlist index to an actual MediaItem object
		// mapping isn't 1:1 because there are directories and DirectoryItems thrown in the mix!
		public MediaItem GetMediaItem(int playlistIndex)
		{
			string mediaItemFullName = Player.PlaylistFullNames[playlistIndex]; // get the playlist media item

			// skip over the directory items
			foreach (MediaItem item in StackPanelExplorer.Children.OfType<MediaItem>())
			{
				if (item.FullName == mediaItemFullName)
					return item;
			}
			return null;
		}

		public int GetMediaItemPlaylistIndex(string fullName)
		{
			if (Player.PlaylistDirectory == CurrentDirectory.FullName)
			{
				int index;
				for (index = 0; index < Player.PlaylistFullNames.Count; index++)
				{
					if (Player.PlaylistFullNames[index] == fullName)
						return index;
				}
			}
			// return -1 if the MediaItem is not in the current playlist
			return Const.InvalidIndex;
		}

		// gets a list of marked MediaItems' FullNames
		public List<string> GetMarkedMediaFiles()
		{
			List<string> markedFiles = new List<string>();

			foreach (MediaItem item in StackPanelExplorer.Children.OfType<MediaItem>())
			{
				if (item.IsMarked)
					markedFiles.Add(item.FullName);
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
			if (CurrentDirectory != null && CurrentDirectory.FullName == Player.PlaylistDirectory)
			{
				if (MediaFiles.Length == Player.PlaylistFullNames.Count)
					return MediaItemStyle.Highlighted;

				else if (Player.PlaylistFullNames.Contains(fullName))
					return MediaItemStyle.IconHighlighted;
			}

			return MediaItemStyle.Normal;
		}

		// sets the playlist icon for selected media files
		public void SetPlaylistMediaItemStyle(IEnumerable<string> playlistItemFullNames, bool toPlaylist)
		{
			if (CurrentDirectory != null && CurrentDirectory.FullName != Player.PlaylistDirectory)
				return;
			
			foreach (MediaItem item in StackPanelExplorer.Children.OfType<MediaItem>())
			{
				if (playlistItemFullNames.Contains(item.FullName)) // if we're in the playlist items
				{
					item.SetMediaIcon(toPlaylist); // set the icon
					item.SetTitleLabelForeground(toPlaylist);
				}
			}
		}
		public void SetMediaItemForeground(IEnumerable<string> playlistItemFullNames)
		{
			if (CurrentDirectory != null && CurrentDirectory.FullName != Player.PlaylistDirectory)
				return;

			foreach (MediaItem item in StackPanelExplorer.Children.OfType<MediaItem>())
			{
				if (playlistItemFullNames.Contains(item.FullName))
					item.SetTitleLabelForeground(true);

				else
					item.SetTitleLabelForeground(false);
			}
		}

		private void SelectMediaItemByIndex(int index)
		{
			MediaItem mediaItem = GetMediaItem(index);
			MediaItem.Select(mediaItem);
		}

		private MediaItem GetCurrentMediaItem()
		{
			if (CurrentDirectory != null && Player.PlaylistDirectory == CurrentDirectory.FullName)
			{
				string currentMediaFullName = Player.CurrentMedia.sourceURL;

				foreach (MediaItem item in StackPanelExplorer.Children.OfType<MediaItem>())
				{
					if (currentMediaFullName.Equals(item.FullName))
						return item;
				}
			}

			return null;
		}

		private ScrollViewer GetPagedScrollViewerExplorer()
		{
			return ScrollViewerExplorer == ScrollViewerExplorerPrimary ? ScrollViewerExplorerSecondary : ScrollViewerExplorerPrimary;
		}
		private StackPanel GetPagedStackPanelExplorer()
		{
			return StackPanelExplorer == StackPanelExplorerPrimary ? StackPanelExplorerSecondary : StackPanelExplorerPrimary;
		}
	}
}