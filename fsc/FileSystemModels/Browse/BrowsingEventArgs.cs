namespace FileSystemModels.Browse
{
    using System;
    using FileSystemModels.Interfaces;

    /// <summary>
    /// A simple event based state model that informs the subscriber about the
    /// state of the browser when changing between locations.
    /// </summary>
    public class BrowsingEventArgs : EventArgs
    {
        /// <summary>
        /// Event type class constructor from parameter
        /// </summary>
        public BrowsingEventArgs(IPathModel location,
                                 bool isBrowsing,
                                 BrowseResult result = BrowseResult.Unknown)
        : this()
        {
            Location = location;
            IsBrowsing = isBrowsing;
            Result = result;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        protected BrowsingEventArgs()
        : base()
        {
            this.Location = null;
            this.IsBrowsing = false;
        }

        /// <summary>
        /// Path we are browsing to or being arrived at.
        /// </summary>
        public IPathModel Location { get; private set; }

        /// <summary>
        /// Determines if we are:
        /// 1) Currently browsing towards this path (display progress) or
        /// 2) if the browsing process has arrived at this location (finish progress display).
        /// </summary>
        public bool IsBrowsing { get; private set; }

        /// <summary>
        /// Gets the result of the browsing process. This should be Unknown
        /// if <see cref="IsBrowsing"/> is true and either Complete or InComplete
        /// in all other cases.
        /// </summary>
        public BrowseResult Result { get; private set; }
    }
}
