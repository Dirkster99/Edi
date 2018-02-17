namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Shape
{
  using System;
  using System.Collections.Generic;
  using System.Xml;
  using Model.ViewModels;
  using Model.ViewModels.Shapes;
  using Base;
  using UmlElements;
  using Converter;

  /// <summary>
  /// This class implements the viewmodel for Uml Package shape
  /// that users can draw on a UML canvas.
  /// </summary>
  public class UmlPackageShapeViewModel : UmlShapeBaseViewModel
  {
    #region constructor
    /// <summary>
    /// Standard contructor hidding XElement constructor
    /// </summary>
    public UmlPackageShapeViewModel(IShapeParent parent,
                                    UmlTypes umlType)
      : base(parent,
             ShapeViewModelKey.PackageShape, ShapeViewModelSubKeys.None,
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
    public static UmlPackageShapeViewModel ReadDocument(XmlReader reader,
                                                       IShapeParent parent,
                                                       UmlTypes umlType)
    {
      UmlPackageShapeViewModel ret = UmlElementDataDef.CreateShape(umlType, new System.Windows.Point(UmlTypeToStringConverter.DefaultX,
                                                                                       UmlTypeToStringConverter.DefaultY), parent)
                                                                                       as UmlPackageShapeViewModel;
      reader.ReadToNextSibling(ret.UmlDataTypeString);

      while (reader.MoveToNextAttribute())
      {
        if (ret.ReadAttributes(reader.Name, reader.Value) == false)
        {
          if (reader.Name.Trim().Length > 0 && reader.Name != XmlComment)
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
    public override void SaveDocument(XmlWriter writer,
                                      IEnumerable<ShapeViewModelBase> root)
    {
      writer.WriteStartElement(mElementName);

      SaveAttributes(writer);

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
