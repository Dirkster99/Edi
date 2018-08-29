namespace Edi.Settings.UserProfile
{
    using Edi.Settings.Interfaces;
    using FileSystemModels.Models;
    using MRULib.MRU.Models.Persist;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// This class implements the model of the user profile part
    /// of the application. Typically, users have implicit run-time
    /// settings that should be re-activated when the application
    /// is re-started at a later point in time (e.g. window size
    /// and position).
    /// 
    /// This class organizes these per user specific profile settings
    /// and is responsible for their storage (at program end) and
    /// retrieval at the start-up of the application.
    /// </summary>
    public class Profile : IProfile
    {
        #region fields
        private MRUList _MruList;

        private List<string> _FindHistoryList;
        private List<string> _ReplaceHistoryList;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public Profile()
        {
            // Set default session data
            MainWindowPosSz = new ViewPosSizeModel(100, 100, 1000, 700);

            IsWorkspaceAreaOptimized = false;

            LastActiveFile = string.Empty;

            _MruList = new MRUList();
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Get/set position and size of MainWindow
        /// </summary>
        [XmlElement(ElementName = "MainWindowPos")]
        public ViewPosSizeModel MainWindowPosSz { get; set; }

        /// <summary>
        /// Gets/sets whether the workspace area is optimized or not.
        /// The optimized workspace is distructive free and does not
        /// show optional stuff like toolbar and status bar.
        /// </summary>
        [XmlAttribute(AttributeName = "IsWorkspaceAreaOptimized")]
        public bool IsWorkspaceAreaOptimized { get; set; }

        /// <summary>
        /// Remember the last active path and name of last active document.
        /// 
        /// This can be useful when selecting active document in next session or
        /// determining a useful default path when there is no document currently open.
        /// </summary>
        [XmlAttribute(AttributeName = "LastActiveFile")]
        public string LastActiveFile { get; set; }

        /// <summary>
        /// List of most recently used files
        /// </summary>
        public MRUList MruList
        {
            get
            {
                if (_MruList == null)
                    _MruList = new MRUList();

                return _MruList;
            }

            set
            {
                if (_MruList != value)
                {
                    _MruList = value;
                }
            }
        }

        /// <summary>
        /// Get/set settings for explorer tool window.
        /// </summary>
        [XmlElement(ElementName = "LastActiveExplorer")]
        public ExplorerUserProfile LastActiveExplorer { get; set; }

        #region Find and Replace
        /// <summary>
        /// List of find history
        /// </summary>
        /// <returns>list of string</returns>
        public List<string> FindHistoryList
        {
            get
            {
                if (_FindHistoryList == null)
                    _FindHistoryList = new List<string>();

                return _FindHistoryList;
            }

            set
            {
                if (_FindHistoryList != value)
                    _FindHistoryList = value;
            }
        }

        /// <summary>
        /// List of replace history
        /// </summary>
        /// <returns>list of string</returns>
        public List<string> ReplaceHistoryList
        {
            get
            {
                if (_ReplaceHistoryList == null)
                    _ReplaceHistoryList = new List<string>();

                return _ReplaceHistoryList;
            }

            set
            {
                if (_ReplaceHistoryList != value)
                    _ReplaceHistoryList = value;
            }
        }
        #endregion Find and Replace
        #endregion properties

        #region methods
        /// <summary>
        /// Checks if current main window position settings are within
        /// the given bounds or not (and corrects them if not).
        /// </summary>
        /// <param name="SystemParameters_VirtualScreenLeft"></param>
        /// <param name="SystemParameters_VirtualScreenTop"></param>
        public void CheckSettingsOnLoad(double SystemParameters_VirtualScreenLeft,
                                        double SystemParameters_VirtualScreenTop)
        {
            if (MainWindowPosSz == null)
                MainWindowPosSz = new ViewPosSizeModel(100, 100, 600, 500);

            if (MainWindowPosSz.DefaultConstruct == true)
                MainWindowPosSz = new ViewPosSizeModel(100, 100, 600, 500);

            MainWindowPosSz.SetValidPos(SystemParameters_VirtualScreenLeft,
                                        SystemParameters_VirtualScreenTop);
        }

        /// <summary>
        /// Get the path of the last active file or empty string
        /// if file does not exists on disk.
        /// </summary>
        /// <returns></returns>
        public string GetLastActivePath()
        {
            try
            {
                if (System.IO.File.Exists(LastActiveFile))
                    return System.IO.Path.GetDirectoryName(LastActiveFile);
            }
            catch
            {
            }

            return string.Empty;
        }
        #endregion methods
    }
}
