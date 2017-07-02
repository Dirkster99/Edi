namespace Edi.Core.Converters.MessageType
{
	using System;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Data;
	using System.Windows.Media.Imaging;

	[ValueConversion(typeof(Edi.Core.Msg.MsgCategory), typeof(System.Windows.Media.ImageSource))]
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
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			// Check input parameter types
			if (value == null)
				return Binding.DoNothing;

			if (!(value is Edi.Core.Msg.MsgCategory))
				throw new ArgumentException("Invalid argument. Expected argument: ViewModel.Base.Msg.MsgType");

			if (targetType != typeof(System.Windows.Media.ImageSource))
				throw new ArgumentException("Invalid return type. Expected return type: System.Windows.Media.ImageSource");

			string resourceUri = "Images/MessageIcons/Unknown.png";
			switch ((Edi.Core.Msg.MsgCategory)value)
			{
				case Edi.Core.Msg.MsgCategory.Information:
					break;
				case Edi.Core.Msg.MsgCategory.Error:
					resourceUri = "Images/MessageIcons/Error.png";
					break;
				case Edi.Core.Msg.MsgCategory.Warning:
					resourceUri = "Images/MessageIcons/Warning.png";
					break;
				case Edi.Core.Msg.MsgCategory.InternalError:
					resourceUri = "Images/MessageIcons/InternalError.png";
					break;
				case Edi.Core.Msg.MsgCategory.Unknown:
				default:
					resourceUri = "Images/MessageIcons/Unknown.png";
					break;
			}

			BitmapImage icon = new BitmapImage();

			try
			{
				icon.BeginInit();
				icon.UriSource = new Uri(string.Format(CultureInfo.InvariantCulture, "pack://application:,,,/{0};component/{1}", "Themes", resourceUri));
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
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is Visibility && targetType == typeof(bool))
			{
				Visibility val = (Visibility)value;
				if (val == Visibility.Visible)
					return true;
				else
					return false;
			}

			throw new ArgumentException("Invalid argument/return type. Expected argument: Visibility and return type: bool");
		}
		#endregion
	}
}
