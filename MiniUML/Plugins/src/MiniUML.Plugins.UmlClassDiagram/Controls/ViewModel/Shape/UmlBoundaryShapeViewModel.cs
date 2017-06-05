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
  /// This class implements the viewmodel for Uml boundary shape
  /// that users can draw on a UML canvas.
  /// </summary>
  public class UmlBoundaryShapeViewModel : UmlShapeBaseViewModel
  {
    #region constructor
    /// <summary>
    /// Standard contructor hidding XElement constructor
    /// </summary>
    public UmlBoundaryShapeViewModel(IShapeParent parent,
                                UmlTypes umlType)

      : base(parent, ShapeViewModelKey.BoundaryShape,
                     ShapeViewModelSubKeys.None,
                     umlType)
    {
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get the (XML) key name of this objects class of elements.
    /// </summary>
    public override string XElementName
    {
      get
      {
        return this.mElementName;
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
    public static UmlBoundaryShapeViewModel ReadDocument(XmlReader reader,
                                                    IShapeParent parent,
                                                    UmlTypes umlType)
    {
      UmlBoundaryShapeViewModel ret = UmlElementDataDef.CreateShape(umlType, new System.Windows.Point(UmlTypeToStringConverter.DefaultX,
                                                                                                 UmlTypeToStringConverter.DefaultY), parent)
                                                                                                 as UmlBoundaryShapeViewModel;
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
    #endregion methods
  }
}
