namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Connect
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Windows.Media;
  using System.Xml;
  using MiniUML.Framework;
  using MiniUML.Model.ViewModels;
  using MiniUML.Model.ViewModels.Shapes;
  using MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.UmlElements;
  using MiniUML.Plugins.UmlClassDiagram.Converter;

  /// <summary>
  /// Maintains a viewmodel that manages the data required for an UML Assoziation
  /// view (thats a line with optional arrows, stroke etc.) between two shapes).
  /// </summary>
  public class UmlAssociationShapeViewModel : ShapeViewModelBase
  {
    #region fields
    protected const string XmlComment = "#comment";
    protected const string NameSpace = "MiniUml";

    protected readonly string mElementName = "GenericUmlAssociation";

    private readonly ShapeViewModelKey mShapeKey = ShapeViewModelKey.AssocationShape;
    private readonly UmlTypes mUmlType = UmlTypes.Undefined;

    private ConnectorKeys mFromConnectorKey = ConnectorKeys.Undefined;
    private ConnectorKeys mToConnectorKey = ConnectorKeys.Undefined;
    private string mFrom = string.Empty;
    private string mTo = string.Empty;
    private Brush mStroke = Brushes.Black;
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard contructor hidding XElement constructor
    /// </summary>
    public UmlAssociationShapeViewModel(IShapeParent parent,
                                          ConnectorKeys fromConnectorKey,
                                          ConnectorKeys toConnectorKey,
                                          UmlTypes umlType)
      : base(parent)
    {
      this.mUmlType = umlType;
      this.mFromConnectorKey = fromConnectorKey;
      this.mToConnectorKey = toConnectorKey;

      this.mElementName = UmlTypeToStringConverter.Instance.Convert(umlType, umlType.GetType(), null,
                                                                    System.Globalization.CultureInfo.InvariantCulture) as string;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get name of Xml node name that is associated with this type of connector shape.
    /// </summary>
    public override string XElementName
    {
      get
      {
        return this.mElementName;
      }
    }

    /// <summary>
    /// Get/set Id of object to connect the line from.
    /// </summary>
    public string From
    {
      get
      {
        return this.mFrom;
      }

      set
      {
        if (this.mFrom != value)
        {
          this.mFrom = value;
          this.NotifyPropertyChanged(() => this.From);
        }
      }
    }

    /// <summary>
    /// Get/set Id of object to connect the line to.
    /// </summary>
    public string To
    {
      get
      {
        return this.mTo;
      }

      set
      {
        if (this.mTo != value)
        {
          this.mTo = value;
          this.NotifyPropertyChanged(() => this.To);
        }
      }
    }

    /// <summary>
    /// Get/set shape of arraow head on the 'from' part of the connecting line
    /// </summary>
    public string FromArrow
    {
      get
      {
        return ConnectorKeyStrings.GetConnectorPresentationKey(this.mFromConnectorKey);
      }

      set
      {
        ConnectorKeys c = ConnectorKeyStrings.GetConnectorEnumKey(value);

        if (this.mFromConnectorKey != c)
        {
          this.mFromConnectorKey = c;

          this.NotifyPropertyChanged(() => this.FromArrow);
        }
      }
    }

    /// <summary>
    /// Get/set shape of arraow head on the 'to' part of the connecting line
    /// </summary>
    public string ToArrow
    {
      get
      {
        return ConnectorKeyStrings.GetConnectorPresentationKey(this.mToConnectorKey);
      }

      set
      {
        ConnectorKeys c = ConnectorKeyStrings.GetConnectorEnumKey(value);

        if (this.mToConnectorKey != c)
        {
          this.mToConnectorKey = c;

          this.NotifyPropertyChanged(() => this.ToArrow);
        }
      }
    }

    /// <summary>
    /// Get/set stroke color of connecting line shape
    /// </summary>
    public Brush Stroke
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
        return ShapeViewModelKeyStrings.GetPresentationStringKey(this.mShapeKey);
      }
    }

    /// <summary>
    /// Get <seealso cref="UmlTypes"/> enumeration member to identify this objects content
    /// </summary>
    public UmlTypes UmlDataType
    {
      get
      {
        return this.mUmlType;
      }
    }
    
    /// <summary>
    /// Get <seealso cref="UmlTypes"/> enumeration member as string to
    /// identify this objects content and map it into a resource key string.
    /// </summary>
    public string UmlDataTypeString
    {
      get
      {
        object o = UmlTypeToStringConverter.Instance.Convert(this.UmlDataType, this.UmlDataType.GetType(),
                                                             null, CultureInfo.InvariantCulture);

        if ((o is string) == false)
          throw new ArgumentException(string.Format("Node name: '{0}' is not supported.", this.UmlDataType));

        string umlType = o as string;

        return umlType;
      }
    }
    #endregion properties

    #region methods

    public static UmlAssociationShapeViewModel ReadDocument(XmlReader reader,
                                                              IShapeParent parent,
                                                              UmlTypes umlType)
    {
      UmlAssociationShapeViewModel ret = UmlElementDataDef.CreateShape(umlType,
                                                new System.Windows.Point(UmlTypeToStringConverter.DefaultX,
                                                                         UmlTypeToStringConverter.DefaultY), parent)
                                                                         as UmlAssociationShapeViewModel;

      reader.ReadToNextSibling(ret.UmlDataTypeString);

      while (reader.MoveToNextAttribute())
      {
        if (ret.ReadAttributes(reader.Name, reader.Value) == false)
        {
          if (reader.Name.Trim().Length > 0 && reader.Name != XmlComment)
            throw new ArgumentException("XML node:'" + reader.Name + "' as child of '" + ret.UmlDataTypeString + "' is not supported.");
        }
      }

      // Read child elements of this XML node
      while (reader.Read())
      {
        if (reader.NodeType == XmlNodeType.Element)
        {
          switch (reader.Name)
          {
            case "Anchor":
              AnchorViewModel p = new AnchorViewModel(parent)
              {
                Left = 0,
                Top = 0
              };

              AnchorViewModel.ReadDocument(reader.ReadSubtree(), parent, p);
              ret.Add(p);
              break;

            default:
              throw new NotImplementedException(string.Format("'{0}' is not a valid sub-node of {1}", reader.Name, ret.XElementName));
          }
        }
      }

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
      writer.WriteStartElement(this.XElementName);

      this.SaveAttributes(writer);

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
    protected virtual void SaveAttributes(System.Xml.XmlWriter writer)
    {
      writer.WriteAttributeString("Name", string.Format("{0}", this.Name));
      writer.WriteAttributeString("ID", string.Format("{0}", this.ID));
      writer.WriteAttributeString("Position", string.Format("{0},{1}", this.Left, this.Top));

      writer.WriteAttributeString("From", string.Format("{0}", this.From));
      writer.WriteAttributeString("FromArrow", string.Format("{0}", this.FromArrow));

      writer.WriteAttributeString("To", string.Format("{0}", this.To));
      writer.WriteAttributeString("ToArrow", string.Format("{0}", this.ToArrow));
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
        case "Name":
          this.Name = readerValue;
          return true;

        case "ID":
          this.ID = readerValue;
          return true;

        case "Position":
          double[] size = FrameworkUtilities.GetDoubleAttributes(readerValue, 2, new double[] { 100, 100 });
          this.Position = new System.Windows.Point(size[0], size[1]);
          return true;

        case "From":
          this.From = readerValue;
          return true;

        case "FromArrow":
          this.FromArrow = readerValue;
          return true;

        case "To":
          this.To = readerValue;
          return true;

        case "ToArrow":
          this.ToArrow = readerValue;
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
