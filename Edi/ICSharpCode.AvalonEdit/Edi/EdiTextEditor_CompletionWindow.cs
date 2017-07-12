namespace ICSharpCode.AvalonEdit.Edi
{
  using System.Windows.Input;

  using ICSharpCode.AvalonEdit.CodeCompletion;
  using ICSharpCode.AvalonEdit.Edi.Intellisense;

  /// <summary>
  /// This part of the AvalonEdit externsion contains methods that
  /// deal with Completion window handling (opening and closing it as the user types).
  /// </summary>
  public partial class EdiTextEditor : TextEditor
  {
    private CompletionWindow _completionWindow;

    void TextEditorTextAreaTextEntered(object sender, TextCompositionEventArgs e)
    {
      ICompletionWindowResolver resolver = new CompletionWindowResolver(this.Text, this.CaretOffset, e.Text, this);
      _completionWindow = resolver.Resolve();
    }

    void TextEditorTextAreaTextEntering(object sender, TextCompositionEventArgs e)
    {
      if (e.Text.Length > 0 && _completionWindow != null)
      {
        if (!char.IsLetterOrDigit(e.Text[0]))
        {
          _completionWindow.CompletionList.RequestInsertion(e);
        }
      }
    }
  }
}
