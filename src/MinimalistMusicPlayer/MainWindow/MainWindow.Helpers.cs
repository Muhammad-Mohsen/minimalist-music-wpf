using MinimalistMusicPlayer.Player;
using MinimalistMusicPlayer.Utility;
using System;
using System.Windows.Shapes;
using WMPLib;

namespace MinimalistMusicPlayer
{
	/// <summary>
	/// Contains helper methods that have direct access to UI elements
	/// This is basically the UI API
	/// </summary>
	public partial class MainWindow
	{
		// expands/collapses Playlist section
		private void ExpandCollapsePlaylistStackPanel(bool toExpanded)
		{
			if (toExpanded)
			{
				// animate the grid
				Anim.AnimateHeight(this, Const.ExpandedWindowHeight, .2);
				Player.IsPlaylistVisible = true;

				Anim.AnimateAngle(ButtonPlaylistIcon, 0, 180, .3, false);
			}
			else
			{
				Anim.AnimateHeight(this, Const.CollapsedWindowHeight, .2);
				Player.IsPlaylistVisible = false;

				Anim.AnimateAngle(ButtonPlaylistIcon, 180, 0, .3, false);
			}
		}

		private void SetPinToTopIcon(bool isTopMost)
		{
			// ButtonPinToTop.Style = isTopMost ? Styles.BackgroundButtonToggleStyle : Styles.BackgroundButtonStyle;
			// ButtonPinToTop.Background = isTopMost ? Brushes.AccentBrush : Brushes.PrimaryBrush;
		}
		//
		// Volume icon
		//
		// there are three volume icons.
		// sets the icon of the volume button depending on current volume level, and whether the player is muted.
		private void SetVolumeIcon(double volume, bool isMute)
		{
			if (volume == 0 || isMute) ButtonVolume.Content = Icons.VolumeMute;
			else if (volume < Const.VolumeMid) ButtonVolume.Content = Icons.VolumeLow;
			else if (volume >= Const.VolumeMid) ButtonVolume.Content = Icons.VolumeHigh;
		}

		private void SetRepeatIcon(RepeatMode oldRepeatMode)
		{
			ButtonRepeat.Content = oldRepeatMode == RepeatMode.Repeat ? Icons.RepeatOne : Icons.Repeat;
			(ButtonRepeat.Content as Path).Stroke = oldRepeatMode == RepeatMode.RepeatOne ? Brushes.AccentBrush : Brushes.PrimaryTextBrush;
		}

		private void SetShuffleIcon(bool isShuffle)
		{
			(ButtonShuffle.Content as Path).Stroke = isShuffle ? Brushes.PrimaryTextBrush: Brushes.AccentBrush;
		}

		// sets UI state for Play/Pause button, playing icon, and taskbar progress icon state
		private void SetPlayPauseUiState(WMPPlayState state)
		{
			switch (state)
			{
				case WMPPlayState.wmppsPlaying:
					ButtonPlayPause.Content = Icons.Pause;
					ThumbButtonInfoPlayPause.ImageSource = Icons.ThumbnailPause;
					Anim.AnimateOpacity(PlayingIcon, Const.OpacityLevel.Opaque, .3);
					TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
					break;

				case WMPPlayState.wmppsStopped:
				case WMPPlayState.wmppsMediaEnded:
					SliderSeek.Value = 0;
					ButtonPlayPause.Content = Icons.Play;
					ThumbButtonInfoPlayPause.ImageSource = Icons.ThumbnailPlay;
					Anim.AnimateOpacity(PlayingIcon, Const.OpacityLevel.Transparent, .3);
					TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
					break;

				case WMPPlayState.wmppsPaused:
					ButtonPlayPause.Content = Icons.Play;
					ThumbButtonInfoPlayPause.ImageSource = Icons.ThumbnailPlay;
					Anim.AnimateOpacity(PlayingIcon, Const.OpacityLevel.Transparent, .3);
					TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Paused;
					break;
			}
		}

		// sets the max duration for the seek slider, and the track duration label
		private void SetDurationValues(IWMPMedia track)
		{
			SliderSeek.Maximum = track.duration;
			LabelTotalTime.Content = track.durationString;
		}

		// sets the track name, album, and artist labels, as well as the application title
		private void SetTrackInfo(IWMPMedia track)
		{
			LabelSongTitle.Content = track.name.Ellipsize(Const.TrackNameMaxLength);

			string author = !string.IsNullOrEmpty(track.getItemInfo("Author")) ? track.getItemInfo("Author") : "Unknown Artist";
			string album = !string.IsNullOrEmpty(track.getItemInfo("AlbumID")) ? track.getItemInfo("AlbumID") : "Unknown Album";

			LabelArtistAlbum.Content = string.Concat(author, " (", album, ")").Ellipsize(Const.TrackInfoMaxLength);

			Title = string.Concat(track.name, " - Minimalist"); // set the window title (in the taskbar)

			ToolTipTrackTitle.Content = track.name; // set track title label tooltip
			ToolTipTrackArtistAlbum.Content = string.Concat(author, " (", album, ")"); // set track artist/album label tooltip
		}

		private void SetSeekTooltip(double position, double maxValue, double width)
		{
			ToolTipSeek.HorizontalOffset = position;
			double seconds = position * maxValue / width;
			ToolTipSeek.Content = TimeSpan.FromSeconds(seconds).ToString().Substring(3, 5);
		}
	}
}
