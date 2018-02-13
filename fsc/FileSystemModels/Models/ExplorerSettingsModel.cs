namespace FileSystemModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Class implements a low level model of the explorer settings that can
    /// stored (up exit of application) and retrieved (upon re-start of application).
    ///
    /// This class is serializable and is a parameter of the
    /// IConfigExplorerSettings methods.
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "ExplorerSettings", IsNullable = true)]
    public class ExplorerSettingsModel
    {
        #region fields
        private readonly List<CustomFolderItemModel> mSpecialFolders = null;

        private ExplorerUserProfile mUserProfile = null;

        private List<string> mRecentFolders = null;
        private List<FilterItemModel> mFilterCollection = null;
        private string mLastSelectedRecentFolder;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor with default value initialization.
        /// </summary>
        /// <param name="createDefaultSettings"></param>
        public ExplorerSettingsModel(bool createDefaultSettings)
        : this()
        {
            if (createDefaultSettings == true)
                this.CreateDefaultSettings();
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public ExplorerSettingsModel()
        {
            this.mUserProfile = new ExplorerUserProfile();

            this.mRecentFolders = new List<string>();

            this.mFilterCollection = new List<FilterItemModel>();

            this.ShowFolders = this.ShowHiddenFiles = this.ShowIcons = true;

            this.mSpecialFolders = this.CreateSpecialFolderCollection();
            this.ShowSpecialFoldersInTreeBrowser = false;
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Gets/Sets user session specific settings that are
        /// stored and loaded on each application shutdown/re-start.
        /// 
        /// User profile should be persisted seperately, therefore, its ignored here.
        /// </summary>
        [XmlIgnore]
        public ExplorerUserProfile UserProfile
        {
            get
            {
                return this.mUserProfile;
            }

            private set
            {
                this.mUserProfile = value;
            }
        }

        /// <summary>
        /// Gets/sets a list of recent folders. Recent folders are
        /// folder shortcuts that users can use frequently to navigate
        /// there with 1 or 2 mouse clicks.
        /// </summary>
        [XmlArray("RecentFolders")]
        public List<string> RecentFolders
        {
            get
            {
                return this.mRecentFolders;
            }

            set
            {
                if (this.mRecentFolders != value)
                    this.mRecentFolders = value;
            }
        }

        /// <summary>
        /// The last recent folder is the folder that was most recently
        /// used for navigation through the list of recent folders.
        /// 
        /// This folder can be presented to the user in a 1 click fashion
        /// since its likely that it may be visited again in the next user session.
        /// </summary>
        [XmlAttribute("LastSelectedRecentFolder")]
        public string LastSelectedRecentFolder
        {
            get
            {
                return this.mLastSelectedRecentFolder;
            }

            set
            {
                if (this.mLastSelectedRecentFolder != value)
                    this.mLastSelectedRecentFolder = value;
            }
        }

        #region filter settings
        /// <summary>
        /// Gets/sets the collection of file filter items. One of these
        /// filter items can be applied in a file list view to filter for
        /// file names with a certain pattern (eg: '*.cs')
        /// </summary>
        [XmlElement(ElementName = "FilterCollection")]
        public List<FilterItemModel> FilterCollection
        {
            get
            {
                return this.mFilterCollection;
            }

            set
            {
                if (this.mFilterCollection != value)
                    this.mFilterCollection = value;
            }
        }
        #endregion filter settings

        /// <summary>
        /// Gets/sets whether the display of files and folders includes
        /// icons or not.
        /// </summary>
        [XmlAttribute(AttributeName = "ShowIcons")]
        public bool ShowIcons { get; set; }

        /// <summary>
        /// Gets/sets whether the display of files includes folders or not.
        /// </summary>
        [XmlAttribute(AttributeName = "ShowFolders")]
        public bool ShowFolders { get; set; }

        /// <summary>
        /// Gets/sets whether the display of files and folders includes
        /// hidden files and folders or not.
        /// </summary>
        [XmlAttribute(AttributeName = "ShowHiddenFiles")]
        public bool ShowHiddenFiles { get; set; }

        /// <summary>
        /// Gets/sets whether the the display of files
        /// is currently filtered or not.
        /// </summary>
        [XmlAttribute(AttributeName = "IsFiltered")]
        public bool IsFiltered { get; set; }

        /// <summary>
        /// Gets the (constant) list of special folders
        /// users can visit in 1 or 2 mouse clicks.
        /// </summary>
        [XmlIgnore]
        public IEnumerable<CustomFolderItemModel> SpecialFolders
        {
            get
            {
                return this.mSpecialFolders;
            }
        }

        /// <summary>
        /// Gets whether special folder shortcuts are shown within
        /// the tree view browser. This is currently disabled and always
        /// set to false because Special Folders are shown elsewhere within
        /// the Explorer (TW).
        /// </summary>
        [XmlIgnore]
        public bool ShowSpecialFoldersInTreeBrowser { get; private set; }
        #endregion properties

        #region methods
        /// <summary>
        /// Compares 2 setting models and returns true if they are equal
        /// (data is same between both models) or otherwise false.
        /// 
        /// Attention: The UserProfile data property is ignored here.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="settings"></param>
        /// <returns>false if both collections differ, otherwise true</returns>
        public static bool CompareSettings(ExplorerSettingsModel input,
                                           ExplorerSettingsModel settings)
        {
            if ((input == null && settings != null) || (input != null && settings == null))
                return false;

            // Reference to same object
            if (input == settings)
                return true;

            // Compare file filter collections and return false if they differ
            // Compare recentfolders collections and return false if they differ
            if ((input.FilterCollection == null && settings.FilterCollection != null) ||
                (input.FilterCollection != null && settings.FilterCollection == null))
                return false;

            if (input.FilterCollection == null && settings.FilterCollection == null)
                return true;

            if (input.FilterCollection.Count != settings.FilterCollection.Count)
                return false;

            foreach (var item in input.FilterCollection)
            {
                var found = settings.FilterCollection.Where(i => i.Equals(item) == true);

                if (found.Count() == 0)
                    return false;
            }

            // Compare recentfolders collections and return false if they differ
            if ((input.RecentFolders == null && settings.RecentFolders != null) ||
                (input.RecentFolders != null && settings.RecentFolders == null))
                return false;

            if (input.RecentFolders == null && settings.RecentFolders == null)
                return true;

            if (input.RecentFolders.Count != settings.RecentFolders.Count)
                return false;

            foreach (var item in input.RecentFolders)
            {
                var found = settings.RecentFolders.Where(i => string.Compare(i, item, true) == 0);

                if (found.Count() == 0)
                    return false;
            }

            if (PathFactory.Compare(input.LastSelectedRecentFolder, settings.LastSelectedRecentFolder) == false)
                return false;

            // Compare Showfolder, Icons, and hidden files options
            if (input.ShowFolders != settings.ShowFolders)
                return false;

            if (input.ShowIcons != settings.ShowIcons)
                return false;

            if (input.ShowHiddenFiles != settings.ShowHiddenFiles)
                return false;

            if (input.IsFiltered != settings.IsFiltered)
                return false;

            return true; // settings are the same
        }

        /// <summary>
        /// Sets the session data for this settings object.
        /// </summary>
        /// <param name="profile"></param>
        public void SetUserProfile(ExplorerUserProfile profile)
        {
            UserProfile = profile;
        }

        /// <summary>
        /// Creates a default collection of special folders (Desktop, MyDocuments, MyMusic, MyVideos).
        /// </summary>
        public List<CustomFolderItemModel> CreateSpecialFolderCollection()
        {
            var specialFolders = new List<CustomFolderItemModel>();

            specialFolders.Add(new CustomFolderItemModel(Environment.SpecialFolder.Desktop));
            specialFolders.Add(new CustomFolderItemModel(Environment.SpecialFolder.MyDocuments));
            specialFolders.Add(new CustomFolderItemModel(Environment.SpecialFolder.MyMusic));
            specialFolders.Add(new CustomFolderItemModel(Environment.SpecialFolder.MyVideos));

            return specialFolders;
        }

        /// <summary>
        /// Adds another special folder into the collection of special folders.
        /// </summary>
        /// <param name="folder"></param>
        public void AddSpecialFolder(Environment.SpecialFolder folder)
        {
            this.mSpecialFolders.Add(new CustomFolderItemModel(folder));
        }

        /// <summary>
        /// Add a recent folder location into the collection of recent folders.
        /// This collection can then be used in the folder combobox drop down
        /// list to store user specific customized folder short-cuts.
        /// </summary>
        /// <param name="folderPath"></param>
        public void AddRecentFolder(string folderPath)
        {
            if ((folderPath = PathFactory.ExtractDirectoryRoot(folderPath)) == null)
                return;

            this.mRecentFolders.Add(folderPath);
        }

        /// <summary>
        /// Removes a recent folder location into the collection of recent folders.
        /// This collection can then be used in the folder combobox drop down
        /// list to store user specific customized folder short-cuts.
        /// </summary>
        /// <param name="path"></param>
        public void RemoveRecentFolder(string path)
        {
            if (string.IsNullOrEmpty(path) == true)
                return;

            this.mRecentFolders.Remove(path);
        }

        /// <summary>
        /// Add a filter item into the collection of filters.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pattern"></param>
        /// <param name="bSelectNewFilter"></param>
        public void AddFilter(string name,
                              string pattern,
                              bool bSelectNewFilter = false)
        {
            var newFilter = new FilterItemModel(name, pattern);

            this.mFilterCollection.Add(newFilter);

            if (bSelectNewFilter == true && this.mUserProfile != null)
                this.mUserProfile.SetCurrentFilter(newFilter);
        }

        /// <summary>
        /// Creates default file filter and recent folder settings
        /// for initialization at the very first time (no settings available for load from persistence).
        /// </summary>
        private void CreateDefaultSettings()
        {
            this.ShowFolders = this.ShowHiddenFiles = this.ShowIcons = true;
            this.IsFiltered = false;
            this.UserProfile.SetCurrentFilter(new FilterItemModel("Text files", "*.txt"));

            this.LastSelectedRecentFolder = @"C:\temp\";

            this.AddRecentFolder(@"C:\temp\");
            this.AddRecentFolder(@"C:\windows\");

            this.AddFilter("XML", "*.aml;*.xml;*.xsl;*.xslt;*.xsd;*.config;*.addin;*.wxs;*.wxi;*.wxl;*.build;*.xfrm;*.targets;*.xpt;*.xft;*.map;*.wsdl;*.disco;*.ps1xml;*.nuspec");
            this.AddFilter("C#", "*.cs;*.manifest;*.resx;*.xaml");
            this.AddFilter("Edi", "*.xshd");
            this.AddFilter("JavaScript", "*.js");
            this.AddFilter("HTML", "*.htm;*.html");
            this.AddFilter("ActionScript3", "*.as");
            this.AddFilter("ASP/XHTML", "*.asp;*.aspx;*.asax;*.asmx;*.ascx;*.master");
            this.AddFilter("Boo", "*.boo");
            this.AddFilter("Coco", "*.atg");
            this.AddFilter("C++", "*.c;*.h;*.cc;*.cpp;*.hpp;*.rc");
            this.AddFilter("CSS", "*.css");
            this.AddFilter("BAT", "*.bat;*.dos");
            this.AddFilter("F#", "*.fs");
            this.AddFilter("INI", "*.cfg;*.conf;*.ini;*.iss;");
            this.AddFilter("Java", "*.java");
            this.AddFilter("Scheme", "*.sls;*.sps;*.ss;*.scm");
            this.AddFilter("LOG", "*.log");
            this.AddFilter("MarkDown", "*.md");
            this.AddFilter("Patch", "*.patch;*.diff");
            this.AddFilter("PowerShell", "*.ps1;*.psm1;*.psd1");
            this.AddFilter("Projects", "*.proj;*.csproj;*.drproj;*.vbproj;*.ilproj;*.booproj");
            this.AddFilter("Python", "*.py");
            this.AddFilter("Ruby", "*.rb");
            this.AddFilter("Scheme", "*.sls;*.sps;*.ss;*.scm");
            this.AddFilter("StyleCop", "*.StyleCop");
            this.AddFilter("SQL", "*.sql");
            this.AddFilter("Squirrel", "*.nut");
            this.AddFilter("Tex", "*.tex");
            this.AddFilter("Text files", "*.txt");
            this.AddFilter("VBNET", "*.vb");
            this.AddFilter("VTL", "*.vtl;*.vm");
            this.AddFilter("All Files", "*.*");
        }
        #endregion methods
    }
}
