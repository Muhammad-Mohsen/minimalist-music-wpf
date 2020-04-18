using System.Windows;

namespace MinimalistMusicPlayer
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void Application_Exit(object sender, ExitEventArgs e)
		{
			MinimalistMusicPlayer.Properties.Settings.Default.Save();
		}
	}
}
