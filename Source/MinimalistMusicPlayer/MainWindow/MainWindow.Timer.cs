using MinimalistMusicPlayer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MinimalistMusicPlayer
{
	/// <summary>
	/// Updates the UI with every tick (elapsed time, seek, fadeout of volume, etc)
	/// </summary>
	public partial class MainWindow : Window
	{
		int VolumeSliderFadeCounter = -1;
		int CurrentTimeLabelFadeCounter = -1;

		private void timer_Tick(object sender, EventArgs e)
		{
			//
			//Seek position
			//
			if (Player.CurrentMedia != null)
			{
				SliderSeek.Value = Player.Controls.currentPosition;
				LabelSeekTime.Content = Player.Controls.currentPositionString;
			}
			//
			//Volume slider fade away
			//
			// increment fade counter
			if (VolumeSliderFadeCounter >= 0) // mouse has left the Main Grid
				VolumeSliderFadeCounter++;

			// animate to transparent if counter reaches 10
			if (VolumeSliderFadeCounter >= 10 && SliderVolume.Opacity == 1)
				Anim.AnimateOpacity(SliderVolume, Const.OpacityLevel.Transparent, .3);
		}
	}
}