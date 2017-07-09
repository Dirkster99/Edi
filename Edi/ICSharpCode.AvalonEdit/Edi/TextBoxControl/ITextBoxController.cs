namespace ICSharpCode.AvalonEdit.Edi.TextBoxControl
{
  /// <summary>
  /// Define a deligate method that is called for processing the SelectAll event.
  /// </summary>
  /// <param name="sender"></param>
  public delegate void SelectAllEventHandler(ITextBoxController sender);

  /// <summary>
  /// Define a deligate method that is called for processing the Select event.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="start"></param>
  /// <param name="lenght"></param>
  public delegate void SelectEventHandler(ITextBoxController sender, int start, int lenght);

  /// <summary>
  /// Delegate event to scroll to a line
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="line"></param>
  public delegate void ScrollToLineEventHandler(ITextBoxController sender, int line);

  /// <summary>
  /// Delegate event to get the currently selected text.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="start"></param>
  /// <param name="length"></param>
  /// <param name="IsRectengularSelection"></param>
  public delegate void CurrentSelectionEventHandler(ITextBoxController sender,
                                                    out int start, out int length,
                                                    out bool IsRectengularSelection);

  /// <summary>
  /// Delegate event to begin change to implement several changes
  /// and call end change (enables undo in one step).
  /// </summary>
  /// <param name="sender"></param>
  public delegate void BeginChangeEventHandler(ITextBoxController sender);

  /// <summary>
  /// Delegate event to end change (enables undo to undo changes
  /// since last begin change event - if any).
  /// </summary>
  /// <param name="sender"></param>
  public delegate void EndChangeEventHandler(ITextBoxController sender);

  /// <summary>
  /// Delegate event to get the currently selected text.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="selectedText"></param>
  public delegate void GetSelectedTextEventHandler(ITextBoxController sender, out string selectedText);

  /// <summary>
  /// Define an interface that must be adhered to when establishing
  /// comunication between ViewModel and attached property.
  /// </summary>
  public interface ITextBoxController
  {
    /// <summary>
    /// Define a deligate method that is called for processing the SelectAll event.
    /// </summary>
    event SelectAllEventHandler SelectAll;

    /// <summary>
    /// Define a deligate method that is called for processing the Select event.
    /// </summary>
    event SelectEventHandler Select;

    /// <summary>
    /// Delegate event to scroll to a line
    /// </summary>
    event ScrollToLineEventHandler ScrollToLineEvent;

    /// <summary>
    /// Delegate event to get the currently selected text.
    /// </summary>
    event CurrentSelectionEventHandler CurrentSelectionEvent;

    /// <summary>
    /// Delegate event to begin change to implement several changes
    /// and call end change (enables undo in one step).
    /// </summary>
    event BeginChangeEventHandler BeginChangeEvent;

    /// <summary>
    /// Delegate event to end change (enables undo to undo changes
    /// since last begin change event - if any).
    /// </summary>
    event EndChangeEventHandler EndChangeEvent;

    /// <summary>
    /// Delegate event to get the currently selected text.
    /// </summary>
    event GetSelectedTextEventHandler GetSelectedTextEvent;
  }
}
