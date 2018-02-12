namespace FileSystemModels.Models.FSItems.Base
{
    /// <summary>
    /// Determine whether a file system item is a folder or a file.
    /// </summary>
    public enum FSItemType
    {
        /// <summary>
        /// Uknown type of file system item.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Reference to harddisk or other drives such as 'C:\'
        /// </summary>
        LogicalDrive = 1,

        /// <summary>
        /// File system item is a folder.
        /// </summary>
        Folder = 2,

        /// <summary>
        /// File system item is a file.
        /// </summary>
        File = 3,

        ////DummyEntry = 4

        ////Computer = 4,
        ////NetworkShare = 5,
    }
}
