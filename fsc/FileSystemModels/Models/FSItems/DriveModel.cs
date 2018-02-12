namespace FileSystemModels.Models.FSItems
{
    using Base;
    using FileSystemModels.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security;

    public class DriveModel : Base.FileSystemModel
    {
        #region constructors
        /// <summary>
        /// Parameterized class  constructor
        /// </summary>
        /// <param name="model"></param>
        [SecuritySafeCritical]
        public DriveModel(IPathModel model)
          : base(model)
        {
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Indicates the amount of available free space on a drive, in bytes
        /// or 0, if there is no space available or drive is not ready.
        /// </summary>
        public long AvailableFreeSpace
        {
            get
            {
                var drv = GetDriveInfo();

                if (drv != null)
                    return drv.AvailableFreeSpace;

                return 0;
            }
        }

        /// <summary>
        /// Gets the name of the file system, such as NTFS or FAT32,
        /// or string.Empty, if there is no space available or drive is not ready.
        /// </summary>
        public string DriveFormat
        {
            get
            {
                var drv = GetDriveInfo();

                if (drv != null)
                    return drv.DriveFormat;

                return string.Empty;
            }
        }

        ////    public DriveType DriveType
        ////    {
        ////      get
        ////      {
        ////        return this.mDrive.DriveType;
        ////      }
        ////    }

        /// <summary>
        /// Gets a true value indicating whether the drive root directory exists,
        /// or false, if there is no space available or drive is not ready.
        /// </summary>
        public bool Exists
        {
            get
            {
                var drv = GetDriveInfo();

                if (drv != null)
                    return drv.RootDirectory.Exists;

                return false;
            }
        }

        /// <summary>
        /// Gets a true value that indicates whether a drive is ready,
        /// or false, if there is no space available or drive is not ready.
        /// </summary>
        public bool IsReady
        {
            get
            {
                var drv = GetDriveInfo();

                if (drv != null)
                    return drv.IsReady;

                return false;
            }
        }

        /// <summary>
        /// Gets the total amount of free space available on a drive, in bytes,
        /// or 0, if there is no space available or drive is not ready.
        /// </summary>
        public long TotalFreeSpace
        {
            get
            {
                var drv = GetDriveInfo();

                if (drv != null)
                    return drv.TotalFreeSpace;

                return 0;
            }
        }

        /// <summary>
        /// Gets the total size of storage space on a drive, in bytes,
        /// or 0, if there is no space available or drive is not ready.
        /// </summary>
        public long TotalSize
        {
            get
            {
                var drv = GetDriveInfo();

                if (drv != null)
                    return drv.TotalSize;

                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the volume label of a drive,
        /// or string.Empty, if there is no space available or drive is not ready.
        /// </summary>
        public string VolumeLabel
        {
            get
            {
                var drv = GetDriveInfo();

                if (drv != null)
                    return drv.VolumeLabel;

                return string.Empty;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Gets all drives that are currently attached/registered on a given computer.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<FileSystemModel> GetLogicalDrives()
        {
            foreach (var item in Environment.GetLogicalDrives())
            {
                if (string.IsNullOrEmpty(item) == false)
                    yield return new DriveModel(new PathModel(item, FSItemType.LogicalDrive));
            }
        }

        private DriveInfo GetDriveInfo()
        {
            try
            {
                var drive = new DriveInfo(Model.Path);
                return drive;
            }
            catch
            {
            }

            return null;
        }
        #endregion methods
    }
}
