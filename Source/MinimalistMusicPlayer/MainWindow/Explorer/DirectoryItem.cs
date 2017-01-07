using MinimalistMusicPlayer.Utility;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MinimalistMusicPlayer.Explorer
{
	public class DirectoryItem : ExplorerItem
	{
		public string Directory { get; set; }

		// Directory item constructor
		public DirectoryItem(string directory)
		{
			IsTabStop = false;

			// set item type
			ItemType = ExplorerItemType.DirectoryItem;

			Directory = directory;

			Grid contentGrid = new Grid()
			{
				Width = Const.ExplorerItemWidth
			};

			Button buttonIcon = CreateDirectoryIcon();
			contentGrid.Children.Add(buttonIcon);

			Label labelTitle = CreateTitleLabel(directory.Split(Const.DirectorySeparators).Last());
			contentGrid.Children.Add(labelTitle);

			Style = Styles.PlaylistButtonStyle;
			Margin = new Thickness(0, 3, 0, 0);
			BorderBrush = null;

			Content = contentGrid;
		}

		// helper that creates a fully-realized title label
		private Label CreateTitleLabel(string title)
		{
			title = title.Ellipsize(Const.ExplorerItemMaxLength);
			return new Label()
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				Content = title,
				FontSize = 12,
				Foreground = Brushes.WhiteBrush,
				Margin = new Thickness(Const.ExplorerItemIconWidth, 0, 0, 0)
			};
		}

		// helper that creates a fully-realized directory icon
		private Button CreateDirectoryIcon()
		{
			return new Button()
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				Style = Styles.AlphaButtonStyle,
				OpacityMask = Icons.Directory,
				Background = Brushes.WhiteBrush,
				Width = Const.ExplorerItemIconWidth,
				Height = Const.ExplorerItemIconHeight,
				Margin = new Thickness(0),
				IsTabStop = false
			};
		}
	}
}
