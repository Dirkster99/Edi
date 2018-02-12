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
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This class is a helper for creating binary sorted lists
    /// </summary>
    internal class BinarySorter<TKey>
    {

        // ************************************************************************
        // Private Fields
        // ************************************************************************
        #region Public Methods

        /// <summary>
        /// Optional comparer used for sorting keys
        /// </summary>
        private IComparer<TKey> _comparer;

        #endregion Private Fields

        // ************************************************************************
        // Public Methods
        // ************************************************************************
        #region Public Methods

        /// <summary>
        /// Constructor that takes a comparer
        /// </summary>
        /// <param name="comparer"></param>
        public BinarySorter(IComparer<TKey> comparer = null)
        {
            _comparer = comparer;
        }

        /// <summary>
        /// Gets the position for a key to be inserted such that the sort order is maintained.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetInsertIndex(int count, TKey key, Func<int, TKey> indexToKey)
        {
            return BinarySearchForIndex(0, count - 1, key, indexToKey);
        }

        #endregion Public Methods

        // ************************************************************************
        // Private Methods
        // ************************************************************************
        #region Private Methods

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <returns></returns>
        private int Compare(TKey key1, TKey key2)
        {
            if (_comparer != null)
            {
                return _comparer.Compare(key1, key2);
            }
            else
            {
                return string.Compare(key1.ToString(), key2.ToString(), StringComparison.InvariantCultureIgnoreCase);
            }
        }

        /// <summary>
        /// Searches for the index of the insertion point for the key passed in such that
        /// the sort order is maintained. Implemented as a non-recursive method.
        /// </summary>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private int BinarySearchForIndex(int low, int high, TKey key, Func<int, TKey> indexToKey)
        {
            while (high >= low)
            {

                // Calculate the mid point and determine if the key passed in
                // should be inserted at this point, below it, or above it.
                int mid = low + ((high - low) >> 1);
                int result = Compare(indexToKey(mid), key);

                // Return the current position, or change the search bounds
                // to be above or below the mid point depending on the result.
                if (result == 0)
                    return mid;
                else if (result < 0)
                    low = mid + 1;
                else
                    high = mid - 1;
            }
            return low;
        }

        #endregion Private Methods
    }
}
