using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MinimalistMusicPlayer.Utility
{
	// styles class.
	// used to set styles programmatically for toggle buttons (e.g. playlist, stay-on-top buttons)
	public static class Styles
	{
		public static readonly Style BackgroundButtonStyle = (Style)Application.Current.Resources["BackgroundButtonStyle"];
		public static readonly Style AlphaButtonStyle = (Style)Application.Current.Resources["AlphaButtonStyle"];
		public static readonly Style ExtendedButtonStyle = (Style)Application.Current.Resources["ExtendedButtonStyle"];
		public static readonly Style ExtendedButtonMaskStyle = (Style)Application.Current.Resources["ExtendedButtonMaskStyle"];
	}

	public static class Brushes
	{
		public static readonly SolidColorBrush PrimaryBrush = (SolidColorBrush)Application.Current.Resources["PrimaryBrush"];
		public static readonly SolidColorBrush AccentBrush = (SolidColorBrush)Application.Current.Resources["AccentBrush"];
		public static readonly SolidColorBrush PrimaryTextBrush = (SolidColorBrush)Application.Current.Resources["PrimaryTextBrush"];
		public static readonly SolidColorBrush SecondaryTextBrush = (SolidColorBrush)Application.Current.Resources["SecondaryTextBrush"];
		public static readonly SolidColorBrush PrimaryHoverBrush = (SolidColorBrush)Application.Current.Resources["PrimaryHoverBrush"];
		public static readonly SolidColorBrush HighlightBrush = (SolidColorBrush)Application.Current.Resources["HighlightBrush"];

		public static readonly SolidColorBrush TransparentBrush = (SolidColorBrush)Application.Current.Resources["TransparentBrush"];
	}

	// programmatically-assigned icons are newed-up here
	// sorry about that initializer syntax.
	public static class Icons
	{
		public static readonly Path Play = new Path()
		{
			Data = (GeometryGroup)Application.Current.Resources["Play"],
			Stroke = Brushes.PrimaryTextBrush,
			StrokeThickness = 1.5,
			Margin = new Thickness(4),
			Stretch = Stretch.Uniform
		};
		public static readonly Path Pause = new Path()
		{
			Data = (GeometryGroup)Application.Current.Resources["Pause"],
			Stroke = Brushes.PrimaryTextBrush,
			StrokeThickness = 1.5,
			Margin = new Thickness(7, 5, 7, 5),
			Stretch = Stretch.Uniform
		};

		public static readonly Path VolumeMute = new Path()
		{
			Data = (GeometryGroup)Application.Current.Resources["VolumeMute"],
			Stroke = Brushes.PrimaryTextBrush,
			StrokeThickness = 1.5,
			Margin = new Thickness(2),
			Stretch = Stretch.Uniform

		};

		public static readonly Path VolumeLow = new Path()
		{
			Data = (GeometryGroup)Application.Current.Resources["VolumeLow"],
			Stroke = Brushes.PrimaryTextBrush,
			StrokeThickness = 1.5,
			Margin = new Thickness(2),
			Stretch = Stretch.Uniform
		};

		public static readonly Path VolumeHigh = new Path()
		{
			Data = (GeometryGroup)Application.Current.Resources["VolumeHigh"],
			Stroke = Brushes.PrimaryTextBrush,
			StrokeThickness = 1.5,
			Margin = new Thickness(2),
			Stretch = Stretch.Uniform
		};

		public static readonly Path RepeatOne = new Path()
		{
			Data = (GeometryGroup)Application.Current.Resources["RepeatOne"],
			Stroke = Brushes.PrimaryTextBrush,
			StrokeThickness = 1.5,
			Margin = new Thickness(1),
			Stretch = Stretch.Uniform
		};

		public static readonly Path Repeat = new Path()
		{
			Data = (GeometryGroup)Application.Current.Resources["Repeat"],
			Stroke = Brushes.PrimaryTextBrush,
			StrokeThickness = 1.5,
			Margin = new Thickness(1),
			Stretch = Stretch.Uniform
		};

		public static readonly VisualBrush Directory = new VisualBrush()
		{
			Visual = new Path()
			{
				Data = (GeometryGroup)Application.Current.Resources["Directory"],
				Stroke = Brushes.PrimaryTextBrush,
				StrokeThickness = 1.5
			},
			Stretch = Stretch.None
		};
		public static readonly VisualBrush Drive = new VisualBrush()
		{
			Visual = new Path()
			{
				Data = (GeometryGroup)Application.Current.Resources["Drive"],
				Stroke = Brushes.PrimaryTextBrush,
				StrokeThickness = 1.5
			},
			Stretch = Stretch.None
		};
		public static readonly VisualBrush Media = new VisualBrush()
		{
			Visual = new Path()
			{
				Data = (GeometryGroup)Application.Current.Resources["Media"],
				Stroke = Brushes.PrimaryTextBrush,
				StrokeThickness = 1.5
			},
			Stretch = Stretch.None
		};
		public static readonly VisualBrush MediaPlaylist = new VisualBrush()
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

		public static readonly ImageSource ThumbnailPlay = (DrawingImage)Application.Current.Resources["ThumbnailPlay"];
		public static readonly ImageSource ThumbnailPause = (DrawingImage)Application.Current.Resources["ThumbnailPause"];
	}
}
