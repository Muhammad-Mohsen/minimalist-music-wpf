using MinimalistMusicPlayer.Explorer;
using MinimalistMusicPlayer.Explorer.Cache;
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

			if (directory == null) // if at the root of the HDD
			{
				StackPanelExplorer.Children.Clear();
				foreach (DriveInfo drive in DriveInfo.GetDrives())
					if (drive.IsReady)
						AddDriveItem(drive.RootDirectory.FullName);
			}
			else
			{
				List<DirectoryItem> directoryItems = GetSubDirectoryItems(directory);
				foreach (DirectoryItem directoryItem in directoryItems)
					StackPanelExplorer.Children.Add(directoryItem);

				MediaFiles = directory.GetMediaFiles();

				var mediaItems = GetMediaItems(directory);
				foreach (MediaItem mediaItem in mediaItems)
					StackPanelExplorer.Children.Add(mediaItem);
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