namespace FolderBrowser.Views.Behaviours
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Attached behaviour to implement the drop event via delegate command binding or routed commands.
    /// </summary>
    public static class TextEnterCommand
    {
        // Field of attached ICommand property
        private static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(TextEnterCommand),
            new PropertyMetadata(null, OnTextEnterCommandChange));

        /// <summary>
        /// Setter method of the attached DropCommand <seealso cref="ICommand"/> property
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        public static void SetCommand(DependencyObject source, ICommand value)
        {
            source.SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Getter method of the attached DropCommand <seealso cref="ICommand"/> property
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ICommand GetCommand(DependencyObject source)
        {
            return (ICommand)source.GetValue(CommandProperty);
        }

        /// <summary>
        /// This method is hooked in the definition of the <seealso cref="ChangedCommandProperty"/>.
        /// It is called whenever the attached property changes - in our case the event of binding
        /// and unbinding the property to a sink is what we are looking for.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnTextEnterCommandChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox uiElement = d as TextBox;   // Remove the handler if it exist to avoid memory leaks

            if (uiElement != null)
            {
                uiElement.KeyUp -= uiElement_KeyUp;

                var command = e.NewValue as ICommand;
                if (command != null)
                {
                    // the property is attached so we attach the Drop event handler
                    uiElement.KeyUp += uiElement_KeyUp;
                }
            }
        }

        /// <summary>
        /// This method is called when the KeyUp event occurs. The sender should be the control
        /// on which this behaviour is attached - so we convert the sender into a <seealso cref="TextBox"/>
        /// and receive the Command through the Command getter listed above.
        /// 
        /// This implementation supports binding of delegate commands and routed commands.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void uiElement_KeyUp(object sender, KeyEventArgs e)
        {
            if (e == null)
                return;

            // Forward key event only if user has hit the return, BackSlash, or Slash key
            if (e.Key != Key.Return)
                return;

            var uiElement = sender as TextBox;

            // Sanity check just in case this was somehow send by something else
            if (uiElement == null)
                return;

            ICommand changedCommand = GetCommand(uiElement);

            // There may not be a command bound to this after all
            if (changedCommand == null)
                return;

            // Check whether this attached behaviour is bound to a RoutedCommand
            if (changedCommand is RoutedCommand)
            {
                // Execute the routed command
                (changedCommand as RoutedCommand).Execute(uiElement.Text, uiElement);
            }
            else
            {
                // Execute the Command as bound delegate
                changedCommand.Execute(uiElement.Text);
            }
        }
    }
}
