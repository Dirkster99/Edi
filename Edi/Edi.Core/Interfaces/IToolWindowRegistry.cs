namespace Edi.Core.Interfaces
{
    using System;
    using System.Collections.ObjectModel;
    using Edi.Core.Models;
    using Edi.Core.ViewModels;

    /// <summary>
    /// Defines an interface for a class that can register
    /// and manage tool window viemodels.
    /// </summary>
    public interface IToolWindowRegistry
	{
        event EventHandler<PublishToolWindowEventArgs> PublishToolWindows;

        ObservableCollection<ToolViewModel> Tools { get; }

		void RegisterTool(ToolViewModel newTool);

        /// <summary>
        /// Publishs all registered tool window definitions via  <see cref="PublishToolWindows"/> 
        /// (into an observable collection).
        /// (Which in turn will execute AvalonDock's LayoutInitializer that takes care of
        /// default positions etc).
        /// </summary>
        void PublishTools();
	}
}