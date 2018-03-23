using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Edi.Core.Utillities
{
	/// <summary>
	/// http://elegantcode.com/2009/05/14/write-a-sortable-observablecollection-for-wpf/
	/// 
	/// sort ascending
  /// MySortableList.Sort(x => x.Name, ListSortDirection.Ascending);
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class SortableObservableCollection<T> : ObservableCollection<T>
	{
		public SortableObservableCollection(List<T> list)
			: base(list)
		{
		}

		public SortableObservableCollection(IEnumerable<T> collection)
			: base(collection)
		{
		}

		public void Sort<TKey>(Func<T, TKey> keySelector, ListSortDirection direction)
		{
			switch (direction)
			{
				case ListSortDirection.Ascending:
					{
						ApplySort(Items.OrderBy(keySelector));
						break;
					}
				case ListSortDirection.Descending:
					{
						ApplySort(Items.OrderByDescending(keySelector));
						break;
					}
			}
		}

		public void Sort<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer)
		{
			ApplySort(Items.OrderBy(keySelector, comparer));
		}

		private void ApplySort(IEnumerable<T> sortedItems)
		{
			var sortedItemsList = sortedItems.ToList();

			foreach (var item in sortedItemsList)
			{
				Move(IndexOf(item), sortedItemsList.IndexOf(item));
			}
		}
	}
}