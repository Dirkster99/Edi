namespace FileSystemModels.Events
{
    using FileSystemModels.Interfaces;
    using System;

    /// <summary>
    /// Implements an event for messaging to a bookmark collection
    /// whether a bookmarked entry should be added or removed ...
    /// from the collection.
    /// </summary>
    public class EditBookmarkEvent : EventArgs
    {
        #region constructor
        /// <summary>
        /// Event type class constructor from parameter
        /// </summary>
        public EditBookmarkEvent(IPathModel path,
                                 RecentFolderAction action = RecentFolderAction.Add)
        : this()
        {
            this.Folder = path;
            this.Action = action;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public EditBookmarkEvent()
        : base()
        {
            this.Folder = null;
            this.Action = RecentFolderAction.Add;
        }
        #endregion constructor

        #region enums
        /// <summary>
        /// Enumeration to define an action on recent folders.
        /// </summary>
        public enum RecentFolderAction
        {
            /// <summary>
            /// Remove a folder from the current collection.
            /// </summary>
            Remove = 0,

            /// <summary>
            /// Add a folder into the current collection.
            /// </summary>
            Add = 1
        }
        #endregion enums

        #region properties
        /// <summary>
        /// Path to this directory...
        /// </summary>
        public IPathModel Folder { get; set; }

        /// <summary>
        /// Gets/sets the type of recent folder action (eg Add/Remove).
        /// </summary>
        public RecentFolderAction Action { get; set; }
        #endregion properties
    }
}
