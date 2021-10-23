using MinimalistMusicPlayer.Utility;
using System.Threading;
using System.Windows;

namespace MinimalistMusicPlayer
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		// WOW!!! all that to suppress a lint warning!!!!
		[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "the out param 'createdNew' is the goal of this whole operation")]
		private static Mutex SingleInstanceMutex = null;

		protected override void OnStartup(StartupEventArgs e)
		{
			// thanks - https://www.c-sharpcorner.com/UploadFile/f9f215/how-to-restrict-the-application-to-just-one-instance/
			SingleInstanceMutex = new Mutex(true, "Minimalist", out bool createdNew);
			if (!createdNew) Current.Shutdown(); //app is already running! Exiting the application

			ApplicationSettings.Load();

			base.OnStartup(e);
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			ApplicationSettings.Save();
		}
	}
}
