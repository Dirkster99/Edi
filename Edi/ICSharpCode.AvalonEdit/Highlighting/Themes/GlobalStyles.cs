namespace ICSharpCode.AvalonEdit.Highlighting.Themes
{
  using System.Collections.Generic;
  using System.Windows.Media;

  /// <summary>
  /// This class manages one highlighting theme for one highlighting pattern (eg.: 'HTML')
  /// </summary>
  public class GlobalStyles
  {
    #region fields
    private Dictionary<string, WidgetStyle> mWidgetStyles;
    #endregion fields

    #region constructor
    /// <summary>
    /// Hidden constructor in favour of constructor with human
    /// readable name and description for this highlighting theme
    /// </summary>
    public GlobalStyles()
    {
    }
    #endregion constructor

    #region properties
    #endregion properties

    #region methods
    /// <summary>
    /// Add another style (foreground color, background color, bold etc...) to the collection
    /// of styles that make up this highlighting theme.
    /// </summary>
    /// <param name="brushName"></param>
    /// <param name="widgetStyle">color and brush representation (eg.: "#FF00FFFF" etc)</param>
    public void AddWordStyle(string brushName, WidgetStyle widgetStyle)
    {
      if (mWidgetStyles == null)
        mWidgetStyles = new Dictionary<string, WidgetStyle>();

      mWidgetStyles.Add(brushName, widgetStyle);
    }

    /// <summary>
    /// Attempt to retrieve a <seealso cref="SolidColorBrush"/> from a word style name
    /// </summary>
    /// <param name="widgetStyleName"></param>
    /// <returns></returns>
    public SolidColorBrush GetFgColorBrush(string widgetStyleName)
    {
      if (mWidgetStyles != null)
      {
        WidgetStyle s;
        mWidgetStyles.TryGetValue(widgetStyleName, out s);

        if (s != null)
          return s.fgColor;
      }

      return null;
    }

    /// <summary>
    /// Attempt to retrieve a <seealso cref="WidgetStyle"/> from a word style name
    /// </summary>
    /// <param name="widgetStyleName"></param>
    /// <returns></returns>
    public WidgetStyle GetWidgetStyle(string widgetStyleName)
    {
      if (mWidgetStyles != null)
      {
        WidgetStyle s;
        mWidgetStyles.TryGetValue(widgetStyleName, out s);

        return s;
      }

      return null;
    }
    #endregion methods
  }
}
