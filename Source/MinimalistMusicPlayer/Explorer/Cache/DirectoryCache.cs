using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalistMusicPlayer.Explorer.Cache
{
	public class DirectoryCache
	{
		public DateTime LastModified { get; set; }
		public DirectoryInfo Directory { get; set; }
	}
}
