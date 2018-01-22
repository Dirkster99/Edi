namespace FileListView.ViewModels
{
  using System;
  using System.Collections.ObjectModel;
  using System.IO;
  using System.Windows.Input;
  using FileListView.Command;
  using FileSystemModels.Events;
  using FileSystemModels.Models;

  /// <summary>
  /// Class implements a viewmodel that can be used for a
  /// combobox that can be used to browse to different folder locations.
  /// </summary>
  public class FolderComboBoxViewModel : Base.ViewModelBase
  {
    #region fields
    private readonly ObservableCollection<FSItemViewModel> mCurrentItems;

    private string mCurrentFolder = string.Empty;
    private FSItemViewModel mSelectedItem = null;

    private RelayCommand<object> mSelectionChanged = null;
    private string mSelectedRecentLocation = string.Empty;

    private object mLockObject = new object();
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    public FolderComboBoxViewModel()
    {
      this.mCurrentItems = new ObservableCollection<FSItemViewModel>();
    }
    #endregion constructor

    #region Events
    /// <summary>
    /// Event is fired when the path in the text portion of the combobox is changed.
    /// </summary>
    public event EventHandler<FolderChangedEventArgs> RequestChangeOfDirectory;
    #endregion

    #region properties
    /// <summary>
    /// Expose a collection of file system items (folders and hard disks and ...) that
    /// can be selected and navigated to in this viewmodel.
    /// </summary>
    public ObservableCollection<FSItemViewModel> CurrentItems
    {
      get
      {
        return this.mCurrentItems;
      }
    }

    /// <summary>
    /// Gets/sets the currently selected file system viewmodel item.
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
    /// Get/sets viewmodel data pointing at the path
    /// of the currently selected folder.
    /// </summary>
    public string CurrentFolder
    {
      get
      {
        return this.mCurrentFolder;
      }

      set
      {
        if (this.mCurrentFolder != value)
        {
          this.mCurrentFolder = value;
          this.RaisePropertyChanged(() => this.CurrentFolder);
          this.RaisePropertyChanged(() => this.CurrentFolderToolTip);
        }
      }
    }

    /// <summary>
    /// Gets a string that can be displayed as a tooltip for the
    /// viewmodel data pointing at the path of the currently selected folder.
    /// </summary>
    public string CurrentFolderToolTip
    {
      get
      {
        if (string.IsNullOrEmpty(this.mCurrentFolder) == false)
          return string.Format("{0}\n{1}", this.mCurrentFolder,
                                           FileSystemModels.Local.Strings.SelectLocationCommand_TT);
        else
          return FileSystemModels.Local.Strings.SelectLocationCommand_TT;
      }
    }
    
    #region commands
    /// <summary>
    /// Command is invoked when the combobox view tells the viewmodel
    /// that the current path selection has changed (via selection changed
    /// event or keyup events).
    /// 
    /// The parameter p can be an array of objects
    /// containing objects of the FSItemVM type or
    /// p can also be string.
    /// 
    /// Each parameter item that adheres to the above types results in
    /// a OnCurrentPathChanged event being fired with the folder path
    /// as parameter.
    /// </summary>
    public ICommand SelectionChanged
    {
      get
      {
        if (this.mSelectionChanged == null)
          this.mSelectionChanged = new RelayCommand<object>((p) => this.SelectionChanged_Executed(p));

        return this.mSelectionChanged;
      }
    }
    #endregion commands
    #endregion properties

    #region methods
    /// <summary>
    /// Can be invoked to refresh the currently visible set of data.
    /// </summary>
    public void PopulateView()
    {
      lock (this.mLockObject)
      {
        ////CurrentItems.Clear();
        string bak = this.CurrentFolder;

        this.mCurrentItems.Clear();
        this.CurrentFolder = bak;

        // add drives
        string pathroot = string.Empty;

        if (string.IsNullOrEmpty(this.CurrentFolder) == false)
        {
          try
          {
            pathroot = System.IO.Path.GetPathRoot(this.CurrentFolder);
          }
          catch
          {
            pathroot = string.Empty;
          }
        }

        foreach (string s in Directory.GetLogicalDrives())
        {
          FSItemViewModel info = FSItemViewModel.CreateLogicalDrive(s);
          this.mCurrentItems.Add(info);

          // add items under current folder if we currently create the root folder of the current path
          if (string.IsNullOrEmpty(pathroot) == false && string.Compare(pathroot, s, true) == 0)
          {
            string[] dirs = this.CurrentFolder.Split(new char[] { System.IO.Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < dirs.Length; i++)
            {
              string curdir = string.Join(string.Empty + System.IO.Path.DirectorySeparatorChar, dirs, 0, i + 1);

              info = new FSItemViewModel(curdir, FSItemType.Folder, dirs[i], i * 10);

              this.mCurrentItems.Add(info);
            }

            // currently selected path was expanded in last for loop -> select the last expanded element 
            if (this.SelectedItem == null)
            {
              this.SelectedItem = this.mCurrentItems[this.mCurrentItems.Count - 1];

              if (this.RequestChangeOfDirectory != null)
                this.RequestChangeOfDirectory(this, new FolderChangedEventArgs(this.SelectedItem.GetModel));
            }
          }
        }

        // Force a selection on to the control when there is no selected item, yet
        if (this.mCurrentItems != null && this.SelectedItem == null)
        {
          if (this.mCurrentItems.Count > 0)
          {
            this.CurrentFolder = this.mCurrentItems[0].FullPath;
            this.SelectedItem = this.mCurrentItems[0];

            if (this.RequestChangeOfDirectory != null)
              this.RequestChangeOfDirectory(this, new FolderChangedEventArgs(this.SelectedItem.GetModel));
          }
        }
      }
    }

    /// <summary>
    /// Method executes when the SelectionChanged command is invoked.
    /// The parameter <paramref name="p"/> can be an array of objects
    /// containing objects of the <seealso cref="FSItemViewModel"/> type or
    /// p can also be string.
    /// 
    /// Each parameter item that adheres to the above types results in
    /// a OnCurrentPathChanged event being fired with the folder path
    /// as parameter.
    /// </summary>
    /// <param name="p"></param>
    private void SelectionChanged_Executed(object p)
    {
      if (p == null)
        return;

      // Check if the given parameter is an array of objects and process it...
      object[] paramObjects = p as object[];
      if (paramObjects != null)
      {
        for (int i = 0; i < paramObjects.Length; i++)
        {
          var item = paramObjects[i] as FSItemViewModel;

          if (item != null)
          {
            if (item.DirectoryPathExists() == true)
            {
              if (this.RequestChangeOfDirectory != null)
                this.RequestChangeOfDirectory(this, new FolderChangedEventArgs(item.GetModel));
            }
          }
        }
      }

      // Check if the given parameter is a string, fire a corresponding event if so...
      var paramString = p as string;
      if (paramString != null)
      {
        var path = new PathModel(paramString, FSItemType.Folder);

        if (path.DirectoryPathExists() == true)
        {
          if (this.RequestChangeOfDirectory != null)
            this.RequestChangeOfDirectory(this, new FolderChangedEventArgs(path));
        }
      }
    }
    #endregion methods
  }
}
