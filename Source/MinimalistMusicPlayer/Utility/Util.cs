using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Shapes;

namespace MinimalistMusicPlayer.Utility
{
    public class Util
    {
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
	}
}