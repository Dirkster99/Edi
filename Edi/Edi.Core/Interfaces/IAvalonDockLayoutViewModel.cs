namespace Edi.Core.Interfaces
{
    using System;
    using System.Windows.Input;
    using Edi.Core.Models.Enums;
    using Edi.Core.ViewModels;

    /// <summary>
    /// Interface defines object properties and methods required
    /// to manage avalondock workspaces and properties (TemplateSelectors etc...).
    /// </summary>
    public interface IAvalonDockLayoutViewModel
	{
		/// <summary>
		/// Gets the layout id for the AvalonDock Layout that is associated with this viewmodel.
		/// This layout id is a form of identification between viewmodel and view to identify whether
		/// a given event aggregated message is for a given recipient or not.
		/// </summary>
		Guid LayoutId { get; }

		AvalonDockViewProperties ViewProperties { get; set; }

		/// <summary>
		/// Gets a command to load the layout of an AvalonDock-DockingManager instance.
		/// This layout defines the position and shape of each document and tool window
		/// displayed in the application.
		/// 
		/// Parameter:
		/// The command expects a reference to a <seealso cref="DockingManager"/> instance to
		/// work correctly. Not supplying that reference results in not loading a layout (silent return).
		/// </summary>
		ICommand LoadLayoutCommand { get; }

		/// <summary>
		/// Gets a command to save the layout of an AvalonDock-DockingManager instance.
		/// This layout defines the position and shape of each document and tool window
		/// displayed in the application.
		/// 
		/// Parameter:
		/// The command expects a reference to a <seealso cref="string"/> instance to
		/// work correctly. The string is supposed to contain the XML layout persisted
		/// from the DockingManager instance. Not supplying that reference to the string
		/// results in not saving a layout (silent return).
		/// </summary>
		ICommand SaveLayoutCommand { get; }

		/// <summary>
		/// Verify whether a custom AvalonDock layout was loaded from persistence or not.
		/// </summary>
		LayoutLoaded LayoutSoure { get; }

		/// <summary>
		/// (Re-)Assigns the internal parent layout property to the new parent <paramref name="parent"/>.
		/// </summary>
		/// <param name="parent"></param>
		////void ResestParent(ILayoutableViewModelParent parent);
	}
}
