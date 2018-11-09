namespace Edi.Core.Models.DocumentTypes
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using Edi.Core.Interfaces.DocumentTypes;
    using Edi.Core.Utillities;
    using Edi.Interfaces.MessageManager;

    /// <summary>
    /// Class manages a list of document types (text, UML, Log4Net etc)
    /// and their associated methods and properties in the framework.
    /// </summary>
    public class DocumentTypeManager : IDocumentTypeManager
	{
		#region fields
		private readonly SortableObservableCollection<IDocumentType> _DocumentTypes;
		#endregion fields

		#region constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		public DocumentTypeManager(IMessageManager messageManager)
            : this()
		{
            this.Messaging = messageManager;
		}

        /// <summary>
        /// Class constructor.
        /// </summary>
        protected DocumentTypeManager()
        {
            _DocumentTypes = new SortableObservableCollection<IDocumentType>(new List<IDocumentType>());
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets a collection of document types that are supported by this application.
        /// </summary>
        public ObservableCollection<IDocumentType> DocumentTypes
        {
            get { return _DocumentTypes; }
        }

        /// <summary>
        /// Gets a reference to the <see cref="IMessageManager"/> service which
        /// can be used to output information about the progress of the
        /// tool window registration.
        /// </summary>
        protected IMessageManager Messaging { get; }
        #endregion properties

        #region Methods
        /// <summary>
        /// Method can be invoked in PRISM MEF module registration to register a new document (viewmodel)
        /// type and its default file extension. The result of this call is an <seealso cref="IDocumentType"/>
        /// object and a <seealso cref="RegisterDocumentTypeEvent"/> event to inform listers about the new
        /// arrival of the new document type.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="description"></param>
        /// <param name="fileFilterName"></param>
        /// <param name="defaultFilter"></param>
        /// <param name="fileOpenMethod"></param>
        /// <param name="createDocumentMethod"></param>
        /// <param name="t"></param>
        /// <param name="sortPriority"></param>
        /// <returns></returns>
        public IDocumentType RegisterDocumentType(string key,
												  string description,
												  string fileFilterName,
												  string defaultFilter,
												  FileOpenDelegate fileOpenMethod,
												  CreateNewDocumentDelegate createDocumentMethod,
												  Type t,
												  int sortPriority = 0)
		{
            try
            {
                Messaging.Output.Append(string.Format("{0} Registering document type: {1} ...",
                    DateTime.Now.ToLongTimeString(), description));

                var newFileType = new DocumentType(key, description, fileFilterName, defaultFilter,
                                                                                     fileOpenMethod, createDocumentMethod,
                                                                                     t, sortPriority);

                _DocumentTypes.Add(newFileType);
                _DocumentTypes.Sort(i => i.SortPriority, ListSortDirection.Ascending);

                return newFileType;
            }
            catch (Exception exp)
            {
                Messaging.Output.AppendLine(exp.Message);
                Messaging.Output.AppendLine(exp.StackTrace);
            }
            finally
            {
                Messaging.Output.AppendLine("Done.");
            }

            return null;
		}

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
		public IDocumentType FindDocumentTypeByExtension(string fileExtension,
		                                                 bool trimPeriod = false)
		{
			if (string.IsNullOrEmpty(fileExtension))
				return null;

			if (trimPeriod)
			{
				int idx;

				if ((idx = fileExtension.LastIndexOf(".", StringComparison.Ordinal)) >= 0)
					fileExtension = fileExtension.Substring(idx + 1);
			}

			if (string.IsNullOrEmpty(fileExtension))
				return null;

			var ret = _DocumentTypes.FirstOrDefault(d => d.DefaultFilter == fileExtension);

			return ret;
		}

		/// <summary>
		/// Find a document type based on its key.
		/// </summary>
		/// <param name="typeOfDoc"></param>
		/// <returns></returns>
		public IDocumentType FindDocumentTypeByKey(string typeOfDoc)
		{
			if (string.IsNullOrEmpty(typeOfDoc))
				return null;

			return _DocumentTypes.FirstOrDefault(d => d.Key == typeOfDoc);
		}

		/// <summary>
		/// Goes through all file/document type definitions and returns a filter string
		/// object that can be used in conjunction with FileOpen and FileSave dialog filters.
		/// </summary>
		/// <param name="key">Get entries for this viewmodel only,
		/// or all entries if key parameter is not set.</param>
		/// <returns></returns>
		public IFileFilterEntries GetFileFilterEntries(string key = "")
		{
			SortedList<int, IFileFilterEntry> ret = new SortedList<int,IFileFilterEntry>();

			if (_DocumentTypes != null)
			{
				foreach (var item in _DocumentTypes)
				{
					if (key == string.Empty || key == item.Key)
					{
						// format filter entry like "Structured Query Language (*.sql) |*.sql"
						var s = new FileFilterEntry($"{item.FileFilterName} (*.{item.DefaultFilter}) |*.{item.DefaultFilter}",
																											item.FileOpenMethod);

						ret.Add(item.SortPriority, s);

						// Add all file sub-filters for this viewmodel class
						item.GetFileFilterEntries(ret, item.FileOpenMethod);
					}
				}
			}

			List<IFileFilterEntry> list = new List<IFileFilterEntry>();

			foreach (var item in ret)
				list.Add(item.Value);

			return new FileFilterEntries(list);
		}
		#endregion Methods
	}
}
