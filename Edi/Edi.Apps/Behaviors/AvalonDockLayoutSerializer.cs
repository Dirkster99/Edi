using Xceed.Wpf.AvalonDock;

namespace Edi.Apps.Behaviors
{
	using System.Windows;
	using System.Windows.Input;
//// using Xceed.Wpf.AvalonDock;

	/// <summary>
	/// Class implements an attached behavior to load/save a layout for AvalonDock manager.
	/// This layout defines the position and shape of each document and tool window
	/// displayed in the application.
	/// 
	/// Load/Save is triggered through command binding
	/// On application start (AvalonDock.Load event results in LoadLayoutCommand) and
	///    application shutdown (AvalonDock.Unload event results in SaveLayoutCommand).
	/// 
	/// This implementation of layout save/load is MVVM compliant, robust, and simple to use.
	/// Just add the following code into your XAML:
	/// 
	/// xmlns:AVBehav="clr-namespace:Edi.View.Behavior"
	/// ...
	/// 
	/// avalonDock:DockingManager AnchorablesSource="{Binding Tools}" 
	///                           DocumentsSource="{Binding Files}"
	///                           ActiveContent="{Binding ActiveDocument, Mode=TwoWay, Converter={StaticResource ActiveDocumentConverter}}"
	///                           Grid.Row="3"
	///                           SnapsToDevicePixels="True"
	///                AVBehav:AvalonDockLayoutSerializer.LoadLayoutCommand="{Binding LoadLayoutCommand}"
	///                AVBehav:AvalonDockLayoutSerializer.SaveLayoutCommand="{Binding SaveLayoutCommand}"
	///                
	/// The LoadLayoutCommand passes a reference of the AvalonDock Manager instance to load the XML layout.
	/// The SaveLayoutCommand passes a string of the XML Layout which can be persisted by the viewmodel/model.
	/// 
	/// Both command bindings work with RoutedCommands or delegate commands (RelayCommand).
	/// </summary>
	public static class AvalonDockLayoutSerializer
	{
		#region fields
		/// <summary>
		/// Backing store for LoadLayoutCommand dependency property
		/// </summary>
		private static readonly DependencyProperty LoadLayoutCommandProperty =
				DependencyProperty.RegisterAttached("LoadLayoutCommand",
				typeof(ICommand),
				typeof(AvalonDockLayoutSerializer),
				new PropertyMetadata(null, OnLoadLayoutCommandChanged));
		#endregion fields

		#region methods
		#region Load Layout
		/// <summary>
		/// Standard get method of <seealso cref="LoadLayoutCommandProperty"/> dependency property.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static ICommand GetLoadLayoutCommand(DependencyObject obj)
		{
			return (ICommand)obj.GetValue(LoadLayoutCommandProperty);
		}

		/// <summary>
		/// Standard set method of <seealso cref="LoadLayoutCommandProperty"/> dependency property.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="value"></param>
		public static void SetLoadLayoutCommand(DependencyObject obj, ICommand value)
		{
			obj.SetValue(LoadLayoutCommandProperty, value);
		}

		/// <summary>
		/// This method is executed if a <seealso cref="LoadLayoutCommandProperty"/> dependency property
		/// is about to change its value (eg: The framewark assigns bindings).
		/// </summary>
		/// <param name="d"></param>
		/// <param name="e"></param>
		private static void OnLoadLayoutCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FrameworkElement framworkElement = d as FrameworkElement; // Remove the handler if it exist to avoid memory leaks
			framworkElement.Loaded -= OnFrameworkElement_Loaded;

            if (e.NewValue is ICommand)
            {
                ICommand command = e.NewValue as ICommand;

                // the property is attached so we attach the Drop event handler
                framworkElement.Loaded += OnFrameworkElement_Loaded;
            }
        }

		/// <summary>
		/// This method is executed when a AvalonDock <seealso cref="DockingManager"/> instance fires the
		/// Load standard (FrameworkElement) event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void OnFrameworkElement_Loaded(object sender, RoutedEventArgs e)
		{
			FrameworkElement frameworkElement = sender as FrameworkElement;

			// Sanity check just in case this was somehow send by something else
			if (frameworkElement == null)
				return;

			ICommand loadLayoutCommand = GetLoadLayoutCommand(frameworkElement);

			// There may not be a command bound to this after all
			if (loadLayoutCommand == null)
				return;

			// Check whether this attached behaviour is bound to a RoutedCommand
			if (loadLayoutCommand is RoutedCommand)
			{
				// Execute the routed command
				(loadLayoutCommand as RoutedCommand).Execute(frameworkElement, frameworkElement);
			}
			else
			{
				// Execute the Command as bound delegate
				loadLayoutCommand.Execute(frameworkElement);
			}
		}
		#endregion Load Layout
		#endregion methods
	}
}