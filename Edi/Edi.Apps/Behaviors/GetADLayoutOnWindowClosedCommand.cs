using Edi.Core.Interfaces;

namespace Edi.Apps.Behaviors
{
	using System.Windows;
	using System.Windows.Input;

	/// <summary>
	/// Source:
	/// http://stackoverflow.com/questions/1034374/drag-and-drop-in-mvvm-with-scatterview
	/// http://social.msdn.microsoft.com/Forums/de-DE/wpf/thread/21bed380-c485-44fb-8741-f9245524d0ae
	/// 
	/// Attached behaviour to implement the Closed event via delegate command binding or routed commands.
	/// </summary>
	public static class GetAdLayoutOnWindowClosedCommand
	{
		// Field of attached ICommand property
		private static readonly DependencyProperty SendLayoutCommandProperty =
			 DependencyProperty.RegisterAttached(
													"SendLayoutCommand",
													typeof(ICommand),
													typeof(GetAdLayoutOnWindowClosedCommand),
													new PropertyMetadata(null, OnSendLayoutCommandChange));

		/// <summary>
		/// Setter method of the attached SendLayoutCommand <seealso cref="ICommand"/> property
		/// </summary>
		/// <param name="source"></param>
		/// <param name="value"></param>
		public static void SetSendLayoutCommand(DependencyObject source, ICommand value)
		{
			source.SetValue(SendLayoutCommandProperty, value);
		}

		/// <summary>
		/// Getter method of the attached SendLayoutCommand <seealso cref="ICommand"/> property
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static ICommand GetSendLayoutCommand(DependencyObject source)
		{
			return (ICommand)source.GetValue(SendLayoutCommandProperty);
		}

		/// <summary>
		/// This method is hooked in the definition of the <seealso cref="SendLayoutCommandProperty"/>.
		/// It is called whenever the attached property changes - in our case the event of binding
		/// and unbinding the property to a sink is what we are looking for.
		/// </summary>
		/// <param name="d"></param>
		/// <param name="e"></param>
		private static void OnSendLayoutCommandChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			// Remove the handler if it exist to avoid memory leaks
			if (d is ILayoutableWindow win)
			{
				win.Closed -= uiElement_Closed;

				if (e.NewValue is ICommand)
				{
					// the property is attached so we attach the closed event handler
					win.Closed += uiElement_Closed;
				}
			}
		}

		/// <summary>
		/// This method is called when the closed event occurs. The sender should be the control
		/// on which this behaviour is attached - so we convert the sender into a <seealso cref="UIElement"/>
		/// and receive the Command through the <seealso cref="GetDropCommand"/> getter listed above.
		/// 
		/// The <paramref name="e"/> parameter contains the standard <seealso cref="ClosedEventArgs"/> data,
		/// which we will ignore here.
		/// 
		/// This implementation supports binding of delegate commands and routed commands.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		static void uiElement_Closed(object sender, System.EventArgs e)
		{
			// Sanity check just in case this was somehow send by something else
			if (!(sender is ILayoutableWindow layoutableElement) || !(sender is FrameworkElement fwElement))
				return;

			string xmlLayout = layoutableElement.CurrentADLayout;

			ICommand sendLayoutCommand = GetSendLayoutCommand(fwElement);

			// There may not be a command bound to this after all
			if (sendLayoutCommand == null)
				return;

			// Check whether this attached behaviour is bound to a RoutedCommand
			if (sendLayoutCommand is RoutedCommand)
			{
				// Execute the routed command
				(sendLayoutCommand as RoutedCommand).Execute(xmlLayout, fwElement);
			}
			else
			{
				// Execute the Command as bound delegate
				sendLayoutCommand.Execute(xmlLayout);
			}
		}
	}
}
