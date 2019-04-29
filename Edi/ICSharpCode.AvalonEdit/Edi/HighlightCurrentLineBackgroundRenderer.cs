namespace ICSharpCode.AvalonEdit.Edi
{
    using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.Rendering;
    using System.Windows.Media;
    using System.Windows;

    /// <summary>
    /// AvalonEdit: highlight current line even when not focused
    /// 
    /// Source: http://stackoverflow.com/questions/5072761/avalonedit-highlight-current-line-even-when-not-focused
    /// </summary>
    public class HighlightCurrentLineBackgroundRenderer : IBackgroundRenderer
    {
        private readonly TextEditor _Editor;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="highlightBackgroundColorBrush"></param>
        public HighlightCurrentLineBackgroundRenderer(TextEditor editor,
                                                      SolidColorBrush highlightBackgroundColorBrush = null)
        {
            this._Editor = editor;

            // Light Blue 0x100000FF
            this.BackgroundColorBrush = new SolidColorBrush((highlightBackgroundColorBrush == null ? Color.FromArgb(0x10, 0x80, 0x80, 0x80) :
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
            if (this._Editor.Document == null)
                return;

            textView.EnsureVisualLines();
            var currentLine = _Editor.Document.GetLineByOffset(_Editor.CaretOffset);

            foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, currentLine))
            {
                drawingContext.DrawRectangle(new SolidColorBrush(this.BackgroundColorBrush.Color), null,
                                             new Rect(rect.Location, new Size(textView.ActualWidth, rect.Height)));
            }
        }
    }
}
