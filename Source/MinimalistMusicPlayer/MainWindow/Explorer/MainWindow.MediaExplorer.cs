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

namespace MinimalistMusicPlayer
{
	// the explorer piece of MainWindow
	public partial class MainWindow : Window
	{
		public FileInfo[] DirectoryMediaFiles { get; set; }

		public StackPanel StackPanelExplorer;
		public ScrollViewer ScrollViewerExplorer;

		public async void PopulateMediaExplorer(DirectoryInfo directory)
		{
			StackPanelExplorer.Children.Clear();

			ScrollViewerExplorer.ScrollToHome();

			// if at the root of the HDD
			if (directory == null)
				DriveInfo.GetDrives().Where(drive => drive.IsReady).ToList().ForEach(drive => AddDriveItem(drive.RootDirectory.FullName)); // added synchronously!

			else
			{
				AddExplorerItemsAsync(StackPanelExplorer, directory);
                await Task.Delay(TimeSpan.FromTicks(0)); // just to make the method async!
			}
		}

		// adds explorer items that are not Drives
		private async void AddExplorerItemsAsync(StackPanel panel, DirectoryInfo newDirectory)
		{
			var subDirectories = newDirectory.GetDirectories().Where(x => (x.Attributes & FileAttributes.Hidden) == 0).ToArray();
			DirectoryMediaFiles = newDirectory.GetMediaFiles();

			foreach (DirectoryInfo dir in subDirectories)
			{
				AddExplorerItem(StackPanelExplorer, dir.FullName, -1);
				await Task.Delay(TimeSpan.FromMilliseconds(Const.AsyncDelay));
			}

			for (int i = 0; i < DirectoryMediaFiles.Length; i++)
			{
				AddExplorerItem(StackPanelExplorer, DirectoryMediaFiles[i].FullName, i);
				await Task.Delay(TimeSpan.FromMilliseconds(Const.AsyncDelay));
			}
		}
		public async void AddExplorerItem(StackPanel panel, string itemPath, int i)
		{
			ExplorerItem item;
			if (i == -1)
				item = CreateDirectoryItem(itemPath);
			else
				item = CreateMediaItem(new FileInfo(itemPath), i);

			item.Opacity = 0;
			panel.Children.Add(item);

			Util.ShowHideFrameworkElement(item, true, Const.ShowHideDelay);
			await Task.Delay(TimeSpan.FromSeconds(Const.ShowHideDelay));
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