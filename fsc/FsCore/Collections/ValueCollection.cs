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
    /// Provides an immutable value collection as an interface to the keys
    /// stored in an observable dictionary
    /// </summary>
    public class ValueCollection<TKey, TValue> : ImmutableCollectionBase<TValue>
    {

        // ************************************************************************
        // Private Fields
        // ************************************************************************
        #region Private Fields

        /// <summary>
        /// The source dictionary
        /// </summary>
        private ObservableDictionary<TKey, TValue> _dictionary;

        #endregion Private Fields

        // ************************************************************************
        // Constructors
        // ************************************************************************
        #region Constructors

        /// <summary>
        /// Constructor that takes the source dictionary as a parameter
        /// </summary>
        public ValueCollection(ObservableDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        #endregion Constructors

        // ************************************************************************
        // ImmutableCollectionBase Implementation
        // ************************************************************************
        #region ImmutableCollectionBase Implementation

        /// <summary>
        /// Gets the number of elements contained in the collection<T>.
        /// </summary>
        public override int Count
        {
            get
            {
                return _dictionary.Count;
            }
        }

        /// </summary>
        /// <param name="item">The object to locate</param>
        /// <returns>true if item is found otherwise false</returns>
        public override bool Contains(TValue item)
        {
            foreach (KeyValuePair<TKey, TValue> pair in _dictionary)
            {
                if (item.Equals(pair.Value))
                    return true;
            }
            return false;
        }

        /// <summary>
        //  Copies the elements of the collection to an array, starting
        /// at a particular index.
        /// </summary>
        public override void CopyTo(TValue[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw (new System.ArgumentNullException());
            }

            foreach (KeyValuePair<TKey, TValue> pair in _dictionary)
            {
                array[arrayIndex] = pair.Value;
                ++arrayIndex;
            }
        }

        /// <summary>
        /// Gets the enumerator for the collection
        /// </summary>
        public override IEnumerator<TValue> GetEnumerator()
        {
            foreach (KeyValuePair<TKey, TValue> pair in _dictionary)
            {
                yield return pair.Value;
            }
        }
        #endregion ImmutableCollectionBase Implementation
    }
}
