using MinimalistMusicPlayer.Utility;
using System.Windows;
using System.Windows.Controls;

namespace MinimalistMusicPlayer.Explorer
{
	public class ExplorerItem : ExtendedButton
	{
		public ExplorerItemType ItemType { get; set; }

		public ExplorerItem()
		{
			IsTabStop = false;
			Focusable = false;

			// styles, margins...
			Style = Styles.ExtendedButtonStyle;
			BorderBrush = Brushes.PrimaryBrush;
			Background = Brushes.TransparentBrush;
			Margin = new Thickness(0, 3, 0, 0);
		}

		protected static Label CreateTitleLabel(string title, System.Windows.Media.Brush foregroundBursh = null)
		{
			if (foregroundBursh == null) foregroundBursh = Brushes.PrimaryTextBrush;

			title = title.Ellipsize(Constant.ExplorerItemMaxLength);
			return new Label()
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				Content = title,
				FontSize = 12,
				Foreground = foregroundBursh,
				Margin = new Thickness(Constant.ExplorerItemIconWidth, 0, 0, 0)
			};
		}
		protected static ExtendedButton CreateIcon(System.Windows.Media.VisualBrush icon)
		{
			return new ExtendedButton()
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				Style = Styles.ExtendedButtonMaskStyle,
				OpacityMask = icon,
				Background = Brushes.AccentBrush,
				Width = Constant.ExplorerItemIconWidth,
				Height = Constant.ExplorerItemIconHeight,
				Margin = new Thickness(0),
				IsTabStop = false,
				Focusable = false
			};
		}
	}

	public enum ExplorerItemType
	{
		Invalid = 0,
		DirectoryItem,
		MediaItem,
		DriveItem
	}
}