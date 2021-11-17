using MinimalistMusicPlayer.Explorer;
using MinimalistMusicPlayer.Utility;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace MinimalistMusicPlayer
{
	// Contains ExplorerItem cache of previously-visited directories
	public partial class MainWindow
	{
		private static readonly Dictionary<string, ScrollViewer> ExplorerCache = new Dictionary<string, ScrollViewer>();
		//
		// API
		//
		public ScrollViewer GetExplorer(DirectoryInfo directory)
		{
			var key = directory == null ? Constant.Root : directory.FullName; // directory is null at the root
			if (ExplorerCache.ContainsKey(key)) // try the cache first
			{
				var explorer = ExplorerCache[key];
				var list = explorer.Content as StackPanel;

				DirectoryMediaFiles = directory.GetMediaFiles();

				// reset the state of all items if not in the playlist directory
				foreach (MediaItem item in list.Children.OfType<MediaItem>())
				{
					if (CurrentDirectory != null && CurrentDirectory.FullName != Playlist.PlaylistDirectory)
					{
						item.SetTitleLabelForeground(false);
						item.SetMediaIcon(false);
					}
					else if (Playlist.CurrentIndex == Playlist.IndexOf(item.FullName, CurrentDirectory)) // if @ the playlist directory
					{
						item?.Select();
					}
				}

				return explorer;
			}
			var container = CreateExplorerContainer(); // otherwise, create the explorer 'window'
			ExplorerCache.Add(key, container); // add it to the cache
			PopulateMediaExplorer(container.Content as StackPanel, directory); // populate it

			GridExplorerMain.Children.Add(container); // don't forget to add it to the UI dummy!!

			return container; // hand it over to whoever is interested
		}

		private static ScrollViewer CreateExplorerContainer()
		{
			return new ScrollViewer()
			{
				IsTabStop = false,
				Focusable = false,
				Margin = Constant.ExplorerMargin.CurrentPage,
				Content = new StackPanel()
				{
					Focusable = false,
				},
				RenderTransform = new ScaleTransform()
				{
					CenterX = 220,
					CenterY = 200
				}
			};
		}
	}
}
