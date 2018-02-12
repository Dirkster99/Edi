// Authored by: John Stewien
// Year: 2011
// Company: Swordfish Computing
// License: 
// The Code Project Open License http://www.codeproject.com/info/cpol10.aspx
// Originally published at:
// http://www.codeproject.com/Articles/208361/Concurrent-Observable-Collection-Dictionary-and-So
// Last Revised: September 2012

namespace FsCore.Collections
{
    using System.Collections.Generic;

    /// <summary>
    /// This class provides a sorted collection that can be bound to
    /// a WPF control.
    /// </summary>
    public class ObservableSortedDictionary<TKey, TValue> : ObservableDictionary<TKey, TValue>
    {

        // ************************************************************************
        // Private Fields
        // ************************************************************************
        #region Private Fields

        /// <summary>
        /// Utitlity object that is used for sorting the list
        /// </summary>
        private BinarySorter<TKey> _sorter;

        #endregion Private Fields

        // ************************************************************************
        // Public Methods
        // ************************************************************************
        #region Public Methods

        /// <summary>
        /// Constructor with an optional IComparer<TKey> parameter.
        /// </summary>
        /// <param name="comparer">Comparer used to sort the keys.</param>
        public ObservableSortedDictionary(IComparer<TKey> comparer = null)
        {
            _sorter = new BinarySorter<TKey>(comparer);
        }

        /// <summary>
        /// Adds an element with the provided key and value to the IDictionary<TKey, TValue>.
        /// </summary>
        /// <param name="key">
        /// The object to use as the key of the element to add.
        /// </param>
        /// <param name="value">
        /// The object to use as the value of the element to add.
        /// </param>
        public override void Add(TKey key, TValue value)
        {
            int index = _sorter.GetInsertIndex(_masterList.Count, key, delegate (int mid)
            {
                return _masterList[mid].Key;
            });

            if (index >= _masterList.Count)
            {
                base.Add(key, value);
            }
            else
            {
                TKey listKey = _masterList[index].Key;
                DoubleLinkListIndexNode next = _keyToIndex[listKey];
                DoubleLinkListIndexNode newNode = new DoubleLinkListIndexNode(next.Previous, next);
                _keyToIndex[key] = newNode;
                _masterList.Insert(index, new KeyValuePair<TKey, TValue>(key, value));
            }
        }

        #endregion Pulbic Methods
    }
}
