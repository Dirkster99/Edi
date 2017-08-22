namespace MRULib.MRU.Interfaces
{
    using MRULib.MRU.Enums;
    using MRULib.MRU.ViewModels;
    using MRULib.MRU.ViewModels.Collections;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Input;

    /// <summary>
    /// Implements a viewmodel that can be used to list all recently used files.
    /// </summary>
    public interface IMRUListViewModel : INotifyPropertyChanged
    {
        #region properties
        /// <summary>
        /// Gets/sets the maximum number of MRU file entries.
        /// </summary>
        int MaxMruEntryCount { get; } 

        /// <summary>
        /// Gets the items collection of MRU File Entries.
        /// </summary>
        ObservableDictionary<string, IMRUEntryViewModel> Entries { get; }

        /// <summary>
        /// Gets a command that will invert the IsPinned property of an
        /// instance of <seealso cref="MRUEntryViewModel"/>.
        /// 
        /// CommandParameter:
        /// The key for the value/key pair is expected to be supplied as
        /// string parameter of the command.
        /// </summary>
        ICommand ItemIsPinnedChanged { get; }
        #endregion properties

        #region AddRemoveEntries
        /// <summary>
        /// Sets the pinning entry mode for this filenamepath entry.
        /// </summary>
        /// <param name="isPinned"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        bool PinUnpinEntry(bool isPinned, IMRUEntryViewModel e);

        /// <summary>
        /// Sets the pinning entry mode for this filenamepath entry.
        /// </summary>
        /// <param name="isPinned"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        bool PinUnpinEntry(bool isPinned, string filepath);

        /// <summary>
        /// Method either adds an entry if the given path was not available until now
        /// (with defaults for isPinned=false, LastUpdate = <seealso cref="DateTime.Now"/>)
        /// or
        /// property: LastUpdate = <seealso cref="DateTime.Now"/> is updated only.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns>true if update or insert was performd, false otherwise.</returns>
        bool UpdateEntry(string filepath);

        /// <summary>
        /// Method adds an entry if the given path was not available until now.
        /// Otherwise, elements properties (IsPinned) are updated only.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="isPinned"></param>
        /// <param name="bUpdate"></param>
        /// <returns>true if update or insert was performd, false otherwise.</returns>
        bool UpdateEntry(string filepath
                          , bool isPinned = false
                          , bool bUpdate = false);

        /// <summary>
        /// Method adds an entry if the given path was not available until now.
        /// Otherwise, elements properties (IsPinned) are updated only.
        /// </summary>
        /// <param name="mruEntry"></param>
        void UpdateEntry(IMRUEntryViewModel mruEntry);

        /// <summary>
        /// Removes all items in the collection.
        /// </summary>
        void Clear();

        /// <summary>
        /// Remove any entry (whether its pinned or not) based on a viewmodel item.
        /// </summary>
        /// <param name="mruEntry"></param>
        /// <returns>true if item was succesfully removed, otherwise false.</returns>
        bool RemoveEntry(IMRUEntryViewModel mruEntry);

        /// <summary>
        /// Removes all items that can be identified with this key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true if item was succesfully removed, otherwise false.</returns>
        bool RemoveEntry(string key);

        /// <summary>
        /// Removes all items in the collection belongong to this group.
        /// </summary>
        /// <param name="groupType"></param>
        bool RemoveEntry(GroupType groupType);

        /// <summary>
        /// Removes all items that or older than the given <see cref="DateTime"/> value.
        /// Items with IsPinned = True are not removed.
        /// </summary>
        /// <param name="removeOlderThanThis"></param>
        void RemoveEntry(DateTime removeOlderThanThis);

        /// <summary>
        /// <paramref name="group"/> param != IsPinned:
        /// Removes all items that or older than the <paramref name="group"/>.
        /// Items with IsPinned = True are not removed.
        /// 
        /// <paramref name="group"/> param == IsPinned:
        /// Removes all items that are not pinned.
        /// </summary>
        /// <param name="group"></param>
        void RemoveEntryOlderThanThis(GroupType group);

        /// <summary>
        /// Removes all items that pinned or all items that are not pinned
        /// depending on the value in <paramref name="isPinned"/>.
        /// </summary>
        /// <param name="isPinned">All pinned items are removed, if this is true.
        /// Otherwise, all items not pinned are removed</param>
        bool RemovePinnedEntries(bool isPinned);
        #endregion AddRemoveEntries

        /// <summary>
        /// Finds an MRU entry by the file reference an returns it or null.
        /// 
        /// Throws a <seealso cref="NotSupportedException"/> if parameter is empty or null.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        IMRUEntryViewModel FindEntry(string filePath);

        /// <summary>
        /// Finds all items in the collection belongong to this <paramref name="groupType"/>.
        /// </summary>
        /// <param name="groupType"></param>
        IEnumerable<KeyValuePair<string, IMRUEntryViewModel>> FindEntries(GroupType groupType);

        /// <summary>
        /// Moves a pinned MRU Entry up/down in the group of pinned items.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="param"></param>
        void MovePinnedEntry(MoveMRUItem direction, IMRUEntryViewModel param);

        /// <summary>
        /// Sets the maximum number of MRU file entries. The oldest file reference entries are
        /// removed if there are more entries being added than what is allowed here.
        /// </summary>
        void ResetMaxMruEntryCount(int maxCount = MRUListViewModel.MaxValidMruEntryCount);
    }
}