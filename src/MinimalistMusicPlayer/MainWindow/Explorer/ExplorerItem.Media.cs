using MinimalistMusicPlayer.Media;
using MinimalistMusicPlayer.Utility;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MinimalistMusicPlayer.Explorer
{
	public class MediaItem : ExplorerItem
	{
		public static int MarkedItemCount { get; set; } = 0; // specifies the count of marked MediaItems
		protected static MediaItem OldSelected { get; set; } // holds a reference to the previously-selected item if any.

		public bool IsMarked { get; set; } // specifies whether this item will be added to the custom playlist
		public ExtendedButton MediaIcon { get; set; }
		public TextBlock LabelTitle { get; set; }
		public TextBlock LabelDuration { get; set; }

		public string FullName { get; set; } // specifies item full path
		public MediaFile File { get; set; }

		// custom event that will be raised whenever MarkedItemCount is changed
		// handler will be attached after constructing the MediaItem (in MediaExplorer)
		public delegate void MarkedItemCountChangeEventHandler(object sender, RoutedEventArgs e);
		public event MarkedItemCountChangeEventHandler MarkedItemCountChange;

		// constructor
		// using default args instead of doing multiple constructors
		public MediaItem(MediaFile mediaFile, MediaItemStyle mediaItemStyle, bool isSelected = false)
		{
			// set item type
			ItemType = ExplorerItemType.MediaItem;

			File = mediaFile;
			FullName = mediaFile.FullName;

			Grid contentGrid = new Grid()
			{
				Width = Constant.ExplorerItemWidth
			};

			// icon
			MediaIcon = CreateIcon(mediaItemStyle == MediaItemStyle.IconHighlighted ? Icons.MediaPlaylist : Icons.Media);
			MediaIcon.Click += MediaIconButton_Click;
			contentGrid.Children.Add(MediaIcon);

			// title
			LabelTitle = CreateTitleLabel(mediaFile.Name, mediaItemStyle != MediaItemStyle.Normal ? Brushes.PrimaryTextBrush : Brushes.SecondaryTextBrush);
			contentGrid.Children.Add(LabelTitle);

			// duration
			LabelDuration = CreateDurationLabel(mediaFile);
			contentGrid.Children.Add(LabelDuration);

			Content = contentGrid;

			if (isSelected) Select(this);
			IsMarked = false;

			LayoutTransform = new System.Windows.Media.ScaleTransform(); // used for filter animations
		}

		private void MediaIconButton_Click(object sender, RoutedEventArgs e)
		{
			IsMarked = !IsMarked;
			MarkMediaIcon((ExtendedButton)sender, IsMarked);
			MarkedItemCount = IsMarked ? MarkedItemCount + 1 : MarkedItemCount - 1;
			MarkedItemCountChange(this, new RoutedEventArgs()); // raise the MarkedItemCountChange event (to show/hide the PlaySelected button)
		}
		//
		// UI helpers
		//
		// helper that creates a fully-realized duration label
		private TextBlock CreateDurationLabel(MediaFile mediaFile)
		{
			return new TextBlock()
			{
				HorizontalAlignment = HorizontalAlignment.Right,
				Text = mediaFile.DurationString,
				FontSize = 12,
				Foreground = Brushes.SecondaryTextBrush,
				Margin = new Thickness(0, Constant.ExplorerItemTextSpacing, Constant.ExplorerItemTextSpacing, 0)
			};
		}

		// marks the media icon
		public static async void MarkMediaIcon(ExtendedButton mediaIcon, bool isMarked)
		{
			mediaIcon.IsSelected = isMarked;
			if (isMarked)
			{
				await Task.Delay(200).ConfigureAwait(true);
				mediaIcon.Background = Brushes.SecondaryTextBrush;
			}
			else
			{
				await Task.Delay(200).ConfigureAwait(true);
				mediaIcon.Background = Brushes.AccentBrush;
			}
		}

		// changes the media icon from media playlist to media and back - used when changing the actual playlist
		public async void SetMediaIcon(bool isPlaylistItem)
		{
			var icon = isPlaylistItem == true ? Icons.MediaPlaylist : Icons.Media;
			MediaIcon.OpacityMask = icon;
			MediaIcon.IsSelected = isPlaylistItem;
			if (isPlaylistItem)
			{
				await Task.Delay(200).ConfigureAwait(true);
				MediaIcon.Background = Brushes.SecondaryTextBrush;
			}
			else
			{
				await Task.Delay(200).ConfigureAwait(true);
				MediaIcon.Background = Brushes.AccentBrush;
			}
		}

		public void SetTitleLabelForeground(bool isPlaylistItem)
		{
			var foregroundColor = isPlaylistItem == true ? Brushes.PrimaryTextBrush : Brushes.SecondaryTextBrush;
			LabelTitle.Foreground = foregroundColor;
		}

		public static void Select(MediaItem item)
		{
			OldSelected?.ToggleSelectionUi(false); // deselect the old item, if applicable

			OldSelected = item; // set the OldItem to this one.

			// check due to item being sometimes NULL if a directory change was made before the WindowsMediaPlayer gets a chance to play the item
			item?.ToggleSelectionUi(true);
		}
		protected void ToggleSelectionUi(bool select)
		{
			IsSelected = select;

			LabelTitle.FontWeight = select ? FontWeights.Bold : FontWeights.Normal;
			LabelDuration.FontWeight = select ? FontWeights.Bold : FontWeights.Normal;
			BorderBrush = select ? Brushes.AccentBrush : Brushes.PrimaryBrush;
		}

		public void Toggle(bool show)
		{
			// Visibility = show ? Visibility.Visible : Visibility.Collapsed;
			//this.AnimateLayoutScaleY(show ? 1 : 0, Constant.DrillAnimDuration);
			if (show) Visibility = Visibility.Visible;
			this.AnimateOpacity(show ? Constant.OpacityLevel.Opaque : Constant.OpacityLevel.Transparent, Constant.DrillAnimDuration, (sender, args) =>
			{
				if (!show) Visibility = Visibility.Collapsed;
			});
		}
	}

	public enum MediaItemStyle
	{
		Normal,
		Highlighted,
		IconHighlighted
	}
}