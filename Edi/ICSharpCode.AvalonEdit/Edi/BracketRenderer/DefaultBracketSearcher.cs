namespace ICSharpCode.AvalonEdit.BracketRenderer
{
  /// <summary>
  /// Class is used to highlight brackets within the text that is being edit.
  /// The user can position his cursor after a bracket character '(' or ')'
  /// and the editor will highlight both characters
  /// (considering inner recursive bracket pairs) if they are visible.
  /// </summary>
  public class DefaultBracketSearcher : IBracketSearcher
  {
    /// <summary>
    /// Start a service implementation of a bracket searcher class.
    /// </summary>
    public static readonly DefaultBracketSearcher DefaultInstance = new DefaultBracketSearcher();

    /// <summary>
    /// Empty implementation of this method to satisfy <seealso cref="IBracketSearcher"/> interface requirements.
    /// </summary>
    /// <param name="document"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public BracketSearchResult SearchBracket(ICSharpCode.AvalonEdit.Document.TextDocument document, int offset)
    {
      return null;
    }
  }
}
