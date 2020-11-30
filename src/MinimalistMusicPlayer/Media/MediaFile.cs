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

		public string Artist { get { return string.IsNullOrWhiteSpace(Metadata.Artist) ? "Unknown Artist" : Metadata.Artist; } }
		public string Album { get { return string.IsNullOrWhiteSpace(Metadata.Album) ? "Unknown Album" : Metadata.Album; } }
		public TimeSpan Duration { get { return TimeSpan.FromSeconds(Metadata.Duration); } }
		public string DurationString
		{
			get
			{
				var format = (Duration.TotalHours >= 1d) ? Constant.LongFormat : Constant.ShortFormat; // Total hours returns something like 0.006, so we have to compare against 1
				return Duration.ToString(format);
			}
		}

		private List<Chapter> _Chapters;
		public List<Chapter> Chapters
		{
			get
			{
				if (_Chapters == null)
				{
					_Chapters = Metadata.Chapters.Select(c => new Chapter
					{
						Title = c.Title,
						StartTime = TimeSpan.FromMilliseconds(c.StartTime)

					}).ToList();
				}

				return _Chapters;
			}
		}

		private Track _Metadata;
		private Track Metadata
		{
			get
			{
				if (_Metadata == null) _Metadata = new Track(File.FullName);
				return _Metadata;
			}
		}

		public MediaFile(FileInfo file)
		{
			File = file;
			FullName = file.FullName;
			Name = file.Name;
			Extension = file.Extension;
		}

		public MediaFile(string fullName) : this(new FileInfo(fullName)) { }

		public bool HasChapters()
		{
			return Chapters.Count > 1;
		}

		public double GetNextChapterStartPosition(double currentPosition)
		{
			return Chapters.Where(c => c.StartPosition >= currentPosition).FirstOrDefault()?.StartPosition ?? Constant.InvalidIndex;
		}
		public double GetPreviousChapterStartPosition(double currentPosition)
		{
			return Chapters.Where(c => c.StartPosition <= (currentPosition - Constant.LargeThreshold.TotalSeconds)).LastOrDefault()?.StartPosition ?? Constant.InvalidIndex;
		}

		public int CompareTo(MediaFile other)
		{
			return string.Compare(FullName, other.FullName, StringComparison.Ordinal);
		}

		public static bool IsMediaFile(FileInfo file)
		{
			return Constant.MediaExtensions.Contains(file.Extension);
		}

		public static TimeSpan GetDuration(FileInfo file)
		{
			var metadata = new Track(file.FullName);
			return TimeSpan.FromSeconds(metadata.Duration);
		}
		public static string GetDurationString(FileInfo file)
		{
			var duration = GetDuration(file);

			var format = (duration.TotalHours >= 1d) ? Constant.LongFormat : Constant.ShortFormat;
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
