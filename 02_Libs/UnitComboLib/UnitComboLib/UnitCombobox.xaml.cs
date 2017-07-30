namespace UnitComboLib
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// This class implements a lookless combobox control with
    /// a unit drop-down selection that can be used to pre-select
    /// a list of useful combobox entries.
    /// </summary>
    [TemplatePartAttribute(Name = "PART_Popup", Type = typeof(Popup))]
    [LocalizabilityAttribute(LocalizationCategory.ComboBox)]
    [TemplatePartAttribute(Name = "PART_EditableTextBox", Type = typeof(TextBox))]
    [StyleTypedPropertyAttribute(Property = "ItemContainerStyle", StyleTargetType = typeof(ComboBoxItem))]
    public class UnitCombobox : ComboBox
    {
        #region constructor
        /// <summary>
        /// Static class constructor to register look-less <seealso cref="UnitCombobox"/> class
        /// control with the dependency property system.
        /// </summary>
        static UnitCombobox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UnitCombobox), new FrameworkPropertyMetadata(typeof(UnitCombobox)));
        }

        /// <summary>
        /// Standard public class constructor.
        /// </summary>
        public UnitCombobox() : base()
        {
        }
        #endregion constructor

////        #region Dependency Properties
////        public string MaxStringLen
////        {
////            get { return (string)GetValue(MaxStringLenProperty); }
////            set { SetValue(MaxStringLenProperty, value); }
////        }
////
////        // Using a DependencyProperty as the backing store for MaxStringLen.  This enables animation, styling, binding, etc...
////        public static readonly DependencyProperty MaxStringLenProperty =
////            DependencyProperty.Register("MaxStringLen",
////                typeof(string), typeof(UnitCombobox), new PropertyMetadata(0));
////        #endregion Dependency Properties
    }
}
