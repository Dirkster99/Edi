namespace Output.Behaviors
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Input;

	/// <summary>
	/// Implements an attached behavior that invokes
	/// a command when a UI Element is loaded.
	/// </summary>
	public class ViewLoaded
	{
		#region fields
		public static readonly DependencyProperty CommandProperty =
				DependencyProperty.RegisterAttached("Command",
				                                    typeof(ICommand),
																						typeof(ViewLoaded),
																						new PropertyMetadata(null, ViewLoaded.OnCommandChanged));
		#endregion fields

		#region methods
		public static ICommand GetCommand(DependencyObject obj)
		{
			return (ICommand)obj.GetValue(CommandProperty);
		}

		public static void SetCommand(DependencyObject obj, ICommand value)
		{
			obj.SetValue(CommandProperty, value);
		}

		private static void OnCommandChanged(DependencyObject d,
																			   DependencyPropertyChangedEventArgs e)
		{
			FrameworkElement frameworkElement = d as FrameworkElement;	  // Remove the handler if it exist to avoid memory leaks
			frameworkElement.Loaded -= frameworkElement_Loaded;

            if (e.NewValue is ICommand)
            {
                ICommand command = e.NewValue as ICommand;

                // the property is attached so we attach the Loaded event handler
                frameworkElement.Loaded += frameworkElement_Loaded;
            }
        }

		/// <summary>
		/// Executes a bound command when the subscribed event occurs.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void frameworkElement_Loaded(object sender, RoutedEventArgs e)
		{
			UIElement uiElement = sender as UIElement;

			// Sanity check just in case this was somehow send by something else
			if (uiElement == null)
				return;

			ICommand loadedCommand = ViewLoaded.GetCommand(uiElement);

			// There may not be a command bound to this after all
			if (loadedCommand == null)
				return;

			// Check whether this attached behaviour is bound to a RoutedCommand
			if (loadedCommand is RoutedCommand)
			{
				// Execute the routed command
				(loadedCommand as RoutedCommand).Execute(loadedCommand, uiElement);
			}
			else
			{
				// Execute the Command as bound delegate
				loadedCommand.Execute(uiElement);
			}			
		}
		#endregion methods
	}
}
