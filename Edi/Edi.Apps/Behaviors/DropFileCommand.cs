namespace Edi.Apps.Behaviors
{
	using System.Windows.Input;
	using System.Windows;

	/// <summary>
	/// Source:
	/// http://stackoverflow.com/questions/1034374/drag-and-drop-in-mvvm-with-scatterview
	/// http://social.msdn.microsoft.com/Forums/de-DE/wpf/thread/21bed380-c485-44fb-8741-f9245524d0ae
	/// 
	/// Attached behaviour to implement the drop event via delegate command binding or routed commands.
	/// </summary>
	public static class DropFileCommand
	{
		// Field of attached ICommand property
		private static readonly DependencyProperty DropCommandProperty = DependencyProperty.RegisterAttached(
				"DropCommand",
				typeof(ICommand),
				typeof(DropFileCommand),
				new PropertyMetadata(null, OnDropCommandChange));

		/// <summary>
		/// Setter method of the attached DropCommand <seealso cref="ICommand"/> property
		/// </summary>
		/// <param name="source"></param>
		/// <param name="value"></param>
		public static void SetDropCommand(DependencyObject source, ICommand value)
		{
			source.SetValue(DropCommandProperty, value);
		}

		/// <summary>
		/// Getter method of the attached DropCommand <seealso cref="ICommand"/> property
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static ICommand GetDropCommand(DependencyObject source)
		{
			return (ICommand)source.GetValue(DropCommandProperty);
		}

		/// <summary>
		/// This method is hooked in the definition of the <seealso cref="DropCommandProperty"/>.
		/// It is called whenever the attached property changes - in our case the event of binding
		/// and unbinding the property to a sink is what we are looking for.
		/// </summary>
		/// <param name="d"></param>
		/// <param name="e"></param>
		private static void OnDropCommandChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			UIElement uiElement = d as UIElement;	  // Remove the handler if it exist to avoid memory leaks
			uiElement.Drop -= UIElement_Drop;

            if (e.NewValue is ICommand)
            {
                ICommand command = e.NewValue as ICommand;

                // the property is attached so we attach the Drop event handler
                uiElement.Drop += UIElement_Drop;
            }
        }

		/// <summary>
		/// This method is called when the Drop event occurs. The sender should be the control
		/// on which this behaviour is attached - so we convert the sender into a <seealso cref="UIElement"/>
		/// and receive the Command through the <seealso cref="GetDropCommand"/> getter listed above.
		/// 
		/// The <paramref name="e"/> parameter contains the standard <seealso cref="DragEventArgs"/> data,
		/// which is unpacked and reales upon the bound command.
		/// 
		/// This implementation supports binding of delegate commands and routed commands.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void UIElement_Drop(object sender, DragEventArgs e)
		{
			UIElement uiElement = sender as UIElement;

			// Sanity check just in case this was somehow send by something else
			if (uiElement == null)
				return;

			ICommand dropCommand = GetDropCommand(uiElement);

			// There may not be a command bound to this after all
			if (dropCommand == null)
				return;

			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] droppedFilePaths =
				e.Data.GetData(DataFormats.FileDrop, true) as string[];

				foreach (string droppedFilePath in droppedFilePaths)
				{
					// Check whether this attached behaviour is bound to a RoutedCommand
					if (dropCommand is RoutedCommand)
					{
						// Execute the routed command
						(dropCommand as RoutedCommand).Execute(droppedFilePath, uiElement);
					}
					else
					{
						// Execute the Command as bound delegate
						dropCommand.Execute(droppedFilePath);
					}
				}
			}
		}
	}
}
