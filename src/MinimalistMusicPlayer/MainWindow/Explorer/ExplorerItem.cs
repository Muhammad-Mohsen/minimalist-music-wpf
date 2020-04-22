using MinimalistMusicPlayer.Utility;
using System.Windows;
using System.Windows.Controls;

namespace MinimalistMusicPlayer.Explorer
{
	public class ExplorerItem : Button
	{
		public ExplorerItemType ItemType { get; set; }

		public ExplorerItem()
		{
			IsTabStop = false;
			Focusable = false;

			// styles, margins...
			Style = Styles.ButtonRevealStyle;
			Background = Brushes.TransparentBrush;
			Margin = new Thickness(0, 3, 0, 0);
			BorderBrush = Brushes.PrimaryBrush;
		}

		protected Label CreateTitleLabel(string title, System.Windows.Media.Brush foregroundBursh = null)
		{
			if (foregroundBursh == null) foregroundBursh = Brushes.PrimaryTextBrush;

			title = title.Ellipsize(Const.ExplorerItemMaxLength);
			return new Label()
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				Content = title,
				FontSize = 12,
				Foreground = foregroundBursh,
				Margin = new Thickness(Const.ExplorerItemIconWidth, 0, 0, 0)
			};
		}
		protected Button CreateIcon(System.Windows.Media.VisualBrush icon)
		{
			return new Button()
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				Style = Styles.AlphaButtonStyle,
				OpacityMask = icon,
				Background = Brushes.AccentBrush,
				Width = Const.ExplorerItemIconWidth,
				Height = Const.ExplorerItemIconHeight,
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