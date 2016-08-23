using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalistMusicPlayer.Utility
{
	public static class Extensions
	{
		// extension method to ellipsize a string if it exceeds a given length
		public static string Ellipsize(this string s, int length)
		{
			// if the string length is smaller than the specified length, do nothing
			if (s.Length < length)
				return s;

			// else, truncate to the specified length - 3 (to acommodate the dots) and append the dots.
			else
				return string.Concat(s.Substring(0, length - 3), "...");
		}

		// returns a list of media files for a given DirectoryInfo, media files extensions defined in Const class
		public static FileInfo[] GetMediaFiles(this DirectoryInfo dir)
		{
			// get files by extension, ignoring hidden files
			FileInfo[] files = dir.EnumerateFiles().Where(f => Const.MediaExtensions.Contains(f.Extension) && (f.Attributes & FileAttributes.Hidden) == 0).ToArray();
			return files;
		}
	}
}
