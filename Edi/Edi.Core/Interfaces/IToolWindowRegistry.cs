using System.Collections.ObjectModel;
using Edi.Core.ViewModels;

namespace Edi.Core.Interfaces
{
	/// <summary>
	/// Defines an interface for a class that can register
	/// and manage tool window viemodels.
	/// </summary>
	public interface IToolWindowRegistry
	{
		ObservableCollection<ToolViewModel> Tools { get; }

		void RegisterTool(ToolViewModel newTool);
		void PublishTools();
	}
}