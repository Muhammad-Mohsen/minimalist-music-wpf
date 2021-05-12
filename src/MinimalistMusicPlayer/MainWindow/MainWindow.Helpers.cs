using MinimalistMusicPlayer.Media;
using MinimalistMusicPlayer.Utility;
using System;
using System.Windows;
using System.Windows.Shapes;

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
				Anim.AnimateHeight(this, Constant.ExpandedWindowHeight, .2);
				IsPlaylistVisible = true;

				Anim.AnimateAngle(ButtonPlaylistIcon, 0, 180, .3, false);
			}
			else
			{
				Anim.AnimateHeight(this, Constant.CollapsedWindowHeight, .2);
				IsPlaylistVisible = false;

				Anim.AnimateAngle(ButtonPlaylistIcon, 180, 0, .3, false);
			}
		}
		//
		// Volume icon
		//
		// there are three volume icons.
		// sets the icon of the volume button depending on current volume level, and whether the player is muted.
		private void SetVolumeIcon(double volume, bool isMute)
		{
			if (volume == 0 || isMute) ButtonVolume.Content = Icons.VolumeMute;
			else if (volume < Constant.VolumeMid) ButtonVolume.Content = Icons.VolumeLow;
			else if (volume >= Constant.VolumeMid) ButtonVolume.Content = Icons.VolumeHigh;
		}

		private void SetRepeatIcon(RepeatMode oldRepeatMode)
		{
			ButtonRepeat.Content = oldRepeatMode == RepeatMode.Repeat ? Icons.RepeatOne : Icons.Repeat;
			(ButtonRepeat.Content as Path).Stroke = oldRepeatMode == RepeatMode.RepeatOne ? Brushes.AccentBrush : Brushes.PrimaryTextBrush;
		}

		private void SetShuffleIcon(bool isShuffle)
		{
			(ButtonShuffle.Content as Path).Stroke = isShuffle ? Brushes.PrimaryTextBrush : Brushes.AccentBrush;
		}

		// sets UI state for Play/Pause button, playing icon, and taskbar progress icon state
		private void SetPlayPauseUiState(PlaybackState state)
		{
			switch (state)
			{
				case PlaybackState.Playing:
					ButtonPlayPause.Content = Icons.Pause;
					ThumbButtonInfoPlayPause.ImageSource = Icons.ThumbnailPause;
					Anim.AnimateOpacity(PlayingIcon, Constant.OpacityLevel.Opaque, .3);
					TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
					break;

				case PlaybackState.Stopped:
				case PlaybackState.Done:
					SliderSeek.Value = 0;
					ButtonPlayPause.Content = Icons.Play;
					ThumbButtonInfoPlayPause.ImageSource = Icons.ThumbnailPlay;
					Anim.AnimateOpacity(PlayingIcon, Constant.OpacityLevel.Transparent, .3);
					TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
					break;

				case PlaybackState.Paused:
					ButtonPlayPause.Content = Icons.Play;
					ThumbButtonInfoPlayPause.ImageSource = Icons.ThumbnailPlay;
					Anim.AnimateOpacity(PlayingIcon, Constant.OpacityLevel.Transparent, .3);
					TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Paused;
					break;
			}
		}

		// sets the max duration for the seek slider, and the track duration label
		private void SetDurationValues(MediaFile track)
		{
			SliderSeek.Maximum = track.Duration.TotalSeconds;
			LabelTotalTime.Content = track.DurationString;
		}

		// sets the track name, album, and artist labels, as well as the application title
		private void SetTrackInfo(MediaFile track)
		{
			LabelSongTitle.Text = track.File.Name;

			string author = track.Artist;
			string album = track.Album;
			LabelArtistAlbum.Text = string.Concat(author, " (", album, ")");

			Title = string.Concat(track.File.Name, " - Minimalist"); // set the window title (in the taskbar)

			ToolTipTrackTitle.Content = track.File.Name; // set track title label tooltip
			ToolTipTrackArtistAlbum.Content = string.Concat(author, " (", album, ")"); // set track artist/album label tooltip
		}

		private void SetSeekTooltip(double position, double maxValue, double width)
		{
			ToolTipSeek.HorizontalOffset = position;
			double seconds = position * maxValue / width;
			ToolTipSeek.Content = TimeSpan.FromSeconds(seconds).ToString().Substring(3, 5);
		}

		private void SetChapterMarkers(MediaFile track)
		{
			GridChapters.Children.Clear();
			if (!track.HasChapters()) return;

			var totalWidthSeconds = track.Duration.TotalSeconds;
			var totalWidthPixels = GridChapters.ActualWidth;

			foreach (var chapter in track.Chapters)
			{
				var ratio = chapter.StartPosition / totalWidthSeconds;
				var marker = CreateChapterMarker(totalWidthPixels * ratio - Constant.SliderThumbWidth * ratio); // account for the width of the slider thumb (the slider itself does!!)
				GridChapters.Children.Add(marker);
			}
		}
		private static Rectangle CreateChapterMarker(double position)
		{
			return new Rectangle
			{
				Width = 1,
				Margin = new Thickness(position, 0, 0, 0),
				HorizontalAlignment = HorizontalAlignment.Left,
				Fill = Brushes.PrimaryHoverBrush,
			};
		}

		// updates everything wholesale
		private void UpdateUi()
		{
			var track = Player.CurrentTrack;

			SelectMediaItemByIndex(Playlist.CurrentIndex);

			SetPlayPauseUiState(Player.State);
			SetDurationValues(track);
			SetTrackInfo(track);
			SetChapterMarkers(track);
		}
	}
}
