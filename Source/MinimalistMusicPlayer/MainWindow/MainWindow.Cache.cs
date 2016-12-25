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
	// the cache piece of MainWindow
	public partial class MainWindow : Window
	{
		public static Dictionary<string, List<DirectoryItem>> SubDirectoryCache = new Dictionary<string, List<DirectoryItem>>();
		public static Dictionary<string, List<MediaItem>> MediaFileCache = new Dictionary<string, List<MediaItem>>();

		public List<DirectoryItem> GetSubDirectoryItems(DirectoryInfo directory)
		{
			// if the directory is already cached, simply return the cache
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
			if (MediaFileCache.ContainsKey(directory.FullName))
				return MediaFileCache[directory.FullName];

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
	}
}
