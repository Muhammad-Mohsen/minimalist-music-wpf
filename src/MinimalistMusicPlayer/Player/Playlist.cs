using MinimalistMusicPlayer.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WMPLib;

namespace MinimalistMusicPlayer.Player
{
	// not used currently
	public class PlaylistWrapper
	{
		private readonly IWMPPlaylist InternalPlaylist;

		public List<string> PlaylistFullNames { get; private set; } // will be used to run FullName-comparisons against because IWMPPlaylist::get_item is dog slow

		// field is used for optimization
		// will be used to determine whether to run isSelected/isPlaylistMediaItem comparisons on MediaItems
		// comparisons will only be made if the MediaExplorer.CurrentDirectory == PlaylistDirectory
		public string PlaylistDirectory { get; set; }

		// index of currently selected music
		public int Index { get; set; }
		// the playlist item count
		public int Count
		{
			get
			{
				return InternalPlaylist.count;
			}
		}
		// whether the playlist is shuffling
		public bool IsShuffle { get; set; }
		// playlist repeat mode
		public RepeatMode RepeatMode { get; set; }
		//
		// Constructors
		//
		public PlaylistWrapper(IEnumerable<FileInfo> items = null)
		{
			// WindowsMediaPlayer automatically plays the next track off of its internal "currentPlaylist"
			// so, the playlist has to be separate from the player.
			// currentMedia property is used in order to have control over the next track being played.
			InternalPlaylist = MainWindow.Player.InternalPlayer.newPlaylist("Playlist", Const.PlaylistUri);
			PlaylistFullNames = new List<string>();

			if (items != null) AddPlaylistItems(items.Select(i => i.FullName));

			Index = Const.InvalidIndex;
			IsShuffle = false;
			RepeatMode = RepeatMode.NoRepeat;
		}

		public void CycleRepeat()
		{
			switch (RepeatMode)
			{
				case RepeatMode.NoRepeat:
					RepeatMode = RepeatMode.Repeat;
					break;

				case RepeatMode.Repeat:
					RepeatMode = RepeatMode.RepeatOne;
					break;

				case RepeatMode.RepeatOne:
					RepeatMode = RepeatMode.NoRepeat;
					break;
			}
		}
		public void CycleShuffle()
		{
			IsShuffle = !IsShuffle;
		}

		public IWMPMedia GetItem(int index)
		{
			return InternalPlaylist.get_Item(index);
		}

		public IWMPMedia GetCurrentItem()
		{
			return GetItem(Index);
		}

		// returns the index of the next track that should play
		// takes shuffle, repeat modes into account
		private int GetNextItemIndex()
		{
			if (IsShuffle)
			{
				Random r = new Random();
				return r.Next(0, Count - 1);
			}

			switch (RepeatMode)
			{
				case RepeatMode.NoRepeat:
					if (Index < Count - 1) return Index + 1;
					else return Const.InvalidIndex;

				case RepeatMode.Repeat:
					if (Index < Count - 1) return Index + 1;
					else return 0;

				case RepeatMode.RepeatOne:
				default:
					return Index;
			}
		}

		// has the side-effect of actually setting the Index!!
		public IWMPMedia GetNextItem()
		{
			int nextIndex = GetNextItemIndex();
			Index = nextIndex;

			if (nextIndex != Const.InvalidIndex) return GetItem(nextIndex);

			return null;
		}

		// adds a single item to both IWMPPlaylist and List<string> playlist references
		public void AddPlaylistItem(string itemFullName)
		{
			if (PlaylistFullNames.Contains(itemFullName)) return; // only add the item if it doesn't already exist in the playlist

			int itemIndex = PlaylistFullNames.AddSorted(itemFullName);
			if (itemIndex <= Index) Index++; // if the item was added before the currently-playing item, increment the current index
			InternalPlaylist.insertItem(PlaylistFullNames.IndexOf(itemFullName), MainWindow.Player.InternalPlayer.newMedia(itemFullName)); // insert the new item in its sorted order
		}
		// adds a list of items
		public void AddPlaylistItems(IEnumerable<string> items)
		{
			// set playlist directory
			// AddToSelection button is enabled iff the current directory is equal to the playlist directory.
			// so the integrity of this will remain intact
			PlaylistDirectory = new FileInfo(items.First()).DirectoryName;

			foreach (string item in items)
			{
				if (IsMediaFile(item)) AddPlaylistItem(item);
			}
		}
		// clears playlist
		public void ClearPlaylistItems()
		{
			InternalPlaylist.clear();
			PlaylistFullNames.Clear();
		}

		// validates the extension of a file
		private bool IsMediaFile(string fileUrl)
		{
			string extension = fileUrl.Split('.').Last().ToLower();
			return Const.MediaExtensions.Contains(extension);
		}
	}

	/// <summary>
	/// Playlist repeat modes
	/// </summary>
	public enum RepeatMode
	{
		NoRepeat,
		RepeatOne,
		Repeat
	}
}
