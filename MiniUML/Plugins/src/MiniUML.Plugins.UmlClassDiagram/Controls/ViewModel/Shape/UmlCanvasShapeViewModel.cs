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

  public class UmlCanvasShapeViewModel : UmlShapeBaseViewModel
  {
    #region fields
    private readonly ShapeViewModelSubKeys mCanvasShape = ShapeViewModelSubKeys.CanvasUmlMan;
    private string mUmlShapeKey = string.Empty;
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard contructor hidding XElement constructor
    /// </summary>
    public UmlCanvasShapeViewModel(IShapeParent parent,
                              ShapeViewModelSubKeys canvasShape,
                              UmlTypes umlType)
      : base(parent, 
             ShapeViewModelKey.CanvasShape, canvasShape, umlType)
    {
      mCanvasShape = canvasShape;
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
        return mElementName;
      }
    }

    /// <summary>
    /// Get/set type of actual shape to represented by this canvas viewmodel
    /// (eg.: UmlMan etc...)
    /// </summary>
    public ShapeViewModelSubKeys CanvasShape
    {
      get
      {
        return mCanvasShape;
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
    public static UmlCanvasShapeViewModel ReadDocument(XmlReader reader,
                                                  IShapeParent parent,
                                                  UmlTypes umlType)
    {
      UmlCanvasShapeViewModel ret = UmlElementDataDef.CreateShape(umlType, new System.Windows.Point(UmlTypeToStringConverter.DefaultX,
                                                                                               UmlTypeToStringConverter.DefaultY), parent)
                                                                                               as UmlCanvasShapeViewModel;
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
