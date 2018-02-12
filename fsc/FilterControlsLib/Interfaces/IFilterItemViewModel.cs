namespace FilterControlsLib.Interfaces
{
    /// <summary>
    /// Defines the properties and methods of a view model
    /// for a filter item displayed in a list of filters.
    /// </summary>
    public interface IFilterItemViewModel
    {
        /// <summary>
        /// Gets the regular expression based filter string eg: '*.exe'.
        /// </summary>
        string FilterText { get; set; }

        /// <summary>
        /// Gets the name for this filter
        /// (human readable for display in tool tip or label).
        /// </summary>
        string FilterDisplayName { get; set; }
    }
}