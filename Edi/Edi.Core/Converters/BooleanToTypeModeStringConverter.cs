namespace Edi.Core.Converters
{
	using System;
	using System.Globalization;
	using System.Windows.Data;

	/// <summary>
	/// </summary>
	[ValueConversion(typeof(bool), typeof(string))]
	public class BooleanToTypeModeStringConverter : IValueConverter
	{
		public readonly static string TypeOver = Util.Local.Strings.STR_EDIT_MODE_TYPEOVER;
		public readonly static string TypeToInsert = Util.Local.Strings.STR_EDIT_MODE_INSERT;

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
			if ((value is bool) == false)
				throw new ArgumentException("Invalid argument/return type. Expected argument: bool (return type: string).");

			if (value == null)
				return TypeToInsert;

			bool bRet = (bool)value;

			if (bRet)
				return (object)TypeToInsert;
			else
				return (object)TypeOver;
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
			throw new NotImplementedException("Conversion from string to bool is not implemented.");
		}
	}
}
