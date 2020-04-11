using System.Windows;
using System.Windows.Media;

namespace MinimalistMusicPlayer.Utility
{
	// custom solid color brushes
	public static class Brushes
	{
		public static SolidColorBrush PrimaryBrush = (SolidColorBrush)Application.Current.Resources["PrimaryBrush"];
		public static SolidColorBrush AccentBrush = (SolidColorBrush)Application.Current.Resources["AccentBrush"];
		public static SolidColorBrush PrimaryTextBrush = (SolidColorBrush)Application.Current.Resources["PrimaryTextBrush"];
		public static SolidColorBrush SecondaryTextBrush = (SolidColorBrush)Application.Current.Resources["SecondaryTextBrush"];
	}
}
