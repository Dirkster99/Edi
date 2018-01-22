namespace FolderBrowser.Views.Behaviours
{
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;

  /// <summary>
  /// Source:
  /// http://stackoverflow.com/questions/1034374/drag-and-drop-in-mvvm-with-scatterview
  /// http://social.msdn.microsoft.com/Forums/de-DE/wpf/thread/21bed380-c485-44fb-8741-f9245524d0ae
  /// 
  /// Attached behaviour to implement the drop event via delegate command binding or routed commands.
  /// </summary>
  public static class TextChangedCommand
  {
    // Field of attached ICommand property
    private static readonly DependencyProperty ChangedCommandProperty = DependencyProperty.RegisterAttached(
        "ChangedCommand",
        typeof(ICommand),
        typeof(TextChangedCommand),
        new PropertyMetadata(null, OnTextChangedCommandChange));

    /// <summary>
    /// Setter method of the attached DropCommand <seealso cref="ICommand"/> property
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    public static void SetChangedCommand(DependencyObject source, ICommand value)
    {
      source.SetValue(ChangedCommandProperty, value);
    }

    /// <summary>
    /// Getter method of the attached DropCommand <seealso cref="ICommand"/> property
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static ICommand GetChangedCommand(DependencyObject source)
    {
      return (ICommand)source.GetValue(ChangedCommandProperty);
    }

    /// <summary>
    /// This method is hooked in the definition of the <seealso cref="ChangedCommandProperty"/>.
    /// It is called whenever the attached property changes - in our case the event of binding
    /// and unbinding the property to a sink is what we are looking for.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void OnTextChangedCommandChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      TextBox uiElement = d as TextBox;   // Remove the handler if it exist to avoid memory leaks

      if (uiElement != null)
      {
        uiElement.TextChanged -= OnText_Changed;

        var command = e.NewValue as ICommand;
        if (command != null)
        {
          // the property is attached so we attach the Drop event handler
          uiElement.TextChanged += OnText_Changed;
        }
      }
    }

    /// <summary>
    /// This method is called when the TextChanged event occurs. The sender should be the control
    /// on which this behaviour is attached - so we convert the sender into a <seealso cref="TextBox"/>
    /// and receive the Command through the GetDropCommand getter listed above.
    /// 
    /// The <paramref name="e"/> parameter contains the standard <seealso cref="DragEventArgs"/> data,
    /// which is unpacked and realesed upon the bound command.
    /// 
    /// This implementation supports binding of delegate commands and routed commands.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void OnText_Changed(object sender, TextChangedEventArgs e)
    {
      TextBox uiElement = sender as TextBox;

      // Sanity check just in case this was somehow send by something else
      if (uiElement == null)
        return;

      ICommand changedCommand = TextChangedCommand.GetChangedCommand(uiElement);

      // There may not be a command bound to this after all
      if (changedCommand == null)
        return;

      var item = uiElement.Text;

      // Check whether this attached behaviour is bound to a RoutedCommand
      if (changedCommand is RoutedCommand)
      {
        // Execute the routed command
        (changedCommand as RoutedCommand).Execute(item, uiElement);
      }
      else
      {
        // Execute the Command as bound delegate
        changedCommand.Execute(item);
      }
    }
  }
}
