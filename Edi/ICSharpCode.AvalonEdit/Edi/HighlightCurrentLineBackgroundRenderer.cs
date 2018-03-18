namespace ICSharpCode.AvalonEdit.Edi
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using AvalonEdit;
  using Rendering;
  using System.Windows.Media;
  using System.Windows;

  /// <summary>
  /// Source: http://stackoverflow.com/questions/5072761/avalonedit-highlight-current-line-even-when-not-focused
  /// </summary>
  public class HighlightCurrentLineBackgroundRenderer : IBackgroundRenderer
  {
    private TextEditor mEditor;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="editor"></param>
    /// <param name="highlightBackgroundColorBrush"></param>
    public HighlightCurrentLineBackgroundRenderer(TextEditor editor,
                                                  SolidColorBrush highlightBackgroundColorBrush = null)
    {
      mEditor = editor;

      // Light Blue 0x100000FF
      BackgroundColorBrush = new SolidColorBrush((highlightBackgroundColorBrush == null ? Color.FromArgb(0x10, 0x80, 0x80, 0x80) :
                                                                                               highlightBackgroundColorBrush.Color));
    }

    /// <summary>
    /// Get the <seealso cref="KnownLayer"/> of the <seealso cref="TextEditor"/> control.
    /// </summary>
    public KnownLayer Layer
    {
      get { return KnownLayer.Background; }
    }

    /// <summary>
    /// Get/Set color brush to show for highlighting current line
    /// </summary>
    public SolidColorBrush BackgroundColorBrush { get; set; }

    /// <summary>
    /// Draw the background line highlighting of the current line.
    /// </summary>
    /// <param name="textView"></param>
    /// <param name="drawingContext"></param>
    public void Draw(TextView textView, DrawingContext drawingContext)
    {
      if (mEditor.Document == null)
        return;

      textView.EnsureVisualLines();
      var currentLine = mEditor.Document.GetLineByOffset(mEditor.CaretOffset);

      foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, currentLine))
      {
        drawingContext.DrawRectangle(new SolidColorBrush(BackgroundColorBrush.Color), null,
                                     new Rect(rect.Location, new Size(textView.ActualWidth, rect.Height)));
      }
    }
  }
}
