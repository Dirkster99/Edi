namespace FolderBrowser.Converters
{
  using System;
  using System.Windows;
  using System.Windows.Data;
  using System.Windows.Media;

  /// <summary>
  /// Converte <seealso cref="System.Environment.SpecialFolder"/> enum members
  /// into <seealso cref="ImageSource"/> from ResourceDictionary or fallback from static resource.
  /// </summary>
  [ValueConversion(typeof(System.Environment.SpecialFolder), typeof(ImageSource))]
  public class SpecialFolderToImageResourceConverter : IValueConverter
  {
    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    public SpecialFolderToImageResourceConverter()
    {
    }
    #endregion constructor

    #region methods
    /// <summary>
    /// Converts a <seealso cref="System.Environment.SpecialFolder"/> enumeration member
    /// into a dynamic resource or a fallback image Url (if dynamic resource is not available).
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
        return Binding.DoNothing;

      if ((value is System.Environment.SpecialFolder) == false)
        return Binding.DoNothing;

      System.Environment.SpecialFolder folder = (System.Environment.SpecialFolder)value;

      var type = (System.Environment.SpecialFolder)value;
      
      object item = Application.Current.Resources[string.Format("SpecialFolder_Image_{0}", Enum.GetName(typeof(System.Environment.SpecialFolder), type))];
      
      if (item != null)
        return item;

      string pathValue = null;

      switch (folder)
      {
        case Environment.SpecialFolder.Desktop:
          pathValue = "pack://application:,,,/FolderBrowser;component/Images/Generic/Desktop.png";
          break;
        case Environment.SpecialFolder.Favorites:
          pathValue = "pack://application:,,,/FolderBrowser;component/Images/Generic/Favourites.png";
          break;
        case Environment.SpecialFolder.MyDocuments:
          pathValue = "pack://application:,,,/FolderBrowser;component/Images/Generic/MyDocuments.png";
          break;
        case Environment.SpecialFolder.MyMusic:
          pathValue = "pack://application:,,,/FolderBrowser;component/Images/Generic/MyMusic.png";
          break;
        case Environment.SpecialFolder.MyPictures:
          pathValue = "pack://application:,,,/FolderBrowser;component/Images/Generic/MyPictures.png";
          break;
        case Environment.SpecialFolder.MyVideos:
          pathValue = "pack://application:,,,/FolderBrowser;component/Images/Generic/MyVideos.png";
          break;
        case Environment.SpecialFolder.ProgramFiles:
        case Environment.SpecialFolder.ProgramFilesX86:
          pathValue = "pack://application:,,,/FolderBrowser;component/Images/Generic/ProgramFiles.png";
          break;
      }

      if (pathValue != null)
      {
        try
        {
          Uri imagePath = new Uri(pathValue, UriKind.RelativeOrAbsolute);
          ImageSource source = new System.Windows.Media.Imaging.BitmapImage(imagePath);

          return source;
        }
        catch
        {
        }
      }

      // Attempt to load fallback folder from ResourceDictionary
      item = Application.Current.Resources[string.Format("SpecialFolder_Image_{0}", "Image_Folder")];

      if (item != null)
        return item;
      else
      {
        // Attempt to load fallback folder from fixed Uri
        pathValue = "pack://application:,,,/FolderBrowser;component/Images/Generic/Folder.png";

        try
        {
          Uri imagePath = new Uri(pathValue, UriKind.RelativeOrAbsolute);
          ImageSource source = new System.Windows.Media.Imaging.BitmapImage(imagePath);

          return source;
        }
        catch
        {
        }
      }

      return null;
    }

    /// <summary>
    /// Convert back method is not implemented an will throw an exception upon execution.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
    #endregion methods
  }
}
