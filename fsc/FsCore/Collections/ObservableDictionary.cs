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
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    /// <summary>
    /// This class provides a dictionary that can be bound to a WPF control.
    /// </summary>
    public class ObservableDictionary<TKey, TValue> :
    INotifyCollectionChanged,
    IDictionary<TKey, TValue>,
    ICollection<KeyValuePair<TKey, TValue>>,
    IEnumerable<KeyValuePair<TKey, TValue>>,
    IEnumerable,
    ICollection
    {

        // ************************************************************************
        // Private Fields
        // ************************************************************************
        #region Private Fields

        /// <summary>
        /// A dictionary of link list nodes to work out for the key the corresponding
        /// index for the master list, key list, and value list.
        /// </summary>
        protected Dictionary<TKey, DoubleLinkListIndexNode> _keyToIndex;
        /// <summary>
        /// An observable list of key value pairs
        /// </summary>
        protected ObservableCollection<KeyValuePair<TKey, TValue>> _masterList;
        /// <summary>
        /// The last node of the link list, used for adding new nodes to the end
        /// </summary>
        protected DoubleLinkListIndexNode _lastNode = null;
        /// <summary>
        /// The list of keys for the keys property
        /// </summary>
        protected KeyCollection<TKey, TValue> _keys;
        /// <summary>
        /// The list of values for the values property
        /// </summary>
        protected ValueCollection<TKey, TValue> _values;

        #endregion Private Fields

        // ************************************************************************
        // Public Methods
        // ************************************************************************
        #region Public Methods

        /// <summary>
        /// Initializes a new instance of this class that is empty, has the default
        /// initial capacity, and uses the default equality comparer for the key
        /// type.
        /// </summary>
        public ObservableDictionary()
        {
            _keyToIndex = new Dictionary<TKey, DoubleLinkListIndexNode>();
            _masterList = new ObservableCollection<KeyValuePair<TKey, TValue>>();
            _masterList.CollectionChanged += new NotifyCollectionChangedEventHandler(masterList_CollectionChanged);

            _keys = new KeyCollection<TKey, TValue>(this);
            _values = new ValueCollection<TKey, TValue>(this);
        }

        /// <summary>
        /// Initializes a new instance of this class that contains elements copied
        /// from the specified IDictionary<TKey, TValue> and uses the default
        /// equality comparer for the key type.
        /// </summary>
        /// <param name="source"></param>
        public ObservableDictionary(IDictionary<TKey, TValue> source)
          : this()
        {

            foreach (KeyValuePair<TKey, TValue> pair in source)
            {
                Add(pair);
            }
        }

        /// <summary>
        /// Initializes a new instance of this class that is empty, has the default
        /// initial capacity, and uses the specified IEqualityComparer<T>.
        /// </summary>
        /// <param name="equalityComparer"></param>
        public ObservableDictionary(IEqualityComparer<TKey> equalityComparer)
          : this()
        {

            _keyToIndex = new Dictionary<TKey, DoubleLinkListIndexNode>(equalityComparer);
        }

        /// <summary>
        /// Initializes a new instance of this class that is empty, has the
        /// specified initial capacity, and uses the default equality comparer for
        /// the key type.
        /// </summary>
        /// <param name="capactity"></param>
        public ObservableDictionary(int capactity)
          : this()
        {

            _keyToIndex = new Dictionary<TKey, DoubleLinkListIndexNode>(capactity);
        }

        /// <summary>
        /// Initializes a new instance of this class that contains elements copied
        /// from the specified IDictionary<TKey, TValue> and uses the specified
        /// IEqualityComparer<T>.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="equalityComparer"></param>
        public ObservableDictionary(IDictionary<TKey, TValue> source, IEqualityComparer<TKey> equalityComparer)
          : this(equalityComparer)
        {

            foreach (KeyValuePair<TKey, TValue> pair in source)
            {
                Add(pair);
            }
        }

        /// <summary>
        /// Initializes a new instance of this class that is empty, has the
        /// specified initial capacity, and uses the specified
        /// IEqualityComparer<T>.
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="equalityComparer"></param>
        public ObservableDictionary(int capacity, IEqualityComparer<TKey> equalityComparer)
          : this()
        {

            _keyToIndex = new Dictionary<TKey, DoubleLinkListIndexNode>(capacity, equalityComparer);
        }

        /// <summary>
        /// Gets the array index of the key passed in.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int IndexOfKey(TKey key)
        {
            return _keyToIndex[key].Index;
        }

        /// <summary>
        /// Tries to get the index of the key passed in. Returns true if succeeded
        /// and false otherwise.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool TryGetIndexOf(TKey key, out int index)
        {
            DoubleLinkListIndexNode node;
            if (_keyToIndex.TryGetValue(key, out node))
            {
                index = node.Index;
                return true;
            }
            else
            {
                index = 0;
                return false;
            }
        }

        #endregion Public Methods

        // ************************************************************************
        // Events, Triggers and Handlers
        // ************************************************************************
        #region Events, Triggers and Handlers

        /// <summary>
        /// Handles when the internal key value list changes, and passes on the
        /// message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void masterList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged(e);
        }

        /// <summary>
        /// Triggers the CollectionChanged event in a way that it can be handled
        /// by controls on a different thread.
        /// </summary>
        /// <param name="e"></param>
        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion Events, Triggers and Handlers

        // ************************************************************************
        // IDictionary<TKey, TValue> Members
        // ************************************************************************
        #region IDictionary<TKey, TValue> Members

        /// <summary>
        /// Adds an element with the provided key and value to the IDictionary<TKey, TValue>.
        /// </summary>
        /// <param name="key">
        /// The object to use as the key of the element to add.
        /// </param>
        /// <param name="value">
        /// The object to use as the value of the element to add.
        /// </param>
        public virtual void Add(TKey key, TValue value)
        {
            DoubleLinkListIndexNode node = new DoubleLinkListIndexNode(_lastNode, _keyToIndex.Count);
            _keyToIndex.Add(key, node);
            _lastNode = node;
            _masterList.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>
        /// Determines whether the IDictionary<TKey, TValue> contains an element with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key to locate in the IDictionary<TKey, TValue>.
        /// </param>
        /// <returns>
        /// True if the IDictionary<TKey, TValue> contains an element with the key; otherwise, false.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            return _keyToIndex.ContainsKey(key);
        }

        /// <summary>
        /// Gets an ICollection<T> containing the keys of the IDictionary<TKey, TValue>.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get
            {
                return _keys;
            }
        }

        /// <summary>
        /// Removes the element with the specified key from the IDictionary<TKey, TValue>.
        /// </summary>
        /// <param name="key">
        /// The key of the element to remove.
        /// </param>
        /// <returns>
        /// True if the element is successfully removed; otherwise, false. This method also returns false if key was not found in the original IDictionary<TKey, TValue>.
        /// </returns>
        public bool Remove(TKey key)
        {
            DoubleLinkListIndexNode node;
            if (_keyToIndex.TryGetValue(key, out node))
            {
                _masterList.RemoveAt(node.Index);
                if (node == _lastNode)
                {
                    _lastNode = node.Previous;
                }
                node.Remove();
            }
            return _keyToIndex.Remove(key);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key whose value to get.
        /// </param>
        /// <param name="value">
        /// When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// True if the object that implements IDictionary<TKey, TValue> contains an element with the specified key; otherwise, false.
        /// </returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            DoubleLinkListIndexNode index;
            if (_keyToIndex.TryGetValue(key, out index))
            {
                value = _masterList[index.Index].Value;
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        /// <summary>
        /// Gets an ICollection<T> containing the values in the IDictionary<TKey, TValue>.
        /// </summary>
        public ICollection<TValue> Values
        {
            get
            {
                return _values;
            }
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key of the element to get or set.
        /// </param>
        /// <returns>
        /// The element with the specified key.
        /// </returns>
        public TValue this[TKey key]
        {
            get
            {
                int index = _keyToIndex[key].Index;
                return _masterList[index].Value;
            }
            set
            {
                if (ContainsKey(key))
                {
                    int index = _keyToIndex[key].Index;
                    _masterList[index] = new KeyValuePair<TKey, TValue>(key, value);
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        #endregion IDictionary<TKey, TValue> Members

        // ************************************************************************
        // ICollection<KeyValuePair<TKey, TValue>> Members
        // ************************************************************************
        #region ICollection<KeyValuePair<TKey, TValue>> Members

        /// <summary>
        /// Adds an item to the ICollection<T>.
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Removes all items from the ICollection<T>.
        /// </summary>
        public void Clear()
        {
            _keyToIndex.Clear();
            _masterList.Clear();
            _lastNode = null;
        }

        /// <summary>
        /// Determines whether the ICollection<T> contains a specific value.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _masterList.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the ICollection<T> to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _masterList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the ICollection<T>.
        /// </summary>
        /// <param name="item">
        /// The object to remove from the ICollection<T>.
        /// </param>
        /// <returns>
        /// True if item was successfully removed from the ICollection<T>; otherwise, false. This method also returns false if item is not found in the original ICollection<T>.
        /// </returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (Contains(item))
            {
                return Remove(item.Key);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the ICollection<T>.
        /// </summary>
        public int Count
        {
            get
            {
                return _masterList.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the ICollection<T> is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return ((ICollection<KeyValuePair<TKey, TValue>>)_masterList).IsReadOnly;
            }
        }

        #endregion ICollection<KeyValuePair<TKey, TValue>> Members

        // ************************************************************************
        // IEnumerable<KeyValuePair<TKey, TValue>> Members
        // ************************************************************************
        #region IEnumerable<KeyValuePair<TKey, TValue>> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A IEnumerator<T> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _masterList.GetEnumerator();
        }

        #endregion IEnumerable<KeyValuePair<TKey, TValue>> Members

        // ************************************************************************
        // IEnumerable Members
        // ************************************************************************
        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An IEnumerator object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable Members

        // ************************************************************************
        // ICollection Members
        // ************************************************************************
        #region ICollection Members

        /// <summary>
        /// Copies the elements of the ICollection to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index)
        {
            ((ICollection)_masterList).CopyTo(array, index);
        }

        /// <summary>
        /// Gets a value indicating whether access to the ICollection is synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized
        {
            get { return ((ICollection)_masterList).IsSynchronized; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the ICollection.
        /// </summary>
        public object SyncRoot
        {
            get { return ((ICollection)_masterList).SyncRoot; }
        }

        #endregion ICollection Members
    }
}
