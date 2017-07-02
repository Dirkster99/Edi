namespace Edi.Apps.Behaviors
{
	using System.Windows.Input;
	using System.Windows;

	/// <summary>
	/// Attached behaviour to implement the activated event
	/// via delegate command binding or routed commands.
	/// </summary>
	public static class ActivatedCommand
	{
		#region fields
		/// <summary>
		/// <seealso cref="ICommand"/> field for command binding.
		/// </summary>
		private static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
				"Command",
				typeof(ICommand),
				typeof(ActivatedCommand),
				new PropertyMetadata(null, ActivatedCommand.OnCommandChange));

		/// <summary>
		/// <seealso cref="object"/> field for CommandParameter binding if user wants to
		/// relay the activated event to a command that receives a parameter.
		/// </summary>
		private static readonly DependencyProperty CommandParameterProperty =
				DependencyProperty.RegisterAttached("CommandParameter",
				typeof(object), typeof(ActivatedCommand), new PropertyMetadata(null));
		#endregion fields

		#region methods
		#region Command
		/// <summary>
		/// Setter method of the attached Command <seealso cref="ICommand"/> property
		/// </summary>
		/// <param name="source"></param>
		/// <param name="value"></param>
		public static void SetCommand(DependencyObject source, ICommand value)
		{
			source.SetValue(CommandProperty, value);
		}

		/// <summary>
		/// Getter method of the attached Command <seealso cref="ICommand"/> property
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static ICommand GetCommand(DependencyObject source)
		{
			return (ICommand)source.GetValue(CommandProperty);
		}
		#endregion Command

		#region CoammandPArameter
		/// <summary>
		/// <seealso cref="object"/> field for CommandParameter binding if user wants to
		/// relay the activated event to a command that receives a parameter.
		/// </summary>
		/// <param name="obj"></param>
		public static object GetCommandParameter(DependencyObject obj)
		{
			return (object)obj.GetValue(CommandParameterProperty);
		}

		/// <summary>
		/// <seealso cref="object"/> field for CommandParameter binding if user wants to
		/// relay the activated event to a command that receives a parameter.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="value"></param>
		public static void SetCommandParameter(DependencyObject obj, object value)
		{
			obj.SetValue(CommandParameterProperty, value);
		}
    #endregion CoammandPArameter

		/// <summary>
		/// This method is hooked in the definition of the <seealso cref="CommandProperty"/>.
		/// It is called whenever the attached property changes - in our case the event of binding
		/// and unbinding the property to a sink is what we are looking for.
		/// </summary>
		/// <param name="d"></param>
		/// <param name="e"></param>
		private static void OnCommandChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var uiElement = d as Window;	  // Remove the handler if it exist to avoid memory leaks
			uiElement.Activated -= UiElement_Activated;

            if (e.NewValue is ICommand command)
            {
                // the property is attached so we attach the Drop event handler
                uiElement.Activated += UiElement_Activated;
            }
        }

		/// <summary>
		/// This method is called when the Activated event occurs. The sender should be the control
		/// on which this behaviour is attached - so we convert the sender into a <seealso cref="UIElement"/>
		/// and receive the Command through the <seealso cref="GetCommand"/> getter listed above.
		/// 
		/// The <paramref name="e"/> parameter contains the standard <seealso cref="DragEventArgs"/> data,
		/// which is unpacked and reales upon the bound command.
		/// 
		/// This implementation supports binding of delegate commands and routed commands.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void UiElement_Activated(object sender, System.EventArgs e)
		{
			var uiElement = sender as Window;

			// Sanity check just in case this was somehow send by something else
			if (uiElement == null)
				return;

			ICommand Command = ActivatedCommand.GetCommand(uiElement);

			object CommandParameter = ActivatedCommand.GetCommandParameter(uiElement);

			// There may not be a command bound to this after all
			if (Command == null)
				return;

			// Check whether this attached behaviour is bound to a RoutedCommand
			if (Command is RoutedCommand)
			{
				// Execute the routed command
				(Command as RoutedCommand).Execute(CommandParameter, uiElement);
			}
			else
			{
				// Execute the Command as bound delegate
				Command.Execute(CommandParameter);
			}
		}
		#endregion methods
	}
}
