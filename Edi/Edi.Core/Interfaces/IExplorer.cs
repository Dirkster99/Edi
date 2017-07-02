namespace Edi.Core.Interfaces
{
	using FileSystemModels.Models;

	public interface IExplorer
	{
		/// <summary>
		/// Gets an interface instance used for setting/getting settings of the Explorer (TW).
		/// </summary>
		ExplorerSettingsModel GetExplorerSettings(ExplorerSettingsModel input);

		/// <summary>
		/// Navigates to viewmodel to the <paramref name="directoryPath"/> folder.
		/// </summary>
		/// <param name="directoryPath"></param>
		void NavigateToFolder(string directoryPath);
	}
}
