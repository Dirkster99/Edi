namespace ICSharpCode.AvalonEdit.Highlighting.Themes
{
  using System.Collections.Generic;
  using System.Linq;
  using System;

  /// <summary>
  /// This class manages a list highlighting themes that
  /// is applied for re-coloring standard highlightings.
  /// 
  /// This enables matching highlighting colors with
  /// application skin colors which is particularly cool
  /// when using dark skins.
  /// </summary>
  public class HighlightingThemes
  {
    #region fields
    /// <summary>
    /// log4net logger
    /// </summary>
    protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private Dictionary<string, HighlightingTheme> mHlThemes;

    private Dictionary<string, WidgetStyle> mGlobalStyles;
    #endregion fields

    #region constructor
    /// <summary>
    /// Constructor that names the <see cref="HighlightingThemes"/> collection
    /// </summary>
    /// <param name="name">Human read-able name of highighting themes collection</param>
    public HighlightingThemes(string name)
      : this()
    {
      Name = name;
    }

    /// <summary>
    /// Standard constructor hidden in favour of a
    /// constructor that names the <see cref="HighlightingThemes"/> collection
    /// </summary>
    protected HighlightingThemes()
    {
      Name = string.Empty;
      FileNamePath = string.Empty;
      
      mHlThemes = null;
      mGlobalStyles = null;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Name of highlighting theme
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Path and file name from which this highlighting theme was loaded from (if any)
    /// </summary>
    public string FileNamePath { get; set; }

    /// <summary>
    /// Returns an (empty) collection of IEnumerable style objects.
    /// </summary>
    public IEnumerable<WidgetStyle> GlobalStyles
    {
      get
      {
        return (mGlobalStyles != null ? mGlobalStyles.Values.AsEnumerable<WidgetStyle>()
                                           : new Dictionary<string, WidgetStyle>().Values.AsEnumerable());
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Print highlighting theme color codes in HTML into the console output
    /// </summary>
    /// <param name="hdef"></param>
    /// <param name="hlThemes"></param>
    public static void PrintThemeToHTML(IHighlightingDefinition hdef,
                                        HighlightingThemes hlThemes)
    {
      if (hdef == null || hlThemes == null)
        return;

      HighlightingTheme theme = hlThemes.FindTheme(hdef.Name);  // Is the current highlighting (eg.: HTML) themable?

      if (theme != null)
      {
        if (hdef.NamedHighlightingColors != null)
        {
          Console.WriteLine("<h2>{0}</h2>\n", theme.HlName);

          Console.WriteLine("<table>");
          Console.WriteLine("<tr>");
          Console.WriteLine("<td>Code</td>");
          Console.WriteLine("<td width=\"100\">Color</td>");
          Console.WriteLine("<td>Description</td>");
          Console.WriteLine("</tr>");

          // Go through each color definition in the highlighting and apply the theme on each match
          foreach (HighlightingColor c in hdef.NamedHighlightingColors)
          {
            WordsStyle s = theme.GetWordsStyle(c.Name);

            if (s != null)
            {
              if (s.fgColor != null)
                Console.WriteLine(string.Format("<tr><td>#{0:x2}{1:x2}{2:x2}</td><td bgColor=\"#{0:x2}{1:x2}{2:x2}\"></td><td>{3}</td></tr>",
                                  s.fgColor.Color.R,
                                  s.fgColor.Color.G,
                                  s.fgColor.Color.B, s.Name));
            }
          }
          Console.WriteLine("</table>");
        }
      }
    }

    /// <summary>
    /// Apply highlighting theme to highlighting pattern definition.
    /// This results in re-defined highlighting colors while keeping
    /// rules for regular expression matching.
    /// </summary>
    /// <param name="hdef">Current highlighting pattern</param>
    /// <param name="hlThemes">Collection of highlighting styles to be applied on current highlighting patterns</param>
    public static void ApplyHighlightingTheme(IHighlightingDefinition hdef,
                                              HighlightingThemes hlThemes)
    {
      if (hdef == null || hlThemes == null)
        return;

      HighlightingTheme theme = hlThemes.FindTheme(hdef.Name);  // Is the current highlighting (eg.: HTML) themable?

      if (theme != null)
      {
        if (hdef.NamedHighlightingColors != null)
        {
          // Go through each color definition in the highlighting and apply the theme on each match
          foreach (HighlightingColor c in hdef.NamedHighlightingColors)
          {
            WordsStyle s = theme.GetWordsStyle(c.Name);

            if (s != null)
            {
              if (s.bgColor != null)
                c.Background = new SimpleHighlightingBrush(s.bgColor);
              else
                c.Background = null;

              if (s.fgColor != null)
                c.Foreground = new SimpleHighlightingBrush(s.fgColor);
              else
                c.Foreground = null;

              if (s.fontStyle != null)
                c.FontStyle = s.fontStyle;
              else
                c.FontStyle = null;

              if (s.fontWeight != null)
                c.FontWeight = s.fontWeight;
              else
                c.FontStyle = null;
            }
            else
              logger.WarnFormat("Named Color: '{0}'in '{1}' does not exist in '{2}'.", c.Name, hdef.Name, hlThemes.FileNamePath);
          }
        }
      }
      else
        logger.WarnFormat("highlighting definition: '{0}' does not have a style in '{1}'.", hdef.Name, hlThemes.FileNamePath);
    }

    /// <summary>
    /// Add a highlighting theme into the collection
    /// of highlighting themes stored in this object.
    /// </summary>
    /// <param name="styleName"></param>
    /// <param name="highlightingTheme"></param>
    public void AddTheme(string styleName, HighlightingTheme highlightingTheme)
    {
      if (mHlThemes == null)
        mHlThemes = new Dictionary<string, HighlightingTheme>();

      mHlThemes.Add(styleName, highlightingTheme);
    }

    /// <summary>
    /// Search a highlighting theme by its name and return a
    /// <see cref="HighlightingTheme"/> object or null.
    /// </summary>
    /// <param name="themeName"></param>
    /// <returns></returns>
    public HighlightingTheme FindTheme(string themeName)
    {
      if (mHlThemes != null)
      {
        KeyValuePair<string, HighlightingTheme> kv = mHlThemes.SingleOrDefault(f => f.Key == themeName);
        
        return kv.Value;
      }

      return null;
    }

    /// <summary>
    /// Add a highlighting theme into the collection
    /// of highlighting themes stored in this object.
    /// </summary>
    /// <param name="styleName"></param>
    /// <param name="style"></param>
    public void AddWidgetStyle(string styleName, WidgetStyle style)
    {
      if (mGlobalStyles == null)
        mGlobalStyles = new Dictionary<string,WidgetStyle>();

      mGlobalStyles.Add(styleName, style);
    }
    #endregion methods
  }
}
