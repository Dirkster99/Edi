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
	public class FileStatsViewModel : Edi.Core.ViewModels.ToolViewModel, IRegisterableToolWindow
	{
		#region fields
		public const string ToolContentId = "<FileStatsTool>";
		public const string ToolName = "File Info";
		private string mFilePathName;

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
		}
		#endregion constructor

		#region properties
		#region FileSize

		private long _fileSize;
		public long FileSize
		{
			get { return _fileSize; }
			set
			{
				if (_fileSize != value)
				{
					_fileSize = value;
					RaisePropertyChanged("FileSize");
				}
			}
		}

		#endregion

		#region LastModified
		private DateTime _lastModified;
		public DateTime LastModified
		{
			get { return _lastModified; }
			set
			{
				if (_lastModified != value)
				{
					_lastModified = value;
					RaisePropertyChanged("LastModified");
				}
			}
		}
		#endregion

		#region FileName
		public string FileName
		{
			get
			{
				if (string.IsNullOrEmpty(this.mFilePathName) == true)
					return string.Empty;

				try
				{
					return System.IO.Path.GetFileName(mFilePathName);
				}
				catch (Exception)
				{
					return string.Empty;
				}
			}
		}
		#endregion

		#region FilePath
		public string FilePath
		{
			get
			{
				if (string.IsNullOrEmpty(this.mFilePathName) == true)
					return string.Empty;

				try
				{
					return System.IO.Path.GetDirectoryName(mFilePathName);
				}
				catch (Exception)
				{
					return string.Empty;
				}
			}
		}
		#endregion

		#region ToolWindow Icon
		public override Uri IconSource
		{
			get
			{
				return new Uri("pack://application:,,,/Edi.Themes;component/Images/property-blue.png", UriKind.RelativeOrAbsolute);
			}
		}
		#endregion ToolWindow Icon

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

		private void OnActiveDocumentChanged(object sender, DocumentChangedEventArgs e)
		{
			this.mFilePathName = string.Empty;
			FileSize = 0;
			LastModified = DateTime.MinValue;

			if (e != null)
			{
				if (e.ActiveDocument != null)
				{

                    if (e.ActiveDocument is FileBaseViewModel)
                    {
                        FileBaseViewModel f = e.ActiveDocument as FileBaseViewModel;


                        if (f.IsFilePathReal == false) // Start page or somethin...
                            return;

                        try
                        {
                            if (File.Exists(f.FilePath) == true)
                            {
                                var fi = new FileInfo(f.FilePath);

                                this.mFilePathName = f.FilePath;

                                this.RaisePropertyChanged(() => this.FileName);
                                this.RaisePropertyChanged(() => this.FilePath);

                                FileSize = fi.Length;
                                LastModified = fi.LastWriteTime;
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
