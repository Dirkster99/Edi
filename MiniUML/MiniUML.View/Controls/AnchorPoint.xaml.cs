namespace MiniUML.View.Controls
{
  using System;
  using System.ComponentModel;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using MiniUML.Framework;
  using MiniUML.View.Views;

  /// <summary>
  /// Interaction logic for AnchorPoint.xaml
  /// </summary>
  public partial class AnchorPoint : Thumb, INotifyPropertyChanged
  {
    #region fields
    public static readonly ISnapTarget InvalidSnapTarget = null;

    public static readonly DependencyProperty SnapTargetProperty = DependencyProperty.Register(
        "SnapTarget",
        typeof(ISnapTarget),
        typeof(AnchorPoint),
        new FrameworkPropertyMetadata(null,
            FrameworkPropertyMetadataOptions.AffectsRender,
            new PropertyChangedCallback(propChanged)));

    public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(
        "Left",
        typeof(double),
        typeof(AnchorPoint),
        new FrameworkPropertyMetadata(0.0,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            new PropertyChangedCallback(propChanged)));

    public static readonly DependencyProperty TopProperty = DependencyProperty.Register(
        "Top",
        typeof(double),
        typeof(AnchorPoint),
        new FrameworkPropertyMetadata(0.0,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            new PropertyChangedCallback(propChanged)));

    private bool mInMoveTo = false;
    #endregion fields

    #region constructor
    public AnchorPoint()
    {
      this.InitializeComponent();
      this.DragDelta += this.AnchorPoint_DragDelta;
    }
    #endregion constructor

    public event PropertyChangedEventHandler PropertyChanged;

    #region properties
    /// <summary>
    /// Get/set dependency property to connect this <seealso cref="AchorPoint"/>
    /// with a <seealso cref="ISnapTarget"/> (shape).
    /// </summary>
    public ISnapTarget SnapTarget
    {
      get { return (ISnapTarget)GetValue(SnapTargetProperty); }
      set { this.SetValue(SnapTargetProperty, value); }
    }

    /// <summary>
    /// Get/set dependency property for X position of this achorpoint on the canvase.
    /// </summary>
    public double Left
    {
      get { return (double)GetValue(LeftProperty); }
      set { this.SetValue(LeftProperty, value); }
    }

    /// <summary>
    /// Get/set dependency property for Y position of this achorpoint on the canvase.
    /// </summary>
    public double Top
    {
      get { return (double)GetValue(TopProperty); }
      set { this.SetValue(TopProperty, value); }
    }

    /// <summary>
    /// The snap angle (argument of normal vector), normalized to 2π > angle ≥ 0.
    /// </summary>
    public double Angle
    {
      get;
      private set;
    }

    /// <summary>
    /// The snap angle in degrees.
    /// </summary>
    public double AngleInDegrees
    {
      get
      {
        return FrameworkUtilities.RadiansToDegrees(this.Angle);
      }
    }

    /// <summary>
    /// Get property to calculate orientation (vertical or horizontal)
    /// based on degrees stored in angle property.
    /// </summary>
    public Orientation SnapOrientation
    {
      get
      {
        double a = this.Angle;

        if (a > Math.PI)
          a = (2 * Math.PI) - a;
        
        return a > Math.PI / 4 && a < Math.PI * 3 / 4 ? Orientation.Vertical : Orientation.Horizontal;
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Static utility method for determining if and where to snap
    /// a point to a line segment (defined by the points from and to).
    /// </summary>
    public static bool SnapToLineSegment(Point from,
                                         Point to,
                                         Point snapOrigin,
                                         ref double bestSnapLengthSq,
                                         ref Point bestSnapPoint, ref double bestSnapAngle)
    {
      if (from == to) return false;

      Vector pointV = snapOrigin - from;
      Vector lineV = to - from;
      double resolute = pointV.ScalarProjection(lineV);

      // Limit resolute to the line segment between point1 and point2.
      if (resolute < 0)
        resolute = 0;
      else
      {
        if (resolute > lineV.Length)
          resolute = lineV.Length;
      }

      Point snapPoint = from + ((resolute * lineV) / lineV.Length);
      double snapLengthSq = (snapPoint - snapOrigin).LengthSquared;

      if (snapLengthSq < bestSnapLengthSq)
      {
        bestSnapLengthSq = snapLengthSq;
        bestSnapPoint = snapPoint;
        bestSnapAngle = (to - from).GetAngularCoordinate() + (Math.PI / 2);

        return true;
      }

      return false;
    }

    /// <summary>
    /// Moves the point to these coordinates when a layout update
    /// is being processed by the canvas on which this control is placed.
    /// </summary>
    public void CoerceCoordinates()
    {
      this.MoveTo(new Point(this.Left, this.Top));
    }

    /// <summary>
    /// Method is executed when the SnapTarget, Left, or Top dependency property changes.
    /// </summary>
    /// <param name="o"></param>
    /// <param name="a"></param>
    private static void propChanged(DependencyObject o, DependencyPropertyChangedEventArgs a)
    {
      AnchorPoint anchor = (AnchorPoint)o;

      if (a.Property == SnapTargetProperty)
      {
        if (a.OldValue != null)
          ((ISnapTarget)a.OldValue).SnapTargetUpdate -= anchor.snapTargetUpdate;

        if (a.NewValue == InvalidSnapTarget)
          anchor.SetValue(SnapTargetProperty, null);
        else
        {
          if (a.NewValue != null)
            ((ISnapTarget)a.NewValue).SnapTargetUpdate += anchor.snapTargetUpdate;
        }
      }
      else // Left or Top.
      {
        // If we're in a moveTo operation, coordinates have already been coerced.
        // Otherwise, coerce them on the next layout update.
        if (!anchor.mInMoveTo)
          anchor.CoerceLater();
      }
    }

    /// <summary>
    /// Move the anchored line into the <paramref name="position"/>
    /// using the current SnapTarget to determine a correct angle.
    /// </summary>
    /// <param name="position"></param>
    private void MoveTo(Point position)
    {
      if (this.SnapTarget != null)
      {
        double angle;

        // Determine the best position position and angle
        // for an AchorPoint to snap to the angle of the snap target line (if any).
        this.SnapTarget.SnapPoint(ref position, out angle);

        this.setAngle(angle);
      }

      try
      {
        this.mInMoveTo = true;

        this.Left = position.X;
        this.Top = position.Y;
      }
      finally
      {
        this.mInMoveTo = false;
      }
    }

    /// <summary>
    /// Occurs when the SnapTargetUpdate event occurs
    /// in the ISnapTarget dependency property.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    private void snapTargetUpdate(ISnapTarget source, SnapTargetUpdateEventArgs e)
    {
      if (e.IsMoveUpdate == MoveUpdate.MoveDelta)
        this.MoveTo(new Point(this.Left, this.Top) + e.MoveDelta);
      else
        this.CoerceLater();
    }

    private void setAngle(double a)
    {
      a = FrameworkUtilities.NormalizeAngle(a);

      if (this.Angle == a)
        return; // no change

      this.Angle = a;

      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs("SnapOrientation"));
        this.PropertyChanged(this, new PropertyChangedEventArgs("Angle"));
        this.PropertyChanged(this, new PropertyChangedEventArgs("AngleInDegrees"));
      }
    }

    #region Coercion
    /// <summary>
    /// Adds a layout handler call to the list of layout update handlers
    /// to MoveTo the Left, Top position when a layout update occurs.
    /// </summary>
    private void CoerceLater()
    {
      CanvasView cv = CanvasView.GetCanvasView(this);
      
      // Coearce coordinates when the associated layout changes
      if (cv != null)
        cv.NotifyOnLayoutUpdated(this.CoerceCoordinates);
    }
    #endregion

    /// <summary>
    /// occurs one or more times as the mouse changes position when a
    /// System.Windows.Controls.Primitives.Thumb control has logical
    /// focus and mouse capture (anchorpoint control is being dragged).
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AnchorPoint_DragDelta(object sender, DragDeltaEventArgs e)
    {
      this.MoveTo(new Point(this.Left + e.HorizontalChange, this.Top + e.VerticalChange));
    }
    #endregion methods
  }
}
