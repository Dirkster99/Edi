namespace FolderBrowser.ViewModels.Interfaces
{
  using System;
  using System.Collections.ObjectModel;
  using System.Windows.Input;
  using FileSystemModels.Events;

  /// <summary>
  /// Implements an interface for a viewmodel that supports
  /// browsing of folder structures in the file system.
  /// </summary>
  public interface IBrowserViewModel
  {
    #region events
    /// <summary>
    /// Event is fired to indicate that user wishes to select a certain Path.
    /// </summary>
    event EventHandler<FolderChangedEventArgs> FinalPathSelectionEvent;

    /// <summary>
    /// This event is triggered when the currently selected folder has changed.
    /// </summary>
    event EventHandler<FolderChangedEventArgs> FolderSelectionChangedEvent;

    /// <summary>
    /// Generate an event to remove or add a recent folder to a collection.
    /// </summary>
    event EventHandler<RecentFolderEvent> RequestEditBookmarkedFolders;
    #endregion events

    #region properties
    /// <summary>
    /// Get/set currently selected folder.
    /// </summary>
    string SelectedFolder { get; set; }

    /// <summary>
    /// Gets the list of drives and folders for display in treeview structure control.
    /// </summary>
    ObservableCollection<IFolderViewModel> Folders { get; }

    /// <summary>
    /// Get/set command to select the current folder.
    /// </summary>
    ICommand FolderSelectedCommand { get; }

    /// <summary>
    /// This command can be used to do a final select of a particular folder and tell
    /// the consumer of this viewmodel that the user wants to select this folder.
    /// The consumer can then diactivate the dialog or browse to the requested location
    /// using whatever is required outside of this control....
    /// </summary>
    ICommand FinalSelectDirectoryCommand { get; }

    /// <summary>
    /// Gets a list of Special Windows Standard folders for display in view.
    /// </summary>
    ObservableCollection<CustomFolderItemViewModel> SpecialFolders { get; }

    /// <summary>
    /// Gets whether the browser view should show a special folder control or not
    /// (A special folder control lets users browse to folders like 'My Documents'
    /// with a mouse click).
    /// </summary>
    bool IsSpecialFoldersVisisble
    {
      get;
    }
    #endregion properties

    #region properties
    /// <summary>
    /// Assign the currently selected folder with this path.
    /// </summary>
    /// <param name="selectedFolder"></param>
    void SetSelectedFolder(string selectedFolder);
    #endregion properties

    #region methods
    /// <summary>
    /// Determines whether the browser view should show a special folder control or not
    /// (A special folder control lets users browse to folders like 'My Documents'
    /// with a mouse click).
    /// </summary>
    /// <param name="visible"></param>
    void SetSpecialFoldersVisibility(bool visible);
    #endregion methods
  }
}
