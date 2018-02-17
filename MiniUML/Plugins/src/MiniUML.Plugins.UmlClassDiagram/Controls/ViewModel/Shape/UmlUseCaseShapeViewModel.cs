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
  /// This class implements the viewmodel for the Uml Use Case shape
  /// that users can draw on a UML canvas.
  /// </summary>
  public class UmlUseCaseShapeViewModel : UmlShapeBaseViewModel
  {
    #region fields
    private string mStroke = string.Empty;
    private string mStrokeDashArray = string.Empty;
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard contructor hidding XElement constructor
    /// </summary>
    public UmlUseCaseShapeViewModel(IShapeParent parent,
                                    UmlTypes umlType)
      : base(parent,
             ShapeViewModelKey.UseCaseShape, ShapeViewModelSubKeys.None,
             umlType)
    {
      MinHeight = 50;
      MinWidth = 50;
    }
    #endregion constructor

    #region properties
    public string Stroke
    {
      get
      {
        return mStroke;
      }

      set
      {
        if (mStroke != value)
        {
          mStroke = value;
          NotifyPropertyChanged(() => Stroke);
        }
      }
    }

    public string StrokeDashArray
    {
      get
      {
        return mStrokeDashArray;
      }

      set
      {
        if (mStrokeDashArray != value)
        {
          mStrokeDashArray = value;
          NotifyPropertyChanged(() => StrokeDashArray);
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
    public static UmlUseCaseShapeViewModel ReadDocument(XmlReader reader,
                                                        IShapeParent parent,
                                                        UmlTypes umlType)
    {
      UmlUseCaseShapeViewModel ret = UmlElementDataDef.CreateShape(umlType, new System.Windows.Point(UmlTypeToStringConverter.DefaultX,
                                                                                                     UmlTypeToStringConverter.DefaultY), parent)
                                                                                                     as UmlUseCaseShapeViewModel;
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
