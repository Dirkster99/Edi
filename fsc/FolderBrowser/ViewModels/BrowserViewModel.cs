namespace FolderBrowser.ViewModels
{
    using FileSystemModels;
    using FileSystemModels.Browse;
    using FileSystemModels.Interfaces;
    using FileSystemModels.Interfaces.Bookmark;
    using FileSystemModels.Models.FSItems;
    using FileSystemModels.Models.FSItems.Base;
    using FileSystemModels.ViewModels.Base;
    using FolderBrowser.Interfaces;
    using FolderBrowser.ViewModels.Messages;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    /// <summary>
    /// A browser viewmodel is about managing activities and properties related
    /// to displaying a treeview that repesents folders in the file system.
    /// 
    /// This viewmodel is almost equivalent to the backend code needed to drive
    /// the Treeview that shows the items in the UI.
    /// </summary>
    internal class BrowserViewModel : ViewModelBase, IBrowserViewModel
    {
        #region fields
        private string _SelectedFolder;
        private bool _IsSpecialFoldersVisisble;

        private ICommand _ExpandCommand;
        private ICommand _FolderSelectedCommand = null;
        private ICommand _SelectedFolderChangedCommand;
        private ICommand _OpenInWindowsCommand = null;
        private ICommand _CopyPathCommand = null;
        private ICommand _RenameCommand;
        private ICommand _StartRenameCommand;
        private ICommand _CreateFolderCommand;
        private ICommand _CancelBrowsingCommand;
        private ICommand _RefreshViewCommand;

        private bool _IsBrowsing;

        private bool _IsExpanding = false;

        private string _InitalPath;
        private bool _UpdateView;
        private bool _IsBrowseViewEnabled;
        private ITreeItemViewModel _SelectedItem = null;
        private SortableObservableDictionaryCollection _Root;
        private ObservableCollection<ICustomFolderItemViewModel> _SpecialFolders;
        private bool _IsExternallyBrowsing;
        #endregion fields

        #region constructor
        /// <summary>
        /// Standard class constructor
        /// </summary>
        public BrowserViewModel()
        {
            DisplayMessage = new DisplayMessageViewModel();
            BookmarkFolder = new EditFolderBookmarks();
            InitializeSpecialFolders();

            _OpenInWindowsCommand = null;
            _CopyPathCommand = null;

            _Root = new SortableObservableDictionaryCollection();

            InitialPath = string.Empty;

            _UpdateView = true;
            _IsBrowseViewEnabled = true;

            _IsBrowsing = true;
            _IsExternallyBrowsing = false;
        }
        #endregion constructor

        #region browsing events
        /// <summary>
        /// Indicates when the viewmodel starts heading off somewhere else
        /// and when its done browsing to a new location.
        /// </summary>
        public event EventHandler<BrowsingEventArgs> BrowseEvent;
        #endregion browsing events

        #region properties
        /// <summary>
        /// This property determines whether the control
        /// is to be updated right now or not. Switching off updates at times
        /// can save performance when browsing long and deep paths with multiple
        /// levels - so we:
        /// 1) Switch off view updates
        /// 2) Browse the structure to a target
        /// 3) Switch on updates and update view at current/new location.
        /// </summary>
        public bool UpdateView
        {
            get
            {
                return _UpdateView;
            }

            set
            {
                if (_UpdateView != value)
                {
                    _UpdateView = value;
                    RaisePropertyChanged(() => UpdateView);
                }
            }
        }

        public bool IsBrowseViewEnabled
        {
            get
            {
                return _IsBrowseViewEnabled;
            }

            set
            {
                if (_IsBrowseViewEnabled != value)
                {
                    _IsBrowseViewEnabled = value;
                    RaisePropertyChanged(() => IsBrowseViewEnabled);
                }
            }
        }

        /// <summary>
        /// Gets whether the tree browser is currently processing
        /// a request for brwosing to a known location.
        /// 
        /// Can only be set by the control if user started browser process
        /// 
        /// Use IsBrowsing and IsExternallyBrowsing to lock the controls UI
        /// during browse operations or display appropriate progress bar(s).
        /// </summary>
        public bool IsBrowsing
        {
            get
            {
                return _IsBrowsing;
            }

            private set
            {
                if (_IsBrowsing != value)
                {
                    _IsBrowsing = value;
                    RaisePropertyChanged(() => IsBrowsing);
                }
            }
        }

        /// <summary>
        /// This should only be set by the controller that started the browser process.
        /// 
        /// Use IsBrowsing and IsExternallyBrowsing to lock the controls UI
        /// during browse operations or display appropriate progress bar(s).
        /// </summary>
        public bool IsExternallyBrowsing
        {
            get
            {
                return _IsExternallyBrowsing;
            }

            protected set
            {
                if (_IsExternallyBrowsing != value)
                {
                    _IsExternallyBrowsing = value;
                    RaisePropertyChanged(() => IsExternallyBrowsing);
                }
            }
        }

        /// <summary>
        /// Gets the list of drives and folders for display in treeview structure control.
        /// </summary>
        public IEnumerable<ITreeItemViewModel> Root
        {
            get
            {
                return _Root;
            }
        }

        /// <summary>
        /// Get/set currently selected folder.
        /// 
        /// This property is used as output of the current path
        /// but also used as a parameter when browsing to a new path.
        /// </summary>
        public string SelectedFolder
        {
            get
            {
                return this._SelectedFolder;
            }

            set
            {
                if (this._SelectedFolder != value)
                {
                    this._SelectedFolder = value;
                    this.RaisePropertyChanged(() => this.SelectedFolder);
                }
            }
        }

        /// <summary>
        /// Gets the currently selected viewmodel object (if any).
        /// </summary>
        public ITreeItemViewModel SelectedItem
        {
            get
            {
                return _SelectedItem;
            }

            private set
            {
                if (_SelectedItem != value)
                {
                    _SelectedItem = value;
                    RaisePropertyChanged(() => SelectedItem);

                    if (_SelectedItem != null)
                        SelectedFolder = _SelectedItem.ItemPath;
                    else
                        SelectedFolder = string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets a command to cancel the current browsing process.
        /// </summary>
        public ICommand CancelBrowsingCommand
        {
            get
            {
                if (_CancelBrowsingCommand == null)
                {
                    _CancelBrowsingCommand = new RelayCommand<object>((p) =>
                    {
//// TODO XXX
////                        if (_Processor != null)
////                        {
////                            if (_Processor.IsCancelable == true)
////                                _Processor.Cancel();
////                        }
                    },
                    (p) =>
                    {
                        if (IsBrowsing == true)
                        {
//// TODO XXX                            if (_Processor.IsCancelable == true)
////                                 return _Processor.IsProcessing;
                        }

                        return false;
                    });
                }

                return _CancelBrowsingCommand;
            }
        }

        /// <summary>
        /// Gets a command that will open the selected item with the current default application
        /// in Windows. The selected item (path to a file) is expected as FSItemVM parameter.
        /// (eg: Item is HTML file -> Open in Windows starts the web browser for viewing the HTML
        /// file if thats the currently associated Windows default application.
        /// </summary>
        public ICommand OpenInWindowsCommand
        {
            get
            {
                if (_OpenInWindowsCommand == null)
                    _OpenInWindowsCommand = new RelayCommand<object>(
                      (p) =>
                      {
                          var vm = p as ITreeItemViewModel;

                          if (vm == null)
                              return;

                          if (string.IsNullOrEmpty(vm.ItemPath) == true)
                              return;

                          FileSystemCommands.OpenContainingFolder(vm.ItemPath);
                      });

                return _OpenInWindowsCommand;
            }
        }

        /// <summary>
        /// Gets a command that will copy the path of an item into the Windows Clipboard.
        /// The item (path to a file) is expected as FSItemVM parameter.
        /// </summary>
        public ICommand CopyPathCommand
        {
            get
            {
                if (_CopyPathCommand == null)
                    _CopyPathCommand = new RelayCommand<object>(
                      (p) =>
                      {
                          var vm = p as ITreeItemViewModel;

                          if (vm == null)
                              return;

                          if (string.IsNullOrEmpty(vm.ItemPath) == true)
                              return;

                          FileSystemCommands.CopyPath(vm.ItemPath);
                      });

                return _CopyPathCommand;
            }
        }

        /// <summary>
        /// Gets a command that executes when the selected item in the treeview has changed.
        /// This updates a text property to inform other attached dependencies property controls
        /// about this change in selection state.
        /// </summary>
        public ICommand SelectedFolderChangedCommand
        {
            get
            {
                if (_SelectedFolderChangedCommand == null)
                {
                    _SelectedFolderChangedCommand = new RelayCommand<object>((p) =>
                    {
                        var param = p as ITreeItemViewModel;

                        if (param != null)
                        {
                            SelectedItem = param;

                            try
                            {
                                var location = PathFactory.Create(param.ItemPath);
                                if (BrowseEvent != null)
                                    BrowseEvent(this, new BrowsingEventArgs(location, false, BrowseResult.Complete));
                            }
                            catch
                            {
                            }

                        }
                    });
                }

                return _SelectedFolderChangedCommand;
            }
        }

        /// <summary>
        /// Gets a command that executes when a user expands a tree view item node in the treeview.
        /// </summary>
        public ICommand ExpandCommand
        {
            get
            {
                if (_ExpandCommand == null)
                {
                    _ExpandCommand = new RelayCommand<object>(async (p) =>
                    {
                        if (IsBrowsing == true) // This is probably not relevant since the
                        {                      // viewmodel is currently processing a navigation
                            return;           // request to browse the view to a new location...
                        }

                        var expandedItem = p as ITreeItemViewModel;

                        if (expandedItem != null && _IsExpanding == false)
                        {
                            if (expandedItem.HasDummyChild == true)
                                await ExpandDummyFolderAsync(expandedItem);
                        }
                    });
                }

                return _ExpandCommand;
            }
        }

        /// <summary>
        /// Starts the rename folder process on the CommandParameter
        /// which must be FolderViewModel item that represented the to be renamed folder.
        /// 
        /// This command implements an event that triggers the actual rename
        /// process in the connected view.
        /// </summary>
        public ICommand StartRenameCommand
        {
            get
            {
                if (this._StartRenameCommand == null)
                    this._StartRenameCommand = new RelayCommand<object>(it =>
                    {
                        var folder = it as FolderViewModel;

                        if (folder != null)
                        {
                            folder.RequestEditMode(InplaceEditBoxLib.Events.RequestEditEvent.StartEditMode);
                        }
                    },
                    (it) =>
                    {
                        var folder = it as FolderViewModel;

                        if (folder != null)
                        {
                            if (folder.IsReadOnly == true)
                                return false;
                        }

                        return true;
                    });

                return this._StartRenameCommand;
            }
        }

        /// <summary>
        /// Renames the folder that is represented by this viewmodel.
        /// This command should be called directly by the implementing view
        /// since the new name of the folder is delivered in the
        /// CommandParameter as a string.
        /// </summary>
        public ICommand RenameCommand
        {
            get
            {
                if (this._RenameCommand == null)
                    this._RenameCommand = new RelayCommand<object>(it =>
                    {
                        var tuple = it as Tuple<string, object>;

                        if (tuple != null)
                        {
                            var folderVM = tuple.Item2 as FolderViewModel;

                            var newFolderName = tuple.Item1;

                            if (folderVM == null)
                                return;

                            if (string.IsNullOrEmpty(newFolderName) == false &&
                                folderVM != null)
                            {
                                var parent = folderVM.Parent;
                                if (parent != null)
                                {
                                    parent.ChildRename(folderVM.ItemName, newFolderName);

                                    this.SelectedFolder = folderVM.ItemPath;
                                }
                            }
                        }
                    });

                return this._RenameCommand;
            }
        }

        /// <summary>
        /// Starts the create folder process by creating a new folder
        /// in the given location. The location is supplied as <seealso cref="System.Windows.Input.ICommandSource.CommandParameter"/>
        /// which is a <seealso cref="ITreeItemViewModel"/> item.
        /// 
        /// So, the <seealso cref="ITreeItemViewModel"/> item is the parent of the new folder
        /// <seealso cref="IFolderViewModel"/> and the new folder is created with a standard
        /// name: 'New Folder n'. The new folder n is selected and in rename mode such that
        /// users can edit the name of the new folder right away.
        /// 
        /// This command implements an event that triggers the actual rename process in the
        /// connected view.
        /// </summary>
        public ICommand CreateFolderCommand
        {
            get
            {
                if (this._CreateFolderCommand == null)
                    this._CreateFolderCommand = new RelayCommand<object>(async it =>
                    {
                        var folder = it as TreeItemViewModel;

                        if (folder == null)
                            return;

                        if (folder.IsExpanded == false)
                        {
                            folder.IsExpanded = true;

                            // Refresh child items if this has been expanded for the 1st time
                            if (folder.HasDummyChild == true)
                            {
                                var x = await RequeryChildItems(folder);
                            }
                        }

                        this.CreateFolderCommandNewFolder(folder);
                    });

                return this._CreateFolderCommand;
            }
        }

        /// <summary>
        /// Gets command to select the current folder.
        /// 
        /// This binding can be used for browsing to a certain folder
        /// e.g. Users Document folder.
        /// 
        /// Expected parameter: string containing a path to be browsed to.
        /// </summary>
        [System.Obsolete("This is no longer supported.")]
        public ICommand FolderSelectedCommand
        {
            get
            {
                if (this._FolderSelectedCommand == null)
                {
                    this._FolderSelectedCommand = new RelayCommand<object>(p =>
                    {
                        string path = p as string;

                        if (string.IsNullOrEmpty(path) == true)
                            return;

                        if (IsBrowsing == true)
                            return;

                        IPathModel location = null;
                        try
                        {
                            location = PathFactory.Create(path);
                            NavigateTo(location, false);
                        }
                        catch
                        {
                        }
                    },
                    (p) => { return ! IsBrowsing; });
                }

                return this._FolderSelectedCommand;
            }
        }

        /// <summary>
        /// Gets a command that will reload the folder view up to the
        /// selected path that is expected as <seealso cref="ITreeItemViewModel"/>
        /// in the CommandParameter.
        /// 
        /// This command is particularly useful when users create/delete a folder
        /// and want to update the treeview accordingly.
        /// </summary>
        public ICommand RefreshViewCommand
        {
            get
            {
                if (this._RefreshViewCommand == null)
                {
                    this._RefreshViewCommand = new RelayCommand<object>(p =>
                    {
                        try
                        {
                            var item = p as ITreeItemViewModel;

                            if (item == null)
                                return;

                            if (string.IsNullOrEmpty(item.ItemPath) == true)
                                return;

                            if (IsBrowsing == true)
                                return;

                            IPathModel location = null;
                            location = PathFactory.Create(item.ItemPath);
                            NavigateTo(location, false);
                        }
                        catch
                        {
                        }
                    }, (p) =>
                        {
                            return ! IsBrowsing;
                        });
                }

                return this._RefreshViewCommand;
            }
        }

        /// <summary>
        /// Expand folder for the very first time (using the process background viewmodel).
        /// </summary>
        /// <param name="expandedItem"></param>
        private async Task ExpandDummyFolderAsync(ITreeItemViewModel expandedItem)
        {
            if (expandedItem != null && _IsExpanding == false)
            {
                if (expandedItem.HasDummyChild == true)
                {
                    _IsExpanding = true;
                    try
                    {
                        if ((expandedItem is TreeItemViewModel) == true)
                        {
                            var item = expandedItem as TreeItemViewModel;

                            item.ChildrenClear();  // Requery sub-folders of this item
                            await item.ChildrenLoadAsync();
                        }
                    }
                    finally
                    {
                        _IsExpanding = false;
                    }
                }
            }
        }

        /// <summary>
        /// Methid executes when expand method is finished processing.
        /// </summary>
        /// <param name="processWasSuccessful"></param>
        /// <param name="exp"></param>
        /// <param name="caption"></param>
        private void ExpandProcessinishedEvent(bool processWasSuccessful, Exception exp, string caption)
        {
            _IsExpanding = false;
        }

        /// <summary>
        /// Requery all child items - this can be useful when we
        /// expand a folder for the very first time. Here we use task library with
        /// async to enable synchronization. This is for parts of other commands
        /// such as New Folder command which requires expansion of sub-folder
        /// items before actual New Folder command can execute.
        /// </summary>
        /// <param name="expandedItem"></param>
        /// <returns></returns>
        private async Task<bool> RequeryChildItems(TreeItemViewModel expandedItem)
        {
            await Task.Run(() => 
            {
                expandedItem.ChildrenClear();  // Requery sub-folders of this item
                expandedItem.ChildrenLoad();
            });

            return true;
        }

        /// <summary>
        /// Gets a property to an object that is used to pop-up UI messages when errors occur.
        /// </summary>
        public IDisplayMessageViewModel DisplayMessage { get; private set; }

        /// <summary>
        /// Expose properties to commands that work with the bookmarking of folders.
        /// </summary>
        public IEditBookmarks BookmarkFolder { get; private set; }

        #region SpecialFolders property
        /// <summary>
        /// Gets a list of Special Windows Standard folders for display in view.
        /// </summary>
        public IEnumerable<ICustomFolderItemViewModel> SpecialFolders
        {
            get
            {
                return _SpecialFolders;
            }
        }

        /// <summary>
        /// Gets whether the browser view should show a special folder control or not
        /// (A special folder control lets users browse to folders like 'My Documents'
        /// with a mouse click).
        /// </summary>
        public bool IsSpecialFoldersVisisble
        {
            get
            {
                return _IsSpecialFoldersVisisble;
            }

            private set
            {
                if (_IsSpecialFoldersVisisble != value)
                {
                    _IsSpecialFoldersVisisble = value;
                    RaisePropertyChanged(() => IsSpecialFoldersVisisble);
                }
            }
        }
        #endregion SpecialFolders property

        /// <summary>
        /// Get/set property to indicate the initial path when control
        /// starts up via Loading. The control attempts to change the
        /// current directory to the indicated directory if the
        /// ... method is called in the Loaded event of the
        /// <seealso cref="FolderBrowserDialog"/>.
        /// </summary>
        public string InitialPath
        {
            get
            {
                return _InitalPath;

            }

            set
            {
                if (_InitalPath != value)
                {
                    _InitalPath = value;
                    RaisePropertyChanged(() => InitialPath);
                }
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Controller can start browser process if IsBrowsing = false
        /// </summary>
        /// <param name="newPath"></param>
        /// <returns></returns>
        bool INavigateable.NavigateTo(IPathModel location)
        {
            return NavigateTo(location, false);
        }

        /// <summary>
        /// Controller can start browser process if IsBrowsing = false
        /// </summary>
        /// <param name="newPath"></param>
        /// <returns></returns>
        async Task<bool> INavigateable.NavigateToAsync(IPathModel location)
        {
            return await Task.Run(() =>
            {
                return NavigateTo(location, false);
            });
        }

        /// <summary>
        /// Sets the IsExternalBrowsing state and cleans up any running processings
        /// if any. This method should only be called by an external controll instance.
        /// </summary>
        /// <param name="isBrowsing"></param>
        public void SetExternalBrowsingState(bool isBrowsing)
        {
            IsExternallyBrowsing = isBrowsing;
        }

        /// <summary>
        /// Determines whether the list of Windows special folder shortcut
        /// buttons (Music, Video etc) is visible or not.
        /// </summary>
        /// <param name="visible"></param>
        public void SetSpecialFoldersVisibility(bool visible)
        {
            this.IsSpecialFoldersVisisble = visible;
        }

        /// <summary>
        /// Controller can start browser process if IsBrowsing = false
        /// </summary>
        /// <param name="newPath"></param>
        /// <returns></returns>
        private bool NavigateTo(IPathModel location,
                                bool ResetBrowserStatus = true)
        {
            CancellationTokenSource cts = null;
            bool ret = false;

            IsBrowsing = true;
            IsBrowseViewEnabled = UpdateView = false;

            try
            {
                ret = InternalBrowsePathAsync(location.Path, ResetBrowserStatus, cts);
            }
            finally
            {
                // Make sure that view updates at the end of browsing process
                IsBrowsing = false;
                IsBrowseViewEnabled = UpdateView = true;
            }

            return ret;
        }

        /// <summary>
        /// Initialize the treeview with a set of local drives
        /// currently available on the computer.
        /// </summary>
        private void SetInitialDrives(CancellationTokenSource cts = null)
        {
            ClearFoldersCollections();

            var items = DriveModel.GetLogicalDrives().ToList();

            foreach (var item in items)
            {
                if (cts != null)
                    cts.Token.ThrowIfCancellationRequested();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    var vmItem = new DriveViewModel(item.Model, null);
                    _Root.AddItem(vmItem);
                });
            }
        }

        private void ClearFoldersCollections()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _Root.Clear();
            });
        }

        private bool InternalBrowsePathAsync(string path,
                                             bool ResetBrowserStatus,
                                             CancellationTokenSource cts = null)
        {
            if (ResetBrowserStatus == true)
                ClearBrowserStates();

            if (System.IO.Directory.Exists(path) == false)
            {
                DisplayMessage.IsErrorMessageAvailable = true;
                DisplayMessage.Message = string.Format(FileSystemModels.Local.Strings.STR_ERROR_FOLDER_DOES_NOT_EXIST, path);
                return false;
            }

            if (_Root.Count == 0)        // Make sure drives are available
                SetInitialDrives(cts);

            if (cts != null)
                cts.Token.ThrowIfCancellationRequested();

            var pathItem = SelectDirectory(PathFactory.Create(path, FSItemType.Folder), cts);

            if (pathItem != null)
            {
                if (pathItem.Length > 0)
                    SelectedItem = pathItem[pathItem.Length - 1];

                return true;
            }

            return false;
        }

        internal ITreeItemViewModel[] SelectDirectory(
            IPathModel inputPath,
            CancellationTokenSource cts = null)
        {
            try
            {
                // Check if a given path exists
                var exists = PathFactory.DirectoryPathExists(inputPath.Path);

                if (exists == false)
                    return null;

                // Transform string into array of normalized path elements
                // Drive 'C:\' , 'Folder', 'SubFolder', etc...
                var folders = PathFactory.GetDirectories(inputPath.Path);

                if (folders != null)
                {
                    // Find the drive that is the root of this path
                    var drive = this._Root.TryGet(folders[0]);

                    return NavigatePath(drive, folders);
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private ITreeItemViewModel[] NavigatePath(
            ITreeItemViewModel parent
          , string[] folders
          , int iMatchIdx = 0)
        {
            ITreeItemViewModel[] pathFolders = new ITreeItemViewModel[folders.Count()];

            pathFolders[0] = parent;

            int iNext = iMatchIdx + 1;
            for (; iNext < folders.Count(); iNext++)
            {
                if (parent.HasDummyChild == true)
                    parent.ChildrenLoad();

                var nextChild = parent.ChildTryGet(folders[iNext]);

                if (nextChild != null)
                {
                    pathFolders[iNext] = nextChild;
                    parent = nextChild;
                }
                else
                    return null; // couln not find target
            }

            return pathFolders;
        }

        /// <summary>
        /// Create a new folder underneath the given parent folder. This method creates
        /// the folder with a standard name (eg 'New folder n') on disk and selects it
        /// in editing mode to give users a chance for renaming it right away.
        /// </summary>
        /// <param name="parentFolder"></param>
        private void CreateFolderCommandNewFolder(TreeItemViewModel parentFolder)
        {
            if (parentFolder == null)
                return;

            // Cast this to access internal methods and setters
            var item = parentFolder.CreateNewDirectory();
            var newSubFolder = item as FolderViewModel;
            SelectedItem = newSubFolder;

            if (newSubFolder != null)
            {
                // Do this with low priority (thanks for that tip to Joseph Leung)
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, (Action)delegate
                {
                    newSubFolder.IsSelected = true;
                    newSubFolder.RequestEditMode(InplaceEditBoxLib.Events.RequestEditEvent.StartEditMode);
                });
            }
        }

        /// <summary>
        /// Clear states of browser control (hide error message and other things that may not apply now)
        /// </summary>
        private void ClearBrowserStates()
        {
            DisplayMessage.Message = string.Empty;
            DisplayMessage.IsErrorMessageAvailable = false;
        }

        private void InitializeSpecialFolders()
        {
            _SpecialFolders = new ObservableCollection<ICustomFolderItemViewModel>();

            try
            {
                _SpecialFolders.Add(new CustomFolderItemViewModel(Environment.SpecialFolder.Desktop));
                _SpecialFolders.Add(new CustomFolderItemViewModel(Environment.SpecialFolder.MyDocuments));
                _SpecialFolders.Add(new CustomFolderItemViewModel(Environment.SpecialFolder.MyMusic));
                _SpecialFolders.Add(new CustomFolderItemViewModel(Environment.SpecialFolder.MyPictures));
                _SpecialFolders.Add(new CustomFolderItemViewModel(Environment.SpecialFolder.MyVideos));
            }
            catch
            {
            }
        }
        #endregion methods
    }
}
