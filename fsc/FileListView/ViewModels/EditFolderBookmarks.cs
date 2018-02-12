namespace FileListView.ViewModels
{
    using FileListView.ViewModels.Base;
    using FileSystemModels;
    using FileSystemModels.Events;
    using FileSystemModels.Interfaces.Bookmark;
    using FileSystemModels.Models.FSItems.Base;
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Implements to Add/Remove Bookmark entries from
    /// a list of bookmarked items.
    /// 
    /// The object should be implenented by any client object that wants to
    /// add or remove entries from a bookmark locations model.
    /// </summary>
    internal class EditFolderBookmarks : IEditBookmarks
    {
        #region fields
        private ICommand _FolderAddCommand;
        private ICommand _FolderRemoveCommand;
        #endregion fields

        /// <summary>
        /// Invokes the actual event that adds/removes a bookmark in the bookmark
        /// model collection at the client side of this event.
        /// </summary>
        public event EventHandler<EditBookmarkEvent> RequestEditBookmarkedFolders;

        #region properties
        /// <summary>
        /// Gets a command that removes folder location via a corresponding event.
        /// Expected parameter is an intherited type from <see cref="LVItemViewModel"/>.
        /// </summary>
        public ICommand RecentFolderRemoveCommand
        {
            get
            {
                if (this._FolderRemoveCommand == null)
                    this._FolderRemoveCommand = new RelayCommand<object>(
                        (p) => this.EditRecentFolder_Executed(
                            p as LVItemViewModel, EditBookmarkEvent.RecentFolderAction.Remove),

                        (p) => this.RecentFolderCommand_CanExecute(p));

                return this._FolderRemoveCommand;
            }
        }

        /// <summary>
        /// Gets a command that adds folder location via a corresponding event.
        /// Expected parameter is an intherited type from <see cref="LVItemViewModel"/>.
        /// </summary>
        public ICommand RecentFolderAddCommand
        {
            get
            {
                if (this._FolderAddCommand == null)
                    this._FolderAddCommand = new RelayCommand<object>(
                        (p) => this.EditRecentFolder_Executed(
                            p as LVItemViewModel, EditBookmarkEvent.RecentFolderAction.Add),

                        (p) => this.RecentFolderCommand_CanExecute(p));

                return this._FolderAddCommand;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Determines whether the add/remove bookmark command can execute.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private bool RecentFolderCommand_CanExecute(object param)
        {
            if (this.RequestEditBookmarkedFolders != null)
            {
                var item = param as LVItemViewModel;

                if (item != null)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Adds or removes the <paramref name="item"/> from the bookmarks collection
        /// at thr receivers (subscriber) end of the event chain.
        /// 
        /// <see cref="RequestEditBookmarkedFolders"/> event.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="action"></param>
        private void EditRecentFolder_Executed(
                LVItemViewModel item,
                EditBookmarkEvent.RecentFolderAction action)
        {
            if (item == null)
                return;

            // Tell client via event to get rid of this entry
            if (this.RequestEditBookmarkedFolders != null)
            {
                this.RequestEditBookmarkedFolders(this,
                    new EditBookmarkEvent(
                        PathFactory.Create(item.FullPath, FSItemType.Folder), action));
            }
        }
        #endregion methods
    }
}
