namespace Edi.Core.Behaviour
{
	using System.Windows;

	public static class DialogCloser
	{
		/// <summary>
		/// Dependency property for attached behaviour in NON-dialog windows.
		/// This can be is used to close a NON-dialog window via ViewModel.
		/// </summary>
		public static readonly DependencyProperty DialogResultProperty =
				DependencyProperty.RegisterAttached(
						"DialogResult",
						typeof(bool?),
						typeof(DialogCloser),
						new PropertyMetadata(DialogResultChanged));

		/// <summary>
		/// Setter of corresponding dependency property
		/// </summary>
		/// <param name="target"></param>
		/// <param name="value"></param>
		public static void SetDialogResult(Window target, bool? value)
		{
			target.SetValue(DialogResultProperty, value);
		}

		private static void DialogResultChanged(DependencyObject d,
																						DependencyPropertyChangedEventArgs e)
		{
			var window = d as Window;

		    // If a shutdown request was cancelled.
		    if (e.NewValue == null)     // Do not react on this ([re-]initialization) event.
		        return;

		    if (window?.Visibility == Visibility.Visible)
		    {
		        // Setting the DialogResult property invokes the close method of the corresponding dialog
		        //// window.DialogResult = e.NewValue as bool?;

		        // Dialog mResult cannot be set on windows that are no shown as dialogs.
		        // Therefore, we close directly via calling the corresponding close method of the view
		        try
		        {
		            window.Close();
		        }
		        catch
		        {
		            // ignored
		        }
		    }
		}
	}
}
