namespace FileListView.ViewModels
{
  using System;
  using System.IO;
  using System.Windows.Media;
  using FileListView.Utils;
  using FileSystemModels.Models;
  using FileSystemModels.Utils;
  using InplaceEditBoxLib.ViewModels;

  /// <summary>
  /// The Viewmodel for file system items
  /// </summary>
  public class FSItemViewModel : EditInPlaceViewModel
  {
    #region fields
    /// <summary>
    /// Logger facility
    /// </summary>
    protected new static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private string mDisplayName;
    private ImageSource mDisplayIcon;
    private PathModel mPathObject;
    private string mVolumeLabel;
    #endregion fields

    #region constructor
    /// <summary>
    /// class constructor
    /// </summary>
    /// <param name="curdir"></param>
    /// <param name="displayName"></param>
    /// <param name="itemType"></param>
    /// <param name="showIcon"></param>
    /// <param name="indentation"></param>
    public FSItemViewModel(string curdir,
                    FSItemType itemType,
                    string displayName,
                    bool showIcon,
                    int indentation = 0)
      : this(curdir, itemType, displayName, indentation)
    {
      this.ShowToolTip = showIcon;
    }

    /// <summary>
    /// class constructor
    /// </summary>
    /// <param name="curdir"></param>
    /// <param name="displayName"></param>
    /// <param name="itemType"></param>
    /// <param name="indentation"></param>
    public FSItemViewModel(string curdir,
                    FSItemType itemType,
                    string displayName,
                    int indentation = 0)
      : this()
    {
      this.mPathObject = new PathModel(curdir, itemType);
      this.DisplayName = displayName;
      this.Indentation = indentation;
    }

    /// <summary>
    /// Hidden standard class constructor
    /// </summary>
    protected FSItemViewModel()
    {
      this.mDisplayIcon = null;
      this.mPathObject = null;
      this.mVolumeLabel = null;

      this.Indentation = 0;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Gets a name that can be used for display
    /// (is not necessarily the same as path)
    /// </summary>
    public string DisplayName
    {
      get
      {
        return this.mDisplayName;
      }
      
      private set
      {
        if (this.mDisplayName != value)
        {
          this.mDisplayName = value;
          this.RaisePropertyChanged(() => this.DisplayName);
        }
      }
    }

    /// <summary>
    /// Gets the path to this item
    /// </summary>
    public string FullPath
    {
      get
      {
        return (this.mPathObject != null ? this.mPathObject.Path : null);
      }
    }

    /// <summary>
    /// Gets the type (folder, file) of this item
    /// </summary>
    public FSItemType Type
    {
      get
      {
        return (this.mPathObject != null ? this.mPathObject.PathType : FSItemType.Unknown);
      }
    }

    /// <summary>
    /// Gets a copy of the internal <seealso cref="PathModel"/> object.
    /// </summary>
    public PathModel GetModel
    {
      get
      {
        return new PathModel(this.mPathObject);
      }
    }

    /// <summary>
    /// Gets an icon to display for this item.
    /// </summary>
    public ImageSource DisplayIcon
    {
      get
      {
        if (this.mDisplayIcon == null)
        {
          try
          {
            if (this.Type == FSItemType.Folder)
              this.mDisplayIcon = IconExtractor.GetFolderIcon(this.FullPath).ToImageSource();
            else
              this.mDisplayIcon = IconExtractor.GetFileIcon(this.FullPath).ToImageSource();
          }
          catch
          {
          }
        }

        return this.mDisplayIcon;
      }

      private set
      {
        if (this.mDisplayIcon != value)
        {
          this.mDisplayIcon = value;
        }
      }
    }

    /// <summary>
    /// Gets whether or not to show a tooltip for this item.
    /// </summary>
    public bool ShowToolTip { get; private set; }

    /// <summary>
    /// Gets an indendation (if any) for this item.
    /// An indendation allows the display of path
    /// items
    ///      in
    ///        stair
    ///             like
    ///                 display
    ///                        fashion.
    /// </summary>
    public int Indentation { get; private set; }
    #endregion properties

    #region methods
    /// <summary>
    /// Public construction method to create a <see cref="FSItemViewModel"/>
    /// object that represents a logical drive (eg 'C:\')
    /// </summary>
    /// <param name="curdir"></param>
    /// <returns></returns>
    public static FSItemViewModel CreateLogicalDrive(string curdir)
    {
      FSItemViewModel item = new FSItemViewModel();

      item.mPathObject = new PathModel(curdir, FSItemType.LogicalDrive);
      item.DisplayName = item.DisplayItemString();

      // Names of Logical drives cannot be changed with this
      item.IsReadOnly = true;

      return item;
    }

    /// <summary>
    /// Standard method to display contents of this class.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return this.FullPath;
    }

    /// <summary>
    /// Assign a certain icon to this item.
    /// </summary>
    /// <param name="src"></param>
    public void SetDisplayIcon(ImageSource src = null)
    {
      if (src == null)
        this.DisplayIcon = IconExtractor.GetFolderIcon(this.FullPath, true).ToImageSource();
      else
        this.DisplayIcon = src;
    }

    /// <summary>
    /// Determine whether a given path is an exeisting directory or not.
    /// </summary>
    /// <returns>true if this directory exists and otherwise false</returns>
    public bool DirectoryPathExists()
    {
      return this.mPathObject.DirectoryPathExists();
    }

    /// <summary>
    /// Gets a folder item string for display purposes.
    /// This string can evaluete to 'C:\ (Windows)' for drives,
    /// if the 'C:\' drive was named 'Windows'.
    /// </summary>
    public string DisplayItemString()
    {
      switch (this.mPathObject.PathType)
      {
        case FSItemType.LogicalDrive:
          try
          {
            if (this.mVolumeLabel == null)
            {
              DriveInfo di = new System.IO.DriveInfo(this.FullPath);

              if (di.IsReady == true)
                this.mVolumeLabel = di.VolumeLabel;
              else
                return string.Format("{0} ({1})", this.FullPath, FileSystemModels.Local.Strings.STR_MSG_DEVICE_NOT_READY);
            }

            return string.Format("{0} {1}", this.FullPath, (string.IsNullOrEmpty(this.mVolumeLabel)
                                                            ? string.Empty
                                                            : string.Format("({0})", this.mVolumeLabel)));
          }
          catch (Exception exp)
          {
            Logger.Warn("DriveInfo cannot be optained for:" + this.FullPath, exp);

            // Just return a folder name if everything else fails (drive may not be ready etc).
            return string.Format("{0} ({1})", this.FullPath, exp.Message.Trim());
          }

        case FSItemType.Folder:
        case FSItemType.File:
        case FSItemType.Unknown:
        default:
          return this.FullPath;
      }
    }

    /// <summary>
    /// Rename the name of a folder or file into a new name.
    /// </summary>
    /// <param name="newFolderName"></param>
    public void RenameFileOrFolder(string newFolderName)
    {
      try
      {
        if (newFolderName != null)
        {
          PathModel newFolderPath;

          if (PathModel.RenameFileOrDirectory(this.mPathObject, newFolderName, out newFolderPath) == true)
          {
            this.mPathObject = newFolderPath;
            this.DisplayName = newFolderPath.Name;
          }
        }
      }
      catch (Exception exp)
      {
        Logger.Error(string.Format("Rename into '{0}' was not succesful.", newFolderName), exp);

        base.ShowNotification(FileSystemModels.Local.Strings.STR_RenameFolderErrorTitle, exp.Message);
      }
      finally
      {
        this.RaisePropertyChanged(() => this.FullPath);
      }
    }
    #endregion methods
  }
}
