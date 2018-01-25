using MinimalistMusicPlayer.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMPLib;

namespace MinimalistMusicPlayer.Player
{
	// not used currently
	public class Playlist
	{
		private IWMPPlaylist P;

		// static instance used to new-up media instances
		private static WindowsMediaPlayer player = new WindowsMediaPlayer();

		// index of currently selected music
		public int Index { get; set; }
		// the playlist item count
		public int Count
		{
			get
			{
				return P.count;
			}
		}
		// whether the playlist is shuffling
		public bool IsShuffle { get; set; }
		// playlist repeat mode
		public RepeatMode RepeatMode { get; set; }
		//
		// Constructors
		//
		public Playlist()
		{
			// WindowsMediaPlayer automatically plays the next track off of its internal "currentPlaylist"
			// so, the playlist has to be separate from the player.
			// currentMedia property is used in order to have control over the next track being played.
			P = new WindowsMediaPlayer().newPlaylist("Playlist", Const.PlaylistUri);

			Index = Const.InvalidIndex;
			IsShuffle = false;
			RepeatMode = RepeatMode.NoRepeat;
		}
		public Playlist(IEnumerable<FileInfo> items)
		{
			P = new WindowsMediaPlayer().newPlaylist("Playlist", Const.PlaylistUri);
			AddPlaylistItems(items.Select(i => i.FullName));

			Index = Const.InvalidIndex;
			IsShuffle = false;
			RepeatMode = RepeatMode.NoRepeat;
		}

		public void AdvanceRepeatMode(RepeatMode oldRepeatMode)
		{
			switch (oldRepeatMode)
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

		// returns the index of the next track that should play
		// takes shuffle, repeat modes into account
		public int GetNextItemIndex(int index)
		{
			if (IsShuffle)
			{
				Random r = new Random();
				return r.Next(0, Count - 1);
			}

			switch (RepeatMode)
			{
				case RepeatMode.NoRepeat:
					if (index < Count - 1)
						return index + 1;
					else
						return Const.InvalidIndex;

				case RepeatMode.Repeat:
					if (index < Count - 1)
						return index + 1;
					else
						return 0;

				case RepeatMode.RepeatOne:
				default:
					return index;
			}
		}

		public IWMPMedia GetItem(int index)
		{
			return P.get_Item(index);
		}

		public IWMPMedia GetNextItem(int currentIndex)
		{
			int nextIndex = GetNextItemIndex(currentIndex);
			// set member variable
			Index = nextIndex;

			if (nextIndex != Const.InvalidIndex)
				return GetItem(nextIndex);

			return null;
		}

		// adds a single item
		public void AddPlaylistItem(string itemUrl)
		{
			P.appendItem(player.newMedia(itemUrl));
		}
		// adds a list of items
		public void AddPlaylistItems(IEnumerable<string> items)
		{
			foreach (string item in items)
			{
				if (Util.IsMediaFile(item))
					AddPlaylistItem(item);
			}
		}
		// clears playlist
		public void ClearPlaylistItems()
		{
			P.clear();
		}
	}
}
