using MinimalistMusicPlayer.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
		public static MediaFile[] GetMediaFiles(this DirectoryInfo dir)
		{
			// get files by extension, ignoring hidden files
			return dir.EnumerateFiles()
					.Where(f => Constant.MediaExtensions.Contains(f.Extension) && (f.Attributes & FileAttributes.Hidden) == 0)
					.Select(f => new MediaFile(f))
					.ToArray();
		}

		// adds an item to a sorted list
		// source: http://stackoverflow.com/questions/12172162/how-to-insert-item-into-list-in-order
		public static int AddSorted<T>(this List<T> list, T item) where T : IComparable<T>
		{
			if (list.Count == 0)
			{
				list.Add(item);
				return 0;
			}
			if (list[list.Count - 1].CompareTo(item) <= 0)
			{
				list.Add(item);
				return list.Count - 1;
			}
			if (list[0].CompareTo(item) >= 0)
			{
				list.Insert(0, item);
				return 0;
			}

			int index = list.BinarySearch(item);
			if (index < 0) index = ~index;
			list.Insert(index, item);
			return index;
		}
	}
}
