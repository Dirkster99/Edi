namespace MiniUML.View.Controls
{
  using System.Windows;

  /// <summary>
  /// Determine when a snaptarget move should be updated.
  /// </summary>
  public enum MoveUpdate
  {
    /// <summary>
    /// Implement update right away.
    /// </summary>
    MoveDelta = 1,

    /// <summary>
    /// Update later with next layout update.
    /// </summary>
    CoerceOnNextLayoutUpdate = 2
  }

  /// <summary>
  /// Interface definition to let <seealso cref="AchorPoint.xaml.cs"/> points
  /// snap to a shape like target.
  /// </summary>
  public interface ISnapTarget
  {
    /// <summary>
    /// The event occurs when a line should snap to a target point(?).
    /// </summary>
    event SnapTargetUpdateHandler SnapTargetUpdate;

    /// <summary>
    /// Determine the best position <paramref name="p"/> and angle <paramref name="snapAngle"/>
    /// for an AchorPoint to snap to the angle of the snap target line (if any).
    /// </summary>
    /// <param name="p"></param>
    /// <param name="snapAngle"></param>
    void SnapPoint(ref Point p, out double snapAngle);

    void NotifySnapTargetUpdate(SnapTargetUpdateEventArgs e);
  }
}
