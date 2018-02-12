namespace FileSystemModels.Interfaces.Bookmark
{
    using FileSystemModels.Events;
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Defines an interface to Add/Remove Bookmark entries from
    /// a list of bookmarked items.
    /// 
    /// The object should be implenented by any client objects that wants to
    /// add or remove entries from a bookmark locations model.
    /// </summary>
    public interface IEditBookmarks
    {
        /// <summary>
        /// Invokes the actual event that adds/removes a bookmark in the bookmark
        /// model collection at the client side of this event.
        /// </summary>
        event EventHandler<EditBookmarkEvent> RequestEditBookmarkedFolders;

        /// <summary>
        /// Gets a command that removes folder location via a corresponding event.
        /// </summary>
        ICommand RecentFolderRemoveCommand { get; }

        /// <summary>
        /// Gets a command that adds folder location via a corresponding event.
        /// </summary>
        ICommand RecentFolderAddCommand { get; }
    }
}
