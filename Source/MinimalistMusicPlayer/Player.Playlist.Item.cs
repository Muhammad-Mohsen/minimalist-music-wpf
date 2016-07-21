using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WMPLib;

namespace MinimalistMusicPlayer
{
	/// <summary>
	/// 
	/// </summary>
	public class PlaylistItem : Button
	{
		public int TrackIndex;

		private static PlaylistItem OldItem; // holds a reference to the previously-selected playlist item if any.
		//
		// Constructor
		//
		public PlaylistItem(IWMPMedia track, int trackIndex, bool isSelected)
		{
			TrackIndex = trackIndex;

			Grid contentGrid = new Grid()
			{
				Width = 430
			};
			
			Label labelTrackTitle = CreateTitleLabel(track.name);
			contentGrid.Children.Add(labelTrackTitle);

			Label labelTrackDuration = CreateDurationLabel(track.durationString);
			contentGrid.Children.Add(labelTrackDuration);

			Style = Application.Current.Resources["PlaylistButton"] as Style;
			Margin = new Thickness(0, 3, 0, 0);
			BorderBrush = null;

			Content = contentGrid;

			if (isSelected)
				SelectPlaylistItem(this);
		}

		public static void SelectPlaylistItem(PlaylistItem item)
		{
			// deselect the old item, if applicable
			if (OldItem != null)
				DeselectPlaylistItem(OldItem);

			OldItem = item; // set the OldItem to this one.

			item.BorderBrush = Util.Brushes.BlueBrush;
			
			// embolden the text...sigh!
			Grid contentGrid = (Grid)item.Content;
			((Label)contentGrid.Children[0]).FontWeight = FontWeights.Bold; // title label
			((Label)contentGrid.Children[1]).FontWeight = FontWeights.Bold; // duration label
		}
		private static void DeselectPlaylistItem(PlaylistItem item)
		{
			item.BorderBrush = null;
			Grid contentGrid = (Grid)item.Content;
			((Label)contentGrid.Children[0]).FontWeight = FontWeights.Normal; // title label
			((Label)contentGrid.Children[1]).FontWeight = FontWeights.Normal; // duration label
		}

		// helper that creates a fully-realized title label
		private Label CreateTitleLabel(string title)
		{
			return new Label()
			{
				HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
				Content = title,
				FontSize = 12,
				Foreground = Util.Brushes.WhiteBrush
			};
		}
		// helper that creates a fully-realized duration label
		private Label CreateDurationLabel(string duration)
		{
			return new Label()
			{
				HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
				Content = duration,
				FontSize = 12,
				Foreground = Util.Brushes.LightGreyBrush
			};
		}
	}
}