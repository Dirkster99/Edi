namespace FileSystemModels.Models
{
  using System;
  using System.IO;
  using System.Xml.Serialization;

  /// <summary>
  /// Class implements basic properties and behaviours
  /// of elements related to a path. Such elements are,
  /// virtual folders, drives, network drives, folder, files,
  /// and shortcuts.
  /// </summary>
  [Serializable]
  public class PathModel
  {
    #region fields
    private FSItemType mItemType;
    private string mPath;
    private PathModel pathModel;
    #endregion fields

    #region constructors
    /// <summary>
    /// Class constructor
    /// </summary>
    public PathModel(string path, FSItemType itemType)
     : this()
    {
      this.mItemType = itemType;

      switch (itemType)
      {
        case FSItemType.Folder:
        case FSItemType.LogicalDrive:
          this.mPath = PathModel.NormalizeDirectoryPath(path);
          break;

        case FSItemType.File:
          this.mPath = PathModel.NormalizePath(path);
          break;

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

      this.mItemType = pathModelCopy.mItemType;
      this.mPath = pathModelCopy.mPath;
      this.pathModel = pathModelCopy.pathModel;
    }

    /// <summary>
    /// Class constructor
    /// </summary>
    public PathModel()
    {
      this.mPath = string.Empty;
      this.mItemType = FSItemType.Unknown;
    }
    #endregion constructors

    #region properties
    /// <summary>
    /// Gets the path of this <seealso cref="PathModel"/> object.
    /// </summary>
    [XmlAttribute(AttributeName = "Path")]
    public string Path
    {
      get
      {
        return this.mPath;
      }

      set
      {
        this.mPath = value;
      }
    }

    /// <summary>
    /// Gets the type of item of this <seealso cref="PathModel"/> object.
    /// </summary>
    [XmlIgnore]
    public FSItemType PathType
    {
      get
      {
        return this.mItemType;
      }
    }

    /// <summary>
    /// Gets the name of this item. For folders this is the folder
    /// name without its path;
    /// </summary>
    [XmlIgnore]
    public string Name
    {
      get
      {
        try
        {
          switch (this.PathType)
          {
            case FSItemType.LogicalDrive:
              return this.Path;

            case FSItemType.Folder:
              DirectoryInfo di = new DirectoryInfo(this.Path);

              return di.Name;

            case FSItemType.File:
              FileInfo fi = new FileInfo(this.Path);

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
    public static bool Compare(PathModel m, PathModel m1)
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
    public static string NormalizePath(string dirOrFilePath)
    {
      if (dirOrFilePath == null)
        return null;

      // The dirinfo constructor will not work with 'C:' but does work with 'C:\'
      if (dirOrFilePath.Length == 2)
      {
        if (dirOrFilePath[dirOrFilePath.Length - 1] == ':')
          dirOrFilePath += System.IO.Path.DirectorySeparatorChar;
      }

      return dirOrFilePath;
    }

    /// <summary>
    /// Normalizes the input string string into a standard (output) notation.
    /// 
    /// Mormalization refers to using backslashes at the end of all directory
    /// path references: 'C:' -> 'C:\' or 'C:\' or 'C:\Temp\'
    /// </summary>
    /// <param name="dirPath"></param>
    /// <returns></returns>
    public static string NormalizeDirectoryPath(string dirPath)
    {
      if (dirPath == null)
        return null;

      // The dirinfo constructor will not work with 'C:' but does work with 'C:\'
      if (dirPath.Length < 2)
        return null;

      // This will normalize directory and drive references into 'C:' or 'C:\Temp'
      if (dirPath[dirPath.Length - 1] == System.IO.Path.DirectorySeparatorChar)
        dirPath = dirPath.Trim(System.IO.Path.DirectorySeparatorChar);

      return dirPath;
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
        return PathModel.NormalizeDirectoryPath(dirPath);
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
          return PathModel.NormalizeDirectoryPath(path);

        return null;
      }
    }

    /// <summary>
    /// Determine whether a given path is an exeisting directory or not.
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
    /// Split the current folder in an array of sub-folder names and return it.
    /// </summary>
    /// <returns>Returns a string array of su-folder names (including drive) or null if there are no sub-folders.</returns>
    public static string[] GetDirectories(string folder)
    {
      if (string.IsNullOrEmpty(folder) == true)
        return null;

      string[] dirs = null;

      try
      {
        dirs = folder.Split(new char[] { System.IO.Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
      }
      catch
      {
      }

      return dirs;
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
    #endregion static helper methods

    /// <summary>
    /// Determine whether a given path is an exeisting directory or not.
    /// </summary>
    /// <returns>true if this directory exists and otherwise false</returns>
    public bool DirectoryPathExists()
    {
      return PathModel.DirectoryPathExists(this.mPath);
    }

    /// <summary>
    /// Rename an existing directory into the <paramref name="newFolderName"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="newFolderName"></param>
    /// <param name="newFolderPathName"></param>
    /// <returns>false Item to be renamed does not exist or something else is not as expected, otherwise true</returns>
    public static bool RenameFileOrDirectory(PathModel source,
                                             string newFolderName,
                                             out PathModel newFolderPathName)
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
    /// <returns>PathModel object to new folder or null</returns>
    public static PathModel CreateDir(PathModel folderPath)
    {
        // Compute default name for new folder
        var newDefaultFolderName = FileSystemModels.Local.Strings.STR_NEW_DEFAULT_FOLDER_NAME;
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
    #endregion methods
  }
}
