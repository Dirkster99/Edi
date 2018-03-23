using System.Windows;

namespace Edi.Core.Behaviour
{
	public class OffLineIndicator
	{
		#region fields
		private static readonly DependencyProperty IsOnlineProperty;
		#endregion fields

		#region constructor
		static OffLineIndicator()
		{
			IsOnlineProperty =
							DependencyProperty.RegisterAttached("IsOnline",
																									typeof(bool),
																									typeof(OffLineIndicator),
																									new UIPropertyMetadata(true, OnSetCallback));
		}
		#endregion constructor

		#region methods
		public static bool GetIsOnline(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsOnlineProperty);
		}

		public static void SetIsOnline(DependencyObject obj, bool value)
		{
			obj.SetValue(IsOnlineProperty, value);
		}

		private static void OnSetCallback(DependencyObject dependencyObject,
																			DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			var frameworkElement = (FrameworkElement)dependencyObject;
			var target = GetIsOnline(frameworkElement);

			//      if (target == null)
			//        return;

			if (frameworkElement == null)
				return;

			frameworkElement.Opacity = target ? 1 : .5;
		}
		#endregion methods
	}
}