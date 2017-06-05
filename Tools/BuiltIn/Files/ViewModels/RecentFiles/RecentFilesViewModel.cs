namespace Files.ViewModels.RecentFiles
{
	using System;
	using Edi.Core.Interfaces.Enums;
	using Settings.Interfaces;
	using SimpleControls.MRU.ViewModel;

	public class RecentFilesViewModel : Edi.Core.ViewModels.ToolViewModel
	{
		#region fields
		public const string ToolContentId = "<RecentFilesTool>";

		private MRUListVM mMruList = null;
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

		public RecentFilesViewModel(ISettingsManager settingsManager)
			: this()
		{
			this.mMruList = settingsManager.SessionData.MruList;
		}
		#endregion constructors

		#region properties
		public override Uri IconSource
		{
			get
			{
				return new Uri("pack://application:,,,/Themes;component/Images/App/PinableListView/NoPin16.png", UriKind.RelativeOrAbsolute);
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

		public MRUListVM MruList
		{
			get
			{
				return this.mMruList;
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
		public void AddNewEntryIntoMRU(string filePath)
		{
			if (this.MruList.FindMRUEntry(filePath) == null)
			{
				MRUEntryVM e = new MRUEntryVM() { IsPinned = false, PathFileName = filePath };

				this.MruList.AddMRUEntry(e);

				this.RaisePropertyChanged(() => this.MruList);
			}
		}
		#endregion methods
	}
}
