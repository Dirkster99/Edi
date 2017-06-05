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
  /// This class implements the viewmodel for Uml decision shapes
  /// with a diamond shape that users can draw on a UML canvas.
  /// Typically, these are decision shapes in activity diagrams
  /// or other items.
  /// </summary>
  public class UmlDecisionShapeViewModel : UmlShapeBaseViewModel
  {
    #region constructor
    /// <summary>
    /// Standard contructor hidding XElement constructor
    /// </summary>
    public UmlDecisionShapeViewModel(IShapeParent parent,
                                     UmlTypes umlType)
      : base(parent,
             ShapeViewModelKey.DecisionShape, ShapeViewModelSubKeys.None,
             umlType)
    {
    }
    #endregion constructor

    #region properties
    #endregion properties

    #region methods
    /// <summary>
    /// Read shape data from XML stream and return it.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="parent"></param>
    /// <param name="umlType"></param>
    /// <returns></returns>
    public static UmlDecisionShapeViewModel ReadDocument(XmlReader reader,
                                                       IShapeParent parent,
                                                       UmlTypes umlType)
    {
      UmlDecisionShapeViewModel ret = UmlElementDataDef.CreateShape(umlType, new Point(UmlTypeToStringConverter.DefaultX,
                                                                                       UmlTypeToStringConverter.DefaultY), parent)
                                                                                       as UmlDecisionShapeViewModel;

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
