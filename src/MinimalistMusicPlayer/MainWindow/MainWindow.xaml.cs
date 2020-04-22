using MinimalistMusicPlayer.Player;
using MinimalistMusicPlayer.Utility;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using WMPLib;

namespace MinimalistMusicPlayer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		// needs to be defined before InitializeComponent, believe it or not!!
		// Player.Position is accessed during InitializeComponent (see ValueChanged events)
		public static readonly PlayerWrapper Player = new PlayerWrapper();
		public static PlaylistWrapper Playlist = new PlaylistWrapper();

		bool IsSeeking = false; // indicates whether the seek slider value is being manually changed

		// move window fields
		Point InitialPosition;
		bool CanMoveWindow;

		public MainWindow()
		{
			// hook up a handler for PlayState change event
			Player.InternalPlayer.PlayStateChange += Player_PlayStateChange;
			Player.InternalPlayer.MediaChange += Player_MediaChange;

			InitializeComponent();

			SliderVolume.Opacity = 0;

			PlayingIcon.Opacity = 0; // initialize playing icon visibility to hidden

			Anim.AnimateAngle(PlayingIcon, 0, 360, 2, true); // start a continuous rotation animation for the playing icon

			string savedDirectory = Properties.Settings.Default[Const.ExplorerDirectorySetting].ToString();
			if (Directory.Exists(savedDirectory)) CurrentDirectory = new DirectoryInfo(savedDirectory);
			else CurrentDirectory = new DirectoryInfo(Const.DefaultMediaDirectory);

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
			if (Player.IsPlaylistVisible == false) ExpandCollapsePlaylistStackPanel(true);

			// go to playlist directory if possible
			if (!string.IsNullOrEmpty(Playlist.PlaylistDirectory)) DirectoryChange(new DirectoryInfo(Playlist.PlaylistDirectory));
		}
		//
		// Button events
		//
		private void ButtonPlaylist_Click(object sender, RoutedEventArgs e)
		{
			ExpandCollapsePlaylistStackPanel(Player.IsPlaylistVisible == false);
		}
		private void ButtonPlayPause_Click(object sender, RoutedEventArgs e)
		{
			if (Player.CurrentMedia != null)
			{
				// more sadness
				if (Playlist.Index == Const.InvalidIndex)
				{
					Playlist.Index = 0;
					Player.Play(Playlist.GetItem(Playlist.Index));
				}

				if (Player.PlayState == WMPPlayState.wmppsPlaying)
					Player.Pause();
				else
					Player.Resume();
			}
		}
		private void ButtonPrevious_Click(object sender, RoutedEventArgs e)
		{
			if (Playlist.Index > 0)
				Playlist.Index--;

			else
				Playlist.Index = Playlist.Count - 1;

			if (Playlist.Count > 0)
				Player.Play(Playlist.GetItem(Playlist.Index));
		}
		private void ButtonNext_Click(object sender, RoutedEventArgs e)
		{
			if (Playlist.Index < Playlist.Count - 1)
				Playlist.Index++;

			else
				Playlist.Index = 0;

			if (Playlist.Count > 0)
				Player.Play(Playlist.GetItem(Playlist.Index));
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
			IsSeeking = true;
		}
		private void SliderSeek_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			IsSeeking = false;
		}
		private void SliderSeek_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsSeeking) Player.Controls.currentPosition = SliderSeek.Value;
		}
		private void SliderSeek_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (Player.CurrentMedia == null) return;
			double position = e.GetPosition((IInputElement)sender).X;
			SetSeekTooltip(position, SliderSeek.Maximum, SliderSeek.ActualWidth);
		}
		//
		// Player events
		//
		private void Player_PlayStateChange(int newState)
		{
			SetPlayPauseUiState((WMPPlayState)newState);
		}
		private async void Player_MediaChange(object item)
		{
			SetDurationValues((IWMPMedia)item);
			SetTrackInfo((IWMPMedia)item);

			SelectMediaItemByIndex(Playlist.Index);

			// play the next track if the current one reaches its end
			if (Player.PlayState == WMPPlayState.wmppsMediaEnded)
			{
				await Task.Delay(100); // work around to ensure that the player actually plays the media. Looks like everything in WPF suffers from racing issues.
				Player.Play(Playlist.GetNextItem());
			}
		}
		//
		// Volume slider/button events
		//
		private void ButtonVolume_Click(object sender, RoutedEventArgs e)
		{
			Player.IsMuted = !Player.IsMuted;
			if (Player.IsMuted)
				SliderVolume.Value = 0;
			else
				SliderVolume.Value = Player.Volume;

			SetVolumeIcon(SliderVolume.Value, Player.IsMuted);
		}
		private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			// only set the player volume if it was changed through the slider
			// not the mute button
			if (!Player.IsMuted)
				Player.Volume = (int)SliderVolume.Value;

			SetVolumeIcon(SliderVolume.Value, Player.IsMuted);
		}
		//
		// Volume slider animation events
		//
		private void Volume_MouseEnter(object sender, MouseEventArgs e)
		{
			VolumeSliderFadeCounter = -1;
			if (SliderVolume.Opacity == 0 && sender.Equals(ButtonVolume)) // only display the volume slider when the mouse enters the button, not the slider itself
				Anim.AnimateOpacity(SliderVolume, Const.OpacityLevel.Opaque, .1);
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
		// always on top
		private void ButtonPinToTop_Click(object sender, RoutedEventArgs e)
		{
			Topmost = !Topmost;
			SetPinToTopIcon(Topmost);
		}
		//
		// move window
		//
		// had to be done this way so that if the mouse is pressed from outside the grid,
		// and then it moves within it, it wouldn't fire the event
		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.OriginalSource.GetType() == typeof (Rectangle) && (e.OriginalSource as Rectangle).Name == "noiseLayer")
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
			}
		}
		// if the mouse wheel gives a positive delta, increment the volume, else decrement it. pretty straight forward really.
		private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (e.Delta > 0)
				SliderVolume.Value += Const.VolumeIncrement;
			else
				SliderVolume.Value -= Const.VolumeIncrement;
		}
		//
		// Thumbnail buttons
		//
		private void ThumbButtonInfoPrevious_Click(object sender, EventArgs e)
		{
			if (Playlist.Index > 0)
				Playlist.Index--;
			else
				Playlist.Index = Playlist.Count - 1;

			if (Playlist.Count > 0)
				Player.Play(Playlist.GetItem(Playlist.Index));
		}
		private void ThumbButtonInfoPlayPause_Click(object sender, EventArgs e)
		{
			if (Player.CurrentMedia != null)
			{
				// more sadness
				if (Playlist.Index == Const.InvalidIndex)
				{
					Playlist.Index = 0;
					Player.Play(Playlist.GetItem(Playlist.Index));
				}

				if (Player.PlayState == WMPPlayState.wmppsPlaying)
					Player.Pause();
				else
					Player.Resume();
			}
		}
		private void ThumbButtonInfoNext_Click(object sender, EventArgs e)
		{
			if (Playlist.Index < Playlist.Count - 1) Playlist.Index++;
			else Playlist.Index = 0;

			if (Playlist.Count > 0) Player.Play(Playlist.GetItem(Playlist.Index));
		}
	}
}
