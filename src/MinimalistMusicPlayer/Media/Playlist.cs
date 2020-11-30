using MinimalistMusicPlayer.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MinimalistMusicPlayer.Media
{
	// TODO look at whether we should inherit List<MediaFile> directly instead of having a List prop
	public class Playlist
	{
		public List<MediaFile> Tracks { get; set; }
		public int Count
		{
			get { return Tracks.Count; }
		}

		// field is used for optimization
		// will be used to determine whether to run isSelected/isPlaylistMediaItem comparisons on MediaItems
		// comparisons will only be made if the MediaExplorer.CurrentDirectory == PlaylistDirectory
		public string PlaylistDirectory { get; set; }

		public int StartIndex { get; set; }
		public int CurrentIndex { get; set; }
		public int NextIndex
		{
			get
			{
				if (IsShuffle)
				{
					Random r = new Random();
					return r.Next(0, Count - 1);
				}

				switch (RepeatMode)
				{
					case RepeatMode.NoRepeat:
						if (CurrentIndex < Count - 1) return CurrentIndex + 1;
						else return Constant.InvalidIndex;

					case RepeatMode.Repeat:
						if (CurrentIndex < Count - 1) return CurrentIndex + 1;
						else return 0;

					case RepeatMode.RepeatOne:
					default:
						return CurrentIndex;
				}
			}
		}

		public MediaFile CurrentTrack
		{
			get
			{
				if (CurrentIndex == Constant.InvalidIndex || Tracks.Count == 0) return null;
				else return Tracks[CurrentIndex];
			}
		}

		public bool IsShuffle { get; set; }
		public RepeatMode RepeatMode { get; set; }

		public Playlist(List<MediaFile> items = null)
		{
			if (items == null) items = new List<MediaFile>();
			Tracks = items;

			CurrentIndex = StartIndex = Constant.InvalidIndex;
			IsShuffle = false;
			RepeatMode = RepeatMode.NoRepeat;
		}

		public void CycleRepeat()
		{
			switch (RepeatMode)
			{
				case RepeatMode.NoRepeat:
					RepeatMode = RepeatMode.Repeat;
					break;

				case RepeatMode.Repeat:
					RepeatMode = RepeatMode.RepeatOne;
					break;

				case RepeatMode.RepeatOne:
					RepeatMode = RepeatMode.NoRepeat;
					break;
			}
		}
		public void CycleShuffle()
		{
			IsShuffle = !IsShuffle;
		}

		public void IncrementIndex()
		{
			if (CurrentIndex < Count - 1) CurrentIndex++;
			else CurrentIndex = 0;
		}
		public void DecrementIndex()
		{
			if (CurrentIndex > 0) CurrentIndex--;
			else CurrentIndex = Count - 1;
		}

		public MediaFile GetTrack(int index)
		{
			if (index >= Tracks.Count || index < 0) return null;
			else return Tracks[index];
		}

		public int IndexOf(string trackName, DirectoryInfo currentDirectory)
		{
			if (PlaylistDirectory != currentDirectory.FullName) return Constant.InvalidIndex; // if the directories don't match up
			else return Tracks.FindIndex(f => f.FullName == trackName);
		}

		// adds a single item to both IWMPPlaylist and List<string> playlist references
		public void AddTrack(MediaFile file)
		{
			if (Contains(file)) return; // only add the item if it doesn't already exist in the playlist
			int itemIndex = Tracks.AddSorted(file);
			if (itemIndex <= CurrentIndex) CurrentIndex++; // if the item was added before the currently-playing item, increment the current index
		}

		// adds a list of items
		public void AddTracks(IEnumerable<MediaFile> tracks)
		{
			PlaylistDirectory = tracks.First().File.DirectoryName;
			foreach (var track in tracks) AddTrack(track);
		}

		public bool Contains(string trackName)
		{
			return Tracks.Exists(track => track.FullName == trackName);
		}
		public bool Contains(MediaFile file)
		{
			return Contains(file.FullName);
		}

		// clears playlist
		public void Clear()
		{
			Tracks.Clear();
		}
	}
	/// <summary>
	/// Playlist repeat modes
	/// </summary>
	public enum RepeatMode
	{
		NoRepeat,
		RepeatOne,
		Repeat
	}


}