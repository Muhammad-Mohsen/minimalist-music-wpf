using MinimalistMusicPlayer.Utility;
using System.Windows;
using System.Windows.Controls;

namespace MinimalistMusicPlayer
{
	public class BreadcrumbButton : Button
	{
		public BreadcrumbButton(string directory)
		{
			Style = Styles.PlaylistButtonStyle;
			Margin = new Thickness(2, 0, 0, 0);
			Padding = new Thickness(3, 0, 3, 0);
			BorderBrush = null;
			IsTabStop = false;

			Content = directory;
		}
	}
}
