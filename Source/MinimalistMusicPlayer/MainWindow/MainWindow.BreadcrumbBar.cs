using MinimalistMusicPlayer.Explorer;
using MinimalistMusicPlayer.Utility;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace MinimalistMusicPlayer
{
	public partial class MainWindow : Window
	{
		public void InitializeBreadcrumbBar(DirectoryInfo directory)
		{
			ButtonUp.Click += UpButton_Click;
			PopulateBreadcrumbBar(directory);
		}
		//
		// click handlers
		//
		private void UpButton_Click(object sender, RoutedEventArgs e)
		{
			// if you're at the computer root, do nothing
			if (CurrentDirectory == null)
				return;

			DirectoryInfo parentDirectory = CurrentDirectory.Parent;

			CurrentDirectory = parentDirectory;
			DirectoryChange(CurrentDirectory);
		}
		private void BreadcrumbButton_Click(object sender, RoutedEventArgs e)
		{
			BreadcrumbButton breadcrumbButton = (BreadcrumbButton)sender;

			// get the current directory up to that particular bread crumb button
			DirectoryInfo newDirectory = new DirectoryInfo(GetDirectory(breadcrumbButton));

			// if you click a crumb button that points to the same directory as the current, don't do anything
			if (newDirectory.FullName == CurrentDirectory.FullName)
				return;

			CurrentDirectory = newDirectory;
			DirectoryChange(CurrentDirectory);
		}
		private void ComputerButton_Click(object sender, RoutedEventArgs e)
		{
			// set the current directory to null, and repopulate everything
			CurrentDirectory = null;
			DirectoryChange(CurrentDirectory);
		}

		// gets the directory upto the given crumb
		public string GetDirectory(BreadcrumbButton crumbButton)
		{
			StringBuilder directory = new StringBuilder();

			foreach (BreadcrumbButton button in StackPanelDirectory.Children)
			{
				directory.Append(button.Content.ToString());
				if (button.Equals(crumbButton)) // if you get to where you want, break
					break;
			}
			
			// remove the "Computer/"
			directory.Replace("Computer/", "");

			// if you get to the root, append a '/'
			if (directory.Length == 2)
				directory.Append(Const.BreadcrumbButtonSeparator);

			return directory.ToString();
		}

		// reinitializes the breadcrumb bar using already existing breadcrumb buttons
		public void PopulateBreadcrumbBar(DirectoryInfo directory)
		{
			// clear everything and start over
			StackPanelDirectory.Children.Clear();

			// Add the computer button
			BreadcrumbButton computerButton = new BreadcrumbButton(Const.Computer);
			computerButton.Click += ComputerButton_Click;
			StackPanelDirectory.Children.Add(computerButton);

			BreadcrumbButton computerSeparatorButton = CreateSeparatorButton();
			StackPanelDirectory.Children.Add(computerSeparatorButton);

			// will be null at the root
			if (directory != null)
			{
				// break up the full path
				string[] breadcrumbs = directory.FullName.Split(Const.DirectorySeparators);

				foreach (string crumb in breadcrumbs)
				{
					// add the breadcrumb button to the breadcrumb bar
					BreadcrumbButton button = new BreadcrumbButton(crumb);
					button.Click += BreadcrumbButton_Click;
					StackPanelDirectory.Children.Add(button);

					// click handler isn't wired up to the event
					BreadcrumbButton separatorButton = CreateSeparatorButton();
					StackPanelDirectory.Children.Add(separatorButton);
				}

				ScrollViewerDirectory.ScrollToRightEnd();
			}
			
			// remove last separator button
			StackPanelDirectory.Children.RemoveAt(StackPanelDirectory.Children.Count - 1);
		}

		private BreadcrumbButton CreateSeparatorButton()
		{
			BreadcrumbButton separatorButton = new BreadcrumbButton(Const.BreadcrumbButtonSeparator);
			separatorButton.IsEnabled = false;
			separatorButton.IsTabStop = false;

			return separatorButton;
		}
	}
}