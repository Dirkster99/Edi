namespace Files.ViewModels.FileExplorer
{
    using FileListView.Interfaces;
    using FileSystemModels.Interfaces;
    using FileSystemModels.Interfaces.Bookmark;
    using FilterControlsLib.Interfaces;
    using FolderBrowser.Interfaces;
    using FolderControlsLib.Interfaces;

    /// <summary>
    /// Class implements a tree/folder/file view model class
    /// that can be used to dispaly filesystem related content in a view or dialog.
    /// 
    /// Common Sample dialogs are file pickers for load/save etc.
    /// </summary>
    public interface ITreeListControllerViewModel : IConfigExplorerSettings
    {
        #region properties
        /// <summary>
        /// Expose a viewmodel that can represent a Folder-ComboBox drop down
        /// element similar to a web browser Uri drop down control but using
        /// local paths only.
        /// </summary>
        IFolderComboBoxViewModel FolderTextPath { get; }

        /// <summary>
        /// Expose a viewmodel that can represent a Filter-ComboBox drop down
        /// similar to the top-right filter/search combo box in Windows Exploer windows.
        /// </summary>
        IFilterComboBoxViewModel Filters { get; }

        /// <summary>
        /// Expose a viewmodel that can support a listview showing folders and files
        /// with their system specific icon.
        /// </summary>
        IFileListViewModel FolderItemsView { get; }

        /// <summary>
        /// Gets the folder browser viewmodel that drives the tree view which displays
        /// drives and their folder items.
        /// </summary>
        IBrowserViewModel TreeBrowser { get; }

        /// <summary>
        /// Gets the viewmodel that exposes recently visited locations (bookmarked folders).
        /// </summary>
        IBookmarksViewModel RecentFolders { get; }

        /// <summary>
        /// Gets/sets the currently selected folder path string.
        /// </summary>
        string SelectedFolder { get; }

        /// <summary>
        /// Gets/sets the currently selected recent location string (if any) or null.
        /// </summary>
        string SelectedRecentLocation { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Master controler interface method to navigate all views
        /// to the folder indicated in <paramref name="folder"/>
        /// - updates all related viewmodels.
        /// </summary>
        /// <param name="itemPath"></param>
        /// <param name="requestor"</param>
        void NavigateToFolder(IPathModel itemPath);

        /// <summary>
        /// Add a recent folder location into the collection of recent folders.
        /// This collection can then be used in the folder combobox drop down
        /// list to store user specific customized folder short-cuts.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="selectNewFolder"></param>
        void AddRecentFolder(string folderPath, bool selectNewFolder = true);

        /// <summary>
        /// Removes a recent folder location into the collection of recent folders.
        /// This collection can then be used in the folder combobox drop down
        /// list to store user specific customized folder short-cuts.
        /// </summary>
        /// <param name="path"></param>
        void RemoveRecentFolder(string path);

        /// <summary>
        /// Copies all of the given bookmars into the destionations bookmarks collection.
        /// </summary>
        /// <param name="srcRecentFolders"></param>
        /// <param name="dstRecentFolders"></param>
        void CloneBookmarks(IBookmarksViewModel srcRecentFolders,
                            IBookmarksViewModel dstRecentFolders);

        /// <summary>
        /// Add a new filter argument to the list of filters and
        /// select this filter if <paramref name="bSelectNewFilter"/>
        /// indicates it.
        /// </summary>
        /// <param name="filterString"></param>
        /// <param name="bSelectNewFilter"></param>
        void AddFilter(string filterString,
                              bool bSelectNewFilter = false);

        /// <summary>
        /// Add a new filter argument to the list of filters and
        /// select this filter if <paramref name="bSelectNewFilter"/>
        /// indicates it.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="filterString"></param>
        /// <param name="bSelectNewFilter"></param>
        void AddFilter(string name,
                       string filterString,
                       bool bSelectNewFilter = false);
        #endregion methods
    }
}
