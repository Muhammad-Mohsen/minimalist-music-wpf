using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MinimalistMusicPlayer.Utility
{
	class Anim
	{
		public static void AnimateHeight(FrameworkElement element, double newValue, double duration)
		{
			AnimateDoubleBasedProperty(element, newValue, duration, FrameworkElement.HeightProperty, true);
		}

		public static void AnimateOpacity(FrameworkElement element, double toValue, double duration)
		{
			AnimateDoubleBasedProperty(element, toValue, duration, UIElement.OpacityProperty, true);
		}

		public static void AnimateMargin(FrameworkElement element, Thickness fromValue, Thickness toValue, double duration)
		{
			AnimateThicknessBasedProperty(element, fromValue, toValue, duration, FrameworkElement.MarginProperty, true);
		}

		public static void AnimateAngle(FrameworkElement element, double oldValue, double newValue, double duration, bool isRepeated)
		{
			DoubleAnimation animation = new DoubleAnimation(oldValue, newValue, new Duration(TimeSpan.FromSeconds(duration)));

			if (isRepeated)
				animation.RepeatBehavior = RepeatBehavior.Forever;

			element.RenderTransform = new RotateTransform();
			element.RenderTransformOrigin = new Point(.5, .5);

			element.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, animation);
		}

		public static void StopRotationAnimation(FrameworkElement element)
		{
			element.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, null);
			element.BeginAnimation(RotateTransform.AngleProperty, null);
		}

		// fades in/out a given framework element
		public static async void ShowHideFrameworkElement(FrameworkElement element, bool shouldShow, double delay)
		{
			// note the difference between showing/hiding
			// the animatable property is Opacity. The element has to be visible (even if it's transparent) for the animation to show
			if (shouldShow)
			{
				element.Visibility = Visibility.Visible; // element still transparent at this point
				AnimateOpacity(element, Const.OpacityLevel.Opaque, delay); // now it's opaque
			}
			else
			{
				AnimateOpacity(element, Const.OpacityLevel.Transparent, delay); // fade out first
				await Task.Delay(TimeSpan.FromSeconds(delay)); // make sure that the animation completes
				element.Visibility = Visibility.Collapsed;
			}
		}

		public static async void SlideFrameworkElement(FrameworkElement element, bool slideIn, double delay)
		{
			Thickness fromMargin;
			Thickness toMargin;

			if (slideIn)
			{
				fromMargin = new Thickness(50, 0, -50, 0);
				toMargin = element.Margin;
			}
			else
			{
				fromMargin = element.Margin;
				toMargin = new Thickness(50, 0, -50, 0);
			}

			Anim.AnimateMargin(element, fromMargin, toMargin, delay);
			await Task.Delay(TimeSpan.FromSeconds(delay));
		}

		// generic double animation method
		private static void AnimateDoubleBasedProperty(FrameworkElement element, double newVal, double duration, DependencyProperty property, bool ease)
		{
			DoubleAnimation anim = new DoubleAnimation(newVal, new Duration(TimeSpan.FromSeconds(duration)));

			if (ease)
				anim.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut };

			element.BeginAnimation(property, anim);
		}
		// generic Thickness animation method
		private static void AnimateThicknessBasedProperty(FrameworkElement element, Thickness oldVal, Thickness newVal, double duration, DependencyProperty property, bool ease)
		{
			ThicknessAnimation anim = new ThicknessAnimation(oldVal, newVal, new Duration(TimeSpan.FromSeconds(duration)));

			if (ease)
				anim.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut };

			element.BeginAnimation(property, anim);
		}
	}
}