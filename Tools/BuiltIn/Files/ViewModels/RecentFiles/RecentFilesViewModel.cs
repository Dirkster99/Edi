namespace Files.ViewModels.RecentFiles
{
    using System;
    using Edi.Core.Interfaces.Enums;
    using MRULib.MRU.Interfaces;
    using Microsoft.Practices.ServiceLocation;

    public class RecentFilesViewModel : Edi.Core.ViewModels.ToolViewModel
    {
        #region fields
        public const string ToolContentId = "<RecentFilesTool>";
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public RecentFilesViewModel()
            : base("Recent Files")
        {
            ////Workspace.This.ActiveDocumentChanged += new EventHandler(OnActiveDocumentChanged);
            ContentId = ToolContentId;
        }
        #endregion constructors

        #region properties
        public override Uri IconSource
        {
            get
            {
                return new Uri("pack://application:,,,/Edi.Themes;component/Images/App/PinableListView/NoPin16.png", UriKind.RelativeOrAbsolute);
            }
        }

        /***
				void OnActiveDocumentChanged(object sender, EventArgs e)
				{
					if (Workspace.This.ActiveDocument != null &&
							Workspace.This.ActiveDocument.FilePath != null &&
							File.Exists(Workspace.This.ActiveDocument.FilePath))
					{
						var fi = new FileInfo(Workspace.This.ActiveDocument.FilePath);
						FileSize = fi.Length;
						LastModified = fi.LastWriteTime;
					}
					else
					{
						FileSize = 0;
						LastModified = DateTime.MinValue;
					}
				}
		***/

        public IMRUListViewModel MruList
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IMRUListViewModel>();
            }
            /***
						private set
						{
							if (Workspace.This.Config.MruList != value)
							{
								Workspace.This.Config.MruList = value;
								this.NotifyPropertyChanged(() => this.MruList);
							}
						}
			 ***/
        }

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
