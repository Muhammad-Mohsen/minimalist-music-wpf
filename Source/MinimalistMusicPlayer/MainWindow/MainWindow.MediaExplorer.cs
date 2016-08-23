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

			MediaItem.SelectMediaItem(item); // set selection styling, deselect the old item while you're at it
			
			Player.Index = GetMediaItemPlaylistIndex(item.FullName);
			// if item is already in the playlist, simply play the itme
			if (Player.Index != Const.InvalidIndex)
				Player.Play(Player.Index);

			// else, repopulate the playlist with all the files in the current directory then play the item
			else
			{
				// reset the icons
				ResetPlaylistMediaItemIcons(Player.PlaylistFullNames);

				Player.ClearPlaylistItems();
				Player.AddPlaylistItems(MediaFiles.Select(f => f.FullName));

				Player.Index = GetMediaItemPlaylistIndex(item.FullName); // update index
				Player.Play(Player.Index); // start playing the item
			}
		}
		// controls whether the Play selected button should be shown
		private void MediaItem_MarkedItemCountChange(object sender, RoutedEventArgs e)
		{
			ShowHidePlaySelectedButton(MediaItem.MarkedItemCount > 0);
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

			CurrentDirectory = new DirectoryInfo(driveItem.Directory);
			DirectoryChange(CurrentDirectory);
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

			CurrentDirectory = new DirectoryInfo(directoryItem.Directory);
			DirectoryChange(CurrentDirectory);
		}

		// maps a given playlist index to an actual MediaItem object
		// mapping isn't 1:1 because there are directories and DirectoryItems thrown in the mix!
		public MediaItem MapIndexToMediaItem(int index)
		{
			string mediaItemFullName = Player.PlaylistFullNames[index]; // get the playlist media item

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
				item.IsMarked = false;
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
		public void SetPlaylistMediaItemIcons(IEnumerable<string> playlistItemFullNames)
		{
			foreach (MediaItem item in StackPanelExplorer.Children.OfType<MediaItem>())
			{
				if (playlistItemFullNames.Contains(item.FullName)) // if we're in the playlist items, 
					item.SetMediaIcon(true); // set the icon
			}
		}

		// resets the playlist icons for all media
		// should be called when changing playlists
		public void ResetPlaylistMediaItemIcons(List<string> playlistItemFullNames)
		{
			for (int i = 0; i < playlistItemFullNames.Count; i++)
			{
				MediaItem mediaItem = MapIndexToMediaItem(i); // method will return null if it doesn't find a match (possibly due to directory change)
				if (mediaItem != null && mediaItem.FullName == playlistItemFullNames[i])
					mediaItem.SetMediaIcon(false);
			}
		}

		// will be called on directory change (up button click, breadcrumb button click, directory item double click)
		public void DirectoryChange(DirectoryInfo directory)
		{
			// set the setting (will be saved in OnExit event in the app class!!)
			Properties.Settings.Default[Const.ExplorerDirectorySetting] = directory != null ? directory.FullName : null;

			PopulateBreadcrumbBar(CurrentDirectory);
			InitializeMediaExplorer(CurrentDirectory);

			// reset item markings
			MediaItem.MarkedItemCount = 0;
			ShowHidePlaySelectedButton(false);
		}
	}
}