namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel
{
    using System;
    using System.Windows;
    using Framework;
    using Model.ViewModels.Document;

  public class PageViewModel : PageViewModelBase
  {
    public const string XmlElementName = "Document";

    /// <summary>
    /// Read an attribute value from the given string and return true if it was successful,
    /// otherwise, return false.
    /// </summary>
    /// <param name="readerName"></param>
    /// <param name="readerValue"></param>
    /// <returns></returns>
    public bool ReadAttribute(string readerName, string readerValue)
    {
      switch (readerName)
      {
        case "Size":
          var size = FrameworkUtilities.GetDoubleAttributes(readerValue, 2,
                                                                  new double[] { DefaultXSize, DefaultYSize });
          prop_PageSize = new Size(size[0], size[1]);
          return true;

        case "xmlns":
          if (readerValue != NameSpace)
            throw new ArgumentException("XML namespace:'" + readerValue + "' is not supported.");

          return true;

        default:
          if (readerName.Trim().Length > 0 && readerName != XmlComment)
            throw new ArgumentException("XML node:'" + readerName + "' as child of '" + XmlElementName + "' is not supported.");
          break;
      }

      return false;
    }
  }
}
