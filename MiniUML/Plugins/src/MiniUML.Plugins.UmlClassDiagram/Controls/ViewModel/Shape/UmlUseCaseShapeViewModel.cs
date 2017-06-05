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
      this.MinHeight = 50;
      this.MinWidth = 50;
    }
    #endregion constructor

    #region properties
    public string Stroke
    {
      get
      {
        return this.mStroke;
      }

      set
      {
        if (this.mStroke != value)
        {
          this.mStroke = value;
          this.NotifyPropertyChanged(() => this.Stroke);
        }
      }
    }

    public string StrokeDashArray
    {
      get
      {
        return this.mStrokeDashArray;
      }

      set
      {
        if (this.mStrokeDashArray != value)
        {
          this.mStrokeDashArray = value;
          this.NotifyPropertyChanged(() => this.StrokeDashArray);
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
