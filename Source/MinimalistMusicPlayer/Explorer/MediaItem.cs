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
			Button buttonIcon = CreateMediaIcon(isPlaylistMedia);
			buttonIcon.Click += MediaIconButton_Click;
			contentGrid.Children.Add(buttonIcon);
			
			// title
			Label labelTitle = CreateTitleLabel(mediaFile.Name);
			contentGrid.Children.Add(labelTitle);

			// duration
			Label labelTrackDuration = CreateDurationLabel(duration);
			contentGrid.Children.Add(labelTrackDuration);

			// styles, margins...
			Style = Styles.PlaylistButtonStyle;
			Margin = new Thickness(0, 3, 0, 0);
			BorderBrush = null;

			Content = contentGrid;

			if (isSelected)
				SelectMediaItem(this);

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
				Foreground = Brushes.WhiteBrush,
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
		private async void MarkMediaIcon(Button mediaIcon, bool isMarked)
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
			Grid contentGrid = (Grid)Content;
			Button iconButton = (Button)contentGrid.Children[0];

			var icon = isPlaylistItem == true ? Icons.MediaPlaylist : Icons.Media;
			iconButton.OpacityMask = icon;
			iconButton.Background = Brushes.WhiteBrush;
		}
		
		public static void SelectMediaItem(MediaItem item)
		{
			// deselect the old item, if applicable
			if (OldItem != null)
				DeselectMediaItem(OldItem);

			OldItem = item; // set the OldItem to this one.

			// check due to item being sometimes NULL if a directory change was made before the WindowsMediaPlayer gets a chance to play the item
			if (item != null)
			{
				item.BorderBrush = Brushes.BlueBrush;

				// embolden the text...sigh!
				Grid contentGrid = (Grid)item.Content;
				((Label)contentGrid.Children[1]).FontWeight = FontWeights.Bold; // title label
				((Label)contentGrid.Children[2]).FontWeight = FontWeights.Bold; // duration label
			}

		}
		protected static void DeselectMediaItem(MediaItem item)
		{
			item.BorderBrush = null;
			Grid contentGrid = (Grid)item.Content;
			((Label)contentGrid.Children[1]).FontWeight = FontWeights.Normal; // title label
			((Label)contentGrid.Children[2]).FontWeight = FontWeights.Normal; // duration label
		}
	}
}