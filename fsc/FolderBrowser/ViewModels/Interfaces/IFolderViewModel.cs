namespace FolderBrowser.ViewModels.Interfaces
{
  using System.Collections.ObjectModel;
  using System.Windows.Input;
  using FileSystemModels.Models;
  using InplaceEditBoxLib.Events;

  /// <summary>
  /// Implement the interface for a viewmodel for one folder entry for a collection of folders.
  /// 
  /// This interface enables the parent viewmodel to abstact away from the actual collection
  /// viewmodel and use this interface (instead) whenever a collection item is accessed.
  /// </summary>
  public interface IFolderViewModel
  {
    #region events
    /// <summary>
    /// Call this method to request of start editing mode for renaming this item.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Returns true if event was successfully send (listener is attached), otherwise false</returns>
    bool RequestEditMode(RequestEditEvent request);
    #endregion events

    #region properties
    /// <summary>
    /// Gets the actual name of the folder that is represented by this object.
    /// </summary>
    string FolderName { get; }

    /// <summary>
    /// Get/set file system Path for this folder.
    /// </summary>
    string FolderPath { get; }

    /// <summary>
    /// Gets a folder item string for display purposes.
    /// This string can evaluete to 'C:\ (Windows)' for drives,
    /// if the 'C:\' drive was named 'Windows'.
    /// </summary>
    string DisplayItemString { get; }

    /// <summary>
    /// Get/set observable collection of sub-folders of this folder.
    /// </summary>
    ObservableCollection<IFolderViewModel> Folders { get; }

    /// <summary>
    /// Get/set whether this folder is currently selected or not.
    /// </summary>
    bool IsSelected { get; set; }

    /// <summary>
    /// Get/set whether this folder is currently expanded or not.
    /// </summary>
    bool IsExpanded { get; set; }

    /// <summary>
    /// Gets the type of this item (eg: Folder, HardDisk etc...).
    /// </summary>
    FSItemType ItemType { get; }

    /// <summary>
    /// Gets a command that will open the selected item with the current default application
    /// in Windows. The selected item (path to a file) is expected as FSItemVM parameter.
    /// (eg: Item is HTML file -> Open in Windows starts the web browser for viewing the HTML
    /// file if thats the currently associated Windows default application.
    /// </summary>
    ICommand OpenInWindowsCommand { get; }

    /// <summary>
    /// Gets a command that will copy the path of an item into the Windows Clipboard.
    /// The item (path to a file) is expected as FSItemVM parameter.
    /// </summary>
    ICommand CopyPathCommand { get; }
    #endregion properties

    #region methods
    /// <summary>
    /// Create a new folder with a standard name
    /// 'New folder n' underneath this folder.
    /// </summary>
    /// <returns>a viewmodel of the newly created directory or null</returns>
    IFolderViewModel CreateNewDirectory();

    /// <summary>
    /// Rename the name of the folder into a new name.
    /// </summary>
    /// <param name="newFolder"></param>
    void RenameFolder(string newFolder);
    #endregion methods
  }
}
