using ATL;
using MinimalistMusicPlayer.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MinimalistMusicPlayer.Media
{
	public class MediaFile : IComparable<MediaFile>
	{
		public FileInfo File { get; private set; } // FileInfo is sealed unfortunately
		public string FullName { get; private set; }
		public string Name { get; private set; }
		public string Extension { get; private set; }

		public string Artist { get; private set; }
		public string Album { get; private set; }
		public TimeSpan Duration { get; private set; }
		public string DurationString { get; private set; }

		public List<Chapter> Chapters { get; private set; }

		public MediaFile(FileInfo file)
		{
			File = file;
			FullName = file.FullName;
			Name = file.Name;
			Extension = file.Extension;

			var metadata = new Track(file.FullName);
			Artist = string.IsNullOrWhiteSpace(metadata.Artist) ? "Unknown Artist" : metadata.Artist;
			Album = string.IsNullOrWhiteSpace(metadata.Album) ? "Unknown Artist" : metadata.Album;
			Duration = TimeSpan.FromSeconds(metadata.Duration);

			var format = (Duration.TotalHours >= 1d) ? Const.LongFormat : Const.ShortFormat; // Total hours returns something like 0.006, so we have to compare against 1
			DurationString = Duration.ToString(format);

			Chapters = metadata.Chapters.Select(c => new Chapter
			{
				Title = c.Title,
				StartTime = TimeSpan.FromMilliseconds(c.StartTime)

			}).ToList();
		}
		public MediaFile(string fullName) : this(new FileInfo(fullName)) {}

		public bool HasChapters()
		{
			return Chapters.Count > 1;
		}

		public double GetNextChapterStartPosition(double currentPosition)
		{
			return Chapters.Where(c => c.StartPosition >= currentPosition).FirstOrDefault()?.StartPosition ?? Const.InvalidIndex;
		}
		public double GetPreviousChapterStartPosition(double currentPosition)
		{
			return Chapters.Where(c => c.StartPosition <= (currentPosition - Const.LargeTolerance.TotalSeconds)).LastOrDefault()?.StartPosition ?? Const.InvalidIndex;
		}

		public int CompareTo(MediaFile other)
		{
			return string.Compare(FullName, other.FullName);
		}

		public static bool IsMediaFile(FileInfo file)
		{
			return Const.MediaExtensions.Contains(file.Extension);
		}

		public static TimeSpan GetDuration(FileInfo file)
		{
			var metadata = new Track(file.FullName);
			return TimeSpan.FromSeconds(metadata.Duration);
		}
		public static string GetDurationString(FileInfo file)
		{
			var duration = GetDuration(file);

			var format = (duration.TotalHours >= 1d) ? Const.LongFormat : Const.ShortFormat;
			return duration.ToString(format);
		}
	}

	public class Chapter
	{
		public string Title { get; set; }
		public TimeSpan StartTime { get; set; }
		public double StartPosition { get { return StartTime.TotalSeconds; } }
	}
}
