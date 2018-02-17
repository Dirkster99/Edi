namespace ICSharpCode.AvalonEdit.Edi.PrintEngine
{
  using System;
  using System.Drawing.Printing;
  using System.Printing;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Documents;

  using Highlighting;
  using Document;
  using AvalonEdit;

  /// <summary>
  /// Print support class for Edi text editor control based ib AvalonEdit class.
  /// </summary>
  public static class Printing
  {
    private static PageSettings mPageSettings;
    private static PrintQueue mPrintQueue = LocalPrintServer.GetDefaultPrintQueue();
    private static PrintTicket mPrintTicket = mPrintQueue.DefaultPrintTicket;
    private static string mDocumentTitle;

    /// <summary>
    /// Invokes a PrintEngine.PrintPreviewDialog to print preview the TextEditor.Document.
    /// </summary>
    public static void PrintPreviewDialog(this TextEditor textEditor)
    {
      PrintPreviewDialog(textEditor, "");
    }

    /// <summary>
    /// Invokes a PrintEngine.PrintPreviewDialog to print preview the TextEditor.Document with specified title.
    /// </summary>
    public static void PrintPreviewDialog(this TextEditor textEditor, string title)
    {
      mDocumentTitle = title;

      InitPageSettings();

      PrintPreviewDialog printPreview = new PrintPreviewDialog();

      printPreview.DocumentViewer.FitToMaxPagesAcross(1);
      printPreview.DocumentViewer.PrintQueue = mPrintQueue;

      if (mPageSettings.Landscape)
        mPrintTicket.PageOrientation = PageOrientation.Landscape;

      printPreview.DocumentViewer.PrintTicket = mPrintTicket;
      printPreview.DocumentViewer.PrintQueue.DefaultPrintTicket.PageOrientation = mPrintTicket.PageOrientation;
      printPreview.LoadDocument(CreateDocumentPaginatorToPrint(textEditor));

      // this is stupid, but must be done to view a whole page:
      DocumentViewer.FitToMaxPagesAcrossCommand.Execute("1", printPreview.DocumentViewer);

      // we never get a return code 'true', since we keep the DocumentViewer open, until user closes the window
      printPreview.ShowDialog();

      mPrintQueue = printPreview.DocumentViewer.PrintQueue;
      mPrintTicket = printPreview.DocumentViewer.PrintTicket;
    }

    /// <summary>
    /// Invokes a System.Windows.Controls.PrintDialog to print the TextEditor.Document.
    /// </summary>
    public static void PrintDialog(this TextEditor textEditor)
    {
      PrintDialog(textEditor, "");
    }

    /// <summary>
    /// Invokes a System.Windows.Controls.PrintDialog to print the TextEditor.Document with specified title.
    /// </summary>
    public static void PrintDialog(this TextEditor textEditor, string title)
    {
      mDocumentTitle = title;

      InitPageSettings();

            PrintDialog printDialog = new PrintDialog
            {
                PrintQueue = mPrintQueue
            };

            if (mPageSettings.Landscape)
        mPrintTicket.PageOrientation = PageOrientation.Landscape;

      printDialog.PrintTicket = mPrintTicket;
      printDialog.PrintQueue.DefaultPrintTicket.PageOrientation = mPrintTicket.PageOrientation;

      if (printDialog.ShowDialog() == true)
      {
        mPrintQueue = printDialog.PrintQueue;

        mPrintTicket = printDialog.PrintTicket;

        printDialog.PrintDocument(CreateDocumentPaginatorToPrint(textEditor), "PrintJob");
      }
    }

    /// <summary>
    /// Prints the the TextEditor.Document to the current printer (no dialogs).
    /// </summary>
    public static void PrintDirect(this TextEditor textEditor)
    {
      PrintDirect(textEditor, "");
    }

    /// <summary>
    /// Prints the the TextEditor.Document to the current printer (no dialogs) with specified title.
    /// </summary>
    public static void PrintDirect(this TextEditor textEditor, string title)
    {
      mDocumentTitle = title;

      InitPageSettings();

            PrintDialog printDialog = new PrintDialog
            {
                PrintQueue = mPrintQueue
            };

            if (mPageSettings.Landscape)
        mPrintTicket.PageOrientation = PageOrientation.Landscape;

      printDialog.PrintTicket = mPrintTicket;
      printDialog.PrintQueue.DefaultPrintTicket.PageOrientation = mPrintTicket.PageOrientation;
      printDialog.PrintDocument(CreateDocumentPaginatorToPrint(textEditor), "PrintDirectJob");
    }

    /// <summary>
    /// If not initialized, initialize a new instance of the PageSettings and sets the default margins.
    /// </summary>
    static void InitPageSettings()
    {
      if (mPageSettings == null)
      {
                mPageSettings = new PageSettings
                {
                    Margins = new Margins(40, 40, 40, 40)
                };
            }
    }

    /// <summary>
    /// Creates a DocumentPaginatorWrapper from TextEditor text to print.
    /// </summary>
    static DocumentPaginatorWrapper CreateDocumentPaginatorToPrint(TextEditor textEditor)
    {
      // this baby adds headers and footers
      IDocumentPaginatorSource dps = CreateFlowDocumentToPrint(textEditor);

            DocumentPaginatorWrapper dpw = new DocumentPaginatorWrapper(dps.DocumentPaginator, mPageSettings, mPrintTicket, textEditor.FontFamily)
            {
                Title = mDocumentTitle
            };

            return dpw;
    }

    /// <summary>
    /// Creates a FlowDocument from TextEditor text to print.
    /// </summary>
    static FlowDocument CreateFlowDocumentToPrint(TextEditor textEditor)
    {
      // this baby has all settings to be printed or previewed in the PrintEngine.PrintPreviewDialog
      FlowDocument doc = CreateFlowDocumentForEditor(textEditor);

      doc.ColumnWidth = mPageSettings.PrintableArea.Width;
      doc.PageHeight = (mPageSettings.Landscape ? (int)mPrintTicket.PageMediaSize.Width : (int)mPrintTicket.PageMediaSize.Height);
      doc.PageWidth = (mPageSettings.Landscape ? (int)mPrintTicket.PageMediaSize.Height : (int)mPrintTicket.PageMediaSize.Width);
      doc.PagePadding = ConvertPageMarginsToThickness(mPageSettings.Margins);
      doc.FontFamily = textEditor.FontFamily;
      doc.FontSize = textEditor.FontSize;

      return doc;
    }

    /// <summary>
    /// Creates a FlowDocument from TextEditor text.
    /// </summary>
    static FlowDocument CreateFlowDocumentForEditor(TextEditor editor)
    {
      // ref.:  http://community.sharpdevelop.net/forums/t/12012.aspx
      IHighlighter highlighter = editor.TextArea.GetService(typeof(IHighlighter)) as IHighlighter;

      FlowDocument doc = new FlowDocument(ConvertTextDocumentToBlock(editor.Document, highlighter));

      return doc;
    }

    /// <summary>
    /// Converts a TextDocument to Block.
    /// </summary>
    static Block ConvertTextDocumentToBlock(TextDocument document, IHighlighter highlighter)
    {
      // ref.:  http://community.sharpdevelop.net/forums/t/12012.aspx
      if (document == null)
        throw new ArgumentNullException(nameof(document));

      Paragraph p = new Paragraph();

      foreach (DocumentLine line in document.Lines)
      {
        int lineNumber = line.LineNumber;

        HighlightedInlineBuilder inlineBuilder = new HighlightedInlineBuilder(document.GetText(line));

        if (highlighter != null)
        {
          HighlightedLine highlightedLine = highlighter.HighlightLine(lineNumber);

          int lineStartOffset = line.Offset;

          foreach (HighlightedSection section in highlightedLine.Sections)
            inlineBuilder.SetHighlighting(section.Offset - lineStartOffset, section.Length, section.Color);
        }

        p.Inlines.AddRange(inlineBuilder.CreateRuns());
        p.Inlines.Add(new LineBreak());
      }

      return p;
    }

    /// <summary>
    /// Converts PaperSize (hundredths of an inch) to PageMediaSize (px).
    /// </summary>
    static PageMediaSize ConvertPaperSizeToMediaSize(PaperSize paperSize)
    {
      return new PageMediaSize(ConvertToPx(paperSize.Width), ConvertToPx(paperSize.Height));
    }

    /// <summary>
    /// Converts specified Margins (hundredths of an inch) to Thickness (px).
    /// </summary>
    static Thickness ConvertPageMarginsToThickness(Margins margins)
    {
            Thickness thickness = new Thickness
            {
                Left = ConvertToPx(margins.Left),
                Top = ConvertToPx(margins.Top),
                Right = ConvertToPx(margins.Right),
                Bottom = ConvertToPx(margins.Bottom)
            };

            return thickness;
    }

    /// <summary>
    /// Converts specified inch (hundredths of an inch) to pixels (px).
    /// </summary>
    static double ConvertToPx(double inch)
    {
      return inch * 0.96;
    }
  }
}
