namespace MRULib.MRU.ViewModels.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Source: https://github.com/gayaK/Gayak.ObservableDictionary
    /// 
    /// Implements the internal <seealso cref="ValueCollection"/> class of the
    /// <seealso cref="CollectionBasedDictionary{TKey, TValue}"/> class.
    /// </summary>
    public partial class CollectionBasedDictionary<TKey, TValue>
    {
        /// <summary>
        /// Manages a collection of values.
        /// </summary>
        public class ValueCollection :
            ICollection<TValue>,
#if !NET40
            IReadOnlyList<TValue>,
#endif
            ICollection
        {
            #region fields
            private CollectionBasedDictionary<TKey, TValue> _source;
            #endregion fields

            #region constructors
            /// <summary>
            /// Class constructor.
            /// </summary>
            /// <param name="source"></param>
            public ValueCollection(CollectionBasedDictionary<TKey, TValue> source)
            {
                _source = source ?? throw new NullReferenceException(nameof(source));
            }
            #endregion constructors

            #region properties
            /// <summary>
            /// Gets the value item for a given index.
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public TValue this[int index] => _source[index].Value;

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
            /// This method is not suppported and will always throw <see cref="NotSupportedException"/>.
            /// </summary>
            /// <param name="item"></param>
            public void Add(TValue item) => throw new NotSupportedException();

            /// <summary>
            /// This method is not suppported and will always throw <see cref="NotSupportedException"/>.
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public bool Remove(TValue item) => throw new NotSupportedException();

            /// <summary>
            /// This method is not suppported and will always throw <see cref="NotSupportedException"/>.
            /// </summary>
            public void Clear() => throw new NotSupportedException();

            /// <summary>
            /// Determines whether the <seealso cref="IDictionary"/> object contains
            /// an element with the specified value.
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public bool Contains(TValue item) => _source
                .Select(x => x.Value)
                .Contains(item);

            /// <summary>
            /// Copies the elements of the ICollection to an <seealso cref="Array"/>,
            /// starting at a particular <seealso cref="Array"/> index.
            /// </summary>
            /// <param name="array"></param>
            /// <param name="arrayIndex"></param>
            public void CopyTo(TValue[] array, int arrayIndex) => _source
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
            void ICollection.CopyTo(Array array, int index) => this.CopyTo((TValue[])array, index);

            /// <summary>
            /// Returns an enumerator that iterates through a collection.(Inherited from IEnumerable.)
            /// </summary>
            /// <returns></returns>
            public IEnumerator<TValue> GetEnumerator() => _source.Select(x => x.Value).GetEnumerator();

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
