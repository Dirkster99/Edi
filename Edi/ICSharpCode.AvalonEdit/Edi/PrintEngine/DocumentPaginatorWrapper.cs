namespace ICSharpCode.AvalonEdit.Edi.PrintEngine
{
  using System;
  using System.IO;
  using System.Printing;
  using System.Drawing.Printing;
  using System.Windows;
  using System.Windows.Media;
  using System.Windows.Documents;

  /// <summary>
  /// Represents the DocumentPaginatorWrapper adding headers and footers to the document.
  /// </summary>
  public class DocumentPaginatorWrapper : DocumentPaginator
  {
    #region fields
    string m_Title;
    Margins m_Margins;
    Size m_PageSize;
    DocumentPaginator m_Paginator;
    Typeface m_Typeface;
    #endregion fields

    #region Properties
    /// <summary>
    /// Sets the header title for each document page.
    /// </summary>
    public string Title
    {
      set => m_Title = value;
    }

    /// <summary>
    /// Gets whether the current page count is valid or not.
    /// </summary>
    public override bool IsPageCountValid => m_Paginator.IsPageCountValid;

      /// <summary>
    /// Gets the number of pages to be printed.
    /// </summary>
    public override int PageCount => m_Paginator.PageCount;

      /// <summary>
    /// Gets/set page size of printed page.
    /// </summary>
    public override Size PageSize
    {
      get => m_Paginator.PageSize;
          set => m_Paginator.PageSize = value;
      }

    /// <summary>
    /// Gets the source object that performs actual content pagination.
    /// </summary>
    public override IDocumentPaginatorSource Source => m_Paginator.Source;

      #endregion

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    /// <param name="paginator"></param>
    /// <param name="pageSettings"></param>
    /// <param name="printTicket"></param>
    /// <param name="headerFooterfontFamily"></param>
    public DocumentPaginatorWrapper(DocumentPaginator paginator, PageSettings pageSettings, PrintTicket printTicket, FontFamily headerFooterfontFamily)
    {
      m_Margins = ConvertMarginsToPx(pageSettings.Margins);

      if (pageSettings.Landscape)
        m_PageSize = new Size((int)printTicket.PageMediaSize.Height, (int)printTicket.PageMediaSize.Width);
      else
        m_PageSize = new Size((int)printTicket.PageMediaSize.Width, (int)printTicket.PageMediaSize.Height);

      m_Paginator = paginator;
      m_Paginator.PageSize = new Size(m_PageSize.Width - m_Margins.Left - m_Margins.Right, m_PageSize.Height - m_Margins.Top - m_Margins.Bottom);

      m_Typeface = new Typeface(headerFooterfontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
    }
    #endregion constructor

    #region methods
    /// <summary>
    /// Get a page for printing layouted with header and footer.
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <returns></returns>
    public override DocumentPage GetPage(int pageNumber)
    {
      DocumentPage page = m_Paginator.GetPage(pageNumber);

      ContainerVisual newpage = new ContainerVisual();

      //
      // Header
      //
      DrawingVisual header = new DrawingVisual();

      using (DrawingContext ctx = header.RenderOpen())
      {
        DrawPath(ctx, m_Margins.Top - 20, m_Title, TextAlignment.Left);
        DrawText(ctx, m_Margins.Top - 20, $"{DateTime.Now}", TextAlignment.Right);
        DrawLine(ctx, m_Margins.Top - 5, 0.5);
      }

      //
      // Footer
      //
      DrawingVisual footer = new DrawingVisual();
      using (DrawingContext ctx = footer.RenderOpen())
      {
        DrawText(ctx, m_PageSize.Height - m_Margins.Bottom + 5, "-" + (pageNumber + 1) + "-", TextAlignment.Center);
        DrawLine(ctx, m_PageSize.Height - m_Margins.Bottom + 5, 0.5);
      }

      ContainerVisual pageVisual = new ContainerVisual();

      pageVisual.Children.Add(page.Visual);

      newpage.Children.Add(header);
      newpage.Children.Add(pageVisual);
      newpage.Children.Add(footer);

      return new DocumentPage(newpage, m_PageSize, Move(page.BleedBox), Move(page.ContentBox));
    }

    private Rect Move(Rect rect)
    {
        if (rect.IsEmpty)
      {
        return rect;
      }

        return new Rect(rect.Left + m_Margins.Left, rect.Top + m_Margins.Top, rect.Width, rect.Height);
    }

    /// <summary>
    /// Draws a text at specified y-postion with specified text alignment.
    /// </summary>
    private void DrawText(DrawingContext ctx, double yPos, string text, TextAlignment alignment)
    {
      if (string.IsNullOrEmpty(text))
        return;

      if (m_Typeface == null)
      {
        m_Typeface = new Typeface("Times New Roman");
      }


      FormattedText formattedText = new FormattedText(text,
              System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
              m_Typeface, 12, Brushes.Black);

      if (alignment == TextAlignment.Left)
      {
        formattedText.TextAlignment = TextAlignment.Left;
        formattedText.MaxTextWidth = m_PageSize.Width * 2 / 3;
        formattedText.MaxTextHeight = 16;
        formattedText.Trimming = TextTrimming.WordEllipsis;
        ctx.DrawText(formattedText, new Point(m_Margins.Left, yPos));
      }
      else if (alignment == TextAlignment.Right)
      {
        formattedText.TextAlignment = TextAlignment.Right;
        ctx.DrawText(formattedText, new Point(m_PageSize.Width - m_Margins.Right, yPos));
      }
      else if (alignment == TextAlignment.Center)
      {
        formattedText.TextAlignment = TextAlignment.Center;
        ctx.DrawText(formattedText, new Point(m_PageSize.Width / 2, yPos));
      }
    }

    /// <summary>
    /// Draws a path as document title.
    /// </summary>
    private void DrawPath(DrawingContext ctx, double yPos, string text, TextAlignment alignment)
    {
      if (string.IsNullOrEmpty(text))
        return;

      if (!File.Exists(text))
      {
        DrawText(ctx, yPos, text, alignment);
        return;
      }

      if (m_Typeface == null)
      {
        m_Typeface = new Typeface("Times New Roman");
      }

        FormattedText formattedText = new FormattedText(text,
              System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
              m_Typeface, 12, Brushes.Black);

      var textWidth = formattedText.Width;

      double maxTextLength = m_PageSize.Width * 2 / 3;

      if (textWidth < maxTextLength)
      {
        ctx.DrawText(formattedText, new Point(m_Margins.Left, yPos));
        return;
      }

      //
      // if someone has a more clever solution
      // to do WordEllipsis trimming for paths, please shout
      //
      string path = Path.GetDirectoryName(text);
      string fileName = "\\" + Path.GetFileName(text);

            // get the length of the trimmed file name
            formattedText = new FormattedText(fileName,
                    System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                    m_Typeface, 12, Brushes.Black)
            {
                MaxTextWidth = maxTextLength - 100,
                MaxTextHeight = 16,
                Trimming = TextTrimming.WordEllipsis
            };
            textWidth = formattedText.Width;

      // draw the path
        formattedText = new FormattedText(path,
            System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
            m_Typeface, 12, Brushes.Black)
        {
            MaxTextWidth = maxTextLength - textWidth,
            MaxTextHeight = 16,
            Trimming = TextTrimming.WordEllipsis
        };

        ctx.DrawText(formattedText, new Point(m_Margins.Left, yPos));
      textWidth = formattedText.Width;

            // draw the file name
            formattedText = new FormattedText(fileName,
                    System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                    m_Typeface, 12, Brushes.Black)
            {
                MaxTextWidth = maxTextLength - textWidth,
                MaxTextHeight = 16,
                Trimming = TextTrimming.WordEllipsis
            };
            ctx.DrawText(formattedText, new Point(m_Margins.Left + textWidth, yPos));
    }

    /// <summary>
    /// Draws a line from left to right margin at specified y-postion.
    /// </summary>
    private void DrawLine(DrawingContext ctx, double yPos, double thickness)
    {
      ctx.DrawLine(new Pen(new SolidColorBrush(Colors.Black), thickness),
                    new Point(m_Margins.Left, yPos),
                    new Point(m_PageSize.Width - m_Margins.Right, yPos));

    }

    /// <summary>
    /// Converts specified Margins (hundredths of an inch) to pixel margin (px).
    /// </summary>
    private Margins ConvertMarginsToPx(Margins margins)
    {

      margins.Left = ConvertToPx(margins.Left);
      margins.Top = ConvertToPx(margins.Top);
      margins.Right = ConvertToPx(margins.Right);
      margins.Bottom = ConvertToPx(margins.Bottom);

      return margins;

    }

    /// <summary>
    /// Converts specified inch (hundredths of an inch) to pixels (px).
    /// </summary>
    private int ConvertToPx(double inch)
    {
      return (int)(inch * 0.96);
    }
    #endregion methods
  }
}
