using MinimalistMusicPlayer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MinimalistMusicPlayer.Explorer
{
	public class DriveItem : ExplorerItem
	{
		public string Directory { get; set; }
		// Drive item constructor
		public DriveItem(string driveName)
		{
			IsTabStop = false;

			ItemType = ExplorerItemType.DriveItem;

			Directory = driveName;
			// set item type

			Grid contentGrid = new Grid()
			{
				Width = Const.ExplorerItemWidth
			};

			Button buttonIcon = CreateDriveIcon();
			contentGrid.Children.Add(buttonIcon);

			Label labelTitle = CreateTitleLabel(driveName);
			contentGrid.Children.Add(labelTitle);

			Style = Styles.PlaylistButtonStyle;
			Margin = new Thickness(0, 3, 0, 0);
			BorderBrush = null;

			Content = contentGrid;
		}

		// helper that creates a fully-realized title label
		private Label CreateTitleLabel(string title)
		{
			// replace the back slash with a forward slash. That simple
			title = title.Replace('\\', '/');
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

		// helper that creates a fully-realized drive icon
		private Button CreateDriveIcon()
		{
			return new Button()
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				Style = Styles.AlphaButtonStyle,
				OpacityMask = Icons.Drive,
				Background = Brushes.WhiteBrush,
				Width = Const.ExplorerItemIconWidth,
				Height = Const.ExplorerItemIconHeight,
				Margin = new Thickness(0),
				IsTabStop = false
			};
		}
	}
}
