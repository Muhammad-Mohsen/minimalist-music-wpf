﻿using MinimalistMusicPlayer.Utility;
using System;
using System.Windows;
using System.Windows.Threading;

namespace MinimalistMusicPlayer
{
	/// <summary>
	/// Updates the UI with every tick (elapsed time, seek, fadeout of volume, etc)
	/// </summary>
	public partial class MainWindow : Window
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
			if (Player.CurrentMedia != null)
			{
				SliderSeek.Value = Player.Controls.currentPosition; // Seek bar
				TaskbarItemInfo.ProgressValue = Player.Controls.currentPosition / SliderSeek.Maximum; // taskbar progress icon

				string currentPosition = Player.Controls.currentPositionString;
				if (string.IsNullOrWhiteSpace(currentPosition))
					currentPosition = "00:00";

				LabelSeekTime.Content = currentPosition;
			}
			//
			// Volume slider fade away
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