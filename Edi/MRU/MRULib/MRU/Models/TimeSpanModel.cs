namespace MRULib.MRU.Models
{
    using MRULib.MRU.Enums;
    using System;

    internal class GroupTimeSpanModel
    {
        public GroupTimeSpanModel(GroupType group)
        {
            // Evaluates to the current day at midnight 00:00:00
            var now = DateTime.Now.Date;

            switch (group)
            {
                case GroupType.IsPinned:
                    MaxTime = GroupTimeSpanModel.IsPinnedMaxTime;
                    MinTime = GroupTimeSpanModel.IsPinnedMinTime;
                    break;

                case GroupType.Today:
                    MaxTime = GroupTimeSpanModel.TodayMaxTime;
                    MinTime = GroupTimeSpanModel.TodayMinTime;
                    break;
                case GroupType.Yesterday:
                    MaxTime = GroupTimeSpanModel.YesterdayMaxTime;
                    MinTime = GroupTimeSpanModel.YesterdayMinTime;
                    break;
                case GroupType.ThisWeek:
                    MaxTime = GroupTimeSpanModel.ThisWeekMaxTime;
                    MinTime = GroupTimeSpanModel.ThisWeekMinTime;
                    break;
                case GroupType.LastWeek:
                    MaxTime = GroupTimeSpanModel.LastWeekMaxTime;
                    MinTime = GroupTimeSpanModel.LastWeekMinTime;
                    break;
                case GroupType.ThisMonth:
                    MaxTime = GroupTimeSpanModel.ThisMonthMaxTime;
                    MinTime = GroupTimeSpanModel.ThisMonthMinTime;
                    break;
                case GroupType.LastMonth:
                    MaxTime = GroupTimeSpanModel.LastMonthMaxTime;
                    MinTime = GroupTimeSpanModel.LastMonthMinTime;
                    break;
                case GroupType.Older:
                    MaxTime = GroupTimeSpanModel.OlderMaxTime;
                    MinTime = GroupTimeSpanModel.OlderMinTime;
                    break;

                default:
                    throw new NotSupportedException("Enum item not supported:" + group.ToString());
            }

            Group = group;
        }

        #region IsPinned static Properties
        static internal DateTime IsPinnedMaxTime
        {
            get { return DateTime.MaxValue; }
        }

        static internal DateTime IsPinnedMinTime
        {
            get { return DateTime.Now.Date; }
        }
        #endregion IsPinned static Properties

        #region Today static Properties
        static internal DateTime TodayMaxTime
        {
            get { return DateTime.Now; }
        }

        static internal DateTime TodayMinTime
        {
            get { return DateTime.Now.Date; }
        }
        #endregion Today static Properties

        #region Yesterday static Properties
        static internal DateTime YesterdayMaxTime
        {
            get{ return DateTime.Now.Date.Add(new TimeSpan(0, 0, 0, 0, 0)); }
        }

        static internal DateTime YesterdayMinTime
        {
            get { return DateTime.Now.Date.Add(new TimeSpan(-1, 0, 0, 0, 0)); }
        }
        #endregion Yesterday static Properties

        #region ThisWeek static Properties
        static internal DateTime ThisWeekMaxTime
        {
            get { return DateTime.Now.Date.Add(new TimeSpan(0, 0, 0, 0, 0)); }
        }

        static internal DateTime ThisWeekMinTime
        {
            get { return DateTime.Now.Date.Add(new TimeSpan(-7, 0, 0, 0, 0)); }
        }
        #endregion ThisWeek static Properties

        #region LastWeek static Properties
        static internal DateTime LastWeekMaxTime
        {
            get { return DateTime.Now.Date.Add(new TimeSpan(-7, 0, 0, 0, 0)); }
        }

        static internal DateTime LastWeekMinTime
        {
            get { return DateTime.Now.Date.Add(new TimeSpan(-14, 0, 0, 0, 0)); }
        }
        #endregion LastWeek static Properties

        #region ThisMonth static Properties
        static internal DateTime ThisMonthMaxTime
        {
            get { return DateTime.Now.Date.Add(new TimeSpan(0, 0, 0, 0, 0)); }
        }

        static internal DateTime ThisMonthMinTime
        {
            get { return DateTime.Now.Date.Add(new TimeSpan(-30, 0, 0, 0, 0)); }
        }
        #endregion ThisMonth static Properties

        #region LastMonth static Properties
        static internal DateTime LastMonthMaxTime
        {
            get { return DateTime.Now.Date.Add(new TimeSpan(-30, 0, 0, 0, 0)); }
        }

        static internal DateTime LastMonthMinTime
        {
            get { return DateTime.Now.Date.Add(new TimeSpan(-60, 0, 0, 0, 0)); }
        }
        #endregion LastMonth static Properties

        #region Older static Properties
        static internal DateTime OlderMaxTime
        {
            get { return DateTime.Now.Date.Add(new TimeSpan(-60, 0, 0, 0, 0)); }
        }

        static internal DateTime OlderMinTime
        {
            get { return DateTime.MaxValue; }
        }
        #endregion Older static Properties

        /// <summary>
        /// Gets the <see cref="GroupType"/> for this period of time.
        /// </summary>
        public GroupType Group { get; private set; }

        /// <summary>
        /// Gets the minimum time that is considered to belong to this timespan.
        /// </summary>
        public DateTime MinTime { get; private set; }

        /// <summary>
        /// Gets the maximum time that is considered to belong to this timespan.
        /// </summary>
        public DateTime MaxTime { get; private set; }
    }
}
