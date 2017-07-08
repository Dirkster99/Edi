namespace Edi.Documents.ViewModels.MiniUml
{
	using System;
	using System.ComponentModel;
	using System.Globalization;
	using System.IO;
	using System.Windows.Input;
	using Edi.Core.Interfaces;
	using Edi.Core.Interfaces.Documents;
	using Edi.Core.Models.Documents;
	using Edi.Core.ViewModels.Command;
	using MiniUML.Model.ViewModels.Document;
	using MsgBox;

	public class MiniUmlViewModel : Edi.Core.ViewModels.FileBaseViewModel
	{
		#region Fields
		public const string DocumentKey = "UMLEditor";
		public const string Description = "Unified Modeling Language (UML)";
		public const string FileFilterName = "Unified Modeling Language";
		public const string DefaultFilter = "uml";

		private MiniUML.Model.ViewModels.Document.RibbonViewModel mRibbonViewModel;
		private MiniUML.Model.ViewModels.Document.AbstractDocumentViewModel mDocumentMiniUML;

		private static int iNewFileCounter = 1;
		private string defaultFileType = "uml";
		private readonly static string defaultFileName = Util.Local.Strings.STR_FILE_DEFAULTNAME;

		private object lockThis = new object();
		#endregion Fields

		#region constructor
		/// <summary>
		/// Class constructor from <seealso cref="IDocumentModel"/> parameter.
		/// </summary>
		/// <param name="documentModel"></param>
		public MiniUmlViewModel(IDocumentModel documentModel)
		: this ()
		{
			this.mDocumentModel = documentModel;
			this.mDocumentModel.SetFileNamePath(this.FilePath, this.IsFilePathReal);
		}

		/// <summary>
		/// Standard constructor. See also static <seealso cref="LoadFile"/> method
		/// for construction from file saved on disk.
		/// </summary>
		protected MiniUmlViewModel()
			: base(MiniUmlViewModel.DocumentKey)
		{
			this.FilePath = string.Format(CultureInfo.InvariantCulture, "{0} {1}.{2}",
																		MiniUmlViewModel.defaultFileName,
																		MiniUmlViewModel.iNewFileCounter++,
																		this.defaultFileType);

			this.mRibbonViewModel = new RibbonViewModel();

			// The plug-in model name identifies the plug-in that takes care of this document
			// So, the supplied string is required to be in sync with
			//
			// MiniUML.Plugins.UmlClassDiagram.PluginModel.ModelName
			//
			this.mDocumentMiniUML = new MiniUML.Model.ViewModels.Document.DocumentViewModel("UMLClassDiagram");

			this.mDocumentMiniUML.dm_DocumentDataModel.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
			{
				if (e.PropertyName == "HasUnsavedData")
					this.IsDirty = this.mDocumentMiniUML.dm_DocumentDataModel.HasUnsavedData;
			};

			this.IsDirty = false;
		}
		#endregion constructor

		#region properties
		#region MiniUML Document ViewModel
		public MiniUML.Model.ViewModels.Document.AbstractDocumentViewModel DocumentMiniUML
		{
			get
			{
				return this.mDocumentMiniUML;
			}

			protected set
			{
				if (this.mDocumentMiniUML != value)
				{
					this.mDocumentMiniUML = value;

					this.RaisePropertyChanged(() => this.DocumentMiniUML);
				}
			}
		}
		#endregion MiniUML Document ViewModel

		#region MiniUML RibbonViewModel
		public RibbonViewModel Vm_RibbonViewModel
		{
			get
			{
				return this.mRibbonViewModel;
			}

			protected set
			{
				if (this.mRibbonViewModel != value)
				{
					this.mRibbonViewModel = value;

					this.RaisePropertyChanged(() => this.Vm_RibbonViewModel);
				}
			}
		}
		#endregion MiniUML RibbonViewModel

		#region FilePath
		private string mFilePath = null;

		/// <summary>
		/// Get/set complete path including file name to where this stored.
		/// This string is never null or empty.
		/// </summary>
		override public string FilePath
		{
			get
			{
				if (this.mFilePath == null || this.mFilePath == String.Empty)
					return string.Format(CultureInfo.CurrentCulture, "{0}.{1}",
															 MiniUmlViewModel.defaultFileName, this.defaultFileType);

				return this.mFilePath;
			}

			protected set
			{
				if (this.mFilePath != value)
				{
					this.mFilePath = value;

					this.RaisePropertyChanged(() => this.FilePath);
					this.RaisePropertyChanged(() => this.FileName);
					this.RaisePropertyChanged(() => this.Title);
				}
			}
		}
		#endregion

		#region Title
		/// <summary>
		/// Title is the string that is usually displayed - with or without dirty mark '*' - in the docking environment
		/// </summary>
		public override string Title
		{
			get
			{
				return this.FileName + (IsDirty == true ? "*" : string.Empty);
			}
		}
		#endregion

		#region FileName
		/// <summary>
		/// FileName is the string that is displayed whenever the application refers to this file, as in:
		/// string.Format(CultureInfo.CurrentCulture, "Would you like to save the '{0}' file", FileName)
		/// 
		/// Note the absense of the dirty mark '*'. Use the Title property if you want to display the file
		/// name with or without dirty mark when the user has edited content.
		/// </summary>
		public override string FileName
		{
			get
			{
				// This option should never happen - its an emergency break for those cases that never occur
				if (FilePath == null || FilePath == String.Empty)
					return string.Format(CultureInfo.InvariantCulture, "{0}.{1}",
															 MiniUmlViewModel.defaultFileName, this.defaultFileType);

				return System.IO.Path.GetFileName(FilePath);
			}
		}

		public override Uri IconSource
		{
			get
			{
				// This icon is visible in AvalonDock's Document Navigator window
				return new Uri("pack://application:,,,/Themes;component/Images/Documents/MiniUml.png", UriKind.RelativeOrAbsolute);
			}
		}
		#endregion FileName

		#region IsReadOnly
		private bool mIsReadOnly = false;
		public bool IsReadOnly
		{
			get
			{
				return this.mIsReadOnly;
			}

			protected set
			{
				if (this.mIsReadOnly != value)
				{
					this.mIsReadOnly = value;
					this.RaisePropertyChanged(() => this.IsReadOnly);
				}
			}
		}

		private string mIsReadOnlyReason = string.Empty;
		public string IsReadOnlyReason
		{
			get
			{
				return this.mIsReadOnlyReason;
			}

			protected set
			{
				if (this.mIsReadOnlyReason != value)
				{
					this.mIsReadOnlyReason = value;
					this.RaisePropertyChanged(() => this.IsReadOnlyReason);
				}
			}
		}
		#endregion IsReadOnly

		#region IsDirty
		private bool mIsDirty = false;

		/// <summary>
		/// IsDirty indicates whether the file currently loaded
		/// in the editor was modified by the user or not.
		/// </summary>
		override public bool IsDirty
		{
			get
			{
				return mIsDirty;
			}

			set
			{
				if (mIsDirty != value)
				{
					mIsDirty = value;

					this.RaisePropertyChanged(() => this.IsDirty);
					this.RaisePropertyChanged(() => this.Title);
				}
			}
		}
		#endregion

		#region CanSaveData
		/// <summary>
		/// Get whether edited data can be saved or not.
		/// A type of document does not have a save
		/// data implementation if this property returns false.
		/// (this is document specific and should always be overriden by descendents)
		/// </summary>
		override public bool CanSaveData
		{
			get
			{
				return true;
			}
		}
		#endregion CanSaveData

		#region SaveCommand
		/// <summary>
		/// Save the document viewed in this viewmodel.
		/// </summary>
		override public bool CanSave()
		{
			return true;  // IsDirty
		}

		/// <summary>
		/// Write text content to disk and (re-)set associated properties
		/// </summary>
		/// <param name="filePath"></param>
		override public bool SaveFile(string filePath)
		{
			try
			{
				this.mDocumentMiniUML.ExecuteSave(filePath);

				if (this.mDocumentModel == null)
					this.mDocumentModel = new DocumentModel();

				this.mDocumentModel.SetFileNamePath(filePath, true);
				this.FilePath = filePath;
				this.ContentId = filePath;
				this.IsDirty = false;

				return true;
			}
			catch (Exception)
			{
				throw;
			}
		}
		#endregion

		#region SaveAsCommand
		override public bool CanSaveAs()
		{
			return true;  // IsDirty
		}
		#endregion

		#region CloseCommand
		RelayCommand<object> _closeCommand = null;

		/// <summary>
		/// This command cloases a single file. The binding for this is in the AvalonDock LayoutPanel Style.
		/// </summary>
		override public ICommand CloseCommand
		{
			get
			{
				if (_closeCommand == null)
					_closeCommand = new RelayCommand<object>((p) => this.OnClose(),
																									 (p) => this.CanClose());

				return _closeCommand;
			}
		}
		#endregion
		#endregion properties

		#region methods
		/// <summary>
		/// Create a new default document based on the given document model.
		/// </summary>
		/// <param name="documentModel"></param>
		/// <returns></returns>
		public static IFileBaseViewModel CreateNewDocument(IDocumentModel documentModel)
		{
			return new MiniUmlViewModel(documentModel);
		}

		#region LoadFile
		/// <summary>
		/// Load a UML editor file based on an <seealso cref="IDocumentModel"/>
		/// representation and an <seealso cref="ISettingsManager"/> instance.
		/// </summary>
		/// <param name="dm"></param>
		/// <param name="o">Should point to a <seealso cref="ISettingsManager"/> instance.</param>
		/// <returns></returns>
		public static MiniUmlViewModel LoadFile(IDocumentModel dm, object o)
		{
			return MiniUmlViewModel.LoadFile(dm.FileNamePath);
		}

		/// <summary>
		/// Load the content of a MiniUML file and store it for
		/// presentation and manipulation in the returned viewmodel.
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		private static MiniUmlViewModel LoadFile(string filePath)
		{
			MiniUmlViewModel vm = new MiniUmlViewModel();

			if (vm.OpenFile(filePath) == true)
				return vm;

			return null;
		}

		/// <summary>
		/// Attempt to open a file and load it into the viewmodel if it exists.
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns>True if file exists and was succesfully loaded. Otherwise false.</returns>
		protected bool OpenFile(string filePath)
		{
			try
			{
				var isReal = File.Exists(filePath);

				if (isReal == true)
				{
					if (this.mDocumentModel == null)
						this.mDocumentModel = new DocumentModel();

					this.mDocumentModel.SetFileNamePath(filePath, isReal);

					this.FilePath = filePath;
					this.ContentId = this.mFilePath;

					// Mark document loaded from persistence as unedited copy (display without dirty mark '*' in name)
					IsDirty = false;

					try
					{
						// XXX TODO Extend log4net FileOpen method to support base.FireFileProcessingResultEvent(...);
						this.mDocumentMiniUML.LoadFile(this.mFilePath);
					}
					catch (Exception ex)
					{
						MsgBox.Msg.Show(ex, ex.Message, "An error has occurred", MsgBoxButtons.OK);

						return false;
					}
				}
				else
					return false;
			}
			catch (Exception exp)
			{
				MsgBox.Msg.Show(exp, exp.Message, "An error has occurred", MsgBoxButtons.OK);

				return false;
			}

			return true;
		}
		#endregion LoadFile

		/// <summary>
		/// Get the path of the file or empty string if file does not exists on disk.
		/// </summary>
		/// <returns></returns>
		override public string GetFilePath()
		{
			try
			{
				if (System.IO.File.Exists(this.FilePath))
					return System.IO.Path.GetDirectoryName(this.FilePath);
			}
			catch
			{
			}

			return string.Empty;
		}
		#endregion methods
	}
}
