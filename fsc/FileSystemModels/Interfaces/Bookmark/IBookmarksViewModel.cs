namespace FileSystemModels.Interfaces.Bookmark
{
    using FileSystemModels.Browse;
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;

    /// <summary>
    /// Define an interface to a class that manages recently visited folder entries,
    /// </summary>
    public interface IBookmarksViewModel : ICloneable, ICanNavigate
    {
        #region properties
        /// <summary>
        /// Request a change of current directory to the directory
        /// stated in <seealso cref="FSItemViewModel"/> in CommandParameter.
        /// </summary>
        ICommand ChangeOfDirectoryCommand { get; }

        /// <summary>
        /// Command removes a folder bookmark from the list of
        /// currently bookmarked folders. Required command parameter
        /// is of type <seealso cref="FSItemViewModel"/>.
        /// </summary>
        ICommand RemoveFolderBookmark { get; }

        /// <summary>
        /// <inheritedoc />
        /// </summary>
        IEnumerable<IListItemViewModel> DropDownItems { get; }

        /// <summary>
        /// Gets/sets the selected item of the RecentLocations property.
        /// 
        /// This should be bound by the view (ItemsControl) to find the SelectedItem here.
        /// </summary>
        IListItemViewModel SelectedItem { get; set; }

        /// <summary>
        /// <inheritedoc />
        /// </summary>
        bool IsOpen { get; set; }
        #endregion properties

        #region methods
        /// <summary>
        /// Gets a data copy of the current object. Object specific fields, like events
        /// and their handlers are not copied.
        /// </summary>
        /// <returns></returns>
        IBookmarksViewModel CloneBookmark();

        /// <summary>
        /// Add a recent folder location into the collection of recent folders.
        /// This collection can then be used in the folder combobox drop down
        /// list to store user specific customized folder short-cuts.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="selectNewItem"></param>
        void AddFolder(string folderPath, bool selectNewItem = false);

        /// <summary>
        /// Removes all data items from the current collection of bookmarked folders.
        /// </summary>
        void ClearFolderCollection();

        /// <summary>
        /// Remove a recent folder location from the collection of recent folders.
        /// This collection can then be used in the folder combobox drop down
        /// list to store user specific customized folder short-cuts.
        /// </summary>
        /// <param name="folderPath"></param>
        void RemoveFolder(IPathModel folderPath);

        /// <summary>
        /// Removes all data items from the current collection of recent folders.
        /// </summary>
        void RemoveFolder(string path);
        #endregion methods
    }
}
