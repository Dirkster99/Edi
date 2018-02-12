namespace FolderBrowser.Converters
{
    using FileSystemModels.Models.FSItems.Base;
    using System;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;
    using System.Windows.Media;

    /// <summary>
    /// XAML markup extension to convert <seealso cref="FSItemType"/> enum members
    /// into <seealso cref="ImageSource"/> from ResourceDictionary or fallback from static resource.
    /// </summary>
    [ValueConversion(typeof(FSItemType), typeof(ImageSource))]
    [MarkupExtensionReturnType(typeof(IMultiValueConverter))]
    public class BrowseItemTypeToImageConverter : MarkupExtension, IMultiValueConverter
    {
        #region fields
        /// <summary>
        /// Log4net logger facility.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static BrowseItemTypeToImageConverter converter;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public BrowseItemTypeToImageConverter()
        {
        }
        #endregion constructor

        #region methods
        /// <summary>
        /// Returns an object that is provided
        /// as the value of the target property for this markup extension.
        /// 
        /// When a XAML processor processes a type node and member value that is a markup extension,
        /// it invokes the ProvideValue method of that markup extension and writes the result into the
        /// object graph or serialization stream. The XAML object writer passes service context to each
        /// such implementation through the serviceProvider parameter.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (converter == null)
            {
                converter = new BrowseItemTypeToImageConverter();
            }

            return converter;
        }

        /// <summary>
        /// Converts a <seealso cref="FSItemType"/> enumeration member
        /// into a dynamic resource or a fallback image Url (if dynamic resource is not available).
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
                return Binding.DoNothing;

            if (values.Length != 2)
                return Binding.DoNothing;

            if (values.Length != 2)
                return Binding.DoNothing;

            bool? bIsExpanded = values[0] as bool?;
            FSItemType? itemType = values[1] as FSItemType?;

            if (bIsExpanded == null && itemType == null)
            {
                bIsExpanded = values[0] as bool?;
                itemType = values[1] as FSItemType?;

                if (bIsExpanded == null && itemType == null)
                    return Binding.DoNothing;
            }

            if (bIsExpanded == true)
                return GetExpandedImages((FSItemType)itemType);
            else
                return GetNotExpandedImages((FSItemType)itemType);
        }

        /// <summary>
        /// Converts back method is not implemented and will throw an exception.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a DynamicResource from ResourceDictionary or a static ImageSource (as fallback) for not expanded folder item.
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        private object GetNotExpandedImages(FSItemType itemType)
        {
            string uriPath = null;

            switch (itemType)
            {
                case FSItemType.Folder:
                    uriPath = string.Format("FolderItem_Image_{0}", "FolderClosed");
                    break;

                case FSItemType.LogicalDrive:
                    uriPath = string.Format("FolderItem_Image_{0}", "HardDisk");
                    break;

                default:
                case FSItemType.File:
                case FSItemType.Unknown:
                    Logger.Error("Type of item is not supported:" + itemType.ToString());
                    break;
            }

            object item = null;

            if (uriPath != null)
            {
                item = Application.Current.Resources[uriPath];

                if (item != null)
                    return item;
            }

            string pathValue = null;

            switch (itemType)
            {
                case FSItemType.Folder:
                    pathValue = "pack://application:,,,/FolderBrowser;component/Images/Generic/FolderClosed.png";
                    break;

                case FSItemType.LogicalDrive:
                    pathValue = "pack://application:,,,/FolderBrowser;component/Images/Generic/HardDisk.ico";
                    break;

                default:
                case FSItemType.File:
                case FSItemType.Unknown:
                    Logger.Error("Type of item is not supported:" + itemType.ToString());
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
            item = Application.Current.Resources[string.Format("FolderItem_Image_{0}", "FolderClosed")];

            if (item != null)
                return item;
            else
            {
                // Attempt to load fallback folder from fixed Uri
                pathValue = "pack://application:,,,/FolderBrowser;component/Images/Generic/FolderClosed.png";

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
        /// Get a DynamicResource from ResourceDictionary or a static ImageSource (as fallback) for expanded folder item.
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        private object GetExpandedImages(FSItemType itemType)
        {
            string uriPath = null;

            switch (itemType)
            {
                case FSItemType.Folder:
                    uriPath = string.Format("FolderItem_Image_{0}", "FolderOpen");
                    break;

                case FSItemType.LogicalDrive:
                    uriPath = string.Format("FolderItem_Image_{0}", "HardDisk");
                    break;

                default:
                case FSItemType.File:
                case FSItemType.Unknown:
                    Logger.Error("Type of item is not supported:" + itemType.ToString());
                    break;
            }

            object item = null;

            if (uriPath != null)
            {
                item = Application.Current.Resources[uriPath];

                if (item != null)
                    return item;
            }

            string pathValue = null;

            switch (itemType)
            {
                case FSItemType.Folder:
                    pathValue = "pack://application:,,,/FolderBrowser;component/Images/Generic/FolderOpen.png";
                    break;

                case FSItemType.LogicalDrive:
                    pathValue = "pack://application:,,,/FolderBrowser;component/Images/Generic/HardDisk.ico";
                    break;

                default:
                case FSItemType.File:
                case FSItemType.Unknown:
                    Logger.Error("Type of item is not supported:" + itemType.ToString());
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
            item = Application.Current.Resources[string.Format("FolderItem_Image_{0}", "FolderOpen")];

            if (item != null)
                return item;
            else
            {
                // Attempt to load fallback folder from fixed Uri
                pathValue = "pack://application:,,,/FolderBrowser;component/Images/Generic/FolderOpen.png";

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
        #endregion methods
    }
}
