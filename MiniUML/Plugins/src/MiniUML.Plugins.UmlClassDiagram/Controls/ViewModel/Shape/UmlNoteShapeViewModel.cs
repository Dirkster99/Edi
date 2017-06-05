namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Shape
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Xml;
  using MiniUML.Model.ViewModels;
  using MiniUML.Model.ViewModels.Shapes;
  using MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Shape.Base;
  using MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.UmlElements;
  using MiniUML.Plugins.UmlClassDiagram.Converter;

  /// <summary>
  /// This class implements the viewmodel for Uml shapes
  /// with a square or rectangulare shape that users can
  /// draw on a UML canvas. Typically, these are class or
  /// table shapes or other items.
  /// </summary>
  public class UmlNoteShapeViewModel : UmlShapeBaseViewModel
  {
    #region fields
    private const string Text_TAG = "Text";

    private string mText = string.Empty;
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard contructor hidding XElement constructor
    /// </summary>
    public UmlNoteShapeViewModel(IShapeParent parent,
                               UmlTypes umlType)
      : base(parent,
             ShapeViewModelKey.NoteShape, ShapeViewModelSubKeys.None,
             umlType)
    {
    }
    #endregion constructor

    #region properties
    public string Text
    {
      get
      {
        return this.mText;
      }

      set
      {
        if (this.mText != value)
        {
          this.mText = value;
          this.NotifyPropertyChanged(() => this.Text);
        }
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Read shape from XML stream and return it.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="parent"></param>
    /// <param name="umlType"></param>
    /// <returns></returns>
    public static UmlNoteShapeViewModel ReadDocument(XmlReader reader,
                                                IShapeParent parent,
                                                UmlTypes umlType)
    {
      UmlNoteShapeViewModel ret = UmlElementDataDef.CreateShape(umlType, new System.Windows.Point(UmlTypeToStringConverter.DefaultX,
                                                                                             UmlTypeToStringConverter.DefaultY), parent)
                                                                                             as UmlNoteShapeViewModel;
      reader.ReadToNextSibling(ret.UmlDataTypeString);

      while (reader.MoveToNextAttribute())
      {
        if (ret.ReadAttributes(reader.Name, reader.Value) == false)
        {
          if (reader.Name.Trim().Length > 0 && reader.Name != UmlShapeBaseViewModel.XmlComment)
            throw new ArgumentException("XML node:'" + reader.Name + "' as child of '" + ret.XElementName + "' is not supported.");
        }
      }

      // Read child elements of this XML node
      while (reader.Read())
      {
        if (reader.NodeType == XmlNodeType.Element)
        {
          string nodeName = reader.Name;
          string error;

          if ((error = ret.ReadXMLNode(nodeName, reader)) != string.Empty)
            throw new NotImplementedException(error);
        }
      }

      return ret;
    }

    /// <summary>
    /// Persist the contents of this object into the given
    /// parameter <paramref name="writer"/> object.
    /// </summary>
    /// <param name="writer"></param>
    public override void SaveDocument(System.Xml.XmlWriter writer,
                                      IEnumerable<ShapeViewModelBase> root)
    {
      writer.WriteStartElement(this.mElementName);

      this.SaveAttributes(writer);

      // Save common model information (eg. comments)
      base.SaveDocument(writer);

      // Write text string as content of text tag
      UmlNoteShapeViewModel.SaveTextDocument(writer, this.Text);

      if (root != null)
      {
        foreach (var item in root)
        {
          item.SaveDocument(writer, item.Elements());
        }
      }

      writer.WriteEndElement();
    }

    /// <summary>
    /// Read the content of an XML node and return an error string
    /// if the XML node was not known to this method.
    /// </summary>
    /// <param name="nodeName"></param>
    /// <param name="reader"></param>
    /// <returns></returns>
    protected new string ReadXMLNode(string nodeName, XmlReader reader)
    {
      switch (nodeName)
      {
        case Text_TAG:
          this.Text = ReadTextDocument(reader.ReadSubtree()); // Write text string as content of this tag
          return string.Empty;

        default:
            if (base.ReadXMLNode(nodeName, reader) != string.Empty)
              return string.Format("'{0}' is not a valid sub-node of {1}", reader.Name, this.XElementName);
            break;
      }

      return string.Empty;
    }
    #endregion methods

    #region Text Document Methods
    /// <summary>
    /// Save a string in an Text Xml tag into the <paramref name="writer"/> parameter.
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="text"></param>
    private static void SaveTextDocument(System.Xml.XmlWriter writer, string text)
    {
      writer.WriteStartElement(Text_TAG);
      writer.WriteString(text);   // Write text string as content of this tag
      writer.WriteEndElement();
    }

    /// <summary>
    /// Load a string from a Text Xml tag from the <paramref name="reader"/> parameter.
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private static string ReadTextDocument(XmlReader reader)
    {
      string ret = string.Empty;

      reader.ReadToNextSibling(Text_TAG);
      while (reader.MoveToNextAttribute())
      {
        if (reader.Name.Trim().Length > 0 && reader.Name != UmlShapeBaseViewModel.XmlComment && reader.Name != "xmlns")
          throw new ArgumentException("XML node:'" + reader.Name + "' as child of '" + Text_TAG + "' is not supported.");
      }

      ret = reader.ReadString();

      return ret;
   }
    #endregion Text Document Methods
  }
}
