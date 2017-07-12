namespace Edi.Core.Behaviour
{
	using System.Windows;

	public class OffLineIndicator
	{
		#region fields
		private static readonly DependencyProperty IsOnlineProperty;
		#endregion fields

		#region constructor
		static OffLineIndicator()
		{
			OffLineIndicator.IsOnlineProperty =
							DependencyProperty.RegisterAttached("IsOnline",
																									typeof(bool),
																									typeof(OffLineIndicator),
																									new UIPropertyMetadata(true, OffLineIndicator.OnSetCallback));
		}
		#endregion constructor

		#region methods
		public static bool GetIsOnline(DependencyObject obj)
		{
			return (bool)obj.GetValue(OffLineIndicator.IsOnlineProperty);
		}

		public static void SetIsOnline(DependencyObject obj, bool value)
		{
			obj.SetValue(OffLineIndicator.IsOnlineProperty, value);
		}

		private static void OnSetCallback(DependencyObject dependencyObject,
																			DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			var frameworkElement = (FrameworkElement)dependencyObject;
			var target = OffLineIndicator.GetIsOnline(frameworkElement);

			//      if (target == null)
			//        return;

			if (frameworkElement == null)
				return;

			if (target == true)
				frameworkElement.Opacity = 1;
			else
				frameworkElement.Opacity = .5;
		}
		#endregion methods
	}
}
