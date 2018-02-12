namespace FileSystemModels.ViewModels
{
    using FileSystemModels;
    using FileSystemModels.Events;
    using FileSystemModels.Interfaces;
    using FileSystemModels.Models.FSItems.Base;
    using FileSystemModels.ViewModels.Base;
    using FileSystemModels.Utils;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using System.Collections.Generic;
    using FileSystemModels.Interfaces.Bookmark;
    using FileSystemModels.Browse;

    /// <summary>
    /// Implement viewmodel for management of recent folder locations.
    /// </summary>
    internal class BookmarkesViewModel : ViewModelBase, IBookmarksViewModel
    {
        #region fields
        private IListItemViewModel mSelectedItem;
        private ObservableCollection<IListItemViewModel> _DropDownItems;

        private object mLockObject = new object();

        private RelayCommand<object> mChangeOfDirectoryCommand;
        private RelayCommand<object> mRemoveFolderBookmark;
        private bool mIsOpen;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public BookmarkesViewModel()
        {
            _DropDownItems = new ObservableCollection<IListItemViewModel>();
            this.IsOpen = false;
        }

        /// <summary>
        /// Copy class constructor
        /// </summary>
        /// <param name="copyThis"></param>
        public BookmarkesViewModel(BookmarkesViewModel copyThis)
            : this()
        {
            if (copyThis == null)
                return;

            ////this.IsOpen = copyThis.IsOpen; this could magically open other drop downs :-)

            foreach (var item in copyThis.DropDownItems)
                _DropDownItems.Add(new ListItemViewModel(item as ListItemViewModel));

            // Select quivalent item in target collection
            if (copyThis.SelectedItem != null)
            {
                string fullPath = copyThis.SelectedItem.FullPath;
                var result = DropDownItems.SingleOrDefault(item => fullPath == item.FullPath);

                if (result != null)
                    SelectedItem = result;
            }
        }
        #endregion constructor

        #region events
        /// <summary>
        /// Indicates when the viewmodel starts heading off somewhere else
        /// and when its done browsing to a new location.
        /// </summary>
        public event EventHandler<BrowsingEventArgs> BrowseEvent;
        #endregion events

        #region properties
        /// <summary>
        /// Gets a command that requests a change of current directory to the
        /// directory stated in <seealso cref="ListItemViewModel"/> in
        /// CommandParameter.  -> Fires a FolderChange Event.
        /// </summary>
        public ICommand ChangeOfDirectoryCommand
        {
            get
            {
                if (this.mChangeOfDirectoryCommand == null)
                    this.mChangeOfDirectoryCommand = new RelayCommand<object>((p) =>
                    {
                        var param = p as IListItemViewModel;

                        if (param != null)
                            this.ChangeOfDirectoryCommand_Executed(param);
                    });

                return this.mChangeOfDirectoryCommand;
            }
        }

        /// <summary>
        /// Command removes a folder bookmark from the list of
        /// currently bookmarked folders. Required command parameter
        /// is of type <seealso cref="ListItemViewModel"/>.
        /// </summary>
        public ICommand RemoveFolderBookmark
        {
            get
            {
                if (this.mRemoveFolderBookmark == null)
                    this.mRemoveFolderBookmark = new RelayCommand<object>((p) =>
                    {
                        var param = p as IListItemViewModel;

                        if (param != null)
                            this.RemoveFolderBookmark_Executed(param);
                    });

                return this.mRemoveFolderBookmark;
            }
        }

        /// <summary>
        /// <inheritedoc />
        /// </summary>
        public IEnumerable<IListItemViewModel> DropDownItems
        {
            get
            {
                return _DropDownItems;
            }
        }

        /// <summary>
        /// Gets/set the selected item of the RecentLocations property.
        /// 
        /// This should be bound by the view (ItemsControl) to find the SelectedItem here.
        /// </summary>
        public IListItemViewModel SelectedItem
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
        /// <inheritedoc />
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return this.mIsOpen;
            }

            set
            {
                if (this.mIsOpen != value)
                {
                    this.mIsOpen = value;
                    this.RaisePropertyChanged(() => this.IsOpen);
                }
            }
        }

        /// <summary>
        /// This control cannot browse towards a certain location which
        /// is why it returns a constant value of false here.
        /// </summary>
        public bool IsBrowsing
        {
            get
            {
                return false;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Gets a data copy of the current object. Object specific fields, like events
        /// and their handlers are not copied.
        /// </summary>
        /// <returns></returns>
        public IBookmarksViewModel CloneBookmark()
        {
            return new BookmarkesViewModel(this);
        }

        /// <summary>
        /// Gets a data copy of the current object. Object specific fields, like events
        /// and their handlers are not copied.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return CloneBookmark();
        }

        /// <summary>
        /// Add a recent folder location into the collection of recent folders.
        /// This collection can then be used in the folder combobox drop down
        /// list to store user specific customized folder short-cuts.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="selectNewItem"></param>
        public void AddFolder(string folderPath,
                                      bool selectNewItem = false)
        {
            lock (this.mLockObject)
            {
                if ((folderPath = PathFactory.ExtractDirectoryRoot(folderPath)) == null)
                    return;

                // select this path if its already there
                var results = this.DropDownItems.Where<IListItemViewModel>(folder => string.Compare(folder.FullPath, folderPath, true) == 0);

                // Do not add this twice
                if (results != null)
                {
                    if (results.Count() != 0)
                    {
                        if (selectNewItem == true)
                            this.SelectedItem = results.First();

                        return;
                    }
                }

                var folderVM = this.CreateFSItemVMFromString(folderPath);

                _DropDownItems.Add(folderVM);

                if (selectNewItem == true)
                    this.SelectedItem = folderVM;
            }
        }

        /// <summary>
        /// Remove a recent folder location from the collection of recent folders.
        /// This collection can then be used in the folder combobox drop down
        /// list to store user specific customized folder short-cuts.
        /// </summary>
        /// <param name="folderPath"></param>
        public void RemoveFolder(IPathModel folderPath)
        {
            lock (this.mLockObject)
            {
                if (folderPath == null)
                    return;

                // Find all items that satisfy the query match and remove them
                // (This statement requires a Linq extension method to work)
                // See FileSystemModels.Utils for more details
                _DropDownItems.Remove(i => string.Compare(folderPath.Path, i.FullPath, true) == 0);
            }
        }

        /// <summary>
        /// Removes all data items from the current collection of recent folders.
        /// </summary>
        public void ClearFolderCollection()
        {
            if (this.DropDownItems != null)
                _DropDownItems.Clear();
        }

        /// <summary>
        /// Removes all data items from the current collection of recent folders.
        /// </summary>
        public void RemoveFolder(string path)
        {
            try
            {
                this.RemoveFolder(PathFactory.Create(path, FSItemType.Folder));
            }
            catch
            {
            }
        }

        /// <summary>
        /// Method is invoked when the control requests the controller to browse
        /// to a new location as selected by the user in the list of recent locations (bookmarks).
        /// </summary>
        /// <param name="path"></param>
        private void ChangeOfDirectoryCommand_Executed(IListItemViewModel path)
        {
            if (path == null)
                return;

            this.IsOpen = false;
            this.SelectedItem = path;

            if (BrowseEvent != null)
            {
                var targetPath = PathFactory.Create(path.FullPath);
                BrowseEvent(this, new BrowsingEventArgs(targetPath, false, BrowseResult.Complete));
            }
        }

        private IListItemViewModel CreateFSItemVMFromString(string folderPath)
        {
            ////folderPath = System.IO.Path.GetDirectoryName(folderPath);

            string displayName = string.Empty;

            try
            {
                displayName = System.IO.Path.GetFileName(folderPath);
            }
            catch
            {
                displayName = folderPath;
            }

            if (displayName.Trim() == string.Empty)
                displayName = folderPath;

            return new ListItemViewModel(folderPath, FSItemType.Folder, displayName, true);
        }

        /// <summary>
        /// Method removes a folder bookmark from the list of currently bookmarked folders.
        /// </summary>
        /// <param name="param"></param>
        private void RemoveFolderBookmark_Executed(IListItemViewModel param)
        {
            this.RemoveFolder(param.GetModel);
        }
        #endregion methods
    }
}
