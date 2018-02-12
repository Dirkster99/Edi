namespace FileSystemModels.Models.FSItems.Base
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using FileSystemModels.Interfaces;

    /// <summary>
    /// Class implements basic properties and behaviours
    /// of elements related to a path. Such elements are,
    /// virtual folders, drives, network drives, folder, files, and shortcuts.
    /// </summary>
    internal class PathModel : IPathModel
    {
        #region fields
        private FSItemType mItemType;
        private string mPath;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public PathModel(string path, FSItemType itemType)
         : this()
        {
            mItemType = itemType;

            switch (itemType)
            {
                case FSItemType.LogicalDrive:
                case FSItemType.File:
                    mPath = PathModel.NormalizePath(path);
                    break;

                case FSItemType.Folder:
                    mPath = PathModel.NormalizePath(path);
                    break;

                ////                case FSItemType.DummyEntry:
                ////                    break;

                case FSItemType.Unknown:
                default:
                    throw new NotImplementedException(string.Format("Enumeration member: '{0}' not supported.", itemType));
            }
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="pathModelCopy"></param>
        public PathModel(PathModel pathModelCopy)
          : this()
        {
            if (pathModelCopy == null)
                return;

            mItemType = pathModelCopy.mItemType;
            mPath = pathModelCopy.mPath;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        protected PathModel()
        {
            mPath = string.Empty;
            mItemType = FSItemType.Unknown;
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets the path of this <seealso cref="PathModel"/> object.
        /// </summary>
        public string Path
        {
            get
            {
                return mPath;
            }

            private set
            {
                mPath = value;
            }
        }

        /// <summary>
        /// Gets the type of item of this <seealso cref="PathModel"/> object.
        /// </summary>
        public FSItemType PathType
        {
            get
            {
                return mItemType;
            }
        }

        /// <summary>
        /// Gets the name of this item. For folders this is the folder
        /// name without its path;
        /// </summary>
        public string Name
        {
            get
            {
                try
                {
                    switch (PathType)
                    {
                        case FSItemType.LogicalDrive:
                            return Path;

                        case FSItemType.Folder:
                            DirectoryInfo di = new DirectoryInfo(Path);

                            return di.Name;

                        case FSItemType.File:
                            FileInfo fi = new FileInfo(Path);

                            return fi.Name;

                        case FSItemType.Unknown:
                        default:
                            break;
                    }

                }
                catch
                {
                }

                return null;
            }
        }
        #endregion properties

        #region methods
        #region static helper methods
        /// <summary>
        /// Compare the paths for 2 <see cref="PathModel"/> objects
        /// and return false if they are not equal, otherwise true.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="m1"></param>
        /// <returns></returns>
        public static bool Compare(IPathModel m, IPathModel m1)
        {
            if ((m == null && m1 != null) || (m != null && m1 == null))
                return false;

            if (m == m1)
                return true;

            if (string.Compare(m.Path, m1.Path, true) != 0)
                return false;

            if (m.PathType != m1.PathType)
                return false;

            return true;
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
            if ((path == null && path1 != null) ||
                (path != null && path1 == null))
                return false;

            if (path == null && path1 == null)
                return true;

            if (string.Compare(path, path1, true) != 0)
                return false;

            return true;
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
            if (string.IsNullOrEmpty(path) == true)
                return false;

            // any reference to a folder or file is at least 2 characters long
            if (path.Length < 2)
                return false;

            return true;
        }

        /// <summary>
        /// Make sure that a path reference does actually work with
        /// <see cref="System.IO.DirectoryInfo"/> by replacing 'C:' by 'C:\'.
        /// </summary>
        /// <param name="dirOrFilePath"></param>
        /// <returns></returns>
        public static string NormalizePath(string dirOrfilePath)
        {
            if (string.IsNullOrEmpty(dirOrfilePath) == true)
                return null;

            // The dirinfo constructor will not work with 'C:' but does work with 'C:\'
            if (dirOrfilePath.Length < 2)
                return null;

            if (dirOrfilePath.Length == 2)
            {
                if (dirOrfilePath[dirOrfilePath.Length - 1] == ':')
                    return dirOrfilePath + System.IO.Path.DirectorySeparatorChar;
            }

            if (dirOrfilePath.Length == 3)
            {
                if (dirOrfilePath[dirOrfilePath.Length - 2] == ':' &&
                    dirOrfilePath[dirOrfilePath.Length - 1] == System.IO.Path.DirectorySeparatorChar)
                    return dirOrfilePath;

                return "" + dirOrfilePath[0] + dirOrfilePath[1] +
                            System.IO.Path.DirectorySeparatorChar + dirOrfilePath[2];
            }

            // Insert a backslash in 3rd character position if not already present
            // C:Temp\myfile -> C:\Temp\myfile
            if (dirOrfilePath.Length >= 3)
            {
                if (char.ToUpper(dirOrfilePath[0]) >= 'A' && char.ToUpper(dirOrfilePath[0]) <= 'Z' &&
                    dirOrfilePath[1] == ':' &&
                    dirOrfilePath[2] != '\\')
                {
                    dirOrfilePath = dirOrfilePath.Substring(0, 2) + "\\" + dirOrfilePath.Substring(2);
                }
            }

            // This will normalize directory and drive references into 'C:' or 'C:\Temp'
            if (dirOrfilePath[dirOrfilePath.Length - 1] == System.IO.Path.DirectorySeparatorChar)
                dirOrfilePath = dirOrfilePath.Trim(System.IO.Path.DirectorySeparatorChar);

            return dirOrfilePath;
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
            bool bExists = false;

            if (PathModel.CheckValidString(dirPath) == false)
                return null;

            try
            {
                bExists = System.IO.Directory.Exists(dirPath);
            }
            catch
            {
            }

            if (bExists == true)
                return PathModel.NormalizePath(dirPath);
            else
            {
                bExists = false;
                string path = string.Empty;

                try
                {
                    // check if this is a file reference and attempt to get its path
                    path = System.IO.Path.GetDirectoryName(dirPath);
                    bExists = System.IO.Directory.Exists(path);
                }
                catch
                {
                }

                if (string.IsNullOrEmpty(path) == true)
                    return null;

                if (path.Length <= 3)
                    return null;

                if (bExists == true)
                    return PathModel.NormalizePath(path);

                return null;
            }
        }

        /// <summary>
        /// Determine whether a given path is an existing directory or not.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>true if this directory exists and otherwise false</returns>
        public static bool DirectoryPathExists(string path)
        {
            if (string.IsNullOrEmpty(path) == true)
                return false;

            bool isPath = false;

            try
            {
                isPath = System.IO.Directory.Exists(path);
            }
            catch
            {
            }

            return isPath;
        }

        /// <summary>
        /// Determine whether a given path is an existing directory or not.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>true if this directory exists and otherwise false</returns>
        public static Task<bool> DirectoryPathExistsAsync(string path)
        {
            return Task.Run(() => { return DirectoryPathExists(path); });
        }

        /// <summary>
        /// Split the current folder in an array of sub-folder names and return it.
        /// </summary>
        /// <returns>Returns a string array of su-folder names (including drive) or null if there are no sub-folders.</returns>
        public static string[] GetDirectories(string folder)
        {
            if (string.IsNullOrEmpty(folder) == true)
                return null;

            folder = PathModel.NormalizePath(folder);

            string[] dirs = null;

            try
            {
                dirs = folder.Split(new char[] { System.IO.Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

                if (dirs != null)
                {
                    if (dirs[0].Length == 2)       // Normalizing Drive representation
                    {                             // from 'C:' to 'C:\'
                        if (dirs[0][1] == ':')   // to ensure correct processing
                            dirs[0] += '\\';    // since 'C:' technically invalid(!)
                    }
                }
            }
            catch
            {
            }

            return dirs;
        }

        /// <summary>
        /// Split the current folder in an array of sub-folder names and return it.
        /// </summary>
        /// <returns>Returns a string array of su-folder names (including drive) or null if there are no sub-folders.</returns>
        public static Task<string[]> GetDirectoriesAsync(string path)
        {
            return Task.Run(() => { return GetDirectories(path); });
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
            string ret = string.Empty;

            if (dirs == null)
                throw new NotSupportedException("dirs array cannot be empty.");

            if (dirs.Length == 0)
                throw new NotSupportedException("dirs array cannot be empty.");

            if (dirs.Length < idxStart || dirs.Length < idxEnd || idxStart > idxEnd)
                throw new NotSupportedException("Index parameters are not valid.");

            ret = dirs[idxStart];

            for (int i = idxStart + 1; i < idxEnd; i++)
                ret = System.IO.Path.Combine(ret, dirs[i]);

            return ret;
        }

        /// <summary>
        /// Determine whether a special folder has physical information on current computer or not.
        /// </summary>
        /// <param name="specialFolder"></param>
        /// <returns>Path to special folder (if any) or null</returns>
        public static string SpecialFolderHasPath(System.Environment.SpecialFolder specialFolder)
        {
            string path = null;

            try
            {
                path = Environment.GetFolderPath(specialFolder);

                if (string.IsNullOrEmpty(path) == true)
                    return null;
                else
                    return path;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Determine whether a special folder has physical information on current computer or not.
        /// </summary>
        /// <param name="specialFolder"></param>
        /// <returns>Path to special folder (if any) or null</returns>
        public static Task<string> SpecialFolderHasPathAsync(System.Environment.SpecialFolder specialFolder)
        {
            return Task.Run(() => { return SpecialFolderHasPath(specialFolder); });
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
            newFolderPathName = null;

            switch (source.PathType)
            {
                case FSItemType.Folder:
                    if (System.IO.Directory.Exists(source.Path))
                    {
                        DirectoryInfo di = new DirectoryInfo(source.Path);

                        string parent = di.Parent.FullName;

                        string newFolderPath = System.IO.Path.Combine(parent, newFolderName);

                        newFolderPathName = new PathModel(newFolderPath, source.PathType);

                        System.IO.Directory.Move(source.Path, newFolderPathName.Path);

                        return true;
                    }
                    break;

                case FSItemType.File:
                    if (System.IO.File.Exists(source.Path))
                    {
                        string parent = System.IO.Directory.GetParent(source.Path).FullName;

                        newFolderPathName = new PathModel(System.IO.Path.Combine(parent, newFolderName), source.PathType);

                        System.IO.Directory.Move(source.Path, newFolderPathName.Path);

                        return true;
                    }
                    break;

                case FSItemType.LogicalDrive:
                case FSItemType.Unknown:
                default:
                    break;
            }

            // Item to be renamed does not exist or something else is not as expected
            return false;
        }

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
            var newFolderName = newDefaultFolderName;
            var newFolderPath = newFolderName;

            try
            {
                if (System.IO.Directory.Exists(folderPath.Path) == false)
                    return null;

                // Compute default name for new folder
                newFolderPath = System.IO.Path.Combine(folderPath.Path, newDefaultFolderName);

                for (int i = 1; System.IO.Directory.Exists(newFolderPath) == true; i++)
                {
                    newFolderName = string.Format("{0} {1}", newDefaultFolderName, i);
                    newFolderPath = System.IO.Path.Combine(folderPath.Path, newFolderName);
                }

                // Create that new folder
                System.IO.Directory.CreateDirectory(newFolderPath);

                return new PathModel(newFolderPath, FSItemType.Folder);
            }
            catch (Exception exp)
            {
                throw new Exception(string.Format("'{0}'", newFolderPath), exp);
            }
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
            return Task.Run(() => { return CreateDir(folderPath, newDefaultFolderName); });
        }

        /// <summary>
        /// Load all sub-folders into the Folders collection via
        /// IEnumerable/Yield.
        /// </summary>
        public static IEnumerable<IPathModel> LoadFolders(string fullPath)
        {
            foreach (string dir in Directory.GetDirectories(fullPath))
            {
                var item = new PathModel(dir, FSItemType.Folder);
                yield return item;
            }
        }

        /// <summary>
        /// Load all sub-folders into the Folders collection via
        /// Async method with complete list return.
        /// </summary>
        public static async Task<IEnumerable<IPathModel>> LoadFoldersAsync(string fullPath)
        {
            var items = await Task.Run(() =>
            {
                try
                {
                    return Directory.GetDirectories(fullPath);
                }
                catch
                {
                }
                return new string[0];
            });

            var ret = new List<IPathModel>();

            foreach (string dir in items)
                ret.Add(new PathModel(dir, FSItemType.Folder));

            return ret;
        }
        #endregion static helper methods

        /// <summary>
        /// Determine whether a given path is an existing directory or not.
        /// </summary>
        /// <returns>true if this directory exists and otherwise false</returns>
        public bool DirectoryPathExists()
        {
            return PathModel.DirectoryPathExists(mPath);
        }

        /// <summary>
        /// Async version to determine whether a given path is an existing directory or not.
        /// </summary>
        /// <returns>true if this directory exists and otherwise false</returns>
        public async Task<bool> DirectoryPathExistsAsync()
        {
            return await Task.Run(() => DirectoryPathExists());
        }

        /// <summary>
        /// Get a copy of this <seealso cref="PathModel"/> object via
        /// the <seealso cref="ICloneable"/> interface.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new PathModel(this);
        }

        /// <summary>
        /// Determine whether two <seealso cref="PathModel"/> objects describe
        /// the same location/item in the file system or not.
        ///
        /// Method implements <seealso cref="IEqualityComparer"/> interface.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public new bool Equals(object x, object y)
        {
            if (x is IPathModel == false)
                throw new System.ArgumentException("Object x does not implement IPathModel interface.");

            if (y is IPathModel == false)
                throw new System.ArgumentException("Object y does not implement IPathModel interface.");

            return PathModel.Compare(x as IPathModel, y as IPathModel);
        }

        /// <summary>
        /// Serves as the default hash function.
        ///
        /// Method implements <seealso cref="IEqualityComparer"/> interface.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>A hash code for the current object.</returns>
        public int GetHashCode(object obj)
        {
            if (this.Path != null)
                return this.Path.GetHashCode();

            return 0;
        }

        /// <summary>
        /// Determine whether two <seealso cref="PathModel"/> objects
        /// describe the same location/item in the file system or not.
        ///
        /// Method implements <seealso cref="IEqualityComparer{IPathModel}"/> interface.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(IPathModel x, IPathModel y)
        {
            return PathModel.Compare(x, y);
        }

        /// <summary>
        /// Serves as the default hash function.
        ///
        /// Method implements <seealso cref="IEqualityComparer{IPathModel}"/> interface.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>A hash code for the current object.</returns>
        public int GetHashCode(IPathModel obj)
        {
            return obj.GetHashCode();
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns
        /// an integer that indicates whether the current instance precedes, follows, or
        /// occurs in the same position in the sort order as the other object.
        ///
        /// Method implements <seealso cref="IComparable"/> interface.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The
        /// return value has these meanings: Value Meaning Less than zero This instance precedes
        /// obj in the sort order. Zero This instance occurs in the same position in the
        /// sort order as obj. Greater than zero This instance follows obj in the sort order.
        ///
        /// Exceptions:
        ///   T:System.ArgumentException:
        ///     Object does not implement IPathModel interface.
        /// </returns>
        public int CompareTo(object obj)
        {
            if (obj is IPathModel == false)
                throw new System.ArgumentException("Object does not implement IPathModel interface.");

            return CompareTo(obj as IPathModel);
        }

        /// <summary>
        /// Determine whether two <seealso cref="PathModel"/> objects describe the same
        /// location/item in the file system or not.
        ///
        /// Method implements <seealso cref="IComparable{IPathModel}"/> interface.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public int CompareTo(IPathModel path)
        {
            if (PathModel.Compare(this, path) == true)
                return 0;
            else
            {
                string spath = PathModel.NormalizePath(path.Path);
                string thispath = PathModel.NormalizePath(this.Path);

                return string.Compare(spath, thispath, true);
            }
        }

        /// <summary>
        /// Determine whether two <seealso cref="PathModel"/> objects describe the same
        /// location/item in the file system or not.
        ///
        /// Method implements <seealso cref="IEquatable{IPathModel}"/> interface.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IPathModel other)
        {
            return PathModel.Compare(this, other);
        }
        #endregion methods
    }
}
