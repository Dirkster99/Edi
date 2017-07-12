namespace Edi.Dialogs.FindReplace.Converter
{
	using System;
	using System.Globalization;
	using System.Windows.Data;

	public class BoolToInt : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((bool)value)
				return 1;
			return 0;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

	}
}
