using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WMPLib;

namespace MinimalistMusicPlayer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {
		WmpPlayer Player;

		// indicates whether the seek slider value is being manually changed
		bool IsSeeking;

		// move window fields
		Point InitialPosition;
		bool CanMoveWindow;

        public MainWindow()
        {
			// needs to be defined before InitializeComponent, believe it or not!!
			// see ValueChanged events
			// Player.Position is accessed during InitializeComponent.
			Player = new WmpPlayer();
			// hook up a handler for PlayState change event
			Player.Player.PlayStateChange += Player_PlayStateChange;
			Player.Player.MediaChange += Player_MediaChange;

			InitializeComponent();

			IsSeeking = false;
			SliderVolume.Opacity = 0;

			// initialize playing icon visibility to hidden
			PlayingIcon.Opacity = 0;
			// start a continuous rotation animation for the playing icon
			Anim.AnimateAngle(PlayingIcon, 0, 360, 2, true);

			// timer to update the seek bar, volume fade, etc.
			DispatcherTimer timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromSeconds(.1);
			timer.Tick += timer_Tick;
			timer.Start();

			// see if the app was started with some command line args
			if (Environment.GetCommandLineArgs().Length > 0)
			{
				Player.AddPlaylistItems(Environment.GetCommandLineArgs());
				AddTracksToPlaylistStackPanel();
				Player.StartPlay(Player.PlaylistIndex);
			}
		}
		//
		// Drag and Drop handlers
		//
		private void StackPanelPlaylist_Drop(object sender, DragEventArgs e)
		{
			string[] sourceStrings = ((string[])e.Data.GetData(DataFormats.FileDrop));

			ClearPlaylistStackPanel();

			Player.AddPlaylistItems(sourceStrings);
			AddTracksToPlaylistStackPanel();
		}
		//
		// Track info grid Events - Cutting corners! To sleepy to do anything decent.
		//
		private void ButtonTrackInfo_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openDialog = new OpenFileDialog();
			openDialog.Title = "Open media";

			openDialog.Multiselect = true;
			openDialog.AddExtension = true;
			openDialog.Filter = "Media files (*.mp3, *.wma, *.wav)|*.mp3; *.wma; *.wav";

