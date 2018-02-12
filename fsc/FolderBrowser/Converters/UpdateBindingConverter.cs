namespace FolderBrowser.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Implements a Converter that can be used to relax the UI thread
    /// when frequent updates can cause the application to wait just for
    /// the UI to catch-up ...
    /// 
    /// Based on:
    /// Prevent a binding from updating too frequently | Josh Smith on WPF
    /// https://joshsmithonwpf.wordpress.com/2007/08/20/prevent-a-binding-from-updating-too-frequently/
    /// </summary>
    [ValueConversion(typeof(bool), typeof(object))]
    public class UpdateBindingConverter : IMultiValueConverter
    {
        /// <summary>
        /// The convert expects 2 bindings:
        /// 1> True/False value to determine whether updates should be shown to UI or not.
        /// 2> The binding that should be shown to you UI or not.
        /// 
        /// Sample Code:
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return Binding.DoNothing;

            var inputValues = values as object[];

            if (inputValues == null)
                return Binding.DoNothing;

            if (inputValues.Length != 3)
                return Binding.DoNothing;

            // IsLoaded Binding: Is the view loaded, yet ???
            if (inputValues[0] is bool == false)  // Updates that are not drawn should not be 
                return Binding.DoNothing;        // hidden from view
                                                //- since init can otherwise fail for pop-ups etc

            if (((bool)inputValues[0]) == false)
                return inputValues[2];           // Lets update the view since it isn't loaded yet

            if (inputValues[1] is bool == false)
                return Binding.DoNothing;

            var UpdateYesNo = (bool)inputValues[2];

            if (UpdateYesNo == false)      // These binding changes are not shown to the view
                return Binding.DoNothing;

            return inputValues[2];       // Return the ItemSource binding for updates since processing is done
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
