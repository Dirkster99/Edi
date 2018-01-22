namespace FileSystemModels.Events
{
  using System;

  /// <summary>
  /// This event tells the receiver that the user wants to open a file.
  /// </summary>
  public class FileOpenEventArgs : EventArgs
  {
    /// <summary>
    /// Path an file name of file to open.
    /// </summary>
    public string FileName { get; set; }
  }
}
