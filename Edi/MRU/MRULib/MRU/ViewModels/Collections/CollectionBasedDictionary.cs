namespace MRULib.MRU.ViewModels.Collections
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;

    /// <summary>
    /// Source: https://github.com/gayaK/Gayak.ObservableDictionary
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    public partial class CollectionBasedDictionary<TKey, TValue> :
                                                    KeyedCollection<TKey, KeyValuePair<TKey, TValue>>,
                                                    IDictionary<TKey, TValue>,
#if !NET40
        IReadOnlyDictionary<TKey, TValue>,
#endif
        IDictionary
    {
        #region fields
        private KeyCollection _keys;
        private ValueCollection _values;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class Constructor
        /// </summary>
        public CollectionBasedDictionary() { }

        /// <summary>
        /// Class Constructor with comparer parameter.
        /// </summary>
        public CollectionBasedDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }

        /// <summary>
        /// Copy Class Constructor.
        /// </summary>
        public CollectionBasedDictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary, null) { }

        /// <summary>
        /// Copy Class Constructor with comparer parameter.
        /// </summary>
        public CollectionBasedDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(comparer)
        {
            foreach (var kvp in dictionary)
            {
                Add(kvp);
            }
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets or sets the element with the specified key.
        /// 
        /// Exceptions:
        ///   T:System.ArgumentNullException:
        ///     key is null.
        /// 
        ///   T:System.Collections.Generic.KeyNotFoundException:
        ///     The property is retrieved and key is not found.
        /// 
        ///   T:System.NotSupportedException:
        ///     The property is set and the System.Collections.Generic.IDictionary`2 is read-only.
        /// </summary>
        /// <param name="key">The key of the element to get or set.</param>
        /// <returns>The element with the specified key.</returns>
        public new TValue this[TKey key]
        {
            get => base[key].Value;
            set
            {
                var newKvp = new KeyValuePair<TKey, TValue>(key, value);
                if (TryGetValueInternal(key, out var oldKvp))
                {
                    SetItem(IndexOf(oldKvp), newKvp);
                }
                else
                {
                    Add(newKvp);
                }
            }
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// 
        /// Exceptions:
        ///   T:System.ArgumentNullException:
        ///     key is null.
        /// 
        ///   T:System.Collections.Generic.KeyNotFoundException:
        ///     The property is retrieved and key is not found.
        /// 
        ///   T:System.NotSupportedException:
        ///     The property is set and the System.Collections.Generic.IDictionary`2 is read-only.
        /// </summary>
        /// <param name="key">The key of the element to get or set.</param>
        /// <returns>The element with the specified key.</returns>
        object IDictionary.this[object key]
        {
            get => this[(TKey)key];
            set => this[(TKey)key] = (TValue)value;
        }

        /// <summary>
        /// Gets a collection of keys stored in this object.
        /// </summary>
        public KeyCollection Keys => _keys ?? (_keys = new KeyCollection(this));

        /// <summary>
        /// Gets a collection of keys stored in this object.
        /// 
        /// Implements the <seealso cref="ICollection{TKey}"/> interface member.
        /// </summary>
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => this.Keys;
#if !NET40

        /// <summary>
        /// Gets a collection of keys stored in this object.
        /// 
        /// implements ths <seealso cref="IEnumerable{TKey}"/> interface member.
        /// </summary>
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => this.Keys;
#endif
        /// <summary>
        /// Gets a collection of keys stored in this object.
        /// 
        /// Implements the <seealso cref="ICollection"/> interface member.
        /// </summary>
        ICollection IDictionary.Keys => this.Keys;

        /// <summary>
        /// Gets a collection of values stored in this object.
        /// </summary>
        public ValueCollection Values => _values ?? (_values = new ValueCollection(this));

        /// <summary>
        /// Gets a collection of keys stored in this object.
        /// 
        /// Implements the <seealso cref="ICollection{TValue}"/> interface member.
        /// </summary>
        ICollection<TValue> IDictionary<TKey, TValue>.Values => this.Values;
#if !NET40
        /// <summary>
        /// Gets a collection of keys stored in this object.
        /// 
        /// Implements the <seealso cref="IEnumerable{TValue}"/> interface member.
        /// </summary>
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => this.Values;
#endif
        /// <summary>
        /// Gets a collection of keys stored in this object.
        /// 
        /// Implements the <seealso cref="ICollection"/> interface member.
        /// </summary>
        ICollection IDictionary.Values => this.Values;

        /// <summary>
        /// Gets a value indicating whether the <seealso cref="IDictionary"/> has a fixed size.
        /// </summary>
        public bool IsFixedSize => false;

        /// <summary>
        /// Gets a value indicating whether objects in the <seealso cref="IDictionary"/>
        /// can only be read or also be edit.
        /// </summary>
        public bool IsReadOnly => false;
        #endregion properties

        #region methods
        /// <summary>
        /// Extracts the key from the specified element.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override TKey GetKeyForItem(KeyValuePair<TKey, TValue> item) => item.Key;

        /// <summary>
        /// Adds an object to the end of the <seealso cref="Collection{T}"/>.
        /// (Inherited from <seealso cref="Collection{T}"/>.)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value) => base.Add(new KeyValuePair<TKey, TValue>(key, value));

        /// <summary>
        /// Adds an object to the end of the <seealso cref="IDictionary"/>.
        /// (Inherited from <seealso cref="IDictionary"/>.)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void IDictionary.Add(object key, object value) => this.Add((TKey)key, (TValue)value);

        /// <summary>
        /// Removes the element with the specified key from the
        /// <seealso cref="IDictionary{TKey, TValue}"/> collection.
        /// </summary>
        /// <param name="key"></param>
        void IDictionary.Remove(object key) => this.Remove((TKey)key);

        /// <summary>
        /// Determines whether the collection contains an element with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key) => Dictionary?.ContainsKey(key) ?? false;

        /// <summary>
        /// Determines whether the collection contains an element with the specified key.
        /// (Implements the <seealso cref="IDictionary"/> memeber).
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool IDictionary.Contains(object key) => this.ContainsKey((TKey)key);

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key,
        /// if the key is found; otherwise, the default value for the type of the value parameter.
        /// This parameter is passed uninitialized.</param>
        /// <returns>true if the <seealso cref="IDictionary{TKey, TValue}"/> contains an element with
        /// the specified key; otherwise, false.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            var ret = TryGetValueInternal(key, out var kvp);
            value = kvp.Value;
            return ret;
        }

        /// <summary>
        /// Gets the <seealso cref="KeyValuePair{TKey, TValue}"/> associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the <seealso cref="KeyValuePair{TKey, TValue}"/>
        /// associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter.
        /// This parameter is passed uninitialized.</param>
        /// <returns>true if the <seealso cref="IDictionary{TKey, TValue}"/> contains an element with
        /// the specified key; otherwise, false.</returns>
        protected bool TryGetValueInternal(TKey key, out KeyValuePair<TKey, TValue> value)
        {
            if (Dictionary == null)
            {
                value = default(KeyValuePair<TKey, TValue>);
                return false;
            }
            return Dictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <seealso cref="IDictionary{TKey, TValue}"/> interface.
        /// </summary>
        /// <returns></returns>
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new DictionaryEnumerator(this.GetEnumerator());
        }
        #endregion methods
    }
}
