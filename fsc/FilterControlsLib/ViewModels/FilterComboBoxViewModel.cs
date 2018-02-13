namespace FilterControlsLib.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;
    using FilterControlsLib.Interfaces;
    using FileSystemModels.Events;
    using FileSystemModels.ViewModels.Base;
    using FilterControlsLib.Collections;

    /// <summary>
    /// Class implements a viewmodel for a combo box like control that
    /// lists a list of regular filter expressions to choose from.
    /// </summary>
    internal class FilterComboBoxViewModel : Base.ViewModelBase, IFilterComboBoxViewModel
    {
        #region fields
        private string _CurrentFilter = string.Empty;
        private IFilterItemViewModel _SelectedItem = null;

        private RelayCommand<object> _SelectionChanged = null;

        private readonly SortableObservableCollection<IFilterItemViewModel> _CurrentItems = null;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public FilterComboBoxViewModel()
        {
            _CurrentItems = new SortableObservableCollection<IFilterItemViewModel>();
        }
        #endregion constructor

        #region Events
        /// <summary>
        /// Event is fired whenever the path in the text portion of the combobox is changed.
        /// </summary>
        public event EventHandler<FilterChangedEventArgs> OnFilterChanged;
        #endregion

        #region properties
        /// <summary>
        /// Gets the list of current filter items in filter view,
        /// eg: "BAT | *.bat; *.cmd", ... ,"XML | *.xml; *.xsd"
        /// </summary>
        public IEnumerable<IFilterItemViewModel> CurrentItems
        {
            get
            {
                return _CurrentItems;
            }
        }

        /// <summary>
        /// Gets the selected filter item.
        /// </summary>
        public IFilterItemViewModel SelectedItem
        {
            get
            {
                return this._SelectedItem;
            }

            protected set
            {
                if (this._SelectedItem != value)
                {
                    this._SelectedItem = value;
                    this.RaisePropertyChanged(() => this.SelectedItem);

                    this.RaisePropertyChanged(() => this.CurrentFilter);
                    this.RaisePropertyChanged(() => this.CurrentFilterToolTip);
                }
            }
        }

        /// <summary>
        /// Get/sets viewmodel data pointing at the path
        /// of the currently selected folder.
        /// </summary>
        public string CurrentFilter
        {
            get
            {
                if (_SelectedItem != null)
                    return _SelectedItem.FilterText;

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets an informative text for the currently selected filter.
        /// </summary>
        public string CurrentFilterToolTip
        {
            get
            {
                if (this._SelectedItem != null)
                {
                    if (string.IsNullOrEmpty(this._SelectedItem.FilterText) == false)
                        return string.Format("{0} ({1})\n{2}", this._SelectedItem.FilterDisplayName,
                                                               this._SelectedItem.FilterText,
                                                               FileSystemModels.Local.Strings.SelectFilterCommand_TT);
                }

                if (string.IsNullOrEmpty(this._CurrentFilter) == false)
                    return string.Format("{0}\n{1}", this._CurrentFilter,
                                                      FileSystemModels.Local.Strings.SelectFilterCommand_TT);
                else
                    return FileSystemModels.Local.Strings.SelectFilterCommand_TT;
            }
        }

        #region commands
        /// <summary>
        /// Command is invoked when the combobox view tells the viewmodel
        /// that the current path selection has changed (via selection changed
        /// event or keyup events).
        /// 
        /// The parameter can be an array of objects
        /// containing objects of the <seealso cref="FilterItemViewModel"/> type or
        /// p can also be string.
        /// 
        /// Each parameter item that adheres to the above types results in
        /// a OnFilterChanged event being fired with the folder path
        /// as parameter.
        /// </summary>
        public ICommand SelectionChanged
        {
            get
            {
                if (this._SelectionChanged == null)
                    this._SelectionChanged = new RelayCommand<object>(
                        (p) => this.SelectionChanged_Executed(p));


                return this._SelectionChanged;
            }
        }
        #endregion commands
        #endregion properties

        #region methods
        /// <summary>
        /// Clears the current list of filters and resets everything
        /// that is file filter related to null.
        /// </summary>
        public void ClearFilter()
        {
            _CurrentItems.Clear();

            this.SelectedItem = null;
        }

        /// <summary>
        /// Add a new filter argument to the list of filters and
        /// select this filter if <paramref name="bSelectNewFilter"/>
        /// indicates it.
        /// </summary>
        /// <param name="filterString"></param>
        /// <param name="bSelectNewFilter"></param>
        public void AddFilter(string filterString,
                              bool bSelectNewFilter = false)
        {
            var item = new FilterItemViewModel(filterString);
            _CurrentItems.Add(item);
            _CurrentItems.Sort(i => i.FilterDisplayName, ListSortDirection.Ascending);

            if (bSelectNewFilter == true)
                this.SelectedItem = item;
        }

        /// <summary>
        /// Add a new filter argument to the list of filters and
        /// select this filter if <paramref name="bSelectNewFilter"/>
        /// indicates it.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="filterString"></param>
        /// <param name="bSelectNewFilter"></param>
        public void AddFilter(string name, string filterString,
                              bool bSelectNewFilter = false)
        {
            var item = new FilterItemViewModel(name, filterString);
            _CurrentItems.Add(item);
            _CurrentItems.Sort(i => i.FilterDisplayName, ListSortDirection.Ascending);

            if (bSelectNewFilter == true)
                this.SelectedItem = item;
        }

        /// <summary>
        /// Attempts to find a filter in the list of current filters
        /// based on the current display name and actual filter string (eg '*.tex').
        /// </summary>
        /// <param name="name"></param>
        /// <param name="filterString"></param>
        /// <returns>A collection of filters found or null.</returns>
        public IEnumerable<IFilterItemViewModel> FindFilter(string name, string filterString)
        {
            if (this.CurrentItems == null)
                return null;

            try
            {
                var vm = from item in this.CurrentItems
                         where
                         (string.Compare(item.FilterDisplayName, name, true) == 0 &&
                          string.Compare(item.FilterText, filterString, true) == 0)
                         select item;

                return vm;
            }
            catch
            {
            }

            return null;
        }

        /// <summary>
        /// Applies the current display name and actual filter string (eg '*.tex')
        /// as current file filter on the list of files being displayed.
        /// </summary>
        /// <param name="filterDisplayName"></param>
        /// <param name="filterText"></param>
        public void SetCurrentFilter(string filterDisplayName, string filterText)
        {
            // Attempt to find this filter item
            var similarFilter = this.FindFilter(filterDisplayName, filterText);

            if (similarFilter != null)
            {
                try
                {
                    var firstElement = similarFilter.First();

                    if (firstElement != null)
                    {
                        // we found this filter item aleady -> make this the currently selected item
                        this.SelectedItem = firstElement;
                        return;
                    }
                }
                catch
                {
                }
            }

            // Add this filter into the collection and mark it as currently selected item
            this.AddFilter(filterDisplayName, filterText, true);
        }

        /// <summary>
        /// Method executes when the SelectionChanged command is invoked.
        /// The parameter <paramref name="p"/> can be an array of objects
        /// containing objects of the <seealso cref="FSItemViewModel"/> type or
        /// p can also be string.
        /// 
        /// Each parameter item that adheres to the above types results in
        /// a OnFilterChanged event being fired with the folder path
        /// as parameter.
        /// </summary>
        /// <param name="p"></param>
        private void SelectionChanged_Executed(object p)
        {
            if (p == null)
                return;

            // Check if the given parameter is an array of objects and process it...
            object[] paramObjects = p as object[];
            if (paramObjects != null)
            {
                for (int i = 0; i < paramObjects.Length; i++)
                {
                    var item = paramObjects[i] as FilterItemViewModel;

                    if (item != null)
                    {
                        SelectedItem = item;

                        if (this.OnFilterChanged != null)
                            this.OnFilterChanged(this, new FilterChangedEventArgs() { FilterText = item.FilterText });
                    }
                }
            }

            // Check if the given parameter is a string and fire a corresponding event if so...
            var paramString = p as string;
            if (paramString != null)
            {
                // Search for filterstring in items collection and select it or
                // create a new item and select it
                IFilterItemViewModel selectedItem = null;
                foreach (var item in CurrentItems)
                {
                    if (item.FilterText == paramString)
                    {
                        selectedItem = item;
                        break;
                    }
                }

                // Item not found -> lets create a new one then ...
                if (selectedItem != null)
                    SelectedItem = selectedItem;
                else
                    AddFilter(paramString, true);

                if (this.OnFilterChanged != null)
                    this.OnFilterChanged(this, new FilterChangedEventArgs() { FilterText = paramString });
            }
        }

        /// <summary>
        /// Determine whether a given path is an exeisting directory or not.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool IsPathDirectory(string path)
        {
            if (string.IsNullOrEmpty(path) == true)
                return false;

            bool isPath = false;

            try
            {
                isPath = System.IO.Directory.Exists(path);
            }
            catch
            {
            }

            return isPath;
        }
        #endregion methods
    }
}
