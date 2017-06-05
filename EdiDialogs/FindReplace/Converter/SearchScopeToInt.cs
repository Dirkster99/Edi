namespace EdiDialogs.FindReplace.Converter
{
	using System;
	using System.Globalization;
	using System.Windows.Data;

	public class SearchScopeToInt : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (int)value;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (EdiDialogs.FindReplace.SearchScope)value;
		}
	}
}
