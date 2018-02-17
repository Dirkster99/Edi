namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Connect
{
  using System;
  using System.Collections.Generic;
  using System.Windows;
  using System.Xml;
  using Framework;
  using Model.ViewModels;
  using Model.ViewModels.Shapes;

  /// <summary>
  /// This class manages an anchor which can be a thumb that can be dragged on a view.
  /// THe class is final (sealed) since it has no <seealso cref="UmlType"/> and hence
  /// cannot be converted using a converter. It is therefore, only applicable as part
  /// of other classes (and shapes).
  /// </summary>
  public sealed class AnchorViewModel : AnchorBaseViewModel
  {
    #region fields
    private const string XmlComment = "#comment";
    private const string NameSpace = "MiniUml";
    private readonly ShapeViewModelKey mShapeKey = ShapeViewModelKey.AssocationShape;
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard contructor hidding XElement constructor
    /// </summary>
    public AnchorViewModel(IShapeParent parent)
      : base(parent)
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
        return "Anchor";
      }
    }

    /// <summary>
    /// Get string identifier that represents the ImplementingViewModel
    /// as <seealso cref="ShapeViewModelKey"/> does. This identifier can
    /// be used by the TemplateSelector to find the implementing view
    /// (this extra property avoids an extra string cast -
    /// a string datatype is necessary since the template selector lives outside of this plug-in).
    /// </summary>
    public override string TypeKey
    {
      get
      {
        return ShapeViewModelKeyStrings.GetPresentationStringKey(mShapeKey);
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Persist the contents of this object into the given
    /// parameter <paramref name="writer"/> object.
    /// </summary>
    /// <param name="writer"></param>
    public override void SaveDocument(XmlWriter writer,
                                      IEnumerable<ShapeViewModelBase> root)
    {
      writer.WriteStartElement(XElementName);

      SaveAttributes(writer);

      if (root != null)
      {
        foreach (var item in root)
        {
          item.SaveDocument(writer, item.Elements());
        }
      }

      writer.WriteEndElement();
    }

    internal static AnchorViewModel ReadDocument(XmlReader reader,
                                                 IShapeParent parent,
                                                 AnchorViewModel anchor)
    {
      reader.ReadToNextSibling(anchor.XElementName);

      while (reader.MoveToNextAttribute())
      {
        if (anchor.ReadAttributes(reader.Name, reader.Value) == false)
          throw new NotImplementedException(string.Format("The attribute '{0}' is not supported in '{1}'.", reader.Name, anchor.XElementName));
      }

      return anchor;
    }

    /// <summary>
    /// Save the attribute values of this class to XML.
    /// </summary>
    /// <param name="writer"></param>
    private void SaveAttributes(XmlWriter writer)
    {
      writer.WriteAttributeString("Name", string.Format("{0}", Name));
      writer.WriteAttributeString("ID", string.Format("{0}", ID));

      writer.WriteAttributeString("Position", string.Format("{0},{1}", Left, Top));
    }

    /// <summary>
    /// Read the attribute values of this class from XML.
    /// </summary>
    /// <param name="readerName"></param>
    /// <param name="readerValue"></param>
    /// <returns></returns>
    private bool ReadAttributes(string readerName, string readerValue)
    {
      switch (readerName)
      {
        case "Name":
          Name = readerValue;
          return true;

        case "ID":
          ID = readerValue;
          return true;

        case "Position":
          double[] size = FrameworkUtilities.GetDoubleAttributes(readerValue, 2, new double[] { 100, 100 });
          Position = new Point(size[0], size[1]);
          return true;

        case "xmlns":
          if (readerValue != NameSpace)
            throw new ArgumentException("XML namespace:'" + readerValue + "' is not supported.");
          return true;

        default:
          return false;
      }
    }
    #endregion methods
  }
}
