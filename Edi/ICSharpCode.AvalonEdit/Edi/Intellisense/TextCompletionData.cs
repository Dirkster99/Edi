namespace ICSharpCode.AvalonEdit.Edi.Intellisense
{
  using System;
  using System.Windows.Media;
  using ICSharpCode.AvalonEdit.CodeCompletion;
  using ICSharpCode.AvalonEdit.Document;
  using ICSharpCode.AvalonEdit.Editing;

  /// <summary>
  /// Class to handle text completion data.
  /// </summary>
  public class TextCompletionData : ICompletionData
	{
    #region fields
		private readonly double _priority;
		private readonly string _header;
		private readonly string _text;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    /// <param name="header"></param>
    /// <param name="text"></param>
    /// <param name="priority"></param>
		public TextCompletionData(string header, string text, double priority = 0)
		{
			_header = header;
			_text = text;
			_priority = priority;
		}
    #endregion fields

    #region properties
    /// <summary>
    /// Get images source of completion (always null).
    /// </summary>
    public ImageSource Image
		{
			get { return null; }
		}

    /// <summary>
    /// Get text completion
    /// </summary>
		public string Text
		{
			get { return _text; }
		}

    /// <summary>
    /// Get content of text completion.
    /// </summary>
		public object Content
		{
			get { return _header; }
		}

    /// <summary>
    /// Get description of text completion.
    /// </summary>
		public object Description
		{
			get { return Text; }
		}

    /// <summary>
    /// Get priority of text completion.
    /// </summary>
		public double Priority
		{
			get { return _priority; }
		}
    #endregion properties

    #region methods
    /// <summary>
    /// Method is executed to complete the text in a <paramref name="completionSegment"/>.
    /// </summary>
    /// <param name="textArea"></param>
    /// <param name="completionSegment"></param>
    /// <param name="insertionRequestEventArgs"></param>
    public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
		{
			textArea.Document.Replace(completionSegment, Text);
		}
    #endregion methods
  }
}