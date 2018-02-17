namespace ICSharpCode.AvalonEdit.BracketRenderer
{
  using System;
  using System.Windows.Media;
  using Document;
  using Rendering;

  /// <summary>
  /// Highlight opening and closing brackets when when moving the carret in the text
  /// 
  /// Source: https://github.com/icsharpcode/SharpDevelop/blob/master/src/AddIns/DisplayBindings/AvalonEdit.AddIn/Src/BracketHighlightRenderer.cs
  /// </summary>
  public class BracketHighlightRenderer : IBackgroundRenderer
  {
    #region fields
    /// <summary>
    /// Default color (blue) used for bracket highlighting.
    /// </summary>
    public static readonly Color DefaultBackground = Color.FromArgb(100, 0, 0, 255);

    /// <summary>
    /// Default border (bright blue) used for bracket highlighting.
    /// </summary>
    public static readonly Color DefaultBorder = Color.FromArgb(52, 0, 0, 255);

    ////public const string BracketHighlight = "Bracket highlight";

    private BracketSearchResult mResult;
    private Pen mBorderPen;
    private Brush mBackgroundBrush;
    private TextView mTextView;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor from <seealso cref="TextView"/> instance.
    /// </summary>
    /// <param name="textView"></param>
    public BracketHighlightRenderer(TextView textView)
    {
            mTextView = textView ?? throw new ArgumentNullException(nameof(textView));

      mTextView.BackgroundRenderers.Add(this);
    }
    #endregion constructor

    #region methods
    ////    public static void ApplyCustomizationsToRendering(BracketHighlightRenderer renderer, IEnumerable<Color> customizations)
    ////    {
    ////      renderer.UpdateColors(DefaultBackground, DefaultBorder);
    ////
    ////      foreach (Color color in customizations)
    ////      {
    ////        //if (color.Name == BracketHighlight) {
    ////        renderer.UpdateColors(color, color);
    ////        //					renderer.UpdateColors(color.Background ?? Colors.Blue, color.Foreground ?? Colors.Blue);
    ////        // 'break;' is necessary because more specific customizations come first in the list
    ////        // (language-specific customizations are first, followed by 'all languages' customizations)
    ////        break;
    ////        //}
    ////      }
    ////    }

    /// <summary>
    /// Assigns the indicated highlighting mResult in <paramref name="result"/>
    /// and invalidates the corresponding layer to force a redraw.
    /// </summary>
    /// <param name="result"></param>
    public void SetHighlight(BracketSearchResult result)
    {
      if (mResult != result)
      {
        mResult = result;
        mTextView.InvalidateLayer(Layer);
      }
    }

    /// <summary>
    /// Gets the <seealso cref="KnownLayer"/> that is used to highlight brackets
    /// within the text.
    /// </summary>
    public KnownLayer Layer => KnownLayer.Selection;

      /// <summary>
    /// Draw method for drawing highlighted brackets.
    /// </summary>
    /// <param name="textView"></param>
    /// <param name="drawingContext"></param>
    public void Draw(TextView textView, DrawingContext drawingContext)
    {
      if (mResult == null)
        return;

            BackgroundGeometryBuilder builder = new BackgroundGeometryBuilder
            {
                CornerRadius = 1,
                AlignToMiddleOfPixels = true
            };

            builder.AddSegment(textView, new TextSegment()
                                   {
                                     StartOffset = mResult.OpeningBracketOffset,
                                     Length = mResult.OpeningBracketLength
                                   });

      builder.CloseFigure(); // prevent connecting the two segments

      builder.AddSegment(textView, new TextSegment()
                                   {
                                     StartOffset = mResult.ClosingBracketOffset,
                                     Length = mResult.ClosingBracketLength
                                   });

      Geometry geometry = builder.CreateGeometry();

      if (mBorderPen == null)
        UpdateColors(DefaultBackground, DefaultBackground);

      if (geometry != null)
      {
        drawingContext.DrawGeometry(mBackgroundBrush, mBorderPen, geometry);
      }
    }

    /// <summary>
    /// Updates the color definition used for the highlighting of brackets.
    /// </summary>
    /// <param name="background"></param>
    /// <param name="foreground"></param>
    private void UpdateColors(Color background, Color foreground)
    {
      mBorderPen = new Pen(new SolidColorBrush(foreground), 1);
      mBorderPen.Freeze();

      mBackgroundBrush = new SolidColorBrush(background);
      mBackgroundBrush.Freeze();
    }
    #endregion methods
  }
}
