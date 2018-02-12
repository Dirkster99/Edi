namespace FolderBrowser.ViewModels
{
    using FileSystemModels;
    using FileSystemModels.Events;
    using FileSystemModels.Interfaces.Bookmark;
    using FileSystemModels.Models.FSItems.Base;
    using FileSystemModels.ViewModels.Base;
    using FolderBrowser.Interfaces;
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Class implements properties and methods required to message folder
    /// bookmark events to a related class which is responsible for viewing
    /// bookmarks (for example in a drop down button).
    /// </summary>
    internal class EditFolderBookmarks : IEditBookmarks
    {
        #region fields
        private ICommand _FolderAddCommand;
        private ICommand _FolderRemoveCommand;
        #endregion fields

        #region events
        /// <summary>
        /// Generate an event to remove or add a recent folder to a collection.
        /// </summary>
        public event EventHandler<EditBookmarkEvent> RequestEditBookmarkedFolders;
        #endregion events

        #region properties
        /// <summary>
        /// Determine whether an Add/Remove command can execute or not.
        /// </summary>
        public bool RecentFolderCommandCanExecute
        {
            get
            {
                // Command requires a registered event listner to be present
                if (this.RequestEditBookmarkedFolders != null)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Gets a command that removes folder location via a corresponding event.
        /// Expected parameter is an intherited type from <see cref="ITreeItemViewModel"/>.
        /// </summary>
        public ICommand RecentFolderRemoveCommand
        {
            get
            {
                if (this._FolderRemoveCommand == null)
                    this._FolderRemoveCommand = new RelayCommand<object>(
                        (p) => this.EditRecentFolder_Executed(
                            p as ITreeItemViewModel, EditBookmarkEvent.RecentFolderAction.Remove),

                        (p) => this.RecentFolderCommand_CanExecute(p));

                return this._FolderRemoveCommand;
            }
        }

        /// <summary>
        /// Gets a command that adds folder location via a corresponding event.
        /// Expected parameter is an intherited type from <see cref="ITreeItemViewModel"/>.
        /// </summary>
        public ICommand RecentFolderAddCommand
        {
            get
            {
                if (this._FolderAddCommand == null)
                    this._FolderAddCommand = new RelayCommand<object>(
                        (p) => this.EditRecentFolder_Executed(
                            p as ITreeItemViewModel, EditBookmarkEvent.RecentFolderAction.Add),

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
                var item = param as ITreeItemViewModel;

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
                ITreeItemViewModel item,
                EditBookmarkEvent.RecentFolderAction action)
        {
            if (item == null)
                return;

            // Tell client via event to get rid of this entry
            if (this.RequestEditBookmarkedFolders != null)
            {
                this.RequestEditBookmarkedFolders(this,
                    new EditBookmarkEvent(
                        PathFactory.Create(item.ItemPath, FSItemType.Folder), action));
            }
        }
        #endregion methods
    }
}
