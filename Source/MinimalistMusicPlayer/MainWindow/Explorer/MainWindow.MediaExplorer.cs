using MinimalistMusicPlayer.Explorer;
using MinimalistMusicPlayer.Utility;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MinimalistMusicPlayer
{
	// the explorer piece of MainWindow
	public partial class MainWindow : Window
	{
		public FileInfo[] DirectoryMediaFiles { get; set; }

		public StackPanel StackPanelExplorer;
		public ScrollViewer ScrollViewerExplorer;

		public void PopulateMediaExplorer(DirectoryInfo directory)
		{
			StackPanelExplorer.Children.Clear();

			if (directory == null) // if at the root of the HDD
				DriveInfo.GetDrives().Where(drive => drive.IsReady).ToList().ForEach(drive => AddDriveItem(drive.RootDirectory.FullName));
			
			else
			{
				List<DirectoryItem> directoryItems = GetSubDirectoryItems(directory);
				directoryItems.ForEach(item => StackPanelExplorer.Children.Add(item));
				
				DirectoryMediaFiles = directory.GetMediaFiles();

				List<MediaItem> mediaItems = GetMediaItems(directory);
				mediaItems.ForEach(item => StackPanelExplorer.Children.Add(item));
			}
		}
		
		// maps a given playlist index to an actual MediaItem object
		// mapping isn't 1:1 because there are directories and DirectoryItems thrown in the mix!
		public MediaItem GetMediaItemByPlaylistIndex(int playlistIndex)
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
		public List<string> GetMarkedMediaFileFullNames()
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
				if (DirectoryMediaFiles.Length == Player.PlaylistFullNames.Count)
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
		// sets the title label text color
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
			MediaItem mediaItem = GetMediaItemByPlaylistIndex(index);
			MediaItem.Select(mediaItem);
		}
		//
		// Directory change animation helpers
		//
		private ScrollViewer GetPagedScrollViewerExplorer()
		{
			return ScrollViewerExplorer == ScrollViewerExplorerPrimary ? ScrollViewerExplorerSecondary : ScrollViewerExplorerPrimary;
		}
		private StackPanel GetPagedStackPanelExplorer()
		{
			return StackPanelExplorer == StackPanelExplorerPrimary ? StackPanelExplorerSecondary : StackPanelExplorerPrimary;
		}
		// returns appropriate margin (left/right) for the media explorer stackPanel animation 
		private Thickness GetExplorerAnimationMargin(DirectoryInfo fromDirectory, DirectoryInfo currentDirectory)
		{
			if (currentDirectory == null)
				return Const.ExplorerMargin.RightPage;
			else if (fromDirectory == null)
				return Const.ExplorerMargin.LeftPage;

			else if (fromDirectory.FullName.Length <= currentDirectory.FullName.Length)
				return Const.ExplorerMargin.LeftPage;
			else
				return Const.ExplorerMargin.RightPage;
		}
	}
}