namespace FileListView.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using FileListView.Command;
    using FileListView.ViewModels.Interfaces;
    using FileSystemModels.Events;
    using FileSystemModels.Interfaces;
    using FileSystemModels.Models;
    using FileSystemModels.Utils;
    using MsgBox;
    using UserNotification.ViewModel;

    /// <summary>
    /// Class implements a list of file items viewmodel for a given directory.
    /// </summary>
    public class FileListViewModel : Base.ViewModelBase, IFileListViewModel
    {
        #region fields
        /// <summary>
        /// Log4Net facility to log errors and warnings for this class.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string mFilterString = string.Empty;
        private string[] mParsedFilter = null;

        private bool mShowFolders = true;
        private bool mShowHidden = true;
        private bool mShowIcons = true;
        private bool mIsFiltered = false;
        private FSItemViewModel mSelectedItem;

        private IBrowseNavigation mBrowseNavigation = null;

        private RelayCommand<object> mNavigateForwardCommand = null;
        private RelayCommand<object> mNavigateBackCommand = null;
        private RelayCommand<object> mNavigateUpCommand = null;
        private RelayCommand<object> mNavigateDownCommand = null;
        private RelayCommand<object> mRefreshCommand = null;
        private RelayCommand<object> mToggleIsFolderVisibleCommand = null;
        private RelayCommand<object> mToggleIsIconVisibleCommand = null;
        private RelayCommand<object> mToggleIsHiddenVisibleCommand = null;
        private RelayCommand<object> mToggleIsFilteredCommand = null;

        private RelayCommand<object> mRecentFolderRemoveCommand = null;
        private RelayCommand<object> mRecentFolderAddCommand = null;

        private RelayCommand<object> mOpenContainingFolderCommand = null;
        private RelayCommand<object> mOpenInWindowsCommand = null;
        private RelayCommand<object> mCopyPathCommand = null;

        private RelayCommand<object> mRenameCommand = null;
        private RelayCommand<object> mStartRenameCommand = null;
        private RelayCommand<object> mCreateFolderCommand = null;

        private SendNotificationViewModel mNotification;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public FileListViewModel(IBrowseNavigation browseNavigation)
          : this()
        {
            this.mBrowseNavigation = browseNavigation;

            this.mParsedFilter = BrowseNavigation.GetParsedFilters(this.mFilterString);
        }

        /// <summary>
        /// Standard class constructor
        /// </summary>
        protected FileListViewModel()
        {
            this.Notification = new SendNotificationViewModel();
            this.CurrentItems = new ObservableCollection<FSItemViewModel>();
        }
        #endregion constructor

        #region Events
        /// <summary>
        /// Event is fired to indicate that user wishes to open a file via this viewmodel.
        /// </summary>
        public event EventHandler<FileOpenEventArgs> OnFileOpen;

        /// <summary>
        /// Event is fired when user interaction in listview requires naviagtion to another location.
        /// </summary>
        public event EventHandler<FolderChangedEventArgs> RequestChangeOfDirectory;

        /// <summary>
        /// Generate an event to remove or add a recent folder to a collection.
        /// </summary>
        public event EventHandler<RecentFolderEvent> RequestEditRecentFolder;
        #endregion

        #region properties
        /// <summary>
        /// Gets/sets list of files and folders to be displayed in connected view.
        /// </summary>
        public ObservableCollection<FSItemViewModel> CurrentItems { get; set; }

        /// <summary>
        /// Get/set select item in filelist viemodel. This property is used to bind
        /// the selectitem of the listbox and enable the BringIntoView behaviour
        /// to scroll a selected item into view.
        /// </summary>
        public FSItemViewModel SelectedItem
        {
            get
            {
                return this.mSelectedItem;
            }

            set
            {
                Logger.DebugFormat("Set SelectedItem '{0}' property", value);

                if (this.mSelectedItem != value)
                {
                    this.mSelectedItem = value;
                    this.RaisePropertyChanged(() => this.SelectedItem);
                }
            }
        }

        /// <summary>
        /// Gets/sets whether the list of folders and files should include folders or not.
        /// </summary>
        public bool ShowFolders
        {
            get
            {
                return this.mShowFolders;
            }

            set
            {
                Logger.DebugFormat("Set ShowFolders '{0}' property", value);

                if (this.mShowFolders != value)
                {
                    this.mShowFolders = value;
                    this.RaisePropertyChanged(() => this.ShowFolders);
                }
            }
        }

        /// <summary>
        /// Gets/sets whether the list of folders and files includes hidden folders or files.
        /// </summary>
        public bool ShowHidden
        {
            get
            {
                return this.mShowHidden;
            }

            set
            {
                Logger.DebugFormat("Set ShowHidden '{0}' property", value);

                if (this.mShowHidden != value)
                {
                    this.mShowHidden = value;
                    this.RaisePropertyChanged(() => this.ShowHidden);
                }
            }
        }

        /// <summary>
        /// Gets/sets whether the list of folders and files includes an icon or not.
        /// </summary>
        public bool ShowIcons
        {
            get
            {
                return this.mShowIcons;
            }

            set
            {
                Logger.DebugFormat("Set ShowIcons '{0}' property", value);

                if (this.mShowIcons != value)
                {
                    this.mShowIcons = value;
                    this.RaisePropertyChanged(() => this.ShowIcons);
                }
            }
        }

        /// <summary>
        /// Gets whether the list of folders and files is filtered or not.
        /// </summary>
        public bool IsFiltered
        {
            get
            {
                return this.mIsFiltered;
            }

            private set
            {
                Logger.DebugFormat("Set IsFiltered '{0}' property", value);

                if (this.mIsFiltered != value)
                {
                    this.mIsFiltered = value;
                    this.RaisePropertyChanged(() => this.IsFiltered);
                }
            }
        }

        /// <summary>
        /// Gets the current path this viewmodel assigned to look at.
        /// This property is not updated (must be polled) so its not
        /// a good idea to bind to it.
        /// </summary>
        public string CurrentFolder
        {
            get
            {
                Logger.DebugFormat("get CurrentFolder property");

                if (this.mBrowseNavigation != null)
                {
                    if (this.mBrowseNavigation.CurrentFolder != null)
                        return this.mBrowseNavigation.CurrentFolder.Path;
                }

                return null;
            }
        }

        #region commands
        /// <summary>
        /// Navigates to a folder that was visited before navigating back (if any).
        /// </summary>
        public ICommand NavigateForwardCommand
        {
            get
            {
                if (this.mNavigateForwardCommand == null)
                    this.mNavigateForwardCommand = new RelayCommand<object>((p) =>
                    {
                        var newFolder = this.mBrowseNavigation.BrowseForward();

                        if (newFolder != null)
                        {
                            this.UpdateView(newFolder.Path);

                            if (this.RequestChangeOfDirectory != null)
                                this.RequestChangeOfDirectory(this, new FolderChangedEventArgs(newFolder));
                        }
                    },
                    (p) => this.mBrowseNavigation.CanBrowseForward());

                return this.mNavigateForwardCommand;
            }
        }

        /// <summary>
        /// Navigates back to a folder that was visited before the current folder (if any).
        /// </summary>
        public ICommand NavigateBackCommand
        {
            get
            {
                if (this.mNavigateBackCommand == null)
                    this.mNavigateBackCommand = new RelayCommand<object>((p) =>
                    {
                        var newFolder = this.mBrowseNavigation.BrowseBack();

                        if (newFolder != null)
                        {
                            this.UpdateView(newFolder.Path);

                            if (this.RequestChangeOfDirectory != null)
                                this.RequestChangeOfDirectory(this, new FolderChangedEventArgs(newFolder));
                        }
                    },
                    (p) => this.mBrowseNavigation.CanBrowseBack());

                return this.mNavigateBackCommand;
            }
        }

        /// <summary>
        /// Browse into the parent folder path of a given path.
        /// </summary>
        public ICommand NavigateUpCommand
        {
            get
            {
                if (this.mNavigateUpCommand == null)
                    this.mNavigateUpCommand = new RelayCommand<object>((p) =>
                    {
                        var newFolder = this.mBrowseNavigation.BrowseUp();

                        if (newFolder != null)
                        {
                            if (newFolder.DirectoryPathExists() == false)
                                return;

                            this.UpdateView(newFolder.Path);

                            if (this.RequestChangeOfDirectory != null)
                                this.RequestChangeOfDirectory(this, new FolderChangedEventArgs(newFolder));
                        }
                    },
                    (p) => this.mBrowseNavigation.CanBrowseUp());

                return this.mNavigateUpCommand;
            }
        }

        /// <summary>
        /// Browse into a given a path.
        /// </summary>
        public ICommand NavigateDownCommand
        {
            get
            {
                if (this.mNavigateDownCommand == null)
                    this.mNavigateDownCommand = new RelayCommand<object>((p) =>
                    {
                        var info = p as FSItemViewModel;

                        if (info == null)
                            return;

                        FSItemType t = this.mBrowseNavigation.BrowseDown(info.Type, info.FullPath);

                        this.PopulateView();

                        if (this.RequestChangeOfDirectory != null && t == FSItemType.Folder)
                            this.RequestChangeOfDirectory(this, new FolderChangedEventArgs(info.GetModel));
                        else
                        {
                            if (this.OnFileOpen != null && t == FSItemType.File)
                                this.OnFileOpen(this, new FileOpenEventArgs() { FileName = info.FullPath });
                        }
                    },
                    (p) =>
                    {
                        return (p as FSItemViewModel) != null;
                    });

                return this.mNavigateDownCommand;
            }
        }

        /// <summary>
        /// Gets the command that updates the currently viewed
        /// list of directory items (files and sub-directories).
        /// </summary>
        public ICommand RefreshCommand
        {
            get
            {
                if (this.mRefreshCommand == null)
                    this.mRefreshCommand = new RelayCommand<object>((p) => this.PopulateView());

                return this.mRefreshCommand;
            }
        }

        /// <summary>
        /// Toggles the visibiliy of folders in the folder/files listview.
        /// </summary>
        public ICommand ToggleIsFolderVisibleCommand
        {
            get
            {
                if (this.mToggleIsFolderVisibleCommand == null)
                    this.mToggleIsFolderVisibleCommand = new RelayCommand<object>((p) => this.ToggleIsFolderVisible_Executed());

                return this.mToggleIsFolderVisibleCommand;
            }
        }

        /// <summary>
        /// Toggles the visibiliy of icons in the folder/files listview.
        /// </summary>
        public ICommand ToggleIsIconVisibleCommand
        {
            get
            {
                if (this.mToggleIsIconVisibleCommand == null)
                    this.mToggleIsIconVisibleCommand = new RelayCommand<object>((p) => this.ToggleIsIconVisible_Executed());

                return this.mToggleIsIconVisibleCommand;
            }
        }

        /// <summary>
        /// Toggles the visibiliy of hidden files/folders in the folder/files listview.
        /// </summary>
        public ICommand ToggleIsHiddenVisibleCommand
        {
            get
            {
                if (this.mToggleIsHiddenVisibleCommand == null)
                    this.mToggleIsHiddenVisibleCommand = new RelayCommand<object>((p) => this.ToggleIsHiddenVisible_Executed());

                return this.mToggleIsHiddenVisibleCommand;
            }
        }

        /// <summary>
        /// Implements a command that adds a removes a folder location.
        /// Expected parameter is of type <seealso cref="FSItemViewModel"/>.
        /// </summary>
        public ICommand RecentFolderRemoveCommand
        {
            get
            {
                if (this.mRecentFolderRemoveCommand == null)
                    this.mRecentFolderRemoveCommand = new RelayCommand<object>((p) => this.RecentFolderRemove_Executed(p));

                return this.mRecentFolderRemoveCommand;
            }
        }

        /// <summary>
        /// Implements a command that adds a recent folder location.
        /// Expected parameter is of type <seealso cref="FSItemViewModel"/>.
        /// </summary>
        public ICommand RecentFolderAddCommand
        {
            get
            {
                if (this.mRecentFolderAddCommand == null)
                    this.mRecentFolderAddCommand = new RelayCommand<object>((p) => this.RecentFolderAdd_Executed(p));

                return this.mRecentFolderAddCommand;
            }
        }

        #region Windows Integration FileSystem Commands
        /// <summary>
        /// Gets a command that will open the folder in which an item is stored.
        /// The item (path to a file) is expected as <seealso cref="FSItemViewModel"/> parameter.
        /// </summary>
        public ICommand OpenContainingFolderCommand
        {
            get
            {
                if (this.mOpenContainingFolderCommand == null)
                    this.mOpenContainingFolderCommand = new RelayCommand<object>(
                      (p) =>
                      {
                          var path = p as FSItemViewModel;

                          if (path == null)
                              return;

                          if (string.IsNullOrEmpty(path.FullPath) == true)
                              return;

                          FileListViewModel.OpenContainingFolderCommand_Executed(path.FullPath);
                      });

                return this.mOpenContainingFolderCommand;
            }
        }

        /// <summary>
        /// Gets a command that will open the selected item with the current default application
        /// in Windows. The selected item (path to a file) is expected as <seealso cref="FSItemViewModel"/> parameter.
        /// (eg: Item is HTML file -> Open in Windows starts the web browser for viewing the HTML
        /// file if thats the currently associated Windows default application.
        /// </summary>
        public ICommand OpenInWindowsCommand
        {
            get
            {
                if (this.mOpenInWindowsCommand == null)
                    this.mOpenInWindowsCommand = new RelayCommand<object>(
                      (p) =>
                      {
                          var path = p as FSItemViewModel;

                          if (path == null)
                              return;

                          if (string.IsNullOrEmpty(path.FullPath) == true)
                              return;

                          FileListViewModel.OpenInWindowsCommand_Executed(path.FullPath);
                      });

                return this.mOpenInWindowsCommand;
            }
        }

        /// <summary>
        /// Gets a command that will copy the path of an item into the Windows Clipboard.
        /// The item (path to a file) is expected as <seealso cref="FSItemViewModel"/> parameter.
        /// </summary>
        public ICommand CopyPathCommand
        {
            get
            {
                if (this.mCopyPathCommand == null)
                    this.mCopyPathCommand = new RelayCommand<object>(
                      (p) =>
                      {
                          var path = p as FSItemViewModel;

                          if (path == null)
                              return;

                          if (string.IsNullOrEmpty(path.FullPath) == true)
                              return;

                          FileListViewModel.CopyPathCommand_Executed(path.FullPath);
                      });

                return this.mCopyPathCommand;
            }
        }

        /// <summary>
        /// Toggles whether a file filter is currently applied on a list
        /// of files or not.
        /// </summary>
        public ICommand ToggleIsFilteredCommand
        {
            get
            {
                if (this.mToggleIsFilteredCommand == null)
                    this.mToggleIsFilteredCommand = new RelayCommand<object>(
                      (p) =>
                      {
                          this.SetIsFiltered(!this.IsFiltered);
                      });

                return this.mToggleIsFilteredCommand;
            }
        }
        #endregion Windows Integration FileSystem Commands

        /// <summary>
        /// Renames the folder that is represented by this viewmodel.
        /// This command should be called directly by the implementing view
        /// since the new name of the folder is delivered as string.
        /// </summary>
        public ICommand RenameCommand
        {
            get
            {
                if (this.mRenameCommand == null)
                    this.mRenameCommand = new RelayCommand<object>(it =>
                    {
                        var tuple = it as Tuple<string, object>;

                        if (tuple != null)
                        {
                            var folderVM = tuple.Item2 as FSItemViewModel;

                            if (tuple.Item1 != null && folderVM != null)
                                folderVM.RenameFileOrFolder(tuple.Item1);
                        }
                    });

                return this.mRenameCommand;
            }
        }

        /// <summary>
        /// Starts the rename folder process by that renames the folder
        /// that is represented by this viewmodel.
        /// 
        /// This command implements an event that triggers the actual rename
        /// process in the connected view.
        /// </summary>
        public ICommand StartRenameCommand
        {
            get
            {
                if (this.mStartRenameCommand == null)
                    this.mStartRenameCommand = new RelayCommand<object>(it =>
                    {
                        var folder = it as FSItemViewModel;

                        if (folder != null)
                            folder.RequestEditMode(InplaceEditBoxLib.Events.RequestEditEvent.StartEditMode);
                    });

                return this.mStartRenameCommand;
            }
        }

        /// <summary>
        /// Starts the create folder process by creating a new folder
        /// in the given location. The location is supplied as <seealso cref="System.Windows.Input.ICommandSource.CommandParameter"/>
        /// which is a <seealso cref="IFolderViewModel"/> item. So, the <seealso cref="IFolderViewModel"/> item
        /// is the parent of the new folder and the new folder is created with a standard name:
        /// 'New Folder n'. The new folder n is selected and in rename mode such that users can edit
        /// the name of the new folder right away.
        /// 
        /// This command implements an event that triggers the actual rename
        /// process in the connected view.
        /// </summary>
        public ICommand CreateFolderCommand
        {
            get
            {
                if (this.mCreateFolderCommand == null)
                    this.mCreateFolderCommand = new RelayCommand<object>(it =>
                    {
                        var folder = it as string;
                        this.CreateFolderCommandNewFolder(folder);
                    });

                return this.mCreateFolderCommand;
            }
        }
        #endregion commands

        /// <summary>
        /// Gets a property that can be bound to the Notification dependency property
        /// of the <seealso cref="UserNotification.View.NotifyableContentControl"/>.
        /// Application developers can invoke the ShowNotification method to show a
        /// short pop-up message to the user. The pop-up message is shown in the
        /// vicinity of the content control that contains the real control (eg: ListBox)
        /// to which this notfication is related to.
        /// </summary>
        public SendNotificationViewModel Notification
        {
            get
            {
                return mNotification;
            }

            set
            {
                Logger.DebugFormat("Set Notification '{0}' property", value);

                if (this.mNotification != value)
                {
                    this.mNotification = value;
                    this.RaisePropertyChanged(() => this.Notification);
                }
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Updates the current display with the given filter string.
        /// </summary>
        /// <param name="p"></param>
        public void UpdateView(string p)
        {
            Logger.DebugFormat("UpdateView method with '{0}' property", p);

            if (string.IsNullOrEmpty(p) == true)
                return;

            this.mBrowseNavigation.SetCurrentFolder(p, false);
            this.PopulateView();
        }

        /// <summary>
        /// Fills the CurrentItems property for display in ItemsControl
        /// </summary>
        public void NavigateToThisFolder(string sFolder)
        {
            Logger.DebugFormat("NavigateToThisFolder method with '{0}'", sFolder);

            this.mBrowseNavigation.BrowseDown(FSItemType.Folder, sFolder);

            ////this.RecentFolders.Push(this.CurrentFolder);
            this.UpdateView(sFolder);
            this.RaisePropertyChanged(() => this.CurrentFolder);
        }

        /// <summary>
        /// Applies a filter string (which can contain multiple
        /// alternative regular expression filter items) and updates
        /// the current display.
        /// </summary>
        /// <param name="filterText"></param>
        public void ApplyFilter(string filterText)
        {
            Logger.DebugFormat("ApplyFilter method with '{0}'", filterText);

            this.mFilterString = filterText;

            string[] tempParsedFilter = BrowseNavigation.GetParsedFilters(this.mFilterString);

            // Optimize nultiple requests for populating same view with unchanged filter away
            if (tempParsedFilter != this.mParsedFilter)
            {
                this.mParsedFilter = tempParsedFilter;
                this.PopulateView();
            }
        }

        /// <summary>
        /// Call this method to determine whether folders are part of the list of
        /// files and folders or not (list only files without folders).
        /// </summary>
        /// <param name="isFolderVisible"></param>
        public void SetIsFolderVisible(bool isFolderVisible)
        {
            Logger.DebugFormat("SetIsFolderVisible method with '{0}'", isFolderVisible);

            this.ShowFolders = isFolderVisible;
            this.PopulateView();
        }

        /// <summary>
        /// Call this method to determine whether folders are part of the list of
        /// files and folders or not (list only files without folders).
        /// </summary>
        /// <param name="isFiltered"></param>
        public void SetIsFiltered(bool isFiltered)
        {
            Logger.DebugFormat("SetIsFiltered method with '{0}'", isFiltered);

            this.IsFiltered = isFiltered;
            this.PopulateView();
        }

        /// <summary>
        /// Fills the CurrentItems property for display in ItemsControl
        /// based view (ListBox, ListView etc.).
        /// 
        /// This method wraps a parameterized version of the same method 
        /// with a call that contains the standard data field.
        /// </summary>
        protected void PopulateView()
        {
            Logger.DebugFormat("PopulateView method");

            this.PopulateView(this.mParsedFilter);
            this.RaisePropertyChanged(() => this.CurrentFolder);
        }

        #region FileSystem Commands
        /// <summary>
        /// Convinience method to open Windows Explorer with a selected file (if it exists).
        /// Otherwise, Windows Explorer is opened in the location where the file should be at.
        /// </summary>
        /// <param name="sFileName"></param>
        /// <returns></returns>
        private static bool OpenContainingFolderCommand_Executed(string sFileName)
        {
            if (string.IsNullOrEmpty(sFileName) == true)
                return false;

            // XXX TODO Add this back in
            //var msg = ServiceLocator.ServiceContainer.Instance.GetService<IMessageBoxService>();
            try
            {
                if (System.IO.File.Exists(sFileName) == true)
                {
                    // combine the arguments together it doesn't matter if there is a space after ','
                    string argument = @"/select, " + sFileName;

                    System.Diagnostics.Process.Start("explorer.exe", argument);
                    return true;
                }
                else
                {
                    string sParentDir = string.Empty;

                    if (System.IO.Directory.Exists(sFileName) == true)
                        sParentDir = sFileName;
                    else
                        sParentDir = System.IO.Directory.GetParent(sFileName).FullName;

                    if (System.IO.Directory.Exists(sParentDir) == false)
                    {
////                        msg.Show(string.Format(FileSystemModels.Local.Strings.STR_MSG_DIRECTORY_DOES_NOT_EXIST, sParentDir),
////                                               FileSystemModels.Local.Strings.STR_MSG_ERROR_FINDING_RESOURCE,
////                                               MsgBoxButtons.OK, MsgBoxImage.Error);

                        return false;
                    }
                    else
                    {
                        // combine the arguments together it doesn't matter if there is a space after ','
                        string argument = @"/select, " + sParentDir;

                        System.Diagnostics.Process.Start("explorer.exe", argument);

                        return true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex);
////                msg.Show(string.Format("{0}\n'{1}'.", ex.Message, (sFileName == null ? string.Empty : sFileName)),
////                          FileSystemModels.Local.Strings.STR_MSG_ERROR_FINDING_RESOURCE,
////                          MsgBoxButtons.OK, MsgBoxImage.Error);

                return false;
            }
        }

        /// <summary>
        /// Process command when a hyperlink has been clicked.
        /// Start a web browser and let it browse to where this points to...
        /// </summary>
        /// <param name="sFileName"></param>
        private static void OpenInWindowsCommand_Executed(string sFileName)
        {
            if (string.IsNullOrEmpty(sFileName) == true)
                return;

            // XXX TODO Add this back in
            //var msg = ServiceLocator.ServiceContainer.Instance.GetService<IMessageBoxService>();
            try
            {
                Process.Start(new ProcessStartInfo(sFileName));
                ////OpenFileLocationInWindowsExplorer(whLink.NavigateUri.OriginalString);
            }
            catch (System.Exception ex)
            {
////                msg.Show(string.Format(CultureInfo.CurrentCulture, "{0}", ex.Message),
////                         FileSystemModels.Local.Strings.STR_MSG_ERROR_FINDING_RESOURCE,
////                         MsgBoxButtons.OK, MsgBoxImage.Error);
            }
        }

        /// <summary>
        /// A hyperlink has been clicked. Start a web browser and let it browse to where this points to...
        /// </summary>
        /// <param name="sFileName"></param>
        private static void CopyPathCommand_Executed(string sFileName)
        {
            if (string.IsNullOrEmpty(sFileName) == true)
                return;

            try
            {
                System.Windows.Clipboard.SetText(sFileName);
            }
            catch
            {
            }
        }
        #endregion FileSystem Commands

        /// <summary>
        /// Fills the CurrentItems property for display in ItemsControl
        /// based view (ListBox, ListView etc.)
        /// 
        /// This version is parameterized since the filterstring can be parsed
        /// seperately and does not need to b parsed each time when this method
        /// executes.
        /// </summary>
        private void PopulateView(string[] filterString)
        {
            Logger.DebugFormat("PopulateView method with filterString parameter");

            this.CurrentItems.Clear();

            if (this.mBrowseNavigation.IsCurrentPathDirectory() == false)
                return;

            try
            {
                DirectoryInfo cur = this.mBrowseNavigation.GetDirectoryInfoOnCurrentFolder();
                ImageSource dummy = new BitmapImage();

                // Retrieve and add (filtered) list of directories
                if (this.ShowFolders)
                {
                    string[] directoryFilter = null;

                    //// if (filterString != null)
                    ////  directoryFilter = new ArrayList(filterString).ToArray() as string[];
                    directoryFilter = null;

                    foreach (DirectoryInfo dir in cur.SelectDirectoriesByFilter(directoryFilter))
                    {
                        if (dir.Attributes.HasFlag(FileAttributes.Hidden) == true)
                        {
                            if (this.ShowHidden == false)
                            {
                                if ((dir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                                    continue;
                            }
                        }

                        FSItemViewModel info = new FSItemViewModel(dir.FullName, FSItemType.Folder, dir.Name);

                        // to prevent the icon from being loaded from file later
                        if (this.ShowIcons == false)
                            info.SetDisplayIcon(dummy);

                        this.CurrentItems.Add(info);
                    }
                }

                if (this.IsFiltered == false) // Do not apply the filter if it is not enabled
                    filterString = null;

                // Retrieve and add (filtered) list of files in current directory
                foreach (FileInfo f in cur.SelectFilesByFilter(filterString))
                {
                    if (this.ShowHidden == false)
                    {
                        if (f.Attributes.HasFlag(FileAttributes.Hidden) == true)
                        {
                            if ((f.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                                continue;
                        }
                    }

                    FSItemViewModel info = new FSItemViewModel(f.FullName, FSItemType.File, f.Name);

                    if (this.ShowIcons == false)
                        info.SetDisplayIcon(dummy);  // to prevent the icon from being loaded from file later

                    this.CurrentItems.Add(info);
                }
            }
            catch
            {
            }

            // reset column width manually (otherwise it is not updated)
            ////this.mFileListView.TheGVColumn.Width = this.mFileListView.TheGVColumn.ActualWidth;
            ////this.mFileListView.TheGVColumn.Width = double.NaN;
        }

        private void ToggleIsFolderVisible_Executed()
        {
            this.ShowFolders = !this.ShowFolders;
            this.PopulateView();
        }

        private void ToggleIsIconVisible_Executed()
        {
            this.ShowIcons = !this.ShowIcons;
            this.PopulateView();
        }

        private void ToggleIsHiddenVisible_Executed()
        {
            this.ShowHidden = !this.ShowHidden;
            this.PopulateView();
        }

        private void RecentFolderRemove_Executed(object param)
        {
            var item = param as FSItemViewModel;

            if (item == null)
                return;

            if (this.RequestEditRecentFolder != null)
                this.RequestEditRecentFolder(this, new RecentFolderEvent(item.GetModel,
                                                                         RecentFolderEvent.RecentFolderAction.Remove));
        }

        private void RecentFolderAdd_Executed(object param)
        {
            var item = param as FSItemViewModel;

            if (item == null)
                return;

            if (this.RequestEditRecentFolder != null)
                this.RequestEditRecentFolder(this, new RecentFolderEvent(item.GetModel,
                                                                         RecentFolderEvent.RecentFolderAction.Add));
        }

        /// <summary>
        /// Create a new folder underneath the given parent folder. This method creates
        /// the folder with a standard name (eg 'New folder n') on disk and selects it
        /// in editing mode to give users a chance for renaming it right away.
        /// </summary>
        /// <param name="parentFolder"></param>
        private void CreateFolderCommandNewFolder(string parentFolder)
        {
            Logger.DebugFormat("CreateFolderCommandNewFolder method with '{0}'", parentFolder);

            if (parentFolder == null)
                return;

            FSItemViewModel newSubFolder = this.CreateNewDirectory(parentFolder);

            if (newSubFolder != null)
            {
                this.SelectedItem = newSubFolder;

                // Do this with low priority (thanks for that tip to Joseph Leung)
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, (Action)delegate
                {
                    newSubFolder.RequestEditMode(InplaceEditBoxLib.Events.RequestEditEvent.StartEditMode);
                });
            }
        }

        /// <summary>
        /// Creates a new folder with a standard name (eg: 'New folder n').
        /// </summary>
        /// <returns></returns>
        private FSItemViewModel CreateNewDirectory(string parentFolder)
        {
            Logger.DebugFormat("CreateNewDirectory method with '{0}'", parentFolder);

            try
            {
                var newSubFolder = PathModel.CreateDir(new PathModel(parentFolder, FSItemType.Folder));

                if (newSubFolder != null)
                {
                    var newFolderVM = new FSItemViewModel(newSubFolder.Path, newSubFolder.PathType, newSubFolder.Name);

                    this.CurrentItems.Add(newFolderVM);

                    return newFolderVM;
                }
            }
            catch (Exception exp)
            {
                Logger.Error(string.Format("Creating new folder underneath '{0}' was not succesful.", parentFolder), exp);
                this.Notification.ShowNotification(FileSystemModels.Local.Strings.STR_CREATE_FOLDER_ERROR_TITLE,
                                                   exp.Message);
            }

            return null;
        }
        #endregion methods
    }
}
