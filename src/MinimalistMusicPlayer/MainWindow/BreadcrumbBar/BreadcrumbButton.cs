using MinimalistMusicPlayer.Utility;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MinimalistMusicPlayer
{
	public class BreadcrumbButton : ExtendedButton
	{
		public DirectoryInfo Dir { get; set; }

		public BreadcrumbButton(string directory)
		{
			Style = Styles.ExtendedButtonStyle;
			BorderBrush = Brushes.BackgroundBrush;
			Margin = new Thickness(2, 0, 0, 0);
			Padding = new Thickness(3, 0, 3, 0);
			IsTabStop = false;

			Content = directory;
		}

		// adds a crumb button and a separator to the given container
		public static void AddCrumb(StackPanel container, string dir, RoutedEventHandler handler, bool isActive = false)
		{
			// add the breadcrumb button to the breadcrumb bar
			BreadcrumbButton button = new BreadcrumbButton(dir)
			{
				FontWeight = isActive ? FontWeights.Bold : FontWeights.Normal,
				// Foreground = isActive ? Brushes.PrimaryBrush : Brushes.SecondaryBrush
			};

			if (handler != null) button.Click += handler;
			container.Children.Add(button);

			// separator
			if (!isActive) container.Children.Add(CreateSeparator());
		}

		public static BreadcrumbButton CreateSeparator()
		{
			return new BreadcrumbButton(Constant.BreadcrumbButtonSeparator)
			{
				IsEnabled = false,
				Foreground = Brushes.SecondaryBrush
			};
		}
	}
}
