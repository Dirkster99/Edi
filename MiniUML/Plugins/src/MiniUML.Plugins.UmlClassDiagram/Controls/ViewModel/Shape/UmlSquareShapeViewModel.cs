namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Shape
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Windows;
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
  public class UmlSquareShapeViewModel : UmlShapeBaseViewModel
  {
    #region fields
    private bool mHorizontalLine = false;
    private string mShapeImageUrl = string.Empty;
    private string mStereotype = string.Empty;
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard contructor hidding XElement constructor
    /// </summary>
    public UmlSquareShapeViewModel(IShapeParent parent,
                                    UmlTypes umlType)
      : base(parent,
             ShapeViewModelKey.SquareShape, ShapeViewModelSubKeys.None,
             umlType)
    {
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get/set property to determine whether the view should show
    /// a horizonal line or not.
    /// </summary>
    public bool HorizontalLine
    {
      get
      {
        return this.mHorizontalLine;
      }

      set
      {
        if (this.mHorizontalLine != value)
        {
          this.mHorizontalLine = value;
          this.NotifyPropertyChanged(() => this.HorizontalLine);
        }
      }
    }

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
    /// Reaf shape data from XML stream and return it.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="parent"></param>
    /// <param name="umlType"></param>
    /// <returns></returns>
    public static UmlSquareShapeViewModel ReadDocument(XmlReader reader,
                                                       IShapeParent parent,
                                                       UmlTypes umlType)
    {
      UmlSquareShapeViewModel ret = UmlElementDataDef.CreateShape(umlType, new Point(UmlTypeToStringConverter.DefaultX,
                                                                                     UmlTypeToStringConverter.DefaultY), parent)
                                                                                     as UmlSquareShapeViewModel;

      reader.ReadToNextSibling(ret.UmlDataTypeString);

      while (reader.MoveToNextAttribute())
      {
        if (ret.ReadAttributes(reader.Name, reader.Value) == false)
        {
          if (reader.Name.Trim().Length > 0 && reader.Name != UmlShapeBaseViewModel.XmlComment)
            throw new ArgumentException("XML node:'" + reader.Name + "' as attribute of '" + ret.XElementName + "' is not supported.");
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
    /// Save the attribute values of this class to XML.
    /// </summary>
    /// <param name="writer"></param>
    protected override void SaveAttributes(System.Xml.XmlWriter writer)
    {
      base.SaveAttributes(writer);

      writer.WriteAttributeString("Stereotype", string.Format("{0}", this.Stereotype));
    }

    protected override bool ReadAttributes(string readerName, string readerValue)
    {
      if (base.ReadAttributes(readerName, readerValue) == true)
        return true;

      switch (readerName)
      {
        case "Stereotype":
          this.Stereotype = readerValue;
        return true;
      }

      return false;
    }
    #endregion methods
  }
}
