namespace Files.ViewModels.FileExplorer
{
	using System;
	using System.IO;
	using System.Windows;
	using System.Windows.Input;
	using Edi.Core.Interfaces;
	using Edi.Core.Interfaces.Enums;
	using Edi.Core.Models;
	using Edi.Core.ViewModels;
	using FileListView.Command;
	using FileListView.ViewModels;
	using FileListView.ViewModels.Interfaces;
	using FileSystemModels.Interfaces;
	using FileSystemModels.Models;
	using FolderBrowser.ViewModels;
	using FolderBrowser.ViewModels.Interfaces;
	using Edi.Settings.Interfaces;

	/// <summary>
	/// This class can be used to present file based information, such as,
	/// Size, Path etc to the user.
	/// </summary>
	public class FileExplorerViewModel : Edi.Core.ViewModels.ToolViewModel,
																			 IRegisterableToolWindow,
																			 IExplorer
	{
		#region fields
		public const string ToolContentId = "<FileExplorerTool>";
		private string mFilePathName = string.Empty;

		private Func<string, bool> mFileOpenMethod = null;

		private RelayCommand<object> mSyncPathWithCurrentDocumentCommand = null;

		private IDocumentParent mParent = null;

		private readonly IFileOpenService mFileOpenService;
		#endregion fields

		#region constructor
		/// <summary>
		/// Class constructor
		/// </summary>
		public FileExplorerViewModel(ISettingsManager programSettings,
																 IFileOpenService fileOpenService)
			: base("Explorer")
		{
			base.ContentId = ToolContentId;

			this.mFileOpenService = fileOpenService;

			this.OnActiveDocumentChanged(null, null);

			this.FolderView = new FolderListViewModel(this.FolderItemsView_OnFileOpen);

			this.SynchronizedTreeBrowser = new BrowserViewModel();
			this.SynchronizedTreeBrowser.SetSpecialFoldersVisibility(false);

			// This must be done before calling configure since browser viewmodel is otherwise not available
			this.FolderView.AttachFolderBrowser(this.SynchronizedTreeBrowser);

			ExplorerSettingsModel settings = null;

			if (programSettings != null)
			{
				if (programSettings.SessionData != null)
				{
					settings = programSettings.SettingData.ExplorerSettings;
				}
			}

			if (settings == null)
				settings = new ExplorerSettingsModel();

			if (programSettings.SessionData.LastActiveExplorer != null)
				settings.UserProfile = programSettings.SessionData.LastActiveExplorer;
			else
				settings.UserProfile.SetCurrentPath(@"C:");

			this.FolderView.ConfigureExplorerSettings(settings);
			this.mFileOpenMethod = this.mFileOpenService.FileOpen;
		}
		#endregion constructor

		#region properties
		/// <summary>
		/// Expose a viewmodel that controls the combobox folder drop down
		/// and the fodler/file list view.
		/// </summary>
		public IFolderListViewModel FolderView { get; set; }

		/// <summary>
		/// Gets an interface instance used for setting/getting settings of the Explorer (TW).
		/// </summary>
		public IConfigExplorerSettings Settings
		{
			get
			{
				if (this.FolderView == null)
					return null;

				return this.FolderView;
			}
		}

		/// <summary>
		/// Gets the viewmodel that drives the folder picker control.
		/// </summary>
		public IBrowserViewModel SynchronizedTreeBrowser { get; private set; }

		#region FileName
		public string FileName
		{
			get
			{
				if (string.IsNullOrEmpty(this.mFilePathName) == true)
					return string.Empty;

				try
				{
					return System.IO.Path.GetFileName(mFilePathName);
				}
				catch (Exception)
				{
					return string.Empty;
				}
			}
		}
		#endregion

		#region FilePath
		public string FilePath
		{
			get
			{
				if (string.IsNullOrEmpty(this.mFilePathName) == true)
					return string.Empty;

				try
				{
					return System.IO.Path.GetDirectoryName(mFilePathName);
				}
				catch (Exception)
				{
					return string.Empty;
				}
			}
		}
		#endregion

		#region ToolWindow Icon
		public override Uri IconSource
		{
			get
			{
				return new Uri("pack://application:,,,/FileListView;component/Images/Generic/Folder/folderopened_yellow_16.png", UriKind.RelativeOrAbsolute);
			}
		}
		#endregion ToolWindow Icon

		#region Commands
		/// <summary>
		/// Can be executed to synchronize the current path with the currently active document.
		/// </summary>
		public ICommand SyncPathWithCurrentDocumentCommand
		{
			get
			{
				if (this.mSyncPathWithCurrentDocumentCommand == null)
					this.mSyncPathWithCurrentDocumentCommand = new RelayCommand<object>(
						(p) => this.SyncPathWithCurrentDocumentCommand_Executed(),
						(p) => string.IsNullOrEmpty(this.mFilePathName) == false);

				return this.mSyncPathWithCurrentDocumentCommand;
			}
		}
		#endregion Commands

		public ExplorerSettingsModel GetExplorerSettings(ExplorerSettingsModel input)
		{
			return this.Settings.GetExplorerSettings(input);
		}

		public override PaneLocation PreferredLocation
		{
			get { return PaneLocation.Right; }
		}
		#endregion properties

		#region methods
		/// <summary>
		/// Save the current user profile settings into the
		/// corresponding property of the SettingsManager.
		/// </summary>
		/// <param name="settingsManager"></param>
		/// <param name="vm"></param>
		public static void SaveSettings(ISettingsManager settingsManager,
																		IExplorer vm)
		{
			var settings = vm.GetExplorerSettings(settingsManager.SettingData.ExplorerSettings);

			if (settings != null) // Explorer settings have changed
			{
				settingsManager.SettingData.IsDirty = true;
				settingsManager.SettingData.ExplorerSettings = settings;

				settingsManager.SessionData.LastActiveExplorer = settings.UserProfile;
			}
			else
				settingsManager.SessionData.LastActiveExplorer = vm.GetExplorerSettings(null).UserProfile;
		}

		/// <summary>
		/// Load Explorer (Tool Window) seetings from persistence.
		/// </summary>
		/// <param name="settingsManager"></param>
		/// <param name="vm"></param>
		public static void LoadSettings(ISettingsManager settingsManager,
																		FileExplorerViewModel vm)
		{
			ExplorerSettingsModel settings = null;

			settings = settingsManager.SettingData.ExplorerSettings;

			if (settings == null)
				settings = new ExplorerSettingsModel();

			settings.UserProfile = settingsManager.SessionData.LastActiveExplorer;

			// (re-)configure previous explorer settings and (re-)activate current location
			vm.Settings.ConfigureExplorerSettings(settings);
		}

		#region IRegisterableToolWindow
		/// <summary>
		/// Set the document parent handling object to deactivation and activation
		/// of documents with content relevant to this tool window viewmodel.
		/// </summary>
		/// <param name="parent"></param>
		public void SetDocumentParent(IDocumentParent parent)
		{
			if (parent != null)
				parent.ActiveDocumentChanged -= this.OnActiveDocumentChanged;

			this.mParent = parent;

			// Check if active document is a log4net document to display data for...
			if (this.mParent != null)
				parent.ActiveDocumentChanged += new DocumentChangedEventHandler(this.OnActiveDocumentChanged);
			else
				this.OnActiveDocumentChanged(null, null);
		}

		/// <summary>
		/// Set the document parent handling object and visibility
		/// to enable tool window to react on deactivation and activation
		/// of documents with content relevant to this tool window viewmodel.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="isVisible"></param>
		public void SetToolWindowVisibility(IDocumentParent parent,
																				bool isVisible = true)
		{
			if (IsVisible == true)
				this.SetDocumentParent(parent);
			else
				this.SetDocumentParent(null);

			base.SetToolWindowVisibility(isVisible);
		}
		#endregion IRegisterableToolWindow

		public void OnActiveDocumentChanged(object sender, DocumentChangedEventArgs e)
		{
			this.mFilePathName = string.Empty;

			if (e != null)
			{
				if (e.ActiveDocument != null)
				{

                    if (e.ActiveDocument is FileBaseViewModel)
                    {
                        FileBaseViewModel f = e.ActiveDocument as FileBaseViewModel;

                        if (File.Exists(f.FilePath) == true)
                        {
                            var fi = new FileInfo(f.FilePath);

                            this.mFilePathName = f.FilePath;

                            this.RaisePropertyChanged(() => this.FileName);
                            this.RaisePropertyChanged(() => this.FilePath);
                        }
                    }
                }
			}
		}

		/// <summary>
		/// Executes when the file open event is fired and class was constructed with statndard constructor.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void FolderItemsView_OnFileOpen(object sender, FileSystemModels.Events.FileOpenEventArgs e)
		{
			if (this.mFileOpenMethod != null)
				this.mFileOpenMethod(e.FileName);
			else
				MessageBox.Show("File Open (method is to null):" + e.FileName);
		}

		/// <summary>
		/// Navigates to viewmodel to the <paramref name="directoryPath"/> folder.
		/// </summary>
		/// <param name="directoryPath"></param>
		public void NavigateToFolder(string directoryPath)
		{
			try
			{
				if (System.IO.Directory.Exists(directoryPath) == false)
					directoryPath = System.IO.Directory.GetParent(directoryPath).FullName;

				if (System.IO.Directory.Exists(directoryPath) == false)
					return;
			}
			catch
			{
			}

			this.FolderView.NavigateToFolder(directoryPath);
		}

		private void SyncPathWithCurrentDocumentCommand_Executed()
		{
			if (string.IsNullOrEmpty(this.mFilePathName) == true)
				return;

			NavigateToFolder(this.mFilePathName);
		}
		#endregion methods
	}
}
