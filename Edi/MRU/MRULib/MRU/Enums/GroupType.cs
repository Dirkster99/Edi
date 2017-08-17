namespace MRULib.MRU.Enums
{
    /// <summary>
    /// Enum is used to Group a file reference into a time span group.
    /// The integer value assigned to each member controls the sort order in the display.
    /// </summary>
    public enum GroupType
    {
        /// <summary>
        /// Items is pinned (bookmarked) to the top of the list.
        /// </summary>
        IsPinned = 1,

        /// <summary>
        /// The last acces to this item was today.
        /// </summary>
        Today = 20,

        /// <summary>
        /// The last acces to this item was yesterday.
        /// </summary>
        Yesterday = 21,

        /// <summary>
        /// The last acces to this item is within this week.
        /// </summary>
        ThisWeek = 30,

        /// <summary>
        /// The last acces to this item is within last month.
        /// </summary>
        LastWeek = 31,

        /// <summary>
        /// The last acces to this item is within this month.
        /// </summary>
        ThisMonth = 40,

        /// <summary>
        /// The last acces to this item is with last month.
        /// </summary>
        LastMonth = 41,

        /// <summary>
        /// The last acces to this item is older than last month.
        /// </summary>
        Older = 100
    }
}
