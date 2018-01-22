namespace FileListView.ViewModels
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows.Input;
  using FileListView.Command;
  using FileSystemModels.Events;

  /// <summary>
  /// Class implements a viewmodel for a combo box like control that
  /// lists a list of regular filter expressions to choose from.
  /// </summary>
  public class FilterComboBoxViewModel : Base.ViewModelBase
  {
    #region fields
    private string mCurrentFilter = string.Empty;
    private FilterItemViewModel mSelectedItem = null;

    private RelayCommand<object> mSelectionChanged = null;    
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    public FilterComboBoxViewModel()
    {
      this.CurrentItems = new SortableObservableCollection<FilterItemViewModel>();
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
    public SortableObservableCollection<FilterItemViewModel> CurrentItems { get; private set; }

    /// <summary>
    /// Gets the selected filter item.
    /// </summary>
    public FilterItemViewModel SelectedItem
    {
      get
      {
        return this.mSelectedItem;
      }

      set
      {
        if (this.mSelectedItem != value)
        {
          this.mSelectedItem = value;
          this.RaisePropertyChanged(() => this.SelectedItem);
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
        return this.mCurrentFilter;
      }

      set
      {
        if (this.mCurrentFilter != value)
        {
          this.mCurrentFilter = value;
          this.SelectionChanged_Executed(value);
          this.RaisePropertyChanged(() => this.CurrentFilter);
          this.RaisePropertyChanged(() => this.CurrentFilterToolTip);
        }
      }
    }

    /// <summary>
    /// Gets an informative text for the currently selected filter.
    /// </summary>
    public string CurrentFilterToolTip
    {
      get
      {
        if (this.mSelectedItem != null)
        {
          if (string.IsNullOrEmpty(this.mSelectedItem.FilterText) == false)
            return string.Format("{0} ({1})\n{2}", this.mSelectedItem.FilterDisplayName, 
                                                   this.mSelectedItem.FilterText,
                                                   FileSystemModels.Local.Strings.SelectFilterCommand_TT);
        }

        if (string.IsNullOrEmpty(this.mCurrentFilter) == false)
          return string.Format("{0}\n{1}", this.mCurrentFilter,
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
        if (this.mSelectionChanged == null)
          this.mSelectionChanged = new RelayCommand<object>((p) => this.SelectionChanged_Executed(p));

        return this.mSelectionChanged;
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
      if (this.CurrentItems != null)
        this.CurrentItems.Clear();

      this.SelectedItem = null;
      this.CurrentFilter = null;
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
      this.CurrentItems.Add(item);
      this.CurrentItems.Sort(i => i.FilterDisplayName, ListSortDirection.Ascending);

      if (bSelectNewFilter == true)
      {
        this.SelectedItem = item;
        this.CurrentFilter = item.FilterText;
      }
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
      this.CurrentItems.Add(item);
      this.CurrentItems.Sort(i => i.FilterDisplayName, ListSortDirection.Ascending);

      if (bSelectNewFilter == true)
      {
        this.SelectedItem = item;
        this.CurrentFilter = item.FilterText;
      }
    }

    /// <summary>
    /// Attempts to find a filter in the list of current filters
    /// based on the current display name and actual filter string (eg '*.tex').
    /// </summary>
    /// <param name="name"></param>
    /// <param name="filterString"></param>
    /// <returns>A collection of filters found or null.</returns>
    public IEnumerable<FilterItemViewModel> FindFilter(string name, string filterString)
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
        var firstElement = similarFilter.First();

        if (firstElement != null)
        {
          // we found this filter item aleady -> make this the currently selected item
          this.CurrentFilter = firstElement.FilterText;
          this.SelectedItem = firstElement;

          return;
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
            if (this.OnFilterChanged != null)
              this.OnFilterChanged(this, new FilterChangedEventArgs() { FilterText = item.FilterText });
          }
        }
      }

      // Check if the given parameter is a string and fire a corresponding event if so...
      var paramString = p as string;
      if (paramString != null)
      {
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
