namespace MRULib.MRU.ViewModels.Collections
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Source: https://github.com/gayaK/Gayak.ObservableDictionary
    /// 
    /// Implements the <see cref="CollectionBasedDictionary{TKey, TValue}"/> class inside the
    /// <see cref="ObservableDictionary{TKey, TValue}"/> class.
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    public class ObservableDictionary<TKey, TValue> :
        CollectionBasedDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region fields
        private static readonly string _indexerName = "Item[]";
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor.
        /// </summary>
        public ObservableDictionary() { }

        /// <summary>
        /// Class constructor from <see cref="IEqualityComparer{TKey}"/> comparer parameter.
        /// </summary>
        /// <param name="comparer"></param>
        public ObservableDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }

        /// <summary>
        /// Copy class constructor from <see cref="IDictionary{TKey, TValue}"/> instance.
        /// </summary>
        /// <param name="dictionary"></param>
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }

        /// <summary>
        /// Copy class constructor from <see cref="IDictionary{TKey, TValue}"/> instance and
        /// <see cref="IEqualityComparer{TKey}"/> comparer parameter.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="comparer"></param>
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer) { }
        #endregion constructors

        #region events
        /// <summary>
        /// Implements the <see cref="System.Collections.Specialized.INotifyCollectionChanged.CollectionChanged"/> event.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged = (_, __) => { };

        /// <summary>
        /// Implements the <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/>
        /// event raised when a property is changed on a component.
        /// </summary>
        protected virtual event PropertyChangedEventHandler PropertyChanged = (_, __) => { };

        /// <summary>
        /// Implements the <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/>
        /// event raised when a property is changed on a component.
        /// </summary>
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add => this.PropertyChanged += value;
            remove => this.PropertyChanged -= value;
        }
        #endregion events

        #region methods
        /// <summary>
        /// Method is invoked after the collection has changed (insert item, remove item).
        /// This will fire the <see cref="CollectionChanged"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged(this, e);
        }

#if NET40
        /// <summary>
        /// Method is invoked when a property is changed to relay that fact via
        /// <see cref="INotifyPropertyChanged"/> interface by firing the
        /// <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="name"></param>
        protected virtual void OnPropertyChanged(string name)
#else
        /// <summary>
        /// Method is invoked when a property is changed to relay that fact via
        /// <see cref="INotifyPropertyChanged"/> interface by firing the
        /// <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="name"></param>
        protected virtual void OnPropertyChanged([CallerMemberName]string name = "")
#endif
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Inserts a <see cref="KeyValuePair{TKey, TValue}"/> object into the current collection.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, KeyValuePair<TKey, TValue> item)
        {
            base.InsertItem(index, item);
            OnCollectionInserted(item, index);
        }

        /// <summary>
        /// Removes a <see cref="KeyValuePair{TKey, TValue}"/> object at the indicated index.
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            var oldItem = this[index];
            base.RemoveItem(index);
            OnCollectionRemoved(oldItem, index);
        }

        /// <summary>
        /// Resets a <see cref="KeyValuePair{TKey, TValue}"/> object at the indicated
        /// index with another <see cref="KeyValuePair{TKey, TValue}"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void SetItem(int index, KeyValuePair<TKey, TValue> item)
        {
            var oldItem = this[index];
            base.SetItem(index, item);
            OnCollectionSet(item, oldItem, index);
        }

        /// <summary>
        /// Removes all <see cref="KeyValuePair{TKey, TValue}"/> objects from the current collection.
        /// </summary>
        protected override void ClearItems()
        {
            base.ClearItems();
            OnCollectionCleared();
        }

        private void OnCollectionInserted(KeyValuePair<TKey, TValue> item, int index)
        {
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(_indexerName);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        private void OnCollectionRemoved(KeyValuePair<TKey, TValue> item, int index)
        {
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(_indexerName);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        private void OnCollectionSet(KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem, int index)
        {
            OnPropertyChanged(_indexerName);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index));
        }

        private void OnCollectionCleared()
        {
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(_indexerName);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        #endregion methods
    }
}
