namespace Files.ViewModels.RecentFiles
{
    using System;
    using Edi.Core.Interfaces.Enums;
    using MRULib.MRU.Interfaces;

    /// <summary>
    /// Implements a viewmodel that drives an AvalonDock toolwindow to display
    /// a list of recently accessed file items.
    /// </summary>
    public class RecentFilesTWViewModel : Edi.Core.ViewModels.ToolViewModel
    {
        #region fields
        /// <summary>
        /// Gets an Id for the toolwindow associated with this viewmodel.
        /// </summary>
        public const string ToolContentId = "<RecentFilesTool>";

        private readonly IMRUListViewModel _MruListViewModel;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public RecentFilesTWViewModel(IMRUListViewModel mruListViewModel)
            : base("Recent Files")
        {
            _MruListViewModel = mruListViewModel;

            ////Workspace.This.ActiveDocumentChanged += new EventHandler(OnActiveDocumentChanged);
            ContentId = ToolContentId;
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets the icon for this tool window's viewmodel.
        /// </summary>
        public override Uri IconSource
        {
            get
            {
                return new Uri("pack://application:,,,/Edi.Themes;component/Images/App/PinableListView/NoPin16.png", UriKind.RelativeOrAbsolute);
            }
        }

        /// <summary>
        /// Gets the MRU List managed in this tool window viewmodel.
        /// </summary>
        public IMRUListViewModel MruList
        {
            get
            {
                return _MruListViewModel;
            }
        }

        /// <summary>
        /// Gets the preferred panel location of this toolwindow.
        /// </summary>
        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Right; }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Adds a new MRU Entry into the available MRU entry implementation
        /// </summary>
        /// <param name="filePath"></param>
        public void AddNewEntryIntoMRU(string filePath)
        {
            if (this.MruList.UpdateEntry(filePath) == true)
                this.RaisePropertyChanged(() => this.MruList);
        }
        #endregion methods
    }
}
