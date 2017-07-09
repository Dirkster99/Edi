namespace ICSharpCode.AvalonEdit.Edi.BlockSurround
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  /// <summary>
  /// This class represents a surrounding text being inserted before and after
  /// a certain text block within the text displayed in the editor.
  /// 
  /// Source: http://blog.tabsstudio.com/2010/10/24/using-avalonedit-as-a-style-editor-in-tabs-studio/
  /// </summary>
  class TextReplaceBlockRegion
  {
    /// <summary>
    /// Class Constructor
    /// 
    /// The end offset is the offset where the comment end string starts from.
    /// </summary>
    public TextReplaceBlockRegion(string commentStart, string commentEnd, int startOffset, int endOffset)
    {
      this.CommentStart = commentStart;
      this.CommentEnd = commentEnd;
      this.StartOffset = startOffset;
      this.EndOffset = endOffset;
    }

    #region properties
    /// <summary>
    /// Represents the string that starts the <see cref="TextReplaceBlockRegion"/>
    /// </summary>
    public string CommentStart { get; private set; }

    /// <summary>
    /// Represents the string that ends the <see cref="TextReplaceBlockRegion"/>
    /// </summary>
    public string CommentEnd { get; private set; }

    /// <summary>
    /// Represents the text offset where the <see cref="TextReplaceBlockRegion"/>
    /// </summary>
    public int StartOffset { get; private set; }

    /// <summary>
    /// Represents the text offset where the <see cref="TextReplaceBlockRegion"/>
    /// </summary>
    public int EndOffset { get; private set; }
    #endregion properties

    #region methods
    /// <summary>
    /// Determine the hash code for this object.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
      int hashCode = 0;
      unchecked
      {
        if (CommentStart != null)
          hashCode += 1000000007 * CommentStart.GetHashCode();

        if (CommentEnd != null)
          hashCode += 1000000009 * CommentEnd.GetHashCode();

        hashCode += 1000000021 * StartOffset.GetHashCode();
        hashCode += 1000000033 * EndOffset.GetHashCode();
      }
      return hashCode;
    }

    /// <summary>
    /// Determine whether this object equals with the other object or not.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
      TextReplaceBlockRegion other = obj as TextReplaceBlockRegion;

      if (other == null)
        return false;

      return this.CommentStart == other.CommentStart &&
              this.CommentEnd == other.CommentEnd &&
              this.StartOffset == other.StartOffset &&
              this.EndOffset == other.EndOffset;
    }
    #endregion methods
  }
}
