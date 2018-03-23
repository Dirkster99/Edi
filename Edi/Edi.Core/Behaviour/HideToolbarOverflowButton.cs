using System.Windows;
using System.Windows.Controls;

namespace Edi.Core.Behaviour
{
	/// <summary>
	/// This behaviour hides the overflow toolbar button on a ToolBar.
	/// 
	/// Known Issue:
	/// The Toolbar Grip appears when changing themes at run-time.
	/// This is because the gripper style is reset but the FramworkElement is not reloaded,
	/// Therfore, this Behaviour attached to the Loaded event is not fired again.
	/// </summary>
	public class HideToolbarOverflowButton
	{
		#region fields
		private static readonly DependencyProperty HideGripProperty;
		#endregion fields

		#region constructor
		static HideToolbarOverflowButton()
		{
			HideGripProperty =
				DependencyProperty.RegisterAttached("HideGrip",
																						typeof(bool),
																						typeof(HideToolbarOverflowButton),
																						new UIPropertyMetadata(false, OnSetCallback));
		}
		#endregion constructor

		#region methods
		public static bool GetHideGrip(DependencyObject obj)
		{
			return (bool)obj.GetValue(HideGripProperty);
		}

		public static void SetHideGrip(DependencyObject obj, bool value)
		{
			obj.SetValue(HideGripProperty, value);
		}

		/// <summary>
		/// Connect or Disconnect EventHandlers when attached value has changed.
		/// </summary>
		/// <param name="dependencyObject"></param>
		/// <param name="dependencyPropertyChangedEventArgs"></param>
		private static void OnSetCallback(DependencyObject dependencyObject,
																			DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			var frameworkElement = (FrameworkElement)dependencyObject;
			var target = OffLineIndicator.GetIsOnline(frameworkElement);

			//      if (target == null)
			//        return;

			if (frameworkElement == null)
				return;

			if (target)
			{
				frameworkElement.Loaded += mainToolBar_Loaded;
				frameworkElement.Unloaded += frameworkElement_Unloaded;
			}
			else
			{
				frameworkElement.Loaded -= mainToolBar_Loaded;
				frameworkElement.Unloaded -= frameworkElement_Unloaded;
			}
		}

		/// <summary>
		/// Disconnect EventHandlers on Unload
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		static void frameworkElement_Unloaded(object sender, RoutedEventArgs e)
		{
			if (!(sender is ToolBar frameworkElement))
				return;

			frameworkElement.Loaded -= mainToolBar_Loaded;
			frameworkElement.Unloaded -= frameworkElement_Unloaded;
		}

		private static void mainToolBar_Loaded(object sender, RoutedEventArgs e)
		{
			if (!(sender is ToolBar mainToolBar))
				return;

            ////      // mainToolBar – instance of toolbar defined in XAML view
            ////      foreach (FrameworkElement a in mainToolBar.Items)
            ////      {
            ////        ToolBar.SetOverflowMode(a, OverflowMode.Never);
            ////      }
            ////

			if (!(mainToolBar.Template.FindName("OverflowGrid", mainToolBar) is FrameworkElement)) return;
			FrameworkElement overflowGrid = mainToolBar.Template.FindName("OverflowGrid", mainToolBar) as FrameworkElement;
			if (overflowGrid != null) overflowGrid.Visibility = Visibility.Collapsed;
		}
		#endregion methods
	}
}