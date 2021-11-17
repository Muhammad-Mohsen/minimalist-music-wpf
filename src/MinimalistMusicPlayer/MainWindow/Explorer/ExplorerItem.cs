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
			BorderBrush = Brushes.BackgroundBrush;
			Background = Brushes.TransparentBrush;
			Margin = new Thickness(0, 3, 0, 0);
		}

		protected static TextBlock CreateTitleLabel(string title, System.Windows.Media.Brush foregroundBursh = null)
		{
			if (foregroundBursh == null) foregroundBursh = Brushes.PrimaryBrush;

			return new TextBlock()
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				Text = title,
				TextTrimming = TextTrimming.CharacterEllipsis,

				FontSize = 12,
				Foreground = foregroundBursh,
				Margin = new Thickness(Constant.ExplorerItemIconSize + Constant.ExplorerItemTextSpacing, Constant.ExplorerItemTextSpacing, 0, Constant.ExplorerItemTextSpacing),
				Width = Constant.ExplorerItemTextWidth
			};
		}
		protected static ExtendedButton CreateIcon(System.Windows.Media.VisualBrush icon)
		{
			var itemIcon =  new ExtendedButton()
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				// Style = Styles.ExtendedButtonStyle,
				OpacityMask = icon,
				Width = Constant.ExplorerItemIconSize,
				Height = Constant.ExplorerItemIconSize,
				Background = Brushes.SecondaryBrush,
				Margin = new Thickness(-2, 0, 0, 0),
				IsTabStop = false,
				Focusable = false,
			};

			return itemIcon;
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