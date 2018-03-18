namespace ICSharpCode.AvalonEdit.Edi.PrintEngine
{
  using System.IO;
  // this *** needs System.Printing reference
  using System.Windows;
  using System.Windows.Documents;
  using System.Windows.Xps;
  using System.Windows.Xps.Packaging; // these bastards are hidden in the ReachFramework reference

  /// <summary>
  /// Represents the PrintPreviewDialog class to preview documents
  /// of type FlowDocument, IDocumentPaginatorSource or DocumentPaginatorWrapper
  /// using the PrintPreviewDocumentViewer class.
  /// </summary>
  public partial class PrintPreviewDialog : Window
  {
    #region fields
    private object mDocument;
    #endregion fields

    #region constructor
    /// <summary>
    /// Initialize a new instance of the PrintEngine.PrintPreviewDialog class.
    /// </summary>
    public PrintPreviewDialog()
    {
      InitializeComponent();
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Gets or sets the document viewer.
    /// </summary>
    public PrintPreviewDocumentViewer DocumentViewer
    {
      get { return documentViewer; }
      set { documentViewer = value; }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Loads the specified FlowDocument document for print preview.
    /// </summary>
    public void LoadDocument(FlowDocument document)
    {
      mDocument = document;

      string temp = Path.GetTempFileName();

      if (File.Exists(temp) == true)
        File.Delete(temp);

      XpsDocument xpsDoc = new XpsDocument(temp, FileAccess.ReadWrite);

      XpsDocumentWriter xpsWriter = XpsDocument.CreateXpsDocumentWriter(xpsDoc);

      xpsWriter.Write(((FlowDocument)document as IDocumentPaginatorSource).DocumentPaginator);

      documentViewer.Document = xpsDoc.GetFixedDocumentSequence();

      xpsDoc.Close();
    }

    /// <summary>
    /// Loads the specified DocumentPaginatorWrapper document for print preview.
    /// </summary>
    public void LoadDocument(DocumentPaginatorWrapper document)
    {
      mDocument = document;

      string temp = Path.GetTempFileName();

      if (File.Exists(temp) == true)
        File.Delete(temp);

      XpsDocument xpsDoc = new XpsDocument(temp, FileAccess.ReadWrite);

      XpsDocumentWriter xpsWriter = XpsDocument.CreateXpsDocumentWriter(xpsDoc);

      xpsWriter.Write(document);

      documentViewer.Document = xpsDoc.GetFixedDocumentSequence();

      xpsDoc.Close();
    }

    /// <summary>
    /// Loads the specified IDocumentPaginatorSource document for print preview.
    /// </summary>
    public void LoadDocument(IDocumentPaginatorSource document)
    {
      mDocument = document;
      documentViewer.Document = (IDocumentPaginatorSource)document;
    }

    private void closeButton_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }
    #endregion methods
  }
}
