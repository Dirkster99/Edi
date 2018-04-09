namespace Edi.Apps.Converters
{
	using System;
	using System.Windows.Data;
	using Core.Interfaces;

    /// <summary>
    /// This converter is invoked when a new ActiveDocument (ActiveContent) is being selected and breught into view.
    /// Return the corresponding ViewModel or Binding.DoNothing (if the document type should not be selected and brought
    /// into view via ActiveDocument property in <seealso cref="ApplicationViewModel"/>.
    /// </summary>
    public class ActiveDocumentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is IFileBaseViewModel)
				return value;

			return Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is IFileBaseViewModel)
				return value;

			return Binding.DoNothing;
		}
	}
}
