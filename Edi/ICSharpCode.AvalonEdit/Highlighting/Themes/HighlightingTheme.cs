namespace ICSharpCode.AvalonEdit.Highlighting.Themes
{
  using System.Collections.Generic;
  using System.Windows.Media;

  /// <summary>
  /// This class manages one highlighting theme for one highlighting pattern (eg.: 'HTML')
  /// </summary>
  public class HighlightingTheme
  {
    #region fields
    private Dictionary<string, WordsStyle> mHlThemes;
    #endregion fields

    #region constructor
    /// <summary>
    /// Constructor constructor with human
    /// readable name and description for this highlighting theme
    /// </summary>
    /// <param name="hlName">Human read-able name of highlighting (eg.: 'html')</param>
    /// <param name="description">Human read-able description for this highlighting pattern</param>
    public HighlightingTheme(string hlName, string description = "")
      : this()
    {
      HlName = hlName;
      HlDesc = description;
    }

    /// <summary>
    /// Hidden constructor in favour of constructor with human
    /// readable name and description for this highlighting theme
    /// </summary>
    protected HighlightingTheme()
    {
      HlDesc = HlName = string.Empty;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Human read-able name of highlighting (eg.: 'html')
    /// </summary>
    public string HlName { get; private set; }

    /// <summary>
    /// Human read-able description for this highlighting pattern
    /// </summary>
    public string HlDesc { get; set; }
    #endregion properties

    #region methods
    /// <summary>
    /// Add another style (foreground color, background color, bold etc...) to the collection
    /// of styles that make up this highlighting theme.
    /// </summary>
    /// <param name="brushName"></param>
    /// <param name="wordStyle">color and brush representation (eg.: "#FF00FFFF" etc)</param>
    public void AddWordStyle(string brushName, WordsStyle wordStyle)
    {
      if (mHlThemes == null)
        mHlThemes = new Dictionary<string, WordsStyle>();

      mHlThemes.Add(brushName, wordStyle);
    }

    /// <summary>
    /// Attempt to retrieve a <seealso cref="SolidColorBrush"/> from a word style name
    /// </summary>
    /// <param name="wordStyleName"></param>
    /// <returns></returns>
    public SolidColorBrush GetFgColorBrush(string wordStyleName)
    {
      if (mHlThemes != null)
      {
        WordsStyle s;
        mHlThemes.TryGetValue(wordStyleName, out s);

        if (s != null)
          return s.fgColor;
      }

      return null;
    }

    /// <summary>
    /// Attempt to retrieve a <seealso cref="WordsStyle"/> from a word style name
    /// </summary>
    /// <param name="BrushName"></param>
    /// <returns></returns>
    public WordsStyle GetWordsStyle(string BrushName)
    {
      if (mHlThemes != null)
      {
        WordsStyle s;
        mHlThemes.TryGetValue(BrushName, out s);

        return s;
      }

      return null;
    }
    #endregion methods
  }
}
