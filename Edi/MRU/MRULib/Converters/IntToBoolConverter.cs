namespace MRULib.Converters
{
    using System;
    using System.Windows.Data;

    /// <summary>
    /// XAML mark up extension to convert a null value into a visibility value.
    /// </summary>
    [ValueConversion(typeof(int), typeof(bool))]
    public class IntToBoolConverter : IValueConverter
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        public IntToBoolConverter()
        {
        }

        #region IValueConverter
        /// <summary>
        /// Int to bool conversion method returns false if int value in <paramref name="value"/>
        /// is zero, otherwse false.
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
                    return false;
            }

            return true;
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
