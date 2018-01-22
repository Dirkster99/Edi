namespace FileListView.ViewModels
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;

  /// <summary>
  /// Class implements a sorted observable collection.
  /// 
  /// Source: http://elegantcode.com/2009/05/14/write-a-sortable-observablecollection-for-wpf/
  /// </summary>
  public class SortableObservableCollection<T> : ObservableCollection<T>
  {
    #region constructor
    /// <summary>
    /// Standard class constructor
    /// </summary>
    public SortableObservableCollection()
      : base()
    {
    }

    /// <summary>
    /// Class constructor from IList interface. />
    /// </summary>
    /// <param name="list"></param>
    public SortableObservableCollection(List<T> list)
      : base(list)
    {
    }

    /// <summary>
    /// Class constructor from IEnumerable.
    /// </summary>
    /// <param name="collection"></param>
    public SortableObservableCollection(IEnumerable<T> collection)
      : base(collection)
    {
    }
    #endregion constructor

    #region methods
    /// <summary>
    /// Sort in descending or ascending order via lamda expression:
    /// 'MySortableList.Sort(x => x.Name, ListSortDirection.Ascending);'
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="keySelector"></param>
    /// <param name="direction"></param>
    public void Sort<TKey>(Func<T, TKey> keySelector, System.ComponentModel.ListSortDirection direction)
    {
      switch (direction)
      {
        case System.ComponentModel.ListSortDirection.Ascending:
        {
          this.ApplySort(Items.OrderBy(keySelector));
          break;
        }

        case System.ComponentModel.ListSortDirection.Descending:
        {
          this.ApplySort(Items.OrderByDescending(keySelector));
          break;
        }
      }
    }

    /// <summary>
    /// Sort in descending or ascending order^via keySelector.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="keySelector"></param>
    /// <param name="comparer"></param>
    public void Sort<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer)
    {
      this.ApplySort(Items.OrderBy(keySelector, comparer));
    }

    private void ApplySort(IEnumerable<T> sortedItems)
    {
      var sortedItemsList = sortedItems.ToList();

      foreach (var item in sortedItemsList)
      {
        this.Move(this.IndexOf(item), sortedItemsList.IndexOf(item));
      }
    }
    #endregion methods
  }
}
