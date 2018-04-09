namespace Edi.Core.Behaviour
{
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// http://www.juanagui.com/blog/?p=107
    /// </summary>
    public static class KeyboardFocus
	{
		#region fields
		public static readonly DependencyProperty OnProperty;
		#endregion fields

		#region constructor
		static KeyboardFocus()
		{
			OnProperty = DependencyProperty.RegisterAttached("On",
																																		 typeof(FrameworkElement),
																																		 typeof(KeyboardFocus),
																																		 new PropertyMetadata(OnSetCallback));
		}
		#endregion constructor

		#region methods
		public static void SetOn(UIElement element, FrameworkElement value)
		{
			element.SetValue(OnProperty, value);
		}

		public static FrameworkElement GetOn(UIElement element)
		{
			return (FrameworkElement)element.GetValue(OnProperty);
		}

		private static void OnSetCallback(DependencyObject dependencyObject,
																			DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			var frameworkElement = (FrameworkElement) dependencyObject;

			if (frameworkElement == null)
				return;

			var target = GetOn(frameworkElement);

			if (target == null)
				return;

			try
			{
				frameworkElement.Loaded += (s, e) => Keyboard.Focus(target);
			}
			catch
			{
				// ignored
			}
		}
		#endregion methods
	}
}