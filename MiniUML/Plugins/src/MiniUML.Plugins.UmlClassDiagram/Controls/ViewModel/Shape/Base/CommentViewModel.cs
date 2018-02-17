using static System.String;

namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Shape.Base
{
  using System;
  using System.Xml;
  using Framework;

  /// <summary>
  /// This class manages text and header elements that can be used to create
  /// comment elements to be displayed in the UI of the application.
  /// </summary>
  public class CommentViewModel : BaseViewModel
  {
    #region fields
    public const string Comment_TAG = "Comment";

    protected const string XmlComment = "#comment";
    protected const string NameSpace = "MiniUml";

    private string mText;
    private string mTitle;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    public CommentViewModel()
    {
      mText = mTitle = Empty;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get/set text content of this comment.
    /// </summary>
    public string Text
    {
      get => mText;

        set
      {
        if (mText != value)
        {
          mText = value;
          NotifyPropertyChanged(() => Text);
        }
      }
    }
    
    /// <summary>
    /// Get/set title of this comment.
    /// </summary>
    public string Title
    {
      get => mTitle;

        set
      {
        if (mTitle != value)
        {
          mTitle = value;
          NotifyPropertyChanged(() => Title);
        }
      }
    }
    #endregion properties

    #region Read Write Comment Methods
    /// <summary>
    /// Load a string from a Text Xml tag from the <paramref name="reader"/> parameter.
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static CommentViewModel ReadDocument(XmlReader reader)
    {
      CommentViewModel ret = new CommentViewModel();

      reader.ReadToNextSibling(Comment_TAG);
      while (reader.MoveToNextAttribute())
      {
        if (ret.ReadAttributes(reader.Name, reader.Value))
        {
          if (reader.Name.Trim().Length > 0 && reader.Name != XmlComment && reader.Name != "xmlns")
            throw new ArgumentException("XML node:'" + reader.Name + "' as child of '" + Comment_TAG + "' is not supported.");
        }
      }

      ret.Text = reader.ReadString();

      // Set of comment tags is optional - so we read this only if there is content to read
      if (IsNullOrEmpty(ret.Text) &&
          IsNullOrEmpty(ret.Title))
        return null;

      return ret;
    }

      /// <summary>
      /// Save a string in an Text Xml tag into the <paramref name="writer"/> parameter.
      /// </summary>
      /// <param name="writer"></param>
      public void SaveDocument(XmlWriter writer)
    {
      // Set of comment tags is optional - so we write this only if there is content to write
      if (IsNullOrEmpty(mText) &&
          IsNullOrEmpty(mTitle))
        return;

      writer.WriteStartElement(Comment_TAG);

      SaveAttributes(writer);

      writer.WriteString(mText);   // Write text string as content of this tag
      writer.WriteEndElement();
    }

    /// <summary>
    /// Save the attribute values of this class to XML.
    /// </summary>
    /// <param name="writer"></param>
    protected virtual void SaveAttributes(XmlWriter writer)
    {
      if (IsNullOrEmpty(mTitle) == false)
        writer.WriteAttributeString("Title", Title);
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
        case "Title":
          mTitle = readerValue;
          return true;

        case "xmlns":
          if (readerValue != NameSpace)
            throw new ArgumentException("XML namespace:'" + readerValue + "' is not supported.");
          return true;

        default:
          return false;
      }
    }
    #endregion Read Write Comment Methods
  }
}
