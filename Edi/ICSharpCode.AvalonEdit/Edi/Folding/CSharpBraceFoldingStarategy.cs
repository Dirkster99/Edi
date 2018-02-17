// Copyright (c) 2009 Daniel Grunwald
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

namespace ICSharpCode.AvalonEdit.Edi.Folding
{
  using System.Collections.Generic;
  using System.Text.RegularExpressions;
  using Document;
  using AvalonEdit.Folding;

  /// <summary>
  /// Allows producing foldings from a document based on braces.
  /// </summary>
  public class CSharpBraceFoldingStrategy : AbstractFoldingStrategy
  {
    /// <summary>
    /// Gets/Sets the opening brace. The default value is '{'.
    /// </summary>
    public char OpeningBrace { get; set; }

    /// <summary>
    /// Gets/Sets the closing brace. The default value is '}'.
    /// </summary>
    public char ClosingBrace { get; set; }

    /// <summary>
    /// Creates a new BraceFoldingStrategy.
    /// </summary>
    public CSharpBraceFoldingStrategy()
    {
      OpeningBrace = '{';
      ClosingBrace = '}';
    }

    /// <summary>
    /// Create <see cref="NewFolding"/>s for the specified document.
    /// </summary>
    public override IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
    {
      firstErrorOffset = -1;

      // Lets not crash the application over something as silly as foldings ...
      try
      {
        return CreateNewFoldings(document);
      }
      catch
      {
      }

      return new List<NewFolding>(); ;
    }

    /// <summary>
    /// Create <see cref="NewFolding"/>s for the specified document.
    /// </summary>
    public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
    {
      List<NewFolding> newFoldings = new List<NewFolding>();

      if (document == null)
        return newFoldings;

      Stack<int> startOffsets = new Stack<int>();
      int lastNewLineOffset = 0;

      char openingBrace = OpeningBrace;
      char closingBrace = ClosingBrace;

      for (int i = 0; i < document.TextLength; i++)
      {
        char c = document.GetCharAt(i);

        if (c == openingBrace)
        {
          startOffsets.Push(i);
        }
        else if (c == closingBrace && startOffsets.Count > 0)
        {
          int startOffset = startOffsets.Pop();

          // don't fold if opening and closing brace are on the same line
          if (startOffset < lastNewLineOffset)
          {
            newFoldings.Add(new NewFolding(startOffset, i + 1));
          }
        }
        else if (c == '\n' || c == '\r')
        {
          lastNewLineOffset = i + 1;
        }
      }

      Stack<FoldLine> LineOffsets = new Stack<FoldLine>();

      string[] lines = document.Text.Split('\n');

      if (lines != null)
      {
        int offset = 0;
        string reStartRegion = "([ ]*)?#region([ A-Za-z]*)?";
        string reEndRegion = "([ ]*)?#endregion([ A-Za-z]*)?";

        ////string reStartThreeSlashComment = "([ ])?[/][/][/].*";

        foreach (string item in lines)
        {
          if (Regex.Match(item, reStartRegion, RegexOptions.IgnoreCase).Success)
          {
            LineOffsets.Push(new FoldLine() { Name = item, Offste = offset, TypeOfFold = FoldType.Line});
          }
          else
          {
            if (Regex.Match(item, reEndRegion, RegexOptions.IgnoreCase).Success)
            {
              FoldLine Line = LineOffsets.Pop();

              // don't fold if opening and closing brace are on the same line
              if (Line.Offste < offset)
                newFoldings.Add(new NewFolding(Line.Offste, offset + (item.Length > 0 ? item.Length - 1 : 0))
                                { Name = Line.Name });
            }
          }
          
          offset += item.Length + 1;
        }
      }

      newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));

      return newFoldings;
    }

    private enum FoldType
    {
      Line = 1,
      ThreeSlashComment = 2
    }

    /// <summary>
    /// This class is just a small utility type class that helps recovering
    /// the Fold Name when creating a new fold from start to end offset.
    /// </summary>
    private class FoldLine
    {
      public int Offste { get; set; }
      public string Name { get; set; }

      public FoldType TypeOfFold { get; set; }
    }
  }
}
