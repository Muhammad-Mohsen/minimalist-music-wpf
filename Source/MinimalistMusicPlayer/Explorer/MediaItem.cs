using MinimalistMusicPlayer.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MinimalistMusicPlayer.Explorer
{
	public class MediaItem : ExplorerItem
	{
		// specifies the count of marked MediaItems
		public static int MarkedItemCount = 0;

		public bool IsMarked { get; set; } // specifies whether this item will be added to the custom playlist
		public string FullName { get; set; } // specifies item full path

		public Button MediaIcon { get; set; }
		public Label LabelTitle { get; set; }
		public Label LabelDuration { get; set; }

		protected static MediaItem OldItem { get; set; } // holds a reference to the previously-selected item if any.

		// custom event that will be raised whenever MarkedItemCount is changed
		// handler will be attached after constructing the MediaItem (in MediaExplorer)
		public delegate void MarkedItemCountChangeDelegate(object sender, RoutedEventArgs e);
		public event MarkedItemCountChangeDelegate MarkedItemCountChange;

		// constructor
		// using default args instead of doing multiple constructors
		public MediaItem(FileInfo mediaFile, string duration, bool isSelected = false, bool isPlaylistMedia = false)
		{
			IsTabStop = false;

			// set item type
			ItemType = ExplorerItemType.MediaItem;

			FullName = mediaFile.FullName;

			Grid contentGrid = new Grid()
			{
				Width = Const.ExplorerItemWidth
			};

			// icon
			MediaIcon = CreateMediaIcon(isPlaylistMedia);
			MediaIcon.Click += MediaIconButton_Click;
			contentGrid.Children.Add(MediaIcon);
			
			// title
			LabelTitle = CreateTitleLabel(mediaFile.Name);
			contentGrid.Children.Add(LabelTitle);

			// duration
			LabelDuration = CreateDurationLabel(duration);
			contentGrid.Children.Add(LabelDuration);

			// styles, margins...
			Style = Styles.PlaylistButtonStyle;
			Margin = new Thickness(0, 3, 0, 0);
			BorderBrush = null;

			Content = contentGrid;

			if (isSelected)
				Select(this);

			IsMarked = false;
		}
		//
		// UI bits helpers
		//
		// helper that creates a fully-realized title label
		private Label CreateTitleLabel(string title)
		{
			title = title.Ellipsize(Const.ExplorerItemMaxLength);
			return new Label()
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				Content = title,
				FontSize = 12,
				Foreground = Brushes.LightGreyBrush,
				Margin = new Thickness(Const.ExplorerItemIconWidth, 0, 0, 0)
			};
		}
		// helper that creates a fully-realized duration label
		private Label CreateDurationLabel(string duration)
		{
			return new Label()
			{
				HorizontalAlignment = HorizontalAlignment.Right,
				Content = duration,
				FontSize = 12,
				Foreground = Brushes.LightGreyBrush
			};
		}
		// helper that creates a media icon
		private Button CreateMediaIcon(bool isPlaylistItem)
		{
			var icon = isPlaylistItem == true ? Icons.MediaPlaylist : Icons.Media;

			return new Button()
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				Style = Styles.AlphaButtonStyle,
				OpacityMask = icon,
				Background = Brushes.WhiteBrush,
				Width = Const.ExplorerItemIconWidth,
				Height = Const.ExplorerItemIconHeight,
				Margin = new Thickness(0),
				IsTabStop = false
			};
		}
		private void MediaIconButton_Click(object sender, RoutedEventArgs e)
		{
			Button mediaIcon = (Button)sender;

			IsMarked = !IsMarked;
			MarkedItemCount = IsMarked ? MarkedItemCount + 1 : MarkedItemCount - 1;

			MarkMediaIcon(mediaIcon, IsMarked);

			// raise the MarkedItemCountChange event (to show/hide the PlaySelected button)
			MarkedItemCountChange(this, new RoutedEventArgs());
		}

		// marks the media icon
		public async void MarkMediaIcon(Button mediaIcon, bool isMarked)
		{
			if (isMarked)
			{
				mediaIcon.Style = Styles.AlphaButtonToggleStyle;
				mediaIcon.Background = Brushes.BlueBrush;
			}

			else
			{
				mediaIcon.Style = Styles.AlphaButtonStyle;

				await Task.Delay(200);
				mediaIcon.Background = Brushes.WhiteBrush;
			}
		}
		// changes the media icon from media playlist to media and back
		// will be used when changing the actual playlist
		public void SetMediaIcon(bool isPlaylistItem)
		{
			var icon = isPlaylistItem == true ? Icons.MediaPlaylist : Icons.Media;
			MediaIcon.OpacityMask = icon;
			MediaIcon.Background = Brushes.WhiteBrush;
		}

		public void SetTitleLabelForeground(bool isPlaylistItem)
		{
			var foregroundColor = isPlaylistItem == true ? Brushes.WhiteBrush : Brushes.LightGreyBrush;
			LabelTitle.Foreground = foregroundColor;
		}
		
		public static void Select(MediaItem item)
		{
			// deselect the old item, if applicable
			if (OldItem != null)
				OldItem.Deselect();

			OldItem = item; // set the OldItem to this one.

			// check due to item being sometimes NULL if a directory change was made before the WindowsMediaPlayer gets a chance to play the item
			if (item != null)
			{
				item.BorderBrush = Brushes.BlueBrush;

				// embolden the text
				item.LabelTitle.FontWeight = FontWeights.Bold;
				item.LabelDuration.FontWeight = FontWeights.Bold;
			}

		}
		protected void Deselect()
		{
			BorderBrush = null;
			Grid contentGrid = (Grid)Content;
			LabelTitle.FontWeight = FontWeights.Normal;
			LabelDuration.FontWeight = FontWeights.Normal;
		}
	}
}