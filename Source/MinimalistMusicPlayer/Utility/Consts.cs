using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalistMusicPlayer.Utility
{
	public class Const
	{
		// window heights when the playlist is collapsed/expanded
		public const int CollapsedWindowHeight = 155;
		public const int ExpandedWindowHeight = 600;

		public const int ExplorerItemWidth = 430;
		public const int ExplorerItemIconWidth = 25;
		public const int ExplorerItemIconHeight = 25;

		// Uri where the IWMPPlaylist will be created each time the app starts
		public const string PlaylistUri = @"";
		public static string DefaultMediaDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

		// track info max lengths (for ellipsizing purposes)
		public const int TrackNameMaxLength = 20;
		public const int TrackInfoMaxLength = 40;

		public const int ExplorerItemMaxLength = 60;

		// volume levels
		public const double VolumeMid = 50;
		// volume increment
		public const double VolumeIncrement = 5;

		// array that contains a list supported music extensions
		public static string[] MediaExtensions = { "mp3", ".mp3", "wma", ".wma", "wav", ".wav" };

		public const string BreadcrumbButtonSeparator = "/";

		public static char[] DirectorySeparators = { '/', '\\' };
		
		public const int InvalidIndex = -1;

		public static class OpacityLevel
		{
			public const int Opaque = 1;
			public const int Transparent = 0;
		}

		public const string Computer = "Computer";

		public const string ExplorerDirectorySetting = "ExplorerDirectory";
	}
}
