namespace FilterControlsLib.Interfaces
{
    using System;
    using System.Collections.Generic;
    using FileSystemModels.Events;

    /// <summary>
    /// Defines members and properties of a viewmodel
    /// for a combo box like control that lists a list
    /// of regular filter expressions to choose from.
    /// </summary>
    public interface IFilterComboBoxViewModel
    {
        /// <summary>
        /// Event is fired whenever the path in the text portion
        /// of the combobox is changed. Client applications should
        /// listen to this event if they want to be informed when
        /// the user selects another item.
        /// </summary>
        event EventHandler<FilterChangedEventArgs> OnFilterChanged;

        #region properties
        /// <summary>
        /// Gets the list of current filter items in filter view,
        /// eg: "BAT | *.bat; *.cmd", ... ,"XML | *.xml; *.xsd"
        /// </summary>
        IEnumerable<IFilterItemViewModel> CurrentItems { get; }

        /// <summary>
        /// Gets the selected filter item.
        /// </summary>
        IFilterItemViewModel SelectedItem { get; }

        /// <summary>
        /// Get/sets viewmodel data pointing at the path
        /// of the currently selected folder.
        /// </summary>
        string CurrentFilter { get; }

        /// <summary>
        /// Gets an informative text for the currently selected filter.
        /// </summary>
        string CurrentFilterToolTip { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Clears the current list of filters and resets everything
        /// that is file filter related to null.
        /// </summary>
        void ClearFilter();

        /// <summary>
        /// Add a new filter argument to the list of filters and
        /// select this filter if <paramref name="bSelectNewFilter"/>
        /// indicates it.
        /// </summary>
        /// <param name="filterString"></param>
        /// <param name="bSelectNewFilter"></param>
        void AddFilter(string filterString, bool bSelectNewFilter = false);

        /// <summary>
        /// Add a new filter argument to the list of filters and
        /// select this filter if <paramref name="bSelectNewFilter"/>
        /// indicates it.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="filterString"></param>
        /// <param name="bSelectNewFilter"></param>
        void AddFilter(string name, string filterString, bool bSelectNewFilter = false);

        /// <summary>
        /// Attempts to find a filter in the list of current filters
        /// based on the current display name and actual filter string (eg '*.tex').
        /// </summary>
        /// <param name="name"></param>
        /// <param name="filterString"></param>
        /// <returns>A collection of filters found or null.</returns>
        IEnumerable<IFilterItemViewModel> FindFilter(string name, string filterString);

        /// <summary>
        /// Applies the current display name and actual filter string (eg '*.tex')
        /// as current file filter on the list of files being displayed.
        /// </summary>
        /// <param name="filterDisplayName"></param>
        /// <param name="filterText"></param>
        void SetCurrentFilter(string filterDisplayName, string filterText);
        #endregion methods
    }
}