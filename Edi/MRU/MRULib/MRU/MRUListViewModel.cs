namespace MRULib.MRU.ViewModels
{
    using MRULib.MRU.Enums;
    using MRULib.MRU.Interfaces;
    using MRULib.MRU.Models;
    using MRULib.MRU.ViewModels.Base;
    using MRULib.MRU.ViewModels.Collections;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;

    /// <summary>
    /// Class hosts a collectionviewmodel and the ability to add/remove and update items
    /// in the collection. There is also command to invert a IsPinned value for a given
    /// item's key.
    /// </summary>
    internal class MRUListViewModel : Base.BaseViewModel, IMRUListViewModel
    {
        #region fields
        /// <summary>
        /// Defines the Minimum number of files that should ever be listed in the MRU.
        /// </summary>
        public const int MinValidMruEntryCount = 5;

        /// <summary>
        /// Defines the Maximum number of files that should ever be listed in the MRU.
        /// </summary>
        public const int MaxValidMruEntryCount = 256;

        private int _MaxMruEntryCount;

        private int _NextIsPinnedValue;

        private readonly ObservableDictionary<string, IMRUEntryViewModel> _Entries = null;
        private ICommand _ItemIsPinnedChanged;
        #endregion fields

        #region constructor
        /// <summary>
        /// Standard class constructor.
        /// </summary>
        public MRUListViewModel()
        {
            _MaxMruEntryCount = 45;
            _NextIsPinnedValue = 1;

            _Entries = new ObservableDictionary<string, IMRUEntryViewModel>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Copy class constructor.
        /// </summary>
        /// <param name="copySource"></param>
        public MRUListViewModel(MRUListViewModel copySource)
        {
            if (copySource == null) return;

            _Entries = new ObservableDictionary<string, IMRUEntryViewModel>(copySource.Entries);

            this.MaxMruEntryCount = copySource.MaxMruEntryCount;
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Gets the maximum number of MRU file entries.
        /// </summary>
        public int MaxMruEntryCount
        {
            get
            {
                return this._MaxMruEntryCount;
            }

            private set
            {
                if (this._MaxMruEntryCount != value)
                {
                    this._MaxMruEntryCount = value;

                    this.RaisePropertyChanged(() => this.MaxMruEntryCount);
                }
            }
        }

        /// <summary>
        /// Gets the items collection of MRU File Entries.
        /// </summary>
        public ObservableDictionary<string, IMRUEntryViewModel> Entries
        {
            get
            {
                return _Entries;
            }
        }

        /// <summary>
        /// Gets a command that will invert the IsPinned property of an
        /// instance of <seealso cref="MRUEntryViewModel"/>.
        /// 
        /// CommandParameter:
        /// The key for the value/key pair is expected to be supplied as
        /// string parameter of the command.
        /// </summary>
        public ICommand ItemIsPinnedChanged
        {
            get
            {
                if (_ItemIsPinnedChanged == null)
                {
                    _ItemIsPinnedChanged = new RelayCommand<object>((p) =>
                    {
                        if (p is string == false)
                            return;

                        var param = p as string;

                        if (param == null)
                            return;

                        IMRUEntryViewModel val;
                        if (Entries.TryGetValue(param, out val) == false)
                            return;

                        PinUnpinEntry(val);
                    });
                }

                return _ItemIsPinnedChanged;
            }
        }
        #endregion properties

        #region methods
        #region AddRemoveEntries
        /// <summary>
        /// Sets the pinning entry mode for this filenamepath entry.
        /// </summary>
        /// <param name="isPinned"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool PinUnpinEntry(bool isPinned, IMRUEntryViewModel e)
        {
            return this.PinUnpinEntry(isPinned, e.PathFileName);
        }

        /// <summary>
        /// Sets the pinning entry mode for this filenamepath entry.
        /// </summary>
        /// <param name="isPinned"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public bool PinUnpinEntry(bool isPinned, string filepath)
        {
            var pathFile = PathModel.NormalizePath(filepath);

            if (this.Entries.Contains(pathFile) == true)
            {
                IMRUEntryViewModel obj = FindEntry(pathFile);

                PinUnpinEntry(obj, isPinned);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Method either adds an entry if the given path was not available until now
        /// (with defaults for isPinned=false, LastUpdate = <seealso cref="DateTime.Now"/>)
        /// or
        /// property: LastUpdate = <seealso cref="DateTime.Now"/> is updated only.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns>true if update or insert was performd, false otherwise.</returns>
        public bool UpdateEntry(string filepath)
        {
            var entry = FindEntry(filepath);
            if (entry != null)
            {
                entry.SetLastUpdate(DateTime.Now);
                UpdateEntry(entry);

                return true;
            }

            this.AddEntry(filepath, new MRUEntryViewModel(filepath));

            return true;
        }

        /// <summary>
        /// Method adds an entry if the given path was not available until now.
        /// Otherwise, elements properties (IsPinned) are updated only.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="isPinned"></param>
        /// <param name="bUpdate"></param>
        /// <returns>true if update or insert was performd, false otherwise.</returns>
        public bool UpdateEntry(string filepath
                              , bool isPinned = false
                              , bool bUpdate = false)
        {
            DateTime lastUpdate = DateTime.Now;
            var pathFile = PathModel.NormalizePath(filepath);

            if (this.Entries.Contains(pathFile) == true)
            {
                if (bUpdate == false)
                    return false;

                IMRUEntryViewModel item = FindEntry(pathFile);

                PinUnpinEntry(item, isPinned);
                item.SetLastUpdate(lastUpdate);

                UpdateItem(item);

                return true;
            }

            // Remove last entry if list has grown too long
            if (this.MaxMruEntryCount <= this.Entries.Count)
                this.Entries.RemoveAt(this.Entries.Count - 1);

            this.AddEntry(pathFile, new MRUEntryViewModel(pathFile));

            return true;
        }

        /// <summary>
        /// Method adds an entry if the given path was not available until now.
        /// Otherwise, elements properties (IsPinned) are updated only.
        /// </summary>
        /// <param name="emp"></param>
        public void UpdateEntry(IMRUEntryViewModel emp)
        {
            if (emp == null)
                return;

            if ((emp is MRUEntryViewModel) == false)
                throw new NotSupportedException("Parameter emp cannot be updated since it is not of a known internal viewmodel type. Consider using a different UpdateEntry() method.");

            var pathFile = PathModel.NormalizePath(emp.File.Path);

            if (this.Entries.Contains(pathFile) == true)
            {
                IMRUEntryViewModel item;
                this.Entries.TryGetValue(pathFile, out item);

                // Assumption: We have noting to pin or unpin if both,
                // old and new value, indicate the same pin state
                if (item.IsPinned != 0 && emp.IsPinned == 0)
                {
                    this.PinUnpinEntry(false, item);     // Remove existing pin
                }
                else
                {
                    if (item.IsPinned == 0 && emp.IsPinned != 0)
                        this.PinUnpinEntry(true, item);        // Set new pin
                }

                item.SetLastUpdate(emp.LastUpdate);
                UpdateItem(item);

                return;
            }

            // Remove last entry if list has grown too long
            if (this.MaxMruEntryCount <= this.Entries.Count)
                this.Entries.RemoveAt(this.Entries.Count - 1); // Remove last existing pin

            // Add a completely new entry
            this.AddEntry(pathFile, emp);
        }

        /// <summary>
        /// Removes all items in the collection.
        /// </summary>
        public void Clear()
        {
            this.Entries.Clear();

            // Reset Pin Sort Indicator since there should be none here anymore.
            _NextIsPinnedValue = 1;
        }

        /// <summary>
        /// Removes all items that can be identified with this key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true if item was succesfully removed, otherwise false.</returns>
        public bool RemoveEntry(string key)
        {
            var pathFile = new PathModel(key, FSItemType.File);

            var item = this.Entries.SingleOrDefault(p => p.Key == pathFile.Path);

            return RemoveEntry(item.Value);
        }

        /// <summary>
        /// Remove any entry (whether its pinned or not) based on a viewmodel item.
        /// </summary>
        /// <param name="mruEntry"></param>
        /// <returns>true if item was succesfully removed, otherwise false.</returns>
        public bool RemoveEntry(IMRUEntryViewModel mruEntry)
        {
            if (mruEntry == null)
                return false;

            if (mruEntry.IsPinned != 0)
                PinUnpinEntry(mruEntry, false);

            this.Entries.Remove(mruEntry.PathFileName);

            return true;
        }

        /// <summary>
        /// Removes all items in the collection belongong to this group.
        /// </summary>
        /// <param name="groupType"></param>
        public bool RemoveEntry(GroupType groupType)
        {
            foreach (var item in FindEntries(groupType))
                RemoveEntry(item.Key);

            if (groupType == GroupType.IsPinned)  // Reset Pin Sort Indicator since
                _NextIsPinnedValue = 1;          // there should be none here anymore.

            return true;
        }

        /// <summary>
        /// Removes all items that or older than the given <see cref="DateTime"/> value.
        /// Items with IsPinned = True are not removed.
        /// </summary>
        /// <param name="removeOlderThanThis"></param>
        public void RemoveEntry(DateTime removeOlderThanThis)
        {
            var list = this.Entries.Where(x => x.Value.LastUpdate <= removeOlderThanThis && x.Value.IsPinned == 0).ToList();

            foreach (var item in list)
                this.Entries.Remove(item);
        }

        /// <summary>
        /// <paramref name="group"/> param != IsPinned:
        /// Removes all items that or older than the <paramref name="group"/>.
        /// Items with IsPinned = True are not removed.
        /// 
        /// <paramref name="group"/> param == IsPinned:
        /// Removes all items that are not pinned.
        /// </summary>
        /// <param name="group"></param>
        public void RemoveEntryOlderThanThis(GroupType group)
        {
            if (group == GroupType.IsPinned)
            {
                RemovePinnedEntries(false);
                return;
            }

            var model = new TimeSpanModelList();

            GroupTimeSpanModel timeSpanModel = null;
            if (model.List.TryGetValue(group, out timeSpanModel))
            {
                RemoveEntry(timeSpanModel.MinTime);
            }
        }

        /// <summary>
        /// Removes all items that are pinned or all items that are not pinned
        /// depending on the value in <paramref name="isPinned"/>.
        /// </summary>
        /// <param name="isPinned">All pinned items are removed, if this is true.
        /// Otherwise, all items not pinned are removed</param>
        public bool RemovePinnedEntries(bool isPinned)
        {
            if (isPinned == true)
                return RemoveEntry(GroupType.IsPinned);

            IEnumerable<KeyValuePair<string, IMRUEntryViewModel>> list = null;

            list = this.Entries.Where(x => x.Value.IsPinned == 0).ToList();

            // Remove each entry that is either pinned or not
            foreach (var item in list)
                this.Entries.Remove(item);

            return true;
        }

        /// <summary>
        /// Sets the maximum number of MRU file entries. The oldest file reference entries are
        /// removed if there are more entries being added than what is allowed here.
        /// </summary>
        public void ResetMaxMruEntryCount(int maxCount = MRUListViewModel.MaxValidMruEntryCount)
        {
            if (maxCount < MinValidMruEntryCount || maxCount > MaxValidMruEntryCount)
                throw new ArgumentOutOfRangeException("MaxMruEntryCount", maxCount, "Valid values are: value >= " + MinValidMruEntryCount + " and value <= " + MaxValidMruEntryCount);

            var pinlList = FindEntries(GroupType.IsPinned).ToList();

            if (this.Entries.Count > maxCount)
            {
                // We have more or same number of pinned entries as the max number of entries
                // so we remove by lastupdate ...
                if (pinlList.Count >= maxCount)
                {
                    var list = this.Entries.ToList();

                    list.Sort((pair1, pair2) => (pair1.Value.LastUpdate.CompareTo(pair2.Value.LastUpdate)));

                    // Removes all superfloues elements if any
                    for (int i = list.Count - 1; i >= maxCount; i--)
                        this.Entries.Remove(list.ElementAt(i).Key);
                }
                else
                {
                    // We have to remove this many entries besides the pinned entries
                    int iDiff = maxCount - pinlList.Count;

                    if (iDiff > 0)
                    {
                        var list = this.Entries.Where(i => i.Value.IsPinned == 0).ToList();

                        list.Sort((pair1, pair2) => (pair1.Value.LastUpdate.CompareTo(pair2.Value.LastUpdate)));

                        // Removes all superfloues elements if any
                        for (int i = list.Count - 1; i >= iDiff; i--)
                            this.Entries.Remove(list.ElementAt(i).Key);
                    }
                }
            }

            this.MaxMruEntryCount = maxCount;
        }
        #endregion AddRemoveEntries

        /// <summary>
        /// Finds an MRU entry by the file reference an returns it or null.
        /// 
        /// Throws a <seealso cref="NotSupportedException"/> if parameter is empty or null.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public IMRUEntryViewModel FindEntry(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) == true)
                throw new NotSupportedException("filePath parameter cannot be empty.");

            var pathFile = PathModel.NormalizePath(filePath);

            IMRUEntryViewModel obj;
            this.Entries.TryGetValue(pathFile, out obj);

            return obj;
        }

        /// <summary>
        /// Finds all items in the collection belongong to this <paramref name="groupType"/>.
        /// </summary>
        /// <param name="groupType"></param>
        public IEnumerable<KeyValuePair<string,IMRUEntryViewModel>> FindEntries(GroupType groupType)
        {
            return this.Entries.Where(x => x.Value.GroupItem.Group == groupType).Cast<KeyValuePair<string, IMRUEntryViewModel>>();
        }

        /// <summary>
        /// Moves a pinned MRU Item up/down in the group of pinned items.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="param"></param>
        public void MovePinnedEntry(MoveMRUItem direction, IMRUEntryViewModel param)
        {
            if (param == null)
                return;

            if (param.IsPinned == 0)  // Move only within the list group of pinned items
                return;

            switch (direction)
            {
                case MoveMRUItem.Up:
                    MoveItemUp(param);
                    break;

                case MoveMRUItem.Down:
                    MoveItemDown(param);
                    break;

                default:
                    throw new NotSupportedException(direction.ToString());
            }
        }

        #region private methods
        /// <summary>
        /// Negates the pin entry state for a given mru list entry.
        /// </summary>
        /// <param name="val"></param>
        private void PinUnpinEntry(IMRUEntryViewModel val)
        {
            var newIsPinnedValue = (val.IsPinned == 0 ? _NextIsPinnedValue : 0);

            var oldIsPinnedValue = val.IsPinned;

            val.SetIsPinned(newIsPinnedValue);

            if (newIsPinnedValue != 0)      // Increment next IsPinned order entry
                _NextIsPinnedValue += 1;   // If this entry is a newly pinned enty
            else
            {
                DecrementPinnedValues(oldIsPinnedValue);
            }
        }

        /// <summary>
        /// Pin or unpines an entry depending on the <paramref name="isPinned"/> parameter.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="isPinned"></param>
        private void PinUnpinEntry(IMRUEntryViewModel val, bool isPinned)
        {
            var oldIsPinnedValue = val.IsPinned;

            if (isPinned == true)
            {
                var newIsPinnedValue = _NextIsPinnedValue;

                val.SetIsPinned(newIsPinnedValue);

                                            // Increment next IsPinned order entry
                _NextIsPinnedValue += 1;   // since this entry is a newly pinned enty
            }
            else
            {
                val.SetIsPinned(0);

                DecrementPinnedValues(oldIsPinnedValue);
            }
        }

        private void DecrementPinnedValues(int oldIsPinnedValue)
        {
            if (_NextIsPinnedValue > 1)   // Decrement current maximum towards 1
                _NextIsPinnedValue -= 1;

            // The pinned entry was the highest available pin value
            // So, we just decrement the current value and are done
            if (oldIsPinnedValue == _NextIsPinnedValue)
                return;

            SortedDictionary<int, IMRUEntryViewModel> sortList = new SortedDictionary<int, IMRUEntryViewModel>();

            var list = this.Entries.Where(x => x.Value.IsPinned != 0).ToList();

            // Add all pinned items into an ordered list to preserve current order
            foreach (var item in list)
                sortList.Add(item.Value.IsPinned, item.Value );

            // Update actual pinned values to preserve current order but close existing gapes
            int isPinned = 1;
            foreach (var item in sortList)
                item.Value.SetIsPinned(isPinned++);
        }

        private bool MoveItemUp(IMRUEntryViewModel param)
        {
            // Nothing to move if Item is already in first spot
            if (param.IsPinned <= 1)
                return false;

            // Get all pinned entries
            var list = this.Entries.Where(x => x.Value.IsPinned != 0).ToList();

            if (list.Count <= 1)
                return false;

            // Find the item that is before the item that is to be moved
            IMRUEntryViewModel upperItem = null;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Value.IsPinned < param.IsPinned)
                {
                    if (upperItem == null)
                        upperItem = list[i].Value;
                    else
                    {
                        if (list[i].Value.IsPinned > upperItem.IsPinned)
                            upperItem = list[i].Value;
                    }
                }
            }


            // Exchange spots via exchange of IsPinned values and direct add/remove
            return ExchangePinnedSpots(upperItem, param);
        }

        private bool MoveItemDown(IMRUEntryViewModel param)
        {
            // Get all pinned entries
            var list = this.Entries.Where(x => x.Value.IsPinned != 0).ToList();

            if (list.Count <= 1)
                return false;

            // Find the item that is before the item that is to be moved
            IMRUEntryViewModel lowerItem = null;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Value.IsPinned > param.IsPinned)
                {
                    if (lowerItem == null)
                        lowerItem = list[i].Value;
                    else
                    {
                        if (list[i].Value.IsPinned < lowerItem.IsPinned)
                            lowerItem = list[i].Value;
                    }
                }
            }

            // Exchange spots via exchange of IsPinned values and direct add/remove
            return ExchangePinnedSpots(lowerItem, param);
        }

        /// <summary>
        /// This is invoked when an existing item's peroperties have changed -
        /// that is the item was already in the list but a sort criteria has changed.
        /// 
        /// To make that change visible we are removing and inserting the item in the list.
        /// There does not seem any other way to get that update across(?):
        /// https://social.msdn.microsoft.com/Forums/vstudio/en-US/cb7c5c62-7ca9-49b5-91a0-379581b1c1aa/listview-sorting-not-updating-when-items-in-the-itemssource-changes?forum=wpf
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool UpdateItem(IMRUEntryViewModel item)
        {
            if (item != null)
            {
                this.Entries.Remove(item.PathFileName);
                this.Entries.Add(item.PathFileName, item);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Exchange the spots of pinned items <paramref name="item1"/>
        /// and <paramref name="item2"/> via exchange of IsPinned values
        /// and direct add/remove in viewmodel collection.
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <returns></returns>
        private bool ExchangePinnedSpots(IMRUEntryViewModel item1
                                       , IMRUEntryViewModel item2)
        {
            if (item1 != null && item2 != null)
            {
                if (item1.IsPinned == 0 || item2.IsPinned == 0)
                    return false;

                this.Entries.Remove(item2.PathFileName);
                this.Entries.Remove(item1.PathFileName);

                var isPinnedBack = item1.IsPinned;
                item1.SetIsPinned(item2.IsPinned);
                item2.SetIsPinned(isPinnedBack);

                this.Entries.Add(item2.PathFileName, item2);
                this.Entries.Add(item1.PathFileName, item1);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a new entry into the list of MRU entries and
        /// ensures the correct pin counter states.
        /// </summary>
        /// <param name="pathFile"></param>
        /// <param name="emp"></param>
        private void AddEntry(string pathFile,
                              IMRUEntryViewModel emp)
        {
            // Add a completely new entry
            var newobj = new MRUEntryViewModel(emp as MRUEntryViewModel);
            var newIsPinnedValue = (newobj.IsPinned == 0 ? false : true);

            // Reset pin counter on item to make sure its correctly placed in the sorted list
            if (newIsPinnedValue == true)
                this.PinUnpinEntry(newobj, newIsPinnedValue);

            this.Entries.Add(pathFile, newobj);
        }
        #endregion private methods
        #endregion methods
    }
}
