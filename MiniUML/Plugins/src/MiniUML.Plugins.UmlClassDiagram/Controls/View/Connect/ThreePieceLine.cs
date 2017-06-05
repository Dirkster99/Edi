namespace MiniUML.Plugins.UmlClassDiagram.Controls.View.Connect
{
  using System.ComponentModel;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Media;
  using System.Windows.Shapes;

  using MiniUML.Framework;
  using MiniUML.View.Controls;

  public class ThreePieceLine : UserControl, INotifyPropertyChanged, ISnapTarget
  {
    #region fields
    #region dependency properties
    private static readonly DependencyProperty FromXProperty = DependencyProperty.Register(
        "FromX", typeof(double), typeof(ThreePieceLine),
        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(propChanged)));

    private static readonly DependencyProperty FromYProperty = DependencyProperty.Register(
        "FromY", typeof(double), typeof(ThreePieceLine),
        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(propChanged)));

    private static readonly DependencyProperty FromOrientationProperty = DependencyProperty.Register(
        "FromOrientation", typeof(Orientation), typeof(ThreePieceLine),
        new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(propChanged)));

    private static readonly DependencyProperty ToXProperty = DependencyProperty.Register(
        "ToX", typeof(double), typeof(ThreePieceLine),
        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(propChanged)));

    private static readonly DependencyProperty ToYProperty = DependencyProperty.Register(
        "ToY", typeof(double), typeof(ThreePieceLine),
        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(propChanged)));

    private static readonly DependencyProperty ToOrientationProperty = DependencyProperty.Register(
        "ToOrientation", typeof(Orientation), typeof(ThreePieceLine),
        new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(propChanged)));
    #endregion dependency properties

    private Polyline mLine;

    private double mFromAngleInDegrees;
    private double mToAngleInDegrees;
    #endregion fields

    #region constructor
    public ThreePieceLine()
    {
      this.mLine = new Polyline();

      Canvas canvas = new Canvas();

      canvas.Children.Add(this.mLine);
      this.Content = canvas;
    }
    #endregion constructor

    #region INotifyPropertyChanged Members
    /// <summary>
    /// Standard event of <seealso cref="INotifyPropertyChanged"/> interface.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Event of <seealso cref="ISnapTarget"/> interface.
    /// </summary>
    public event SnapTargetUpdateHandler SnapTargetUpdate;
    #endregion

    #region properties
    /// <summary>
    /// Get angle of the line joining the origin, in degrees.
    /// </summary>
    public double FromAngleInDegrees
    {
      get
      {
        return this.mFromAngleInDegrees;
      }
    }

    /// <summary>
    /// Get angle of the line joining the destination, in degrees.
    /// </summary>
    public double ToAngleInDegrees
    {
      get
      {
        return this.mToAngleInDegrees;
      }
    }

    #region dependency properties
    public double FromX
    {
      get { return (double)this.GetValue(FromXProperty); }
      set { this.SetValue(FromXProperty, value); }
    }

    public double FromY
    {
      get { return (double)this.GetValue(FromYProperty); }
      set { this.SetValue(FromYProperty, value); }
    }

    public Orientation FromOrientation
    {
      get { return (Orientation)this.GetValue(FromOrientationProperty); }
      set { this.SetValue(FromOrientationProperty, value); }
    }

    public double ToX
    {
      get { return (double)this.GetValue(ToXProperty); }
      set { this.SetValue(ToXProperty, value); }
    }

    public double ToY
    {
      get { return (double)this.GetValue(ToYProperty); }
      set { this.SetValue(ToYProperty, value); }
    }

    public Orientation ToOrientation
    {
      get { return (Orientation)this.GetValue(ToOrientationProperty); }
      set { this.SetValue(ToOrientationProperty, value); }
    }
    #endregion dependency properties
    #endregion properties

    #region methods
    #region ISnapTarget Members
    /// <summary>
    /// Compute best snap point position and angle for a given position.
    /// </summary>
    /// <param name="snapPosition"></param>
    /// <param name="snapAngle"></param>
    public void SnapPoint(ref Point snapPosition, out double snapAngle)
    {
      snapAngle = 0;

      PointCollection points = this.mLine.Points;

      if (points.Count < 2)
        return;

      Point bestSnapPoint = new Point(double.NaN, double.NaN);
      double bestSnapLengthSq = double.PositiveInfinity;

      Point lastPoint = points[0];
      for (int i = 1; i < points.Count; i++)
      {
        Point curPoint = points[i];
        AnchorPoint.SnapToLineSegment(lastPoint, curPoint, snapPosition, ref bestSnapLengthSq, ref bestSnapPoint, ref snapAngle);
        lastPoint = curPoint;
      }

      snapPosition = bestSnapPoint;
    }

    /// <summary>
    /// Standard event of <seealso cref="INotifyPropertyChanged"/> interface.
    /// </summary>
    /// <param name="e"></param>
    public void NotifySnapTargetUpdate(SnapTargetUpdateEventArgs e)
    {
      if (this.SnapTargetUpdate != null)
        this.SnapTargetUpdate(this, e);
    }
    #endregion

    private static void propChanged(DependencyObject o, DependencyPropertyChangedEventArgs a)
    {
      ((ThreePieceLine)o).rerouteLine();
    }

    private void rerouteLine()
    {
      Point from = new Point(this.FromX, this.FromY);
      Point to = new Point(this.ToX, this.ToY);

      this.mLine.Points.Clear();
      this.mLine.Points.Add(from);

      if (this.FromOrientation != this.ToOrientation)
      {
        // Fine, we'll just do a two-piece line, then.
        if (this.FromOrientation == Orientation.Horizontal)
          this.mLine.Points.Add(new Point(to.X, from.Y));
        else this.mLine.Points.Add(new Point(from.X, to.Y));
      }
      else if (this.FromOrientation /* == ToOrientation */ == Orientation.Horizontal)
      {
        double mid = from.X + ((to.X - from.X) / 2);

        this.mLine.Points.Add(new Point(mid, from.Y));
        this.mLine.Points.Add(new Point(mid, to.Y));
      }
      else /* FromOrientation == ToOrientation == Orientation.Vertical */
      {
        double mid = from.Y + ((to.Y - from.Y) / 2);

        this.mLine.Points.Add(new Point(from.X, mid));
        this.mLine.Points.Add(new Point(to.X, mid));
      }

      this.mLine.Points.Add(to);

      Vector firstSegment = from - this.mLine.Points[1];
      Vector lastSegment = to - this.mLine.Points[this.mLine.Points.Count() - 2];
      this.mFromAngleInDegrees = FrameworkUtilities.RadiansToDegrees(firstSegment.GetAngularCoordinate());
      this.mToAngleInDegrees = FrameworkUtilities.RadiansToDegrees(lastSegment.GetAngularCoordinate());

      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs("FromAngleInDegrees"));
        this.PropertyChanged(this, new PropertyChangedEventArgs("ToAngleInDegrees"));
      }

      this.NotifySnapTargetUpdate(new SnapTargetUpdateEventArgs());
    }

    #endregion methods
  }
}