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
		public const int ExpandedWindowHeight = 500;

		// Uri where the IWMPPlaylist will be created each time the app starts
		public const string PlaylistUri = @"";

		// track info max lengths (for ellipsizing purposes)
		public const int TrackNameMaxLength = 20;
		public const int TrackInfoMaxLength = 40;

		// volume levels
		public const double VolumeMid = 50;
		// volume increment
		public const double VolumeIncrement = 5;
	}
}
