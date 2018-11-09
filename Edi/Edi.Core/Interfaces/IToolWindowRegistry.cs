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
        event EventHandler<RegisterToolWindowEventArgs> RegisterToolWindowEvent;

        ObservableCollection<ToolViewModel> Tools { get; }

		void RegisterTool(ToolViewModel newTool);
		void PublishTools();
	}
}