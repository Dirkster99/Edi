namespace Edi.Apps.Converters
{
  using System;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Data;

    /// <summary>
    /// Converter to truncate middle of long string.
    /// This can be helpful if information at the end is important.
    /// It truncates a long document title with an ellipsis in the middle.
    ///
    /// https://github.com/tgjones/gemini/pull/294
    /// </summary>
    public class TruncateMiddleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            string result = value.ToString();
            int frontLength = 15;
            int backLength = 16;

            var lengths = parameter as int[];
            if (lengths != null && lengths.Length == 2)
            {
                frontLength = lengths[0];
                backLength = lengths[1];
            }

            if (result.Length > frontLength + 3 + backLength)
            {
                result = result.Substring(0, frontLength).Trim() + "..." + result.Substring(result.Length - backLength).Trim();
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
