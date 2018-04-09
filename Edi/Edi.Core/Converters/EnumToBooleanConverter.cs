namespace Edi.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// This class can be used to databind a (group of) radio button control(s)
    /// with an enumeration in a ViewModel.
    /// 
    /// Source: http://www.wpftutorial.net/RadioButton.html
    /// </summary>
    public class EnumToBooleanConverter : IValueConverter
	{
		/// <summary>
		/// Enum to Boolean Converter method
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || parameter == null)
				return false;

			string checkValue = value.ToString();
			string targetValue = parameter.ToString();

			bool bRet = checkValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase);

			return bRet;
		}

		/// <summary>
		/// Boolean to Enum Converter method
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || parameter == null)
				return null;

			bool useValue = (bool)value;
			string targetValue = parameter.ToString();
			if (useValue)
				return Enum.Parse(targetType, targetValue);

			return null;
		}
	}
}
