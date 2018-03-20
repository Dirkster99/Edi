namespace ICSharpCode.AvalonEdit.BracketRenderer
{
  /// <summary>
  /// Describes a pair of matching brackets found by IBracketSearcher.
  /// </summary>
  public class BracketSearchResult
  {
    #region class constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    /// <param name="openingBracketOffset"></param>
    /// <param name="openingBracketLength"></param>
    /// <param name="closingBracketOffset"></param>
    /// <param name="closingBracketLength"></param>
    public BracketSearchResult(int openingBracketOffset, int openingBracketLength,
                               int closingBracketOffset, int closingBracketLength)
    {
      this.OpeningBracketOffset = openingBracketOffset;
      this.OpeningBracketLength = openingBracketLength;
      this.ClosingBracketOffset = closingBracketOffset;
      this.ClosingBracketLength = closingBracketLength;
    }
    #endregion class constructor

    #region properties
    /// <summary>
    /// Text offset of the opening/starting bracket.
    /// </summary>
    public int OpeningBracketOffset { get; private set; }

    /// <summary>
    /// Length of the opening/starting bracket.
    /// </summary>
    public int OpeningBracketLength { get; private set; }

    /// <summary>
    /// Text offset of the closing/ending bracket.
    /// </summary>
    public int ClosingBracketOffset { get; private set; }

    /// <summary>
    /// Length of the closing/ending bracket.
    /// </summary>
    public int ClosingBracketLength { get; private set; }
    #endregion properties
  }
}
