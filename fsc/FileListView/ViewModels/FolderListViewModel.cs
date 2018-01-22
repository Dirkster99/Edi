namespace FileListView.ViewModels
{
  using System.Windows;
  using FileListView.ViewModels.Interfaces;
  using FileSystemModels.Interfaces;
  using FileSystemModels.Models;
  using FolderBrowser.ViewModels;
  using FolderBrowser.ViewModels.Interfaces;

  /// <summary>
  /// Class implements a folder/file view model class
  /// that can be used to dispaly filesystem related content in an ItemsControl.
  /// </summary>
  public class FolderListViewModel : Base.ViewModelBase, IFolderListViewModel
  {
    #region fields
    private string mSelectedFolder = string.Empty;

    private object mLockObject = new object();
    #endregion fields

    #region constructor
    /// <summary>
    /// Custom class constructor
    /// </summary>
    /// <param name="onFileOpenMethod"></param>
    public FolderListViewModel(System.EventHandler<FileSystemModels.Events.FileOpenEventArgs> onFileOpenMethod)
      : this()
    {
      // Remove the standard constructor event that is fired when a user opens a file
      this.FolderItemsView.OnFileOpen -= this.FolderItemsView_OnFileOpen;

      // ...and establish a new link (if any)
      if (onFileOpenMethod != null)
        this.FolderItemsView.OnFileOpen += onFileOpenMethod;
    }

    /// <summary>
    /// Class constructor
    /// </summary>
    public FolderListViewModel()
    {
      // This viewmodel can work with or without folderbrowser
      this.FolderBrowser = null;

      this.FolderItemsView = new FileListViewModel(new BrowseNavigation());

      this.FolderTextPath = new FolderComboBoxViewModel();

      this.RecentFolders = new RecentLocationsViewModel();

      // This is fired when the user selects a new folder bookmark from the drop down button
      this.RecentFolders.RequestChangeOfDirectory += this.OnRequestChangeOfDirectory;

      // This is fired when the text path in the combobox changes to another existing folder
      this.FolderTextPath.RequestChangeOfDirectory += this.OnRequestChangeOfDirectory;

      this.Filters = new FilterComboBoxViewModel();
      this.Filters.OnFilterChanged += this.FileViewFilter_Changed;

      // This is fired when the current folder in the listview changes to another existing folder
      this.FolderItemsView.RequestChangeOfDirectory += this.OnRequestChangeOfDirectory;

      // This is fired when the user requests to add a folder into the list of recently visited folders
      this.FolderItemsView.RequestEditRecentFolder += this.FolderItemsView_RequestEditBookmarkedFolders;

      // This event is fired when a user opens a file
      this.FolderItemsView.OnFileOpen += this.FolderItemsView_OnFileOpen;

      this.FolderTextPath.PopulateView();
    }
    #endregion constructor

    #region properties
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

    /// <summary>
    /// Gets the viewmodel that exposes recently visited locations (bookmarked folders).
    /// </summary>
    public RecentLocationsViewModel RecentFolders { get; private set; }

    /// <summary>
    /// Expose a viewmodel that can represent a Folder-ComboBox drop down
    /// element similar to a web browser Uri drop down control but using
    /// local paths only.
    /// </summary>
    public FolderComboBoxViewModel FolderTextPath { get; private set; }

    /// <summary>
    /// Expose a viewmodel that can represent a Filter-ComboBox drop down
    /// similar to the top-right filter/search combo box in Windows Exploer windows.
    /// </summary>
    public FilterComboBoxViewModel Filters { get; private set; }

    /// <summary>
    /// Expose a viewmodel that can support a listview showing folders and files
    /// with their system specific icon.
    /// </summary>
    public IFileListViewModel FolderItemsView { get; private set; }

    /// <summary>
    /// Gets a synchronized <seealso cref="IBrowserViewModel"/> if there is 1 attached
    /// and otherwise null;
    /// </summary>
    public IBrowserViewModel FolderBrowser { get; set; }

    /// <summary>
    /// Gets the currently selected folder path string.
    /// </summary>
    public string SelectedFolder
    {
      get
      {
        return this.mSelectedFolder;
      }

      private set
      {
        if (this.mSelectedFolder != value)
        {
          this.mSelectedFolder = value;
          this.RaisePropertyChanged(() => this.SelectedFolder);
        }
      }
    }
    #endregion properties

    #region methods
    #region Folder Browser Attachment methods
    /// <summary>
    /// Attach a <seealso cref="IBrowserViewModel"/> to synchronize the
    /// current path with the <seealso cref="IFolderListViewModel"/>.
    /// </summary>
    /// <param name="folderBrowser"></param>
    public void AttachFolderBrowser(IBrowserViewModel folderBrowser)
    {
      this.DetachFolderBrowser();

      lock (this.mLockObject)
      {
        this.FolderBrowser = folderBrowser;

        if (this.FolderBrowser != null)
        {
          this.FolderBrowser.FolderSelectionChangedEvent += this.OnRequestChangeOfDirectory;
          this.FolderBrowser.RequestEditBookmarkedFolders += this.FolderItemsView_RequestEditBookmarkedFolders;

          this.FolderBrowser.SetSelectedFolder(this.SelectedFolder);
        }
      }
    }

    /// <summary>
    /// Detach the <seealso cref="IBrowserViewModel"/> (if any) to stop
    /// synchronizing the current path with the <seealso cref="IFolderListViewModel"/>.
    /// elements with it.
    /// </summary>
    public void DetachFolderBrowser()
    {
      lock (this.mLockObject)
      {
        if (this.FolderBrowser != null)
        {
          this.FolderBrowser.FolderSelectionChangedEvent += this.OnRequestChangeOfDirectory;
          this.FolderBrowser.RequestEditBookmarkedFolders -= this.FolderItemsView_RequestEditBookmarkedFolders;
        }

        this.FolderBrowser = null;
      }
    }
    #endregion Folder Browser Attachment methods

    #region Explorer settings model
    /// <summary>
    /// Configure this viewmodel (+ attached browser viewmodel) with the given settings and
    /// initialize viewmodels with UserProfile.CurrentPath location.
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    bool IConfigExplorerSettings.ConfigureExplorerSettings(ExplorerSettingsModel settings)
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
        this.NavigateToFolder(settings.UserProfile.CurrentPath.Path);

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

        this.RecentFolders.ClearRecentFolderCollection();

        // Set collection of recent folder locations
        foreach (var item in settings.RecentFolders)
          this.RecentFolders.AddRecentFolder(item);

        if (string.IsNullOrEmpty(settings.LastSelectedRecentFolder) == false)
          this.AddRecentFolder(settings.LastSelectedRecentFolder, true);

        this.FolderItemsView.ShowIcons = settings.ShowIcons;
        this.FolderItemsView.SetIsFolderVisible(settings.ShowFolders);
        this.FolderItemsView.ShowHidden = settings.ShowHiddenFiles;
        this.FolderItemsView.SetIsFiltered(settings.IsFiltered);

        if (this.FolderBrowser != null)
        {
          this.FolderBrowser.SpecialFolders.Clear();

          foreach (var item in settings.SpecialFolders)
            this.FolderBrowser.SpecialFolders.Add(new CustomFolderItemViewModel(item.SpecialFolder));

          ////this.FolderBrowser.SetSpecialFoldersVisibility(settings.ShowSpecialFoldersInTreeBrowser);
          this.FolderBrowser.SetSpecialFoldersVisibility(false);
        }
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
    ExplorerSettingsModel IConfigExplorerSettings.GetExplorerSettings(ExplorerSettingsModel input)
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

        if (this.FolderBrowser != null)
        {
          foreach (var item in this.FolderBrowser.SpecialFolders)
            settings.AddSpecialFolder(item.SpecialFolder);

          //// settings.ShowSpecialFoldersInTreeBrowser = this.FolderBrowser.IsSpecialFoldersVisisble;
        }

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
      this.RecentFolders.AddRecentFolder(folderPath, selectNewFolder);
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

      this.RecentFolders.RemoveRecentFolder(path);
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
    /// Master controler interface method to navigate all views
    /// to the folder indicated in <paramref name="folder"/>
    /// - updates all related viewmodels.
    /// </summary>
    /// <param name="folder"></param>
    public void NavigateToFolder(string folder)
    {
      lock (this.mLockObject)
      {
        this.SelectedFolder = folder;

        // Navigate Folder ComboBox to this folder
        this.FolderTextPath.CurrentFolder = folder;
        this.FolderTextPath.PopulateView();
        
        // Navigate Folder/File ListView to this folder
        this.FolderItemsView.NavigateToThisFolder(folder);

        // Navigate Folder Treeview to this folder
        if (this.FolderBrowser != null)
          this.FolderBrowser.SetSelectedFolder(folder);
      }
    }
    
    /// <summary>
    /// Executes when the file open event is fired and class was constructed with statndard constructor.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void FolderItemsView_OnFileOpen(object sender,
                                              FileSystemModels.Events.FileOpenEventArgs e)
    {
      MessageBox.Show("File Open:" + e.FileName);
    }

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
    private void FolderItemsView_RequestEditBookmarkedFolders(object sender, FileSystemModels.Events.RecentFolderEvent e)
    {
      switch (e.Action)
      {
        case FileSystemModels.Events.RecentFolderEvent.RecentFolderAction.Remove:
          this.RecentFolders.RemoveRecentFolder(e.Folder);
          break;

        case FileSystemModels.Events.RecentFolderEvent.RecentFolderAction.Add:
          this.RecentFolders.AddRecentFolder(e.Folder.Path);
          break;

        default:
          break;
      }
    }

    /// <summary>
    /// Method is executed when a control (folder combobox, directory tree, or file/folder listview)
    /// requests a change of directory - in other words, all other controls should navigate to a new
    /// location. The event is processed by calling all corresponding methods in the viewmodels of
    /// each control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnRequestChangeOfDirectory(object sender, FileSystemModels.Events.FolderChangedEventArgs e)
    {
      lock (this.mLockObject)
      {
        if (string.Compare(this.SelectedFolder, e.Folder.Path, true) != 0)
        {
          this.NavigateToFolder(e.Folder.Path);
 
          if (this.FolderBrowser != null)
          {
            if (sender != this.FolderBrowser)
              this.FolderBrowser.SetSelectedFolder(e.Folder.Path);
          }
        }
      }
    }
    #endregion methods
  }
}
