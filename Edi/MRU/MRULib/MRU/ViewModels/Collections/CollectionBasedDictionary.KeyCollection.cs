namespace MRULib.MRU.ViewModels.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Source: https://github.com/gayaK/Gayak.ObservableDictionary
    /// 
    /// Implements the <seealso cref="KeyCollection"/> class inside
    /// the <seealso cref="CollectionBasedDictionary{TKey, TValue}"/> class.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public partial class CollectionBasedDictionary<TKey, TValue>
    {
        /// <summary>
        /// Manages a collection of keys.
        /// </summary>
        public class KeyCollection : ICollection<TKey>,
#if !NET40
            IReadOnlyList<TKey>,
#endif
            ICollection
        {
            #region fields
            private CollectionBasedDictionary<TKey, TValue> _source;
            #endregion fields

            #region constructors
            /// <summary>
            /// Class constructor
            /// </summary>
            /// <param name="source"></param>
            public KeyCollection(CollectionBasedDictionary<TKey, TValue> source)
            {
                _source = source ?? throw new NullReferenceException(nameof(source));
            }
            #endregion constructors

            #region properties
            /// <summary>
            /// Gets the key item for a given index.
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public TKey this[int index] => _source[index].Key;

            /// <summary>
            /// Gets the count of items currently stored in this collection.
            /// </summary>
            public int Count => _source.Count;

            /// <summary>
            /// Gets whether the collection is ready only (cannot be modified) or not.
            /// </summary>
            public bool IsReadOnly => true;

            /// <summary>
            /// Gets an object that can be used to synchronize access to
            /// the <seealso cref="ICollection"/>.
            /// </summary>
            public object SyncRoot { get; } = new object();

            /// <summary>
            /// Gets a value indicating whether access to the ICollection is synchronized (thread safe).
            /// </summary>
            public bool IsSynchronized => false;
            #endregion properties

            #region methods
            /// <summary>
            /// Adds an element with the provided key and value to the <seealso cref="IDictionary"/> object.
            /// </summary>
            /// <param name="item"></param>
            public void Add(TKey item) => throw new NotSupportedException();

            /// <summary>
            /// Removes the element with the specified key from the <seealso cref="IDictionary"/> object.
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public bool Remove(TKey item) => throw new NotSupportedException();

            /// <summary>
            /// Removes all elements from the <seealso cref="IDictionary"/> object.
            /// </summary>
            public void Clear() => throw new NotSupportedException();

            /// <summary>
            /// Determines whether the <seealso cref="IDictionary"/> object contains
            /// an element with the specified key.
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public bool Contains(TKey item) => _source.ContainsKey(item);

            /// <summary>
            /// Copies the elements of the ICollection to an <seealso cref="Array"/>,
            /// starting at a particular <seealso cref="Array"/> index.
            /// </summary>
            /// <param name="array"></param>
            /// <param name="arrayIndex"></param>
            public void CopyTo(TKey[] array, int arrayIndex) => _source
                .Select(x => x.Value)
                .ToArray()
                .CopyTo(array, arrayIndex);

            /// <summary>
            /// Copies the elements of the ICollection to an <seealso cref="Array"/>,
            /// starting at a particular <seealso cref="Array"/> index.
            /// 
            /// Implements the <seealso cref="ICollection"/> interface member.
            /// </summary>
            /// <param name="array"></param>
            /// <param name="index"></param>
            void ICollection.CopyTo(Array array, int index) => this.CopyTo((TKey[])array, index);

            /// <summary>
            /// Returns an enumerator that iterates through a collection.(Inherited from IEnumerable.)
            /// </summary>
            /// <returns></returns>
            public IEnumerator<TKey> GetEnumerator() => _source.Select(x => x.Key).GetEnumerator();

            /// <summary>
            /// Returns an enumerator that iterates through a collection.(Inherited from IEnumerable.)
            /// 
            /// Implements the <seealso cref="IEnumerable"/> interface member.
            /// </summary>
            /// <returns></returns>
            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
            #endregion methods
        }
    }
}

