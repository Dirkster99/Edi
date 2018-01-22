namespace FileSystemModels.Events
{
  using System;
  using FileSystemModels.Models;

  /// <summary>
  /// Class implements an event for messaging operations
  /// on a recent folder collection.
  /// </summary>
  public class RecentFolderEvent : EventArgs
  {
    #region constructor
    /// <summary>
    /// Event type class constructor from parameter
    /// </summary>
    public RecentFolderEvent(PathModel path,
                             RecentFolderAction action = RecentFolderAction.Add)
    : this()
    {
      this.Folder = path;
      this.Action = action;
    }

    /// <summary>
    /// Class constructor
    /// </summary>
    public RecentFolderEvent()
    : base()
    {
      this.Folder = null;
      this.Action = RecentFolderAction.Add;
    }
    #endregion constructor

    #region enums
    /// <summary>
    /// Enumeration to define an action on recent folders.
    /// </summary>
    public enum RecentFolderAction
    {
      /// <summary>
      /// Remove a folder from the current collection.
      /// </summary>
      Remove = 0,

      /// <summary>
      /// Add a folder into the current collection.
      /// </summary>
      Add = 1
    }
    #endregion enums

    #region properties
    /// <summary>
    /// Path to this directory...
    /// </summary>
    public PathModel Folder { get; set; }

    /// <summary>
    /// Gets/sets the type of recent folder action (eg Add/Remove).
    /// </summary>
    public RecentFolderAction Action { get; set; }
    #endregion properties
  }
}
