namespace UnitComboLib.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using UnitComboLib.Models;

    public interface IUnitViewModel
    {
        #region properties
        #region IDataErrorInfo Interface
        /// <summary>
        /// Source: http://joshsmithonwpf.wordpress.com/2008/11/14/using-a-viewmodel-to-provide-meaningful-validation-error-messages/
        /// </summary>
        string Error { get; }

        /// <summary>
        /// Standard property that is part of the <seealso cref="IDataErrorInfo"/> interface.
        /// 
        /// Evaluetes whether StringValue parameter represents a value within the expected range
        /// and sets a corresponding errormessage in the ValueTip property if not.
        /// 
        /// Source: http://joshsmithonwpf.wordpress.com/2008/11/14/using-a-viewmodel-to-provide-meaningful-validation-error-messages/
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        string this[string propertyName] { get; }
        #endregion IDataErrorInfo Interface

        /// <summary>
        /// Get/sets a string that represents a convinient maximum length in
        /// characters to measure the width for the displaying control.
        /// </summary>
        string MaxStringLengthValue { get; set; }

        /// <summary>
        /// Get the legal maximum value in dependency of the current unit.
        /// </summary>
        double MaxValue { get; }

        /// <summary>
        /// Get the legal minimum value in dependency of the current unit.
        /// </summary>
        double MinValue { get; }

        /// <summary>
        /// Currently selected value in screen points. This property is needed because the run-time system
        /// cannot work with percent values directly. Therefore, this property always ensures a correct
        /// font size no matter what the user selected in percent.
        /// </summary>
        int ScreenPoints { get; set; }

        /// <summary>
        /// Get/set currently selected unit key, converter, and default value list.
        /// </summary>
        ListItem SelectedItem { get; set; }

        /// <summary>
        /// Get command to be executed when the user has selected a unit
        /// (eg. 'Km' is currently used but user selected 'm' to be used next)
        /// </summary>
        ICommand SetSelectedItemCommand { get; }

        /// <summary>
        /// String representation of the double value that
        /// represents the unit scaled value in this object.
        /// </summary>
        string StringValue { get; set; }

        /// <summary>
        /// Get list of units, their default value lists, itemkeys etc.
        /// </summary>
        ObservableCollection<ListItem> UnitList { get; }

        /// <summary>
        /// Get double value represented in unit as indicated by SelectedItem.Key.
        /// </summary>
        double Value { get; set; }

        /// <summary>
        /// Get a string that indicates the format of the
        /// expected input or a an error if the current input is not valid.
        /// </summary>
        string ValueTip { get; }
        #endregion properties
    }
}