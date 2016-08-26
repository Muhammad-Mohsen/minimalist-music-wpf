using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MinimalistMusicPlayer.Utility
{
	public class Util
    {
		// returns a friendly time string from a timespan
		public static string GetTimeStringFromTimeSpan(TimeSpan t)
		{
			string s = t.ToString();
			return s.Substring(3, 5);
		}
		public static string GetTimeStringFromPosition(double position, double maxValue, double width)
		{
			double seconds = position * maxValue / width;
			return TimeSpan.FromSeconds(seconds).ToString().Substring(3, 5);
		}
		// validates the extension of a file
		public static bool IsMediaFile(string fileUrl)
		{
			string extension = fileUrl.Split('.').Last().ToLower();
			return Const.MediaExtensions.Contains(extension);
		}

		// fades in/out a given framework element
		public static async void ShowHideFrameworkElement(FrameworkElement element, bool shouldShow, double delay)
		{
			// note the difference between showing/hiding
			// the animatable property is Opacity. The element has to be visible (even if it's transparent) for the animation to show
			if (shouldShow)
			{
				element.Visibility = Visibility.Visible; // element still transparent at this point
				Anim.AnimateOpacity(element, Const.OpacityLevel.Opaque, delay); // now it's opaque
			}
			else
			{
				Anim.AnimateOpacity(element, Const.OpacityLevel.Transparent, delay); // fade out first
				await Task.Delay(TimeSpan.FromSeconds(delay)); // make sure that the animation completes
				element.Visibility = Visibility.Collapsed;
			}
		}
	}
}