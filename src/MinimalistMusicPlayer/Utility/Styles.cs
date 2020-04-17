using System.Windows;

namespace MinimalistMusicPlayer.Utility
{
	// styles class.
	// used to set styles programmatically for toggle buttons (e.g. playlist, stay-on-top buttons)
	public static class Styles
	{
		public static Style BackgroundButtonToggleStyle = (Style)Application.Current.Resources["BackGroundButtonToggleStyle"];
		public static Style BackgroundButtonStyle = (Style)Application.Current.Resources["BackgroundButtonStyle"];

		public static Style AlphaButtonToggleStyle = (Style)Application.Current.Resources["AlphaButtonToggleStyle"];
		public static Style AlphaButtonStyle = (Style)Application.Current.Resources["AlphaButtonStyle"];

		public static Style PlaylistButtonStyle = (Style)Application.Current.Resources["PlaylistButton"];
	}
}
