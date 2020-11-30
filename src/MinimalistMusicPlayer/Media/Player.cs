using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;
using MinimalistMusicPlayer.Utility;
using System;

namespace MinimalistMusicPlayer.Media
{
	/// <summary>
	/// Hides the actual player so that it's easily interchangeable
	/// </summary>
	public sealed class Player : IDisposable
	{
		private ISoundOut SoundOut;
		private IWaveSource Source;

		public delegate void OnPlayerStateChangeEventHandler(object sender, PlaybackStateChangeEventArgs e);
		public event OnPlayerStateChangeEventHandler OnPlayerStateChange;

		// Playback State
		public PlaybackState State
		{
			get
			{
				if (SoundOut?.WaveSource == null) return PlaybackState.Invalid; // player hasn't been initialized yet

				switch (SoundOut.PlaybackState)
				{
					case CSCore.SoundOut.PlaybackState.Playing: return PlaybackState.Playing;
					case CSCore.SoundOut.PlaybackState.Paused: return PlaybackState.Paused;
					case CSCore.SoundOut.PlaybackState.Stopped: return PlaybackState.Stopped;
					default: return PlaybackState.Done;
				}
			}
		}

		// whether the player is muted - manually implemented unfortunately with backing fields for both the mute and volume values
		private bool _IsMuted;
		public bool IsMuted
		{
			get
			{
				return _IsMuted;
			}
			set
			{
				_IsMuted = value;
				if (value)
				{
					if (State == PlaybackState.Invalid) return;
					SoundOut.Volume = 0;
				}
				else
				{
					if (State == PlaybackState.Invalid) return;
					SoundOut.Volume = _Volume;
				}
			}
		}

		// player volume level
		private float _Volume;
		public float Volume
		{
			get { return _Volume; }
			set
			{
				_Volume = value;

				if (State == PlaybackState.Invalid) return;
				SoundOut.Volume = value;
			}
		}

		public MediaFile CurrentTrack { get; private set; }

		public TimeSpan CurrentPosition
		{
			get { return Source?.GetPosition() ?? TimeSpan.Zero; }
			set { Source?.SetPosition(value); }
		}
		public string CurrentPositionString
		{
			get
			{
				var format = (Source.GetPosition().TotalHours >= 1) ? Constant.LongFormat : Constant.ShortFormat;
				return Source.GetPosition().ToString(format);
			}
		}

		// ctor
		public Player()
		{
			SoundOut = new WasapiOut() { Latency = 100 };
			SoundOut.Stopped += SoundOut_Stopped;
		}

		public void Play() { SoundOut?.Play(); }
		public void Pause() { SoundOut?.Pause(); }
		public void Resume() { SoundOut?.Play(); }
		public void Stop() { SoundOut?.Stop(); }

		public void PlayTrack(MediaFile item)
		{
			if (item == null) return;
			CurrentTrack = item;

			// a bit of cleanup
			if (State == PlaybackState.Paused) Stop();
			Source?.Dispose();

			Source = CodecFactory.Instance
					.GetCodec(item.FullName)
					.ToSampleSource()
					.ToWaveSource();

			SoundOut.Initialize(Source);
			SoundOut.Volume = IsMuted ? 0 : _Volume;

			Play();
		}

		public double IncrementChapter()
		{
			return CurrentTrack.GetNextChapterStartPosition(CurrentPosition.TotalSeconds);
		}
		public double DecrementChapter()
		{
			return CurrentTrack.GetPreviousChapterStartPosition(CurrentPosition.TotalSeconds);
		}

		public void Dispose()
		{
			SoundOut?.Dispose();
			SoundOut = null;

			Source?.Dispose();
			Source = null;
		}
		//
		// Events
		//
		private void SoundOut_Stopped(object sender, PlaybackStoppedEventArgs e)
		{
			if (CurrentTrack.Duration - CurrentPosition < Constant.SmallTolerance) OnPlayerStateChange.Invoke(this, new PlaybackStateChangeEventArgs(PlaybackState.Done));
		}
	}
	//
	// Play State
	//
	public enum PlaybackState
	{
		Invalid = -1,
		Stopped = 0,
		Playing,
		Paused,
		Done
	}
	//
	// Event Args
	//
	public class PlaybackStateChangeEventArgs : EventArgs
	{
		public PlaybackState State { get; set; }
		public PlaybackStateChangeEventArgs(PlaybackState s)
		{
			State = s;
		}
	}
}
