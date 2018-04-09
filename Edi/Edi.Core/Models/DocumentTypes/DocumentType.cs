namespace Edi.Core.Models.DocumentTypes
{
    using System;
    using System.Collections.Generic;
    using Edi.Core.Interfaces.DocumentTypes;

    /// <summary>
    /// This class manages document specific data items. Such as, filter for file open dialog,
    /// a FileOpenMethod that returns the correct viewmodel etc.
    /// 
    /// Moduls can use this class to register new document types via the <seealso cref="IDocumentType"/>
    /// interface using the <seealso cref="IDocumentTypeManager"/> service.
    /// </summary>
    internal class DocumentType : IDocumentType
	{
		#region constructors

		/// <summary>
		/// Class constructor.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="description"></param>
		/// <param name="fileFilterName"></param>
		/// <param name="defaultFilter"></param>
		/// <param name="fileOpenMethod"></param>
		/// <param name="createDocumentMethod"></param>
		/// <param name="classType"></param>
		/// <param name="sortPriority"></param>
		public DocumentType(string key,
										 string description,
										 string fileFilterName,
										 string defaultFilter,
										 FileOpenDelegate fileOpenMethod,
										 CreateNewDocumentDelegate createDocumentMethod,
										 Type classType,
										 int sortPriority = 0)
		{
			Key = key;
			Description = description;
			FileFilterName = fileFilterName;
			SortPriority = sortPriority;
			DefaultFilter = defaultFilter;
			FileOpenMethod = fileOpenMethod;
			CreateDocumentMethod = createDocumentMethod;
			ClassType = classType;

			FileTypeExtensions = null;
		}
		#endregion constructors

		#region properties
		/// <summary>
		/// Gets a list of file extensions that are supported by this document type.
		/// </summary>
		public List<IDocumentTypeItem> FileTypeExtensions { get; private set; }

		/// <summary>
		/// Gets the default file filter that should be used to save/load a document.
		/// </summary>
		public string DefaultFilter { get; }

		/// <summary>
		/// Gets a string that can be displayed with the DefaultFilter
		/// string in filter drop down section of the file open/save dialog.
		/// </summary>
		public string FileFilterName { get; }

		/// <summary>
		/// Gets the file open method that can be used to read a document of this type from disk.
		/// </summary>
		public FileOpenDelegate FileOpenMethod { get; }

		/// <summary>
		/// Gets the file new method that can be used to read a document of this type from disk.
		/// This property can be null indicating that this type of document cannot be created
		/// with this module (this document type can only be read and viewed from disk).
		/// </summary>
		public CreateNewDocumentDelegate CreateDocumentMethod { get; }

		/// <summary>
		/// Gets the key of this document type.
		/// </summary>
		public string Key { get; }

		/// <summary>
		/// Gets a description that can be displayed for file open/new/save methods.
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// Gets the sort priority to determine a sort criteria when sorting this
		/// document type against other types in a list of supported document types.
		/// </summary>
		public int SortPriority { get; }

		/// <summary>
		/// Gets the actual type of the viewmodel class that implements this document type.
		/// </summary>
		public Type ClassType { get; }
		#endregion properties

		#region method

		/// <summary>
		/// Convinience methode to create an item for the collection of
		/// <seealso cref="IDocumentTypeItem"/> items managed in this class.
		/// </summary>
		/// <param name="description"></param>
		/// <param name="extensions"></param>
		/// <param name="sortPriority"></param>
		/// <returns></returns>
		public IDocumentTypeItem CreateItem(string description, List<string> extensions, int sortPriority = 0)
		{
			return new DocumentTypeItem(description, extensions, sortPriority);
		}

		public void RegisterFileTypeItem(IDocumentTypeItem fileType)
		{
			if (FileTypeExtensions == null)
				FileTypeExtensions = new List<IDocumentTypeItem>();

			FileTypeExtensions.Add(fileType);
		}

		public string GetFileOpenFilter()
		{
			string ret = string.Empty;

			if (FileTypeExtensions == null)
				return ret;

			foreach (var item in FileTypeExtensions)
			{
				string ext1;

				if (item.DocFileTypeExtensions.Count <= 0)
					continue;

				var ext = ext1 = $"*.{item.DocFileTypeExtensions[0]}";

				for (int i = 1; i < item.DocFileTypeExtensions.Count; i++)
				{
					ext = $"{ext},*.{item.DocFileTypeExtensions[i]}";
					ext1 = $"{ext1};*.{item.DocFileTypeExtensions[i]}";
				}

				// log4net XML output (*.log4j,*.log,*.txt,*.xml)|*.log4j;*.log;*.txt;*.xml
				var s = $"{item.Description} ({ext}) |{ext1}";

				if (ret == string.Empty)
					ret = s;
				else
					ret = ret + "|" + s;

			}

			return ret;
		}

		public void GetFileFilterEntries(SortedList<int, IFileFilterEntry> ret, FileOpenDelegate fileOpenMethod)
		{
			foreach (var item in FileTypeExtensions)
			{
				string ext1;

				if (item.DocFileTypeExtensions.Count <= 0)
					continue;

				var ext = ext1 = $"*.{item.DocFileTypeExtensions[0]}";

				for (int i = 1; i < item.DocFileTypeExtensions.Count; i++)
				{
					ext = $"{ext},*.{item.DocFileTypeExtensions[i]}";
					ext1 = $"{ext1};*.{item.DocFileTypeExtensions[i]}";
				}

				// log4net XML output (*.log4j,*.log,*.txt,*.xml)|*.log4j;*.log;*.txt;*.xml
				var filterString = new FileFilterEntry($"{item.Description} ({ext}) |{ext1}", fileOpenMethod);

				ret.Add(item.SortPriority, filterString);
			}
		}
		#endregion method
	}
}
