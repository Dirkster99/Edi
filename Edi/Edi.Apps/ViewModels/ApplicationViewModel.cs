namespace Edi.Apps.ViewModels
{
    using Edi.Apps.Enums;
    using Edi.Apps.Interfaces.ViewModel;
    using Edi.Core.Interfaces;
    using Edi.Core.Interfaces.Documents;
    using Edi.Core.Interfaces.DocumentTypes;
    using Edi.Core.Models.Documents;
    using Edi.Core.ViewModels;
    using Edi.Core.ViewModels.Base;
    using Edi.Core.ViewModels.Command;
    using Edi.Core.ViewModels.Events;
    using Edi.Dialogs.About;
    using Edi.Documents.ViewModels.EdiDoc;
    using Edi.Documents.ViewModels.MiniUml;
    using Edi.Documents.ViewModels.StartPage;
    using Edi.Settings.Interfaces;
    using Edi.SettingsView.Config.ViewModels;
    using Edi.Themes.Interfaces;
    using EdiApp.Events;   // XXX TODO Implementation in Edi.Core shouold have a different namespace
    using Files.ViewModels.RecentFiles;
    using Microsoft.Practices.Prism.Modularity;
    using Microsoft.Practices.Prism.PubSubEvents;
    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Win32;
    using MsgBox;
    using SimpleControls.MRU.ViewModel;
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
        public static readonly string Log4netFileFilter = Edi.Util.Local.Strings.STR_FileType_FileFilter_Log4j;

        public const string MiniUMLFileExtension = "uml";
        public static readonly string UMLFileFilter = Edi.Util.Local.Strings.STR_FileType_FileFilter_UML;

        private static string EdiTextEditorFileFilter =
                                    Edi.Util.Local.Strings.STR_FileType_FileFilter_AllFiles +
                                    "|" + Edi.Util.Local.Strings.STR_FileType_FileFilter_TextFiles +
                                    "|" + Edi.Util.Local.Strings.STR_FileType_FileFilter_CSharp +
                                    "|" + Edi.Util.Local.Strings.STR_FileType_FileFilter_HTML +
                                    "|" + Edi.Util.Local.Strings.STR_FileType_FileFilter_SQL;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool? mDialogCloseResult = null;
        private bool? mIsNotMaximized = null;
        private bool mIsWorkspaceAreaOptimized = false;

        private bool mShutDownInProgress = false;
        private bool mShutDownInProgress_Cancel = false;

        private ObservableCollection<IFileBaseViewModel> mFiles = null;
        private ReadOnlyObservableCollection<IFileBaseViewModel> mReadonyFiles = null;

        private IFileBaseViewModel mActiveDocument = null;
        private RelayCommand<object> mMainWindowActivated = null;

        private readonly IModuleManager mModuleManager = null;
        private readonly IAppCoreModel mAppCore = null;
        private readonly IAvalonDockLayoutViewModel mAVLayout = null;
        private readonly IToolWindowRegistry mToolRegistry = null;
        private readonly ISettingsManager mSettingsManager = null;
        private readonly IThemesManager mThemesManager = null;
        private readonly IMessageManager mMessageManager = null;

        private readonly IDocumentTypeManager mDocumentTypeManager;
        private IDocumentType mSelectedOpenDocumentType = null;

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
            this.mAppCore = appCore;
            this.mAVLayout = avLayout;
            this.mModuleManager = moduleManager;

            this.mMessageManager = messageManager;
            if (messageManager.MessageBox != null)   // reset messagebox service
                _MsgBox = messageManager.MessageBox;
            else
            {
                _MsgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
            }

            this.mToolRegistry = toolRegistry;
            this.mSettingsManager = programSettings;
            this.mThemesManager = themesManager;
            this.mDocumentTypeManager = documentTypeManager;

            this.mModuleManager.LoadModuleCompleted += this.ModuleManager_LoadModuleCompleted;
        }

        public ApplicationViewModel()
        {
            this.mAVLayout = null;
            this.mFiles = new ObservableCollection<IFileBaseViewModel>();

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
        public IThemesManager ApplicationThemes
        {
            get
            {
                return this.mThemesManager;
            }
        }

        private object mLock = new object();
        private bool mIsMainWindowActivationProcessed = false;
        private bool mIsMainWindowActivationProcessingEnabled = false;

        /// <summary>
        /// Activates/deactivates processing of the mainwindow activated event.
        /// </summary>
        /// <param name="bActivate"></param>
        public void EnableMainWindowActivated(bool bActivate)
        {
            this.mIsMainWindowActivationProcessingEnabled = bActivate;
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
                if (this.mMainWindowActivated == null)
                    this.mMainWindowActivated = new RelayCommand<object>((p) =>
                    {
                        // Is processing of this event currently enabled?
                        if (this.mIsMainWindowActivationProcessingEnabled == false)
                            return;

                        // Is this event already currently being processed?
                        if (this.mIsMainWindowActivationProcessed == true)
                            return;

                        lock (this.mLock)
                        {
                            try
                            {
                                if (this.mIsMainWindowActivationProcessed == true)
                                    return;

                                this.mIsMainWindowActivationProcessed = true;

                                foreach (var item in this.Files)
                                {
                                    if (item.WasChangedExternally == true)
                                    {
                                        var result = _MsgBox.Show(string.Format("File '{0}' was changed externally. Click OK to reload or Cancel to keep current content.", item.FileName),
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
                                _MsgBox.Show(exp, Edi.Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                                             this.mAppCore.IssueTrackerLink, this.mAppCore.IssueTrackerLink,
                                             Edi.Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
                            }
                            finally
                            {
                                this.mIsMainWindowActivationProcessed = false;
                            }
                        }
                    });

                return this.mMainWindowActivated;
            }
        }

        #region ActiveDocument
        /// <summary>
        /// Gets/sets the dcoument that is currently active (has input focus) - if any.
        /// </summary>
        public IFileBaseViewModel ActiveDocument
        {
            get
            {
                return this.mActiveDocument;
            }

            set
            {
                if (this.mActiveDocument != value)
                {
                    this.mActiveDocument = value;

                    this.RaisePropertyChanged(() => this.ActiveDocument);
                    this.RaisePropertyChanged(() => this.ActiveEdiDocument);
                    this.RaisePropertyChanged(() => this.vm_DocumentViewModel);

                    // Ensure that no pending calls are in the dispatcher queue
                    // This makes sure that we are blocked until bindings are re-established
                    // (Bindings are, for example, required to scroll a selection into view for search/replace)
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (Action)delegate
                    {
                        if (ActiveDocumentChanged != null)
                        {
                            ActiveDocumentChanged(this, new DocumentChangedEventArgs(this.mActiveDocument)); //this.ActiveDocument

                            if (value != null && this.mShutDownInProgress == false)
                            {
                                if (value.IsFilePathReal == true)
                                    this.mSettingsManager.SessionData.LastActiveFile = value.FilePath;
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
        public EdiViewModel ActiveEdiDocument
        {
            get
            {
                return this.mActiveDocument as EdiViewModel;
            }
        }

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

                if (this.mActiveDocument is MiniUmlViewModel vm)
                {
                    return vm.DocumentMiniUML as MiniUML.Model.ViewModels.Document.AbstractDocumentViewModel;
                }

                return null;
            }
        }
        #endregion

        public IDocumentType SelectedOpenDocumentType
        {
            get
            {
                return this.mSelectedOpenDocumentType;
            }

            private set
            {
                if (this.mSelectedOpenDocumentType != value)
                {
                    this.mSelectedOpenDocumentType = value;
                    this.RaisePropertyChanged(() => this.SelectedOpenDocumentType);
                }
            }
        }

        public ObservableCollection<IDocumentType> DocumentTypes
        {
            get
            {
                return this.mDocumentTypeManager.DocumentTypes;
            }
        }

        /// <summary>
        /// Principable data source for collection of documents managed in the the document manager (of AvalonDock).
        /// </summary>
        public ReadOnlyObservableCollection<IFileBaseViewModel> Files
        {
            get
            {
                if (mReadonyFiles == null)
                    mReadonyFiles = new ReadOnlyObservableCollection<IFileBaseViewModel>(this.mFiles);

                return mReadonyFiles;
            }
        }

        /// <summary>
        /// Principable data source for collection of tool window viewmodels
        /// whos view templating is managed in the the document manager of AvalonDock.
        /// </summary>
        public ObservableCollection<ToolViewModel> Tools
        {
            get
            {
                return this.mToolRegistry.Tools;
            }
        }

        public RecentFilesViewModel RecentFiles
        {
            get
            {
                var ret = this.GetToolWindowVM<RecentFilesViewModel>();

                return ret;
            }
        }

        /// <summary>
        /// Expose command to load/save AvalonDock layout on application startup and shut-down.
        /// </summary>
        public IAvalonDockLayoutViewModel ADLayout
        {
            get
            {
                return this.mAVLayout;
            }
        }

        public bool ShutDownInProgress_Cancel
        {
            get
            {
                return this.mShutDownInProgress_Cancel;
            }

            set
            {
                if (this.mShutDownInProgress_Cancel != value)
                    this.mShutDownInProgress_Cancel = value;
            }
        }

        #region ApplicationName
        /// <summary>
        /// Get the name of this application in a human read-able fashion
        /// </summary>
        public string ApplicationTitle
        {
            get
            {
                return this.mAppCore.AssemblyTitle;
            }
        }
        #endregion ApplicationName

        /// <summary>
        /// Convienance property to filter (cast) documents that represent
        /// actual text documents out of the general documents collection.
        /// 
        /// Items such as start page or program settings are not considered
        /// documents in this collection.
        /// </summary>
        private List<EdiViewModel> Documents
        {
            get
            {
                return this.mFiles.OfType<EdiViewModel>().ToList();
            }
        }
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
            this.Open(file);
            return true;
        }

        /// <summary>
        /// Open a file supplied in <paramref name="filePath"/> (without displaying a file open dialog).
        /// </summary>
        /// <param name="filePath">file to open</param>
        /// <param name="AddIntoMRU">indicate whether file is to be added into MRU or not</param>
        /// <returns></returns>
        public IFileBaseViewModel Open(string filePath,
                                                    CloseDocOnError closeDocumentWithoutMessageOnError = CloseDocOnError.WithUserNotification,
                                                    bool AddIntoMRU = true,
                                                    string typeOfDoc = "EdiTextEditor")
        {
            logger.InfoFormat("TRACE EdiViewModel.Open param: '{0}', AddIntoMRU {1}", filePath, AddIntoMRU);

            this.SelectedOpenDocumentType = this.DocumentTypes[0];

            // Verify whether file is already open in editor, and if so, show it
            IFileBaseViewModel fileViewModel = this.Documents.FirstOrDefault(fm => fm.FilePath == filePath);

            if (fileViewModel != null) // File is already open so show it to the user
            {
                this.ActiveDocument = fileViewModel;
                return fileViewModel;
            }

            IDocumentModel dm = new DocumentModel();
            dm.SetFileNamePath(filePath, true);

            // 1st try to find a document type handler based on the supplied extension
            var docType = this.mDocumentTypeManager.FindDocumentTypeByExtension(dm.FileExtension, true);

            // 2nd try to find a document type handler based on the name of the prefered viewer
            // (Defaults to EdiTextEditor if no name is given)
            if (docType == null)
                docType = this.mDocumentTypeManager.FindDocumentTypeByKey(typeOfDoc);

            if (docType != null)
            {
                fileViewModel = docType.FileOpenMethod(dm, this.mSettingsManager);
            }
            else
            {
                ////if ((dm.FileExtension == string.Format(".{0}", ApplicationViewModel.MiniUMLFileExtension) && typeOfDoc == "EdiTextEditor") || typeOfDoc == "UMLEditor")
                ////{
                ////	fileViewModel = MiniUmlViewModel.LoadFile(filePath);
                ////}
                ////else
                ////{
                bool closeOnErrorWithoutMessage = false;

                if (closeDocumentWithoutMessageOnError == CloseDocOnError.WithoutUserNotification)
                    closeOnErrorWithoutMessage = true;

                // try to load a standard text file from the file system as a fallback method
                fileViewModel = EdiViewModel.LoadFile(dm, this.mSettingsManager, closeOnErrorWithoutMessage);
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

                if (this.mSettingsManager.SessionData.MruList.FindMRUEntry(filePath) != null)
                {
                    if (_MsgBox.Show(string.Format(Edi.Util.Local.Strings.STR_ERROR_LOADING_FILE_MSG, filePath),
                                                   Edi.Util.Local.Strings.STR_ERROR_LOADING_FILE_CAPTION, MsgBoxButtons.YesNo) == MsgBoxResult.Yes)
                    {
                        this.mSettingsManager.SessionData.MruList.RemoveEntry(filePath);
                    }
                }

                return null;
            }

            fileViewModel.DocumentEvent += this.ProcessDocumentEvent;
            fileViewModel.ProcessingResultEvent += this.Vm_ProcessingResultEvent;
            this.mFiles.Add(fileViewModel);

            // reset viewmodel options in accordance to current program settings

            if (fileViewModel is IDocumentEdi ediVM)
            {
                this.SetActiveDocumentOnNewFileOrOpenFile(ediVM);
            }
            else
            {
                this.SetActiveFileBaseDocument(fileViewModel);
            }

            if (AddIntoMRU == true)
                this.GetToolWindowVM<RecentFilesViewModel>().AddNewEntryIntoMRU(filePath);

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
            var anchorable_vm = this.Tools.FirstOrDefault(d => d.ContentId == content_id);


            if (anchorable_vm is IRegisterableToolWindow registerTW)
                registerTW.SetDocumentParent(this);

            if (anchorable_vm != null)
                return anchorable_vm;

            // Query for a matching document and return it
            if (this.mSettingsManager.SettingData.ReloadOpenFilesOnAppStart == true)
                return this.ReloadDocument(content_id);

            return null;
        }

        #region NewCommand
        private void OnNew(TypeOfDocument t = TypeOfDocument.EdiTextEditor)
        {
            try
            {
                var typeOfDocKey = this.mDocumentTypeManager.FindDocumentTypeByKey(t.ToString());
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

                            ediVM.InitInstance(this.mSettingsManager.SettingData);

                            ediVM.IncreaseNewCounter();
                            ediVM.DocumentEvent += this.ProcessDocumentEvent;

                            ediVM.ProcessingResultEvent += Vm_ProcessingResultEvent;
                            ediVM.CreateNewDocument();

                            this.mFiles.Add(ediVM);
                            this.SetActiveDocumentOnNewFileOrOpenFile(ediVM);
                        }
                        else
                            throw new NotSupportedException(string.Format("Creating Documents of type: '{0}'", t.ToString()));
                    }
                    else
                    {
                        // Modul registration with PRISM is missing here
                        if (t == TypeOfDocument.UMLEditor)
                        {
                            var umlVM = new MiniUmlViewModel(dm);

                            umlVM.DocumentEvent += this.ProcessDocumentEvent;
                            this.mFiles.Add(umlVM);
                            this.SetActiveFileBaseDocument(umlVM);
                        }
                        else
                            throw new NotSupportedException(string.Format("Creating Documents of type: '{0}'", t.ToString()));
                    }
                }
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Edi.Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             this.mAppCore.IssueTrackerLink,
                             this.mAppCore.IssueTrackerLink,
                             Edi.Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
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
                fileEntries = this.mDocumentTypeManager.GetFileFilterEntries(typeOfDocument);
                dlg.Filter = fileEntries.GetFilterString();

                dlg.Multiselect = true;
                dlg.InitialDirectory = this.GetDefaultPath();

                if (dlg.ShowDialog().GetValueOrDefault())
                {
                    // Smallest value in filterindex is 1
                    FileOpenDelegate fo = fileEntries.GetFileOpenMethod(dlg.FilterIndex - 1);

                    foreach (string fileName in dlg.FileNames)
                    {
                        var dm = new DocumentModel();
                        dm.SetFileNamePath(fileName, true);

                        // Execute file open method from delegate and integrate new viewmodel instance
                        var vm = fo(dm, this.mSettingsManager);

                        IntegrateDocumentVM(vm, fileName, true);
                    }

                    // Pre-select this document type in collection of document types that can be opened and viewed
                    var typeOfDocKey = this.mDocumentTypeManager.FindDocumentTypeByKey(typeOfDocument);
                    if (typeOfDocKey != null)
                        this.SelectedOpenDocumentType = typeOfDocKey;
                }
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Edi.Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             this.mAppCore.IssueTrackerLink,
                             this.mAppCore.IssueTrackerLink,
                             Edi.Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
            }
        }
        #endregion OnOpen

        #region Application_Exit_Command
        private void AppExit_CommandExecuted()
        {
            try
            {
                if (this.Closing_CanExecute() == true)
                {
                    this.mShutDownInProgress_Cancel = false;
                    this.OnRequestClose();
                }
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Edi.Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             this.mAppCore.IssueTrackerLink,
                             this.mAppCore.IssueTrackerLink,
                             Edi.Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
            }
        }
        #endregion Application_Exit_Command

        private void AppProgramSettings_CommandExecuted()
        {
            try
            {
                // Initialize view model for editing settings
                ConfigViewModel dlgVM = new ConfigViewModel();
                dlgVM.LoadOptionsFromModel(this.mSettingsManager.SettingData);

                // Create dialog and attach viewmodel to view datacontext
                Window dlg = ViewSelector.GetDialogView(dlgVM, Application.Current.MainWindow);

                dlg.ShowDialog();

                if (dlgVM.WindowCloseResult == true)
                {
                    dlgVM.SaveOptionsToModel(this.mSettingsManager.SettingData);

                    if (this.mSettingsManager.SettingData.IsDirty == true)
                        this.mSettingsManager.SaveOptions(this.mAppCore.DirFileAppSettingsData, this.mSettingsManager.SettingData);
                }
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Edi.Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             this.mAppCore.IssueTrackerLink,
                             this.mAppCore.IssueTrackerLink,
                             Edi.Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
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
                _MsgBox.Show(exp, Edi.Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             this.mAppCore.IssueTrackerLink,
                             this.mAppCore.IssueTrackerLink,
                             Edi.Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
            }
        }
        #endregion Application_About_Command

        #region Recent File List Pin Unpin Commands
        private void PinCommand_Executed(object o, ExecutedRoutedEventArgs e)
        {
            try
            {
                var cmdParam = o as MRUEntryVM;

                if (cmdParam == null)
                    return;

                if (e != null)
                    e.Handled = true;

                this.GetToolWindowVM<RecentFilesViewModel>().MruList.PinUnpinEntry(!cmdParam.IsPinned, cmdParam);
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Edi.Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             this.mAppCore.IssueTrackerLink,
                             this.mAppCore.IssueTrackerLink,
                             Edi.Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
            }
        }

        private void AddMRUEntry_Executed(object o, ExecutedRoutedEventArgs e)
        {
            try
            {
                var cmdParam = o as MRUEntryVM;

                if (cmdParam == null)
                    return;

                if (e != null)
                    e.Handled = true;

                this.GetToolWindowVM<RecentFilesViewModel>().MruList.AddMRUEntry(cmdParam);
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Edi.Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             this.mAppCore.IssueTrackerLink,
                             this.mAppCore.IssueTrackerLink,
                             Edi.Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
            }
        }

        private void RemoveMRUEntry_Executed(object o, ExecutedRoutedEventArgs e)
        {
            try
            {
                var cmdParam = o as MRUEntryVM;

                if (cmdParam == null)
                    return;

                if (e != null)
                    e.Handled = true;

                this.GetToolWindowVM<RecentFilesViewModel>().MruList.RemovePinEntry(cmdParam);
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Edi.Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             this.mAppCore.IssueTrackerLink,
                             this.mAppCore.IssueTrackerLink,
                             Edi.Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
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
                if (this.mShutDownInProgress == false)
                {
                    if (this.DialogCloseResult == null)
                        this.DialogCloseResult = true;      // Execute Close event via attached property

                    if (this.mShutDownInProgress_Cancel == true)
                    {
                        this.mShutDownInProgress = false;
                        this.mShutDownInProgress_Cancel = false;
                        this.DialogCloseResult = null;
                    }
                    else
                    {
                        this.mShutDownInProgress = true;

                        CommandManager.InvalidateRequerySuggested();

                        this.RequestClose?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            catch (Exception exp)
            {
                this.mShutDownInProgress = false;

                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Edi.Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             this.mAppCore.IssueTrackerLink,
                             this.mAppCore.IssueTrackerLink,
                             Edi.Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
            }
        }
        #endregion // RequestClose [event]

        private void SetActiveFileBaseDocument(IFileBaseViewModel vm)
        {
            try
            {
                this.ActiveDocument = vm;
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Edi.Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             this.mAppCore.IssueTrackerLink,
                             this.mAppCore.IssueTrackerLink,
                             Edi.Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
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
                vm.InitScaleView(this.mSettingsManager.SettingData.DocumentZoomUnit,
                                                 this.mSettingsManager.SettingData.DocumentZoomView);

                this.ActiveDocument = vm;
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Edi.Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             this.mAppCore.IssueTrackerLink,
                             this.mAppCore.IssueTrackerLink,
                             Edi.Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
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
                if (this.ActiveEdiDocument != null)
                    sPath = this.ActiveEdiDocument.GetFilePath();

                if (sPath == string.Empty)
                    sPath = this.mSettingsManager.SessionData.GetLastActivePath();

                if (sPath == string.Empty)
                    sPath = this.mAppCore.MyDocumentsUserDir;
                else
                {
                    try
                    {
                        if (System.IO.Directory.Exists(sPath) == false)
                            sPath = this.mAppCore.MyDocumentsUserDir;
                    }
                    catch
                    {
                        sPath = this.mAppCore.MyDocumentsUserDir;
                    }
                }
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Edi.Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             this.mAppCore.IssueTrackerLink,
                             this.mAppCore.IssueTrackerLink,
                             Edi.Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
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

            if (doc.CanSaveData == true)
            {
                var defaultFilter = ApplicationViewModel.GetDefaultFileFilter(doc, this.mDocumentTypeManager);

                return this.OnSaveDocumentFile(doc, saveAsFlag, defaultFilter);
            }

            throw new NotSupportedException((doc != null ? doc.ToString() : Edi.Util.Local.Strings.STR_MSG_UnknownDocumentType));
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
                if (filePath == string.Empty || saveAsFlag == true)   // Execute SaveAs function
                {
                    var dlg = new SaveFileDialog();

                    try
                    {
                        dlg.FileName = System.IO.Path.GetFileName(filePath);
                    }
                    catch
                    {
                    }

                    dlg.InitialDirectory = this.GetDefaultPath();

                    if (string.IsNullOrEmpty(FileExtensionFilter) == false)
                        dlg.Filter = FileExtensionFilter;

                    if (dlg.ShowDialog().GetValueOrDefault() == true)     // SaveAs file if user OK'ed it so
                    {
                        filePath = dlg.FileName;

                        fileToSave.SaveFile(filePath);
                    }
                    else
                        return false;
                }
                else                                                  // Execute Save function
                    fileToSave.SaveFile(filePath);

                this.GetToolWindowVM<RecentFilesViewModel>().AddNewEntryIntoMRU(filePath);

                return true;
            }
            catch (Exception Exp)
            {
                string sMsg = Edi.Util.Local.Strings.STR_MSG_ErrorSavingFile;

                if (filePath.Length > 0)
                    sMsg = string.Format(CultureInfo.CurrentCulture, Edi.Util.Local.Strings.STR_MSG_ErrorWhileSavingFileX, Exp.Message, filePath);
                else
                    sMsg = string.Format(CultureInfo.CurrentCulture, Edi.Util.Local.Strings.STR_MSG_ErrorWhileSavingAFile, Exp.Message);

                _MsgBox.Show(Exp, sMsg, Edi.Util.Local.Strings.STR_MSG_ErrorSavingFile, MsgBoxButtons.OK);
            }

            return false;
        }

        internal bool OnCloseSaveDirtyFile(IFileBaseViewModel fileToClose)
        {
            if (fileToClose.IsDirty == true &&
                    fileToClose.CanSaveData == true)
            {
                var res = _MsgBox.Show(string.Format(CultureInfo.CurrentCulture, Edi.Util.Local.Strings.STR_MSG_SaveChangesForFile, fileToClose.FileName),
                                       this.ApplicationTitle,
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
                    if (this.OnCloseSaveDirtyFile(doc) == false)
                        return false;

                    doc.DocumentEvent -= this.ProcessDocumentEvent;
                    doc.ProcessingResultEvent -= this.Vm_ProcessingResultEvent;

                    if (doc is IDocumentEdi)
                    {
                        var ediDoc = doc as IDocumentEdi;

                        ediDoc.ProcessingResultEvent -= this.Vm_ProcessingResultEvent;
                    }

                    int idx = this.mFiles.IndexOf(doc);

                    this.mFiles.Remove(doc);
                    doc.Dispose();

                    if (this.Documents.Count > idx)
                        this.ActiveDocument = this.mFiles[idx];
                    else
                        if (this.Documents.Count > 1 && this.Documents.Count == idx)
                        this.ActiveDocument = this.mFiles[idx - 1];
                    else
                            if (this.Documents.Count == 0)
                        this.ActiveDocument = null;
                    else
                        this.ActiveDocument = this.mFiles[0];

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
                _MsgBox.Show(exp, Edi.Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             this.mAppCore.IssueTrackerLink,
                             this.mAppCore.IssueTrackerLink,
                             Edi.Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
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
            get
            {
                return this.mDialogCloseResult;
            }

            private set
            {
                if (this.mDialogCloseResult != value)
                {
                    this.mDialogCloseResult = value;
                    this.RaisePropertyChanged(() => this.DialogCloseResult);
                }
            }
        }

        /// <summary>
        /// Get/set property to determine whether window is in maximized state or not.
        /// (this can be handy to determine when a resize grip should be shown or not)
        /// </summary>
        public bool? IsNotMaximized
        {
            get
            {
                return this.mIsNotMaximized;
            }

            set
            {
                if (this.mIsNotMaximized != value)
                {
                    this.mIsNotMaximized = value;
                    this.RaisePropertyChanged(() => this.IsNotMaximized);
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
            get
            {
                return this.mIsWorkspaceAreaOptimized;
            }

            set
            {
                if (this.mIsWorkspaceAreaOptimized != value)
                {
                    this.mIsWorkspaceAreaOptimized = value;
                    this.RaisePropertyChanged(() => this.IsWorkspaceAreaOptimized);
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
            if (this.mShutDownInProgress == true)
                return true;

            try
            {
                if (this.mFiles != null)               // Close all open files and make sure there are no unsaved edits
                {                                     // If there are any: Ask user if edits should be saved
                    for (int i = 0; i < this.Files.Count; i++)
                    {
                        IFileBaseViewModel f = this.Files[i];

                        if (this.OnCloseSaveDirtyFile(f) == false)
                        {
                            this.mShutDownInProgress = false;
                            return false;               // Cancel shutdown process (return false) if user cancels saving edits
                        }
                    }
                }

                // Do layout serialization after saving/closing files
                // since changes implemented by shut-down process are otherwise lost
                try
                {
                    this.mAppCore.CreateAppDataFolder();
                    ////this.SerializeLayout(sender);            // Store the current layout for later retrieval
                }
                catch
                {
                }
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Edi.Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             this.mAppCore.IssueTrackerLink,
                             this.mAppCore.IssueTrackerLink,
                             Edi.Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
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
                if (this.Files.Count >= 0)
                {
                    EdiViewModel fi = this.Documents.SingleOrDefault(f => f.FilePath == fileNamePath);

                    if (fi != null)
                    {
                        this.ActiveDocument = fi;
                        return true;
                    }
                }
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                _MsgBox.Show(exp, Edi.Util.Local.Strings.STR_MSG_UnknownError_Caption,
                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                             this.mAppCore.IssueTrackerLink,
                             this.mAppCore.IssueTrackerLink,
                             Edi.Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
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
            List<StartPageViewModel> l = this.mFiles.OfType<StartPageViewModel>().ToList();

            if (l.Count == 0)
            {
                if (CreateNewViewModelIfNecessary == false)
                    return null;
                else
                {
                    var s = new StartPageViewModel(this.mSettingsManager.SessionData.MruList);

                    s.DocumentEvent += ProcessDocumentEvent;

                    this.mFiles.Add(s);

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
                        this.CloseDocument(f);
                    break;

                case FileEventType.AdjustCurrentPath:
                    if (f != null)
                    {
                        // Query for an explorer tool window and return it
                        var eplorerTW = this.GetToolWindowVM<IExplorer>();

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
                if (f is EdiViewModel eVM)
                    eVM.ProcessingResultEvent -= Vm_ProcessingResultEvent;

                this.Close(f);
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

            if (sender is IDocumentFileWatcher watcher)
            {
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
                                this.CloseDocument(vm);
                                vm = null;

                                if (error != null && filePath != null)
                                {
                                    if (error is FileNotFoundException)
                                    {
                                        if (this.mSettingsManager.SessionData.MruList.FindMRUEntry(filePath) != null)
                                        {
                                            if (_MsgBox.Show(string.Format(Edi.Util.Local.Strings.STR_ERROR_LOADING_FILE_MSG, filePath),
                                                             Edi.Util.Local.Strings.STR_ERROR_LOADING_FILE_CAPTION, MsgBoxButtons.YesNo) == MsgBoxResult.Yes)
                                            {
                                                this.mSettingsManager.SessionData.MruList.RemoveEntry(filePath);
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
                        if (this.GetStartPage(false) == null)
                        {
                            ret = this.GetStartPage(true);
                        }
                        break;

                    default:
                        if (path.Contains("<") == true && path.Contains(">") == true)
                        {
                            this.mMessageManager.Output.AppendLine(
                                string.Format("Warning: Cannot resolve tool window or document page: '{0}'.", path));

                            this.mMessageManager.Output.AppendLine(
                                string.Format("Check the current program configuration to make that it is present.", path));

                            return null;
                        }

                        // Re-create Edi document (text file or log4net document) content
                        ret = this.Open(path, CloseDocOnError.WithoutUserNotification);
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
            return this.Tools.FirstOrDefault(d => d as T != null) as T;
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
                    this.RaisePropertyChanged(() => this.RecentFiles);
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
            if (this.mMessageManager.Output != null)
            {
                this.mMessageManager.Output.AppendLine(
                string.Format("Loading MEF Module: {0},\n" +
                                            "                    Type: {1},\n" +
                                            "     Initialization Mode: {2},\n" +
                                            "                   State: {3}, Ref: '{4}'\n", e.ModuleInfo.ModuleName,
                                                                                                                                         e.ModuleInfo.ModuleType,
                                                                                                                                         e.ModuleInfo.InitializationMode,
                                                                                                                                         e.ModuleInfo.State,
                                                                                                                                         e.ModuleInfo.Ref));
            }
        }
        #endregion methods
    }
}
