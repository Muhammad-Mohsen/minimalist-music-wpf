using System.Windows;
using System.Windows.Controls;

namespace MinimalistMusicPlayer
{
	public class ExtendedButton : Button
	{
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(ExtendedButton), new PropertyMetadata(false));

		public CornerRadius CornerRadius
		{
			get { return (CornerRadius)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}
		public static readonly DependencyProperty CornerRadiusProperty
			= DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ExtendedButton), new PropertyMetadata(default(CornerRadius)));
	}
}
