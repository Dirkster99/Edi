namespace ICSharpCode.AvalonEdit.Edi.Folding
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Data;
  using System.Linq;
  using System.Diagnostics;
  using AvalonEdit.Folding;
  using Document;

  /// <summary>
  /// Folding Strategy for Visual Basic
  /// 
  /// Source: http://stackoverflow.com/questions/11023569/avalonedit-folding-strategy-for-vb
  /// </summary>
  public class VBNetFoldingStrategy : AbstractFoldingStrategy
  {
    #region fields
    private string[] foldableKeywords =
	  {
		  "namespace",
		  "class",
		  "sub",
		  "function",
		  "structure",
		  "enum"
	  };
    #endregion fields

    #region methods
    /// <summary>
    /// Create new folding for a givem visual basic document.
    /// </summary>
    /// <param name="document"></param>
    /// <param name="firstErrorOffset"></param>
    /// <returns></returns>
    public override IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
    {
      firstErrorOffset = -1;

      List<NewFolding> foldings = new List<NewFolding>();

      string text = document.Text;
      string lowerCaseText = text.ToLower();

      foreach (string foldableKeyword in foldableKeywords)
      {
        foldings.AddRange(GetFoldings(text, lowerCaseText, foldableKeyword));
      }

      return foldings.OrderBy(f => f.StartOffset);
    }

    /// <summary>
    /// Create new folding for a givem visual basic string representation.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="lowerCaseText"></param>
    /// <param name="keyword"></param>
    /// <returns></returns>
    public IEnumerable<NewFolding> GetFoldings(string text, string lowerCaseText, string keyword)
    {
      const char vbCr = '\r';
      const char vbLf = '\n';
      ////const string vbCrLf = "\r\n";

      List<NewFolding> foldings = new List<NewFolding>();

      string closingKeyword = "end " + keyword;
      int closingKeywordLength = closingKeyword.Length;

      keyword += " ";
      int keywordLength = keyword.Length;

      Stack<int> startOffsets = new Stack<int>();

      for (int i = 0; i <= text.Length - closingKeywordLength; i++)
      {
        if (lowerCaseText.Substring(i, keywordLength) == keyword)
        {
          int k = i;
          if (k > 0)
          {
            int lastLetterPos = i;
            while (!(text[k - 1] == vbLf || text[k - 1] == vbCr))
            {
              if (char.IsLetter(text[k]))
                lastLetterPos = k;

              k -= 1;

              if (k == 0)
                break;
            }

            //if (lastLetterPos > k) Dirkster99 NOT_APPLIED Not sure why this is necessary
            //  k = lastLetterPos;
          }

          startOffsets.Push(k);
        }
        else if (lowerCaseText.Substring(i, closingKeywordLength) == closingKeyword)
        {
          int startOffset = startOffsets.Pop();
          NewFolding newFolding = new NewFolding(startOffset, i + closingKeywordLength);
          int p = text.IndexOf(vbLf, startOffset);

          if (p == -1)
            p = text.IndexOf(vbCr, startOffset);

          if (p == -1)
            p = text.Length - 1;

          newFolding.Name = text.Substring(startOffset, p - startOffset);
          foldings.Add(newFolding);
        }
      }

      return foldings;
    }
    #endregion methods

    /***
     * 
    /// <summary>
    /// Dirkster99
    /// Create a meaningful name of a folding (content of the complete first line)
    /// and set this as name of the folding.
    /// </summary>
    /// <param name="document"></param>
    /// <param name="foldStart"></param>
    private static string CreateElementFoldName(TextDocument document, XmlFoldStart foldStart)
    {
      const char vbCr = '\r';
      const char vbLf = '\n';

      int EndNameIndex = document.Text.IndexOf(vbLf, foldStart.StartOffset);

      if (EndNameIndex == -1)
        EndNameIndex = document.Text.IndexOf(vbCr, foldStart.StartOffset);

      if (EndNameIndex == -1)
        EndNameIndex = document.Text.Length - 1;

      return document.Text.Substring(foldStart.StartOffset, EndNameIndex - foldStart.StartOffset);
    }
     * 
     ***/
  }
}
