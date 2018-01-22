namespace FileSystemModels.Events
{
  using System;
  using FileSystemModels.Models;

  /// <summary>
  /// Class implements ...
  /// </summary>
  public class FolderChangedEventArgs : EventArgs
  {
    /// <summary>
    /// Event type class constructor from parameter
    /// </summary>
    public FolderChangedEventArgs(PathModel path)
    : this()
    {
      this.Folder = path;
    }

    /// <summary>
    /// Class constructor
    /// </summary>
    public FolderChangedEventArgs()
    : base()
    {
      this.Folder = null;
    }

    /// <summary>
    /// Path to this directory...
    /// </summary>
    public PathModel Folder { get; set; }
  }
}
