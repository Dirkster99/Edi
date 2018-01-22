namespace FileListView.ViewModels.Interfaces
{
  using FileSystemModels.Interfaces;
  using FolderBrowser.ViewModels.Interfaces;

  /// <summary>
  /// Interface implements a folder/file view model class
  /// that can be used to dispaly filesystem related content in an ItemsControl.
  /// </summary>
  public interface IFolderListViewModel : IConfigExplorerSettings
  {
    #region properties
    /// <summary>
    /// Expose a viewmodel that can represent a Folder-ComboBox drop down
    /// element similar to a web browser Uri drop down control but using
    /// local paths only.
    /// </summary>
    FolderComboBoxViewModel FolderTextPath { get; }

    /// <summary>
    /// Expose a viewmodel that can represent a Filter-ComboBox drop down
    /// similar to the top-right filter/search combo box in Windows Exploer windows.
    /// </summary>
    FilterComboBoxViewModel Filters { get; }

    /// <summary>
    /// Expose a viewmodel that can support a listview showing folders and files
    /// with their system specific icon.
    /// </summary>
    IFileListViewModel FolderItemsView { get; }

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

    /// <summary>
    /// Master controler interface method to navigate all views
    /// to the folder indicated in <paramref name="folder"/>
    /// - updates all related viewmodels.
    /// </summary>
    /// <param name="folder"></param>
    void NavigateToFolder(string folder);

    #region Synchronize with IBrowserViewModel methods
    /// <summary>
    /// Attach a <seealso cref="IBrowserViewModel"/> to synchronize the
    /// current path with the <seealso cref="IFolderListViewModel"/>.
    /// </summary>
    /// <param name="folderBrowser"></param>
    void AttachFolderBrowser(IBrowserViewModel folderBrowser);

    /// <summary>
    /// Detach the <seealso cref="IBrowserViewModel"/> (if any) to stop
    /// synchronizing the current path with the <seealso cref="IFolderListViewModel"/>.
    /// elements with it.
    /// </summary>
    void DetachFolderBrowser();
    #endregion Synchronize with IBrowserViewModel methods
    #endregion methods
  }
}
