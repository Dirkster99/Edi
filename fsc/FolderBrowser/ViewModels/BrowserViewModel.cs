namespace FolderBrowser.ViewModels
{
  using System;
  using System.Collections.ObjectModel;
  using System.Linq;
  using System.Windows.Input;
  using System.Windows.Threading;
  using FileSystemModels.Events;
  using FileSystemModels.Models;
  using FolderBrowser.Command;
  using FolderBrowser.ViewModels.Interfaces;

  /// <summary>
  /// Class implements a viewmodel for browsing folder structures in the file system.
  /// 
  /// Source: http://www.codeproject.com/Articles/352874/WPF-Folder-Browser
  /// (but with RelayCommand instead of PRISM).
  /// </summary>
  public class BrowserViewModel : Base.ViewModelBase, IBrowserViewModel
  {
    #region fields
    /// <summary>
    /// Log4net logger facility for this class.
    /// </summary>
    protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private string mSelectedFolder;
    private bool mExpanding = false;

    private RelayCommand<object> mFolderSelectedCommand = null;
    private RelayCommand<object> mFinalSelectDirectoryCommand = null;

    private RelayCommand<object> mRecentFolderRemoveCommand = null;
    private RelayCommand<object> mRecentFolderAddCommand = null;

    private RelayCommand<object> mRenameCommand = null;
    private RelayCommand<object> mStartRenameCommand = null;
    private RelayCommand<object> mCreateFolderCommand = null;

    private object mLockObject = new object();

    private bool mIsSpecialFoldersVisisble;
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard <seealso cref="BrowserViewModel"/> constructor
    /// </summary>
    public BrowserViewModel()
    {
      this.IsSpecialFoldersVisisble = true;

      this.Folders = new ObservableCollection<IFolderViewModel>();
      Environment.GetLogicalDrives().ToList().ForEach(it =>
      {
        try
        {
          string driveLetter = it.TrimEnd('\\');

          if (string.IsNullOrEmpty(driveLetter) == false)
            this.Folders.Add(FolderViewModel.ConstructDriveFolderViewModel(it));
        }
        catch
        {
        }
      });

      this.SpecialFolders = new ObservableCollection<CustomFolderItemViewModel>();

      this.SpecialFolders.Add(new CustomFolderItemViewModel(Environment.SpecialFolder.Desktop));
      this.SpecialFolders.Add(new CustomFolderItemViewModel(Environment.SpecialFolder.MyDocuments));
      this.SpecialFolders.Add(new CustomFolderItemViewModel(Environment.SpecialFolder.MyMusic));
      this.SpecialFolders.Add(new CustomFolderItemViewModel(Environment.SpecialFolder.MyVideos));
    }
    #endregion constructor
    
    #region events
    /// <summary>
    /// Event is fired to indicate that user wishes to select a certain Path.
    /// </summary>
    public event EventHandler<FolderChangedEventArgs> FinalPathSelectionEvent;

    /// <summary>
    /// This event is triggered when the currently selected folder has changed.
    /// </summary>
    public event EventHandler<FolderChangedEventArgs> FolderSelectionChangedEvent;

    /// <summary>
    /// Generate an event to remove or add a recent folder to a collection.
    /// </summary>
    public event EventHandler<RecentFolderEvent> RequestEditBookmarkedFolders;
    #endregion events

    #region properties
    /// <summary>
    /// Get/set currently selected folder.
    /// </summary>
    public string SelectedFolder
    {
      get
      {
        return this.mSelectedFolder;
      }
      
      set
      {
        Logger.DebugFormat("set SelectedFolder property");

        lock (this.mLockObject)
        {
          if (this.mSelectedFolder != value)
          {
            this.mSelectedFolder = value;
            this.RaisePropertyChanged(() => this.SelectedFolder);
          }
        }
      }
    }

    /// <summary>
    /// Gets the list of drives and folders for display in treeview structure control.
    /// </summary>
    public ObservableCollection<IFolderViewModel> Folders
    {
      get; private set;
    }

    /// <summary>
    /// Get/set command to select the current folder.
    /// </summary>
    public ICommand FolderSelectedCommand
    {
      get
      {
        Logger.DebugFormat("get FolderSelectedCommand property");

        if (this.mFolderSelectedCommand == null)
        {
          this.mFolderSelectedCommand = new RelayCommand<object>(p =>
          {
            try
            {
              string path = p as string;

              if (string.IsNullOrEmpty(path) == true)
              {
                var item = p as IFolderViewModel;

                if (item != null)
                  path = item.FolderPath;

                if (string.IsNullOrEmpty(path) == true)
                  return;
              }

              // Set new path if it changed in comparison to old path
              this.SetSelectedFolder(PathModel.NormalizePath(path), true);
            }
            catch
            {
            }
          });
        }

        return this.mFolderSelectedCommand;
      }
    }

    /// <summary>
    /// This command can be used to do a final select of a particular folder and tell
    /// the consumer of this viewmodel that the user wants to select this folder.
    /// The consumer can then diactivate the dialog or browse to the requested location
    /// using whatever is required outside of this control....
    /// </summary>
    public ICommand FinalSelectDirectoryCommand
    {
      get
      {
        Logger.DebugFormat("get FinalSelectDirectoryCommand property");

        if (this.mFinalSelectDirectoryCommand == null)
          this.mFinalSelectDirectoryCommand = new RelayCommand<object>(it =>
          {
            var path = it as IFolderViewModel;

            if (path != null)
            {
              this.SetSelectedFolder(path.FolderPath, true);

              if (this.FinalPathSelectionEvent != null)
                this.FinalPathSelectionEvent(this, new FolderChangedEventArgs(new PathModel(path.FolderPath, FSItemType.Folder)));
            }
          });

        return this.mFinalSelectDirectoryCommand;
      }
    }

    /// <summary>
    /// Renames the folder that is represented by this viewmodel.
    /// This command should be called directly by the implementing view
    /// since the new name of the folder is delivered as string.
    /// </summary>
    public ICommand RenameCommand
    {
      get
      {
        Logger.DebugFormat("get RenameCommand property");

        if (this.mRenameCommand == null)
          this.mRenameCommand = new RelayCommand<object>(it =>
          {
            var tuple = it as Tuple<string, object>;

            if (tuple != null)
            {
              var folderVM = tuple.Item2 as IFolderViewModel;

              if (tuple.Item1 != null && folderVM != null)
              {
                folderVM.RenameFolder(tuple.Item1);
                this.SelectedFolder = folderVM.FolderPath;
              }
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
        Logger.DebugFormat("get StartRenameCommand property");

        if (this.mStartRenameCommand == null)
          this.mStartRenameCommand = new RelayCommand<object>(it =>
          {
            var folder = it as IFolderViewModel;

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
        Logger.DebugFormat("get CreateFolderCommand property");

        if (this.mCreateFolderCommand == null)
          this.mCreateFolderCommand = new RelayCommand<object>(it =>
          {
            var folder = it as IFolderViewModel;
            this.CreateFolderCommandNewFolder(folder);
          });

        return this.mCreateFolderCommand;
      }
    }

    #region Add Remove Recent Folder commands
    /// <summary>
    /// Determine whether an Add/Remove command can execute or not.
    /// </summary>
    public bool RecentFolderCommandCanExecute
    {
      get
      {
        if (this.RequestEditBookmarkedFolders != null)
          return true;

        return false;
      }
    }

    /// <summary>
    /// Implements a command that adds a removes a folder location.
    /// Expected parameter is of type FSItemVM.
    /// </summary>
    public ICommand RecentFolderRemoveCommand
    {
      get
      {
        if (this.mRecentFolderRemoveCommand == null)
          this.mRecentFolderRemoveCommand = new RelayCommand<object>((p) => this.RecentFolderRemove_Executed(p),
                                                                     (p) => this.RecentFolderCommand_CanExecute(p));

        return this.mRecentFolderRemoveCommand;
      }
    }

    /// <summary>
    /// Implements a command that adds a recent folder location.
    /// Expected parameter is of type FSItemVM.
    /// </summary>
    public ICommand RecentFolderAddCommand
    {
      get
      {
        if (this.mRecentFolderAddCommand == null)
          this.mRecentFolderAddCommand = new RelayCommand<object>((p) => this.RecentFolderAdd_Executed(p),
                                                                  (p) => this.RecentFolderCommand_CanExecute(p));

        return this.mRecentFolderAddCommand;
      }
    }
    #endregion Add Remove Recent Folder commands

    #region SpecialFolders property
    /// <summary>
    /// Gets a list of Special Windows Standard folders for display in view.
    /// </summary>
    public ObservableCollection<CustomFolderItemViewModel> SpecialFolders { get; private set; }

    /// <summary>
    /// Gets whether the browser view should show a special folder control or not
    /// (A special folder control lets users browse to folders like 'My Documents'
    /// with a mouse click).
    /// </summary>
    public bool IsSpecialFoldersVisisble
    {
      get
      {
        return this.mIsSpecialFoldersVisisble;
      }

      private set
      {
        if (this.mIsSpecialFoldersVisisble != value)
        {
          this.mIsSpecialFoldersVisisble = value;
          this.RaisePropertyChanged(() => this.IsSpecialFoldersVisisble);
        }
      }
    }
    #endregion SpecialFolders property
    #endregion properties

    #region methods
    /// <summary>
    /// Determines whether the browser view should show a special folder control or not
    /// (A special folder control lets users browse to folders like 'My Documents'
    /// with a mouse click).
    /// </summary>
    /// <param name="visible"></param>
    public void SetSpecialFoldersVisibility(bool visible)
    {
      this.IsSpecialFoldersVisisble = visible;
    }

    /// <summary>
    /// Select the <paramref name="selectedFolder"/> in the current viewmodel
    /// to update display of folders in this regard.
    /// </summary>
    /// <param name="selectedFolder"></param>
    void IBrowserViewModel.SetSelectedFolder(string selectedFolder)
    {
      this.SetSelectedFolder(selectedFolder, false);
    }

    /// <summary>
    /// Get all child entries for a given path entry
    /// </summary>
    /// <param name="childFolders"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    private static IFolderViewModel Expand(ObservableCollection<IFolderViewModel> childFolders, string path)
    {
      Logger.DebugFormat("static Expand folder method for {0}", path);

      if (string.IsNullOrEmpty(path) || childFolders.Count == 0)
      {
        return null;
      }

      string folderName = path;
      if (path.Contains('/') || path.Contains('\\'))
      {
        int idx = path.IndexOfAny(new char[] { '/', '\\' });
        folderName = path.Substring(0, idx);
        path = path.Substring(idx + 1);
      }
      else
      {
        path = null;
      }

      // bugfix: Folder names on windows are case insensitiv
      var results = childFolders.Where<IFolderViewModel>(folder => string.Compare(folder.FolderName, folderName, true) == 0);
      if (results != null && results.Count() > 0)
      {
        IFolderViewModel fvm = results.First();
        fvm.IsExpanded = true;

        var retVal = BrowserViewModel.Expand(fvm.Folders, path);
        if (retVal != null)
        {
          return retVal;
        }
        else
        {
          return fvm;
        }
      }

      return null;
    }

    /// <summary>
    /// Assign the currently selected folder with this path.
    /// </summary>
    /// <param name="selectedFolder"></param>
    /// <param name="updateViews"></param>
    private void SetSelectedFolder(string selectedFolder, bool updateViews)
    {
      Logger.DebugFormat("SetSelectedFolder folder method for {0}", selectedFolder);

      this.SelectedFolder = PathModel.NormalizePath(selectedFolder);
      this.OnSelectedFolderChanged();

      if (updateViews == true)
      {
        // Send the event if there is a receiver
        if (this.FolderSelectionChangedEvent != null)
          this.FolderSelectionChangedEvent(this, new FolderChangedEventArgs(new PathModel(this.SelectedFolder, FSItemType.Folder)));
      }
    }

    private void OnSelectedFolderChanged()
    {
      Logger.DebugFormat("OnSelectedFolderChanged method");

      if (this.mExpanding == false)
      {
        try
        {
          this.mExpanding = true;
          IFolderViewModel child = BrowserViewModel.Expand(this.Folders, this.SelectedFolder);

          if (child != null)
            child.IsSelected = true;
        }
        finally
        {
          this.mExpanding = false;
        }
      }
    }

    /// <summary>
    /// Create a new folder underneath the given parent folder. This method creates
    /// the folder with a standard name (eg 'New folder n') on disk and selects it
    /// in editing mode to give users a chance for renaming it right away.
    /// </summary>
    /// <param name="parentFolder"></param>
    private void CreateFolderCommandNewFolder(IFolderViewModel parentFolder)
    {
      Logger.DebugFormat("CreateFolderCommandNewFolder method for '{0}'", parentFolder);

      if (parentFolder == null)
        return;

      IFolderViewModel newSubFolder = parentFolder.CreateNewDirectory();

      ////this.SelectedFolder = newSubFolder.FolderPath;
      ////this.SetSelectedFolder(newSubFolder.FolderPath, true);

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

    #region Add Remove Recent Folder commands
    private bool RecentFolderCommand_CanExecute(object param)
    {
      if (this.RequestEditBookmarkedFolders != null)
      {
        var item = param as FolderViewModel;
      
        if (item != null)
          return true;
      }

      return false;
    }

    private void RecentFolderRemove_Executed(object param)
    {
      var item = param as FolderViewModel;
      
      if (item == null)
        return;
      
      if (this.RequestEditBookmarkedFolders != null)
        this.RequestEditBookmarkedFolders(this, new RecentFolderEvent(new PathModel(item.FolderPath, FSItemType.Folder),
                                                                   RecentFolderEvent.RecentFolderAction.Remove));
    }

    private void RecentFolderAdd_Executed(object param)
    {
      var item = param as FolderViewModel;
      
      if (item == null)
        return;
      
      if (this.RequestEditBookmarkedFolders != null)
        this.RequestEditBookmarkedFolders(this, new RecentFolderEvent(new PathModel(item.FolderPath, FSItemType.Folder),
                                                                   RecentFolderEvent.RecentFolderAction.Add));
    }
    #endregion Add Remove Recent Folder commands
    #endregion methods
  }
}
