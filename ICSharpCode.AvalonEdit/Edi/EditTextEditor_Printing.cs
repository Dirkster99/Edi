namespace ICSharpCode.AvalonEdit.Edi
{
  using System.Windows.Input;
  using ICSharpCode.AvalonEdit.Edi.PrintEngine;

  public partial class EdiTextEditor : TextEditor
  {
    /// <summary>
    /// Executes the collapse all folds command (which folds all text foldings but the first).
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void PrintDocument(object sender, ExecutedRoutedEventArgs e)
    {
      EdiTextEditor edi = sender as EdiTextEditor;

      if (edi == null)
        return;

      string filename = null;
      try
      {
        if (e != null)
        {
          if (e.Parameter != null)
          {
            filename = e.Parameter as string;

            if (filename != null)
            {
              int MaxLen = 52;

              // Work with elipses if string is too long
              if (filename.Length > (MaxLen + 8))
                filename = filename.Substring(0, 5) + "..." + filename.Substring((filename.Length - MaxLen), MaxLen);
            }
          }
        }
      }
      catch
      {
      }

      edi.PrintPreviewDocument(filename);
    }

    /// <summary>
    /// Determines whether a folding command can be executed or not and sets correspondind
    /// <paramref name="e"/>.CanExecute property value.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void PrintDocumentCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = false;
      e.Handled = true;

      EdiTextEditor edi = sender as EdiTextEditor;

      if (edi == null)
        return;

      e.CanExecute = true;
    }

    private void PrintPreviewDocument(string printDocumentName = "")
    {
      // Printing.PageSetupDialog();              // .NET dialog

      Printing.PrintPreviewDialog(this, printDocumentName); // WPF print preview dialog

      /* Printing.PrintPreviewDialog(filename);   // WPF print preview dialog, filename as document title

      Printing.PrintDialog();                   // WPF print dialog

      Printing.PrintDialog(filename);          // WPF print dialog, filename as document title

      Printing.PrintDirect();                   // prints to default or previously selected printer

      Printing.PrintDirect(filename);           // prints to default or previously selected printer, filename as document title
       */
    }
  }
}
