namespace MRULib.MRU.Models.Persist
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Implements a pojo class for saving/loading listed data to/from XML.
    /// </summary>
    [Serializable]
    public class MRUList
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        public MRUList()
        {
            MaxMruEntryCount = 45;
            this.ListOfMRUEntries = new List<MRUEntry>();
        }

        /// <summary>
        /// Gets/sets the maximum number of entries hosted in the list.
        /// </summary>
        public int MaxMruEntryCount { get; set; }

        /// <summary>
        /// Gets/sets the lsit of MRU entries.
        /// </summary>
        public List<MRUEntry> ListOfMRUEntries { get; set; }
    }
}
