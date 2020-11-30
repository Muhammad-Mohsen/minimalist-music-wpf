using MinimalistMusicPlayer.Media;
using MinimalistMusicPlayer.Utility;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MinimalistMusicPlayer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		// needs to be defined before InitializeComponent, believe it or not!!
		// Player.Position is accessed during InitializeComponent (see ValueChanged events)
		private static readonly Player Player = new Player();
		private static readonly Playlist Playlist = new Playlist();

		bool IsSeeking = false; // indicates whether the seek slider value is being manually changed
		float TempVolume;

		// move window fields
		Point InitialPosition;
		bool CanMoveWindow;

		public MainWindow()
		{
			// hook up a handler for PlayState change event
			Player.OnPlayerStateChange += Player_PlayStateChange;

			InitializeComponent();

			SliderVolume.Opacity = 0;

			PlayingIcon.Opacity = 0; // initialize playing icon visibility to hidden
			Anim.AnimateAngle(PlayingIcon, 0, 360, 2, true); // start a continuous rotation animation for the playing icon

			string savedDirectory = Properties.Settings.Default[Constant.ExplorerDirectorySetting].ToString();
			if (Directory.Exists(savedDirectory)) CurrentDirectory = new DirectoryInfo(savedDirectory);
			else CurrentDirectory = new DirectoryInfo(Constant.DefaultMediaDirectory);

			// intialize the explorer
			ScrollViewerExplorer = GetExplorer(CurrentDirectory);
			StackPanelExplorer = ScrollViewerExplorer.Content as StackPanel;

			// initialize the breadcrumb bar
			InitializeBreadcrumbBar(CurrentDirectory);

			InitializeTimer(); // timer to update the seek bar, volume fade, etc.

			// set up progress icon on the taskbar icon
			TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
			TaskbarItemInfo.ProgressValue = 0;
		}
		//
		// Track info grid Events - Cutting corners! Too sleepy to do anything decent.
		//
		private void ButtonTrackInfo_Click(object sender, RoutedEventArgs e)
		{
			// Expand playlist
			if (IsPlaylistVisible == false) ExpandCollapsePlaylistStackPanel(true);

			// go to playlist directory if possible
			if (!string.IsNullOrEmpty(Playlist.PlaylistDirectory)) DirectoryChange(new DirectoryInfo(Playlist.PlaylistDirectory));
		}
		//
		// Button events
		//
		private void ButtonPlaylist_Click(object sender, RoutedEventArgs e)
		{
			ExpandCollapsePlaylistStackPanel(IsPlaylistVisible == false);
		}
		private void ButtonPlayPause_Click(object sender, RoutedEventArgs e)
		{
			if (Player.CurrentTrack == null) return;

			// more sadness - check if the playlist is done, and if it is, go back to the start...sadness no more guys!!
			if (Playlist.CurrentIndex == Constant.InvalidIndex)
			{
				Playlist.CurrentIndex = 0;
				Player.PlayTrack(Playlist.GetTrack(Playlist.CurrentIndex));
			}

			if (Player.State == PlaybackState.Playing) Player.Pause();
			else Player.Resume();

			SetPlayPauseUiState(Player.State);
			SetDurationValues(Player.CurrentTrack);
			SetTrackInfo(Player.CurrentTrack);
		}
		private void ButtonNextPrevious_Click(object sender, RoutedEventArgs e)
		{
			// check to see if file has chapters or not and try to increment those first
			if (Player.CurrentTrack.HasChapters())
			{
				var chapterPosition = (((Button)sender).Name == "ButtonPrev") ? Player.DecrementChapter() : Player.IncrementChapter();
				if (chapterPosition != Constant.InvalidIndex)
				{
					Player.CurrentPosition = TimeSpan.FromSeconds(chapterPosition);
					return;
				}
			}

			if (((Button)sender).Name == "ButtonPrev") Playlist.DecrementIndex();
			else Playlist.IncrementIndex();

			var track = Playlist.GetTrack(Playlist.CurrentIndex);
			if (track == null) return;

			Player.PlayTrack(track);
			SelectMediaItemByIndex(Playlist.CurrentIndex);

			SetPlayPauseUiState(PlaybackState.Playing);
			SetDurationValues(track);
			SetTrackInfo(track);
		}
		private void ButtonRepeat_Click(object sender, RoutedEventArgs e)
		{
			SetRepeatIcon(Playlist.RepeatMode);
			Playlist.CycleRepeat(); // method takes the old repeat mode, and sets the new repeat mode.
		}
		private void ButtonShuffle_Click(object sender, RoutedEventArgs e)
		{
			Playlist.IsShuffle = !Playlist.IsShuffle;
			SetShuffleIcon(Playlist.IsShuffle);
		}
		//
		// Seek slider events
		//
		private void SliderSeek_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			TempVolume = Player.Volume;
			Player.Volume = 0;
			IsSeeking = true;
		}
		private void SliderSeek_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			IsSeeking = false;
			Player.Volume = TempVolume;
		}
		private void SliderSeek_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsSeeking) Player.CurrentPosition = TimeSpan.FromSeconds(SliderSeek.Value);
		}
		private void SliderSeek_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (Player.CurrentTrack == null) return;
			double position = e.GetPosition((IInputElement)sender).X;
			SetSeekTooltip(position, SliderSeek.Maximum, SliderSeek.ActualWidth);
		}
		//
		// Player events
		//
		private void Player_PlayStateChange(object sender, PlaybackStateChangeEventArgs args)
		{
			if (args.State != PlaybackState.Done) return;

			Playlist.CurrentIndex = Playlist.NextIndex;
			var track = Playlist.GetTrack(Playlist.CurrentIndex);

			if (track == null)
			{
				SetPlayPauseUiState(args.State);
				return;
			}

			Player.PlayTrack(track);

			SetPlayPauseUiState(args.State);
			SetDurationValues(track);
			SetTrackInfo(track);
			SelectMediaItemByIndex(Playlist.CurrentIndex);
		}
		//
		// Volume slider/button events
		//
		private void ButtonVolume_Click(object sender, RoutedEventArgs e)
		{
			Player.IsMuted = !Player.IsMuted;
			if (Player.IsMuted) SliderVolume.Value = 0;
			else SliderVolume.Value = Player.Volume;

			SetVolumeIcon(SliderVolume.Value, Player.IsMuted);
		}
		private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			// only set the player volume if it was changed through the slider
			// not the mute button
			if (!Player.IsMuted) Player.Volume = (float)SliderVolume.Value;
			SetVolumeIcon(SliderVolume.Value, Player.IsMuted);
		}
		//
		// Volume slider animation events
		//
		private void Volume_MouseEnter(object sender, MouseEventArgs e)
		{
			VolumeSliderFadeCounter = -1;
			if (SliderVolume.Opacity == 0 && sender.Equals(ButtonVolume)) // only display the volume slider when the mouse enters the button, not the slider itself
				Anim.AnimateOpacity(SliderVolume, Constant.OpacityLevel.Opaque, .1);
		}
		private void Volume_MouseLeave(object sender, MouseEventArgs e)
		{
			VolumeSliderFadeCounter = 0;
		}
		//
		// Window button events
		//
		// minimize
		private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}
		// close
		private void ButtonClose_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
		//
		// move window
		//
		// had to be done this way so that if the mouse is pressed from outside the grid,
		// and then it moves within it, it wouldn't fire the event
		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.OriginalSource.GetType() == typeof(Border))
			{
				InitialPosition = e.GetPosition(null);
				CanMoveWindow = true;
			}
		}
		private void Window_MouseMove(object sender, MouseEventArgs e)
		{
			if (CanMoveWindow)
			{
				Point currentPosition = e.GetPosition(null);
				// Left and Top are window Properties
				Left = Left + currentPosition.X - InitialPosition.X;
				Top = Top + currentPosition.Y - InitialPosition.Y;
			}
		}
		private void Window_MouseUp(object sender, MouseButtonEventArgs e)
		{
			CanMoveWindow = false;
		}
		//
		// shortcut keys
		//
		// so far we got play/pause shortcut
		private void Window_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Space: // play/pause
					ButtonPlayPause_Click(null, null);
					break;
				case Key.Left: // seek left
					IsSeeking = true;
					SliderSeek.Value = Math.Max(0, SliderSeek.Value - SliderSeek.Maximum / Constant.SeekDivisor);
					IsSeeking = false;
					break;
				case Key.Right: // seek right
					IsSeeking = true;
					SliderSeek.Value = Math.Min(SliderSeek.Maximum, SliderSeek.Value + SliderSeek.Maximum / Constant.SeekDivisor);
					IsSeeking = false;
					break;
				case Key.NumPad0: // seek right
					IsSeeking = true;
					SliderSeek.Value = 0;
					IsSeeking = false;
					break;
			}
		}
		// if the mouse wheel gives a positive delta, increment the volume, else decrement it. pretty straight forward really.
		private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (e.Delta > 0)
				SliderVolume.Value += Constant.VolumeIncrement;
			else
				SliderVolume.Value -= Constant.VolumeIncrement;
		}
		//
		// Thumbnail buttons
		//
		private void ThumbButtonInfoPrevious_Click(object sender, EventArgs e)
		{
			ButtonNextPrevious_Click(new Button { Name = "ButtonPrev" }, null);
		}
		private void ThumbButtonInfoPlayPause_Click(object sender, EventArgs e)
		{
			ButtonPlayPause_Click(null, null);
		}
		private void ThumbButtonInfoNext_Click(object sender, EventArgs e)
		{
			ButtonNextPrevious_Click(new Button { Name = "ButtonNext" }, null);
		}

		private void Window_Deactivated(object sender, EventArgs e)
		{
			WindowBorder.BorderBrush = Brushes.SecondaryTextBrush;
		}
		private void Window_Activated(object sender, EventArgs e)
		{
			WindowBorder.BorderBrush = Brushes.AccentBrush;
		}
		private void Window_Closed(object sender, EventArgs e)
		{
			Player.Dispose();
		}
	}
}
