using MinimalistMusicPlayer.Explorer;
using MinimalistMusicPlayer.Player;
using MinimalistMusicPlayer.Utility;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WMPLib;

namespace MinimalistMusicPlayer
{
	/// <summary>
	/// Contains helper methods that have direct access to UI elements
	/// This is basically the UI API
	/// </summary>
	public partial class MainWindow : Window
    {
		// expands/collapses Playlist section
		private async void ExpandCollapsePlaylistStackPanel(bool toExpanded)
		{
			if (toExpanded)
			{
				ButtonPlaylist.Style = Styles.AlphaButtonToggleStyle; // style with blue background in its rest state
				ButtonPlaylist.Background = Brushes.BlueBrush;
				// animate the grid
				Anim.AnimateHeight(this, Const.ExpandedWindowHeight, .2);
				Player.IsPlaylistVisible = true;
			}
			else
			{
				ButtonPlaylist.Style = Styles.AlphaButtonStyle; // style with white background in its rest state
				Anim.AnimateHeight(this, Const.CollapsedWindowHeight, .2);
				Player.IsPlaylistVisible = false;

				// await the animation (defined in AlphaButtonStyle control tempalte) to complete before setting the background to white.
				await Task.Delay(200);
				ButtonPlaylist.Background = Brushes.WhiteBrush;
			}
		}

		private void SetPinToTopIcon(bool isTopMost)
		{
			ButtonPinToTop.Style = isTopMost ? Styles.BackgroundButtonToggleStyle : Styles.BackgroundButtonStyle;
			ButtonPinToTop.Background = isTopMost ? Brushes.BlueBrush : Brushes.BackgroundBrush;
		}

		//
		// Volume icon
		//
		// there are three volume icons.
		// sets the icon of the volume button depending on current volume level, and whether the player is muted.
		private void SetVolumeIcon(double volume, bool isMute)
		{
			if (volume == 0 || isMute)
				ButtonVolume.OpacityMask = Icons.VolumeMute;

			else if (volume < Const.VolumeMid)
				ButtonVolume.OpacityMask = Icons.VolumeLow;

			else if (volume >= Const.VolumeMid)
				ButtonVolume.OpacityMask = Icons.VolumeHigh;
		}

		private async void SetRepeatIcon(RepeatMode repeatMode)
		{
			switch (Player.RepeatMode)
			{
				case RepeatMode.NoRepeat:
					ButtonRepeat.OpacityMask = Icons.Repeat;
					ButtonRepeat.Style = Styles.AlphaButtonToggleStyle;
					ButtonRepeat.Background = Brushes.BlueBrush;
					break;

				case RepeatMode.Repeat:
					ButtonRepeat.OpacityMask = Icons.RepeatOne;
					ButtonRepeat.Style = Styles.AlphaButtonToggleStyle;
					ButtonRepeat.Background = Brushes.BlueBrush;
					break;

				case RepeatMode.RepeatOne:
					ButtonRepeat.OpacityMask = Icons.Repeat;
					ButtonRepeat.Style = Styles.AlphaButtonStyle;
					await Task.Delay(200);
					ButtonRepeat.Background = Brushes.WhiteBrush;
					break;
			}
		}

		private async void SetShuffleIcon(bool isShuffle)
		{
			if (isShuffle)
			{
				ButtonShuffle.Style = Styles.AlphaButtonToggleStyle;
				ButtonShuffle.Background = Brushes.BlueBrush;
			}

			else
			{
				ButtonShuffle.Style = Styles.AlphaButtonStyle;

				await Task.Delay(200);
				ButtonShuffle.Background = Brushes.WhiteBrush;
			}
		}

		// sets the enable state of the next/previous track buttons
		// should be disabled when shuffle is on
		private void SetNextPrevEnableState(Button button, bool enable)
		{
			button.IsEnabled = enable;
		}

		// sets UI state for Play/Pause button, playing icon
		private void SetPlayPauseUiState(WMPPlayState state)
		{
			switch (state)
			{
				case WMPPlayState.wmppsPlaying:
					ButtonPlayPause.OpacityMask = Icons.Pause;
					Anim.AnimateOpacity(PlayingIcon, Const.OpacityLevel.Opaque, .3);
					break;

				case WMPPlayState.wmppsStopped:
				case WMPPlayState.wmppsMediaEnded:
					SliderSeek.Value = 0;
					ButtonPlayPause.OpacityMask = Icons.Play;
					Anim.AnimateOpacity(PlayingIcon, Const.OpacityLevel.Transparent, .3);
					break;

				case WMPPlayState.wmppsPaused:
					ButtonPlayPause.OpacityMask = Icons.Play;
					Anim.AnimateOpacity(PlayingIcon, Const.OpacityLevel.Transparent, .3);
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

			// set the window title (in the taskbar)
			this.Title = string.Concat(track.name, " - Minimalist");

			// set track title label tooltip
			ToolTipTrackTitle.Content = track.name;
			// set track artist/album label tooltip
			ToolTipTrackArtistAlbum.Content = string.Concat(author, " (", album, ")");
		}
	}
}