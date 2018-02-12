namespace FolderBrowser.ViewModels
{
    using FileSystemModels;
    using FileSystemModels.Interfaces;
    using FileSystemModels.Models.FSItems.Base;
    using FolderBrowser.Interfaces;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Implment the viewmodel for one folder entry for a collection of folders.
    /// </summary>
    internal class FolderViewModel : TreeItemViewModel
    {
        #region fields
        private object _LockObject = new object();
        #endregion fields

        #region constructor
        /// <summary>
        /// Construct a folder viewmodel item from a path.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public FolderViewModel(IPathModel model, ITreeItemViewModel parent)
            : base(model, parent)
        {
        }

        /// <summary>
        /// Construct a <seealso cref="FolderViewModel"/> item that represents a Windows
        /// file system folder object (eg.: FolderPath = 'C:\Temp\', FolderName = 'Temp').
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        internal FolderViewModel(string dir, ITreeItemViewModel parent)
           : this(PathFactory.Create(dir, FSItemType.Folder), parent)
        {
        }

        /// <summary>
        /// Standard <seealso cref="FolderViewModel"/> constructor
        /// </summary>
        protected FolderViewModel()
          : base()
        {
        }
        #endregion constructor

        #region methods
        /// <summary>
        /// Load all sub-folders into this Folders collection.
        /// </summary>
        public override void ChildrenLoad()
        {
            FolderViewModel.LoadFolders(this);
        }

        public override async Task<int> ChildrenLoadAsync()
        {
            await Task.Run(() => { FolderViewModel.LoadFolders(this); });

            return base.ChildrenCount;
        }

        /// <summary>
        /// Load all sub-folders into the Folders collection of the
        /// given <paramref name="parentItem"/>.
        /// </summary>
        /// <param name="parentItem"></param>
        internal static void LoadFolders(ITreeItemViewModel parentItem)
        {
            try
            {
                parentItem.ChildrenClear();

                foreach (string dir in Directory.GetDirectories(parentItem.ItemPath))
                {
                    try
                    {
                        FolderViewModel.AddFolder(dir, parentItem);
                    }
                    catch (UnauthorizedAccessException ae)
                    {
                        parentItem.ShowNotification(FileSystemModels.Local.Strings.STR_MSG_UnknownError, ae.Message);
                    }
                    catch (IOException ie)
                    {
                        parentItem.ShowNotification(FileSystemModels.Local.Strings.STR_MSG_UnknownError, ie.Message);
                    }
                }
            }
            catch (UnauthorizedAccessException ae)
            {
                parentItem.ShowNotification(FileSystemModels.Local.Strings.STR_MSG_UnknownError, ae.Message);
            }
            catch (IOException ie)
            {
                parentItem.ShowNotification(FileSystemModels.Local.Strings.STR_MSG_UnknownError, ie.Message);
            }
        }

        /// <summary>
        /// Add a new folder indicated by <paramref name="dir"/> as path
        /// into the sub-folder viewmodel collection of this folder item.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        internal static TreeItemViewModel AddFolder(string dir,
                                                    ITreeItemViewModel parentItem)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(dir);

                // create the sub-structure only if this is not a hidden directory
//                if ((di.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
//                {
                    var newFolder = new FolderViewModel(dir, parentItem);

                    parentItem.ChildAdd(newFolder);

                    return newFolder;
//                }
            }
            catch (UnauthorizedAccessException ae)
            {
                parentItem.ShowNotification(FileSystemModels.Local.Strings.STR_MSG_UnknownError, ae.Message);
            }
            catch (Exception e)
            {
                parentItem.ShowNotification(FileSystemModels.Local.Strings.STR_MSG_UnknownError, e.Message);
            }

            return null;
        }

        /// <summary>
        /// Create a new folder with a standard name
        /// 'New folder n' underneath this folder.
        /// </summary>
        /// <returns>a viewmodel of the newly created directory or null</returns>
        public override ITreeItemViewModel CreateNewDirectory()
        {
            Logger.DebugFormat("Detail: Create new directory with standard name.");

            lock (_LockObject)
            {
                try
                {
                    string defaultFolderName = FileSystemModels.Local.Strings.STR_NEW_DEFAULT_FOLDER_NAME;
                    var model = PathFactory.Create(ItemPath, FSItemType.Folder);
                    var newSubFolder = PathFactory.CreateDir(model, defaultFolderName);

                    if (newSubFolder != null)
                    {
                        var item = new FolderViewModel(newSubFolder, this);
                        ChildAdd(item);
                        return item;
                    }
                }
                catch (Exception exp)
                {
                    this.ShowNotification(FileSystemModels.Local.Strings.STR_MSG_UnknownError, exp.Message);
                }
            }

            return null;
        }
        #endregion methods
    }
}
