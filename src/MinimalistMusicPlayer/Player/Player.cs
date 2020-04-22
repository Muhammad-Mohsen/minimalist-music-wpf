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

		//
		// constructor
		//
		public PlayerWrapper()
		{
			InternalPlayer = new WindowsMediaPlayer();

			Controls = InternalPlayer.controls;
			Settings = InternalPlayer.settings;
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
			if (media == null) return; // at the end of the playlist

			InternalPlayer.currentMedia = media;
			Controls.play();
		}
	}
}