namespace ICSharpCode.AvalonEdit.Highlighting.Themes.XML
{
  using System.IO;
  using System.Xml;
  using System.Xml.Schema;
  using System.Collections.Generic;
  using System.Globalization;

  /// <summary>
  /// Check whether an XML file can be validated
  /// against an XSD (XML Schema Definition) file or not.
  /// 
  /// Use this class by:
  /// 1> Constructing a new object
  /// 2> Running the CheckXML_XSD member function
  /// 3> Check the <seealso cref="ErrorMessages"/> and <seealso cref="IsSchemaValid"/> properties to see 
  /// </summary>
  internal class SchemaValidator
  {
    #region fields
    private List<string> mErrorMessages = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// constructor
    /// </summary>
    public SchemaValidator()
    {
      mErrorMessages = null;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// List messages to document errors in XSD schema validation
    /// </summary>
    public List<string> ErrorMessages
    {
      get
      {
        return (mErrorMessages == null ? new List<string>() : mErrorMessages);
      }
    }

    /// <summary>
    /// Get if XML validation was successful or not.
    /// </summary>
    /// <returns></returns>
    public bool IsSchemaValid
    {
      get
      {
        return (mErrorMessages == null ? true : ((mErrorMessages.Count) > 0 ? false : true));
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Check XML file against an XSD accessible via Stream object.
    /// </summary>
    /// <param name="xmlPathFileName">PathFilename to XML file</param>
    /// <param name="xsdStream">Stream constructed from XSD (resource) file.</param>
    /// <param name="xmlSettings">Optional settings to determine level of detail to check</param>
    public void CheckXML_XSD(string xmlPathFileName,
                             Stream xsdStream,
                             XmlReaderSettings xmlSettings = null)
    {
      StreamReader strmrStreamReader = new StreamReader(xsdStream);
      XmlSchema xSchema = new XmlSchema();
      xSchema = XmlSchema.Read(strmrStreamReader, null);

      // Set the validation settings.
      if (xmlSettings == null)
      {
        xmlSettings = new XmlReaderSettings();
        xmlSettings.Schemas.Add(xSchema);
        xmlSettings.ValidationType = ValidationType.Schema;
        xmlSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
        xmlSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
        xmlSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
      }

      xmlSettings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

      using (XmlReader reader = XmlReader.Create(xmlPathFileName, xmlSettings))
      {
        while (reader.Read())
        {
        }
      }
    }

    /// <summary>
    /// Internal Callback function to store validation errors and warnings via callback
    /// in an internal collection.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    protected void ValidationCallBack(object sender, ValidationEventArgs args)
    {
      if (mErrorMessages == null)
        mErrorMessages = new List<string>();

      switch (args.Severity)
      {
        case XmlSeverityType.Warning:
          mErrorMessages.Add(string.Format(CultureInfo.CurrentCulture, "Line: {0}, Position: {1} {2}",
                         args.Exception.LineNumber,
                         args.Exception.LinePosition,
                         args.Exception.Message));
          break;

        case XmlSeverityType.Error:
          mErrorMessages.Add(string.Format(CultureInfo.CurrentCulture, "Line: {0}, Position: {1} {2}",
                         args.Exception.LineNumber,
                         args.Exception.LinePosition,
                         args.Exception.Message));
          break;

        default:
          mErrorMessages.Add(string.Format(CultureInfo.CurrentCulture ,"Unhandled XML error with severity of type: {0} and message: {1}",
                                                args.Severity.ToString(), args.Message));
          break;
      }
    }
    #endregion methods
  }
}
