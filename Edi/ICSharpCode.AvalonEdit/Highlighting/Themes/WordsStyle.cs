namespace ICSharpCode.AvalonEdit.Highlighting.Themes
{
  using System.Windows;
  using System.Windows.Media;
  using System;
  using System.Globalization;
  
  /// <summary>
  /// Class to manage a highlighting style 
  /// </summary>
  public class WordsStyle
  {
    #region constructors
    /// <summary>
    /// Construct a named (eg. 'Comment') WordStyle object
    /// </summary>
    /// <param name="name"></param>
    public WordsStyle(string name)
      : this()
    {
      Name = name;
    }

    /// <summary>
    /// Hidden standard constructor in favour of named element cosntructor
    /// </summary>
    protected WordsStyle()
    {
      Name = string.Empty;

      fgColor = null;
      bgColor = null;

      fontWeight = null;
      fontStyle = null;
    }
    #endregion constructors

    #region properties
    /// <summary>
    /// Name of <seealso cref="WordsStyle"/> object
    /// 
    /// (this is usually the key in a collection of these)
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Get/set brush definition for the foreground used in this style
    /// </summary>
    public SolidColorBrush fgColor { get; set; }
    
    /// <summary>
    /// Get/set brush definition for the background used in this style
    /// </summary>
    public SolidColorBrush bgColor { get; set; }

    /// <summary>
    /// Get/set brush definition for the fontweight used in this style
    /// </summary>
    public FontWeight? fontWeight { get; set; }

    /// <summary>
    /// Get/set fontStyle used in this WordsStyle
    /// </summary>
    public FontStyle? fontStyle { get; set; }
    #endregion properties
  }
}
