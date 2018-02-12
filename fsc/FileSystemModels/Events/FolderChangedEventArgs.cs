namespace FileSystemModels.Events
{
    using System;
    using FileSystemModels.Interfaces;

    /// <summary>
    /// Class implements an event that signals that
    /// event source (caller) would like to change to indicated path.
    /// </summary>
    public class FolderChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Event type class constructor from parameter
        /// </summary>
        public FolderChangedEventArgs(IPathModel path)
        : this()
        {
            this.Folder = path;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public FolderChangedEventArgs()
        : base()
        {
            this.Folder = null;
        }

        /// <summary>
        /// Path to this directory...
        /// </summary>
        public IPathModel Folder { get; private set; }
    }
}
