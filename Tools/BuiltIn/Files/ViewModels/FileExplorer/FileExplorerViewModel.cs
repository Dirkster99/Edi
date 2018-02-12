namespace Files.ViewModels.FileExplorer
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Input;
    using Edi.Core.Interfaces;
    using Edi.Core.Interfaces.Enums;
    using Edi.Core.ViewModels;
    using FileListView.Interfaces;
    using FileSystemModels.Interfaces;
    using FileSystemModels.Models;
    using FolderBrowser.Interfaces;
    using Edi.Settings.Interfaces;
    using Edi.Core.ViewModels.Command;
    using FileSystemModels;
    using System.Threading;
    using FileSystemModels.Interfaces.Bookmark;
    using FolderControlsLib.Interfaces;
    using FilterControlsLib.Interfaces;
    using FileSystemModels.Browse;
    using System.Threading.Tasks;
    using FileSystemModels.Events;

    /// <summary>
    /// This class can be used to present file based information, such as,
    /// Size, Path etc to the user.
    /// </summary>
    public class FileExplorerViewModel : Edi.Core.ViewModels.ToolViewModel,
										 IRegisterableToolWindow,
										 IExplorer,
                                         ITreeListControllerViewModel
    {
		#region fields
		public const string ToolContentId = "<FileExplorerTool>";
		private string mFilePathName = string.Empty;

		private Func<string, bool> mFileOpenMethod = null;

		private RelayCommand<object> mSyncPathWithCurrentDocumentCommand = null;

		private IDocumentParent mParent = null;

		private readonly IFileOpenService mFileOpenService;

        private readonly SemaphoreSlim SlowStuffSemaphore;
        private readonly object _LockObject;
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
            SlowStuffSemaphore = new SemaphoreSlim(1, 1);
            _LockObject = new object();

            FolderItemsView = FileListView.Factory.CreateFileListViewModel(new BrowseNavigation());
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
				settings.UserProfile = programSettings.SessionData.LastActiveExplorer;
			else
				settings.UserProfile.SetCurrentPath(@"C:");

			this.ConfigureExplorerSettings(settings);
			this.mFileOpenMethod = this.mFileOpenService.FileOpen;
		}
        #endregion constructor

        #region properties
        /// <summary>
        /// Gets the viewmodel that exposes recently visited locations (bookmarked folders).
        /// </summary>
        public IBookmarksViewModel RecentFolders { get; }

        /// <summary>
        /// Expose a viewmodel that can represent a Folder-ComboBox drop down
        /// element similar to a web browser Uri drop down control but using
        /// local paths only.
        /// </summary>
        public IFolderComboBoxViewModel FolderTextPath { get; }

        /// <summary>
        /// Expose a viewmodel that can represent a Filter-ComboBox drop down
        /// similar to the top-right filter/search combo box in Windows Exploer windows.
        /// </summary>
        public IFilterComboBoxViewModel Filters { get; }

        /// <summary>
        /// Expose a viewmodel that can support a listview showing folders and files
        /// with their system specific icon.
        /// </summary>
        public IFileListViewModel FolderItemsView { get; }

        /// <summary>
        /// Gets the viewmodel that drives the folder picker control.
        /// </summary>
        public IBrowserViewModel TreeBrowser { get; }

        /// <summary>
        /// Gets the currently selected folder path string.
        /// </summary>
        public string SelectedFolder
        {
            get
            {
                return this._SelectedFolder;
            }

            private set
            {
                if (this._SelectedFolder != value)
                {
                    this._SelectedFolder = value;
                    base.RaisePropertyChanged(() => this.SelectedFolder);
                }
            }
        }

        /// <summary>
        /// Gets the currently selected recent location string (if any) or null.
        /// </summary>
        public string SelectedRecentLocation
        {
            get
            {
                if (this.RecentFolders != null)
                {
                    if (this.RecentFolders.SelectedItem != null)
                        return this.RecentFolders.SelectedItem.FullPath;
                }

                return null;
            }
        }

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
				return new Uri("pack://application:,,,/FileListView;component/Images/Generic/Folder/folderopened_yellow_16.png", UriKind.RelativeOrAbsolute);
			}
		}
		#endregion ToolWindow Icon

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

			settings.UserProfile = settingsManager.SessionData.LastActiveExplorer;

			// (re-)configure previous explorer settings and (re-)activate current location
			vm.ConfigureExplorerSettings(settings);
		}

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

                        if (File.Exists(f.FilePath) == true)
                        {
                            var fi = new FileInfo(f.FilePath);

                            this.mFilePathName = f.FilePath;

                            this.RaisePropertyChanged(() => this.FileName);
                            this.RaisePropertyChanged(() => this.FilePath);
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
		public void FolderItemsView_OnFileOpen(object sender, FileSystemModels.Events.FileOpenEventArgs e)
		{
			if (this.mFileOpenMethod != null)
				this.mFileOpenMethod(e.FileName);
			else
				MessageBox.Show("File Open (method is to null):" + e.FileName);
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

            // XXX Todo Keep task reference, support cancel, and remove on end?
            var t = NavigateToFolderAsync(location, null);
        }

        private void SyncPathWithCurrentDocumentCommand_Executed()
		{
			if (string.IsNullOrEmpty(this.mFilePathName) == true)
				return;

			NavigateToFolder(this.mFilePathName);
		}

        // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
        #region Explorer settings model
        /// <summary>
        /// Configure this viewmodel (+ attached browser viewmodel) with the given settings and
        /// initialize viewmodels with UserProfile.CurrentPath location.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public bool ConfigureExplorerSettings(ExplorerSettingsModel settings)
        {
            if (settings == null)
                return false;

            if (settings.UserProfile == null)
                return false;

            if (settings.UserProfile.CurrentPath == null)
                return false;

            try
            {
                // Set currently viewed folder in Explorer Tool Window
                var t = new Task(async () =>
                {
                    await this.NavigateToFolderAsync(settings.UserProfile.CurrentPath, null);
                });

                t.RunSynchronously();
            }
            catch
            {
                NavigateToFolder(@"C:\\");
            }

            try
            {
                this.Filters.ClearFilter();

                // Set file filter in file/folder list view
                foreach (var item in settings.FilterCollection)
                    this.Filters.AddFilter(item.FilterDisplayName, item.FilterText);

                // Add a current filter setting (if any is available)
                if (settings.UserProfile.CurrentFilter != null)
                {
                    this.Filters.SetCurrentFilter(settings.UserProfile.CurrentFilter.FilterDisplayName,
                                                  settings.UserProfile.CurrentFilter.FilterText);
                }

                this.RecentFolders.ClearFolderCollection();

                // Set collection of recent folder locations
                foreach (var item in settings.RecentFolders)
                    this.RecentFolders.AddFolder(item);

                this.FolderItemsView.SetShowIcons(settings.ShowIcons);
                this.FolderItemsView.SetIsFolderVisible(settings.ShowFolders);
                this.FolderItemsView.SetShowHidden(settings.ShowHiddenFiles);
                this.FolderItemsView.SetIsFiltered(settings.IsFiltered);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Compare given <paramref name="input"/> settings with current settings
        /// and return a new settings model if the current settings
        /// changed in comparison to the given <paramref name="input"/> settings.
        /// 
        /// Always return current settings if <paramref name="input"/> settings is null.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ExplorerSettingsModel GetExplorerSettings(ExplorerSettingsModel input)
        {
            var settings = new ExplorerSettingsModel();

            try
            {
                settings.UserProfile.SetCurrentPath(this.SelectedFolder);

                foreach (var item in settings.FilterCollection)
                {
                    if (item == settings.UserProfile.CurrentFilter)
                        this.AddFilter(item.FilterDisplayName, item.FilterText, true);
                    else
                        this.AddFilter(item.FilterDisplayName, item.FilterText);
                }

                foreach (var item in this.Filters.CurrentItems)
                {
                    if (item == this.Filters.SelectedItem)
                        settings.AddFilter(item.FilterDisplayName, item.FilterText, true);
                    else
                        settings.AddFilter(item.FilterDisplayName, item.FilterText);
                }

                foreach (var item in this.RecentFolders.DropDownItems)
                    settings.AddRecentFolder(item.FullPath);

                if (string.IsNullOrEmpty(this.SelectedRecentLocation) == false)
                {
                    settings.LastSelectedRecentFolder = this.SelectedRecentLocation;
                    settings.AddRecentFolder(this.SelectedRecentLocation);
                }

                settings.ShowIcons = this.FolderItemsView.ShowIcons;
                settings.ShowFolders = this.FolderItemsView.ShowFolders;
                settings.ShowHiddenFiles = this.FolderItemsView.ShowHidden;
                settings.IsFiltered = this.FolderItemsView.IsFiltered;

                if (ExplorerSettingsModel.CompareSettings(input, settings) == false)
                    return settings;
                else
                    return null;
            }
            catch
            {
                throw;
            }
        }
        #endregion Explorer settings model

        /// <summary>
        /// Master controller interface method to navigate all views
        /// to the folder indicated in <paramref name="folder"/>
        /// - updates all related viewmodels.
        /// </summary>
        /// <param name="itemPath"></param>
        /// <param name="requestor"</param>
        public void NavigateToFolder(IPathModel itemPath)
        {
            // XXX Todo Keep task reference, support cancel, and remove on end?
            var t = NavigateToFolderAsync(itemPath, null);
        }

        #region Bookmarked Folders Methods
        /// <summary>
        /// Add a recent folder location into the collection of recent folders.
        /// This collection can then be used in the folder combobox drop down
        /// list to store user specific customized folder short-cuts.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="selectNewFolder"></param>
        public void AddRecentFolder(string folderPath, bool selectNewFolder = false)
        {
            this.RecentFolders.AddFolder(folderPath, selectNewFolder);
        }

        /// <summary>
        /// Removes a recent folder location into the collection of recent folders.
        /// This collection can then be used in the folder combobox drop down
        /// list to store user specific customized folder short-cuts.
        /// </summary>
        /// <param name="path"></param>
        public void RemoveRecentFolder(string path)
        {
            if (string.IsNullOrEmpty(path) == true)
                return;

            this.RecentFolders.RemoveFolder(path);
        }

        /// <summary>
        /// Copies all of the given bookmars into the destionations bookmarks collection.
        /// </summary>
        /// <param name="srcRecentFolders"></param>
        /// <param name="dstRecentFolders"></param>
        public void CloneBookmarks(IBookmarksViewModel srcRecentFolders,
                                   IBookmarksViewModel dstRecentFolders)
        {
            if (srcRecentFolders == null || dstRecentFolders == null)
                return;

            dstRecentFolders.ClearFolderCollection();

            // Set collection of recent folder locations
            foreach (var item in srcRecentFolders.DropDownItems)
                dstRecentFolders.AddFolder(item.FullPath);
        }
        #endregion Bookmarked Folders Methods

        #region Change filter methods
        /// <summary>
        /// Add a new filter argument to the list of filters and
        /// select this filter if <paramref name="bSelectNewFilter"/>
        /// indicates it.
        /// </summary>
        /// <param name="filterString"></param>
        /// <param name="bSelectNewFilter"></param>
        public void AddFilter(string filterString,
                              bool bSelectNewFilter = false)
        {
            this.Filters.AddFilter(filterString, bSelectNewFilter);
        }

        /// <summary>
        /// Add a new filter argument to the list of filters and
        /// select this filter if <paramref name="bSelectNewFilter"/>
        /// indicates it.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="filterString"></param>
        /// <param name="bSelectNewFilter"></param>
        public void AddFilter(string name, string filterString,
                              bool bSelectNewFilter = false)
        {
            this.Filters.AddFilter(name, filterString, bSelectNewFilter);
        }
        #endregion Change filter methods

        /// <summary>
        /// Applies the file/directory filter from the combobox on the listview entries.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileViewFilter_Changed(object sender, FileSystemModels.Events.FilterChangedEventArgs e)
        {
            this.FolderItemsView.ApplyFilter(e.FilterText);
        }

        /// <summary>
        /// The list view of folders and files requests to add or remove a folder
        /// to/from the collection of recent folders.
        /// -> Forward event to <seealso cref="FolderComboBoxViewModel"/> who manages
        /// the actual list of recent folders.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FolderItemsView_RequestEditBookmarkedFolders(object sender, EditBookmarkEvent e)
        {
            switch (e.Action)
            {
                case EditBookmarkEvent.RecentFolderAction.Remove:
                    this.RecentFolders.RemoveFolder(e.Folder);
                    break;

                case EditBookmarkEvent.RecentFolderAction.Add:
                    this.RecentFolders.AddFolder(e.Folder.Path);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Master controler interface method to navigate all views
        /// to the folder indicated in <paramref name="folder"/>
        /// - updates all related viewmodels.
        /// </summary>
        /// <param name="itemPath"></param>
        /// <param name="requestor"</param>
        private async Task NavigateToFolderAsync(IPathModel itemPath, object sender)
        {
            // Make sure the task always processes the last input but is not started twice
            await SlowStuffSemaphore.WaitAsync();
            try
            {
                TreeBrowser.SetExternalBrowsingState(true);
                FolderItemsView.SetExternalBrowsingState(true);
                FolderTextPath.SetExternalBrowsingState(true);

                bool? browseResult = null;

                // Navigate TreeView to this file system location
                if (TreeBrowser != sender)
                    browseResult = await TreeBrowser.NavigateToAsync(itemPath);

                // Navigate Folder ComboBox to this folder
                if (FolderTextPath != sender && browseResult != false)
                    browseResult = await FolderTextPath.NavigateToAsync(itemPath);

                // Navigate Folder/File ListView to this folder
                if (FolderItemsView != sender && browseResult != false)
                    browseResult = await FolderItemsView.NavigateToAsync(itemPath);

                if (browseResult == true)
                    SelectedFolder = itemPath.Path;
            }
            catch { }
            finally
            {
                TreeBrowser.SetExternalBrowsingState(true);
                FolderItemsView.SetExternalBrowsingState(false);
                FolderTextPath.SetExternalBrowsingState(false);

                SlowStuffSemaphore.Release();
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
                // XXX Todo Keep task reference, support cancel, and remove on end?
                var t = NavigateToFolderAsync(location, sender);
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
        #endregion methods
    }
}
