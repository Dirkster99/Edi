namespace Edi.Core.Behaviour
{
	using System.Windows;
	using System.Windows.Input;

	/// <summary>
	/// Class implements a <seealso cref="FrameworkElement"/> double click
	/// to command binding attached behaviour.
	/// </summary>
	public class DoubleClickImageToCommand
	{
		#region fields
		private static readonly DependencyProperty DoubleClickItemCommandProperty =
														DependencyProperty.RegisterAttached("DoubleClickItemCommand",
																						typeof(ICommand),
																						typeof(DoubleClickImageToCommand),
																						new PropertyMetadata(null,
																						DoubleClickImageToCommand.OnClickItemCommand));



		public static ICommand GetRightClickItemCommand(DependencyObject obj)
		{
			return (ICommand)obj.GetValue(RightClickItemCommandProperty);
		}

		public static void SetRightClickItemCommand(DependencyObject obj, ICommand value)
		{
			obj.SetValue(RightClickItemCommandProperty, value);
		}

		// Using a DependencyProperty as the backing store for RightClickItemCommand.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty RightClickItemCommandProperty =
				DependencyProperty.RegisterAttached("RightClickItemCommand",
																						typeof(ICommand),
																						typeof(DoubleClickImageToCommand),
																						new PropertyMetadata(null,
																						DoubleClickImageToCommand.OnClickItemCommand));
		#endregion fields

		#region constructor
		/// <summary>
		/// Class constructor
		/// </summary>
		public DoubleClickImageToCommand()
		{
		}
		#endregion constructor

		#region methods
		#region attached dependency property methods
		/// <summary>
		/// Gets the command of this dependency property.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static ICommand GetDoubleClickItemCommand(DependencyObject obj)
		{
			return (ICommand)obj.GetValue(DoubleClickItemCommandProperty);
		}

		/// <summary>
		/// Sets the command of this dependency property.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="value"></param>
		public static void SetDoubleClickItemCommand(DependencyObject obj, ICommand value)
		{
			obj.SetValue(DoubleClickItemCommandProperty, value);
		}
		#endregion attached dependency property methods

		private static void OnClickItemCommand(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var fwElement = d as FrameworkElement;

			// Remove the handler if it exist to avoid memory leaks
			if (fwElement != null)
				fwElement.MouseDown -= FrameworkElement_MouseClick;

            if (e.NewValue is ICommand command)
            {
                // the property is attached so we attach the Drop event handler
                fwElement.MouseDown += FrameworkElement_MouseClick;
            }
        }

		private static void FrameworkElement_MouseClick(object sender, MouseButtonEventArgs e)
		{
			// Send should be this class or a descendent of it
			var fwElement = sender as FrameworkElement;

			// Sanity check just in case this was somehow send by something else
			if (fwElement == null)
				return;

			// Handle right mouse click event if there is a command attached for this
			if (e.ChangedButton == MouseButton.Right)
			{
				ICommand clickCommand = DoubleClickImageToCommand.GetRightClickItemCommand(fwElement);

				if (clickCommand != null)
				{
					// Check whether this attached behaviour is bound to a RoutedCommand
					if (clickCommand is RoutedCommand)
					{
						// Execute the routed command
						(clickCommand as RoutedCommand).Execute(fwElement, fwElement);
						e.Handled = true;
					}
					else
					{
						// Execute the Command as bound delegate
						clickCommand.Execute(fwElement);
						e.Handled = true;
					}
				}
			}

			// Filter for left mouse button double-click
			if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
			{
				ICommand doubleclickCommand = DoubleClickImageToCommand.GetDoubleClickItemCommand(fwElement);

				// There may not be a command bound to this after all
				if (doubleclickCommand == null)
					return;

				// Check whether this attached behaviour is bound to a RoutedCommand
				if (doubleclickCommand is RoutedCommand)
				{
					// Execute the routed command
					(doubleclickCommand as RoutedCommand).Execute(fwElement, fwElement);
					e.Handled = true;
				}
				else
				{
					// Execute the Command as bound delegate
					doubleclickCommand.Execute(fwElement);
					e.Handled = true;
				}
			}
		}
		#endregion methods
	}
}
