namespace MRULib.MRU.Models.Persist
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Implements a pojo class for saving/loading data to/from XML.
    /// </summary>
    [Serializable]
    public class MRUEntry
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        public MRUEntry()
        {

        }

        /// <summary>
        /// Parameterized class constructor
        /// </summary>
        public MRUEntry(string pathFileName, bool isPinned, DateTime lastUpdate)
        {
            PathFileName = pathFileName;
            IsPinned = isPinned;
            LastUpdate = lastUpdate;
        }

        /// <summary>
        /// Gets/set path and filename of referenced file.
        /// </summary>
        [XmlAttribute(AttributeName = "PathFileName")]
        public string PathFileName { get; set; }

        /// <summary>
        /// Gets/set whether the file reference is pinned or not.
        /// </summary>
        [XmlAttribute(AttributeName = "IsPinned")]
        public bool IsPinned { get; set; }

        /// <summary>
        /// Gets/set the time of the last update for this file reference.
        /// </summary>
        [XmlAttribute(AttributeName = "LastUpdate")]
        public DateTime LastUpdate { get; set; }
    }
}
