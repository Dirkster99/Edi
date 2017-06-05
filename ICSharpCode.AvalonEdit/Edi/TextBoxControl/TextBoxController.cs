namespace ICSharpCode.AvalonEdit.Edi.TextBoxControl
{
  using System;

  /// <summary>
  /// This class implements the ITextBoxController interface
  /// which can be used to connect viewmodel and view to tell
  /// the view about the text that should be shown and selected
  /// in the edit control.
  /// </summary>
  public class TextBoxController : ITextBoxController
  {
    #region Events
    /// <summary>
    /// Select all the text visible in the text control.
    /// </summary>
    public event SelectAllEventHandler SelectAll;

    /// <summary>
    /// Execute select event to be propagated into view via dependency property.
    /// </summary>
    public event SelectEventHandler Select;

    /// <summary>
    /// Scroll to line n in a tex file into view.
    /// </summary>
    public event ScrollToLineEventHandler ScrollToLineEvent;

    /// <summary>
    /// Execute current selection event to be propagated into view via dependency property
    /// </summary>
    public event CurrentSelectionEventHandler CurrentSelectionEvent;

    /// <summary>
    /// Start an edit event (useful for combining multiple changes into one undo step).
    /// </summary>
    public event BeginChangeEventHandler BeginChangeEvent;

    /// <summary>
    /// End an edit event (useful for combining multiple changes into one undo step).
    /// </summary>
    public event EndChangeEventHandler EndChangeEvent;

    /// <summary>
    /// Get the text that is currently selected in the edit control.
    /// </summary>
    public event GetSelectedTextEventHandler GetSelectedTextEvent;
    #endregion Events

    #region methods
    /// <summary>
    /// Select all the text visible in the text control.
    /// </summary>
    public void SelectAllText()
    {
      if (this.SelectAll != null)
        this.SelectAll(this);
    }

    /// <summary>
    /// Execute select event to be propagated into view via dependency property.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="length"></param>
    public void SelectText(int start, int length)
    {
      if (this.Select != null)
      {
        this.Select(this, start, length);
      }
    }

    /// <summary>
    /// Scroll to line n in a tex file into view.
    /// </summary>
    /// <param name="line"></param>
    public void ScrollToLine(int line)
    {
      if (this.ScrollToLineEvent != null)
        this.ScrollToLineEvent(this, line);
    }

    /// <summary>
    /// Execute current selection event to be propagated into view via dependency property
    /// </summary>
    /// <param name="start"></param>
    /// <param name="length"></param>
    /// <param name="IsRectengularSelection"></param>
    public void CurrentSelection(out int start, out int length, out bool IsRectengularSelection)
    {
      start = length = 0;
      IsRectengularSelection = false;

      if (this.CurrentSelectionEvent != null)
        this.CurrentSelectionEvent(this, out start, out length, out IsRectengularSelection);
    }

    /// <summary>
    /// Start an edit event (useful for combining multiple changes into one undo step).
    /// </summary>
    public void BeginChange()
    {
      if (this.BeginChangeEvent != null)
        this.BeginChangeEvent(this);
    }

    /// <summary>
    /// End an edit event (useful for combining multiple changes into one undo step).
    /// </summary>
    public void EndChange()
    {
      if (this.EndChangeEvent != null)
        this.EndChangeEvent(this);
    }

    /// <summary>
    /// Get the text that is currently selected in the edit control.
    /// </summary>
    /// <param name="selectedText"></param>
    public void GetSelectedText(out string selectedText)
    {
      selectedText = string.Empty;

      if (this.GetSelectedTextEvent != null)
        this.GetSelectedTextEvent(this, out selectedText);
    }
    #endregion methods
  }
}
