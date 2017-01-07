using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MinimalistMusicPlayer.Explorer
{
	public class ExplorerItem : Button
	{
		public ExplorerItemType ItemType { get; set; }
	}

	public enum ExplorerItemType
	{
		Invalid = 0,
		DirectoryItem,
		MediaItem,
		DriveItem
	}
}