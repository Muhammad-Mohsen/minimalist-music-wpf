using MinimalistMusicPlayer.Explorer;
using MinimalistMusicPlayer.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WMPLib;

namespace MinimalistMusicPlayer
{
	// Contains ExplorerItem cache of previously-visited directories
	public partial class MainWindow : Window
	{
		public static Dictionary<string, List<DirectoryItem>> SubDirectoryCache = new Dictionary<string, List<DirectoryItem>>();
		public static Dictionary<string, List<MediaItem>> MediaFileCache = new Dictionary<string, List<MediaItem>>();
		//
		// API
		//
		public List<DirectoryItem> GetSubDirectoryItems(DirectoryInfo directory)
		{
			if (directory == null)
				return null;

			// if the directory is already cached, simply return the cached info
			if (SubDirectoryCache.ContainsKey(directory.FullName))
				return SubDirectoryCache[directory.FullName];

			else
			{
				// ignore hidden folders
				var subDirectories = directory.GetDirectories().Where(x => (x.Attributes & FileAttributes.Hidden) == 0).ToArray();

				List<DirectoryItem> directoryItems = new List<DirectoryItem>();
				foreach (DirectoryInfo subDirectory in subDirectories)
					directoryItems.Add(CreateDirectoryItem(subDirectory.FullName));

				// add to the cache
				SubDirectoryCache.Add(directory.FullName, directoryItems);

				return directoryItems;
			}
		}
		public List<MediaItem> GetMediaItems(DirectoryInfo directory)
		{
			if (directory == null)
				return null;

			if (MediaFileCache.ContainsKey(directory.FullName))
			{
				List<MediaItem> cachedMediaItems = MediaFileCache[directory.FullName];

				// if the directory is not the same as the playlist directory, make sure to reset the selection states
				if (directory.FullName != Player.PlaylistDirectory)
					cachedMediaItems.ForEach(i => { i.SetMediaIcon(false); i.SetTitleLabelForeground(false); });

				// else if at the playlist directory, select the currently playing item
				else
					MediaItem.Select(GetCachedMediaItemByPlaylistIndex(Player.Index, cachedMediaItems));

				return cachedMediaItems;
			}

			else
			{
				var mediaFiles = directory.GetMediaFiles();

				List<MediaItem> mediaItems = new List<MediaItem>();
				for (int i = 0; i < mediaFiles.Count(); i++)
					mediaItems.Add(CreateMediaItem(mediaFiles[i], i));

				MediaFileCache.Add(directory.FullName, mediaItems);

				return mediaItems;
			}
		}

		// DirectoryItems
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

		// MediaItems
		public MediaItem CreateMediaItem(FileInfo mediaFile, int index)
		{
			IWMPMedia media = Player.Player.newMedia(mediaFile.FullName);

			MediaItemStyle mediaItemStyle = GetMediaItemStyle(mediaFile.FullName);

			bool isSelected = false;
			if (CurrentDirectory != null && CurrentDirectory.FullName == Player.PlaylistDirectory)
				isSelected = Player.Index == GetMediaItemPlaylistIndex(mediaFile.FullName);

			MediaItem mediaItem = new MediaItem(mediaFile, media.durationString, mediaItemStyle, isSelected);
			mediaItem.MouseDoubleClick += MediaItem_MouseDoubleClick;
			mediaItem.MarkedItemCountChange += MediaItem_MarkedItemCountChange;

			return mediaItem;
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
				Player.AddPlaylistItems(DirectoryMediaFiles.Select(f => f.FullName));

				Player.Index = GetMediaItemPlaylistIndex(item.FullName); // update index
				Player.Play(Player.Index); // start playing the item

				// reset the icons for this particular item
				SetPlaylistMediaItemStyle(Player.PlaylistFullNames, false);
				SetMediaItemForeground(Player.PlaylistFullNames);

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
				SetAddToSelectionEnableState(CurrentDirectory.FullName, Player.PlaylistDirectory, Player.Count);
		}

		// currently Drive Items aren't cached!
		// pretty much a duplicate of DirectoryItem code, but I'm alright with that
		public void AddDriveItem(string root)
		{
			DriveItem driveItem = new DriveItem(root);
			driveItem.MouseDoubleClick += DriveItem_MouseDoubleClick;

			StackPanelExplorer.Children.Add(driveItem);
		}
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

		private MediaItem GetCachedMediaItemByPlaylistIndex(int playlistIndex, List<MediaItem> cachedMediaItems)
		{
			string mediaItemFullName = Player.PlaylistFullNames[playlistIndex]; // get the playlist media item
			return cachedMediaItems.Where(s => s.FullName == mediaItemFullName).First();
		}
	}
}
