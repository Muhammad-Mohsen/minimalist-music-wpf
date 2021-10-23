using MinimalistMusicPlayer.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace MinimalistMusicPlayer.Utility
{
	public static class Extensions
	{
		[Obsolete]
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

		public static bool IsMediaFile(this FileInfo file)
		{
			return Constant.MediaExtensions.Contains(file.Extension);
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

		// thanks - https://thomaslevesque.com/2019/11/18/using-foreach-with-index-in-c/
		public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
		{
			return source.Select((item, index) => (item, index));
		}

		// clamp polyfill
		public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
		{
			if (val.CompareTo(min) < 0) return min;
			else if (val.CompareTo(max) > 0) return max;
			else return val;
		}

		public static float Clamp(this float val, float min, float max)
		{
			return Clamp<float>(val, min, max);
		}
		public static double Clamp(this double val, double min, double max)
		{
			return Clamp<double>(val, min, max);
		}

		public static bool FuzzyMatch(this string str, string match)
		{
			str = str.ToLower();
			match = match.ToLower();

			int currentIndex = 0;
			foreach (char c in match)
			{
				currentIndex = str.IndexOf(c, currentIndex) + 1;
				if (currentIndex == 0) return false;
			}

			return true;
		}

		public static void Add<T>(this ICollection<T> list, params T[] entries)
		{
			foreach (var entry in entries) list.Add(entry);
		}
	}
}
