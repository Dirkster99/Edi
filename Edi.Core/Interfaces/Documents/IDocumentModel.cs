namespace Edi.Core.Interfaces.Documents
{
	using System;

	/// <summary>
	/// Interface defines properties and methods of a base class for modelling
	/// file access on persistent storage.
	/// </summary>
	public interface IDocumentModel : IDisposable
	{
		/// <summary>
		/// Occurs when the file name has changed.
		/// </summary>
		event EventHandler FileNameChanged;

		#region properties
		/// <summary>
		/// Gets whether the file content on storage (harddisk) can be changed
		/// or whether file is readonly through file properties.
		/// </summary>
		bool IsReadonly { get; }

		/// <summary>
		/// Determines whether a document has ever been stored on disc or whether
		/// the current path and other file properties are currently just initialized
		/// in-memory with defaults.
		/// </summary>
		bool IsReal { get; }

		/// <summary>
		/// Gets the complete path and file name for this document.
		/// </summary>
		string FileNamePath { get; }

		/// <summary>
		/// Gets the name of a file.
		/// </summary>
		string FileName { get; }

		/// <summary>
		/// Gets the path of a file.
		/// </summary>
		string Path { get; }

		/// <summary>
		/// Gets the file extension of the document represented by this path.
		/// </summary>
		string FileExtension { get; }

		/// <summary>
		/// Gets/sets a property to indicate whether this
		/// file was changed externally (by another editor) or not.
		/// 
		/// Setter can be used to override re-loading (keep current content)
		/// at the time of detection.
		/// </summary>
		bool WasChangedExternally { get; set; }
		#endregion properties

		#region methods
		/// <summary>
		/// Assigns a filename and path to this document model. This will also
		/// refresh all properties (IsReadOnly etc..) that can be queried for this document.
		/// </summary>
		/// <param name="fileNamePath"></param>
		/// <param name="isReal">Determines whether file exists on disk
		/// (file open -> properties are refreshed from persistence) or not
		/// (properties are reset to default).</param>
		void SetFileNamePath(string fileNamePath, bool isReal);

		/// <summary>
		/// Resets the IsReal property to adjust model when a new document has been saved
		/// for the very first time.
		/// </summary>
		/// <param name="isReal">Determines whether file exists on disk
		/// (file open -> properties are refreshed from persistence) or not
		/// (properties are reset to default).</param>
		void SetIsReal(bool isReal);

		/// <summary>
		/// Set a file specific value to determine whether file
		/// watching is enabled/disabled for this file.
		/// </summary>
		/// <param name="IsEnabled"></param>
		/// <returns></returns>
		bool EnableDocumentFileWatcher(bool IsEnabled);
		#endregion methods
	}
}
