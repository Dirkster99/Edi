namespace Edi.Core.Behaviour
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Source: http://csharpbestpractices.blogspot.de/2011/09/mvvm-textbox-focus.html
    /// </summary>
    public static class FocusExtension
	{

		private static readonly DependencyProperty IsFocusedProperty =
						DependencyProperty.RegisterAttached(
						 "IsFocused", typeof(bool), typeof(FocusExtension),
						 new UIPropertyMetadata(false, OnIsFocusedPropertyChanged));

		public static bool GetIsFocused(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsFocusedProperty);
		}

		public static void SetIsFocused(DependencyObject obj, bool value)
		{
			obj.SetValue(IsFocusedProperty, value);
		}

		/// <summary>
		/// Attempt to acquire the focus on the attached control when the boolean dependency value is set to true.
		/// </summary>
		/// <param name="d"></param>
		/// <param name="e"></param>
		private static void OnIsFocusedPropertyChanged(DependencyObject d,
																									 DependencyPropertyChangedEventArgs e)
		{
			var uie = (UIElement)d;
			if (!(bool) e.NewValue) return;
			// Delay the call to allow the current batch of processing to finish before we shift focus.
			uie.Dispatcher.BeginInvoke(
				(Action)(() =>
				{
					if (uie.Focusable)
					{
						uie.Focus();
					}
				}), DispatcherPriority.Input);

			uie.Focus(); // Don't care about false values.
		}
	}
}