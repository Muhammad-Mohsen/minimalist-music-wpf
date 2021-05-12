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

		protected static TextBlock CreateTitleLabel(string title, System.Windows.Media.Brush foregroundBursh = null)
		{
			if (foregroundBursh == null) foregroundBursh = Brushes.PrimaryTextBrush;

			return new TextBlock()
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				Text = title,
				TextTrimming = TextTrimming.CharacterEllipsis,

				FontSize = 12,
				Foreground = foregroundBursh,
				Margin = new Thickness(Constant.ExplorerItemIconWidth + Constant.ExplorerItemTextSpacing, Constant.ExplorerItemTextSpacing, 0, Constant.ExplorerItemTextSpacing),
				Width = Constant.ExplorerItemTextWidth
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