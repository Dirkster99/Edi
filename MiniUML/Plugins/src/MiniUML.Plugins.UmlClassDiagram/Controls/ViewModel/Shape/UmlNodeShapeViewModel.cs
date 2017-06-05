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
  /// with a node or circular point shape that users can
  /// draw on a UML canvas. Typically, these are
  /// Activity Start or Activity End shapes or other items.
  /// </summary>
  public class UmlNodeShapeViewModel : UmlShapeBaseViewModel
  {
    #region fields
    private string mShapeImageUrl = string.Empty;
    private string mStereotype = string.Empty;
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard contructor hidding XElement constructor
    /// </summary>
    public UmlNodeShapeViewModel(IShapeParent parent,
                                 UmlTypes umlType)
      : base(parent,
             ShapeViewModelKey.NodeShape, ShapeViewModelSubKeys.None, umlType)
    {
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get/set property to determine whether the view should show
    /// an additional image or not.
    /// 
    /// Todo: This should be implemented via converter based on UmlShapeKey ???
    /// </summary>
    public string ShapeImageUrl
    {
      get
      {
        return this.mShapeImageUrl;
      }

      set
      {
        if (this.mShapeImageUrl != value)
        {
          this.mShapeImageUrl = value;
          this.NotifyPropertyChanged(() => this.ShapeImageUrl);
        }
      }
    }

    /// <summary>
    /// Get/set stereotype for the corresponding shape displayed in the view.
    /// </summary>
    public string Stereotype
    {
      get
      {
        return this.mStereotype;
      }

      set
      {
        if (this.mStereotype != value)
        {
          this.mStereotype = value;
          this.NotifyPropertyChanged(() => this.Stereotype);
        }
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Read shape data from XML stream and return it.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="parent"></param>
    /// <param name="umlType"></param>
    /// <returns></returns>
    public static UmlNodeShapeViewModel ReadDocument(XmlReader reader,
                                                IShapeParent parent,
                                                UmlTypes umlType)
    {
      UmlNodeShapeViewModel ret = UmlElementDataDef.CreateShape(umlType, new System.Windows.Point(UmlTypeToStringConverter.DefaultX,
                                                                                                  UmlTypeToStringConverter.DefaultY), parent)
                                                                                                  as UmlNodeShapeViewModel;
      reader.ReadToNextSibling(ret.UmlDataTypeString);

      while (reader.MoveToNextAttribute())
      {
        if (ret.ReadAttributes(reader.Name, reader.Value) == false)
        {
          if (reader.Name.Trim().Length > 0 && reader.Name != UmlShapeBaseViewModel.XmlComment)
            throw new ArgumentException("XML node:'" + reader.Name + "' as child of '" + ret.XElementName + "' is not supported.");
        }
      }

      // Read common model information (eg. comments)
      UmlShapeBaseViewModel.ReadDocument(reader, ret);

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
    /// Read the attribute values of this class from XML.
    /// </summary>
    /// <param name="readerName"></param>
    /// <param name="readerValue"></param>
    /// <returns></returns>
    protected override bool ReadAttributes(string readerName, string readerValue)
    {
      if (base.ReadAttributes(readerName, readerValue) == true)
        return true;

      switch (readerName)
      {
        case "Stereotype":
          this.Stereotype = readerValue;
          return true;

        case "ID":
          this.ID = readerValue;
          return true;

        case "xmlns":
          if (readerValue != UmlShapeBaseViewModel.NameSpace)
            throw new ArgumentException("XML namespace:'" + readerValue + "' is not supported.");
          return true;

        default:
          return false;
      }
    }

    /// <summary>
    /// Save the attribute values of this class to XML.
    /// </summary>
    /// <param name="writer"></param>
    protected override void SaveAttributes(System.Xml.XmlWriter writer)
    {
      base.SaveAttributes(writer);

      writer.WriteAttributeString("Stereotype", string.Format("{0}", this.Stereotype));
    }
    #endregion methods
  }
}
