namespace Edi.Core.Interfaces
{
	/// <summary>
	/// Implements an interface to support a file watcher
	/// functionality for a certain type of document (eg.: text, log4net, etc ...).
	/// </summary>
	public interface IDocumentFileWatcher
	{
		/// <summary>
		/// Set a file specific value to determine whether file
		/// watching is enabled/disabled for this file.
		/// </summary>
		/// <param name="IsEnabled"></param>
		/// <returns></returns>
		bool EnableDocumentFileWatcher(bool IsEnabled);
	}
}
