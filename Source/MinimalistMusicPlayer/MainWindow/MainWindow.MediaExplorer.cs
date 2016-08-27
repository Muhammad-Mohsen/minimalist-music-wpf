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

			// this is a little sad
			if (MediaFiles.Length == Player.PlaylistFullNames.Count)
				SetMediaItemForeground(Player.PlaylistFullNames, true);
		}
		
		public void AddMediaItem(FileInfo mediaFile, int index)
		{
			IWMPMedia media = Player.Player.newMedia(mediaFile.FullName);

			bool isPlaylistMediaItem = IsPlaylistMedia(mediaFile.FullName);
			bool isSelected = isPlaylistMediaItem && Player.Index == GetMediaItemPlaylistIndex(mediaFile.FullName);

			// ignore the playlist media items if the entire directory's media files are in the playlist
			if (MediaFiles.Length == Player.PlaylistFullNames.Count)
				isPlaylistMediaItem = false;

			MediaItem mediaItem = new MediaItem(mediaFile, media.durationString, isSelected, isPlaylistMediaItem);
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
				// reset the icons
				SetPlaylistMediaItemStyle(Player.PlaylistFullNames, false);
				SetMediaItemForeground(Player.PlaylistFullNames, true);

				Player.ClearPlaylistItems();
				Player.AddPlaylistItems(MediaFiles.Select(f => f.FullName));

				Player.Index = GetMediaItemPlaylistIndex(item.FullName); // update index
				Player.Play(Player.Index); // start playing the item
			}
		}
		
		// controls whether the Play selected button should be shown
		private void MediaItem_MarkedItemCountChange(object sender, RoutedEventArgs e)
		{
			bool shouldShowSelectMode = MediaItem.MarkedItemCount > 0;
			TogglePlaylistSelectMode(shouldShowSelectMode);
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
		
		public bool IsPlaylistMedia(string fullName)
		{
			if (Player.PlaylistDirectory == CurrentDirectory.FullName)
			{
				foreach (string itemFullName in Player.PlaylistFullNames)
					if (itemFullName == fullName)
						return true;
			}

			return false;
		}

		// sets the playlist icon for selected media files
		public void SetPlaylistMediaItemStyle(IEnumerable<string> playlistItemFullNames, bool toPlaylist)
		{
			foreach (MediaItem item in StackPanelExplorer.Children.OfType<MediaItem>())
			{
				if (playlistItemFullNames.Contains(item.FullName)) // if we're in the playlist items
				{
					item.SetMediaIcon(toPlaylist); // set the icon
					item.SetTitleLabelForeground(toPlaylist);
				}
			}
		}
		public void SetMediaItemForeground(IEnumerable<string> playlistItemFullNames, bool toPlaylist)
		{
			foreach (MediaItem item in StackPanelExplorer.Children.OfType<MediaItem>())
				item.SetTitleLabelForeground(toPlaylist);
		}

		private void SelectMediaItemByIndex(int index)
		{
			MediaItem mediaItem = GetMediaItem(index);
			MediaItem.Select(mediaItem);
		}
	}
}