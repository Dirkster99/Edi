namespace ICSharpCode.AvalonEdit.Edi.Intellisense
{
  using System.Collections.Generic;
  using CodeCompletion;

  /// <summary>
  /// Data provider interface for completion window
  /// </summary>
  public interface ICompletionDataProvider
	{
    /// <summary>
    /// Get text completion data for a word at a certain position.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="position"></param>
    /// <param name="input"></param>
    /// <param name="highlightingName"></param>
    /// <returns></returns>
		IEnumerable<ICompletionData> GetData(string text, int position, string input, string highlightingName); 
	}
}