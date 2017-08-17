namespace MRULib.MRU.ViewModels
{
    using MRULib.MRU.Enums;
    using MRULib.MRU.Interfaces;
    using MRULib.MRU.Models;
    using MRULib.MRU.ViewModels.Base;
    using MRULib.MRU.ViewModels.Collections;
    using System;
    using System.Linq;
    using System.Windows.Input;

    /// <summary>
    /// Class hosts a collectionviewmodel and the ability to add/remove and update items
    /// in the collection. There is also command to invert a IsPinned value for a given
    /// item's key.
    /// </summary>
    public class MRUListViewModel : Base.BaseViewModel, IMRUListViewModel
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

        private readonly ObservableDictionary<string, MRUEntryViewModel> _Entries = null;
        private ICommand _ItemIsPinnedChanged;
        #endregion fields

        #region constructor
        /// <summary>
        /// Standard class constructor.
        /// </summary>
        public MRUListViewModel()
        {
            _MaxMruEntryCount = 45;

            _Entries = new ObservableDictionary<string, MRUEntryViewModel>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Copy class constructor.
        /// </summary>
        /// <param name="copySource"></param>
        public MRUListViewModel(MRUListViewModel copySource)
        {
            if (copySource == null) return;

            _Entries = new ObservableDictionary<string, MRUEntryViewModel>(copySource.Entries);

            this.MaxMruEntryCount = copySource.MaxMruEntryCount;
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Gets/sets the maximum number of MRU file entries.
        /// </summary>
        public int MaxMruEntryCount
        {
            get
            {
                return this._MaxMruEntryCount;
            }

            set
            {
                if (this._MaxMruEntryCount != value)
                {
                    if (value < MinValidMruEntryCount || value > MaxValidMruEntryCount)
                        throw new ArgumentOutOfRangeException("MaxMruEntryCount", value, "Valid values are: value >= 5 and value <= 256");

                    this._MaxMruEntryCount = value;

                    this.RaisePropertyChanged(() => this.MaxMruEntryCount);
                }
            }
        }

        /// <summary>
        /// Gets the items collection of MRU File Entries.
        /// </summary>
        public ObservableDictionary<string, MRUEntryViewModel> Entries
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

                        MRUEntryViewModel val;
                        if (Entries.TryGetValue(param, out val) == false)
                            return;

                        val.SetIsPinned(!val.IsPinned);
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
        public bool PinUnpinEntry(bool isPinned, MRUEntryViewModel e)
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
                MRUEntryViewModel obj = FindMRUEntry(pathFile);

                obj.SetIsPinned(isPinned);

                return true;
            }

            return false;
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

                MRUEntryViewModel obj = FindMRUEntry(pathFile);

                obj.SetIsPinned(isPinned);
                obj.SetLastUpdate(lastUpdate);

                return true;
            }

            // Remove last entry if list has grown too long
            if (this.MaxMruEntryCount <= this.Entries.Count)
                this.Entries.RemoveAt(this.Entries.Count - 1);

            this.Entries.Add(pathFile, new MRUEntryViewModel(pathFile));

            return true;
        }

        /// <summary>
        /// Method adds an entry if the given path was not available until now.
        /// Otherwise, elements properties (IsPinned) are updated only.
        /// </summary>
        /// <param name="emp"></param>
        public void UpdateEntry(MRUEntryViewModel emp)
        {
            if (emp == null)
                return;

            var pathFile = PathModel.NormalizePath(emp.File.Path);

            if (this.Entries.Contains(pathFile) == true)
            {
                MRUEntryViewModel obj;
                this.Entries.TryGetValue(pathFile, out obj);

                obj.SetIsPinned(emp.IsPinned);
                obj.SetLastUpdate(emp.LastUpdate);

                return;
            }

            // Remove last entry if list has grown too long
            if (this.MaxMruEntryCount <= this.Entries.Count)
                this.Entries.RemoveAt(this.Entries.Count - 1);

            this.Entries.Add(pathFile, new MRUEntryViewModel(emp));
        }

        /// <summary>
        /// Remove any entry (whether its pinned or not) based on a viewmodel item.
        /// </summary>
        /// <param name="mruEntry"></param>
        /// <returns></returns>
        public bool RemovePinEntry(MRUEntryViewModel mruEntry)
        {
            try
            {
                if (mruEntry == null)
                    return false;

                this.RemoveEntry(mruEntry.PathFileName);

                return true;
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Removes all items that can be identified with this key.
        /// </summary>
        /// <param name="key"></param>
        public void RemoveEntry(string key)
        {
            var pathFile = new PathModel(key, FSItemType.File);

            this.Entries.Remove(key);
        }

        /// <summary>
        /// Removes all items in the collection.
        /// </summary>
        public void Clear()
        {
            this.Entries.Clear();
        }

        /// <summary>
        /// Removes all items in the collection belongong to this group.
        /// </summary>
        /// <param name="groupType"></param>
        public void Remove(GroupType groupType)
        {
            var list = this.Entries.Where(x => x.Value.GroupItem.Group == groupType).ToList();

            foreach (var item in list)
                this.Entries.Remove(item);
        }

        /// <summary>
        /// Removes all items that or older than the given <see cref="DateTime"/> value.
        /// Items with IsPinned = True are not removed.
        /// </summary>
        /// <param name="removeOlderThanThis"></param>
        public void Remove(DateTime removeOlderThanThis)
        {
            var list = this.Entries.Where(x => x.Value.LastUpdate <= removeOlderThanThis && x.Value.IsPinned == false).ToList();

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
        public void RemoveOlderThanThis(GroupType group)
        {
            if (group == GroupType.IsPinned)
            {
                RemovePinnedItems(false);
                return;
            }

            var model = new TimeSpanModelList();

            GroupTimeSpanModel timeSpanModel = null;
            if (model.List.TryGetValue(group, out timeSpanModel))
            {
                Remove(timeSpanModel.MinTime);
            }
        }

        /// <summary>
        /// Removes all items that pinned or all items that are not pinned
        /// depending on the value in <paramref name="isPinned"/>.
        /// </summary>
        /// <param name="isPinned">All pinned items are removed, if this is true.
        /// Otherwise, all items not pinned are removed</param>
        public void RemovePinnedItems(bool isPinned)
        {
            var list = this.Entries.Where(x => x.Value.IsPinned == isPinned).ToList();

            foreach (var item in list)
                this.Entries.Remove(item);
        }
        #endregion AddRemoveEntries


        /// <summary>
        /// Finds an MRU entry by the file reference an returns it or null.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public MRUEntryViewModel FindMRUEntry(string filePath)
        {
            var pathFile = PathModel.NormalizePath(filePath);

            MRUEntryViewModel obj;
            this.Entries.TryGetValue(pathFile, out obj);

            return obj;
        }
        #endregion methods
        }
}
