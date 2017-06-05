namespace MiniUML.View.Views.RubberBand
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Documents;
  using System.Windows.Input;
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
                                    new PropertyMetadata(null, RubberbandAdorner.OnChangeEndPoint));

    private Point? mStartPoint = null;
    private Point? mEndPoint = null;
    private Pen mRubberbandPen;
    private UIElement mDesignerCanvas = null;

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
      this.mDesignerCanvas = designerCanvas;
      this.mStartPoint = dragStartPoint;

      this.mRubberbandPen = new Pen(Brushes.LightSlateGray, 1);
      this.mRubberbandPen.DashStyle = new DashStyle(new double[] { 2 }, 1);

      this.mChrome = new RubberBandChrome();
      this.mVisuals = new VisualCollection(this);

      this.mVisuals.Add(this.mChrome);
      this.mChrome.DataContext = designerCanvas;  // Apply data context from this object to chrome object
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get/set dependency property to show the end point of a rubber band selection
    /// at the end of a rubber band selection gesture.
    /// </summary>
    public Point? EndPoint
    {
      get { return (Point)GetValue(EndPointProperty); }
      set { SetValue(EndPointProperty, value); }
    }

    /// <summary>
    /// Count the number of children hosted in this view.
    /// </summary>
    protected override int VisualChildrenCount
    {
      get
      {
        return this.mVisuals.Count;
      }
    }
    #endregion properties

    #region methods

    protected override void OnRender(DrawingContext dc)
    {
      base.OnRender(dc);

      // without a background the OnMouseMove event would not be fired !
      // Alternative: implement a Canvas as a child of this adorner, like the ConnectionAdorner does.
      ////dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));

      if (this.mStartPoint.HasValue && this.mEndPoint.HasValue)
      {
        ////dc.DrawRectangle(Brushes.Transparent, this.mRubberbandPen, new Rect(this.mStartPoint.Value, this.mEndPoint.Value));

        this.mChrome.Arrange(new Rect(this.mStartPoint.Value, this.mEndPoint.Value));
        this.mChrome.InvalidateArrange();
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
      return this.mVisuals[index];
    }

    /// <summary>
    /// Method is invoked when the EndPoint dependency property is changed.
    /// This event updates the size of this view.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void OnChangeEndPoint(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      RubberbandAdorner rba = d as RubberbandAdorner;

      if (rba != null && e.NewValue != null)
      {
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
        this.mEndPoint = new Point(endPoint.Value.X, endPoint.Value.Y);

      this.InvalidateVisual();
    }
    #endregion methods
  }
}
