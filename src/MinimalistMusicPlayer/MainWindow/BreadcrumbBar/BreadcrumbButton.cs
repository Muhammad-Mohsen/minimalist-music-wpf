using MinimalistMusicPlayer.Utility;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MinimalistMusicPlayer
{
	public class BreadcrumbButton : Button
	{
		public DirectoryInfo Dir;

		public BreadcrumbButton(string directory)
		{
			Style = Styles.PlaylistButtonStyle;
			Margin = new Thickness(2, 0, 0, 0);
			Padding = new Thickness(3, 0, 3, 0);
			BorderBrush = Brushes.PrimaryBrush;
			IsTabStop = false;

			Content = directory;
		}

		// adds a crumb button and a separator to the given container
		public static void AddCrumb(StackPanel container, string dir, RoutedEventHandler handler = null)
		{
			// add the breadcrumb button to the breadcrumb bar
			BreadcrumbButton button = new BreadcrumbButton(dir);
			if (handler != null) button.Click += handler;
			container.Children.Add(button);

			// separator
			BreadcrumbButton separatorButton = CreateSeparator();
			container.Children.Add(separatorButton);
		}

		public static BreadcrumbButton CreateSeparator()
		{
			return new BreadcrumbButton(Const.BreadcrumbButtonSeparator)
			{
				IsEnabled = false
			};
		}
	}
}
