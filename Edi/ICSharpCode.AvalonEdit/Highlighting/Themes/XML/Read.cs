namespace ICSharpCode.AvalonEdit.Highlighting.Themes.XML
{
  using System;
  using System.Globalization;
  using System.IO;
  using System.Reflection;
  using System.Windows;
  using System.Windows.Media;
  using System.Xml;

  using Themes;
  using System.Collections.Generic;

  /// <summary>
  /// Read Highlighting Style information from XML and return
  /// a in-memory representation as <seealso cref="HighlightingThemes"/> object from it.
  /// </summary>
  public class Read
  {
    #region Fields
    /// <summary>
    /// log4net logger
    /// </summary>
    protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    internal const string XMLComment = "#comment";
    internal const string XMLNameSpace = "xmlns";

    /// <summary>
    /// Name of the XML tag that identifies the corresponding tag of this class
    /// </summary>
    public const string XMLName = "HlThemes";

    /// <summary>
    /// ReadHlThemes supports an (XML) property xmlns:xsi with the name xmlns_xsi
    /// </summary>
    public const string attr_name = "name";
    #endregion Fields;

    #region methods
    /// <summary>
    /// Load custom highlighting theme definition from XML file.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    /// <param name="VerifySchema"></param>
    /// <returns></returns>
    public static HighlightingThemes ReadXML(String path = "",
                                             String fileName = @"AvalonEdit\HighLighting_Themes\DeepBlack.xshd",
                                             bool VerifySchema = true)
    {
      if (string.IsNullOrEmpty(path) && string.IsNullOrEmpty(fileName))
        return null;

      HighlightingThemes HlThemeRoot = null;

      try
      {
        string sPathfileName = fileName;

        if (string.IsNullOrEmpty( path ) == false)
          sPathfileName = Path.Combine(path, fileName);

        if (File.Exists(sPathfileName) == false)
        {
          logger.Error(string.Format(CultureInfo.InvariantCulture,
                       "File '{0}' does not exist at '{1}' or cannot be accessed.", fileName, path));

          return null;
        }

        if (VerifySchema) // Test whether XSD schema is valid or not
        {
          List<string> errorMsgs;

          if ((errorMsgs = TestXML(sPathfileName)) != null)
          {
            // log error (if any) and return null
            foreach(string s in errorMsgs)
              logger.Error(s);

            return null;
          }
        }

        using (XmlReader reader = XmlReader.Create(sPathfileName))
        {
          string rootTagName = XMLName;

          string name = string.Empty;

          reader.ReadToNextSibling(XMLName);
          while (reader.MoveToNextAttribute())
          {
            switch (reader.Name)
            {
              case attr_name:
                name = reader.Value;
                break;

              case XMLNameSpace:
                string s = reader.Value;
                break;
            }
          }

                    HlThemeRoot = new HighlightingThemes(name)
                    {
                        FileNamePath = sPathfileName
                    };

                    // Find all children that belong into the second level right below the XML root tag
                    while (reader.Read())
          {
            if (reader.NodeType == XmlNodeType.Element)
            {
              switch (reader.Name)
              {
                case ReadLexerStyles.XMLName:
                  ReadLexerStyles.ReadNode(reader.ReadSubtree(), HlThemeRoot);
                  break;

                case ReadGlobalStyles.XMLName:
                  ReadGlobalStyles.ReadNode(reader.ReadSubtree(), HlThemeRoot);
                  break;

                default:
                  if (reader.Name.Trim().Length > 0 && reader.Name != XMLComment)
                    logger.Warn("Parsing the XML child:'" + reader.Name + "' of '" + rootTagName + "' is not implemented.");
                  break;
              }
            }
          }
        }
      }
      catch (Exception e)
      {
        logger.Error(
            $"An error occurred while reading a highlighting theme file at path '{(path == null ? "(null)" : path)}', filename '{(fileName == null ? "(null)" : fileName)}':\n\n" + "Details:" + e.ToString());

        return null;
      }

      return HlThemeRoot;
    }

    /// <summary>
    /// Pulls an XSD file from an application resource and uses it to
    /// verify whether submitted XML file in filesystem is valid or not
    /// </summary>
    /// <param name="sXML">XML file to validate</param>
    /// <returns>Error message (if any). Returns string with zero length
    /// if everythings fine.</returns>
    private static List<string> TestXML(string sXML)
    {
      string sAssembly = Assembly.GetExecutingAssembly().CodeBase;

      // Replace the information in '<>' brackets with a valid path
      // to a XSD file (that you added into your Visual Studio project)
      // Be careful: Names are case sensitiv and '.' are delimters.
      // Make sure your XSD file is an 'embedded resource'
      // "<Namespace>.<FolderName>.<Filename>.xsd"
      const string XSD_Location = "ICSharpCode.AvalonEdit.Highlighting.Themes.XML.HlTheme.xsd";
      SchemaValidator vs = new SchemaValidator();

      Assembly a = Assembly.LoadFrom(sAssembly);

      using (Stream strm = a.GetManifestResourceStream(XSD_Location))
      {
        if (strm == null)
        {
          List<string> l = new List<string>();

          l.Add(string.Format(CultureInfo.InvariantCulture, "Unable to load XSD: '{0}' file from internal resource.", XSD_Location));

          return(l);
        }

        vs.CheckXML_XSD(sXML, strm);

        if (vs.IsSchemaValid == false)  // Return strings describing problems (if any)
          return vs.ErrorMessages;
      }

      return null;
    }
    #endregion methods
  }

  /// <summary>
  /// Support reading LexerStyles XML tag
  /// </summary>
  class ReadLexerStyles
  {
    #region Fields
    protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    internal const string XMLComment = "#comment";
    internal const string XMLNameSpace = "xmlns";

    /// <summary>
    /// Name of the XML tag that identifies the corresponding tag of this class
    /// </summary>
    public const string XMLName = "LexerStyles";
    #endregion Fields;

    #region Constructors

      #endregion Constructors;

    #region Methods
    /// <summary>
    /// Read the LexerStyles XML tag with its attributes (if any) and insert
    /// a resulting <seealso cref="HighlightingTheme"/> object into the <seealso cref="HighlightingThemes"/> root object.
    /// </summary>
    internal static void ReadNode(XmlReader reader, HighlightingThemes HlThemeRoot)
    {
      reader.ReadToNextSibling(XMLName);
      while (reader.Read())
      {
        if (reader.IsStartElement())
        {
          switch (reader.Name)
          {
            case ReadLexerType.XMLName:
              HighlightingTheme t = ReadLexerType.ReadNode(reader.ReadSubtree());

              try
              {
                HlThemeRoot.AddTheme(t.HlName, t);
              }
              catch (Exception e)   // Reading one errornous style node should not crash the whole process
              {
                logger.Error("Error reading LexerType node", e);
              }
              break;

            case XMLNameSpace:
              break;

            default:
              if (reader.Name.Trim().Length > 0 && reader.Name != XMLComment)
                logger.Warn("Parsing the XML child:'" + reader.Name + "' of '" + XMLName + "' is not implemented.");
              break;
          }
        }
      }
    }
    #endregion Methods
  }

  /// <summary>
  /// Support reading LexerType XML tag
  /// </summary>
  internal class ReadLexerType
  {
    #region Fields
    protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    internal const string XMLComment = "#comment";
    internal const string XMLNameSpace = "xmlns";

    /// <summary>
    /// Name of the XML tag that identifies the corresponding tag of this class
    /// </summary>
    public const string XMLName = "LexerType";

    /// <summary>
    /// ReadLexerType supports an (XML) property name with the name name
    /// </summary>
    public const string attr_name = "name";
    /// <summary>
    /// ReadLexerType supports an (XML) property desc with the name desc
    /// </summary>
    public const string attr_desc = "desc";
    #endregion Fields;

    #region Constructors

      #endregion Constructors;

    #region Methods
    /// <summary>
    /// Read the LexerType XML tag with its attributes (if any) and return
    /// a resulting <seealso cref="HighlightingTheme"/> object.
    /// </summary>
    internal static HighlightingTheme ReadNode(XmlReader reader)
    {
      HighlightingTheme hlTheme = null;

      string name = string.Empty;
      string desc = string.Empty;

      reader.ReadToNextSibling(XMLName);
      while (reader.MoveToNextAttribute())
      {
        switch (reader.Name)
        {
          case attr_name:
            name = reader.Value;
            break;

          case XMLNameSpace:
            break;

          case attr_desc:
            desc = (reader.Value == null ? string.Empty : reader.Value);
            break;
        }
      }

            hlTheme = new HighlightingTheme(name)
            {
                HlDesc = desc
            };


            while (reader.Read())
      {
        if (reader.IsStartElement())
        {
          switch (reader.Name)
          {
            case ReadWordsStyle.XMLName:
              WordsStyle t = ReadWordsStyle.ReadNode(reader.ReadSubtree());
                  
              hlTheme.AddWordStyle(t.Name, t);
              break;

            case XMLNameSpace:
              break;

            default:
              if (reader.Name.Trim().Length > 0 && reader.Name != XMLComment)
                logger.Warn("Parsing the XML child:'" + reader.Name + "' of '" + XMLName + "' is not implemented.");
              break;
          }
        }
      }

      return hlTheme;
    }
    #endregion Methods
  }

  /// <summary>
  /// Support reading WordsStyle XML tag
  /// </summary>
  internal class ReadWordsStyle : ReadStyle
  {
    #region Fields
    protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    internal const string XMLComment = "#comment";
    internal const string XMLNameSpace = "xmlns";

    /// <summary>
    /// Name of the XML tag that identifies the corresponding tag of this class
    /// </summary>
    public const string XMLName = "WordsStyle";

    /// <summary>
    /// ReadLexerType supports an (XML) property name
    /// </summary>
    public const string attr_name = "name";

    /// <summary>
    /// ReadLexerType supports an (XML) property fgColor
    /// </summary>
    public const string attr_fgColor = "fgColor";

    /// <summary>
    /// ReadLexerType supports an (XML) property bgColor
    /// </summary>
    public const string attr_bgColor = "bgColor";

    /// <summary>
    /// ReadLexerType supports an (XML) property fontWeight
    /// </summary>
    public const string attr_fontWeight = "fontWeight";

    /// <summary>
    /// ReadLexerType supports an (XML) property FontStyle
    /// </summary>
    public const string attr_FontStyle = "fontStyle";
    #endregion Fields;

    #region Constructors

      #endregion Constructors;

    #region Methods
    /// <summary>
    /// Read the WordsStyle XML tag with its attributes (if any) and return
    /// a resulting <seealso cref="WordsStyle"/> object.
    /// </summary>
    internal static WordsStyle ReadNode(XmlReader reader)
    {
      WordsStyle ret = null;
		  FontWeightConverter FontWeightConverter = new FontWeightConverter();
		  FontStyleConverter FontStyleConverter = new FontStyleConverter();

      string name = string.Empty;
      string fgColor = string.Empty;
      string bgColor = string.Empty;
      string fontWeight = string.Empty;
      string FontStyle = string.Empty;

      reader.ReadToNextSibling(XMLName);
      while (reader.MoveToNextAttribute())
      {
        switch (reader.Name)
        {
          case attr_name:
            name = reader.Value;
            break;

          case attr_fgColor:
            fgColor = (reader.Value == null ? string.Empty : reader.Value);
            break;

          case attr_bgColor:
            bgColor = (reader.Value == null ? string.Empty : reader.Value);
            break;

          case attr_fontWeight:
            fontWeight = (reader.Value == null ? string.Empty : reader.Value);
            break;

          case attr_FontStyle:
            FontStyle = (reader.Value == null ? string.Empty : reader.Value);
            break;

          case XMLNameSpace:
            break;

          default:
            if (reader.Name.Trim().Length > 0 && reader.Name != XMLComment)
              logger.Warn("Parsing the XML child:'" + reader.Name + "' of '" + ReadLexerType.XMLName + "' is not implemented.");
          break;
        }
      }

      ret = new WordsStyle(name);

      if (fgColor != string.Empty)
        ret.fgColor = SetColorFromString(attr_fgColor, fgColor);

      if (bgColor != string.Empty)
        ret.bgColor = SetColorFromString(attr_bgColor, bgColor);

      if (fontWeight != string.Empty)
        ret.fontWeight = ParseFontWeight(attr_fontWeight, fontWeight, FontWeightConverter);

      if (FontStyle != string.Empty)
        ret.fontStyle = ParseFontStyle(attr_FontStyle, FontStyle, FontStyleConverter);

      return ret;
    }
    #endregion Methods
  }


  /// <summary>
  /// Support reading GlobalStyles XML tag
  /// </summary>
  class ReadGlobalStyles
  {
    #region Fields
    protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    internal const string XMLComment = "#comment";
    internal const string XMLNameSpace = "xmlns";

    /// <summary>
    /// Name of the XML tag that identifies the corresponding tag of this class
    /// </summary>
    public const string XMLName = "GlobalStyles";
    #endregion Fields;

    #region Constructors

      #endregion Constructors;

    #region Methods
    /// <summary>
    /// Read the GlobalStyles XML tag with its attributes (if any) and insert
    /// a resulting <seealso cref="HighlightingTheme"/> object into the <seealso cref="HighlightingThemes"/> root object.
    /// </summary>
    internal static void ReadNode(XmlReader reader, HighlightingThemes HlThemeRoot)
    {
      reader.ReadToNextSibling(XMLName);
      while (reader.Read())
      {
        if (reader.IsStartElement())
        {
          switch (reader.Name)
          {
            case ReadWidgetStyle.XMLName_Hyperlink:
            case ReadWidgetStyle.XMLName_DefaultStyle:
              WidgetStyle t = ReadWidgetStyle.ReadForegroundBackgroundColorNode(reader.ReadSubtree(), reader.Name);

              try
              {
                HlThemeRoot.AddWidgetStyle(t.Name, t);
              }
              catch (Exception e)   // Reading one errornous style node should not crash the whole process
              {
                logger.Error("Error reading DefaultStyle node", e);
              }
              break;

            case ReadWidgetStyle.XMLName_CurrentLineBackground:
              t = ReadWidgetStyle.ReadCurrentLineBackgroundNode(reader.ReadSubtree());

              try
              {
                HlThemeRoot.AddWidgetStyle(t.Name, t);
              }
              catch (Exception e)   // Reading one errornous style node should not crash the whole process
              {
                logger.Error("Error reading CurrentLineBackground node", e);
              }
              break;

            case ReadWidgetStyle.XMLName_LineNumbersForeground:
            case ReadWidgetStyle.XMLName_NonPrintableCharacter:
              t = ReadWidgetStyle.ReadForegroundColorNode(reader.ReadSubtree(), reader.Name);

              try
              {
                HlThemeRoot.AddWidgetStyle(t.Name, t);
              }
              catch (Exception e)   // Reading one errornous style node should not crash the whole process
              {
                logger.Error("Error reading LineNumbersForeground node", e);
              }
              break;

            case ReadWidgetStyle.XMLName_Selection:
              t = ReadWidgetStyle.ReadSelectionNode(reader.ReadSubtree());

              try
              {
                HlThemeRoot.AddWidgetStyle(t.Name, t);
              }
              catch (Exception e)   // Reading one errornous style node should not crash the whole process
              {
                logger.Error("Error reading Selection node", e);
              }
              break;

            case XMLNameSpace:
              break;

            default:
              if (reader.Name.Trim().Length > 0 && reader.Name != XMLComment)
                logger.Warn("Parsing the XML child:'" + reader.Name + "' of '" + XMLName + "' is not implemented.");
              break;
          }
        }
      }
    }
    #endregion Methods
  }

  /// <summary>
  /// Support reading WidgetStyle XML tag
  /// </summary>
  internal class ReadWidgetStyle : ReadStyle
  {
    #region Fields
    protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    internal const string XMLComment = "#comment";
    internal const string XMLNameSpace = "xmlns";

    /// <summary>
    /// Name of the XML tag that identifies the corresponding tag of this class
    /// </summary>
    public const string XMLName_DefaultStyle = "DefaultStyle";

    /// <summary>
    /// XML name of tag that stores the default color brush for the current line highlighting
    /// </summary>
    public const string XMLName_CurrentLineBackground = "CurrentLineBackground";

    /// <summary>
    /// XML name of tag that stores the foreground color brush of the line number display
    /// </summary>
    public const string XMLName_LineNumbersForeground = "LineNumbersForeground";

    /// <summary>
    /// XML name of tag that stores the foreground, background, and border color of the selection control displayed in the text.
    /// </summary>
    public const string XMLName_Selection = "Selection";

    /// <summary>
    /// XML name of tag that stores the foreground and background color of the hyperlink control displayed in the text.
    /// </summary>
    public const string XMLName_Hyperlink = "Hyperlink";

    /// <summary>
    /// XML name of tag that stores the foreground color of non-printable characters (tab, enter, space) when displayed in the text.
    /// </summary>
    public const string XMLName_NonPrintableCharacter = "NonPrintableCharacter";

    /// <summary>
    /// ReadLexerType supports an (XML) property fgColor
    /// </summary>
    public const string attr_fgColor = "fgColor";

    /// <summary>
    /// ReadLexerType supports an (XML) property bgColor
    /// </summary>
    public const string attr_bgColor = "bgColor";

    /// <summary>
    /// ReadLexerType supports an (XML) property borderColor
    /// </summary>
    public const string attr_borderColor = "borderColor";
    #endregion Fields;

    #region Constructors

      #endregion Constructors;

    #region Methods
    internal static WidgetStyle ReadCurrentLineBackgroundNode(XmlReader reader)
    {
      WidgetStyle ret = null;

      string bgColor = string.Empty;

      reader.ReadToNextSibling(XMLName_CurrentLineBackground);
      while (reader.MoveToNextAttribute())
      {
        switch (reader.Name)
        {
          case attr_bgColor:
            bgColor = (reader.Value == null ? string.Empty : reader.Value);
            break;

          case XMLNameSpace:
            break;

          default:
            if (reader.Name.Trim().Length > 0 && reader.Name != XMLComment)
              logger.Warn("Parsing the XML child:'" + reader.Name + "' of '" + ReadLexerType.XMLName + "' is not implemented.");
            break;
        }
      }

      ret = new WidgetStyle(XMLName_CurrentLineBackground);

      if (bgColor != string.Empty)
        ret.bgColor = SetColorFromString(attr_bgColor, bgColor);

      return ret;
    }

    internal static WidgetStyle ReadForegroundColorNode(XmlReader reader, string nameOfNode)
    {
      WidgetStyle ret = null;

      string fgColor = string.Empty;

      reader.ReadToNextSibling(nameOfNode);
      while (reader.MoveToNextAttribute())
      {
        switch (reader.Name)
        {
          case attr_fgColor:
            fgColor = (reader.Value == null ? string.Empty : reader.Value);
            break;

          case XMLNameSpace:
            break;

          default:
            if (reader.Name.Trim().Length > 0 && reader.Name != XMLComment)
              logger.Warn("Parsing the XML child:'" + reader.Name + "' of '" + ReadLexerType.XMLName + "' is not implemented.");
            break;
        }
      }

      ret = new WidgetStyle(nameOfNode);

      if (fgColor != string.Empty)
        ret.fgColor = SetColorFromString(attr_fgColor, fgColor);

      return ret;
    }

    internal static WidgetStyle ReadSelectionNode(XmlReader reader)
    {
      WidgetStyle ret = null;

      string fgColor     = string.Empty;
      string bgColor     = string.Empty;
      string borderColor = string.Empty;

      reader.ReadToNextSibling(XMLName_Selection);
      while (reader.MoveToNextAttribute())
      {
        switch (reader.Name)
        {
          case attr_fgColor:
            fgColor = (reader.Value == null ? string.Empty : reader.Value);
            break;

          case attr_bgColor:
            bgColor = (reader.Value == null ? string.Empty : reader.Value);
            break;

          case attr_borderColor:
            borderColor = (reader.Value == null ? string.Empty : reader.Value);
            break;

          case XMLNameSpace:
            break;

          default:
            if (reader.Name.Trim().Length > 0 && reader.Name != XMLComment)
              logger.Warn("Parsing the XML child:'" + reader.Name + "' of '" + ReadLexerType.XMLName + "' is not implemented.");
            break;
        }
      }

      ret = new WidgetStyle(XMLName_Selection);

      if (fgColor != string.Empty)
        ret.fgColor = SetColorFromString(attr_fgColor, fgColor);

      if (bgColor != string.Empty)
        ret.bgColor = SetColorFromString(attr_bgColor, bgColor);

      if (borderColor != string.Empty)
        ret.borderColor = SetColorFromString(attr_borderColor, borderColor);

      return ret;
    }

    internal static WidgetStyle ReadForegroundBackgroundColorNode(XmlReader reader, string nameOfNode)
    {
      WidgetStyle ret = null;

      string fgColor = string.Empty;
      string bgColor = string.Empty;

      reader.ReadToNextSibling(nameOfNode);
      while (reader.MoveToNextAttribute())
      {
        switch (reader.Name)
        {
          case attr_fgColor:
            fgColor = (reader.Value == null ? string.Empty : reader.Value);
            break;

          case attr_bgColor:
            bgColor = (reader.Value == null ? string.Empty : reader.Value);
            break;

          case XMLNameSpace:
            break;

          default:
            if (reader.Name.Trim().Length > 0 && reader.Name != XMLComment)
              logger.Warn("Parsing the XML child:'" + reader.Name + "' of '" + ReadLexerType.XMLName + "' is not implemented.");
            break;
        }
      }

      ret = new WidgetStyle(nameOfNode);

      if (fgColor != string.Empty)
        ret.fgColor = SetColorFromString(attr_fgColor, fgColor);

      if (bgColor != string.Empty)
        ret.bgColor = SetColorFromString(attr_bgColor, bgColor);

      return ret;
    }
    #endregion Methods
  }

  internal abstract class ReadStyle
  {
    #region Methods
    /// <summary>
    /// Convert a standard color string, such as, "#FF223344" into a solidcolorbrush object
    /// and set it as foreground color value.
    /// </summary>
    /// <param name="stringColor"></param>
    /// <param name="stringAttribName"></param>
    protected static SolidColorBrush SetColorFromString(string stringAttribName, string stringColor)
    {
      try
      {
        BrushConverter bc = new BrushConverter();
        return bc.ConvertFromString(stringColor) as SolidColorBrush;
      }
      catch (Exception exp)
      {
        throw new Exception(string.Format(CultureInfo.InvariantCulture, "Invalid color attribute value '{0}'=\"{1}\"", stringAttribName, stringColor), exp);
      }
    }

    /// <summary>
    /// Convert a string representation into a null-able <seealso cref="FontWeight"/> object and return it.
    /// </summary>
    /// <param name="stringAttribName"></param>
    /// <param name="fontWeight"></param>
    /// <param name="converter"></param>
    /// <returns></returns>
    protected static FontWeight? ParseFontWeight(string stringAttribName, string fontWeight, FontWeightConverter converter)
    {
      if (string.IsNullOrEmpty(fontWeight))
        return null;

      try
      {
        return (FontWeight?)converter.ConvertFromInvariantString(fontWeight);
      }
      catch (Exception exp)
      {
        throw new Exception(string.Format(CultureInfo.InvariantCulture, "Invalid FontWeight attribute value '{0}'=\"{1}\"", stringAttribName, fontWeight), exp);
      }
    }

    /// <summary>
    /// Convert a string representation into a null-able <seealso cref="FontStyle"/> object and return it.
    /// </summary>
    /// <param name="stringAttribName"></param>
    /// <param name="fontStyle"></param>
    /// <param name="converter"></param>
    /// <returns></returns>
    protected static FontStyle? ParseFontStyle(string stringAttribName, string fontStyle, FontStyleConverter converter)
    {
      if (string.IsNullOrEmpty(fontStyle))
        return null;

      try
      {
        return (FontStyle?)converter.ConvertFromInvariantString(fontStyle);
      }
      catch (Exception exp)
      {
        throw new Exception(string.Format(CultureInfo.InvariantCulture, "Invalid FontStyle attribute value '{0}'=\"{1}\"", stringAttribName, fontStyle), exp);
      }
    }
    #endregion Methods  
  }
}
