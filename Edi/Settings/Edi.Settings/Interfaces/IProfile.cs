namespace Edi.Settings.Interfaces
{
    using Edi.Settings.UserProfile;
    using FileSystemModels.Models;
    using MRULib.MRU.Models.Persist;
    using System.Collections.Generic;

    /// <summary>
    /// This interface defines the API to the model of the user profile part
    /// of the application. Typically, users have implicit run-time
    /// settings that should be re-activated when the application
    /// is re-started at a later point in time (e.g. window size and position).
    /// 
    /// This interface organizes the per user specific profile settings
    /// and is responsible for their storage (at program end) and
    /// retrieval at the start-up of the application.
    /// </summary>
    public interface IProfile
	{
        #region properties
        /// <summary>
        /// Get/set position and size of MainWindow
        /// </summary>
        ViewPosSizeModel MainWindowPosSz { get; set; }

        /// <summary>
        /// Gets/sets whether the workspace area is optimized or not.
        /// The optimized workspace is distructive free and does not
        /// show optional stuff like toolbar and status bar.
        /// </summary>
        bool IsWorkspaceAreaOptimized { get; set; }

        /// <summary>
        /// Remember the last active path and name of last active document.
        /// 
        /// This can be useful when selecting active document in next session or
        /// determining a useful default path when there is no document currently open.
        /// </summary>
        string LastActiveFile { get; set; }

        /// <summary>
        /// List of most recently used files
        /// </summary>
        MRUList MruList { get; }

        /// <summary>
        /// Get/set settings for explorer tool window.
        /// </summary>
        ExplorerUserProfile LastActiveExplorer { get; set; }

        #region Find and Replace
        /// <summary>
        /// List of find history
        /// </summary>
        /// <returns>list of string</returns>
        List<string> FindHistoryList { get; }

        /// <summary>
        /// List of replace history
        /// </summary>
        /// <returns>list of string</returns>
        List<string> ReplaceHistoryList { get; }
        #endregion Find and Replace
        #endregion properties

        #region methods
        /// <summary>
        /// Checks if current main window position settings are within
        /// the given bounds or not (and corrects them if not).
        /// </summary>
        /// <param name="SystemParameters_VirtualScreenLeft"></param>
        /// <param name="SystemParameters_VirtualScreenTop"></param>
        void CheckSettingsOnLoad(double SystemParameters_VirtualScreenLeft,
                                 double SystemParameters_VirtualScreenTop);

        /// <summary>
        /// Get the path of the file or empty string if file does not exists on disk.
        /// </summary>
        /// <returns></returns>
        string GetLastActivePath();
        #endregion methods
    }
}
