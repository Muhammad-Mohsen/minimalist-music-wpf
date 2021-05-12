using MinimalistMusicPlayer.Utility;
using System.Windows.Controls;

namespace MinimalistMusicPlayer.Explorer
{
	public class DriveItem : ExplorerItem
	{
		public string Directory { get; set; }

		// Drive item constructor
		public DriveItem(string driveName)
		{
			ItemType = ExplorerItemType.DriveItem;

			Directory = driveName;
			// set item type

			Grid contentGrid = new Grid()
			{
				Width = Constant.ExplorerItemWidth
			};

			Button buttonIcon = CreateIcon(Icons.Drive);
			contentGrid.Children.Add(buttonIcon);

			TextBlock labelTitle = CreateTitleLabel(driveName.Replace('\\', '/'));
			contentGrid.Children.Add(labelTitle);

			Content = contentGrid;
		}
	}
}
