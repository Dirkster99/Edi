namespace Edi.Core.Converters.MessageType
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    [ValueConversion(typeof(int), typeof(Visibility))]
	public class CountToVisibilityHiddenConverter : IValueConverter
	{
		#region IValueConverter Members
		/// <summary> 
		/// Converts a value. 
		/// </summary> 
		/// <param name="value">The value produced by the binding source.</param> 
		/// <param name="targetType">The type of the binding target property.</param> 
		/// <param name="parameter">The converter parameter to use.</param> 
		/// <param name="culture">The culture to use in the converter.</param> 
		/// <returns> 
		/// A converted value. If the method returns null, the valid null value is used. 
		/// </returns> 
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
            if (value is int && targetType == typeof(Visibility))
            {
                int val = (int)value;
                if (val > 0)
                    return Visibility.Visible;
                else
                    if (parameter != null && parameter is Visibility)
                    return parameter;
                else
                    return Visibility.Hidden;        ////return Visibility.Collapsed;
            }

            throw new ArgumentException("Invalid argument/return type. Expected argument: bool and return type: Visibility");
        }

        /// <summary> 
        /// Converts a value. 
        /// </summary> 
        /// <param name="value">The value that is produced by the binding target.</param> 
        /// <param name="targetType">The type to convert to.</param> 
        /// <param name="parameter">The converter parameter to use.</param> 
        /// <param name="culture">The culture to use in the converter.</param> 
        /// <returns> 
        /// A converted value. If the method returns null, the valid null value is used. 
        /// </returns> 
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is Visibility) || targetType != typeof(bool))
				throw new ArgumentException("Invalid argument/return type. Expected argument: Visibility and return type: bool");
			Visibility val = (Visibility)value;
			return val == Visibility.Visible;
		}
		#endregion
	}
}