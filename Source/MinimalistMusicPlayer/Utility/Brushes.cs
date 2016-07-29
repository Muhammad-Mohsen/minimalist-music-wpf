using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MinimalistMusicPlayer.Utility
{
	// custom solid color brushes
	public static class Brushes
	{
		public static SolidColorBrush BlueBrush = (SolidColorBrush)Application.Current.Resources["BlueBrush"];
		public static SolidColorBrush WhiteBrush = (SolidColorBrush)Application.Current.Resources["TextBrushWhite"];
		public static SolidColorBrush GreyBrush = (SolidColorBrush)Application.Current.Resources["ActiveButtonBackgroundBrush"];
		public static SolidColorBrush LightGreyBrush = (SolidColorBrush)Application.Current.Resources["DisabledBorderBrush"];
		public static SolidColorBrush BackgroundBrush = (SolidColorBrush)Application.Current.Resources["BackgroundBrush"];
	}
}
