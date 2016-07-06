using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMPLib;

namespace MinimalistMusicPlayer
{
	/// <summary>
	/// Wrapper around WindowsMediaPlayer class
	/// </summary>
	public class WmpPlayer
	{
		// wrapper props
		public WindowsMediaPlayer Player { get; private set; }
		public IWMPPlaylist Playlist { get; private set; }
		public IWMPControls Controls { get; private set; }
		public IWMPSettings Settings { get; private set; }
		// current playstate (Playing, paused, stopped, etc)
		public WMPPlayState PlayState
		{
			get
			{
				return Player.playState;
			}
		}
		public IWMPMedia CurrentMedia
		{
			get
			{
				return Player.currentMedia;
			}
		}
		// whether the player is muted
		public bool IsMuted
		{
			get
			{
				return Player.settings.mute;
			}
			set
			{
				Player.settings.mute = value;
			}
		}
		// player volume level
		public int Volume
		{
			get
			{
				return Player.settings.volume;
			}
			set
			{
				Player.settings.volume = value;
			}
		}
		// index of currently selected music
		public int PlaylistIndex { get; set; }
		// the playlist item count
        public int PlaylistCount 
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
		// whether the playlist is visible
		public bool IsPlaylistVisible { get; set; }
		//
		// constructor
		//
        public WmpPlayer()
        {
			Player = new WindowsMediaPlayer();
			// WindowsMediaPlayer automatically plays the next track off of its internal "currentPlaylist"
			// so, the playlist has to be separate from the player.
			// currentMedia property is used in order to have control over the next track being played.
			Playlist = Player.newPlaylist("Playlist", Util.PlaylistUri);
			Controls = Player.controls;
			Settings = Player.settings;
			
			PlaylistIndex = -1;
			IsShuffle = false;
			RepeatMode = RepeatMode.NoRepeat;
        }
		//
		// Player API
		//
		// play selected music track based on index
		public void StartPlay(int index)
		{
			// if there's nothing to play...
			if (PlaylistCount == 0)
				return;

			// validate index falls within the playlist boundries - probably unnecessary
			if (index < 0)
				index = 0;

			if (index >= PlaylistCount)
				index = PlaylistCount - 1;

			PlaylistIndex = index; // set member
			Player.currentMedia = Playlist.get_Item(index);
			Controls.play();
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
		// play next music track based on RepeatMode, and shuffle state
		public void StartPlayNext()
		{
			PlaylistIndex = GetNextTrackIndex();

			// if you got to the end of the playlist and you're at NoRepeat, stop playing
			if (RepeatMode == RepeatMode.NoRepeat && PlaylistIndex == 0)
				Stop();

			// else, just keep on playin'
			else
				StartPlay(PlaylistIndex);
		}

		public void SetNewRepeatMode(RepeatMode oldRepeatMode)
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
		private int GetNextTrackIndex()
		{
			if (IsShuffle)
			{
				Random r = new Random();
				return r.Next(0, PlaylistCount - 1);
			}

			switch (RepeatMode)
			{
				case RepeatMode.NoRepeat:
				case RepeatMode.Repeat:
					if (PlaylistIndex < PlaylistCount - 1)
						return PlaylistIndex + 1;
					else
						return 0;

				case RepeatMode.RepeatOne:
				default:
					return PlaylistIndex;
			}
		}
		//
		// Playlist API
		//
		// adds a single item
		public void AddPlaylistItem(string itemUrl)
		{
			Playlist.appendItem(Player.newMedia(itemUrl));
		}
		// adds a list of items
		public void AddPlaylistItems(IEnumerable<string> items)
		{
			foreach (string item in items)
			{
				if (Util.IsValidMediaFile(item))
					AddPlaylistItem(item);
			}
		}
		// clears playlist
		public void ClearPlaylistItems()
		{
			Playlist.clear();
		}
	}
}