namespace Edi.Core.Behaviour
{
	using System.Windows;

	/// <summary>
	/// Implements a behavour that binds to a viewmodel property
	/// and updates its boolean value when the maximized window state changes.
	/// </summary>
	public static class WindowMaximized
	{
		// Using a DependencyProperty as the backing store for IsMaximized.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsNotMaximizedProperty =
				DependencyProperty.RegisterAttached("IsNotMaximized",
																						typeof(bool?),
																						typeof(WindowMaximized),
																						new PropertyMetadata(null, IsNotMaximizedChanged));

		public static bool? GetIsNotMaximized(DependencyObject obj)
		{
			return (bool?)obj.GetValue(IsNotMaximizedProperty);
		}

		public static void SetIsNotMaximized(DependencyObject obj, bool? value)
		{
			obj.SetValue(IsNotMaximizedProperty, value);
		}

		private static void IsNotMaximizedChanged(DependencyObject d,
																							DependencyPropertyChangedEventArgs e)
		{
			var window = d as Window;

			if (window != null)
				window.StateChanged -= window_StateChanged;

			if (window != null)
			{
				window.StateChanged += window_StateChanged;

				if (window.WindowState == WindowState.Maximized)
					WindowMaximized.SetIsNotMaximized(window, false);
				else
					WindowMaximized.SetIsNotMaximized(window, true);
			}
		}

		static void window_StateChanged(object sender, System.EventArgs e)
		{

            if (sender is Window w)
            {
                if (w.WindowState == WindowState.Maximized)
                    WindowMaximized.SetIsNotMaximized(w, false);
                else
                    WindowMaximized.SetIsNotMaximized(w, true);
            }
        }
	}
}
