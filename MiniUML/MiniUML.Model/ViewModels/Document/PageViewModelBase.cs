namespace MiniUML.Model.ViewModels.Document
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.IO;
  using System.Windows;
  using System.Xml;

  using Framework;
  using Shapes;

  /// <summary>
  /// Define the size of a page and contains the child elements
  /// </summary>
  public class PageViewModelBase : BaseViewModel
  {
    #region fields
    public const string AttrPageWidth = "PageWidth";
    public const string AttrPageHeight = "PageHeight";
    public const string AttrPageMargins = "PageMargins";

    public const double DefaultXSize = 1793.79;
    public const double DefaultYSize = 1793.79;

    public const double DefaultLeftMargin = 37.79;
    public const double DefaultTopMargin = 37.79;
    public const double DefaultRightMargin = 37.79;
    public const double DefaultBottomMargin = 37.79;

    protected const string NameSpace = "MiniUml";

    protected const string ShapeElementName = "Document";

    protected const string XmlComment = "#comment";

    private readonly ObservableCollection<ShapeViewModelBase> mElements = new ObservableCollection<ShapeViewModelBase>();

    private Thickness mPageMargins;
    private Size mPageSize;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class Constructor
    /// </summary>
    public PageViewModelBase()
    {
      prop_PageSize = new Size(DefaultXSize, DefaultYSize);
      prop_PageMargins = new Thickness(DefaultLeftMargin, DefaultTopMargin,
                                            DefaultRightMargin, DefaultBottomMargin);
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copyThis"></param>
    public PageViewModelBase(PageViewModelBase copyThis)
    : this()
    {
      if (copyThis == null)
        return;

      if (copyThis.mElements != null)
        mElements = new ObservableCollection<ShapeViewModelBase>(copyThis.mElements);

      if (copyThis.mPageMargins != null)
        mPageMargins = new Thickness(copyThis.mPageMargins.Left, copyThis.mPageMargins.Top,
                                          copyThis.mPageMargins.Right, copyThis.mPageMargins.Bottom);

      if (copyThis.mPageSize != null)
        mPageSize = new Size(copyThis.mPageSize.Width, copyThis.mPageSize.Height);
    }
    #endregion constructor
    
    #region properties
    public string ElementName => ShapeElementName;

      /// <summary>
    /// Get/set size of document page
    /// </summary>
    public Size prop_PageSize
    {
      get => mPageSize;

          set
      {
        mPageSize = value;
        NotifyPropertyChanged(() => prop_PageSize);
      }
    }

    /// <summary>
    /// Get/set thickness of margin around the document page
    /// </summary>
    public Thickness prop_PageMargins
    {
      get => mPageMargins;

        set
      {
        mPageMargins = value;
        NotifyPropertyChanged(() => prop_PageMargins);
      }
    }

    /*/ <summary>
    /// This property is inherited from an abstract property
    /// but does not make sense for this class - we therefore throw an error here.
    /// </summary>
    public override string TypeKey
    {
      get
      {
        throw new NotImplementedException();
      }
    }
     */
    #endregion properties

    #region methods
    /// <summary>
    /// Persist an Xml document and its content into an XML formated string.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="docRoot"></param>
    /// <returns></returns>
    public static string Write(PageViewModelBase root,
                               IEnumerable<ShapeViewModelBase> docRoot)
    {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",         // 2 spaces as indentation
                NewLineOnAttributes = true
            };

            try
      {
        using (var sw = new StringWriter())
        {
          using (XmlWriter writer = XmlWriter.Create(sw, settings))
          {
            root.SaveDocument(writer, docRoot);
          }

          return sw.ToString();
        }
      }
      catch (Exception e)
      {
        throw new Exception("An exception occured when writing XML.", e);
      }
    }

    /// <summary>
    /// Persist an Xml document and its content into an XML formated text file.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="docRoot"></param>
    /// <returns></returns>
    public static bool Write(string filePathName,
                             PageViewModelBase root,
                             IEnumerable<ShapeViewModelBase> docRoot)
    {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",         // 2 spaces as indentation
                NewLineOnAttributes = true
            };

            try
      {
        using (XmlWriter writer = XmlWriter.Create(filePathName, settings))
        {
          root.SaveDocument(writer, docRoot);
        }

          return true;
      }
      catch (Exception e)
      {
        throw new Exception("An exception occured when writing XML.", e);
      }
    }

    /// <summary>
    /// Add child objects
    /// into the collection of child objects.
    /// </summary>
    /// <param name="shape"></param>
    public void Add(ShapeViewModelBase shape)
    {
      mElements.Add(shape);
    }

    /// <summary>
    /// Add child objects from a given collection
    /// into the collection of child objects.
    /// </summary>
    /// <param name="shapes"></param>
    public void Add(IEnumerable<ShapeViewModelBase> shapes)
    {
      if (shapes != null)
      {
        foreach (var item in shapes)
        {
          mElements.Add(item);
        }
      }
    }

    public IEnumerable<ShapeViewModelBase> Elements()
    {
      return (IEnumerable<ShapeViewModelBase>)mElements;
    }

    /// <summary>
    /// Persist the contents of this object into the given
    /// parameter <paramref name="writer"/> object.
    /// </summary>
    /// <param name="writer"></param>
    public void SaveDocument(XmlWriter writer,
                             IEnumerable<ShapeViewModelBase> root)
    {
      writer.WriteStartElement(string.Empty, ElementName, NameSpace);

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

    /// <summary>
    /// Save the current content of the document as XML string
    /// (this inlcudes all children if any).
    /// </summary>
    /// <returns></returns>
    public string SaveDocument(IEnumerable<ShapeViewModelBase> mDocRoot)
    {
      return Write(this, mDocRoot);
    }

    /// <summary>
    /// Save the current content of the document into a file on the file system
    /// (this inlcudes all children if any).
    /// </summary>
    /// <param name="filename"></param>
    public bool SaveDocument(string filename, ObservableCollection<ShapeViewModelBase> mDocRoot)
    {
      return Write(filename, this, mDocRoot);
    }

    /// <summary>
    /// Save the attribute list for this shape or class
    /// </summary>
    /// <param name="writer"></param>
    protected void SaveAttributes(XmlWriter writer)
    {
      // Write documents attributes
      writer.WriteAttributeString("Size",
          $"{prop_PageSize.Width},{prop_PageSize.Height}");
    }
    #endregion methods
  }
}
