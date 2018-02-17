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
	public class FileStatsViewModel : ToolViewModel, IRegisterableToolWindow
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
			: base(ToolName)
		{
			ContentId = ToolContentId;

			OnActiveDocumentChanged(null, null);
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
				if (string.IsNullOrEmpty(mFilePathName))
					return string.Empty;

				try
				{
					return Path.GetFileName(mFilePathName);
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
				if (string.IsNullOrEmpty(mFilePathName))
					return string.Empty;

				try
				{
					return Path.GetDirectoryName(mFilePathName);
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
				parent.ActiveDocumentChanged -= OnActiveDocumentChanged;

			mParent = parent;

			// Check if active document is a log4net document to display data for...
			if (mParent != null)
				parent.ActiveDocumentChanged += new DocumentChangedEventHandler(OnActiveDocumentChanged);
			else
				OnActiveDocumentChanged(null, null);
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
			if (IsVisible)
				SetDocumentParent(parent);
			else
				SetDocumentParent(null);

			base.SetToolWindowVisibility(isVisible);
		}

		private void OnActiveDocumentChanged(object sender, DocumentChangedEventArgs e)
		{
			mFilePathName = string.Empty;
			FileSize = 0;
			LastModified = DateTime.MinValue;

			if (e != null)
			{
				if (e.ActiveDocument != null)
				{

                    if (e.ActiveDocument is FileBaseViewModel)
                    {
                        FileBaseViewModel f = e.ActiveDocument as FileBaseViewModel;

                        if (File.Exists(f.FilePath))
                        {
                            var fi = new FileInfo(f.FilePath);

                            mFilePathName = f.FilePath;

                            RaisePropertyChanged(() => FileName);
                            RaisePropertyChanged(() => FilePath);

                            FileSize = fi.Length;
                            LastModified = fi.LastWriteTime;
                        }
                    }
                }
			}
		}
		#endregion methods
	}
}
