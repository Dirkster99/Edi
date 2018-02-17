namespace Edi.Documents.Converter
{
	using System;
	using System.Windows.Data;
	using System.Windows.Markup;
	using ViewModels.EdiDoc;

	/// <summary>
	/// XAML mark up extension to convert a null value into a visibility value.
	/// This class is used to decide whether text document related controls,
	/// such as, highlighting patterns is shown or not.
	/// 
	/// The converter returns <seealso cref="System.Windows.Visibility.Visible"/>
	/// if the currently Active document is a text file that can be highlighted
	/// and <seealso cref="System.Windows.Visibility.Collapsed"/> otherwise.
	/// </summary>
	[MarkupExtensionReturnType(typeof(IValueConverter))]
	public class ActiveDocumentToVisibilityConverter : MarkupExtension, IValueConverter
	{
		#region field
		private static ActiveDocumentToVisibilityConverter converter;
		#endregion field

		#region constructor
		/// <summary>
		/// Standard Constructor
		/// </summary>
		public ActiveDocumentToVisibilityConverter()
		{
		}
		#endregion constructor

		#region MarkupExtension
		/// <summary>
		/// When implemented in a derived class, returns an object that is provided
		/// as the value of the target property for this markup extension.
		/// 
		/// When a XAML processor processes a type node and member value that is a markup extension,
		/// it invokes the ProvideValue method of that markup extension and writes the mResult into the
		/// object graph or serialization stream. The XAML object writer passes service context to each
		/// such implementation through the serviceProvider parameter.
		/// </summary>
		/// <param name="serviceProvider"></param>
		/// <returns></returns>
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (converter == null)
			{
				converter = new ActiveDocumentToVisibilityConverter();
			}

			return converter;
		}
		#endregion MarkupExtension

		#region IValueConverter
		/// <summary>
		/// Null to visibility conversion method
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

			if (value is EdiViewModel)
				return System.Windows.Visibility.Visible;

			return System.Windows.Visibility.Collapsed;
		}

		/// <summary>
		/// Visibility to Null conversion method (is not implemented)
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return Binding.DoNothing;
		}
		#endregion IValueConverter
	}
}
