namespace FileSystemModels.Models.FSItems
{
  using System.IO;
  using System.Security;

  public class DriveModel : Base.FileSystemModel
  {
    #region fields
    DriveInfo mDrive;
    #endregion fields

    #region constructors
    /// <summary>
    /// Parameterized class  constructor
    /// </summary>
    /// <param name="model"></param>
    [SecuritySafeCritical]
    public DriveModel(PathModel model)
      : base(model)
    {
      this.mDrive = new DriveInfo(model.Path);
    }
    #endregion constructors

    #region properties
    public long AvailableFreeSpace
    {
      get
      {
        return this.mDrive.AvailableFreeSpace;
      }
    }
    
    public string DriveFormat
    {
      get
      {
        return this.mDrive.DriveFormat;
      }
    }
    
////    public DriveType DriveType
////    {
////      get
////      {
////        return this.mDrive.DriveType;
////      }
////    }

    public bool Exists
    {
      get
      {
        return this.mDrive.RootDirectory.Exists;
      }
    }

    public bool IsReady
    {
      get
      {
        return this.mDrive.IsReady;
      }
    }
    
    public long TotalFreeSpace
    {
      get
      {
        return this.mDrive.TotalFreeSpace;
      }
    }
    
    public long TotalSize
    {
      get
      {
        return this.mDrive.TotalSize;
      }
    }
    
    public string VolumeLabel
    {
      get
      { 
        return this.mDrive.VolumeLabel;
      }
    }
    #endregion properties
  }
}
