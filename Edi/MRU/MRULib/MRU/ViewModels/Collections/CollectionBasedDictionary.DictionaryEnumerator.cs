namespace MRULib.MRU.ViewModels.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Source: https://github.com/gayaK/Gayak.ObservableDictionary
    /// 
    /// Implements the <seealso cref="DictionaryEnumerator"/> class
    /// in the <seealso cref="CollectionBasedDictionary{TKey, TValue}"/> class.
    /// </summary>
    public partial class CollectionBasedDictionary<TKey, TValue>
    {
        /// <summary>
        /// Enumerates the elements of a nongeneric dictionary with an
        /// <seealso cref="IDisposable"/> implementation.
        /// </summary>
        public class DictionaryEnumerator : IDictionaryEnumerator, IDisposable
        {
            #region fields
            private IEnumerator<KeyValuePair<TKey, TValue>> _source;
            #endregion fields

            #region constructors
            /// <summary>
            /// Class constructor.
            /// </summary>
            /// <param name="source"></param>
            public DictionaryEnumerator(IEnumerator<KeyValuePair<TKey, TValue>> source)
            {
                _source = source ?? throw new ArgumentNullException(nameof(source));
            }
            #endregion constructors

            #region properties
            /// <summary>
            /// Gets the key of the current dictionary entry.
            /// 
            /// Returns: The key of the current element of the enumeration.
            /// 
            /// Exceptions:
            ///   T:System.InvalidOperationException:
            ///     The System.Collections.IDictionaryEnumerator is positioned before the first entry
            ///     of the dictionary or after the last entry.
            /// </summary>
            public object Key => _source.Current.Key;

            /// <summary>
            /// Gets the value of the current dictionary entry.
            /// 
            /// Returns: The value of the current element of the enumeration.
            /// 
            /// Exceptions:
            ///   T:System.InvalidOperationException:
            ///     The System.Collections.IDictionaryEnumerator is positioned before the first entry
            ///     of the dictionary or after the last entry.
            /// </summary>
            public object Value => _source.Current.Value;

            /// <summary>
            /// Gets both the key and the value of the current dictionary entry.
            /// 
            /// Returns:
            /// A System.Collections.DictionaryEntry containing both the key and the value of
            /// the current dictionary entry.
            /// 
            /// Exceptions:
            ///   T:System.InvalidOperationException:
            ///     The System.Collections.IDictionaryEnumerator is positioned before the first entry
            ///     of the dictionary or after the last entry.
            /// </summary>
            public DictionaryEntry Entry => new DictionaryEntry(Key, Value);
            #endregion properties

            #region methods
            /// <summary>
            /// Gets the current <seealso cref="KeyValuePair{TKey, TValue}"/> in the collection.
            /// </summary>
            public object Current => _source.Current;

            /// <summary>
            /// Moves to the next current <seealso cref="KeyValuePair{TKey, TValue}"/> in the collection.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext() => _source.MoveNext();

            /// <summary>
            ///     Sets the enumerator to its initial position, which is before the first element
            ///     in the collection.
            /// 
            /// Exceptions:
            ///   T:System.InvalidOperationException:
            ///     The collection was modified after the enumerator was created.
            /// </summary>
            public void Reset() => _source.Reset();

            #region IDisposable Support
            private bool disposedValue = false;

            /// <summary>
            /// Standard internal dispose method.
            /// </summary>
            /// <param name="disposing"></param>
            protected virtual void Dispose(bool disposing)
            {
                if (disposedValue == false)
                {
                    if (disposing == true)
                    {
                        (_source as IDisposable)?.Dispose();
                    }
                    disposedValue = true;
                }
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting
            /// unmanaged resources.
            /// </summary>
            public void Dispose() => Dispose(true);
            #endregion
            #endregion methods
        }
    }
}
