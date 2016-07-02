using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

namespace MinimalistMusicPlayer
{
	/// <summary>
	/// Contains constants, and dynamically-assigned icons, brushes, and some helpers
	/// </summary>
    public class Util
    {
		//
		// Consts
		//
		// window heights when the playlist is collapsed/expanded
		public const int CollapsedWindowHeight = 155;
		public const int ExpandedWindowHeight = 500;

		private const string ImgBaseUriDevelopment = @"../../img/";
		private const string ImgBaseUriDeployment = @"img/";

		// Uri where the IWMPPlaylist will be created each time the app starts
		public const string PlaylistUri = @"";

		// track info max lengths (for ellipsizing purposes)
		public const int TrackNameMaxLength = 20;
		public const int TrackInfoMaxLength = 40;

		// volume levels
		public const double VolumeMid = 50;
		// volume increment
		public const double VolumeIncrement = 5;

		// returns a friendly time string from a timespan
		public static string GetTimeStringFromTimeSpan(TimeSpan t)
		{
			string s = t.ToString();
			return s.Substring(3, 5);
		}
		public static string GetTimeStringFromPosition(double position, double maxValue, double width)
		{
			double seconds = position * maxValue / width;
			return TimeSpan.FromSeconds(seconds).ToString().Substring(3, 5);
		}
		// validates the extension of a file
		public static bool IsValidMediaFile(string fileUrl)
		{
			string extension = fileUrl.Split('.').Last().ToLower();

			return extension == "mp3" ||
				   extension == "wma" ||
				   extension == "wav";
		}


		// programmatically-assigned icons are newed-up here
		public static class Icons
		{
			private static string BaseUri = ImgBaseUriDevelopment;

			public static ImageBrush Play = new ImageBrush()
			{
				ImageSource = new BitmapImage(new Uri(BaseUri + "play.png", UriKind.Relative)),
				Stretch = Stretch.None
			};

			public static ImageBrush Pause = new ImageBrush()
			{
				ImageSource = new BitmapImage(new Uri(BaseUri + "pause.png", UriKind.Relative)),
				Stretch = Stretch.None
			};

			public static ImageBrush VolumeMute = new ImageBrush()
			{
				ImageSource = new BitmapImage(new Uri(BaseUri + "volumeMute.png", UriKind.Relative)),
				Stretch = Stretch.None
			};

			public static ImageBrush VolumeLow = new ImageBrush()
			{
				ImageSource = new BitmapImage(new Uri(BaseUri + "volumeLow.png", UriKind.Relative)),
				Stretch = Stretch.None
			};

			public static ImageBrush VolumeHigh = new ImageBrush()
			{
				ImageSource = new BitmapImage(new Uri(BaseUri + "volumeHigh.png", UriKind.Relative)),
				Stretch = Stretch.None
			};

			public static ImageBrush RepeatOne = new ImageBrush()
			{
				ImageSource = new BitmapImage(new Uri(BaseUri + "ReOne.png", UriKind.Relative)),
				Stretch = Stretch.None
			};

			public static ImageBrush Repeat = new ImageBrush()
			{
				ImageSource = new BitmapImage(new Uri(BaseUri + "Re.png", UriKind.Relative)),
				Stretch = Stretch.None
			};
		}

		// custom solid color brushes
		public static class Brushes
		{
			public static SolidColorBrush BlueBrush = (SolidColorBrush)Application.Current.Resources["BlueBrush"];
			public static SolidColorBrush WhiteBrush = (SolidColorBrush)Application.Current.Resources["TextBrushWhite"];
			public static SolidColorBrush GreyBrush = (SolidColorBrush)Application.Current.Resources["ActiveButtonBackgroundBrush"];
			public static SolidColorBrush LightGreyBrush = (SolidColorBrush)Application.Current.Resources["DisabledBorderBrush"];
			public static SolidColorBrush BackgroundBrush = (SolidColorBrush)Application.Current.Resources["BackgroundBrush"];
		}

		// styles class.
		// used to set styles programmatically for toggle buttons (e.g. playlist, stay-on-top buttons)
		public static class Styles
		{
			public static Style BackgroundButtonToggleStyle = (Style)Application.Current.Resources["BackGroundButtonToggleStyle"];
			public static Style BackgroundButtonStyle = (Style)Application.Current.Resources["BackGroundButtonStyle"];

			public static Style AlphaButtonToggleStyle = (Style)Application.Current.Resources["AlphaButtonToggleStyle"];
			public static Style AlphaButtonStyle = (Style)Application.Current.Resources["AlphaButtonStyle"];
		}
	}

	/// <summary>
	/// Playlist repeat modes
	/// </summary>
	public enum RepeatMode
	{
		Invalid = 0,
		NoRepeat,
		RepeatOne,
		Repeat
	}

}