using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MinimalistMusicPlayer.Utility
{
	// styles class.
	// used to set styles programmatically for toggle buttons (e.g. playlist, stay-on-top buttons)
	public static class Styles
	{
		public static Style BackgroundButtonToggleStyle = (Style)Application.Current.Resources["BackGroundButtonToggleStyle"];
		public static Style BackgroundButtonStyle = (Style)Application.Current.Resources["BackgroundButtonStyle"];

		public static Style ButtonRevealStyle = (Style)Application.Current.Resources["ButtonRevealStyle"];

		public static Style AlphaButtonToggleStyle = (Style)Application.Current.Resources["AlphaButtonToggleStyle"];
		public static Style AlphaButtonStyle = (Style)Application.Current.Resources["AlphaButtonStyle"];

		public static Style PlaylistButtonStyle = (Style)Application.Current.Resources["PlaylistButton"];
	}

	public static class Brushes
	{
		public static SolidColorBrush PrimaryBrush = (SolidColorBrush)Application.Current.Resources["PrimaryBrush"];
		public static SolidColorBrush AccentBrush = (SolidColorBrush)Application.Current.Resources["AccentBrush"];
		public static SolidColorBrush PrimaryTextBrush = (SolidColorBrush)Application.Current.Resources["PrimaryTextBrush"];
		public static SolidColorBrush SecondaryTextBrush = (SolidColorBrush)Application.Current.Resources["SecondaryTextBrush"];
		public static SolidColorBrush PrimaryHoverBrush = (SolidColorBrush)Application.Current.Resources["PrimaryHoverBrush"];
		public static SolidColorBrush HighlightBrush = (SolidColorBrush)Application.Current.Resources["HighlightBrush"];

		public static SolidColorBrush TransparentBrush = (SolidColorBrush)Application.Current.Resources["TransparentBrush"];
	}

	// programmatically-assigned icons are newed-up here
	// sorry about that initializer syntax.
	public static class Icons
	{
		public static Path Play = new Path()
		{
			Data = (GeometryGroup)Application.Current.Resources["Play"],
			Stroke = Brushes.PrimaryTextBrush,
			StrokeThickness = 1.5,
			Margin = new Thickness(5),
			Stretch = Stretch.Uniform
		};
		public static Path Pause = new Path()
		{
			Data = (GeometryGroup)Application.Current.Resources["Pause"],
			Stroke = Brushes.PrimaryTextBrush,
			StrokeThickness = 1.5,
			Margin = new Thickness(8, 5, 8, 5),
			Stretch = Stretch.Uniform
		};

		public static Path VolumeMute = new Path()
		{
			Data = (GeometryGroup)Application.Current.Resources["VolumeMute"],
			Stroke = Brushes.PrimaryTextBrush,
			StrokeThickness = 1.5,
			Margin = new Thickness(3),
			Stretch = Stretch.Uniform

		};

		public static Path VolumeLow = new Path()
		{
			Data = (GeometryGroup)Application.Current.Resources["VolumeLow"],
			Stroke = Brushes.PrimaryTextBrush,
			StrokeThickness = 1.5,
			Margin = new Thickness(3),
			Stretch = Stretch.Uniform
		};

		public static Path VolumeHigh = new Path()
		{
			Data = (GeometryGroup)Application.Current.Resources["VolumeHigh"],
			Stroke = Brushes.PrimaryTextBrush,
			StrokeThickness = 1.5,
			Margin = new Thickness(3),
			Stretch = Stretch.Uniform
		};

		public static Path RepeatOne = new Path()
		{
			Data = (GeometryGroup)Application.Current.Resources["RepeatOne"],
			Stroke = Brushes.PrimaryTextBrush,
			StrokeThickness = 1.5,
			Margin = new Thickness(2),
			Stretch = Stretch.Uniform
		};

		public static Path Repeat = new Path()
		{
			Data = (GeometryGroup)Application.Current.Resources["Repeat"],
			Stroke = Brushes.PrimaryTextBrush,
			StrokeThickness = 1.5,
			Margin = new Thickness(2),
			Stretch = Stretch.Uniform
		};

		public static VisualBrush Directory = new VisualBrush()
		{
			Visual = new Path()
			{
				Data = (GeometryGroup)Application.Current.Resources["Directory"],
				Stroke = Brushes.PrimaryTextBrush,
				StrokeThickness = 1.5
			},
			Stretch = Stretch.None
		};
		public static VisualBrush Drive = new VisualBrush()
		{
			Visual = new Path()
			{
				Data = (GeometryGroup)Application.Current.Resources["Drive"],
				Stroke = Brushes.PrimaryTextBrush,
				StrokeThickness = 1.5
			},
			Stretch = Stretch.None
		};
		public static VisualBrush Media = new VisualBrush()
		{
			Visual = new Path()
			{
				Data = (GeometryGroup)Application.Current.Resources["Media"],
				Stroke = Brushes.PrimaryTextBrush,
				StrokeThickness = 1.5
			},
			Stretch = Stretch.None
		};
		public static VisualBrush MediaPlaylist = new VisualBrush()
		{
			Visual = new Path()
			{
				Data = (GeometryGroup)Application.Current.Resources["MediaPlaylist"],
				Stroke = Brushes.PrimaryTextBrush,
				StrokeThickness = 1.5
			},
			Stretch = Stretch.None,
			RelativeTransform = new TranslateTransform(-.1, 0)
		};

		public static ImageSource ThumbnailPlay = (DrawingImage)Application.Current.Resources["ThumbnailPlay"];
		public static ImageSource ThumbnailPause = (DrawingImage)Application.Current.Resources["ThumbnailPause"];
	}
}
