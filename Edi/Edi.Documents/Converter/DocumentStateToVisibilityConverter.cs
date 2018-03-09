namespace Edi.Core.Converters
{
	using System;
	using System.Windows;
	using System.Windows.Data;
	using System.Windows.Markup;
	using Interfaces.Enums;

	/// <summary>
	/// XAML mark up extension to convert a null value into a visibility value.
	/// </summary>
	[MarkupExtensionReturnType(typeof(IValueConverter))]
	[ValueConversion(typeof(DocumentState), typeof(Visibility))]
	public class DocumentStateToVisibilityConverter : MarkupExtension, IValueConverter
	{
		#region field
		private static DocumentStateToVisibilityConverter converter;
		#endregion field

		#region constructor

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
		    return converter ?? (converter = new DocumentStateToVisibilityConverter());
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
				return Visibility.Hidden;

			if ((value is DocumentState) == false)
				return Visibility.Hidden;

			DocumentState state = (DocumentState)value;

			switch (state)
			{
				case DocumentState.IsLoading:
					return Visibility.Hidden;

				case DocumentState.IsEditing:
					Console.WriteLine(@"Document is visble");
					return Visibility.Visible;
			}

			return Visibility.Hidden;
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
