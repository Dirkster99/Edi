namespace MRULib.Converters
{
    using System;
    using System.Windows.Data;

    /// <summary>
    /// XAML mark up extension to convert a null value into a visibility value.
    /// </summary>
    [ValueConversion(typeof(int), typeof(System.Windows.Visibility))]
    public class ZeroToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        public ZeroToVisibilityConverter()
        {
        }

        #region IValueConverter
        /// <summary>
        /// Zero to visibility conversion method
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return System.Windows.Visibility.Collapsed;

            if (value is int)
            {
                if ((int)value == 0)
                    return System.Windows.Visibility.Collapsed;
            }

            return System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Visibility to Zero conversion method (is disabled and will throw an exception when invoked)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion IValueConverter
    }
}
