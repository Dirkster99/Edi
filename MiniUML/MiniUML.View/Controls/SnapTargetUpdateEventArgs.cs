namespace MiniUML.View.Controls
{
  using System;
  using System.Windows;

  /// <summary>
  /// Delegate handler method for handling events of the <see cref="SnapTargetUpdateEventArgs"/> type.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="e"></param>
  public delegate void SnapTargetUpdateHandler(ISnapTarget source, SnapTargetUpdateEventArgs e);

  /// <summary>
  /// Handle snap to target <seealso cref="AnchorPoint"/> events based on
  /// the information that is managed by an object of this class.
  /// </summary>
  public class SnapTargetUpdateEventArgs : EventArgs
  {
    #region constructor
    /// <summary>
    /// Standard Constructor
    /// </summary>
    public SnapTargetUpdateEventArgs()
    {
      this.IsMoveUpdate = MoveUpdate.CoerceOnNextLayoutUpdate;
    }

    /// <summary>
    /// Method to setup the event such that <seealso cref="AnchorPoint"/> moves
    /// immidiately by the given <paramref name="moveDelta"/>.
    /// </summary>
    /// <param name="moveDelta"></param>
    public SnapTargetUpdateEventArgs(Vector moveDelta)
    {
      this.IsMoveUpdate = MoveUpdate.MoveDelta;
      this.MoveDelta = moveDelta;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Vecor to move the anchor point by (if any).
    /// </summary>
    public Vector MoveDelta
    {
      get;
      set;
    }

    /// <summary>
    /// Determine whether point is to be moved right away or
    /// coerced later into a certain position.
    /// </summary>
    public MoveUpdate IsMoveUpdate
    {
      get;
      set;
    }
    #endregion properties
  }
}
