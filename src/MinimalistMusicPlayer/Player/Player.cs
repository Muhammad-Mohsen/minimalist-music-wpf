using MinimalistMusicPlayer.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMPLib;

namespace MinimalistMusicPlayer.Player
{
	/// <summary>
	/// Wrapper around WindowsMediaPlayer class
	/// </summary>
	public class PlayerWrapper
	{
		// wrapper props
		public WindowsMediaPlayer InternalPlayer { get; private set; }

		public IWMPPlaylist Playlist { get; private set; }
		public List<string> PlaylistFullNames { get; private set; } // will be used to run FullName-comparisons against because IWMPPlaylist::get_item is dog slow
		
		public IWMPControls Controls { get; private set; }
		public IWMPSettings Settings { get; private set; }
		// current play state (Playing, paused, stopped, etc)
		public WMPPlayState PlayState
		{
			get
			{
				return InternalPlayer.playState;
			}
		}
		public IWMPMedia CurrentMedia
		{
			get
			{
				return InternalPlayer.currentMedia;
			}
		}

		// field is used for optimization
		// will be used to determine whether to run isSelected/isPlaylistMediaItem comparisons on MediaItems
		// comparisons will only be made if the MediaExplorer.CurrentDirectory == PlaylistDirectory
		public string PlaylistDirectory { get; set; }

		// whether the player is muted
		public bool IsMuted
		{
			get
			{
				return InternalPlayer.settings.mute;
			}
			set
			{
				InternalPlayer.settings.mute = value;
			}
		}
		// player volume level
		public int Volume
		{
			get
			{
				return InternalPlayer.settings.volume;
			}
			set
			{
				InternalPlayer.settings.volume = value;
			}
		}
		
		// whether the playlist is visible
		public bool IsPlaylistVisible { get; set; }

		// index of currently selected music
		public int Index { get; set; }
		// the playlist item count
		public int Count
		{
			get
			{
				return Playlist.count;
			}
		}
		// whether the playlist is shuffling
		public bool IsShuffle { get; set; }
		// playlist repeat mode
		public RepeatMode RepeatMode { get; set; }
		//
		// constructor
		//
		public PlayerWrapper()
		{
			InternalPlayer = new WindowsMediaPlayer();
			Playlist = InternalPlayer.newPlaylist("Playlist", Const.PlaylistUri);
			PlaylistFullNames = new List<string>();

			Controls = InternalPlayer.controls;
			Settings = InternalPlayer.settings;

			Index = Const.InvalidIndex;
			IsShuffle = false;
			RepeatMode = RepeatMode.NoRepeat;
		}
		
		// resume media playback
		public void Resume()
		{
			Controls.play();
		}
		// pause media playback
		public void Pause()
		{
			Controls.pause();
			// return Player.playState == WMPPlayState.wmppsPaused;
		}
		// stop media playback
		public void Stop()
		{
			Controls.stop();
			// return Player.playState == WMPPlayState.wmppsStopped;
		}
		//
		// Player API
		//
		// play selected music track
		public void Play(IWMPMedia media)
		{
			InternalPlayer.currentMedia = media;
			Controls.play();
		}
		// play selected music track based on index
		public void Play(int trackPlaylistIndex)
		{
			IWMPMedia item = GetItem(trackPlaylistIndex);
			InternalPlayer.currentMedia = item;
			Controls.play();
		}
		// play next music track based on RepeatMode, and shuffle state
		public void PlayNextTrack(int currentIndex)
		{
			IWMPMedia track = GetNextItem(currentIndex);
			
			// it's null when nothing should be played (NoRepeat && end of playlist)
			if (track != null)
				Play(track);
			else
				Stop();
		}
		//
		// Playlist API
		//
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

		public IWMPMedia GetItem(int index)
		{
			return Playlist.get_Item(index);
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
		
		public IWMPMedia GetNextItem(int currentIndex)
		{
			int nextIndex = GetNextItemIndex(currentIndex);
			// set member variable
			Index = nextIndex;

			if (nextIndex != Const.InvalidIndex)
				return GetItem(nextIndex);

			return null;
		}

		// adds a single item to both IWMPPlaylist and List<string> playlist references
		public void AddPlaylistItem(string itemFullName)
		{
			if (!PlaylistFullNames.Contains(itemFullName)) // only add the item if it doesn't already exist in the playlist
			{
				int itemIndex = PlaylistFullNames.AddSorted(itemFullName);

				// if the item was added before the currently-playing item, increment the current index
				if (itemIndex <= Index)
					Index++;

				Playlist.insertItem(PlaylistFullNames.IndexOf(itemFullName), InternalPlayer.newMedia(itemFullName)); // insert the new item in its sorted order
			}
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
				if (PlaylistWrapper.IsMediaFile(item))
					AddPlaylistItem(item);
			}
		}
		// clears playlist
		public void ClearPlaylistItems()
		{
			Playlist.clear();
			PlaylistFullNames.Clear();
		}
	}
}