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

    /// <summary>
    /// This class provides the base restrictions for an immutable collection
    /// </summary>
    public abstract class ImmutableCollectionBase<T> : ICollection<T>, IEnumerable<T>, ICollection, IEnumerable {

    /// <summary>
    /// Gets the number of elements contained in the collection<T>.
    /// </summary>
    public abstract int Count { get; }

    /// <summary>
    /// Gets a value indicating that the collection is read-only.
    /// </summary>
    public bool IsReadOnly {
      get {
        return true;
      }
    }

    /// <summary>
    /// Throws the exception System.NotSupportedException:
    /// </param>
    public void Add(T item) {
      throw (new System.NotSupportedException("The Swordfish.NET.Collections.KeyCollection<TKey,TValue> is read-only."));
    }

    /// <summary>
    /// Throws the exception System.NotSupportedException:
    /// </param>
    public void Clear() {
      throw (new System.NotSupportedException("The Swordfish.NET.Collections.KeyCollection<TKey,TValue> is read-only."));
    }

    /// </summary>
    /// <param name="item">The object to locate</param>
    /// <returns>true if item is found otherwise false</returns>
    public abstract bool Contains(T item);

    /// <summary>
    //  Copies the elements of the collection to an array, starting
    /// at a particular index.
    /// </summary>
    public abstract void CopyTo(T[] array, int arrayIndex);

    /// <summary>
    /// Throws the exception System.NotSupportedException:
    /// </param>
    public bool Remove(T item) {
      throw (new System.NotSupportedException("The Swordfish.NET.Collections.KeyCollection<TKey,TValue> is read-only."));
    }

    /// <summary>
    /// Gets the enumerator for the collection
    /// </summary>
    public abstract IEnumerator<T> GetEnumerator();

    /// <summary>
    /// Gets the enumerator for the collection
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    void ICollection.CopyTo(Array array, int index) {
      CopyTo((T[])array, index);
    }

    bool ICollection.IsSynchronized {
      get { throw new NotImplementedException(); }
    }

    object ICollection.SyncRoot {
      get { throw new NotImplementedException(); }
    }
  }
}
