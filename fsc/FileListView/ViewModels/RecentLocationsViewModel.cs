namespace FileListView.ViewModels
{
  using System;
  using System.Collections.ObjectModel;
  using System.Linq;
  using System.Windows.Input;
  using FileListView.Command;
  using FileSystemModels.Events;
  using FileSystemModels.Models;
  using FileSystemModels.Utils;

  /// <summary>
  /// Implement viewmodel for management of recent folder locations.
  /// </summary>
  public class RecentLocationsViewModel : Base.ViewModelBase
  {
    #region fields
    private FSItemViewModel mSelectedItem;

    private object mLockObject = new object();

    private RelayCommand<object> mChangeOfDirectoryCommand;
    private RelayCommand<object> mRemoveFolderBookmark;
    private bool mIsOpen;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    public RecentLocationsViewModel()
    {
      this.DropDownItems = new ObservableCollection<FSItemViewModel>();
      this.IsOpen = false;
    }
    #endregion constructor

    #region events
    /// <summary>
    /// Event is fired whenever the path in the text portion of the combobox is changed.
    /// </summary>
    public event EventHandler<FolderChangedEventArgs> RequestChangeOfDirectory;
    #endregion events

    #region properties
    /// <summary>
    /// <inheritedoc />
    /// </summary>
    public ICommand ChangeOfDirectoryCommand
    {
      get
      {
        if (this.mChangeOfDirectoryCommand == null)
          this.mChangeOfDirectoryCommand = new RelayCommand<object>((p) =>
          {
            var param = p as FSItemViewModel;

            if (param != null)
              this.ChangeOfDirectoryCommand_Executed(param);
          });

        return this.mChangeOfDirectoryCommand;
      }
    }

    /// <summary>
    /// Command removes a folder bookmark from the list of
    /// currently bookmarked folders. Required command parameter
    /// is of type <seealso cref="FSItemViewModel"/>.
    /// </summary>
    public ICommand RemoveFolderBookmark
    {
      get
      {
        if (this.mRemoveFolderBookmark == null)
          this.mRemoveFolderBookmark = new RelayCommand<object>((p) =>
          {
            var param = p as FSItemViewModel;

            if (param != null)
              this.RemoveFolderBookmark_Executed(param);
          });

        return this.mRemoveFolderBookmark;
      }
    }

    /// <summary>
    /// <inheritedoc />
    /// </summary>
    public ObservableCollection<FSItemViewModel> DropDownItems { get; private set; }

    /// <summary>
    /// Gets/set the selected item of the RecentLocations property.
    /// 
    /// This should be bound by the view (ItemsControl) to find the SelectedItem here.
    /// </summary>
    public FSItemViewModel SelectedItem
    {
      get
      {
        return this.mSelectedItem;
      }

      set
      {
        if (this.mSelectedItem != value)
        {
          this.mSelectedItem = value;
          this.RaisePropertyChanged(() => this.SelectedItem);
        }
      }
    }

    /// <summary>
    /// <inheritedoc />
    /// </summary>
    public bool IsOpen
    {
      get
      {
        return this.mIsOpen;
      }

      set
      {
        if (this.mIsOpen != value)
        {
          this.mIsOpen = value;
          this.RaisePropertyChanged(() => this.IsOpen);
        }
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Add a recent folder location into the collection of recent folders.
    /// This collection can then be used in the folder combobox drop down
    /// list to store user specific customized folder short-cuts.
    /// </summary>
    /// <param name="folderPath"></param>
    /// <param name="selectNewItem"></param>
    internal void AddRecentFolder(string folderPath,
                                  bool selectNewItem = false)
    {
      lock (this.mLockObject)
      {
        if ((folderPath = PathModel.ExtractDirectoryRoot(folderPath)) == null)
          return;

        // select this path if its already there
        var results = this.DropDownItems.Where<FSItemViewModel>(folder => string.Compare(folder.FullPath, folderPath, true) == 0);

        // Do not add this twice
        if (results != null)
        {
          if (results.Count() != 0)
          {
            if (selectNewItem == true)
              this.SelectedItem = results.First();

            return;
          }
        }

        var folderVM = this.CreateFSItemVMFromString(folderPath);

        this.DropDownItems.Add(folderVM);

        if (selectNewItem == true)
          this.SelectedItem = folderVM;
      }
    }

    /// <summary>
    /// Remove a recent folder location from the collection of recent folders.
    /// This collection can then be used in the folder combobox drop down
    /// list to store user specific customized folder short-cuts.
    /// </summary>
    /// <param name="folderPath"></param>
    internal void RemoveRecentFolder(PathModel folderPath)
    {
      lock (this.mLockObject)
      {
        if (folderPath == null)
          return;

        // Find all items that satisfy the query match and remove them
        // (This statement requires a Linq extension method to work)
        this.DropDownItems.Remove(i => string.Compare(folderPath.Path, i.FullPath, true) == 0);
      }
    }

    internal void ClearRecentFolderCollection()
    {
      if (this.DropDownItems != null)
        this.DropDownItems.Clear();
    }

    internal void RemoveRecentFolder(string path)
    {
      try
      {
        this.RemoveRecentFolder(new PathModel(path, FSItemType.Folder));
      }
      catch
      {
      }
    }

    private void ChangeOfDirectoryCommand_Executed(FSItemViewModel path)
    {
      if (path == null)
        return;

      this.IsOpen = false;
      this.SelectedItem = path;

      if (this.RequestChangeOfDirectory != null)
        this.RequestChangeOfDirectory(this, new FolderChangedEventArgs(new PathModel(path.FullPath, FSItemType.Folder)));
    }

    private FSItemViewModel CreateFSItemVMFromString(string folderPath)
    {
      ////folderPath = System.IO.Path.GetDirectoryName(folderPath);

      string displayName = string.Empty;

      try
      {
        displayName = System.IO.Path.GetFileName(folderPath);
      }
      catch
      {
      }

      if (displayName.Trim() == string.Empty)
        displayName = folderPath;

      return new FSItemViewModel(folderPath, FSItemType.Folder, displayName, true);
    }

    /// <summary>
    /// Method removes a folder bookmark from the list of currently bookmarked folders.
    /// </summary>
    /// <param name="param"></param>
    private void RemoveFolderBookmark_Executed(FSItemViewModel param)
    {
      this.RemoveRecentFolder(param.GetModel);
    }
    #endregion methods
  }
}
