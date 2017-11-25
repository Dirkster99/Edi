namespace MiniUML.Framework
{
  using System;
  using System.Windows;
  using System.Windows.Input;

  /// <summary>
  /// Attached property that can be used to create a binding for a CommandModel. Set the
  /// CreateCommandBinding.Command property to a CommandModel.
  /// To set multiple bindings on the same DependencyObject, use the method SetCommands.
  /// </summary>
  public static class CreateCommandBinding
  {
    public static readonly DependencyProperty CommandProperty
        = DependencyProperty.RegisterAttached("Command", typeof(CommandModel), typeof(CreateCommandBinding),
          new PropertyMetadata(new PropertyChangedCallback(OnCommandInvalidated)));

    public static CommandModel GetCommand(DependencyObject sender)
    {
      return (CommandModel)sender.GetValue(CommandProperty);
    }

    /// <summary>
    /// Method used to set a single binding on an element.
    /// Any exsisting bindings will be removed.
    /// </summary>
    public static void SetCommand(DependencyObject sender, CommandModel command)
    {
      sender.SetValue(CommandProperty, command);
    }

    /// <summary>
    /// Method used to set multiple bindings on the same element.
    /// Any exsisting bindings will be removed.
    /// </summary>
    public static void SetCommands(DependencyObject dependencyObject, params CommandModel[] commandModels)
    {
      if (dependencyObject == null)
        throw new ArgumentNullException("dependencyObject");

      UIElement element = (UIElement)dependencyObject;

      // Clear the exisiting bindings on the element we are attached to.
      element.CommandBindings.Clear();

      // If we're given a new command model, set up a binding.
      if (commandModels != null)
      {
        foreach (CommandModel commandModel in commandModels)
          element.CommandBindings.Add(new CommandBinding(commandModel.Command, commandModel.OnExecute, commandModel.OnQueryEnabled));
      }

      // Suggest to WPF to refresh commands.
      CommandManager.InvalidateRequerySuggested();
    }

    /// <summary>
    /// Callback when the Command property is set or changed.
    /// </summary>
    private static void OnCommandInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
      if (dependencyObject == null)
        throw new ArgumentNullException("dependencyObject");

      UIElement element = (UIElement)dependencyObject;

      // Clear the exisiting bindings on the element we are attached to.
      element.CommandBindings.Clear();

            // If we're given a new command model, set up a binding.
            if (e.NewValue is CommandModel)
            {
                CommandModel commandModel = e.NewValue as CommandModel;
                element.CommandBindings.Add(new CommandBinding(commandModel.Command, commandModel.OnExecute, commandModel.OnQueryEnabled));
            }

            // Suggest to WPF to refresh commands.
            CommandManager.InvalidateRequerySuggested();
    }
  }
}