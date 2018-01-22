namespace FileSystemModels.Utils
{
  using System;
  using System.Collections.ObjectModel;
  using System.Linq;

  /// <summary>
  /// Class provides extension methods for manipulating collections with Linq.
  /// </summary>
  public static class LinqExtensions
  {
    #region methods
    /// <summary>
    /// Remove all elements that satisfy a condition from an observable collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="coll"></param>
    /// <param name="condition"></param>
    /// <returns>item count of removed items</returns>
    public static int Remove<T>(this ObservableCollection<T> coll, Func<T, bool> condition)
    {
      var itemsToRemove = coll.Where(condition).ToList();

      foreach (var itemToRemove in itemsToRemove)
        coll.Remove(itemToRemove);

      return itemsToRemove.Count;
    }
    #endregion methods
  }
}
