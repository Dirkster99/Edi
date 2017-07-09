namespace ICSharpCode.AvalonEdit.Edi.Intellisense
{
  using ICSharpCode.AvalonEdit.CodeCompletion;

  /// <summary>
  /// Interface to window that is used to resolve incomplete words that were typed by the user.
  /// </summary>
  public interface ICompletionWindowResolver
	{
    /// <summary>
    /// Get the window to resolve a text completion.
    /// </summary>
    /// <returns></returns>
		CompletionWindow Resolve();
	}
}