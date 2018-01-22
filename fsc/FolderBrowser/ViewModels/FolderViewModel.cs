namespace FolderBrowser.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows.Input;
    using FileSystemModels.Models;
    using FolderBrowser.Command;
    using FolderBrowser.ViewModels.Interfaces;
    using InplaceEditBoxLib.ViewModels;
    using MsgBox;

    /// <summary>
    /// Implment the viewmodel for one folder entry for a collection of folders.
    /// </summary>
    public class FolderViewModel : EditInPlaceViewModel, IFolderViewModel
    {
        #region fields
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected new static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool mIsSelected;
        private bool mIsExpanded;

        private PathModel mModel;

        private ObservableCollection<IFolderViewModel> mFolders;

        private RelayCommand<object> mOpenInWindowsCommand;
        private RelayCommand<object> mCopyPathCommand;

        private string mVolumeLabel;

        private object mLockObject = new object();
        #endregion fields

        #region constructor
        /// <summary>
        /// Construct a folder viewmodel item from a path.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public FolderViewModel(PathModel model)
        {
            Logger.Debug("Detail: Construct FolderViewModel");

            this.mModel = new PathModel(model);
        }

        /// <summary>
        /// Standard <seealso cref="FolderViewModel"/> constructor
        /// </summary>
        public FolderViewModel()
          : base()
        {
            this.mIsExpanded = this.mIsSelected = false;

            this.mModel = null;

            this.mFolders = null;

            this.mOpenInWindowsCommand = null;
            this.mCopyPathCommand = null;

            this.mVolumeLabel = null;
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Gets the name of this folder (without its root path component).
        /// </summary>
        public string FolderName
        {
            get
            {
                if (this.mModel != null)
                    return this.mModel.Name;

                return string.Empty;
            }
        }


        /// <summary>
        /// Get/set file system Path for this folder.
        /// </summary>
        public string FolderPath
        {
            get
            {
                if (this.mModel != null)
                    return this.mModel.Path;

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets a folder item string for display purposes.
        /// This string can evaluete to 'C:\ (Windows)' for drives,
        /// if the 'C:\' drive was named 'Windows'.
        /// </summary>
        public string DisplayItemString
        {
            get
            {
                Logger.Debug("Detail: get DisplayItemString");

                switch (this.ItemType)
                {
                    case FSItemType.LogicalDrive:
                        try
                        {
                            if (this.mVolumeLabel == null)
                            {
                                DriveInfo di = new System.IO.DriveInfo(this.FolderName);

                                if (di.IsReady == true)
                                    this.mVolumeLabel = di.VolumeLabel;
                                else
                                    return string.Format("{0} ({1})", this.FolderName, FileSystemModels.Local.Strings.STR_MSG_DEVICE_NOT_READY);
                            }

                            return string.Format("{0} {1}", this.FolderName, (string.IsNullOrEmpty(this.mVolumeLabel)
                                                                                ? string.Empty
                                                                                : string.Format("({0})", this.mVolumeLabel)));
                        }
                        catch (Exception exp)
                        {
                            Logger.Error("DriveInfo cannot be optained for:" + this.FolderName, exp);

                            // Just return a folder name if everything else fails (drive may not be ready etc).
                            return string.Format("{0} ({1})", this.FolderName, exp.Message.Trim());
                        }

                    case FSItemType.Folder:
                    case FSItemType.Unknown:
                    default:
                        return this.FolderName;
                }
            }
        }

        /// <summary>
        /// Get/set observable collection of sub-folders of this folder.
        /// </summary>
        public ObservableCollection<IFolderViewModel> Folders
        {
            get
            {
                Logger.Debug("Detail: get Folders collection");

                if (this.mFolders == null)
                    this.mFolders = new ObservableCollection<IFolderViewModel>();

                return this.mFolders;
            }
        }

        /// <summary>
        /// Get/set whether this folder is currently selected or not.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.mIsSelected;
            }

            set
            {
                if (this.mIsSelected != value)
                {
                    Logger.Debug("Detail: set Folder IsSelected");

                    this.mIsSelected = value;

                    this.RaisePropertyChanged(() => this.IsSelected);

                    if (value == true)
                        this.IsExpanded = true;                 // Default windows behaviour of expanding the selected folder
                }
            }
        }

        /// <summary>
        /// Get/set whether this folder is currently expanded or not.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return this.mIsExpanded;
            }

            set
            {
                if (this.mIsExpanded != value)
                {
                    Logger.Debug("Detail: set Folder IsExpanded");

                    this.mIsExpanded = value;

                    this.RaisePropertyChanged(() => this.IsExpanded);

                    // Load all sub-folders into the Folders collection.
                    this.LoadFolders();
                }
            }
        }

        /// <summary>
        /// Gets the type of this item (eg: Folder, HardDisk etc...).
        /// </summary>
        public FSItemType ItemType
        {
            get
            {
                if (this.mModel != null)
                    return this.mModel.PathType;

                return FSItemType.Unknown;
            }
        }

        /// <summary>
        /// Gets a command that will open the selected item with the current default application
        /// in Windows. The selected item (path to a file) is expected as FSItemVM parameter.
        /// (eg: Item is HTML file -> Open in Windows starts the web browser for viewing the HTML
        /// file if thats the currently associated Windows default application.
        /// </summary>
        public ICommand OpenInWindowsCommand
        {
            get
            {
                Logger.Debug("Detail: get OpenInWindowsCommand");

                if (this.mOpenInWindowsCommand == null)
                    this.mOpenInWindowsCommand = new RelayCommand<object>(
                      (p) =>
                      {
                          var path = p as FolderViewModel;

                          if (path == null)
                              return;

                          if (string.IsNullOrEmpty(path.FolderPath) == true)
                              return;

                          FolderViewModel.OpenInWindowsCommand_Executed(path.FolderPath);
                      });

                return this.mOpenInWindowsCommand;
            }
        }

        /// <summary>
        /// Gets a command that will copy the path of an item into the Windows Clipboard.
        /// The item (path to a file) is expected as FSItemVM parameter.
        /// </summary>
        public ICommand CopyPathCommand
        {
            get
            {
                Logger.Debug("Detail: get CopyPathCommand");

                if (this.mCopyPathCommand == null)
                    this.mCopyPathCommand = new RelayCommand<object>(
                      (p) =>
                      {
                          var path = p as FolderViewModel;

                          if (path == null)
                              return;

                          if (string.IsNullOrEmpty(path.FolderPath) == true)
                              return;

                          FolderViewModel.CopyPathCommand_Executed(path.FolderPath);
                      });

                return this.mCopyPathCommand;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Construct a <seealso cref="FolderViewModel"/> item that represents a Windows
        /// file system drive object (eg.: 'C:\').
        /// </summary>
        /// <param name="driveLetter"></param>
        /// <returns></returns>
        public static FolderViewModel ConstructDriveFolderViewModel(string driveLetter)
        {
            Logger.Debug("Detail: Query Drives");

            try
            {
                FolderViewModel f = new FolderViewModel(new PathModel(driveLetter, FSItemType.LogicalDrive));

                return f;
            }
            catch
            {
            }

            return null;
        }

        /// <summary>
        /// Construct a <seealso cref="FolderViewModel"/> item that represents a Windows
        /// file system folder object (eg.: FolderPath = 'C:\Temp\', FolderName = 'Temp').
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static FolderViewModel ConstructFolderFolderViewModel(string dir)
        {
            Logger.Debug("Detail: get FolderViewModel");

            try
            {
                return new FolderViewModel(new PathModel(dir, FSItemType.Folder));
            }
            catch
            {
            }

            return null;
        }

        /// <summary>
        /// Rename the name of the folder into a new name.
        /// </summary>
        /// <param name="newFolderName"></param>
        public void RenameFolder(string newFolderName)
        {
            Logger.DebugFormat("Detail: Rename into new folder {0}:", newFolderName);

            lock (this.mLockObject)
            {
                try
                {
                    if (newFolderName != null)
                    {
                        PathModel sourceDir = new PathModel(this.FolderPath, FSItemType.Folder);
                        PathModel newFolderPath;

                        if (PathModel.RenameFileOrDirectory(sourceDir, newFolderName, out newFolderPath) == true)
                        {
                            this.ResetModel(newFolderPath);
                        }
                    }
                }
                catch (Exception exp)
                {
                    Logger.Error(string.Format("RenameFolder into '{0}' was not succesful.", newFolderName), exp);

                    base.ShowNotification(FileSystemModels.Local.Strings.STR_RenameFolderErrorTitle, exp.Message);
                }
                finally
                {
                    this.RaisePropertyChanged(() => this.FolderName);
                    this.RaisePropertyChanged(() => this.FolderPath);
                    this.RaisePropertyChanged(() => this.DisplayItemString);
                }
            }
        }

        /// <summary>
        /// Create a new folder with a standard name
        /// 'New folder n' underneath this folder.
        /// </summary>
        /// <returns>a viewmodel of the newly created directory or null</returns>
        public IFolderViewModel CreateNewDirectory()
        {
            Logger.DebugFormat("Detail: Create new directory with standard name.");

            lock (this.mLockObject)
            {
                try
                {
                    var newSubFolder = PathModel.CreateDir(new PathModel(this.FolderPath, FSItemType.Folder));

                    if (newSubFolder != null)
                    {
                        var newFolder = new FolderViewModel(newSubFolder);
                        this.Folders.Add(newFolder);

                        return newFolder;
                    }
                }
                catch (Exception exp)
                {
                    Logger.Error(string.Format("Creating new folder underneath '{0}' was not succesful.", this.FolderPath), exp);

                    base.ShowNotification(FileSystemModels.Local.Strings.STR_CREATE_FOLDER_ERROR_TITLE, exp.Message);
                }
            }

            return null;
        }

        #region FileSystem Commands
        /// <summary>
        /// Convinience method to open Windows Explorer with a selected file (if it exists).
        /// Otherwise, Windows Explorer is opened in the location where the file should be at.
        /// </summary>
        /// <param name="sFileName"></param>
        /// <returns></returns>
        private static bool OpenContainingFolderCommand_Executed(string sFileName)
        {
            if (string.IsNullOrEmpty(sFileName) == true)
                return false;

            // XXX TODO Add this back in
            //var msg = ServiceLocator.ServiceContainer.Instance.GetService<IMessageBoxService>();
            try
            {
                if (System.IO.File.Exists(sFileName) == true)
                {
                    // combine the arguments together it doesn't matter if there is a space after ','
                    string argument = @"/select, " + sFileName;

                    System.Diagnostics.Process.Start("explorer.exe", argument);
                    return true;
                }
                else
                {
                    string sParentDir = string.Empty;

                    if (System.IO.Directory.Exists(sFileName) == true)
                        sParentDir = sFileName;
                    else
                        sParentDir = System.IO.Directory.GetParent(sFileName).FullName;

                    if (System.IO.Directory.Exists(sParentDir) == false)
                    {
////                        msg.Show(string.Format(FileSystemModels.Local.Strings.STR_MSG_DIRECTORY_DOES_NOT_EXIST, sParentDir),
////                                               FileSystemModels.Local.Strings.STR_MSG_ERROR_FINDING_RESOURCE,
////                                               MsgBoxButtons.OK, MsgBoxImage.Error);
////
                        return false;
                    }
                    else
                    {
                        // combine the arguments together it doesn't matter if there is a space after ','
                        string argument = @"/select, " + sParentDir;
                        System.Diagnostics.Process.Start("explorer.exe", argument);

                        return true;
                    }
                }
            }
            catch (System.Exception ex)
            {
////                msg.Show(string.Format("{0}\n'{1}'.", ex.Message, (sFileName == null ? string.Empty : sFileName)),
////                         FileSystemModels.Local.Strings.STR_MSG_ERROR_FINDING_RESOURCE,
////                         MsgBoxButtons.OK, MsgBoxImage.Error);

                return false;
            }
        }

        /// <summary>
        /// Opens a file with the current Windows default application.
        /// </summary>
        /// <param name="sFileName"></param>
        private static void OpenInWindowsCommand_Executed(string sFileName)
        {
            if (string.IsNullOrEmpty(sFileName) == true)
                return;

            // XXX TODO Add this back in
            //var msg = ServiceLocator.ServiceContainer.Instance.GetService<IMessageBoxService>();
            try
            {
                Process.Start(new ProcessStartInfo(sFileName));
            }
            catch (System.Exception ex)
            {
////                msg.Show(string.Format(CultureInfo.CurrentCulture, "{0}", ex.Message),
////                         FileSystemModels.Local.Strings.STR_MSG_ERROR_FINDING_RESOURCE,
////                         MsgBoxButtons.OK, MsgBoxImage.Error);
            }
        }

        /// <summary>
        /// Copies the given string into the Windows clipboard.
        /// </summary>
        /// <param name="sFileName"></param>
        private static void CopyPathCommand_Executed(string sFileName)
        {
            if (string.IsNullOrEmpty(sFileName) == true)
                return;

            try
            {
                System.Windows.Clipboard.SetText(sFileName);
            }
            catch
            {
            }
        }
        #endregion FileSystem Commands

        /// <summary>
        /// Load all sub-folders into the Folders collection.
        /// </summary>
        private void LoadFolders()
        {
            Logger.DebugFormat("Detail: Load sub-folders of this folder.");

            try
            {
                if (this.Folders.Count > 0)
                    return;

                string[] dirs = null;

                string fullPath = Path.Combine(this.FolderPath, this.FolderName);

                if (this.FolderName.Contains(':'))                  // This is a drive
                    fullPath = string.Concat(this.FolderName, "\\");
                else
                    fullPath = this.FolderPath;

                try
                {
                    dirs = Directory.GetDirectories(fullPath);
                }
                catch (Exception)
                {
                }

                this.Folders.Clear();

                if (dirs != null)
                {
                    foreach (string dir in dirs)
                        AddFolder(dir);
                }
            }
            catch (UnauthorizedAccessException ae)
            {
                Console.WriteLine(ae.Message);
            }
            catch (IOException ie)
            {
                Console.WriteLine(ie.Message);
            }
        }

        /// <summary>
        /// Add a new folder indicated by <paramref name="dir"/> as path
        /// into the sub-folder viewmodel collection of this folder item.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private FolderViewModel AddFolder(string dir)
        {
            Logger.DebugFormat("Detail: AddFolder '{0}' into sub-folder collection of this folder.", dir);

            try
            {
                DirectoryInfo di = new DirectoryInfo(dir);

                // create the sub-structure only if this is not a hidden directory
                if ((di.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    var newFolder = FolderViewModel.ConstructFolderFolderViewModel(dir);
                    this.Folders.Add(newFolder);

                    return newFolder;
                }
            }
            catch (UnauthorizedAccessException ae)
            {
                Logger.Warn("Directory Access not authorized", ae);
            }
            catch (Exception e)
            {
                Logger.Warn(e);
            }

            return null;
        }

        private void ResetModel(PathModel model)
        {
            var oldModel = this.mModel;

            this.mModel = new PathModel(model);

            if (oldModel == null && model == null)
                return;

            if (oldModel == null && model != null || oldModel != null && model == null)
            {
                this.RaisePropertyChanged(() => this.ItemType);
                this.RaisePropertyChanged(() => this.FolderName);
                this.RaisePropertyChanged(() => this.FolderPath);

                return;
            }

            if (oldModel.PathType != this.mModel.PathType)
                this.RaisePropertyChanged(() => this.ItemType);

            if (string.Compare(oldModel.Name, this.mModel.Name, true) != 0)
                this.RaisePropertyChanged(() => this.FolderName);

            if (string.Compare(oldModel.Path, this.mModel.Path, true) != 0)
                this.RaisePropertyChanged(() => this.FolderPath);
        }
        #endregion methods
    }
}
