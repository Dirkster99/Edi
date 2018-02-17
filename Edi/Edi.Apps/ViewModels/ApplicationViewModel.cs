namespace Edi.Apps.ViewModels
{
    using Enums;
    using Interfaces.ViewModel;
    using Core.Interfaces;
    using Core.Interfaces.Documents;
    using Core.Interfaces.DocumentTypes;
    using Core.Models.Documents;
    using Core.ViewModels;
    using Core.ViewModels.Base;
    using Core.ViewModels.Command;
    using Core.ViewModels.Events;
    using Dialogs.About;
    using Documents.ViewModels.EdiDoc;
    using Documents.ViewModels.MiniUml;
    using Documents.ViewModels.StartPage;
    using Settings.Interfaces;
    using SettingsView.Config.ViewModels;
    using Themes.Interfaces;
    using EdiApp.Events;   // XXX TODO Implementation in Edi.Core should have a different namespace
    using Files.ViewModels.RecentFiles;
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    /// <summary>
    /// This class manages the complete application life cyle from start to end.
    /// It publishes the methodes, properties, and events necessary to integrate
    /// the application into a given shell (BootStrapper, App.xaml.cs etc).
    /// </summary>
    [Export(typeof(IApplicationViewModel))]
    [Export(typeof(IFileOpenService))]
    public partial class ApplicationViewModel : ViewModelBase,
                                                IViewModelResolver,
                                                IApplicationViewModel,
                                                IDocumentParent,
                                                IFileOpenService
    {
        #region fields
        public const string Log4netFileExtension = "log4j";
        public static readonly string Log4netFileFilter = Util.Local.Strings.STR_FileType_FileFilter_Log4j;

        public const string MiniUMLFileExtension = "uml";
        public static readonly string UMLFileFilter = Util.Local.Strings.STR_FileType_FileFilter_UML;

        private static string EdiTextEditorFileFilter =
                                    Util.Local.Strings.STR_FileType_FileFilter_AllFiles +
                                    "|" + Util.Local.Strings.STR_FileType_FileFilter_TextFiles +
                                    "|" + Util.Local.Strings.STR_FileType_FileFilter_CSharp +
                                    "|" + Util.Local.Strings.STR_FileType_FileFilter_HTML +
                                    "|" + Util.Local.Strings.STR_FileType_FileFilter_SQL;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool? mDialogCloseResult;
        private bool? mIsNotMaximized;
        private bool mIsWorkspaceAreaOptimized;

        private bool mShutDownInProgress;
        private bool mShutDownInProgress_Cancel;

        private ObservableCollection<IFileBaseViewModel> mFiles;
        private ReadOnlyObservableCollection<IFileBaseViewModel> mReadonyFiles;

        private IFileBaseViewModel mActiveDocument;
        private RelayCommand<object> mMainWindowActivated;

        private readonly IModuleManager mModuleManager = null;
        private readonly IAppCoreModel mAppCore;
        private readonly IToolWindowRegistry mToolRegistry;
        private readonly ISettingsManager mSettingsManager;
        private readonly IMessageManager mMessageManager;

        private readonly IDocumentTypeManager mDocumentTypeManager;
        private IDocumentType mSelectedOpenDocumentType;

        private readonly IMessageBoxService _MsgBox = null;
        #endregion fields

        #region constructor
        /// <summary>
        /// Constructor
        /// </summary>
        [ImportingConstructor]
        public ApplicationViewModel(IAppCoreModel appCore,
                                    IAvalonDockLayoutViewModel avLayout,
                                    IToolWindowRegistry toolRegistry,
                                    IMessageManager messageManager,
                                    IModuleManager moduleManager,
                                    ISettingsManager programSettings,
                                    IThemesManager themesManager,
                                    IDocumentTypeManager documentTypeManager)
            : this()
        {
            mAppCore = appCore;
            ADLayout = avLayout;
            mModuleManager = moduleManager;

            mMessageManager = messageManager;
            if (messageManager.MessageBox != null)   // reset messagebox service
                _MsgBox = messageManager.MessageBox;
            else
            {
                _MsgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
            }

            mToolRegistry = toolRegistry;
            mSettingsManager = programSettings;
            ApplicationThemes = themesManager;
            mDocumentTypeManager = documentTypeManager;

            mModuleManager.LoadModuleCompleted += this.ModuleManager_LoadModuleCompleted;
        }

        public ApplicationViewModel()
        {
            ADLayout = null;
            mFiles = new ObservableCollection<IFileBaseViewModel>();

            // Subscribe to publsihers who relay the fact that a new tool window has been registered
            // Register this methods to receive PRISM event notifications
            RegisterToolWindowEvent.Instance.Subscribe(this.OnRegisterToolWindow, ThreadOption.BackgroundThread);
        }
        #endregion constructor

        #region events
        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        public event EventHandler RequestClose;

        /// <summary>
        /// The document with the current input focus has changed when this event fires.
        /// </summary>
        public event DocumentChangedEventHandler ActiveDocumentChanged;
        #endregion events

        #region Properties
        /// <summary>
        /// Gets an instance of the current application theme manager.
        /// </summary>
        public IThemesManager ApplicationThemes { get; }

        private object mLock = new object();
        private bool mIsMainWindowActivationProcessed;
        private bool mIsMainWindowActivationProcessingEnabled;

        /// <summary>
        /// Activates/deactivates processing of the mainwindow activated event.
        /// </summary>
        /// <param name="bActivate"></param>
        public void EnableMainWindowActivated(bool bActivate)
        {
            mIsMainWindowActivationProcessingEnabled = bActivate;
        }

        /// <summary>
        /// Gets a property to a <seealso cref="ICommand"/> that executes
        /// when the user activates the mainwindow (eg: does ALT+TAB between applications).
        /// This event is used to check whether a file has chnaged in the meantime or not.
        /// </summary>
        public ICommand MainWindowActivated
        {
            get
            {
                if (mMainWindowActivated == null)
                    mMainWindowActivated = new RelayCommand<object>((p) =>
                    {
                        // Is processing of this event currently enabled?
                        if (mIsMainWindowActivationProcessingEnabled == false)
                            return;

                        // Is this event already currently being processed?
                        if (mIsMainWindowActivationProcessed)
                            return;

                        lock (mLock)
                        {
                            try
                            {
                                if (mIsMainWindowActivationProcessed)
                                    return;

                                mIsMainWindowActivationProcessed = true;

                                foreach (var item in Files)
                                {
                                    if (item.WasChangedExternally)
                                    {
                                        var result = _MsgBox.Show(
                                            $"File '{item.FileName}' was changed externally. Click OK to reload or Cancel to keep current content.",
                                                                  "File changed externally", MsgBoxButtons.OKCancel);

                                        if (result == MsgBoxResult.OK)
                                        {
                                            item.ReOpen();
                                        }
                                    }
                                }
                            }
                            catch (Exception exp)
                            {
                                logger.Error(exp.Message, exp);
                                _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                                             mAppCore.IssueTrackerLink, mAppCore.IssueTrackerLink,
                                             Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
                            }
                            finally
                            {
                                mIsMainWindowActivationProcessed = false;
                            }
                        }
                    });

                return mMainWindowActivated;
            }
        }

        #region ActiveDocument
        /// <summary>
        /// Gets/sets the dcoument that is currently active (has input focus) - if any.
        /// </summary>
        public IFileBaseViewModel ActiveDocument
        {
            get => mActiveDocument;

            set
            {
                if (mActiveDocument != value)
                {
                    mActiveDocument = value;

                    RaisePropertyChanged(() => ActiveDocument);
                    RaisePropertyChanged(() => ActiveEdiDocument);
                    RaisePropertyChanged(() => vm_DocumentViewModel);

                    // Ensure that no pending calls are in the dispatcher queue
                    // This makes sure that we are blocked until bindings are re-established
                    // (Bindings are, for example, required to scroll a selection into view for search/replace)
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (Action)delegate
                    {
                        if (ActiveDocumentChanged != null)
                        {
                            ActiveDocumentChanged(this, new DocumentChangedEventArgs(mActiveDocument)); //this.ActiveDocument

                            if (value != null && mShutDownInProgress == false)
                            {
                                if (value.IsFilePathReal)
                                    mSettingsManager.SessionData.LastActiveFile = value.FilePath;
                            }
                        }
                    });
                }
            }
        }

        /// <summary>
        /// This is a type safe ActiveDocument property that is used to bind
        /// to an ActiveDocument of type <seealso cref="EdiViewModel"/>.
        /// This property returns null (thus avoiding binding errors) if the
        /// ActiveDocument is not of <seealso cref="EdiViewModel"/> type.
        /// </summary>
        public EdiViewModel ActiveEdiDocument => mActiveDocument as EdiViewModel;

        /// <summary>
        /// This is a type safe ActiveDocument property that is used to bind
        /// to an ActiveDocument of type <seealso cref="MiniUML.Model.ViewModels.DocumentViewModel"/>.
        /// This property returns null (thus avoiding binding errors) if the
        /// ActiveDocument is not of <seealso cref="MiniUML.Model.ViewModels.DocumentViewModel"/> type.
        /// 
        /// This particular property is also required to load MiniUML Plugins.
        /// </summary>
        public MiniUML.Model.ViewModels.Document.AbstractDocumentViewModel vm_DocumentViewModel
        {
            get
            {
                MiniUmlViewModel vm = mActiveDocument as MiniUmlViewModel;

                return vm?.DocumentMiniUML;
            }
        }
        #endregion

        public IDocumentType SelectedOpenDocumentType
        {
            get => mSelectedOpenDocumentType;

            private set
            {
                if (mSelectedOpenDocumentType != value)
                {
                    mSelectedOpenDocumentType = value;
                    RaisePropertyChanged(() => SelectedOpenDocumentType);
                }
            }
        }

        public ObservableCollection<IDocumentType> DocumentTypes => mDocumentTypeManager.DocumentTypes;

        /// <summary>
        /// Principable data source for collection of documents managed in the the document manager (of AvalonDock).
        /// </summary>
        public ReadOnlyObservableCollection<IFileBaseViewModel> Files => mReadonyFiles ?? (mReadonyFiles = new ReadOnlyObservableCollection<IFileBaseViewModel>(mFiles));

        /// <summary>
        /// Principable data source for collection of tool window viewmodels
        /// whos view templating is managed in the the document manager of AvalonDock.
        /// </summary>
        public ObservableCollection<ToolViewModel> Tools => mToolRegistry.Tools;

        public RecentFilesViewModel RecentFiles
        {
            get
            {
                var ret = GetToolWindowVM<RecentFilesViewModel>();

                return ret;
            }
        }

        /// <summary>
        /// Expose command to load/save AvalonDock layout on application startup and shut-down.
        /// </summary>
        public IAvalonDockLayoutViewModel ADLayout { get; }

        public bool ShutDownInProgress_Cancel
        {
            get => mShutDownInProgress_Cancel;

            set
            {
                if (mShutDownInProgress_Cancel != value)
                    mShutDownInProgress_Cancel = value;
            }
        }

        #region ApplicationName
        /// <summary>
        /// Get the name of this application in a human read-able fashion
        /// </summary>
        public string ApplicationTitle => mAppCore.AssemblyTitle;

        #endregion ApplicationName

        /// <summary>
        /// Convienance property to filter (cast) documents that represent
        /// actual text documents out of the general documents collection.
        /// 
        /// Items such as start page or program settings are not considered
        /// documents in this collection.
        /// </summary>
        private List<EdiViewModel> Documents => mFiles.OfType<EdiViewModel>().ToList();

        #endregion Properties

        #region methods
        /// <summary>
        /// Wrapper method for file open
        /// - is executed when a file open is requested from external party such as tool window.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool FileOpen(string file)
        {
            Open(file);
            return true;
        }

        /// <summary>
        /// Open a file supplied in <paramref name="filePath"/> (without displaying a file open dialog).
        /// </summary>
        /// <param name="filePath">file to open</param>
        /// <param name="closeDocumentWithoutMessageOnError"></param>
        /// <param name="AddIntoMRU">indicate whether file is to be added into MRU or not</param>
        /// <param name="typeOfDoc"></param>
        /// <returns></returns>
        public IFileBaseViewModel Open(string filePath,
                                        CloseDocOnError closeDocumentWithoutMessageOnError = CloseDocOnError.WithUserNotification,
                                        bool AddIntoMRU = true,
                                        string typeOfDoc = "EdiTextEditor")
        {
            logger.InfoFormat("TRACE EdiViewModel.Open param: '{0}', AddIntoMRU {1}", filePath, AddIntoMRU);

            SelectedOpenDocumentType = DocumentTypes[0];

            // Verify whether file is already open in editor, and if so, show it
            IFileBaseViewModel fileViewModel = Documents.FirstOrDefault(fm => fm.FilePath == filePath);

            if (fileViewModel != null) // File is already open so show it to the user
            {
                ActiveDocument = fileViewModel;
                return fileViewModel;
            }

            IDocumentModel dm = new DocumentModel();
            dm.SetFileNamePath(filePath, true);

            // 1st try to find a document type handler based on the supplied extension
            var docType = mDocumentTypeManager.FindDocumentTypeByExtension(dm.FileExtension, true) ??
                          mDocumentTypeManager.FindDocumentTypeByKey(typeOfDoc);

            // 2nd try to find a document type handler based on the name of the prefered viewer
            // (Defaults to EdiTextEditor if no name is given)

            if (docType != null)
            {
                fileViewModel = docType.FileOpenMethod(dm, mSettingsManager);
            }
            else
            {
                ////if ((dm.FileExtension == string.Format(".{0}", ApplicationViewModel.MiniUMLFileExtension) && typeOfDoc == "EdiTextEditor") || typeOfDoc == "UMLEditor")
                ////{
                ////	fileViewModel = MiniUmlViewModel.LoadFile(filePath);
                ////}
                ////else
                ////{
                bool closeOnErrorWithoutMessage = closeDocumentWithoutMessageOnError == CloseDocOnError.WithoutUserNotification;

                // try to load a standard text file from the file system as a fallback method
                fileViewModel = EdiViewModel.LoadFile(dm, mSettingsManager, closeOnErrorWithoutMessage);
                ////}
            }

            return IntegrateDocumentVM(fileViewModel, filePath, AddIntoMRU);
        }

        private IFileBaseViewModel IntegrateDocumentVM(IFileBaseViewModel fileViewModel,
                                                        string filePath,
                                                        bool AddIntoMRU)
        {
            if (fileViewModel == null)
            {
                var mruList = ServiceLocator.Current.GetInstance<IMRUListViewModel>();

                if (mruList.FindEntry(filePath) != null)
                {
                    if (_MsgBox.Show(string.Format(Util.Local.Strings.STR_ERROR_LOADING_FILE_MSG, filePath),
                                                   Util.Local.Strings.STR_ERROR_LOADING_FILE_CAPTION, MsgBoxButtons.YesNo) == MsgBoxResult.Yes)
                    {
                        mruList.RemoveEntry(filePath);
                    }
                }

                return null;
            }

            fileViewModel.DocumentEvent += ProcessDocumentEvent;
            fileViewModel.ProcessingResultEvent += Vm_ProcessingResultEvent;
            mFiles.Add(fileViewModel);

            // reset viewmodel options in accordance to current program settings

            if (fileViewModel is IDocumentEdi ediVm)
            {
                SetActiveDocumentOnNewFileOrOpenFile(ediVm);
            }
            else
            {
                SetActiveFileBaseDocument(fileViewModel);
            }

            if (AddIntoMRU)
                GetToolWindowVM<RecentFilesViewModel>().AddNewEntryIntoMRU(filePath);

            return fileViewModel;
        }

        /// <summary>
        /// <seealso cref="IViewModelResolver"/> method for resolving
        /// AvalonDock contentid's against a specific viewmodel.
        /// </summary>
        /// <param name="content_id"></param>
        /// <returns></returns>
        public object ContentViewModelFromID(string content_id)
        {
            // Query for a tool window and return it
            var anchorable_vm = Tools.FirstOrDefault(d => d.ContentId == content_id);


            if (anchorable_vm is IRegisterableToolWindow)
            {
                IRegisterableToolWindow registerTW = anchorable_vm as IRegisterableToolWindow;

                registerTW.SetDocumentParent(this);
            }

            if (anchorable_vm != null)
                return anchorable_vm;

            // Query for a matching document and return it
            if (mSettingsManager.SettingData.ReloadOpenFilesOnAppStart)
                return ReloadDocument(content_id);

            return null;
        }

        #region NewCommand
        private void OnNew(TypeOfDocument t = TypeOfDocument.EdiTextEditor)
        {
            try
            {
                var typeOfDocKey = mDocumentTypeManager.FindDocumentTypeByKey(t.ToString());
                if (typeOfDocKey != null)
                {
                    var dm = new DocumentModel();

                    // Does this document type support creation of new documents?
                    if (typeOfDocKey.CreateDocumentMethod != null)
                    {
                        IFileBaseViewModel vm = typeOfDocKey.CreateDocumentMethod(dm);

                        if (vm is IDocumentEdi)              // Process Edi ViewModel specific items
                        {
                            var ediVM = vm as IDocumentEdi;

                            ediVM.InitInstance(mSettingsManager.SettingData);

                            ediVM.IncreaseNewCounter();
                            ediVM.DocumentEvent += ProcessDocumentEvent;

                            ediVM.ProcessingResultEvent += Vm_ProcessingResultEvent;
                            ediVM.CreateNewDocument();

                            mFiles.Add(ediVM);
                            SetActiveDocumentOnNewFileOrOpenFile(ediVM);
                        }
                        else
                            throw new NotSupportedException($"Creating Documents of type: '{t.ToString()}'");
                    }
                    else
                    {
                        // Modul registration with PRISM is missing here
                        if (t == TypeOfDocument.UMLEditor)
                        {
                            var umlVM = new MiniUmlViewModel(dm);

                            umlVM.DocumentEvent += ProcessDocumentEvent;
                            mFiles.Add(umlVM);
                            SetActiveFileBaseDocument(umlVM);
                        }
                        else
                            throw new NotSupportedException($"Creating Documents of type: '{t.ToString()}'");
                    }
                }
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             mAppCore.IssueTrackerLink,
                             mAppCore.IssueTrackerLink,
                             Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
            }
        }
        #endregion NewCommand

        #region OpenCommand
        /// <summary>
        /// Open a type of document from file persistence with dialog
        /// and user interaction.
        /// </summary>
        /// <param name="typeOfDocument"></param>
        private void OnOpen(string typeOfDocument = "")
        {
            try
            {
                var dlg = new OpenFileDialog();
                IFileFilterEntries fileEntries = null;

                // Get filter strings for document specific filters or all filters
                // depending on whether type of document is set to a key or not.
                fileEntries = mDocumentTypeManager.GetFileFilterEntries(typeOfDocument);
                dlg.Filter = fileEntries.GetFilterString();

                dlg.Multiselect = true;
                dlg.InitialDirectory = GetDefaultPath();

                if (dlg.ShowDialog().GetValueOrDefault())
                {
                    // Smallest value in filterindex is 1
                    FileOpenDelegate fo = fileEntries.GetFileOpenMethod(dlg.FilterIndex - 1);

                    foreach (string fileName in dlg.FileNames)
                    {
                        var dm = new DocumentModel();
                        dm.SetFileNamePath(fileName, true);

                        // Execute file open method from delegate and integrate new viewmodel instance
                        var vm = fo(dm, mSettingsManager);

                        IntegrateDocumentVM(vm, fileName, true);
                    }

                    // Pre-select this document type in collection of document types that can be opened and viewed
                    var typeOfDocKey = mDocumentTypeManager.FindDocumentTypeByKey(typeOfDocument);
                    if (typeOfDocKey != null)
                        SelectedOpenDocumentType = typeOfDocKey;
                }
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             mAppCore.IssueTrackerLink,
                             mAppCore.IssueTrackerLink,
                             Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
            }
        }
        #endregion OnOpen

        #region Application_Exit_Command
        private void AppExit_CommandExecuted()
        {
            try
            {
                if (Closing_CanExecute())
                {
                    mShutDownInProgress_Cancel = false;
                    OnRequestClose();
                }
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             mAppCore.IssueTrackerLink,
                             mAppCore.IssueTrackerLink,
                             Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
            }
        }
        #endregion Application_Exit_Command

        private void AppProgramSettings_CommandExecuted()
        {
            try
            {
                // Initialize view model for editing settings
                ConfigViewModel dlgVM = new ConfigViewModel();
                dlgVM.LoadOptionsFromModel(mSettingsManager.SettingData);

                // Create dialog and attach viewmodel to view datacontext
                Window dlg = ViewSelector.GetDialogView(dlgVM, Application.Current.MainWindow);

                dlg.ShowDialog();

                if (dlgVM.WindowCloseResult == true)
                {
                    dlgVM.SaveOptionsToModel(mSettingsManager.SettingData);

                    if (mSettingsManager.SettingData.IsDirty)
                        mSettingsManager.SaveOptions(mAppCore.DirFileAppSettingsData, mSettingsManager.SettingData);
                }
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             mAppCore.IssueTrackerLink,
                             mAppCore.IssueTrackerLink,
                             Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
            }
        }

        #region Application_About_Command
        private void AppAbout_CommandExecuted()
        {
            try
            {
                var vm = new AboutViewModel();
                Window dlg = ViewSelector.GetDialogView(vm, Application.Current.MainWindow);

                dlg.ShowDialog();
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             mAppCore.IssueTrackerLink,
                             mAppCore.IssueTrackerLink,
                             Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
            }
        }
        #endregion Application_About_Command

        #region Recent File List Pin Unpin Commands
        private void PinCommand_Executed(object o, ExecutedRoutedEventArgs e)
        {
            try
            {
                var cmdParam = o as IMRUEntryViewModel;

                if (cmdParam == null)
                    return;

                if (e != null)
                    e.Handled = true;

                bool isPinnedParam = cmdParam.IsPinned == 0;    // Pin this if it was not pinned before or

                GetToolWindowVM<RecentFilesViewModel>().MruList.PinUnpinEntry(isPinnedParam, cmdParam);
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             mAppCore.IssueTrackerLink,
                             mAppCore.IssueTrackerLink,
                             Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
            }
        }

        private void AddMRUEntry_Executed(object o, ExecutedRoutedEventArgs e)
        {
            try
            {
                var cmdParam = o as IMRUEntryViewModel;

                if (cmdParam == null)
                    return;

                if (e != null)
                    e.Handled = true;

                GetToolWindowVM<RecentFilesViewModel>().MruList.UpdateEntry(cmdParam);
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             mAppCore.IssueTrackerLink,
                             mAppCore.IssueTrackerLink,
                             Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
            }
        }

        private void RemoveMRUEntry_Executed(object o, ExecutedRoutedEventArgs e)
        {
            try
            {
                var cmdParam = o as IMRUEntryViewModel;

                if (cmdParam == null)
                    return;

                if (e != null)
                    e.Handled = true;

                GetToolWindowVM<RecentFilesViewModel>().MruList.RemoveEntry(cmdParam);
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             mAppCore.IssueTrackerLink,
                             mAppCore.IssueTrackerLink,
                             Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
            }
        }
        #endregion Recent File List Pin Unpin Commands

        #region RequestClose [event]
        /// <summary>
        /// Method to be executed when user (or program) tries to close the application
        /// </summary>
        public void OnRequestClose()
        {
            try
            {
                if (mShutDownInProgress == false)
                {
                    if (DialogCloseResult == null)
                        DialogCloseResult = true;      // Execute Close event via attached property

                    if (mShutDownInProgress_Cancel)
                    {
                        mShutDownInProgress = false;
                        mShutDownInProgress_Cancel = false;
                        DialogCloseResult = null;
                    }
                    else
                    {
                        mShutDownInProgress = true;

                        CommandManager.InvalidateRequerySuggested();

                        RequestClose?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            catch (Exception exp)
            {
                mShutDownInProgress = false;

                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             mAppCore.IssueTrackerLink,
                             mAppCore.IssueTrackerLink,
                             Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
            }
        }
        #endregion // RequestClose [event]

        private void SetActiveFileBaseDocument(IFileBaseViewModel vm)
        {
            try
            {
                ActiveDocument = vm;
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             mAppCore.IssueTrackerLink,
                             mAppCore.IssueTrackerLink,
                             Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
            }
        }

        /// <summary>
        /// Reset file view options in accordance with current program settings
        /// whenever a new file is internally created (on File Open or New File)
        /// </summary>
        /// <param name="vm"></param>
        private void SetActiveDocumentOnNewFileOrOpenFile(IDocumentEdi vm)
        {
            try
            {
                // Set scale factor in default size of text font
                vm.InitScaleView(mSettingsManager.SettingData.DocumentZoomUnit,
                                                 mSettingsManager.SettingData.DocumentZoomView);

                ActiveDocument = vm;
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             mAppCore.IssueTrackerLink,
                             mAppCore.IssueTrackerLink,
                             Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
            }
        }

        /// <summary>
        /// Implement part of requirement § 3.1.0 
        /// 
        /// The Open/SaveAs file dialog opens in the location of the currently active document (if any).
        /// 
        /// Otherwise, if there is no active document or the active document has never been saved before,
        /// the location of the last file open or file save/save as (which ever was last)
        /// is displayed in the Open/SaveAs File dialog.
        /// 
        /// The Open/SaveAs file dialog opens in the MyDocuments Windows user folder
        /// if none of the above conditions are true. (eg.: Open file for the very first
        /// time or last location does not exist).
        /// 
        /// The Open/Save/SaveAs file dialog opens in "C:\" if none of the above requirements
        /// can be implemented (eg.: MyDocuments folder does not exist or user has no access).
        /// 
        /// The last Open/Save/SaveAs file location used is stored and recovered between user sessions.
        /// </summary>
        /// <returns></returns>
        private string GetDefaultPath()
        {
            string sPath = string.Empty;

            try
            {
                // Generate a default path from cuurently or last active document
                if (ActiveEdiDocument != null)
                    sPath = ActiveEdiDocument.GetFilePath();

                if (sPath == string.Empty)
                    sPath = mSettingsManager.SessionData.GetLastActivePath();

                if (sPath == string.Empty)
                    sPath = mAppCore.MyDocumentsUserDir;
                else
                {
                    try
                    {
                        if (Directory.Exists(sPath) == false)
                            sPath = mAppCore.MyDocumentsUserDir;
                    }
                    catch
                    {
                        sPath = mAppCore.MyDocumentsUserDir;
                    }
                }
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             mAppCore.IssueTrackerLink,
                             mAppCore.IssueTrackerLink,
                             Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
            }

            return sPath;
        }

        /// <summary>
        /// Attempt to save data in file when
        /// File>Save As... or File>Save command
        /// is executed.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="saveAsFlag"></param>
        /// <returns></returns>
        internal bool OnSave(IFileBaseViewModel doc, bool saveAsFlag = false)
        {
            if (doc == null)
                return false;

            if (doc.CanSaveData)
            {
                var defaultFilter = GetDefaultFileFilter(doc, mDocumentTypeManager);

                return OnSaveDocumentFile(doc, saveAsFlag, defaultFilter);
            }

            throw new NotSupportedException((doc != null ? doc.ToString() : Util.Local.Strings.STR_MSG_UnknownDocumentType));
        }

        /// <summary>
        /// Returns the default file extension filter strings
        /// that can be used for each corresponding document
        /// type (viewmodel), or an empty string if no document
        /// type (viewmodel) was matched.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        internal static string GetDefaultFileFilter(IFileBaseViewModel f, IDocumentTypeManager docManager)
        {
            if (f == null)
                return string.Empty;

            var filefilter = docManager.GetFileFilterEntries(f.DocumentTypeKey);

            if (filefilter != null)
                return filefilter.GetFilterString();

            return string.Empty;
        }

        internal bool OnSaveDocumentFile(IFileBaseViewModel fileToSave,
                                                                         bool saveAsFlag = false,
                                                                         string FileExtensionFilter = "")
        {
            string filePath = (fileToSave == null ? string.Empty : fileToSave.FilePath);

            // Offer SaveAs file dialog if file has never been saved before (was created with new command)
            if (fileToSave != null)
                saveAsFlag = saveAsFlag | !fileToSave.IsFilePathReal;

            try
            {
                if (filePath == string.Empty || saveAsFlag)   // Execute SaveAs function
                {
                    var dlg = new SaveFileDialog();

                    try
                    {
                        dlg.FileName = Path.GetFileName(filePath);
                    }
                    catch
                    {
                    }

                    dlg.InitialDirectory = GetDefaultPath();

                    if (string.IsNullOrEmpty(FileExtensionFilter) == false)
                        dlg.Filter = FileExtensionFilter;

                    if (dlg.ShowDialog().GetValueOrDefault())     // SaveAs file if user OK'ed it so
                    {
                        filePath = dlg.FileName;

                        fileToSave.SaveFile(filePath);
                    }
                    else
                        return false;
                }
                else                                                  // Execute Save function
                    fileToSave.SaveFile(filePath);

                GetToolWindowVM<RecentFilesViewModel>().AddNewEntryIntoMRU(filePath);

                return true;
            }
            catch (Exception Exp)
            {
                string sMsg = Util.Local.Strings.STR_MSG_ErrorSavingFile;

                if (filePath.Length > 0)
                    sMsg = string.Format(CultureInfo.CurrentCulture, Util.Local.Strings.STR_MSG_ErrorWhileSavingFileX, Exp.Message, filePath);
                else
                    sMsg = string.Format(CultureInfo.CurrentCulture, Util.Local.Strings.STR_MSG_ErrorWhileSavingAFile, Exp.Message);

                _MsgBox.Show(Exp, sMsg, Util.Local.Strings.STR_MSG_ErrorSavingFile, MsgBoxButtons.OK);
            }

            return false;
        }

        internal bool OnCloseSaveDirtyFile(IFileBaseViewModel fileToClose)
        {
            if (fileToClose.IsDirty &&
                    fileToClose.CanSaveData)
            {
                var res = _MsgBox.Show(string.Format(CultureInfo.CurrentCulture, Util.Local.Strings.STR_MSG_SaveChangesForFile, fileToClose.FileName),
                                       ApplicationTitle,
                                       MsgBoxButtons.YesNoCancel, MsgBoxImage.Question,
                                       MsgBoxResult.Yes, false,
                                       MsgBoxResult.Yes);

                if (res == MsgBoxResult.Cancel)
                    return false;

                if (res == MsgBoxResult.Yes)
                {
                    return OnSave(fileToClose);
                }
            }

            return true;
        }

        /// <summary>
        /// Close the currently active file and set the file with the lowest index as active document.
        /// 
        /// TODO: The last active document that was active before the document being closed should be activated next.
        /// </summary>
        /// <param name="fileToClose"></param>
        /// <returns></returns>
        internal bool Close(IFileBaseViewModel doc)
        {
            try
            {
                {
                    if (OnCloseSaveDirtyFile(doc) == false)
                        return false;

                    doc.DocumentEvent -= ProcessDocumentEvent;
                    doc.ProcessingResultEvent -= Vm_ProcessingResultEvent;

                    if (doc is IDocumentEdi)
                    {
                        var ediDoc = doc as IDocumentEdi;

                        ediDoc.ProcessingResultEvent -= Vm_ProcessingResultEvent;
                    }

                    int idx = mFiles.IndexOf(doc);

                    mFiles.Remove(doc);
                    doc.Dispose();

                    if (Documents.Count > idx)
                        ActiveDocument = mFiles[idx];
                    else
                        if (Documents.Count > 1 && Documents.Count == idx)
                        ActiveDocument = mFiles[idx - 1];
                    else
                            if (Documents.Count == 0)
                        ActiveDocument = null;
                    else
                        ActiveDocument = mFiles[0];

                    return true;
                }

                /*
					// This could be a StartPage, Log4Net, or UML file or any other (read-only) document type
					if (doc != null)
					{
						if (doc.IsDirty == true)
						{
							if (this.OnCloseSaveDirtyFile(doc) == false)
								return false;
						}

						mFiles.Remove(doc);

						if (this.Documents.Count == 0)
							this.ActiveDocument = null;
						else
							this.ActiveDocument = this.mFiles[0];

						return true;
					}
				*/
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             mAppCore.IssueTrackerLink,
                             mAppCore.IssueTrackerLink,
                             Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
            }

            // Throw an exception if this method does not know how the input document type is to be closed
            throw new NotSupportedException(doc.ToString());
        }

        /// <summary>
        /// This can be used to close the attached view via ViewModel
        /// 
        /// Source: http://stackoverflow.com/questions/501886/wpf-mvvm-newbie-how-should-the-viewmodel-close-the-form
        /// </summary>
        public bool? DialogCloseResult
        {
            get => mDialogCloseResult;

            private set
            {
                if (mDialogCloseResult != value)
                {
                    mDialogCloseResult = value;
                    RaisePropertyChanged(() => DialogCloseResult);
                }
            }
        }

        /// <summary>
        /// Get/set property to determine whether window is in maximized state or not.
        /// (this can be handy to determine when a resize grip should be shown or not)
        /// </summary>
        public bool? IsNotMaximized
        {
            get => mIsNotMaximized;

            set
            {
                if (mIsNotMaximized != value)
                {
                    mIsNotMaximized = value;
                    RaisePropertyChanged(() => IsNotMaximized);
                }
            }
        }

        /// <summary>
        /// Gets/sets whether the workspace area is optimized or not.
        /// The optimized workspace is distructive free and does not
        /// show optional stuff like toolbar and status bar.
        /// </summary>
        public bool IsWorkspaceAreaOptimized
        {
            get => mIsWorkspaceAreaOptimized;

            set
            {
                if (mIsWorkspaceAreaOptimized != value)
                {
                    mIsWorkspaceAreaOptimized = value;
                    RaisePropertyChanged(() => IsWorkspaceAreaOptimized);
                }
            }
        }

        /// <summary>
        /// Check if pre-requisites for closing application are available.
        /// Save session data on closing and cancel closing process if necessary.
        /// </summary>
        /// <returns>true if application is OK to proceed closing with closed, otherwise false.</returns>
        public bool Exit_CheckConditions(object sender)
        {
            if (mShutDownInProgress)
                return true;

            try
            {
                if (mFiles != null)               // Close all open files and make sure there are no unsaved edits
                {                                     // If there are any: Ask user if edits should be saved
                    for (int i = 0; i < Files.Count; i++)
                    {
                        IFileBaseViewModel f = Files[i];

                        if (OnCloseSaveDirtyFile(f) == false)
                        {
                            mShutDownInProgress = false;
                            return false;               // Cancel shutdown process (return false) if user cancels saving edits
                        }
                    }
                }

                // Do layout serialization after saving/closing files
                // since changes implemented by shut-down process are otherwise lost
                try
                {
                    mAppCore.CreateAppDataFolder();
                    ////this.SerializeLayout(sender);            // Store the current layout for later retrieval
                }
                catch
                {
                }
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             mAppCore.IssueTrackerLink,
                             mAppCore.IssueTrackerLink,
                             Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
            }

            return true;
        }

        /// <summary>
        /// Set the active document to the file in <seealso cref="fileNamePath"/>
        /// if this is currently open.
        /// </summary>
        /// <param name="fileNamePath"></param>
        internal bool SetActiveDocument(string fileNamePath)
        {
            try
            {
                if (Files.Count >= 0)
                {
                    EdiViewModel fi = Documents.SingleOrDefault(f => f.FilePath == fileNamePath);

                    if (fi != null)
                    {
                        ActiveDocument = fi;
                        return true;
                    }
                }
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             mAppCore.IssueTrackerLink,
                             mAppCore.IssueTrackerLink,
                             Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
            }

            return false;
        }

        /// <summary>
        /// Construct and add a new <seealso cref="StartPageViewModel"/> to intenral
        /// list of documents, if none is already present, otherwise return already
        /// present <seealso cref="StartPageViewModel"/> from internal document collection.
        /// </summary>
        /// <param name="CreateNewViewModelIfNecessary"></param>
        /// <returns></returns>
        internal StartPageViewModel GetStartPage(bool CreateNewViewModelIfNecessary)
        {
            List<StartPageViewModel> l = mFiles.OfType<StartPageViewModel>().ToList();

            if (l.Count == 0)
            {
                if (CreateNewViewModelIfNecessary == false)
                    return null;
                else
                {
                    var s = new StartPageViewModel();

                    s.DocumentEvent += ProcessDocumentEvent;

                    mFiles.Add(s);

                    return s;
                }
            }

            return l[0];
        }

        /// <summary>
        /// Close document via dedicated event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessDocumentEvent(object sender, FileBaseEvent e)
        {
            var f = sender as FileBaseViewModel;

            switch (e.TypeOfEvent)
            {
                case FileEventType.Unknown:
                    break;

                case FileEventType.CloseDocument:
                    if (f != null)
                        CloseDocument(f);
                    break;

                case FileEventType.AdjustCurrentPath:
                    if (f != null)
                    {
                        // Query for an explorer tool window and return it
                        var eplorerTW = GetToolWindowVM<IExplorer>();

                        if (eplorerTW != null)
                            eplorerTW.NavigateToFolder(f.GetAlternativePath());
                    }
                    break;

                default:
                    break;
            }
        }

        private void CloseDocument(FileBaseViewModel f)
        {
            if (f != null)
            {
                // Detach EdiViewModel specific events
                if (f is EdiViewModel)
                {
                    EdiViewModel eVM = f as EdiViewModel;
                    eVM.ProcessingResultEvent -= Vm_ProcessingResultEvent;
                }

                Close(f);
            }
        }

        /// <summary>
        /// Handle Processing results from asynchronous tasks that are
        /// executed in a viewmodel and return later with a Result (eg.: Async load of document)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Vm_ProcessingResultEvent(object sender, ProcessResultEvent e)
        {

            if (sender is IDocumentFileWatcher)
            {
                IDocumentFileWatcher watcher = sender as IDocumentFileWatcher;

                try
                {
                    // Activate file watcher for this document
                    watcher.EnableDocumentFileWatcher(true);
                }
                catch (Exception ex)
                {
                    _MsgBox.Show(ex, "An unexpected error occured", MsgBoxButtons.OK, MsgBoxImage.Alert);
                }

                var vm = sender as EdiViewModel;

                try
                {
                    switch (e.TypeOfResult)
                    {
                        case TypeOfResult.FileLoad:      // Process an EdiViewModel file load event mResult
                            if (e.InnerException != null)
                            {
                                Exception error = vm.GetInnerMostException(e.InnerException);

                                string filePath = vm.FilePath;
                                CloseDocument(vm);
                                vm = null;

                                if (error != null && filePath != null)
                                {
                                    if (error is FileNotFoundException)
                                    {
                                        var mruList = ServiceLocator.Current.GetInstance<IMRUListViewModel>();
                                        if (mruList.FindEntry(filePath) != null)
                                        {
                                            if (_MsgBox.Show(string.Format(Util.Local.Strings.STR_ERROR_LOADING_FILE_MSG, filePath),
                                                             Util.Local.Strings.STR_ERROR_LOADING_FILE_CAPTION, MsgBoxButtons.YesNo) == MsgBoxResult.Yes)
                                            {
                                                mruList.RemoveEntry(filePath);
                                            }
                                        }

                                        return;
                                    }
                                }

                                _MsgBox.Show(e.InnerException, "An unexpected error occured",
                                             MsgBoxButtons.OK, MsgBoxImage.Alert);
                            }
                            break;

                        default:
                            throw new NotImplementedException(e.TypeOfResult.ToString());
                    }
                }
                catch (Exception exp)
                {
                    logger.Error(exp);

                    _MsgBox.Show(exp, "An unexpected error occured", MsgBoxButtons.OK, MsgBoxImage.Alert);
                }
                finally
                {

                }
            }
        }

        /// <summary>
        /// Helper method for viewmodel resolution for avalondock contentids
        /// and specific document viewmodels. Careful: the Start Page is also
        /// a document but cannot be loaded, saved, or edit as other documents can.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private IFileBaseViewModel ReloadDocument(string path)
        {
            IFileBaseViewModel ret = null;

            if (!string.IsNullOrWhiteSpace(path))
            {
                switch (path)
                {
                    case StartPageViewModel.StartPageContentId: // Re-create start page content
                        if (GetStartPage(false) == null)
                        {
                            ret = GetStartPage(true);
                        }
                        break;

                    default:
                        if (path.Contains("<") && path.Contains(">"))
                        {
                            mMessageManager.Output.AppendLine(
                                $"Warning: Cannot resolve tool window or document page: '{path}'.");

                            mMessageManager.Output.AppendLine(
                                string.Format("Check the current program configuration to make that it is present.", path));

                            return null;
                        }

                        // Re-create Edi document (text file or log4net document) content
                        ret = Open(path, CloseDocOnError.WithoutUserNotification);
                        break;
                }
            }

            return ret;
        }

        /// <summary>
        /// Return a typed viewmodel from a collection of tool window viewmodels.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T GetToolWindowVM<T>() where T : class
        {
            // Query for a RecentFiles tool window and return it
            return Tools.FirstOrDefault(d => d as T != null) as T;
        }

        /// <summary>
        /// Method executes when tool window registration publishers
        /// relay the fact that a new tool window has been registered
        /// via PRISM event aggregator notification.
        /// </summary>
        /// <param name="args"></param>
        private void OnRegisterToolWindow(RegisterToolWindowEventArgs args)
        {
            if (args != null)
            {
                // This particular event is needed since the build in RecentFiles
                // property is otherwise without content since it may be queried
                // for the menu entry - before the tool window is registered
                if (args.Tool is RecentFilesViewModel)
                    RaisePropertyChanged(() => RecentFiles);
            }
        }

        /// <summary>
        /// Is invoked when PRISM registers a module.
        /// The output should be visible in the output tool window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModuleManager_LoadModuleCompleted(object sender, LoadModuleCompletedEventArgs e)
        {
            if (mMessageManager.Output != null)
            {
                mMessageManager.Output.AppendLine(
                    $"Loading MEF Module: {e.ModuleInfo.ModuleName},\n" +
                    $"                    Type: {e.ModuleInfo.ModuleType},\n" +
                    $"     Initialization Mode: {e.ModuleInfo.InitializationMode},\n" +
                    $"                   State: {e.ModuleInfo.State}, Ref: '{e.ModuleInfo.Ref}'\n");
            }
        }
        #endregion methods
    }
}
