﻿namespace ICSharpCode.AvalonEdit.Edi.PrintEngine
{
  using System.Printing;
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  /// Represents the PrintPreviewDocumentViewer class with PrintQueue and PrintTicket properties for the document viewer.
  /// </summary>
  [TemplatePart(Name = "PART_ContentHost", Type = typeof(ScrollViewer))]
  [TemplatePart(Name = "PART_FindToolBarHost", Type = typeof(ContentControl))]
  public class PrintPreviewDocumentViewer : DocumentViewer
  {
    #region fields

      #endregion fields

    #region constructor
    /// <summary>
    /// Static constructor
    /// </summary>
    static PrintPreviewDocumentViewer()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PrintPreviewDocumentViewer),
                                               new FrameworkPropertyMetadata(typeof(PrintPreviewDocumentViewer)));
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Gets or sets the print queue manager.
    /// </summary>
    public PrintQueue PrintQueue { get; set; } = LocalPrintServer.GetDefaultPrintQueue();

      /// <summary>
    /// Gets or sets the print settings for the print job.
    /// </summary>
    public PrintTicket PrintTicket { get; set; }

      #endregion properties

    #region methods
    /// <summary>
    /// Get a print dialog, defaulted to default printer and default printer's preferences.
    /// </summary>
    protected override void OnPrintCommand()
    {
            // get a print dialog, defaulted to default printer and default printer's preferences.
            PrintDialog printDialog = new PrintDialog
            {
                PrintQueue = PrintQueue,

                PrintTicket = PrintTicket
            };

            if (printDialog.ShowDialog() == true)
      {
        PrintQueue = printDialog.PrintQueue;

        PrintTicket = printDialog.PrintTicket;

        printDialog.PrintDocument(Document.DocumentPaginator, "PrintPreviewJob");
      }
    }
    #endregion methods
  }
}
