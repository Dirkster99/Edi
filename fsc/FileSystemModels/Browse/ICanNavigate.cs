namespace FileSystemModels.Browse
{
    using System;

    /// <summary>
    /// Defines an interface that should be exposed if a control can
    /// request the controller (and hence other controls) to browse to
    /// a new file system location.
    /// </summary>
    public interface ICanNavigate
    {
        /// <summary>
        /// Indicates when the viewmodel starts heading off somewhere else
        /// and when its done browsing to a new location.
        /// </summary>
        event EventHandler<BrowsingEventArgs> BrowseEvent;

        /// <summary>
        /// Can only be set by the control if user started browser process
        /// 
        /// Use IsBrowsing and IsExternallyBrowsing to lock the controls UI
        /// during browse operations or display appropriate progress bar(s).
        /// </summary>
        bool IsBrowsing { get; }
    }
}

