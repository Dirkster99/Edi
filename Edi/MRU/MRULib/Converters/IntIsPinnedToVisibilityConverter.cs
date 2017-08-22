namespace MRULib.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Source: http://stackoverflow.com/questions/534575/how-do-i-invert-booleantovisibilityconverter
    /// 
    /// Implements a Boolean to Visibility converter
    /// Use ConverterParameter=true to negate the visibility - boolean interpretation.
    /// </summary>
    [ValueConversion(typeof(int), typeof(Visibility))]
    public sealed class IntIsPinnedToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Standard constructor.
        /// </summary>
        public IntIsPinnedToVisibilityConverter()
        {
            ConvertZeroToVisible = true;
        }

        /// <summary>
        /// Gets/sets a property that determines whether the integer value 0
        /// results in a value of <seealso cref="Visibility.Visibility"/> or not.
        /// 
        /// This converter will either convert:
        /// 1) int == 0 to <seealso cref="Visibility.Visibility"/> and all other values
        ///    int != 0 to <seealso cref="Visibility.Collapsed"/>
        ///    
        /// or
        /// 
        /// 2) int != 0 to <seealso cref="Visibility.Visibility"/> and all other values
        ///    int == 0 to <seealso cref="Visibility.Collapsed"/>
        /// </summary>
        public bool ConvertZeroToVisible { get; set; }

        /// <summary>
        /// Converts a <seealso cref="Boolean"/> value
        /// into a <seealso cref="Visibility"/> value based on
        /// the <seealso cref="ConvertZeroToVisible"/> property.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is int) == false)
                return Binding.DoNothing;

            int intValue = (int)value;

            if (ConvertZeroToVisible == true)
            {
                if (intValue == 0)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            else
            {
                if (intValue != 0)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Convert back is not supported.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
