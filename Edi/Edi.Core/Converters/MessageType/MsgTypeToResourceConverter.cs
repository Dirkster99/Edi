namespace Edi.Core.Converters.MessageType
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    [ValueConversion(typeof(Msg.MsgCategory), typeof(ImageSource))]
	public class MsgTypeToResourceConverter : IValueConverter
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
			// Check input parameter types
			if (value == null)
				return Binding.DoNothing;

			if (!(value is Msg.MsgCategory))
				throw new ArgumentException("Invalid argument. Expected argument: ViewModel.Base.Msg.MsgType");

			if (targetType != typeof(ImageSource))
				throw new ArgumentException("Invalid return type. Expected return type: System.Windows.Media.ImageSource");

			string resourceUri = "Images/MessageIcons/Unknown.png";
			switch ((Msg.MsgCategory) value)
			{
				case Msg.MsgCategory.Information:
					break;
				case Msg.MsgCategory.Error:
					resourceUri = "Images/MessageIcons/Error.png";
					break;
				case Msg.MsgCategory.Warning:
					resourceUri = "Images/MessageIcons/Warning.png";
					break;
				case Msg.MsgCategory.InternalError:
					resourceUri = "Images/MessageIcons/InternalError.png";
					break;
				default:
					resourceUri = "Images/MessageIcons/Unknown.png";
					break;
			}

			BitmapImage icon = new BitmapImage();

			try
			{
				icon.BeginInit();
				icon.UriSource = new Uri(string.Format(CultureInfo.InvariantCulture, "pack://application:,,,/{0};component/{1}",
					"Themes", resourceUri));
				icon.EndInit();
			}
			catch
			{
				return Binding.DoNothing;
			}

			return icon;
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
			if (value is Visibility val && targetType == typeof(bool))
			{
				return val == Visibility.Visible;
			}

			throw new ArgumentException("Invalid argument/return type. Expected argument: Visibility and return type: bool");
		}

		#endregion
	}
}