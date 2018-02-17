namespace ICSharpCode.AvalonEdit.Edi.BlockSurround
{
  using System;
  using System.Linq;
  using Document;
  using Editing;

  /// <summary>
  /// Expand selection with intro and outro text elements
  /// This can be useful when commenting out more than one line
  /// or converting plain text to HTML, XML, or any other tag based encoding
  /// 
  /// Source: http://blog.tabsstudio.com/2010/10/24/using-avalonedit-as-a-style-editor-in-tabs-studio/
  /// </summary>
  class AvalonHelper
  {
    /// <summary>
    /// Insert a start string and an end string
    /// before and after the currently selected text.
    /// </summary>
    /// <param name="editor"></param>
    /// <param name="block"></param>
    public static void SurroundSelectionWithBlockComment(EdiTextEditor editor,
                                                         BlockDefinition block)
    {
      if (editor == null)
        return;

            // Rectangle selection is a different deal...
            if ((editor.TextArea.Selection is RectangleSelection sel))
      {
// Single line block selection can be treaded like normal selection
        if (sel.IsMultiline)
        {
          SurroundRectangleSelectionWithBlockComment(editor, block, sel);
          return;
        }
      }

      // BlockSurround the current cursor location if there is no current selection
      ////if (editor.SelectionLength == 0)
      ////  return;

      // Attempt to find the currently set comment before and after the current selection
      TextReplaceBlockRegion region = null;

      switch (block.TypeOfBlock)
      {
        case BlockDefinition.BlockAt.Start:
          region = FindSelectedStartCommentRegion(block.StartBlock,
                                                  editor.Document,
                                                  editor.SelectedText, editor.SelectionStart, editor.SelectionLength);
          break;
        case BlockDefinition.BlockAt.End:
          region = FindSelectedEndCommentRegion(block.EndBlock,
                                                editor.Document,
                                                editor.SelectedText,
                                                editor.SelectionStart);
          break;
        case BlockDefinition.BlockAt.StartAndEnd:
          region = FindSelectedCommentRegion(block.StartBlock, block.EndBlock,
                                             editor.Document,
                                             editor.SelectedText, editor.SelectionStart, editor.SelectionLength);

          break;

        default:
          throw new NotImplementedException(block.TypeOfBlock.ToString());
      }

      // Remove the block surround (comment) if there is a match available
      if (region != null)
      {
        switch (block.TypeOfBlock)
        {
          case BlockDefinition.BlockAt.Start:
            editor.Document.Remove(region.StartOffset, region.CommentStart.Length);
            break;
          case BlockDefinition.BlockAt.End:
            editor.Document.Remove(region.EndOffset, region.CommentEnd.Length);
            break;
          case BlockDefinition.BlockAt.StartAndEnd:
            editor.Document.Remove(region.EndOffset, region.CommentEnd.Length);
            editor.Document.Remove(region.StartOffset, region.CommentStart.Length);
            break;

          default:
            throw new NotImplementedException(block.TypeOfBlock.ToString());
        }
      }
      else
      {
        int startOffset = editor.SelectionStart;
        int endOffset = editor.SelectionStart + editor.SelectionLength;
        int lenOffset = editor.SelectionLength;

        switch (block.TypeOfBlock)
        {
          case BlockDefinition.BlockAt.Start:
            editor.Document.Insert(startOffset, block.StartBlock);
            break;
          case BlockDefinition.BlockAt.End:
            editor.Document.Insert(endOffset, block.EndBlock);
            break;
          case BlockDefinition.BlockAt.StartAndEnd:    // Insert a new comment since we could not find one ...
            editor.Document.Insert(endOffset, block.EndBlock);
            editor.Document.Insert(startOffset, block.StartBlock);
            break;

          default:
            throw new NotImplementedException(block.TypeOfBlock.ToString());
        }

        // Reset selection to keep the text that was originally selected
        editor.Select(startOffset + block.StartBlock.Length, lenOffset);
      }
    }

    private static void SurroundRectangleSelectionWithBlockComment(EdiTextEditor editor,
                                                                   BlockDefinition block,
                                                                   RectangleSelection sel)
    {
      if (sel == null)
        return;

      // Backup current view position of rectangular selection
      int selectionStart = editor.SelectionStart;
      int selectionLength = editor.SelectionLength;
      int caretOffset = editor.CaretOffset;

      var startPos = new TextViewPosition(sel.StartPosition.Line, sel.StartPosition.Column, sel.StartPosition.VisualColumn);
      var endPos = new TextViewPosition(sel.EndPosition.Line, sel.EndPosition.Column, sel.EndPosition.VisualColumn);

      TextReplaceBlockRegion[] region = new TextReplaceBlockRegion[sel.Segments.Count()];
      bool bFoundNoMatch = true;

      for (int i = sel.Segments.Count() - 1; i >= 0; i--)
      {
        var item = sel.Segments.ElementAt(i);

        // Attempt to find the currently set comment before and after the current selection
        switch (block.TypeOfBlock)
        {
          case BlockDefinition.BlockAt.Start:
            region[i] = FindSelectedStartCommentRegion(block.StartBlock,
                                                       editor.Document,
                                                       editor.Document.GetText(item),
                                                       item.StartOffset,
                                                       item.Length);
            break;

          case BlockDefinition.BlockAt.End:
            region[i] = FindSelectedEndCommentRegion(block.EndBlock,
                                                     editor.Document,
                                                     editor.Document.GetText(item),
                                                     item.StartOffset);
            break;

          case BlockDefinition.BlockAt.StartAndEnd:
            region[i] = FindSelectedCommentRegion(block.StartBlock,
                                                  block.EndBlock,
                                                  editor.Document,
                                                  editor.Document.GetText(item),
                                                  item.StartOffset,
                                                  item.Length);
            break;

          default:
            throw new NotImplementedException(block.TypeOfBlock.ToString());
        }

        if (region[i] == null)
          bFoundNoMatch = false;
      }

      if (bFoundNoMatch)
      {
        for (int i = sel.Segments.Count() - 1; i >= 0; i--)
        {
          var item = sel.Segments.ElementAt(i);

          // Remove the block surround (comment) if there is a match available
          switch (block.TypeOfBlock)
          {
            case BlockDefinition.BlockAt.Start:
              editor.Document.Remove(region[i].StartOffset, region[i].CommentStart.Length);
              break;

            case BlockDefinition.BlockAt.End:
              editor.Document.Remove(region[i].EndOffset, region[i].CommentEnd.Length);
              break;

            case BlockDefinition.BlockAt.StartAndEnd:
              editor.Document.Remove(region[i].EndOffset, region[i].CommentEnd.Length);
              editor.Document.Remove(region[i].StartOffset, region[i].CommentStart.Length);
              break;

            default:
              throw new NotImplementedException(block.TypeOfBlock.ToString());
          }
        }
      }
      else
      {
        for (int i = sel.Segments.Count() - 1; i >= 0; i--)
        {
          var item = sel.Segments.ElementAt(i);

          switch (block.TypeOfBlock)
          {
            case BlockDefinition.BlockAt.Start:
              editor.Document.Insert(item.StartOffset, block.StartBlock);
              break;

            case BlockDefinition.BlockAt.End:
              editor.Document.Insert(item.EndOffset, block.EndBlock);
              break;

            case BlockDefinition.BlockAt.StartAndEnd:    // Insert a new comment since we could not find one ...
              editor.Document.Insert(item.EndOffset, block.EndBlock);
              editor.Document.Insert(item.StartOffset, block.StartBlock);
              break;

            default:
              throw new NotImplementedException(block.TypeOfBlock.ToString());
          }
        }

        // Move original selection to the rigth and apply rectangular selection
        editor.CaretOffset = caretOffset;

        editor.SelectionStart = selectionStart;
        editor.SelectionLength = selectionLength;

        //startPos.Column += block.StartBlock.Length;
        //endPos.Column += block.StartBlock.Length;

        // Reset selection to keep the text that was originally selected
        editor.TextArea.Selection = new RectangleSelection(editor.TextArea, startPos, endPos);
      }
    }

    /// <summary>
    /// Search in the current selection and before/after the current selection to determine
    /// whether a start/end comment match can be found there. Return the matched positions
    /// for uncommentin if so.
    /// 
    /// This can include  a position before the current selection and a position at the end
    /// of the current selection if the user has partially selected a commented area.
    /// </summary>
    /// <param name="commentStart"></param>
    /// <param name="commentEnd"></param>
    /// <param name="document"></param>
    /// <param name="selectedText"></param>
    /// <param name="selectionLength"></param>
    /// <param name="selectionStart"></param>
    /// <returns></returns>
    public static TextReplaceBlockRegion FindSelectedCommentRegion(string commentStart,
                                                                   string commentEnd,
                                                                   TextDocument document,
                                                                   string selectedText,
                                                                   int selectionStart,
                                                                   int selectionLength
                                                                   )
    {
      if (document == null)
        return null;

      ////if (document.TextLength == 0)
      ////  return null;

      // Find start of comment in selected text.
      int commentEndOffset = -1;

      int commentStartOffset = commentStart.IndexOf(selectedText);
      if (commentStartOffset >= 0)
        commentStartOffset += selectionStart;

      // Find end of comment in selected text.
      if (commentStartOffset >= 0)
        commentEndOffset = selectedText.IndexOf(commentEnd, commentStartOffset + commentStart.Length - selectionStart);
      else
        commentEndOffset = selectedText.IndexOf(commentEnd);

      if (commentEndOffset >= 0)
        commentEndOffset += selectionStart;

      // Find start of comment before or partially inside the selected text.
        if (commentStartOffset == -1)
      {
        int offset = selectionStart + selectionLength + commentStart.Length - 1;
        if (offset > document.TextLength)
          offset = document.TextLength;

        string text = document.GetText(0, offset);
        commentStartOffset = text.LastIndexOf(commentStart);
        if (commentStartOffset >= 0)
        {
            // Find end of comment before comment start.
            var commentEndBeforeStartOffset = text.IndexOf(commentEnd, commentStartOffset, selectionStart - commentStartOffset);

            if (commentEndBeforeStartOffset > commentStartOffset)
            commentStartOffset = -1;
        }
      }

      // Find end of comment after or partially after the selected text.
      if (commentEndOffset == -1)
      {
        int offset = selectionStart + 1 - commentEnd.Length;
        if (offset < 0)
          offset = selectionStart;

        string text = document.GetText(offset, document.TextLength - offset);

        commentEndOffset = text.IndexOf(commentEnd);

        if (commentEndOffset >= 0)
          commentEndOffset += offset;
      }

      if (commentStartOffset != -1 && commentEndOffset != -1)
        return new TextReplaceBlockRegion(commentStart, commentEnd, commentStartOffset, commentEndOffset);

      return null;
    }

    public static TextReplaceBlockRegion FindSelectedStartCommentRegion(string commentStart,
                                                                        TextDocument document,
                                                                        string selectedText,
                                                                        int selectionStart,
                                                                        int selectionLength)
    {
      if (document == null)
        return null;

      ////if (document.TextLength == 0)
      ////  return null;

      // Find start of comment in selected text.
      int commentStartOffset = selectedText.IndexOf(commentStart);
      if (commentStartOffset >= 0)
        commentStartOffset += selectionStart;

      // Find start of comment before or partially inside the selected text.
      if (commentStartOffset == -1)
      {
        int offset = selectionStart + selectionLength + commentStart.Length - 1;
        if (offset > document.TextLength)
          offset = document.TextLength;

        string text = document.GetText(0, offset);
        commentStartOffset = text.LastIndexOf(commentStart);
      }

      if (commentStartOffset != -1)
        return new TextReplaceBlockRegion(commentStart, string.Empty, commentStartOffset, -1);

      return null;
    }

    public static TextReplaceBlockRegion FindSelectedEndCommentRegion(string commentEnd,
                                                                      TextDocument document,
                                                                      string selectedText,
                                                                      int selectionStart)
    {
      if (document == null)
        return null;

      ////if (document.TextLength == 0)
      ////  return null;

      // Find start of comment in selected text.
      int commentEndOffset = -1;

      commentEndOffset = selectedText.IndexOf(commentEnd);

      if (commentEndOffset >= 0)
        commentEndOffset += selectionStart;

      // Find end of comment after or partially after the selected text.
      if (commentEndOffset == -1)
      {
        int offset = selectionStart + 1 - commentEnd.Length;
        if (offset < 0)
          offset = selectionStart;

        string text = document.GetText(offset, document.TextLength - offset);

        commentEndOffset = text.IndexOf(commentEnd);

        if (commentEndOffset >= 0)
          commentEndOffset += offset;
      }

      if (commentEndOffset != -1)
        return new TextReplaceBlockRegion(string.Empty, commentEnd, -1, commentEndOffset);

      return null;
    }
  }
}
