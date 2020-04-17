using MinimalistMusicPlayer.Explorer;
using MinimalistMusicPlayer.Utility;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MinimalistMusicPlayer
{
	// Contains ExplorerItem cache of previously-visited directories
	public partial class MainWindow : Window
	{
		public static Dictionary<string, List<DirectoryItem>> SubDirectoryCache = new Dictionary<string, List<DirectoryItem>>(); // NOT USED
		public static Dictionary<string, List<MediaItem>> MediaFileCache = new Dictionary<string, List<MediaItem>>(); // NOT USED

		public static Dictionary<string, ScrollViewer> ExplorerCache = new Dictionary<string, ScrollViewer>();
		//
		// API
		//
		public ScrollViewer GetExplorer(DirectoryInfo directory)
		{
			var key = directory == null ? Const.Root : directory.FullName; // directory is null at the root
			if (ExplorerCache.ContainsKey(key)) return ExplorerCache[key]; // try the cache first

			var container = CreateExplorerContainer(); // otherwise, create the explorer 'window'
			ExplorerCache.Add(key, container); // add it to the cache
			PopulateMediaExplorer(container.Content as StackPanel, directory); // populate it

			GridExplorerMain.Children.Add(container); // don't forget to add it to the UI dummy!!

			return container; // hand it over to whoever is interested
		}

		private ScrollViewer CreateExplorerContainer()
		{
			return new ScrollViewer()
			{
				IsTabStop = false,
				Focusable = false,
				Margin = Const.ExplorerMargin.CurrentPage,
				Content = new StackPanel()
				{
					Focusable = false,
				}
			};
		}
		//
		// API - NOT USED
		//
		public List<DirectoryItem> GetSubDirectoryItems(DirectoryInfo directory)
		{
			if (directory == null) return null;

			// if the directory is already cached, simply return the cached info
			if (SubDirectoryCache.ContainsKey(directory.FullName)) return SubDirectoryCache[directory.FullName];

			// ignore hidden folders
			var subDirectories = directory.GetDirectories().Where(x => (x.Attributes & FileAttributes.Hidden) == 0).ToArray();

			List<DirectoryItem> directoryItems = new List<DirectoryItem>();
			foreach (DirectoryInfo subDirectory in subDirectories)
				directoryItems.Add(CreateDirectoryItem(subDirectory.FullName));

			// add to the cache
			SubDirectoryCache.Add(directory.FullName, directoryItems);

			return directoryItems;
		}
		public List<MediaItem> GetMediaItems(DirectoryInfo directory)
		{
			if (directory == null) return null;

			if (MediaFileCache.ContainsKey(directory.FullName))
			{
				List<MediaItem> cachedMediaItems = MediaFileCache[directory.FullName];

				// if the directory is not the same as the playlist directory, make sure to reset the selection states
				if (directory.FullName != Playlist.PlaylistDirectory) cachedMediaItems.ForEach(i => { i.SetMediaIcon(false); i.SetTitleLabelForeground(false); });

				// else if at the playlist directory, select the currently playing item
				else MediaItem.Select(GetCachedMediaItemByPlaylistIndex(Playlist.Index, cachedMediaItems));

				return cachedMediaItems;
			}

			else
			{
				var mediaFiles = directory.GetMediaFiles();

				List<MediaItem> mediaItems = new List<MediaItem>();
				for (int i = 0; i < mediaFiles.Count(); i++)
					mediaItems.Add(CreateMediaItem(mediaFiles[i]));

				MediaFileCache.Add(directory.FullName, mediaItems);

				return mediaItems;
			}
		}
		private MediaItem GetCachedMediaItemByPlaylistIndex(int playlistIndex, List<MediaItem> cachedMediaItems)
		{
			string mediaItemFullName = Playlist.PlaylistFullNames[playlistIndex]; // get the playlist media item
			return cachedMediaItems.Where(s => s.FullName == mediaItemFullName).First();
		}
	}
}
