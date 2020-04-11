using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MinimalistMusicPlayer.Utility
{
	// programmatically-assigned icons are newed-up here
	// sorry about that initializer syntax.
	public static class Icons
	{
		public static VisualBrush Play = new VisualBrush() // VisualBrush is used to get that GeometryGroup resource
		{
			Visual = new Path() // Remember: Geometry objects can't render themselves; They need Shape objects to render them.
			{
				Data = (GeometryGroup)Application.Current.Resources["Play"],
				Stroke = Brushes.PrimaryTextBrush,
				StrokeThickness = 1.5
			},
			Stretch = Stretch.None
		};

		public static VisualBrush Pause = new VisualBrush()
		{
			Visual = new Path()
			{
				Data = (GeometryGroup)Application.Current.Resources["Pause"],
				Stroke = Brushes.PrimaryTextBrush,
				StrokeThickness = 1.5
			},
			Stretch = Stretch.None
		};

		public static VisualBrush VolumeMute = new VisualBrush()
		{
			Visual = new Path()
			{
				Data = (GeometryGroup)Application.Current.Resources["VolumeMute"],
				Stroke = Brushes.PrimaryTextBrush,
				StrokeThickness = 1.5

			},
			Stretch = Stretch.None
		};

		public static VisualBrush VolumeLow = new VisualBrush()
		{
			Visual = new Path()
			{
				Data = (GeometryGroup)Application.Current.Resources["VolumeLow"],
				Stroke = Brushes.PrimaryTextBrush,
				StrokeThickness = 1.5
			},
			Stretch = Stretch.None
		};

		public static VisualBrush VolumeHigh = new VisualBrush()
		{
			Visual = new Path()
			{
				Data = (GeometryGroup)Application.Current.Resources["VolumeHigh"],
				Stroke = Brushes.PrimaryTextBrush,
				StrokeThickness = 1.5
			},
			Stretch = Stretch.None
		};

		public static VisualBrush RepeatOne = new VisualBrush()
		{
			Visual = new Path()
			{
				Data = (GeometryGroup)Application.Current.Resources["RepeatOne"],
				Stroke = Brushes.PrimaryTextBrush,
				StrokeThickness = 1.5
			},
			Stretch = Stretch.None
		};

		public static VisualBrush Repeat = new VisualBrush()
		{
			Visual = new Path()
			{
				Data = (GeometryGroup)Application.Current.Resources["Repeat"],
				Stroke = Brushes.PrimaryTextBrush,
				StrokeThickness = 1.5
			},
			Stretch = Stretch.None
		};

		public static VisualBrush Up = new VisualBrush()
		{
			Visual = new Path()
			{
				Data = (GeometryGroup)Application.Current.Resources["Up"],
				Fill = Brushes.PrimaryTextBrush
			},
			Stretch = Stretch.None
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
