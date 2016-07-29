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
				Fill = Brushes.WhiteBrush
			},
			Stretch = Stretch.None
		};

		public static VisualBrush Pause = new VisualBrush()
		{
			Visual = new Path()
			{
				Data = (GeometryGroup)Application.Current.Resources["Pause"],
				Fill = Brushes.WhiteBrush
			},
			Stretch = Stretch.None
		};

		public static VisualBrush VolumeMute = new VisualBrush()
		{
			Visual = new Path()
			{
				Data = (GeometryGroup)Application.Current.Resources["VolumeMute"],
				Fill = Brushes.WhiteBrush
			},
			Stretch = Stretch.None
		};

		public static VisualBrush VolumeLow = new VisualBrush()
		{
			Visual = new Path()
			{
				Data = (GeometryGroup)Application.Current.Resources["VolumeLow"],
				Fill = Brushes.WhiteBrush
			},
			Stretch = Stretch.None
		};

		public static VisualBrush VolumeHigh = new VisualBrush()
		{
			Visual = new Path()
			{
				Data = (GeometryGroup)Application.Current.Resources["VolumeHigh"],
				Fill = Brushes.WhiteBrush
			},
			Stretch = Stretch.None
		};

		public static VisualBrush RepeatOne = new VisualBrush()
		{
			Visual = new Path()
			{
				Data = (GeometryGroup)Application.Current.Resources["RepeatOne"],
				Fill = Brushes.WhiteBrush
			},
			Stretch = Stretch.None
		};

		public static VisualBrush Repeat = new VisualBrush()
		{
			Visual = new Path()
			{
				Data = (GeometryGroup)Application.Current.Resources["Repeat"],
				Fill = Brushes.WhiteBrush
			},
			Stretch = Stretch.None
		};
	}
}