			// will only be true if files are selected
			if ((bool)openDialog.ShowDialog())
			{
				Player.ClearPlaylistItems();
				ClearPlaylistStackPanel();
				Player.PlaylistIndex = 0;

				Player.AddPlaylistItems(openDialog.FileNames);
				AddTracksToPlaylistStackPanel();

				Player.StartPlay(Player.PlaylistIndex);
			}
		}

		//
		// Button events
		//
		private void ButtonPlaylist_Click(object sender, RoutedEventArgs e)
		{
			if (Player.IsPlaylistVisible == false) 
			{
				ExpandCollapsePlaylistStackPanel(true);
			}
			else
			{
				ExpandCollapsePlaylistStackPanel(false);
			}
		}
		private void ButtonPlayPause_Click(object sender, RoutedEventArgs e)
		{
			if (Player.CurrentMedia != null)
			{
				if (Player.PlayState == WMPPlayState.wmppsPlaying)
				{
					Player.Pause();
				}
				else
				{
					Player.Resume();
				}
			}
		}
		private void ButtonPrevious_Click(object sender, RoutedEventArgs e)
		{
            if (Player.PlaylistIndex > 0)
				Player.PlaylistIndex--;

			else
				Player.PlaylistIndex = Player.PlaylistCount - 1;

			if (Player.PlaylistCount > 0)
				Player.StartPlay(Player.PlaylistIndex);
		}
		private void ButtonNext_Click(object sender, RoutedEventArgs e)
		{
			if (Player.PlaylistIndex < Player.PlaylistCount - 1)
				Player.PlaylistIndex++;

			else
				Player.PlaylistIndex = 0;

			if (Player.PlaylistCount > 0)
				Player.StartPlay(Player.PlaylistIndex);
		}
		private void ButtonRepeat_Click(object sender, RoutedEventArgs e)
		{
			SetRepeatIcon(Player.RepeatMode);
			Player.SetNewRepeatMode(Player.RepeatMode); // method takes the old repeat mode, and sets the new repeat mode.
		}
		private void ButtonShuffle_Click(object sender, RoutedEventArgs e)
		{
			Player.IsShuffle = !Player.IsShuffle;
			SetShuffleIcon(Player.IsShuffle);
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
			if (IsSeeking)
			{
				Player.Controls.currentPosition = SliderSeek.Value;
			}
		}
		private void SliderSeek_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (Player.CurrentMedia != null)
			{
				double position = e.GetPosition((IInputElement)sender).X;
				ToolTipSeek.HorizontalOffset = position;
				ToolTipSeek.Content = Util.GetTimeStringFromPosition(position, SliderSeek.Maximum, SliderSeek.ActualWidth);
			}
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

			SelectPlaylistItem(Player.PlaylistIndex);

			// play the next track if the current one reaches its end
			if (Player.PlayState == WMPPlayState.wmppsMediaEnded)
			{
				await Task.Delay(100); // work around to ensure that the player actually plays the media. Looks like everything in WPF suffers from racing issues.
				Player.StartPlayNext();
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
		private void Volume_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			VolumeSliderFadeCounter = -1;
			if (SliderVolume.Opacity == 0 && sender.Equals(ButtonVolume)) // only display the volume slider when the mouse enters the button, not the slider itself
				Anim.AnimateOpacity(SliderVolume, 0, 1, .1);
		}
		private void Volume_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			VolumeSliderFadeCounter = 0;
		}
		//
		// Window button events
		//
		// minimize
		private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
		{
			WindowState = System.Windows.WindowState.Minimized;
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
		private void GridMain_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.OriginalSource.Equals(GridMain))
			{
				InitialPosition = e.GetPosition(null);
				CanMoveWindow = true;
			}
		}
		private void GridMain_MouseMove(object sender, MouseEventArgs e)
		{
			if (CanMoveWindow)
			{
				Point currentPosition = e.GetPosition(null);
				// Left and Top are window Properties
				Left = Left + currentPosition.X - InitialPosition.X;
				Top = Top + currentPosition.Y - InitialPosition.Y;
			}
		}
		private void GridMain_MouseUp(object sender, MouseButtonEventArgs e)
		{
			CanMoveWindow = false;
		}
		//
		// shortcut keys
		//
		// half ass-ing it by copying/pasting code from the above events...sadly
		// so far we got play/pause, and open tracks dialog box shortcuts
		private void Window_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Space: // play/pause
					if (Player.CurrentMedia != null)
					{
						if (Player.PlayState == WMPPlayState.wmppsPlaying)
						{
							Player.Pause();
						}
						else
						{
							Player.Resume();
						}
					}
					break;

				case Key.O: // open tracks dialog
					if (Keyboard.Modifiers == ModifierKeys.Control)
					{
						OpenFileDialog OpenDialog = new OpenFileDialog();
						OpenDialog.Title = "Open media";

						OpenDialog.Multiselect = true;
						OpenDialog.AddExtension = true;
						OpenDialog.Filter = "Media files (*.mp3, *.wma, *.wav)|*.mp3; *.wma; *.wav";

						// will only be true if files are selected
						if ((bool)OpenDialog.ShowDialog())
						{
							Player.ClearPlaylistItems();
							ClearPlaylistStackPanel();
							Player.PlaylistIndex = 0;

							foreach (string fileName in OpenDialog.FileNames)
								if (Util.IsValidMediaFile(fileName))
									Player.AddPlaylistItem(fileName);

							AddTracksToPlaylistStackPanel();
							Player.StartPlay(Player.PlaylistIndex);
						}
					}
					break;
			}
		}
		// if the mouse wheel gives a positive delta, increment the volume, else decrement it.
		// pretty straight forward really.
		private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (e.Delta > 0)
				SliderVolume.Value += Util.VolumeIncrement;
			else
				SliderVolume.Value -= Util.VolumeIncrement;
		}

	}
}
