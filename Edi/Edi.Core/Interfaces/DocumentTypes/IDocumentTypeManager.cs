using System;
using System.Collections.ObjectModel;
using Edi.Core.Interfaces.Documents;
using Edi.Core.ViewModels;

namespace Edi.Core.Interfaces.DocumentTypes
{
	/// <summary>
	/// Delegates the file open method to a method that can be registered in a module.
	/// The registered methid should return a viewmodel which in turn has registered a
	/// view and/or tool window viewmodels and views...
	/// </summary>
	/// <param name="fileModel"></param>
	/// <param name="settingsManager"></param>
	/// <returns></returns>
	public delegate IFileBaseViewModel FileOpenDelegate(IDocumentModel fileModel, object settingsManager);

	/// <summary>
	/// Delegates the file new method to a method that can be registered in a module.
	/// The registered method should return a viewmodel which in turn has registered a
	/// view for document display.
	/// 
	/// Create a new default document based on the given document model.
	/// </summary>
	/// <param name="documentModel"></param>
	/// <returns></returns>
	public delegate IFileBaseViewModel CreateNewDocumentDelegate(IDocumentModel documentModel);

	/// <summary>
	/// Interface specification for the document management service that drives
	/// creation, loading and saving of documents in the low level backend.
	/// </summary>
	public interface IDocumentTypeManager
	{
		#region properties
		ObservableCollection<IDocumentType> DocumentTypes { get; }
		#endregion properties

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="fileFilterName"></param>
		/// <param name="defaultFilter"></param>
		/// <param name="fileOpenMethod">Is a static method that returns <seealso cref="FileBaseViewModel"/>
		/// and takes a string (path) and ISettingsManager as parameter.</param>
		/// <param name="t"></param>
		/// <returns></returns>
		IDocumentType RegisterDocumentType(string key,
																			 string name,
																			 string fileFilterName,
																			 string defaultFilter,               // eg: 'log4j'
																			 FileOpenDelegate fileOpenMethod,
																			 CreateNewDocumentDelegate createDocumentMethod,
																			 Type t,
																			 int sortPriority = 0
																			 );

		/// <summary>
		/// Finds a document type that can handle a file
		/// with the given file extension eg ".txt" or "txt"
		/// when the original file name was "Readme.txt".
		/// 
		/// Always returns the 1st document type handler that matches the extension.
		/// </summary>
		/// <param name="fileExtension"></param>
		/// <param name="trimPeriod">Determines if an additional '.' character is removed
		/// from the given extension string or not.</param>
		/// <returns></returns>
		IDocumentType FindDocumentTypeByExtension(string fileExtension,
																							bool trimPeriod = false);

		IDocumentType FindDocumentTypeByKey(string typeOfDoc);

		/// <summary>
		/// Goes through all file/document type definitions and returns a filter string
		/// object that can be used in conjunction with FileOpen and FileSave dialog filters.
		/// </summary>
		/// <param name="key">Get entries for this viewmodel only,
		/// or all entries if key parameter is not set.</param>
		/// <returns></returns>
		IFileFilterEntries GetFileFilterEntries(string key = "");
	}
}
