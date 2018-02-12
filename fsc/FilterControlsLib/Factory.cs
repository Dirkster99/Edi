namespace FilterControlsLib
{
    using FilterControlsLib.Interfaces;
    using FilterControlsLib.ViewModels;

    /// <summary>
    /// Implements factory methods that creates library objects that are accessible
    /// through interfaces but are otherwise invisible for the outside world.
    /// </summary>
    public sealed class Factory
    {
        private Factory(){ }

        /// <summary>
        /// returns a new view model instance that can be used to drive a ComboBox
        /// filter control. A ComboBox filter control contains a list of filter options
        /// that can be selected by the user to adjust the list of items visible in a
        /// view.
        /// </summary>
        /// <returns></returns>
        public static IFilterComboBoxViewModel CreateFilterComboBoxViewModel()
        {
            return new FilterComboBoxViewModel();
        }

        /// <summary>
        /// returns one new view model ITEM instance that can be used to
        /// drive 1 entry in a filter control. A filter control contains a
        /// list of filter options that can be selected by the user to adjust
        /// the list of items visible in a view.
        /// </summary>
        /// <returns></returns>
        public static IFilterItemViewModel CreateFilterItem()
        {
            return new FilterItemViewModel();
        }
    }
}
