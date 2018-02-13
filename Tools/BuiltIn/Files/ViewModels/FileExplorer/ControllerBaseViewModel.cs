namespace Files.ViewModels.FileExplorer
{
    using Edi.Core.ViewModels.Command;
    using FileListView.Interfaces;
    using FileSystemModels;
    using FileSystemModels.Events;
    using FileSystemModels.Interfaces;
    using FileSystemModels.Interfaces.Bookmark;
    using FileSystemModels.Models;
    using FilterControlsLib.Interfaces;
    using FolderControlsLib.Interfaces;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Base class for Explorer Window to drive and control the various views
    /// (folder combobox, folder listview, treebrowser) in a coordinated fashion.
    /// </summary>
    public abstract class ControllerBaseViewModel : Edi.Core.ViewModels.ToolViewModel,
                                                    IConfigExplorerSettings
    {
        #region fields
        private string _SelectedFolder = string.Empty;
        private ICommand mRefreshCommand;
        #endregion fields

        #region constructors
        /// <summary>
        /// Base constructor from nam of tool window item
        /// </summary>
        /// <param name="name">Name of tool window displayed in GUI</param>
        public ControllerBaseViewModel(string name)
            : base(name)
        {
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets the command that updates the currently viewed
        /// list of directory items (files and sub-directories).
        /// </summary>
        public ICommand RefreshCommand
        {
            get
            {
                if (this.mRefreshCommand == null)
                    this.mRefreshCommand = new RelayCommand<object>
                        ((p) =>
                        {
                            RefreshCommand_Executed(p as string);
                        });

                return this.mRefreshCommand;
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

        /// <summary>
        /// Gets the viewmodel that exposes recently visited locations (bookmarked folders).
        /// </summary>
        public IBookmarksViewModel RecentFolders { get; protected set; }

        /// <summary>
        /// Expose a viewmodel that can represent a Folder-ComboBox drop down
        /// element similar to a web browser Uri drop down control but using
        /// local paths only.
        /// </summary>
        public IFolderComboBoxViewModel FolderTextPath { get; protected set; }

        /// <summary>
        /// Expose a viewmodel that can represent a Filter-ComboBox drop down
        /// similar to the top-right filter/search combo box in Windows Exploer windows.
        /// </summary>
        public IFilterComboBoxViewModel Filters { get; protected set; }

        /// <summary>
        /// Expose a viewmodel that can support a listview showing folders and files
        /// with their system specific icon.
        /// </summary>
        public IFileListViewModel FolderItemsView { get; protected set; }

        /// <summary>
        /// Gets the currently selected folder path string.
        /// </summary>
        public string SelectedFolder
        {
            get
            {
                return this._SelectedFolder;
            }

            protected set
            {
                if (this._SelectedFolder != value)
                {
                    this._SelectedFolder = value;
                    base.RaisePropertyChanged(() => this.SelectedFolder);
                }
            }
        }
        #endregion properties

        #region methods
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
//// This should be done on visibility of tool_window view
////                this.NavigateToFolder(settings.UserProfile.CurrentPath);
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

                if (Filters.SelectedItem != null)
                    settings.UserProfile.SetCurrentFilter(
                        new FilterItemModel(Filters.SelectedItem.FilterDisplayName, Filters.SelectedItem.FilterText));

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
        /// Master controller interface method to navigate all views
        /// to the folder indicated in <paramref name="folder"/>
        /// - updates all related viewmodels.
        /// </summary>
        /// <param name="itemPath"></param>
        /// <param name="requestor"</param>
        public abstract void NavigateToFolder(IPathModel itemPath);

        /// <summary>
        /// Method executes when the user requests via UI bound command
        /// to refresh the currently displayed list of items.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected virtual void RefreshCommand_Executed(string path = null)
        {
            try
            {
                IPathModel location = null;
                if (path != null)
                    location = PathFactory.Create(path);
                else
                    location = PathFactory.Create(FolderTextPath.CurrentFolder);

                // XXX Todo Keep task reference, support cancel, and remove on end?
                NavigateToFolder(location);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Executes when the file open event is fired and class was constructed with statndard constructor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void FolderItemsView_OnFileOpen(object sender,
                                                          FileOpenEventArgs e)
        {
            MessageBox.Show("File Open:" + e.FileName);
        }

        /// <summary>
        /// Applies the file/directory filter from the combobox on the listview entries.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FileViewFilter_Changed(object sender, FilterChangedEventArgs e)
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
        protected void FolderItemsView_RequestEditBookmarkedFolders(object sender, EditBookmarkEvent e)
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
        #endregion methods
    }
}
