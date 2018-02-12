namespace FolderBrowser.Interfaces
{
    using FileSystemModels.Browse;
    using FileSystemModels.Interfaces.Bookmark;
    using FolderBrowser.ViewModels.Messages;
    using System.Collections.Generic;
    using System.Windows.Input;

    /// <summary>
    /// A browser viewmodel is about managing activities and properties related
    /// to displaying a treeview that repesents folders in the file system.
    /// 
    /// This viewmodel is almost equivalent to the backend code needed to drive
    /// the Treeview that shows the items in the UI.
    /// </summary>
    public interface IBrowserViewModel : INavigateable
    {
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
        bool UpdateView { get; set; }

        bool IsBrowseViewEnabled { get; set; }

        /// <summary>
        /// Gets a property to an object that is used to pop-up UI messages when errors occur.
        /// </summary>
        IDisplayMessageViewModel DisplayMessage { get; }

        /// <summary>
        /// Gets the list of drives and folders for display in treeview structure control.
        /// </summary>
        IEnumerable<ITreeItemViewModel> Root { get; }

        /// <summary>
        /// Get/set property to indicate the initial path when control
        /// starts up via Loading. The control attempts to change the
        /// current directory to the indicated directory if the
        /// ... method is called in the Loaded event of the
        /// <seealso cref="FolderBrowserDialog"/>.
        /// </summary>
        string InitialPath { get; set; }

        /// <summary>
        /// Get/set currently selected folder.
        /// </summary>
        string SelectedFolder { get; set; }

        /// <summary>
        /// Gets a list of Special Windows Standard folders for display in view.
        /// </summary>
        IEnumerable<ICustomFolderItemViewModel> SpecialFolders { get; }

        /// <summary>
        /// Gets whether the browser view should show a special folder control or not
        /// (A special folder control lets users browse to folders like 'My Documents'
        /// with a mouse click).
        /// </summary>
        bool IsSpecialFoldersVisisble { get; }

        /// <summary>
        /// Gets a command to cancel the current browsing process.
        /// </summary>
        ICommand CancelBrowsingCommand { get; }
        
        /// <summary>
        /// Gets a command that will copy the path of an item into the Windows Clipboard.
        /// The item (path to a file) is expected as FSItemVM parameter.
        /// </summary>
        ICommand CopyPathCommand { get; }

        /// <summary>
        /// Starts the create folder process by creating a new folder
        /// in the given location. The location is supplied as <seealso cref="System.Windows.Input.ICommandSource.CommandParameter"/>
        /// which is a <seealso cref="ITreeItemViewModel"/> item. So, the <seealso cref="ITreeItemViewModel"/> item
        /// is the parent of the new folder and the new folder is created with a standard name:
        /// 'New Folder n'. The new folder n is selected and in rename mode such that users can edit
        /// the name of the new folder right away.
        /// 
        /// This command implements an event that triggers the actual rename
        /// process in the connected view.
        /// </summary>
        ICommand CreateFolderCommand { get; }

        /// <summary>
        /// Gets a command that executes when a user expands a tree view item node in the treeview.
        /// </summary>
        ICommand ExpandCommand { get; }

        /// <summary>
        /// Get/set command to select the current folder.
        /// </summary>
        ICommand FolderSelectedCommand { get; }

        /// <summary>
        /// Gets a command that will open the selected item with the current default application
        /// in Windows. The selected item (path to a file) is expected as FSItemVM parameter.
        /// (eg: Item is HTML file -> Open in Windows starts the web browser for viewing the HTML
        /// file if thats the currently associated Windows default application.
        /// </summary>
        ICommand OpenInWindowsCommand { get; }

        /// <summary>
        /// Gets a command that will reload the folder view up to the
        /// selected path that is expected as <seealso cref="ITreeItemViewModel"/>
        /// in the CommandParameter.
        /// 
        /// This command is particularly useful when users create/delete a folder
        /// and want to update the treeview accordingly.
        /// </summary>
        ICommand RefreshViewCommand { get; }

        /// <summary>
        /// Renames the folder that is represented by this viewmodel.
        /// This command should be called directly by the implementing view
        /// since the new name of the folder is delivered as string.
        /// </summary>
        ICommand RenameCommand { get; }

        /// <summary>
        /// Gets a command that executes when the selected item in the treeview has changed.
        /// This updates a text property to inform other attached dependencies property controls
        /// about this change in selection state.
        /// </summary>
        ICommand SelectedFolderChangedCommand { get; }

        /// <summary>
        /// Starts the rename folder process by that renames the folder
        /// that is represented by this viewmodel.
        /// 
        /// This command implements an event that triggers the actual rename
        /// process in the connected view.
        /// </summary>
        ICommand StartRenameCommand { get; }

        /// <summary>
        /// Expose properties to commands that work with the bookmarking of folders.
        /// </summary>
        IEditBookmarks BookmarkFolder { get; }
        #endregion properties

        #region methods
/***
        /// <summary>
        /// Call this method in order to initialize a location
        /// when binding to view is not yet relized, but progress is already visible.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ResetBrowserStatus"></param>
        void BrowsePath(string path,
                        bool ResetBrowserStatus = true);
***/
        /// <summary>
        /// Determines whether the list of Windows special folder shortcut
        /// buttons (Music, Video etc) is visible or not.
        /// </summary>
        /// <param name="visible"></param>
        void SetSpecialFoldersVisibility(bool visible);
        #endregion methods
    }
}
