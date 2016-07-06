using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WMPLib;

namespace MinimalistMusicPlayer
{
	/// <summary>
	/// Contains helper methods that have direct access to UI elements
	/// This is basically the UI API
	/// </summary>
	public partial class MainWindow : Window
    {
		//
		// Playlist UI API
		//
		private void ClearPlaylistStackPanel()
		{
			StackPanelPlaylist.Children.Clear();
		}
		private void AddTracksToPlaylistStackPanel()
		{
			for (int i = 0; i < Player.PlaylistCount; i++)
			{
				PlaylistItem item = new PlaylistItem(Player, Player.Playlist.get_Item(i), i, i == Player.PlaylistIndex);
				StackPanelPlaylist.Children.Add(item);
			}
		}
		private void SelectPlaylistItem(int index)
		{
			PlaylistItem item = (PlaylistItem)StackPanelPlaylist.Children[index];
			PlaylistItem.SelectPlaylistItem(item);
		}
		// expands/collapses Playlist section
		private async void ExpandCollapsePlaylistStackPanel(bool toExpanded)
		{
			if (toExpanded)
			{
				ButtonPlaylist.Style = Util.Styles.AlphaButtonToggleStyle; // style with blue background in its rest state
				ButtonPlaylist.Background = Util.Brushes.BlueBrush;
				// animate the grid
				Anim.AnimateHeight(this, Util.CollapsedWindowHeight, Util.ExpandedWindowHeight, .2);
				Player.IsPlaylistVisible = true;
			}
			else
			{
				ButtonPlaylist.Style = Util.Styles.AlphaButtonStyle; // style with white background in its rest state
				Anim.AnimateHeight(this, Util.ExpandedWindowHeight, Util.CollapsedWindowHeight, .2);
				Player.IsPlaylistVisible = false;

				// await the animation (defined in AlphaButtonStyle control tempalte) to complete before setting the background to white.
				await Task.Delay(200);
				ButtonPlaylist.Background = Util.Brushes.WhiteBrush;
			}
		}

		private void SetPinToTopIcon(bool isTopMost)
		{
			ButtonPinToTop.Style = isTopMost ? Util.Styles.BackgroundButtonToggleStyle : Util.Styles.BackgroundButtonStyle;
			ButtonPinToTop.Background = isTopMost ? Util.Brushes.BlueBrush : Util.Brushes.BackgroundBrush;
		}

		//
		// Volume icon
		//
		// there are three volume icons.
		// sets the icon of the volume button depending on current volume level, and whether the player is muted.
		private void SetVolumeIcon(double volume, bool isMute)
		{
			if (volume == 0 || isMute)
				ButtonVolume.OpacityMask = Util.Icons.VolumeMute;

			else if (volume < Util.VolumeMid)
				ButtonVolume.OpacityMask = Util.Icons.VolumeLow;

			else if (volume >= Util.VolumeMid)
				ButtonVolume.OpacityMask = Util.Icons.VolumeHigh;
		}

		private async void SetRepeatIcon(RepeatMode repeatMode)
		{
			switch (Player.RepeatMode)
			{
				case RepeatMode.NoRepeat:
					ButtonRepeat.OpacityMask = Util.Icons.Repeat;
					ButtonRepeat.Style = Util.Styles.AlphaButtonToggleStyle;
					ButtonRepeat.Background = Util.Brushes.BlueBrush;
					break;

				case RepeatMode.Repeat:
					ButtonRepeat.OpacityMask = Util.Icons.RepeatOne;
					ButtonRepeat.Style = Util.Styles.AlphaButtonToggleStyle;
					ButtonRepeat.Background = Util.Brushes.BlueBrush;
					break;

				case RepeatMode.RepeatOne:
					ButtonRepeat.OpacityMask = Util.Icons.Repeat;
					ButtonRepeat.Style = Util.Styles.AlphaButtonStyle;
					await Task.Delay(200);
					ButtonRepeat.Background = Util.Brushes.WhiteBrush;
					break;
			}
		}

		private async void SetShuffleIcon(bool isShuffle)
		{
			if (isShuffle)
			{
				ButtonShuffle.Style = Util.Styles.AlphaButtonToggleStyle;
				ButtonShuffle.Background = Util.Brushes.BlueBrush;
			}

			else
			{
				ButtonShuffle.Style = Util.Styles.AlphaButtonStyle;

				await Task.Delay(200);
				ButtonShuffle.Background = Util.Brushes.WhiteBrush;
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
					ButtonPlayPause.OpacityMask = Util.Icons.Pause;
					Anim.AnimateOpacity(PlayingIcon, 0, 1, .3);
					break;

				case WMPPlayState.wmppsStopped:
				case WMPPlayState.wmppsMediaEnded:
					SliderSeek.Value = 0;
					ButtonPlayPause.OpacityMask = Util.Icons.Play;
					Anim.AnimateOpacity(PlayingIcon, 1, 0, .3);
					break;

				case WMPPlayState.wmppsPaused:
					ButtonPlayPause.OpacityMask = Util.Icons.Play;
					Anim.AnimateOpacity(PlayingIcon, 1, 0, .3);
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
			LabelSongTitle.Content = track.name.Ellipsize(Util.TrackNameMaxLength);

			string author = !string.IsNullOrEmpty(track.getItemInfo("Author")) ? track.getItemInfo("Author") : "Unknown Artist";
			string album = !string.IsNullOrEmpty(track.getItemInfo("AlbumID")) ? track.getItemInfo("AlbumID") : "Unknown Album";

			LabelArtistAlbum.Content = string.Concat(author, " (", album, ")").Ellipsize(Util.TrackInfoMaxLength);

			// set the window title (in the taskbar)
			this.Title = string.Concat(track.name, " - Minimalist");

			// set track title label tooltip
			ToolTipTrackTitle.Content = track.name;
			// set track artist/album label tooltip
			ToolTipTrackArtistAlbum.Content = string.Concat(author, " (", album, ")");
		}
    }
}