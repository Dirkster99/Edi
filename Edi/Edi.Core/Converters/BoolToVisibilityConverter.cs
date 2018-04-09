namespace Edi.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    /// <summary>
    /// Source: http://stackoverflow.com/questions/534575/how-do-i-invert-booleantovisibilityconverter
    /// 
    /// Implements a Boolean to Visibility converter
    /// Use ConverterParameter=true to negate the visibility - boolean interpretation.
    /// </summary>
    [ValueConversion(typeof(Boolean), typeof(Visibility))]
	[MarkupExtensionReturnType(typeof(IValueConverter))]
	public sealed class BoolToVisibilityConverter : MarkupExtension, IValueConverter
	{
		#region fields
		private static BoolToVisibilityConverter _mConverter;
		#endregion fields

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
			return _mConverter ?? (_mConverter = new BoolToVisibilityConverter());
		}

		/// <summary>
		/// Converts a <seealso cref="Boolean"/> value
		/// into a <seealso cref="Visibility"/> value.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var flag = false;
			if (value is bool b)
			{
				flag = b;
			}

			if (parameter != null)
			{
				if (bool.Parse((string)parameter))
				{
					flag = !flag;
				}
			}
			return flag ? Visibility.Visible : Visibility.Collapsed;
		}

		/// <summary>
		/// Converts a <seealso cref="Visibility"/> value
		/// into a <seealso cref="Boolean"/> value.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var back = ((value is Visibility visibility) && (visibility == Visibility.Visible));

			if (parameter == null) return back;
			if ((bool)parameter)
			{
				back = !back;
			}
			return back;
		}
	}
}