using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalistMusicPlayer.Player
{
	public class MediaFile
	{
		public string FullName { get; private set; }
		public string Name { get; private set; }
		public string Extension { get; private set; }
		public double Duration { get; private set; }
		public string Album { get; private set; }
		public string Artist { get; private set; }

		public MediaFile() { }

		public MediaFile(FileInfo file)
		{
			FullName = file.FullName;
			Name = file.Name;
			Extension = file.Extension;

			Duration = GetMediaDuration(file);
			Album = GetMediaAlbum(file);
			Artist = GetMediaArtist(file);
		}

		public string GetMediaArtist(FileInfo file)
		{
			throw new NotImplementedException();
		}

		public string GetMediaAlbum(FileInfo file)
		{
			throw new NotImplementedException();
		}

		public double GetMediaDuration(FileInfo file)
		{
			throw new NotImplementedException();
		}
	}
}
