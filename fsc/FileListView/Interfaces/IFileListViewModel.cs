namespace FileListView.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;
    using FileSystemModels.Browse;
    using FileSystemModels.Events;
    using FileSystemModels.Interfaces.Bookmark;

    /// <summary>
    /// Interface implements a common ground for a class that organizes a filter combobox
    /// view with a file list view.
    /// </summary>
    public interface IFileListViewModel : INavigateable
    {
        #region Events
        /// <summary>
        /// Event is fired to indicate that user wishes to open a file via this viewmodel.
        /// </summary>
        event EventHandler<FileOpenEventArgs> OnFileOpen;
        #endregion

        #region properties
        /// <summary>
        /// Expose properties to commands that work with the bookmarking of folders.
        /// </summary>
        IEditBookmarks BookmarkFolder { get; }

        /// <summary>
        /// Gets/sets list of files and folders to be displayed in connected view.
        /// </summary>
        IEnumerable<ILVItemViewModel> CurrentItems { get; }

        /// <summary>
        /// Gets/sets whether the list of folders and files should include folders or not.
        /// </summary>
        bool ShowFolders{ get; }

        /// <summary>
        /// Gets/sets whether the list of folders and files includes hidden folders or files.
        /// </summary>
        bool ShowHidden { get; }

        /// <summary>
        /// Gets/sets whether the list of folders and files includes an icon or not.
        /// </summary>
        bool ShowIcons{ get; }

        /// <summary>
        /// Gets whether the list of folders and files is filtered or not.
        /// </summary>
        bool IsFiltered { get; }

        /// <summary>
        /// Gets the current path this viewmodel assigned to look at.
        /// This property is not updated (must be polled) so its not
        /// a good idea to bind to it.
        /// </summary>
        string CurrentFolder { get; }

        #region commands
        /// <summary>
        /// Navigates to a folder that was visited before navigating back (if any).
        /// </summary>
        ICommand NavigateForwardCommand
        {
            get;
        }

        /// <summary>
        /// Navigates back to a folder that was visited before the current folder (if any).
        /// </summary>
        ICommand NavigateBackCommand
        {
            get;
        }

        /// <summary>
        /// Browse into the parent folder path of a given path.
        /// </summary>
        ICommand NavigateUpCommand
        {
            get;
        }

        /// <summary>
        /// Browse into a given a path.
        /// </summary>
        /// <returns></returns>
        ICommand NavigateDownCommand
        {
            get;
        }

        /// <summary>
        /// Gets the command that updates the currently viewed
        /// list of directory items (files and sub-directories).
        /// </summary>
        ICommand RefreshCommand
        {
            get;
        }

        /// <summary>
        /// Toggles the visibiliy of folders in the folder/files listview.
        /// </summary>
        ICommand ToggleIsFolderVisibleCommand
        {
            get;
        }

        /// <summary>
        /// Toggles the visibiliy of icons in the folder/files listview.
        /// </summary>
        ICommand ToggleIsIconVisibleCommand
        {
            get;
        }

        /// <summary>
        /// Toggles the visibiliy of hidden files/folders in the folder/files listview.
        /// </summary>
        ICommand ToggleIsHiddenVisibleCommand
        {
            get;
        }

        /// <summary>
        /// Gets a command that will open the folder in which an item is stored.
        /// The item (path to a file) is expected as <seealso cref="FSItemViewModel"/> parameter.
        /// </summary>
        ICommand OpenContainingFolderCommand
        {
            get;
        }

        /// <summary>
        /// Gets a command that will open the selected item with the current default application
        /// in Windows. The selected item (path to a file) is expected as <seealso cref="FSItemViewModel"/> parameter.
        /// (eg: Item is HTML file -> Open in Windows starts the web browser for viewing the HTML
        /// file if thats the currently associated Windows default application.
        /// </summary>
        ICommand OpenInWindowsCommand
        {
            get;
        }

        /// <summary>
        /// Gets a command that will copy the path of an item into the Windows Clipboard.
        /// The item (path to a file) is expected as <seealso cref="FSItemViewModel"/> parameter.
        /// </summary>
        ICommand CopyPathCommand
        {
            get;
        }
        #endregion commands
        #endregion properties

        #region methods
        /// <summary>
        /// Applies a filter string (which can contain multiple
        /// alternative regular expression filter items) and updates
        /// the current display.
        /// </summary>
        /// <param name="filterText"></param>
        void ApplyFilter(string filterText);

        /// <summary>
        /// Configure whether folders are part of the list of
        /// files and folders or not (list only files without folders).
        /// </summary>
        /// <param name="isFolderVisible"></param>
        void SetIsFolderVisible(bool isFolderVisible);

        /// <summary>
        /// Configure whether folders are part of the list of
        /// files and folders or not (list only files without folders).
        /// </summary>
        /// <param name="isFolderVisible"></param>
        void SetIsFiltered(bool isFolderVisible);

        /// <summary>
        /// Configure whether icons in listview should be shown or not.
        /// </summary>
        /// <param name="showIcons"></param>
        void SetShowIcons(bool showIcons);

        /// <summary>
        /// Configure whether or not hidden files are shown in listview.
        /// </summary>
        /// <param name="showHiddenFiles"></param>
        void SetShowHidden(bool showHiddenFiles);
        #endregion methods
    }
}
