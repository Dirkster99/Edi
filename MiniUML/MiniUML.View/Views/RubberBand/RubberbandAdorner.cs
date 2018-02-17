namespace MiniUML.View.Views.RubberBand
{
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;

  /// <summary>
  /// This class implements a rubber band selection that can be used to
  /// select multiple canvas elements with a mouse gesture.
  /// 
  /// This view design is based on the rubberband adorner published in
  /// http://www.codeproject.com/Articles/23871/WPF-Diagram-Designer-Part-3
  /// </summary>
  public class RubberbandAdorner : Adorner
  {
    #region fields
    public static readonly DependencyProperty EndPointProperty =
        DependencyProperty.Register("EndPoint",
                                    typeof(Point?),
                                    typeof(RubberbandAdorner),
                                    new PropertyMetadata(null, OnChangeEndPoint));

    private Point? mStartPoint;
    private Point? mEndPoint;
    private Pen mRubberbandPen;
    private UIElement mDesignerCanvas;

    private VisualCollection mVisuals;
    private RubberBandChrome mChrome;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    /// <param name="designerCanvas"></param>
    /// <param name="dragStartPoint"></param>
    public RubberbandAdorner(UIElement designerCanvas, Point? dragStartPoint)
      : base(designerCanvas)
    {
      mDesignerCanvas = designerCanvas;
      mStartPoint = dragStartPoint;

            mRubberbandPen = new Pen(Brushes.LightSlateGray, 1)
            {
                DashStyle = new DashStyle(new double[] { 2 }, 1)
            };

            mChrome = new RubberBandChrome();
      mVisuals = new VisualCollection(this);

      mVisuals.Add(mChrome);
      mChrome.DataContext = designerCanvas;  // Apply data context from this object to chrome object
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get/set dependency property to show the end point of a rubber band selection
    /// at the end of a rubber band selection gesture.
    /// </summary>
    public Point? EndPoint
    {
      get => (Point)GetValue(EndPointProperty);
        set => SetValue(EndPointProperty, value);
    }

    /// <summary>
    /// Count the number of children hosted in this view.
    /// </summary>
    protected override int VisualChildrenCount => mVisuals.Count;

      #endregion properties

    #region methods

    protected override void OnRender(DrawingContext dc)
    {
      base.OnRender(dc);

      // without a background the OnMouseMove event would not be fired !
      // Alternative: implement a Canvas as a child of this adorner, like the ConnectionAdorner does.
      ////dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));

      if (mStartPoint.HasValue && mEndPoint.HasValue)
      {
        ////dc.DrawRectangle(Brushes.Transparent, this.mRubberbandPen, new Rect(this.mStartPoint.Value, this.mEndPoint.Value));

        mChrome.Arrange(new Rect(mStartPoint.Value, mEndPoint.Value));
        mChrome.InvalidateArrange();
      }
    }

    /// <summary>
    /// Returns a child at the specified index from a collection of child elements.
    ///
    /// Parameters:
    ///   index:
    ///     The zero-based index of the requested child element in the collection.
    ///
    /// Returns:
    ///     The requested child element. This should not return null; if the provided
    ///     index is out of range, an exception is thrown.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    protected override Visual GetVisualChild(int index)
    {
      return mVisuals[index];
    }

    /// <summary>
    /// Method is invoked when the EndPoint dependency property is changed.
    /// This event updates the size of this view.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void OnChangeEndPoint(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {

            if (d is RubberbandAdorner && e.NewValue != null)
            {
                RubberbandAdorner rba = d as RubberbandAdorner;
                if (e.NewValue is Point?)
                {
                    Point? db = e.NewValue as Point?;

                    rba.UpdateOnMouseMove(db);
                }
            }
        }

    /// <summary>
    /// Method updates the EndPoint dependency property.
    /// </summary>
    /// <param name="endPoint"></param>
    private void UpdateOnMouseMove(Point? endPoint)
    {
      if (endPoint.HasValue)
        mEndPoint = new Point(endPoint.Value.X, endPoint.Value.Y);

      InvalidateVisual();
    }
    #endregion methods
  }
}
