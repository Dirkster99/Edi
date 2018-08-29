namespace Files.ViewModels.FileStats
{
	using System;
	using System.IO;
	using Edi.Core.Interfaces;
	using Edi.Core.Interfaces.Enums;
	using Edi.Core.ViewModels;

	/// <summary>
	/// This class can be used to present file based information, such as,
	/// Size, Path etc to the user.
	/// </summary>
	internal class FileStatsViewModel : Edi.Core.ViewModels.ToolViewModel, IRegisterableToolWindow
	{
		#region fields
		public const string ToolContentId = "<FileStatsTool>";
		public const string ToolName = "File Info";
		private string _FilePathName;
        private DateTime _lastModified;
        private string _FilePath;
        private long _FileSize;

        private IDocumentParent mParent = null;
		#endregion fields

		#region constructor
		/// <summary>
		/// Class constructor
		/// </summary>
		public FileStatsViewModel()
			: base(FileStatsViewModel.ToolName)
		{
			ContentId = FileStatsViewModel.ToolContentId;

			this.OnActiveDocumentChanged(null, null);

            _FilePath = string.Empty;
        }
		#endregion constructor

		#region properties
        /// <summary>
        /// Gets the size of the file in bytes.
        /// </summary>
		public long FileSize
		{
			get { return _FileSize; }
			private set
			{
				if (_FileSize != value)
				{
					_FileSize = value;
					RaisePropertyChanged("FileSize");
				}
			}
		}

        /// <summary>
        /// Gets the date and time of the time when the displayed
        /// file was modified on storage space.
        /// </summary>
        public DateTime LastModified
		{
			get { return _lastModified; }
			private set
			{
				if (_lastModified != value)
				{
					_lastModified = value;
					RaisePropertyChanged("LastModified");
				}
			}
		}

        /// <summary>
        /// Gets the file name for the currently displayed file infornation.
        /// </summary>
        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(_FilePathName) == true)
                    return string.Empty;

                try
                {
                    return System.IO.Path.GetFileName(_FilePathName);
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets the full directory for the currently displayed file infornation.
        /// </summary>
        public string FilePath
        {
            get { return _FilePath; }
            private set
            {
                if (_FilePath != value)
                {
                    _FilePath = value;
                    RaisePropertyChanged(() => FilePath);
                }
            }
        }

        /// <summary>
        /// Gets the Uri of the Icon Resource for this tool window.
        /// </summary>
        public override Uri IconSource
		{
			get
			{
				return new Uri("pack://application:,,,/Edi.Themes;component/Images/property-blue.png", UriKind.RelativeOrAbsolute);
			}
		}

        /// <summary>
        /// Gets the preferred location of the tool window
        /// (for positioning it for the very first time).
        /// </summary>
		public override PaneLocation PreferredLocation
		{
			get { return PaneLocation.Right; }
		}
		#endregion properties

		#region methods
		/// <summary>
		/// Set the document parent handling object to deactivation and activation
		/// of documents with content relevant to this tool window viewmodel.
		/// </summary>
		/// <param name="parent"></param>
		public void SetDocumentParent(IDocumentParent parent)
		{
			if (parent != null)
				parent.ActiveDocumentChanged -= this.OnActiveDocumentChanged;

			this.mParent = parent;

			// Check if active document is a log4net document to display data for...
			if (this.mParent != null)
				parent.ActiveDocumentChanged += new DocumentChangedEventHandler(this.OnActiveDocumentChanged);
			else
				this.OnActiveDocumentChanged(null, null);
		}

		/// <summary>
		/// Set the document parent handling object and visibility
		/// to enable tool window to react on deactivation and activation
		/// of documents with content relevant to this tool window viewmodel.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="isVisible"></param>
		public void SetToolWindowVisibility(IDocumentParent parent,
																				bool isVisible = true)
		{
			if (IsVisible == true)
				this.SetDocumentParent(parent);
			else
				this.SetDocumentParent(null);

			base.SetToolWindowVisibility(isVisible);
		}

        /// <summary>
        /// Method executes when (user in) AvalonDock has (loaded) selected another document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void OnActiveDocumentChanged(object sender, DocumentChangedEventArgs e)
		{
			_FilePathName = string.Empty;
			FileSize = 0;
			LastModified = DateTime.MinValue;

			if (e != null)
			{
				if (e.ActiveDocument != null)
				{
                    if (e.ActiveDocument is IFileBaseViewModel)
                    {
                        IFileBaseViewModel f = e.ActiveDocument as IFileBaseViewModel;


                        if (f.IsFilePathReal == false) // Start page or somethin...
                            return;

                        try
                        {
                            if (File.Exists(f.FilePath) == true)
                            {
                                var fi = new FileInfo(f.FilePath);

                                _FilePathName = f.FilePath;

                                this.RaisePropertyChanged(() => this.FileName);

                                FileSize = fi.Length;
                                LastModified = fi.LastWriteTime;

                                FilePath = fi.DirectoryName;
                            }
                        }
                        catch { }
                    }
                }
			}
		}
		#endregion methods
	}
}
