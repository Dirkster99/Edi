namespace FileSystemModels.Models
{
    using FileSystemModels.Interfaces;
    using FileSystemModels.Models.FSItems.Base;
    using System;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// Class implements an object that handles the user profile settings
    /// of the explorer component. The user profile settings are typically
    /// settings that change in every session and are therefore stored and
    /// retrieved on EACH application start and shut-down.
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "ExplorerUserProfile", IsNullable = true)]
    public class ExplorerUserProfile : IXmlSerializable
    {
        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public ExplorerUserProfile()
        {
            this.CurrentPath = PathFactory.Create(@"C:\", FSItemType.Folder);
            this.CurrentFilter = null;
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Gets/sets the currently viewed path.
        /// Use this property to save/re-restore data when the application
        /// starts or shutsdown.
        /// </summary>
        public IPathModel CurrentPath { get; set; }

        /// <summary>
        /// Sets the currently viewed path.
        /// Use this property to save/re-restore data when the application
        /// starts or shutsdown.
        /// </summary>
        public FilterItemModel CurrentFilter { get; set; }
        #endregion properties

        #region methods
        /// <summary>
        /// Resets the currently viewed path to the path indicated by <paramref name="path"/>.
        /// </summary>
        /// <param name="path"></param>
        public void SetCurrentPath(string path)
        {
            this.CurrentPath = PathFactory.Create(path, FSItemType.Folder);
        }

        #region IXmlSerializable
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();

            while (reader.NodeType == System.Xml.XmlNodeType.Whitespace)
                reader.Read();

            if (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                try
                {
                    var path = reader.GetAttribute("Path");
                    CurrentPath = PathFactory.Create(path);
                }
                catch
                {
                    CurrentPath = PathFactory.Create("C:\\");
                }
            }

            reader.ReadStartElement("CurrentPath");

            while (reader.NodeType == System.Xml.XmlNodeType.Whitespace)
                reader.Read();

            // Read current filter settings
            var deserializer = new XmlSerializer(typeof(FilterItemModel));
            CurrentFilter = (FilterItemModel)deserializer.Deserialize(reader);
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("CurrentPath");
            writer.WriteAttributeString("Path", CurrentPath.Path);
            writer.WriteEndElement();

            // Write current filter settings
            var serializer = new XmlSerializer(typeof(FilterItemModel));
            serializer.Serialize(writer, CurrentFilter);
        }
        #endregion IXmlSerializable
        #endregion methods
    }
}
