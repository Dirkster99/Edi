namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Shape.Base
{
  using System;
  using System.Collections.ObjectModel;
  using System.Globalization;
  using System.Xml;
  using Framework;
  using Model.ViewModels;
  using Model.ViewModels.Shapes;
  using Converter;

  public abstract class UmlShapeBaseViewModel : ShapeSizeViewModelBase
  {
    #region fields
    protected const string XmlComment = "#comment";
    protected const string NameSpace = "MiniUml";

    protected readonly string mElementName = string.Empty;

    private readonly ShapeViewModelKey mShapeKey = ShapeViewModelKey.Undefined;
    private readonly ShapeViewModelSubKeys mShapeViewModelSubKeys = ShapeViewModelSubKeys.Undefined;
    private readonly UmlTypes mUmlType = UmlTypes.Undefined;
    private readonly ObservableCollection<CommentViewModel> mComments = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="ElementName"></param>
    /// <param name="shapeKey"></param>
    /// <param name="shapeViewModelSubKeys"></param>
    public UmlShapeBaseViewModel(IShapeParent parent,
                                 ShapeViewModelKey shapeKey,
                                 ShapeViewModelSubKeys shapeViewModelSubKeys,
                                 UmlTypes umlType)
      : base(parent)
    {
      mShapeKey = shapeKey;
      mShapeViewModelSubKeys = shapeViewModelSubKeys;
      mUmlType = umlType;

      mComments = new ObservableCollection<CommentViewModel>();

      mElementName = UmlTypeToStringConverter.Instance.Convert(umlType, umlType.GetType(), null,
                                                                    CultureInfo.InvariantCulture) as string;
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
    /// This key identifies the viewmodel implementation that is associated with a view control.
    /// </summary>
    public ShapeViewModelKey ShapeKey
    {
      get
      {
        return mShapeKey;
      }
    }

    /// <summary>
    /// Some viewmodels can manage multiple sub-views. This property holds
    /// the key to those sub-view <-> sub-viewmodel association (if any).
    /// </summary>
    public ShapeViewModelSubKeys ShapeSubKey
    {
      get
      {
        return mShapeViewModelSubKeys;
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

    /// <summary>
    /// Get the unique enumerable type of the view-viewmodel type
    /// that is managed in this object.
    /// </summary>
    public UmlTypes UmlDataType
    {
      get
      {
        return mUmlType;
      }
    }

    /// <summary>
    /// Sting representation of the UmlDataType property. This can be used when
    /// direct string matching is required.
    /// </summary>
    public string UmlDataTypeString
    {
      get
      {
        object o = UmlTypeToStringConverter.Instance.Convert(UmlDataType, UmlDataType.GetType(),
                                                             null, CultureInfo.InvariantCulture);

        if ((o is string) == false)
          throw new ArgumentException(string.Format("Node name: '{0}' is not supported.", UmlDataType));

        string umlType = o as string;

        return umlType;
      }
    }
    #endregion properties

    #region methodes
    /// <summary>
    /// Read the complete content of the associated XML node for this type of object.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="ret"></param>
    /// <returns></returns>
    public static string ReadDocument(XmlReader reader,
                                      UmlShapeBaseViewModel ret)
    {
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

      return string.Empty;
    }

    public void AddComment(CommentViewModel c)
    {
      mComments.Add(c);
    }

    /// <summary>
    /// Persist the contents of this object into the given
    /// parameter <paramref name="writer"/> object.
    /// </summary>
    /// <param name="writer"></param>
    public void SaveDocument(XmlWriter writer)
    {
      // persist comment information as series of optional XML tags
      foreach (var item in mComments)
      {
        item.SaveDocument(writer);
      }
    }

    /// <summary>
    /// Read the content of an XML node and return an error string
    /// if the XML node was not known to this method.
    /// </summary>
    /// <param name="nodeName"></param>
    /// <param name="reader"></param>
    /// <returns></returns>
    protected string ReadXMLNode(string nodeName, XmlReader reader)
    {
      switch (nodeName)
      {
        case CommentViewModel.Comment_TAG:
          CommentViewModel c = CommentViewModel.ReadDocument(reader.ReadSubtree());

          if (c != null)           // Add into list of comments
            AddComment(c);
          return string.Empty;

        default:
          return string.Format("'{0}' is not a valid sub-node of {1}", reader.Name, XElementName);
      }
    }

    /// <summary>
    /// Save the attribute values of this class to XML.
    /// </summary>
    /// <param name="writer"></param>
    protected virtual void SaveAttributes(XmlWriter writer)
    {
      writer.WriteAttributeString("Name", string.Format("{0}", Name));
      writer.WriteAttributeString("ID", string.Format("{0}", ID));

      writer.WriteAttributeString("Position", string.Format("{0},{1},{2},{3}",
                                              Left, Top, Width, Height));
    }

    /// <summary>
    /// Read the attribute values of this class from XML.
    /// </summary>
    /// <param name="readerName"></param>
    /// <param name="readerValue"></param>
    /// <returns></returns>
    protected virtual bool ReadAttributes(string readerName, string readerValue)
    {
      switch (readerName)
      {
        case "Position":
          double[] size = FrameworkUtilities.GetDoubleAttributes(readerValue, 4,
                                                                 new double[] { 100, 100, 200, 200 });
          Position = new System.Windows.Point(size[0], size[1]);
          Width = size[2];
          Height = size[3];
          return true;

        case "ID":
          ID = readerValue;
          return true;

        case "Name":
          Name = readerValue;
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
