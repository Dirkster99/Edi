namespace Edi.Documents.ViewModels.MiniUml
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Windows.Input;
    using CommonServiceLocator;
    using Core.Interfaces;
    using Core.Interfaces.Documents;
    using Core.Models.Documents;
    using Core.ViewModels.Command;
    using MiniUML.Model.ViewModels.Document;
    using MsgBox;

    public class MiniUmlViewModel : Core.ViewModels.FileBaseViewModel
	{
		#region Fields
		public const string DocumentKey = "UmlEditor";
		public const string Description = "Unified Modeling Language (UML)";
		public const string FileFilterName = "Unified Modeling Language";
		public const string DefaultFilter = "uml";

		private RibbonViewModel _mRibbonViewModel;
		private AbstractDocumentViewModel _mDocumentMiniUml;

		private static int _iNewFileCounter = 1;
		private string _defaultFileType = "uml";
		private static readonly string DefaultFileName = Util.Local.Strings.STR_FILE_DEFAULTNAME;

	    #endregion Fields

		#region constructor
		/// <summary>
		/// Class constructor from <seealso cref="IDocumentModel"/> parameter.
		/// </summary>
		/// <param name="documentModel"></param>
		public MiniUmlViewModel(IDocumentModel documentModel)
		: this ()
		{
			MDocumentModel = documentModel;
			MDocumentModel.SetFileNamePath(FilePath, IsFilePathReal);
		}

		/// <summary>
		/// Standard constructor. See also static <seealso cref="LoadFile"/> method
		/// for construction from file saved on disk.
		/// </summary>
		protected MiniUmlViewModel()
			: base(DocumentKey)
		{
			FilePath = string.Format(CultureInfo.InvariantCulture, "{0} {1}.{2}",
																		DefaultFileName,
																		_iNewFileCounter++,
																		_defaultFileType);

			_mRibbonViewModel = new RibbonViewModel();

			// The plug-in model name identifies the plug-in that takes care of this document
			// So, the supplied string is required to be in sync with
			//
			// MiniUML.Plugins.UmlClassDiagram.PluginModel.ModelName
			//
			_mDocumentMiniUml = new DocumentViewModel("UMLClassDiagram");

			_mDocumentMiniUml.dm_DocumentDataModel.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
			{
				if (e.PropertyName == "HasUnsavedData")
					IsDirty = _mDocumentMiniUml.dm_DocumentDataModel.HasUnsavedData;
			};

			IsDirty = false;
		}
		#endregion constructor

		#region properties
		#region MiniUML Document ViewModel
		public AbstractDocumentViewModel DocumentMiniUml
		{
			get { return _mDocumentMiniUml; }

		    protected set
			{
				if (_mDocumentMiniUml != value)
				{
					_mDocumentMiniUml = value;

					RaisePropertyChanged(() => DocumentMiniUml);
				}
			}
		}
		#endregion MiniUML Document ViewModel

		#region MiniUML RibbonViewModel
		public RibbonViewModel vm_RibbonViewModel
		{
			get { return _mRibbonViewModel; }

		    protected set
			{
				if (_mRibbonViewModel != value)
				{
					_mRibbonViewModel = value;

					RaisePropertyChanged(() => vm_RibbonViewModel);
				}
			}
		}
		#endregion MiniUML RibbonViewModel

		#region FilePath
		private string _mFilePath;

		/// <summary>
		/// Get/set complete path including file name to where this stored.
		/// This string is never null or empty.
		/// </summary>
		public sealed override string FilePath
		{
			get
			{
				if (string.IsNullOrEmpty(_mFilePath))
					return string.Format(CultureInfo.CurrentCulture, "{0}.{1}",
															 DefaultFileName, _defaultFileType);

				return _mFilePath;
			}

			protected set
			{
				if (_mFilePath != value)
				{
					_mFilePath = value;

					RaisePropertyChanged(() => FilePath);
					RaisePropertyChanged(() => FileName);
					RaisePropertyChanged(() => Title);
				}
			}
		}
		#endregion

		#region Title
		/// <summary>
		/// Title is the string that is usually displayed - with or without dirty mark '*' - in the docking environment
		/// </summary>
		public override string Title => FileName + (IsDirty == true ? "*" : string.Empty);

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
				if (string.IsNullOrEmpty(FilePath))
					return string.Format(CultureInfo.InvariantCulture, "{0}.{1}",
															 DefaultFileName, _defaultFileType);

				return Path.GetFileName(FilePath);
			}
		}

		public override Uri IconSource => new Uri("pack://application:,,,/Edi.Themes;component/Images/Documents/MiniUml.png", UriKind.RelativeOrAbsolute);

	    #endregion FileName

		#region IsReadOnly
		private bool _mIsReadOnly;
		public bool IsReadOnly
		{
			get => _mIsReadOnly;

		    protected set
			{
				if (_mIsReadOnly != value)
				{
					_mIsReadOnly = value;
					RaisePropertyChanged(() => IsReadOnly);
				}
			}
		}

		private string _mIsReadOnlyReason = string.Empty;
		public string IsReadOnlyReason
		{
			get => _mIsReadOnlyReason;

		    protected set
			{
				if (_mIsReadOnlyReason != value)
				{
					_mIsReadOnlyReason = value;
					RaisePropertyChanged(() => IsReadOnlyReason);
				}
			}
		}
		#endregion IsReadOnly

		#region IsDirty
		private bool _mIsDirty;

		/// <summary>
		/// IsDirty indicates whether the file currently loaded
		/// in the editor was modified by the user or not.
		/// </summary>
		public override bool IsDirty
		{
			get => _mIsDirty;

		    set
			{
				if (_mIsDirty != value)
				{
					_mIsDirty = value;

					RaisePropertyChanged(() => IsDirty);
					RaisePropertyChanged(() => Title);
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
		public override bool CanSaveData => true;

	    #endregion CanSaveData

		#region SaveCommand
		/// <summary>
		/// Save the document viewed in this viewmodel.
		/// </summary>
		public override bool CanSave()
		{
			return true;  // IsDirty
		}

		/// <summary>
		/// Write text content to disk and (re-)set associated properties
		/// </summary>
		/// <param name="filePath"></param>
		public override bool SaveFile(string filePath)
		{
		    _mDocumentMiniUml.ExecuteSave(filePath);

		    if (MDocumentModel == null)
		        MDocumentModel = new DocumentModel();

		    MDocumentModel.SetFileNamePath(filePath, true);
		    FilePath = filePath;
		    ContentId = filePath;
		    IsDirty = false;

		    return true;
		}
		#endregion

		#region SaveAsCommand
		public override bool CanSaveAs()
		{
			return true;  // IsDirty
		}
		#endregion

		#region CloseCommand

	    private RelayCommand<object> _closeCommand;

		/// <summary>
		/// This command cloases a single file. The binding for this is in the AvalonDock LayoutPanel Style.
		/// </summary>
		public override ICommand CloseCommand
		{
			get
			{
			    return _closeCommand ?? (_closeCommand = new RelayCommand<object>((p) => OnClose(),
			               (p) => CanClose()));
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
			return LoadFile(dm.FileNamePath);
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
					if (MDocumentModel == null)
						MDocumentModel = new DocumentModel();

					MDocumentModel.SetFileNamePath(filePath, isReal);

					FilePath = filePath;
					ContentId = _mFilePath;

					// Mark document loaded from persistence as unedited copy (display without dirty mark '*' in name)
					IsDirty = false;

					try
					{
						// XXX TODO Extend log4net FileOpen method to support base.FireFileProcessingResultEvent(...);
						_mDocumentMiniUml.LoadFile(_mFilePath);
					}
					catch (Exception ex)
					{
                        var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
                        msgBox.Show(ex, ex.Message, "An error has occurred", MsgBoxButtons.OK);

						return false;
					}
				}
				else
					return false;
			}
			catch (Exception exp)
			{
                var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
                msgBox.Show(exp, exp.Message, "An error has occurred", MsgBoxButtons.OK);

				return false;
			}

			return true;
		}
		#endregion LoadFile

		/// <summary>
		/// Get the path of the file or empty string if file does not exists on disk.
		/// </summary>
		/// <returns></returns>
		public override string GetFilePath()
		{
			try
			{
				if (File.Exists(FilePath))
					return Path.GetDirectoryName(FilePath);
			}
		    catch
		    {
		        // ignored
		    }

		    return string.Empty;
		}
		#endregion methods
	}
}
