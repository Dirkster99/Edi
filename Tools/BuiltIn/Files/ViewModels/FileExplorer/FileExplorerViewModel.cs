namespace Files.ViewModels.FileExplorer
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Input;
    using Edi.Core.Interfaces;
    using Edi.Core.Interfaces.Enums;
    using Edi.Core.ViewModels;
    using FileSystemModels.Interfaces;
    using FileSystemModels.Models;
    using FolderBrowser.Interfaces;
    using Edi.Settings.Interfaces;
    using Edi.Core.ViewModels.Command;
    using FileSystemModels;
    using System.Threading;
    using FileSystemModels.Browse;
    using System.Threading.Tasks;
    using Files.ViewModels.FileExplorer.Tasks;

    /// <summary>
    /// This class can be used to present file based information, such as,
    /// Size, Path etc to the user.
    /// </summary>
    public class FileExplorerViewModel : ControllerBaseViewModel,
										 IRegisterableToolWindow,
                                         IExplorer,
                                         ITreeListControllerViewModel
    {
		#region fields
		public const string ToolContentId = "<FileExplorerTool>";

        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string mFilePathName = string.Empty;

		private Func<string, bool> mFileOpenMethod = null;

		private RelayCommand<object> mSyncPathWithCurrentDocumentCommand = null;

		private IDocumentParent mParent = null;

		private readonly IFileOpenService mFileOpenService;

        private readonly SemaphoreSlim _SlowStuffSemaphore;
        private readonly OneTaskLimitedScheduler _OneTaskScheduler;
        private readonly CancellationTokenSource _CancelTokenSourc;
        private bool _disposed = false;

        private readonly string _InitialPath;

        private string _SelectedFolder = string.Empty;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public FileExplorerViewModel(ISettingsManager programSettings,
									IFileOpenService fileOpenService)
			: base("Explorer")
		{
			base.ContentId = ToolContentId;

			this.mFileOpenService = fileOpenService;

			this.OnActiveDocumentChanged(null, null);

            // 
            _SlowStuffSemaphore = new SemaphoreSlim(1, 1);
            _OneTaskScheduler = new OneTaskLimitedScheduler();
            _CancelTokenSourc = new CancellationTokenSource();

            FolderItemsView = FileListView.Factory.CreateFileListViewModel();
            FolderTextPath = FolderControlsLib.Factory.CreateFolderComboBoxVM();
            RecentFolders = FileSystemModels.Factory.CreateBookmarksViewModel();
            TreeBrowser = FolderBrowser.FolderBrowserFactory.CreateBrowserViewModel(false);

            // This is fired when the user selects a new folder bookmark from the drop down button
            RecentFolders.BrowseEvent += FolderTextPath_BrowseEvent;

            // This is fired when the text path in the combobox changes to another existing folder
            FolderTextPath.BrowseEvent += FolderTextPath_BrowseEvent;

            Filters = FilterControlsLib.Factory.CreateFilterComboBoxViewModel();
            Filters.OnFilterChanged += this.FileViewFilter_Changed;

            // This is fired when the current folder in the listview changes to another existing folder
            this.FolderItemsView.BrowseEvent += FolderTextPath_BrowseEvent;

            // Event fires when the user requests to add a folder into the list of recently visited folders
            this.FolderItemsView.BookmarkFolder.RequestEditBookmarkedFolders += this.FolderItemsView_RequestEditBookmarkedFolders;

            // This event is fired when a user opens a file
            this.FolderItemsView.OnFileOpen += this.FolderItemsView_OnFileOpen;

            TreeBrowser.BrowseEvent += FolderTextPath_BrowseEvent;

            // Event fires when the user requests to add a folder into the list of recently visited folders
            TreeBrowser.BookmarkFolder.RequestEditBookmarkedFolders += this.FolderItemsView_RequestEditBookmarkedFolders;

			ExplorerSettingsModel settings = null;

			if (programSettings != null)
			{
				if (programSettings.SessionData != null)
				{
					settings = programSettings.SettingData.ExplorerSettings;
				}
			}

			if (settings == null)
				settings = new ExplorerSettingsModel();

			if (programSettings.SessionData.LastActiveExplorer != null)
				settings.SetUserProfile(programSettings.SessionData.LastActiveExplorer);
			else
				settings.UserProfile.SetCurrentPath(@"C:");

            if (settings.UserProfile.CurrentPath != null)
                _InitialPath = settings.UserProfile.CurrentPath.Path;


            this.ConfigureExplorerSettings(settings);
            this.mFileOpenMethod = this.mFileOpenService.FileOpen;
		}
        #endregion constructor

        #region properties
        /// <summary>
        /// Gets the viewmodel that drives the folder picker control.
        /// </summary>
        public IBrowserViewModel TreeBrowser { get; }

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

        /// <summary>
        /// ToolWindow Icon
        /// </summary>
        public override Uri IconSource
		{
			get
			{
				return new Uri("pack://application:,,,/FileListView;component/Images/Generic/Folder/folderopened_yellow_16.png", UriKind.RelativeOrAbsolute);
			}
		}

		#region Commands
		/// <summary>
		/// Can be executed to synchronize the current path with the currently active document.
		/// </summary>
		public ICommand SyncPathWithCurrentDocumentCommand
		{
			get
			{
				if (this.mSyncPathWithCurrentDocumentCommand == null)
					this.mSyncPathWithCurrentDocumentCommand = new RelayCommand<object>(
						(p) => this.SyncPathWithCurrentDocumentCommand_Executed(),
						(p) => string.IsNullOrEmpty(this.mFilePathName) == false);

				return this.mSyncPathWithCurrentDocumentCommand;
			}
		}



		#endregion Commands

        public override PaneLocation PreferredLocation
		{
			get { return PaneLocation.Right; }
		}
		#endregion properties

		#region methods
		/// <summary>
		/// Save the current user profile settings into the
		/// corresponding property of the SettingsManager.
		/// </summary>
		/// <param name="settingsManager"></param>
		/// <param name="vm"></param>
		public static void SaveSettings(ISettingsManager settingsManager,
										IExplorer vm)
		{
			var settings = vm.GetExplorerSettings(settingsManager.SettingData.ExplorerSettings);

			if (settings != null) // Explorer settings have changed
			{
				settingsManager.SettingData.IsDirty = true;
				settingsManager.SettingData.ExplorerSettings = settings;

				settingsManager.SessionData.LastActiveExplorer = settings.UserProfile;
			}
			else
				settingsManager.SessionData.LastActiveExplorer = vm.GetExplorerSettings(null).UserProfile;
		}
/***
		/// <summary>
		/// Load Explorer (Tool Window) seetings from persistence.
		/// </summary>
		/// <param name="settingsManager"></param>
		/// <param name="vm"></param>
		public static void LoadSettings(ISettingsManager settingsManager,
										FileExplorerViewModel vm)
		{
			ExplorerSettingsModel settings = null;

			settings = settingsManager.SettingData.ExplorerSettings;

			if (settings == null)
				settings = new ExplorerSettingsModel();

			settings.SetUserProfile(settingsManager.SessionData.LastActiveExplorer);

			// (re-)configure previous explorer settings and (re-)activate current location
			vm.ConfigureExplorerSettings(settings);
		}
***/
		#region IRegisterableToolWindow
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
		#endregion IRegisterableToolWindow

		public void OnActiveDocumentChanged(object sender, DocumentChangedEventArgs e)
		{
			this.mFilePathName = string.Empty;

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
                            }
                        }
                        catch
                        {
                        }
                    }
                }
			}
		}

		/// <summary>
		/// Executes when the file open event is fired and class was constructed with statndard constructor.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void FolderItemsView_OnFileOpen(object sender, FileSystemModels.Events.FileOpenEventArgs e)
		{
			if (this.mFileOpenMethod != null)
				this.mFileOpenMethod(e.FileName);
			else
				MessageBox.Show("File Open (method is null):" + e.FileName);
		}

		/// <summary>
		/// Navigates to viewmodel to the <paramref name="directoryPath"/> folder.
		/// </summary>
		/// <param name="directoryPath"></param>
		public void NavigateToFolder(string directoryPath)
		{
            IPathModel location = null;
			try
			{
				if (System.IO.Directory.Exists(directoryPath) == false)
					directoryPath = System.IO.Directory.GetParent(directoryPath).FullName;

				if (System.IO.Directory.Exists(directoryPath) == false)
					return;

                location = PathFactory.Create(directoryPath);
            }
			catch
			{
			}

            NavigateToFolder(location);
        }

        private void SyncPathWithCurrentDocumentCommand_Executed()
		{
			if (string.IsNullOrEmpty(this.mFilePathName) == true)
				return;

			NavigateToFolder(this.mFilePathName);
		}

        // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
        // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
        // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

        /// <summary>
        /// Master controller interface method to navigate all views
        /// to the folder indicated in <paramref name="folder"/>
        /// - updates all related viewmodels.
        /// </summary>
        /// <param name="itemPath"></param>
        /// <param name="requestor"</param>
        public override void NavigateToFolder(IPathModel itemPath)
        {
            try
            {
                // XXX Todo Keep task reference, support cancel, and remove on end?
                var timeout = TimeSpan.FromSeconds(5);
                var actualTask = new Task(() =>
                {
                    var request = new BrowseRequest(itemPath, _CancelTokenSourc.Token);

                    var t = Task.Factory.StartNew(() => NavigateToFolderAsync(request, null),
                                                        request.CancelTok,
                                                        TaskCreationOptions.LongRunning,
                                                        _OneTaskScheduler);

                    if (t.Wait(timeout) == true)
                        return;

                    _CancelTokenSourc.Cancel();       // Task timed out so lets abort it
                    return;                     // Signal timeout here...
                });

                actualTask.Start();
                actualTask.Wait();
            }
            catch (System.AggregateException e)
            {
                Logger.Error(e);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        #region Disposable Interfaces
        /// <summary>
        /// Standard dispose method of the <seealso cref="IDisposable" /> interface.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Source: http://www.codeproject.com/Articles/15360/Implementing-IDisposable-and-the-Dispose-Pattern-P
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing == true)
                {
                    // Dispose of the curently displayed content
                    _OneTaskScheduler.Dispose();
                    _SlowStuffSemaphore.Dispose();
                    _CancelTokenSourc.Dispose();
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }

            _disposed = true;

            //// If it is available, make the call to the
            //// base class's Dispose(Boolean) method
            ////base.Dispose(disposing);
        }
        #endregion Disposable Interfaces

        /// <summary>
        /// Master controler interface method to navigate all views
        /// to the folder indicated in <paramref name="folder"/>
        /// - updates all related viewmodels.
        /// </summary>
        /// <param name="request"></param>
        private async Task<FinalBrowseResult> NavigateToFolderAsync(BrowseRequest request,
                                                                    object sender)
        {
            // Make sure the task always processes the last input but is not started twice
            await _SlowStuffSemaphore.WaitAsync();
            try
            {
                var newPath = request.NewLocation;
                var cancel = request.CancelTok;

                if (cancel != null)
                    cancel.ThrowIfCancellationRequested();

                TreeBrowser.SetExternalBrowsingState(true);
                FolderItemsView.SetExternalBrowsingState(true);
                FolderTextPath.SetExternalBrowsingState(true);

                FinalBrowseResult browseResult = null;

                // Navigate TreeView to this file system location
                if (TreeBrowser != sender)
                {
                    browseResult = await TreeBrowser.NavigateToAsync(request);

                    if (cancel != null)
                        cancel.ThrowIfCancellationRequested();

                    if (browseResult.Result != BrowseResult.Complete)
                        return FinalBrowseResult.FromRequest(request, BrowseResult.InComplete);
                }

                // Navigate Folder ComboBox to this folder
                if (FolderTextPath != sender)
                {
                    browseResult = await FolderTextPath.NavigateToAsync(request);

                    if (cancel != null)
                        cancel.ThrowIfCancellationRequested();

                    if (browseResult.Result != BrowseResult.Complete)
                        return FinalBrowseResult.FromRequest(request, BrowseResult.InComplete);
                }

                if (cancel != null)
                    cancel.ThrowIfCancellationRequested();

                // Navigate Folder/File ListView to this folder
                if (FolderItemsView != sender)
                {
                    browseResult = await FolderItemsView.NavigateToAsync(request);

                    if (cancel != null)
                        cancel.ThrowIfCancellationRequested();

                    if (browseResult.Result != BrowseResult.Complete)
                        return FinalBrowseResult.FromRequest(request, BrowseResult.InComplete);
                }

                if (browseResult != null)
                {
                    if (browseResult.Result == BrowseResult.Complete)
                    {
                        SelectedFolder = newPath.Path;

                        // Log location into history of recent locations
                        NaviHistory.Forward(newPath);
                    }
                }

                return browseResult;
            }
            catch (Exception exp)
            {
                var result = FinalBrowseResult.FromRequest(request, BrowseResult.InComplete);
                result.UnexpectedError = exp;
                return result;
            }
            finally
            {
                TreeBrowser.SetExternalBrowsingState(true);
                FolderItemsView.SetExternalBrowsingState(false);
                FolderTextPath.SetExternalBrowsingState(false);

                _SlowStuffSemaphore.Release();
            }
        }

        /// <summary>
        /// One of the controls has changed its location in the filesystem.
        /// This method is invoked to synchronize this change with all other controls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FolderTextPath_BrowseEvent(object sender,
                                                FileSystemModels.Browse.BrowsingEventArgs e)
        {
            var location = e.Location;

            SelectedFolder = location.Path;

            if (e.IsBrowsing == false && e.Result == BrowseResult.Complete)
            {
                NavigateToFolder(location);
            }
            else
            {
                if (e.IsBrowsing == true)
                {
                    // The sender has messaged: "I am chnaging location..."
                    // So, we set this property to tell the others:
                    // 1) Don't change your location now (eg.: Disable UI)
                    // 2) We'll be back to tell you the location when we know it
                    if (TreeBrowser != sender)
                        TreeBrowser.SetExternalBrowsingState(true);

                    if (FolderTextPath != sender)
                        FolderTextPath.SetExternalBrowsingState(true);

                    if (FolderItemsView != sender)
                        FolderItemsView.SetExternalBrowsingState(true);
                }
            }
        }

        /// <summary>
        /// Initialize Explorer ViewModel as soon as view is loaded.
        /// This is required for virtualized controls, such as, treeviews
        /// since they won't synchronize with the viewmodel, otherwise.
        /// </summary>
        internal void InitialzeOnLoad()
        {
            try
            {
                if (_InitialPath != null)
                    NavigateToFolder(_InitialPath);
                else
                    NavigateToFolder(@"C:\");
            }
            catch
            {
                NavigateToFolder(@"C:\");
            }
        }
        #endregion methods
    }
}
