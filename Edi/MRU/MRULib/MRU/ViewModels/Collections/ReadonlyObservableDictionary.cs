namespace MRULib.MRU.ViewModels.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;

    /// <summary>
    /// Source: https://github.com/gayaK/Gayak.ObservableDictionary
    /// 
    /// Implements a <see cref="ReadonlyObservableDictionary{TKey, TValue}"/> class
    /// which can be used to store and retrieve readonly data items.
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    public class ReadonlyObservableDictionary<TKey, TValue> :
        ReadOnlyCollection<KeyValuePair<TKey, TValue>>,
        IDictionary<TKey, TValue>,
#if !NET40
        IReadOnlyDictionary<TKey, TValue>,
#endif
        IDictionary,
        INotifyCollectionChanged,
        INotifyPropertyChanged
    {
        #region constructors
        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="dic"></param>
        public ReadonlyObservableDictionary(ObservableDictionary<TKey, TValue> dic) : base(dic)
        {
            Dictionary = dic ?? throw new ArgumentNullException(nameof(dic));

            ((INotifyPropertyChanged)Dictionary).PropertyChanged += (_, e) => OnPropertyChanged(e);
            Dictionary.CollectionChanged += (_, e) => OnCollectionChanged(e);
        }
        #endregion constructors

        #region events
        /// <summary>
        /// Implements the <see cref="System.Collections.Specialized.INotifyCollectionChanged.CollectionChanged"/> event.
        /// </summary>
        protected virtual event NotifyCollectionChangedEventHandler CollectionChanged = (_, __) => { };

        /// <summary>
        /// Implements the <see cref="System.Collections.Specialized.INotifyCollectionChanged.CollectionChanged"/> event
        /// for the <see cref="INotifyCollectionChanged"/> interface.
        /// </summary>
        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add { CollectionChanged += value; }
            remove { CollectionChanged -= value; }
        }

        /// <summary>
        /// Implements the <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/>
        /// event raised when a property is changed on a component.
        /// </summary>
        protected virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Implements the <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/>
        /// event raised when a property is changed on a component.
        /// </summary>
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { PropertyChanged += value; }
            remove { PropertyChanged -= value; }
        }
        #endregion events

        #region properties
        /// <summary>
        /// Gets the internal <see cref="ObservableDictionary{TKey, TValue}"/> instance.
        /// </summary>
        protected ObservableDictionary<TKey, TValue> Dictionary { get; private set; }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// Setting this is not supported and will throw <see cref="NotSupportedException"/>.
        /// 
        /// Exceptions:
        ///   T:System.ArgumentNullException:
        ///     key is null.
        /// 
        ///   T:System.NotSupportedException:
        ///     The property is set and the System.Collections.IDictionary object is read-only.-or-
        ///     The property is set, key does not exist in the collection, and the System.Collections.IDictionary
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get => Dictionary[key];
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// Setting this is not supported and will throw <see cref="NotSupportedException"/>.
        /// 
        /// (implements the <see cref="IDictionary"/> interface member).
        /// 
        /// Exceptions:
        ///   T:System.ArgumentNullException:
        ///     key is null.
        /// 
        ///   T:System.NotSupportedException:
        ///     The property is set and the System.Collections.IDictionary object is read-only.-or-
        ///     The property is set, key does not exist in the collection, and the System.Collections.IDictionary
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object IDictionary.this[object key]
        {
            get => this[(TKey)key];
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// Gets an object containing the keys
        /// of the <see cref="System.Collections.IDictionary"/> object.
        /// </summary>
        public CollectionBasedDictionary<TKey, TValue>.KeyCollection Keys => Dictionary.Keys;

        /// <summary>
        /// Gets an <see cref="System.Collections.ICollection"/> object containing the keys
        /// of the <see cref="System.Collections.IDictionary"/> object for the
        /// <see cref="IDictionary{TKey, TValue}"/> member method.
        /// </summary>
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => this.Keys;
#if !NET40
        /// <summary>
        /// Gets an <see cref="IEnumerable{TKey}"/> object containing the keys
        /// of the <see cref="System.Collections.IDictionary"/> object for the
        /// <see cref="IReadOnlyDictionary{TKey, TValue}"/> member method.
        /// </summary>
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => this.Keys;
#endif
        /// <summary>
        /// Gets an <see cref="System.Collections.ICollection"/> object containing the keys
        /// of the <see cref="System.Collections.IDictionary"/> object for the
        /// <see cref="IDictionary"/> member method.
        /// </summary>
        ICollection IDictionary.Keys => this.Keys;

        /// <summary>
        /// Gets an object containing the elements
        /// of the <see cref="System.Collections.IDictionary"/> object.
        /// </summary>
        public CollectionBasedDictionary<TKey, TValue>.ValueCollection Values => Dictionary.Values;

        /// <summary>
        /// Gets an <see cref="System.Collections.ICollection"/> object containing the elements
        /// of the <see cref="System.Collections.IDictionary"/> object.
        /// </summary>
        ICollection<TValue> IDictionary<TKey, TValue>.Values => this.Values;
#if !NET40
        /// <summary>
        /// Gets an <see cref="IEnumerable{TValue}"/> object containing the elements
        /// of the <see cref="System.Collections.IDictionary"/> object for the
        /// <see cref="IReadOnlyDictionary{TKey, TValue}"/> member method.
        /// </summary>
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => this.Values;
#endif

        /// <summary>
        /// Gets an <see cref="ICollection"/> object containing the elements
        /// of the <see cref="System.Collections.IDictionary"/> object for the
        /// <see cref="IDictionary"/> member method.
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
        public bool IsReadOnly => true;
        #endregion properties

        #region methods
        /// <summary>
        /// Method is not supported an will throw <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value) => throw new NotSupportedException();

        /// <summary>
        /// Method is not supported an will throw <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void IDictionary.Add(object key, object value) => throw new NotSupportedException();

        /// <summary>
        /// Method is not supported an will throw <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key) => throw new NotSupportedException();

        /// <summary>
        /// Method is not supported an will throw <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="key"></param>
        void IDictionary.Remove(object key) => throw new NotSupportedException();

        /// <summary>
        /// Determines whether the collection contains an element with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key) => Dictionary.ContainsKey(key);

        /// <summary>
        /// Determines whether the collection contains an element with the specified key.
        /// (Implements the <seealso cref="IDictionary"/> memeber).
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool IDictionary.Contains(object key) => this.ContainsKey((TKey)key);

        /// <summary>
        /// Method is not supported and will throw <see cref="NotSupportedException"/>.
        /// </summary>
        public void Clear() => throw new NotSupportedException();

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key,
        /// if the key is found; otherwise, the default value for the type of the value parameter.
        /// This parameter is passed uninitialized.</param>
        /// <returns>true if the <seealso cref="IDictionary{TKey, TValue}"/> contains an element with
        /// the specified key; otherwise, false.</returns>
        public bool TryGetValue(TKey key, out TValue value) => Dictionary.TryGetValue(key, out value);

        /// <summary>
        /// Returns an enumerator that iterates through the <seealso cref="IDictionary"/> interface.
        /// </summary>
        /// <returns></returns>
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)Dictionary).GetEnumerator();
        }

        /// <summary>
        /// Methode fires the <see cref="CollectionChanged"/> event with the notification parameter <paramref name="args"/>.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args) => CollectionChanged(this, args);

        /// <summary>
        /// Methode fires the <see cref="PropertyChanged"/> event with the notification parameter <paramref name="args"/>.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args) => PropertyChanged(this, args);
        #endregion methods
    }
}
