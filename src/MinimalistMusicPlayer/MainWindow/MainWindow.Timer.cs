using MinimalistMusicPlayer.Utility;
using System;
using System.Windows.Threading;

namespace MinimalistMusicPlayer
{
	/// <summary>
	/// Updates the UI with every tick (elapsed time, seek, fadeout of volume, etc)
	/// </summary>
	public partial class MainWindow
	{
		int VolumeSliderFadeCounter = -1;

		private void InitializeTimer()
		{
			DispatcherTimer timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromSeconds(.1)
			};
			timer.Tick += TimerTick;
			timer.Start();
		}

		private void TimerTick(object sender, EventArgs e)
		{
			// Seek position
			//
			if (Player.CurrentTrack != null)
			{
				SliderSeek.Value = Player.CurrentPosition.TotalSeconds; // Seek bar
				TaskbarItemInfo.ProgressValue = Player.CurrentPosition.TotalSeconds / SliderSeek.Maximum; // taskbar progress icon
				LabelSeekTime.Content = Player.CurrentPositionString;
			}
			//
			// Volume slider fade away
			//
			// mouse has left the Main Grid - increment fade counter
			if (VolumeSliderFadeCounter >= 0) VolumeSliderFadeCounter++;

			// animate to transparent if counter reaches 10
			if (VolumeSliderFadeCounter >= 10 && SliderVolume.Opacity == 1) SliderVolume.AnimateOpacity(Const.OpacityLevel.Transparent, .3);
		}
	}
}
