using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;
using CommonServiceLocator;
using Edi.Core.Interfaces;
using Edi.Core.Interfaces.Documents;
using Edi.Core.Interfaces.Enums;
using Edi.Core.Models.Documents;
using Edi.Core.ViewModels.Command;
using Edi.Core.ViewModels.Events;
using Edi.Util.Local;
using MsgBox;

namespace Edi.Core.ViewModels
{
	/// <summary>
    /// Base class that shares common properties, methods, and intefaces
    /// among viewmodels that represent documents in Edi
    /// (text file edits, Start Page, Program Settings).
    /// </summary>
    public abstract class FileBaseViewModel : PaneViewModel, IFileBaseViewModel
    {
        #region Fields
        private object _lockObject = new object();

        private DocumentState _mState = DocumentState.IsLoading;

        private RelayCommand<object> _mOpenContainingFolderCommand;
        private RelayCommand<object> _mCopyFullPathtoClipboard;
        private RelayCommand<object> _mSyncPathToExplorerCommand;

        private readonly string _mDocumentTypeKey = string.Empty;

        protected IDocumentModel MDocumentModel;
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="documentTypeKey"></param>
        public FileBaseViewModel(string documentTypeKey)
            : this()
        {
            MDocumentModel = new DocumentModel();
            _mDocumentTypeKey = documentTypeKey;
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
        public virtual event EventHandler<FileBaseEvent> DocumentEvent;

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
                return _mDocumentTypeKey;
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
                if (MDocumentModel != null)
                    return MDocumentModel.IsReal;

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
                lock (_lockObject)
                {
                    return _mState;
                }
            }

            protected set
            {
                lock (_lockObject)
                {
                    if (_mState != value)
                    {
                        _mState = value;

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
        public abstract string FileName { get; }
        #endregion FileName

        #region IsDirty
        /// <summary>
        /// Get whether the current information was edit and needs to be saved or not.
        /// </summary>
        public abstract bool IsDirty { get; set; }
        #endregion IsDirty

        #region CanSaveData
        /// <summary>
        /// Get whether edited data can be saved or not.
        /// A type of document does not have a save
        /// data implementation if this property returns false.
        /// (this is document specific and should always be overriden by descendents)
        /// </summary>
        public abstract bool CanSaveData { get; }
        #endregion CanSaveData

        #region Commands
        /// <summary>
        /// This command cloases a single file. The binding for this is in the AvalonDock LayoutPanel Style.
        /// </summary>
        public abstract ICommand CloseCommand
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
                if (_mOpenContainingFolderCommand == null)
                    _mOpenContainingFolderCommand = new RelayCommand<object>(p => OnOpenContainingFolderCommand());

                return _mOpenContainingFolderCommand;
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
                if (_mCopyFullPathtoClipboard == null)
                    _mCopyFullPathtoClipboard = new RelayCommand<object>(p => OnCopyFullPathtoClipboardCommand());

                return _mCopyFullPathtoClipboard;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ICommand SyncPathToExplorerCommand
        {
            get
            {
                if (_mSyncPathToExplorerCommand == null)
                    _mSyncPathToExplorerCommand = new RelayCommand<object>(p => OnSyncPathToExplorerCommand());

                return _mSyncPathToExplorerCommand;
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
                if (MDocumentModel == null)
                    return false;

                return MDocumentModel.WasChangedExternally;
            }

            private set
            {
                if (MDocumentModel == null)
                    return;

                if (MDocumentModel.WasChangedExternally != value)
                    MDocumentModel.WasChangedExternally = value;
            }
        }
        #endregion properties

        #region methods
        #region abstract methods
        /// <summary>
        /// Indicate whether document can be saved in the currennt state.
        /// </summary>
        /// <returns></returns>
        public abstract bool CanSave();

        /// <summary>
        /// Indicate whether document can be saved as.
        /// </summary>
        /// <returns></returns>
        public abstract bool CanSaveAs();

        /// <summary>
        /// Save this document as.
        /// </summary>
        /// <returns></returns>
        public abstract bool SaveFile(string filePath);

        /// <summary>
        /// Return the path of the file representation (if any).
        /// </summary>
        /// <returns></returns>
        public abstract string GetFilePath();
        #endregion abstract methods

        /// <summary>
        /// Is executed when the user wants to refresh/re-load
        /// the current content with the currently stored inforamtion.
        /// </summary>
        public virtual void ReOpen()
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
                Clipboard.SetText(FilePath);
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
                if (File.Exists(FilePath))
                {
                    // combine the arguments together it doesn't matter if there is a space after ','
                    string argument = @"/select, " + FilePath;

                    Process.Start("explorer.exe", argument);
                }
                else
                {
                    string parentDir = Directory.GetParent(FilePath).FullName;

                    if (Directory.Exists(parentDir) == false)
                        msgBox.Show(string.Format(CultureInfo.CurrentCulture, Strings.STR_ACCESS_DIRECTORY_ERROR, parentDir),
                                    Strings.STR_FILE_FINDING_CAPTION,
                                    MsgBoxButtons.OK, MsgBoxImage.Error);
                    else
                    {
                        string argument = @"/select, " + parentDir;

                        Process.Start("EXPLORER.EXE", argument);
                    }
                }
            }
            catch (Exception ex)
            {
                msgBox.Show(string.Format(CultureInfo.CurrentCulture, "{0}\n'{1}'.", ex.Message, (FilePath ?? string.Empty)),
                            Strings.STR_FILE_FINDING_CAPTION,
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
            if (MDocumentModel != null)
            {
                MDocumentModel.Dispose();
                MDocumentModel = null;
            }
        }
        #endregion methods
    }
}
