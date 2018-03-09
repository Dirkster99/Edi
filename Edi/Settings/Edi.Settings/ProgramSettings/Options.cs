namespace Edi.Settings.ProgramSettings
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    using FileSystemModels.Models;
    using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.Highlighting.Themes;
    using Edi.Settings.Interfaces;
    using Edi.Themes;
    using Edi.Themes.Interfaces;
    using UnitComboLib.Models.Unit;

    /// <summary>
    /// Determine whether Zoom units of the text editor
    /// are displayed in percent or font related points.
    /// </summary>
    public enum ZoomUnit
    {
        Percentage = 0,
        Points = 1
    }

    /// <summary>
    /// This class implements the model of the programm settings part
    /// of the application. Typically, users have options that they want
    /// to set or reset durring the live time of an application. This
    /// class organizes these options and is responsible for their
    /// storage (when being changed) and retrieval at program start-up.
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Options", IsNullable = false)]
    public class Options : IOptions
    {
        #region fields
        public const string DefaultLocal = "en-US";

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool mWordWarpText;
        private int mDocumentZoomView;
        private ZoomUnit mDocumentZoomUnit;

        private bool mReloadOpenFilesOnAppStart;
        private bool mRunSingleInstance;

        private string mCurrentTheme;

        private string mLanguageSelected;
        private bool mIsDirty = false;

        private bool mTextToHTML_ShowLineNumbers = true;                        // Text to HTML export
        private bool mTextToHTML_TextToHTML_AlternateLineBackground = true;

        private readonly IThemesManager mThemesManager = null;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor from <paramref name="themesManager"/> parameter.
        /// </summary>
        /// <param name="themesManager"></param>
        public Options(IThemesManager themesManager)
            : this()
        {
            this.mThemesManager = themesManager;
        }

        /// <summary>
        /// Hidden class Constructor
        /// </summary>
        protected Options()
        {
            this.mThemesManager = null;

            this.EditorTextOptions = new TextEditorOptions();
            this.mWordWarpText = false;

            this.mDocumentZoomUnit = ZoomUnit.Percentage;     // Zoom View in Percent
            this.mDocumentZoomView = 100;                     // Font Size 12 is 100 %

            this.mReloadOpenFilesOnAppStart = true;
            this.mRunSingleInstance = true;
            this.mCurrentTheme = Edi.Themes.Factory.DefaultThemeName;
            this.mLanguageSelected = Options.DefaultLocal;

            this.HighlightOnFileNew = true;
            this.FileNewDefaultFileName = Edi.Util.Local.Strings.STR_FILE_DEFAULTNAME;
            this.FileNewDefaultFileExtension = ".txt";

            this.ExplorerSettings = new ExplorerSettingsModel(true);

            this.mIsDirty = false;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="copyThis"></param>
        public Options(Options copyThis)
            : this()
        {
            if (copyThis == null)
                return;

            this.mThemesManager = copyThis.mThemesManager;
            this.EditorTextOptions = copyThis.EditorTextOptions;
            this.mWordWarpText = copyThis.mWordWarpText;

            this.mDocumentZoomUnit = copyThis.mDocumentZoomUnit;     // Zoom View in Percent
            this.mDocumentZoomView = copyThis.mDocumentZoomView;     // Font Size 12 is 100 %

            this.mReloadOpenFilesOnAppStart = copyThis.mReloadOpenFilesOnAppStart;
            this.mRunSingleInstance = copyThis.mRunSingleInstance;
            this.mCurrentTheme = copyThis.mCurrentTheme;
            this.mLanguageSelected = copyThis.mLanguageSelected;

            this.mIsDirty = copyThis.mIsDirty;
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Get/set whether WordWarp should be applied in editor (by default) or not.
        /// </summary>
        [XmlElement(ElementName = "WordWarpText")]
        public bool WordWarpText
        {
            get
            {
                return this.mWordWarpText;
            }

            set
            {
                if (this.mWordWarpText != value)
                {
                    this.mWordWarpText = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Get/set options that are applicable to the texteditor which is based on AvalonEdit.
        /// </summary>
        public TextEditorOptions EditorTextOptions { get; set; }

        /// <summary>
        /// Percentage Size of data to be viewed by default
        /// </summary>
        [XmlAttribute(AttributeName = "DocumentZoomView")]
        public int DocumentZoomView
        {
            get
            {
                return this.mDocumentZoomView;
            }

            set
            {
                if (this.mDocumentZoomView != value)
                {
                    this.mDocumentZoomView = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Get/set standard size of display for text editor.
        /// </summary>
        [XmlAttribute(AttributeName = "DocumentZoomUnit")]
        public ZoomUnit DocumentZoomUnit
        {
            get
            {
                return this.mDocumentZoomUnit;
            }

            set
            {
                if (this.mDocumentZoomUnit != value)
                {
                    this.mDocumentZoomUnit = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Get/set whether application re-loads files open in last sesssion or not
        /// </summary>
        [XmlAttribute(AttributeName = "ReloadOpenFilesFromLastSession")]
        public bool ReloadOpenFilesOnAppStart
        {
            get
            {
                return this.mReloadOpenFilesOnAppStart;
            }

            set
            {
                if (this.mReloadOpenFilesOnAppStart != value)
                {
                    this.mReloadOpenFilesOnAppStart = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Get/set whether application can be started more than once.
        /// </summary>
        [XmlElement(ElementName = "RunSingleInstance")]
        public bool RunSingleInstance
        {
            get
            {
                return this.mRunSingleInstance;
            }

            set
            {
                if (this.mRunSingleInstance != value)
                {
                    this.mRunSingleInstance = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Get/set WPF theme configured for the complete Application
        /// </summary>
        [XmlElement("CurrentTheme")]
        public string CurrentTheme
        {
            get
            {
                return this.mCurrentTheme;
            }

            set
            {
                if (this.mCurrentTheme != value)
                {
                    this.mCurrentTheme = value;
                    this.IsDirty = true;
                }
            }
        }

        [XmlElement("LanguageSelected")]
        public string LanguageSelected
        {
            get
            {
                return this.mLanguageSelected;
            }

            set
            {
                if (this.mLanguageSelected != value)
                {
                    this.mLanguageSelected = value;
                    this.IsDirty = true;
                }
            }
        }

        #region HTML Export
        [XmlElement("TextToHTML_ShowLineNumbers")]
        public bool TextToHTML_ShowLineNumbers
        {
            get
            {
                return this.mTextToHTML_ShowLineNumbers;
            }

            set
            {
                if (this.mTextToHTML_ShowLineNumbers != value)
                {
                    this.mTextToHTML_ShowLineNumbers = value;
                    this.IsDirty = true;
                }
            }
        }

        [XmlElement("TextToHTML_AlternateLineBackground")]
        public bool TextToHTML_AlternateLineBackground
        {
            get
            {
                return this.mTextToHTML_TextToHTML_AlternateLineBackground;
            }

            set
            {
                if (this.mTextToHTML_TextToHTML_AlternateLineBackground != value)
                {
                    this.mTextToHTML_TextToHTML_AlternateLineBackground = value;
                    this.IsDirty = true;
                }
            }
        }
        #endregion HTML Export

        #region New File Default options
        /// <summary>
        /// Determine whether a file created with File>New should be highlighted or not.
        /// </summary>
        [XmlElement("NewFile_HighlightOnFileNew")]
        public bool HighlightOnFileNew { get; set; }

        /// <summary>
        /// Get/set default name of file that is created via File>New.
        /// </summary>
        [XmlElement("NewFile_DefaultFileName")]
        public string FileNewDefaultFileName { get; set; }

        /// <summary>
        /// Get/set default string of file extension (including '.' character)
        /// that is created via File>New.
        /// </summary>
        [XmlElement("NewFile_DefaultFileExtension")]
        public string FileNewDefaultFileExtension { get; set; }
        #endregion New File Default options

        [XmlElement(ElementName = "ExplorerSettings")]
        public ExplorerSettingsModel ExplorerSettings { get; set; }

        /// <summary>
        /// Get/set whether the settings stored in this instance have been
        /// changed and need to be saved when program exits (at the latest).
        /// </summary>
        [XmlIgnore]
        public bool IsDirty
        {
            get
            {
                return this.mIsDirty;
            }

            set
            {
                if (this.mIsDirty != value)
                    this.mIsDirty = value;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Get a list of all supported languages in Edi.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<LanguageCollection> GetSupportedLanguages()
        {
            List<LanguageCollection> ret = new List<LanguageCollection>();

            ret.Add(new LanguageCollection() { Language = "de", Locale = "DE", Name = "Deutsch (German)" });
            ret.Add(new LanguageCollection() { Language = "en", Locale = "US", Name = "English (English)" });
            ret.Add(new LanguageCollection() { Language = "es", Locale = "ES", Name = "Español (Spanish)" });
            ret.Add(new LanguageCollection() { Language = "fr", Locale = "FR", Name = "Français (French)" });
            ret.Add(new LanguageCollection() { Language = "it", Locale = "IT", Name = "Italiano (Italian)" });
            ret.Add(new LanguageCollection() { Language = "ru", Locale = "RU", Name = "Русский (Russian)" });
            ret.Add(new LanguageCollection() { Language = "id", Locale = "ID", Name = "Bahasa Indonesia(Indonesian)" });
            ret.Add(new LanguageCollection() { Language = "ja", Locale = "JP", Name = "日本語 (Japanese)" });
            ret.Add(new LanguageCollection() { Language = "zh-Hans", Locale = "", Name = "简体中文 (Simplified)" });
            ret.Add(new LanguageCollection() { Language = "pt", Locale = "PT", Name = "Português (Portuguese)" });
            ret.Add(new LanguageCollection() { Language = "hi", Locale = "IN", Name = "हिंदी (Hindi)" });

            return ret;
        }

        /// <summary>
        /// Initialize Scale View with useful units in percent and font point size
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<UnitComboLib.Models.ListItem> GenerateScreenUnitList()
        {
            List<UnitComboLib.Models.ListItem> unitList = new List<UnitComboLib.Models.ListItem>();

            var percentDefaults = new ObservableCollection<string>() { "25", "50", "75", "100", "125", "150", "175", "200", "300", "400", "500" };
            var pointsDefaults = new ObservableCollection<string>() { "3", "6", "8", "9", "10", "12", "14", "16", "18", "20", "24", "26", "32", "48", "60" };

            unitList.Add(new UnitComboLib.Models.ListItem(Itemkey.ScreenPercent, Edi.Util.Local.Strings.STR_SCALE_VIEW_PERCENT, Edi.Util.Local.Strings.STR_SCALE_VIEW_PERCENT_SHORT, percentDefaults));
            unitList.Add(new UnitComboLib.Models.ListItem(Itemkey.ScreenFontPoints, Edi.Util.Local.Strings.STR_SCALE_VIEW_POINT, Edi.Util.Local.Strings.STR_SCALE_VIEW_POINT_SHORT, pointsDefaults));

            return unitList;
        }

        /// <summary>
        /// Check whether the <paramref name="hlThemeName"/> is configured
        /// with a highlighting theme and return it if that is the case.
        /// </summary>
        /// <param name="hlThemeName"></param>
        /// <returns>List of highlighting themes that should be applied for this WPF theme</returns>
        public HighlightingThemes FindHighlightingTheme(string hlThemeName)
        {
            return this.mThemesManager.GetTextEditorHighlighting(hlThemeName);
        }

        /// <summary>
        /// Reset the dirty flag (e.g. after saving program options when they where edit).
        /// </summary>
        /// <param name="flag"></param>
        internal void SetDirtyFlag(bool flag)
        {
            this.IsDirty = flag;
        }
        #endregion methods
    }
}
