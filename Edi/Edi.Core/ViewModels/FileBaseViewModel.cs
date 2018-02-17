namespace Edi.Core.ViewModels
{
    using System;
    using System.Globalization;
    using System.Windows.Input;
    using Interfaces;
    using Interfaces.Documents;
    using Interfaces.Enums;
    using Models.Documents;
    using Command;
    using Events;

    /// <summary>
    /// Base class that shares common properties, methods, and intefaces
    /// among viewmodels that represent documents in Edi
    /// (text file edits, Start Page, Program Settings).
    /// </summary>
    public abstract class FileBaseViewModel : PaneViewModel, IFileBaseViewModel
    {
        #region Fields
        private object lockObject = new object();

        private DocumentState mState = DocumentState.IsLoading;

        private RelayCommand<object> mOpenContainingFolderCommand = null;
        private RelayCommand<object> mCopyFullPathtoClipboard = null;
        private RelayCommand<object> mSyncPathToExplorerCommand = null;

        private readonly string mDocumentTypeKey = string.Empty;

        protected IDocumentModel mDocumentModel = null;
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="documentTypeKey"></param>
        public FileBaseViewModel(string documentTypeKey)
            : this()
        {
            mDocumentModel = new DocumentModel();
            mDocumentTypeKey = documentTypeKey;
        }

        /// <summary>
        /// Standard class constructor.
        /// </summary>
        protected FileBaseViewModel()
        {
        }
        #endregion Constructors

        #region events
        /// <summary>
        /// This event is fired when a document tells the framework that is wants to be closed.
        /// The framework can then close it and clean-up whatever is left to clean-up.
        /// </summary>
        virtual public event EventHandler<FileBaseEvent> DocumentEvent;

        /// <summary>
        /// Supports asynchrone processing by implementing a result event when processing is done.
        /// </summary>
        public event EventHandler<ProcessResultEvent> ProcessingResultEvent;
        #endregion events

        #region properties
        /// <summary>
        /// Gets the key that is associated with the type of this document.
        /// This key is relevant for the framework to implement the correct
        /// file open/save filter settings etc...
        /// </summary>
        public string DocumentTypeKey
        {
            get
            {
                return mDocumentTypeKey;
            }
        }

        /// <summary>
        /// Get/set whether a given file path is a real existing path or not.
        /// 
        /// This is used to identify files that have never been saved and can
        /// those not be remembered in an MRU etc...
        /// </summary>
        public bool IsFilePathReal
        {
            get
            {
                if (mDocumentModel != null)
                    return mDocumentModel.IsReal;

                return false;
            }

            ////set
            ////{
            ////	this.mIsFilePathReal = value;
            ////}
        }

        /// <summary>
        /// Gets the current state of the document. States may differ during
        /// initialization, loading, and other asynchron processings...
        /// </summary>
        public DocumentState State
        {
            get
            {
                lock (lockObject)
                {
                    return mState;
                }
            }

            protected set
            {
                lock (lockObject)
                {
                    if (mState != value)
                    {
                        mState = value;

                        RaisePropertyChanged(() => State);
                    }
                }
            }
        }

        public virtual string FilePath { get; protected set; }

        #region FileName
        /// <summary>
        /// FileName is the string that is displayed whenever the application refers to this file, as in:
        /// string.Format(CultureInfo.CurrentCulture, "Would you like to save the '{0}' file", FileName)
        /// 
        /// Note the absense of the dirty mark '*'. Use the Title property if you want to display the file
        /// name with or without dirty mark when the user has edited content.
        /// </summary>
        abstract public string FileName { get; }
        #endregion FileName

        #region IsDirty
        /// <summary>
        /// Get whether the current information was edit and needs to be saved or not.
        /// </summary>
        abstract public bool IsDirty { get; set; }
        #endregion IsDirty

        #region CanSaveData
        /// <summary>
        /// Get whether edited data can be saved or not.
        /// A type of document does not have a save
        /// data implementation if this property returns false.
        /// (this is document specific and should always be overriden by descendents)
        /// </summary>
        abstract public bool CanSaveData { get; }
        #endregion CanSaveData

        #region Commands
        /// <summary>
        /// This command cloases a single file. The binding for this is in the AvalonDock LayoutPanel Style.
        /// </summary>
        abstract public ICommand CloseCommand
        {
            get;
        }

        /// <summary>
        /// Get open containing folder command which will open
        /// the folder indicated by the path in windows explorer
        /// and select the file (if path points to one).
        /// </summary>
        public ICommand OpenContainingFolderCommand
        {
            get
            {
                if (mOpenContainingFolderCommand == null)
                    mOpenContainingFolderCommand = new RelayCommand<object>((p) => OnOpenContainingFolderCommand());

                return mOpenContainingFolderCommand;
            }
        }


        /// <summary>
        /// Get CopyFullPathtoClipboard command which will copy
        /// the path of the executable into the windows clipboard.
        /// </summary>
        public ICommand CopyFullPathtoClipboard
        {
            get
            {
                if (mCopyFullPathtoClipboard == null)
                    mCopyFullPathtoClipboard = new RelayCommand<object>((p) => OnCopyFullPathtoClipboardCommand());

                return mCopyFullPathtoClipboard;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ICommand SyncPathToExplorerCommand
        {
            get
            {
                if (mSyncPathToExplorerCommand == null)
                    mSyncPathToExplorerCommand = new RelayCommand<object>((p) => OnSyncPathToExplorerCommand());

                return mSyncPathToExplorerCommand;
            }
        }
        #endregion commands

        /// <summary>
        /// Gets/sets a property to indicate whether this
        /// file was changed externally (by another editor) or not.
        /// 
        /// Setter can be used to override re-loading (keep current content)
        /// at the time of detection.
        /// </summary>
        public bool WasChangedExternally
        {
            get
            {
                if (mDocumentModel == null)
                    return false;

                return mDocumentModel.WasChangedExternally;
            }

            private set
            {
                if (mDocumentModel == null)
                    return;

                if (mDocumentModel.WasChangedExternally != value)
                    mDocumentModel.WasChangedExternally = value;
            }
        }
        #endregion properties

        #region methods
        #region abstract methods
        /// <summary>
        /// Indicate whether document can be saved in the currennt state.
        /// </summary>
        /// <returns></returns>
        abstract public bool CanSave();

        /// <summary>
        /// Indicate whether document can be saved as.
        /// </summary>
        /// <returns></returns>
        abstract public bool CanSaveAs();

        /// <summary>
        /// Save this document as.
        /// </summary>
        /// <returns></returns>
        abstract public bool SaveFile(string filePath);

        /// <summary>
        /// Return the path of the file representation (if any).
        /// </summary>
        /// <returns></returns>
        abstract public string GetFilePath();
        #endregion abstract methods

        /// <summary>
        /// Is executed when the user wants to refresh/re-load
        /// the current content with the currently stored inforamtion.
        /// </summary>
        virtual public void ReOpen()
        {
            WasChangedExternally = false;
        }

        /// <summary>
        /// Search for most inner exceptions and return it to caller.
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public Exception GetInnerMostException(Exception exp)
        {
            if (exp != null)
            {
                while (exp.InnerException != null)
                    exp = exp.InnerException;
            }

            if (exp != null)
                return exp;

            return null;
        }

        /// <summary>
        /// Get a path that does not represent this document that indicates
        /// a useful alternative representation (eg: StartPage -> Assembly Path).
        /// </summary>
        /// <returns></returns>
        public string GetAlternativePath()
        {
            return FilePath;
        }

        /// <summary>
        /// This method is executed to tell the surrounding framework to close the document.
        /// </summary>
        protected virtual void OnClose()
        {
            DocumentEvent?.Invoke(this, new FileBaseEvent(FileEventType.CloseDocument));
        }

        /// <summary>
        /// Indicate whether document can be closed or not.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanClose()
        {
            return (DocumentEvent != null);
        }

        private void OnCopyFullPathtoClipboardCommand()
        {
            try
            {
                System.Windows.Clipboard.SetText(FilePath);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Opens the folder in which this document is stored in the Windows Explorer.
        /// </summary>
        private void OnOpenContainingFolderCommand()
        {
            var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();

            try
            {
                if (System.IO.File.Exists(FilePath))
                {
                    // combine the arguments together it doesn't matter if there is a space after ','
                    string argument = @"/select, " + FilePath;

                    System.Diagnostics.Process.Start("explorer.exe", argument);
                }
                else
                {
                    string parentDir = System.IO.Directory.GetParent(FilePath).FullName;

                    if (System.IO.Directory.Exists(parentDir) == false)
                        msgBox.Show(string.Format(CultureInfo.CurrentCulture, Util.Local.Strings.STR_ACCESS_DIRECTORY_ERROR, parentDir),
                                    Util.Local.Strings.STR_FILE_FINDING_CAPTION,
                                    MsgBoxButtons.OK, MsgBoxImage.Error);
                    else
                    {
                        string argument = @"/select, " + parentDir;

                        System.Diagnostics.Process.Start("EXPLORER.EXE", argument);
                    }
                }
            }
            catch (Exception ex)
            {
                msgBox.Show(string.Format(CultureInfo.CurrentCulture, "{0}\n'{1}'.", ex.Message, (FilePath ?? string.Empty)),
                            Util.Local.Strings.STR_FILE_FINDING_CAPTION,
                            MsgBoxButtons.OK, MsgBoxImage.Error);
            }
        }

        /// <summary>
        /// Synchronizes the path of the Explorer Tool Window with
        /// the path of this document.
        /// </summary>
        private void OnSyncPathToExplorerCommand()
        {
            try
            {
                DocumentEvent?.Invoke(this, new FileBaseEvent(FileEventType.AdjustCurrentPath));
            }
            catch
            {
            }
        }

        public bool FireFileProcessingResultEvent(ResultEvent e, TypeOfResult typeOfResult)
        {
            // Continue processing in parent of this viewmodel if there is any such requested
            if (ProcessingResultEvent != null)
            {
                ProcessingResultEvent(this, new ProcessResultEvent(e.Message, e.Error, e.Cancel, typeOfResult,
                                                                                                                                e.ResultObjects, e.InnerException));

                return true;
            }

            return false;
        }

        public void Dispose()
        {
            if (mDocumentModel != null)
            {
                mDocumentModel.Dispose();
                mDocumentModel = null;
            }
        }
        #endregion methods
    }
}
