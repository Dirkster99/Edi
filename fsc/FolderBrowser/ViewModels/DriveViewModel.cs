namespace FolderBrowser.ViewModels
{
    using FileSystemModels;
    using FileSystemModels.Interfaces;
    using FileSystemModels.Models.FSItems.Base;
    using FolderBrowser.Interfaces;
    using System;
    using System.Threading.Tasks;

    internal class DriveViewModel : TreeItemViewModel, IDriveViewModel
    {
        #region fields
        private object _LockObject = new object();
        #endregion fields

        #region constructors
        /// <summary>
        /// Constructs a drive's viewmodel.
        /// </summary>
        public DriveViewModel(IPathModel model, ITreeItemViewModel parent)
           : base(model, parent)
        {
        }
        #endregion constructors

        #region methods
        /// <summary>
        /// Load all sub-folders into the Folders collection.
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
