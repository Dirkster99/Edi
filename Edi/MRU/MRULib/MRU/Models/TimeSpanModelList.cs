namespace MRULib.MRU.Models
{
    using MRULib.MRU.Enums;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// List all time span max and min values between now and older.
    /// </summary>
    internal class TimeSpanModelList
    {
        private readonly Dictionary<GroupType, GroupTimeSpanModel> _list = null;

        /// <summary>
        /// Class constructor.
        /// </summary>
        public TimeSpanModelList()
        {
            _list = new Dictionary<GroupType, GroupTimeSpanModel>();

            foreach (var item in Enum.GetValues(typeof(GroupType)))
            {
                var timeSpan = new GroupTimeSpanModel((GroupType)item);
                _list.Add(timeSpan.Group, timeSpan);
            }
        }

        /// <summary>
        /// Gets a list of min max time spans relevant for each enumerated group.
        /// </summary>
        internal Dictionary<GroupType, GroupTimeSpanModel> List
        {
            get
            {
                return _list;
            }
        }
    }
}
