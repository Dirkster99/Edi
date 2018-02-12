namespace FileSystemModels
{
    using FileSystemModels.Interfaces;
    using FileSystemModels.Models.FSItems.Base;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Class implements base services for working with the <see ref="IPathModel">
    /// interface and its associated methods.
    /// </summary>
    public sealed class PathFactory
    {
        private PathFactory(){}

        /// <summary>
        /// Constructs a new <seealso cref="IPathModel"/> object and returns it.
        /// </summary>
        public static IPathModel Create(string path, FSItemType itemType)
        {
            return new PathModel(path, itemType);
        }

        /// <summary>
        /// Compare the paths for 2 <see cref="PathModel"/> objects
        /// and return false if they are not equal, otherwise true.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="m1"></param>
        /// <returns></returns>
        public static bool Compare(IPathModel m, IPathModel m1)
        {
            return PathModel.Compare(m, m1);
        }

        /// <summary>
        /// Compare 2 <see cref="string"/> objects that represent a path
        /// and returns false if they are equal.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="path1"></param>
        /// <returns></returns>
        public static bool Compare(string path, string path1)
        {
            return PathModel.Compare(path, path1);
        }

        /// <summary>
        /// Check whether a string has basic properties that
        /// (not null, at least 2 characters) it could contain
        /// a path reference.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CheckValidString(string path)
        {
            return PathModel.CheckValidString(path);
        }

        /// <summary>
        /// Make sure that a path reference does actually work with
        /// <see cref="System.IO.DirectoryInfo"/> by replacing 'C:' by 'C:\'.
        /// </summary>
        /// <param name="dirOrFilePath"></param>
        /// <returns></returns>
        public static string NormalizePath(string dirOrfilePath)
        {
            return PathModel.NormalizePath(dirOrfilePath);
        }

        /// <summary>
        /// Returns a normalized directory reference from a path reference
        /// or the parent directory path if the <paramref name="dirPath"/>
        /// reference points to a file.
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static string ExtractDirectoryRoot(string dirPath)
        {
            return PathModel.ExtractDirectoryRoot(dirPath);
        }

        /// <summary>
        /// Determine whether a given path is an existing directory or not.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>true if this directory exists and otherwise false</returns>
        public static bool DirectoryPathExists(string path)
        {
            return PathModel.DirectoryPathExists(path);
        }

        /// <summary>
        /// Determine whether a given path is an existing directory or not.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>true if this directory exists and otherwise false</returns>
        public static Task<bool> DirectoryPathExistsAsync(string path)
        {
            return PathModel.DirectoryPathExistsAsync(path);
        }

        /// <summary>
        /// Split the current folder in an array of sub-folder names and return it.
        /// </summary>
        /// <returns>Returns a string array of su-folder names (including drive) or null if there are no sub-folders.</returns>
        public static string[] GetDirectories(string folder)
        {
            return PathModel.GetDirectories(folder);
        }

        /// <summary>
        /// Split the current folder in an array of sub-folder names and return it.
        /// </summary>
        /// <returns>Returns a string array of su-folder names (including drive) or null if there are no sub-folders.</returns>
        public static Task<string[]> GetDirectoriesAsync(string path)
        {
            return PathModel.GetDirectoriesAsync(path);
        }

        /// <summary>
        /// Joins all string elements in <paramref name="dirs"/> tp one valid string.
        /// Inverse function of string[] GetDirectories(string path) method.
        /// </summary>
        /// <param name="dirs"></param>
        /// <param name="idxStart"></param>
        /// <param name="idxEnd"></param>
        /// <returns></returns>
        public static string Join(string[] dirs, int idxStart, int idxEnd)
        {
            return PathModel.Join(dirs, idxStart, idxEnd);
        }

        /// <summary>
        /// Determine whether a special folder has physical information on current computer or not.
        /// </summary>
        /// <param name="specialFolder"></param>
        /// <returns>Path to special folder (if any) or null</returns>
        public static string SpecialFolderHasPath(System.Environment.SpecialFolder specialFolder)
        {
            return PathModel.SpecialFolderHasPath(specialFolder);
        }

        /// <summary>
        /// Determine whether a special folder has physical information on current computer or not.
        /// </summary>
        /// <param name="specialFolder"></param>
        /// <returns>Path to special folder (if any) or null</returns>
        public static Task<string> SpecialFolderHasPathAsync(System.Environment.SpecialFolder specialFolder)
        {
            return PathModel.SpecialFolderHasPathAsync(specialFolder);
        }

        /// <summary>
        /// Rename an existing directory into the <paramref name="newFolderName"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="newFolderName"></param>
        /// <param name="newFolderPathName"></param>
        /// <returns>false Item to be renamed does not exist or something else is not as expected, otherwise true</returns>
        public static bool RenameFileOrDirectory(IPathModel source,
                                                 string newFolderName,
                                                 out IPathModel newFolderPathName)
        {
            return PathModel.RenameFileOrDirectory(source,
                                                   newFolderName,
                                                   out newFolderPathName);
        }

        /// <summary>
        /// Create a new folder new standard sub folder in <paramref name="folderPath"/>.
        /// The new folder has a standard name like 'New folder n'.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="newDefaultFolderName">Compute default name for new folder</param>
        /// <returns>PathModel object to new folder or null</returns>
        /// <summary>
        /// Create a new folder new standard sub folder in <paramref name="folderPath"/>.
        /// The new folder has a standard name like 'New folder n'.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="newDefaultFolderName">Compute default name for new folder</param>
        /// <returns>PathModel object to new folder or null</returns>
        public static IPathModel CreateDir(IPathModel folderPath,
                                           string newDefaultFolderName = "New Folder")
        {
            return PathModel.CreateDir(folderPath, newDefaultFolderName);
        }

        /// <summary>
        /// Create a new folder new standard sub folder in <paramref name="folderPath"/>.
        /// The new folder has a standard name like 'New folder n'.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="newDefaultFolderName">Compute default name for new folder</param>
        /// <returns>PathModel object to new folder or null</returns>
        public static Task<IPathModel> CreateDirAsync(IPathModel folderPath,
                                                      string newDefaultFolderName = "New Folder")
        {
            return PathModel.CreateDirAsync(folderPath, newDefaultFolderName);
        }

        /// <summary>
        /// Load all sub-folders into the Folders collection via
        /// IEnumerable/Yield.
        /// </summary>
        public static IEnumerable<IPathModel> LoadFolders(string fullPath)
        {
            return PathModel.LoadFolders(fullPath);
        }

        /// <summary>
        /// Load all sub-folders into the Folders collection via
        /// Async method with complete list return.
        /// </summary>
        public static async Task<IEnumerable<IPathModel>> LoadFoldersAsync(string fullPath)
        {
            return await PathModel.LoadFoldersAsync(fullPath);
        }

        public static IPathModel Create(string path)
        {
            if (System.IO.Directory.Exists(path) == true)
                return new PathModel(path, FSItemType.Folder);

            if (System.IO.File.Exists(path) == true)
                return new PathModel(path, FSItemType.File);

            DirectoryInfo d = new DirectoryInfo(path);

            if (d.Parent == null)
                return new PathModel(path, FSItemType.LogicalDrive);

            throw new NotSupportedException(string.Format("Type of file system item '{0}' not supported.", path));
        }
    }
}