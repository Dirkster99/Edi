namespace MRULib.MRU.ViewModels
{
    using MRULib.MRU.Enums;
    using MRULib.MRU.Models;
    using System;
    using System.Globalization;

    /// <summary>
    /// Implements a viewmodel that can be used to list all recently used files.
    /// </summary>
    public class MRUEntryViewModel : Base.BaseViewModel
    {
        #region fields
        PathModel _File;
        private bool _IsPinned;
        private DateTime _LastUpdate;

        private readonly GroupViewModel _GroupItem;
        #endregion fields

        #region constructor
        /// <summary>
        /// Parameterized standard constructor
        /// </summary>
        /// <param name="pathFileName"></param>
        /// <param name="isPinned"></param>
        public MRUEntryViewModel(string pathFileName
                               , bool isPinned = false)
                : this(pathFileName, DateTime.Now, isPinned)
            {
            }

        /// <summary>
        /// Parameterized standard constructor
        /// </summary>
        /// <param name="pathFileName"></param>
        /// <param name="lastUpdate"></param>
        /// <param name="isPinned"></param>
        public MRUEntryViewModel(string pathFileName
                                , DateTime lastUpdate
                                , bool isPinned = false)
            : this()
        {
            this.File = new PathModel(pathFileName, FSItemType.File);

            this.SetIsPinned(isPinned);
            this.SetLastUpdate(lastUpdate);
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        public MRUEntryViewModel(MRUEntryViewModel copyFrom)
                : this()
        {
            if (copyFrom == null) return;

            this.LastUpdate = copyFrom.LastUpdate;
            this.File = new PathModel(copyFrom.File);
            this.IsPinned = copyFrom.IsPinned;
            this.GroupItem.SetGroup(copyFrom.GroupItem.Group);
        }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        protected MRUEntryViewModel()
        {
            _File = null;
            _IsPinned = false;
            _LastUpdate = DateTime.Now;

            _GroupItem = new GroupViewModel(GroupType.Today);
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Gets the pathmodel stored in this viewmodel.
        /// </summary>
        public PathModel File
        {
            get
            {
                return _File;
            }

            private set
            {
                if (value != _File)
                {
                    _File = value;
                    this.RaisePropertyChanged(() => PathFileName);
                    this.RaisePropertyChanged(() => File);
                }
            }
        }

        /// <summary>
        /// Gets the path and file name of the referenced file.
        /// </summary>
        public string PathFileName
        {
            get
            {
                if (_File != null)
                    return _File.Path;

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the path and file name of the referenced file in a Uri
        /// object using the file:/// Uri syntax convention.
        /// </summary>
        public string PathfileNameUri
        {
            get
            {
                try
                {
                    var path = PathModel.NormalizePath(PathFileName);
                    return "file:///" + path;
                }
                catch
                {
                    return "file:///C:\\";
                }
            }
        }

        /// <summary>
        /// Gets the fact whether this item is currently pinned in the list or not.
        /// </summary>
        public bool IsPinned
        {
            get
            {
                return _IsPinned;
            }

            // See SetIsPinned() method in this class
            private set
            {
                if (value != _IsPinned)
                {
                    _IsPinned = value;
                    this.RaisePropertyChanged(() => IsPinned);
                }
            }
        }

        /// <summary>
        /// Gets the path and file name of the referenced file.
        /// </summary>
        public DateTime LastUpdate
        {
            get
            {
                return _LastUpdate;
            }

            // See SetLastUpdate() method in this class
            private set
            {
                if (_LastUpdate != value)
                {
                    _LastUpdate = value;
                    this.RaisePropertyChanged(() => LastUpdate);
                }
            }
        }

        /// <summary>
        /// Gets the Group (today, yesterday, etc.) for the current LastUpdate
        /// with reference to <see cref="DateTime.Now"/> or IsPinned if IsPinned is true.
        /// </summary>
        public GroupViewModel GroupItem
        {
            get
            {
                return _GroupItem;
            }
        }

        /// <summary>
        /// Gets a shortend path (with ellipses if necessary) for output in UI components.
        /// </summary>
        public string DisplayPathFileName
        {
            get
            {
                int n = 32;
                return (this.PathFileName.Length > n ? this.PathFileName.Substring(0, 3) +
                        "... " + this.PathFileName.Substring(this.PathFileName.Length - n)
                        : this.PathFileName);
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Method updates the <see cref="GroupItem"/> property with regard to the
        /// gap between the current system time and the time stored in
        /// <see cref="LastUpdate"/> property.
        /// </summary>
        public void UpdateGroup()
        {
            if (this.IsPinned == true)
            {
                GroupItem.SetGroup(GroupType.IsPinned);
            }
            else
            {
                if (this.LastUpdate >= GroupTimeSpanModel.TodayMinTime)
                    GroupItem.SetGroup(GroupType.Today);
                else
                {
                    if (this.LastUpdate >= GroupTimeSpanModel.YesterdayMinTime)
                        GroupItem.SetGroup(GroupType.Yesterday);
                    else
                    {
                        if (this.LastUpdate >= GroupTimeSpanModel.ThisWeekMinTime)
                            GroupItem.SetGroup(GroupType.ThisWeek);
                        else
                        {
                            if (this.LastUpdate >= GroupTimeSpanModel.LastWeekMinTime)
                                GroupItem.SetGroup(GroupType.LastWeek);
                            else
                            {
                                if (this.LastUpdate >= GroupTimeSpanModel.ThisMonthMinTime)
                                    GroupItem.SetGroup(GroupType.ThisMonth);
                                else
                                {
                                    if (this.LastUpdate >= GroupTimeSpanModel.LastMonthMinTime)
                                        GroupItem.SetGroup(GroupType.LastMonth);
                                    else
                                    {
                                        GroupItem.SetGroup(GroupType.Older);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the <see cref="IsPinned"/> property to true or false.
        /// </summary>
        /// <param name="isPinned"></param>
        public void SetIsPinned(bool isPinned)
        {
            this.IsPinned = isPinned;
            UpdateGroup();
        }

        /// <summary>
        /// Sets the <see cref="LastUpdate"/> property to the specified value.
        /// </summary>
        /// <param name="lastUpdate"></param>
        public void SetLastUpdate(DateTime lastUpdate)
        {
            this.LastUpdate = lastUpdate;
            UpdateGroup();
        }

        /// <summary>
        /// Standard ToString() method overide implementation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Path {0}, IsPinned:{1}, LastUpdate:{2}",
                   (this.PathFileName == null ? "(null)" : this.File.Path),
                   this.IsPinned, LastUpdate);
        }
        #endregion methods
    }
}
