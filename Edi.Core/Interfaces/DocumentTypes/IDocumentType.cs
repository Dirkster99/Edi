namespace Edi.Core.Interfaces.DocumentTypes
{
	using System;
	using System.Collections.Generic;

	public interface IDocumentType
	{
		#region properties
		/// <summary>
		/// Gets a list of file extensions that are supported by this document type.
		/// </summary>
		List<IDocumentTypeItem> FileTypeExtensions { get; }

		/// <summary>
		/// Gets the default file filter that should be used to save/load a document.
		/// </summary>
		string DefaultFilter { get; }

		/// <summary>
		/// Gets a string that can be displayed with the DefaultFilter
		/// string in filter drop down section of the file open/save dialog.
		/// </summary>
		string FileFilterName { get; }

		/// <summary>
		/// Gets the file open method that can be used to read a document of this type from disk.
		/// </summary>
		FileOpenDelegate FileOpenMethod { get; }

		/// <summary>
		/// Gets the file new method that can be used to read a document of this type from disk.
		/// This property can be null indicating that this type of document cannot be created
		/// with this module (this document type can only be read and viewed from disk).
		/// </summary>
		CreateNewDocumentDelegate CreateDocumentMethod { get; }

		/// <summary>
		/// Gets the key of this document type.
		/// </summary>
		string Key { get; }

		/// <summary>
		/// Gets a description that can be displayed for file open/new/save methods.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Gets the sort priority to determine a sort criteria when sorting this
		/// document type against other types in a list of supported document types.
		/// </summary>
		int SortPriority { get; }

		/// <summary>
		/// Gets the actual type of the viewmodel class that implements this document type.
		/// </summary>
		Type ClassType { get; }
		#endregion properties

		#region methods
		void RegisterFileTypeItem(IDocumentTypeItem fileType);

		/// <summary>
		/// Convinience methode to create an item for the collection of
		/// <seealso cref="IDocumentTypeItem"/> items managed in this class.
		/// </summary>
		/// <param name="description"></param>
		/// <param name="extensions"></param>
		/// <returns></returns>
		IDocumentTypeItem CreateItem(string description, List<string> extensions, int SortPriority=0);

		string GetFileOpenFilter();

		void GetFileFilterEntries(SortedList<int, IFileFilterEntry> ret, FileOpenDelegate fileOpenMethod);
		#endregion methods
	}
}
