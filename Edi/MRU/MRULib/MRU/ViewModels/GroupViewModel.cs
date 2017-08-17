namespace MRULib.MRU.ViewModels
{
    using MRULib.MRU.Enums;
    using System;

    /// <summary>
    /// Implements a viewmodel that descripes a group though an enumerated (keyed)
    /// value and a corresponding string value for output in UI bindings.
    /// </summary>
    public class GroupViewModel : Base.BaseViewModel
    {
        private GroupType _Group;

        #region constructor
        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="group"></param>
        public GroupViewModel(GroupType group)
            : this()
        {
        }

        /// <summary>
        /// class constructor
        /// </summary>
        protected GroupViewModel()
        {
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Gets the group string for display in UI in dependence
        /// of the current group key (enumerated value).
        /// </summary>
        public string GroupName
        {
            get
            {
                switch (Group)
                {
                    case GroupType.IsPinned:
                        return "Pinned";

                    case GroupType.Today:
                        return "Today";

                    case GroupType.Yesterday:
                        return "Yesterday";

                    case GroupType.ThisWeek:
                        return "This Week";

                    case GroupType.LastWeek:
                        return "Last Week";

                    case GroupType.ThisMonth:
                        return "This Month";

                    case GroupType.LastMonth:
                        return "Last Month";
                }

                return "Older";
            }
        }

        /// <summary>
        /// Gets the current Group (today, yesterday, etc.).
        /// </summary>
        public GroupType Group
        {
            get
            {
                return _Group;
            }

            private set
            {
                if (value != _Group)
                {
                    _Group = value;

                    this.RaisePropertyChanged(() => Group);
                    this.RaisePropertyChanged(() => GroupName);
                }
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Sets the group enumeration key for this group object.
        /// The corresponding name will update automatically.
        /// </summary>
        /// <param name="groupType"></param>
        public void SetGroup(GroupType groupType)
        {
            Group = groupType;
        }
        #endregion methods
    }
}
