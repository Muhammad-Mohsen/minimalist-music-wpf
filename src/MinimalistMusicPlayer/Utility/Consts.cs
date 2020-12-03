using System;
using System.Windows;

namespace MinimalistMusicPlayer.Utility
{
	public static class Constant
	{
		public static readonly string DefaultMediaDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
		public const string Root = "Computer";
		public const string ExplorerDirectorySetting = "ExplorerDirectory";

		// track info max lengths (for ellipsizing purposes)
		public const int TrackNameMaxLength = 25;
		public const int TrackInfoMaxLength = 40;
		public const int ExplorerItemMaxLength = 60;

		public const double VolumeMid = 0.5; // volume levels
		public const double VolumeIncrement = 0.1; // volume increment

		public const int SeekDivisor = 25;

		// array that contains a list supported music extensions
		public static readonly string[] MediaExtensions = { ".mp3", ".wma", ".wav", ".m4b" };

		public const string BreadcrumbButtonSeparator = "/";

		public static readonly char[] DirectorySeparators = { '/', '\\' };

		public const string ShortFormat = @"mm\:ss";
		public const string LongFormat = @"h\:mm\:ss";

		public const int InvalidIndex = -1;

		public static readonly TimeSpan SmallThreshold = TimeSpan.FromMilliseconds(1000);
		public static readonly TimeSpan LargeThreshold = TimeSpan.FromSeconds(5);

		public static class OpacityLevel
		{
			public const int Opaque = 1;
			public const int Transparent = 0;
		}

		// window heights when the playlist is collapsed/expanded
		public const int CollapsedWindowHeight = 155;
		public const int ExpandedWindowHeight = 615;

		public const int ExplorerItemWidth = 430;
		public const int ExplorerItemIconWidth = 25;
		public const int ExplorerItemIconHeight = 25;

		public const int SliderThumbWidth = 16;

		public static class DrillScale
		{
			public const double In = .85;
			public const double Normal = 1;
			public const double Out = 1.15;
		}

		public static class ExplorerMargin
		{
			public static readonly Thickness CurrentPage = new Thickness(0, 32, 0, 0);
			public static readonly Thickness RightPage = new Thickness(500, 32, -500, 0);
			public static readonly Thickness LeftPage = new Thickness(-500, 32, 500, 0);
		}

		public const double ShowHideDelay = .2; // in seconds
		public const double AsyncDelay = 5; // in millis
		public const double DrillAnimDuration = .2; // in seconds
	}
}
