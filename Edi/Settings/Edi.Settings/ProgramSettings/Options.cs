namespace Edi.Settings.ProgramSettings
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    using FileSystemModels.Models;
    using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.Highlighting.Themes;
    using Interfaces;
    using Themes;
    using Themes.Interfaces;

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
            mThemesManager = themesManager;
        }

        /// <summary>
        /// Hidden class Constructor
        /// </summary>
        protected Options()
        {
            mThemesManager = null;

            EditorTextOptions = new TextEditorOptions();
            mWordWarpText = false;

            mDocumentZoomUnit = ZoomUnit.Percentage;     // Zoom View in Percent
            mDocumentZoomView = 100;                     // Font Size 12 is 100 %

            mReloadOpenFilesOnAppStart = true;
            mRunSingleInstance = true;
            mCurrentTheme = ThemesManager.DefaultThemeName;
            mLanguageSelected = DefaultLocal;

            HighlightOnFileNew = true;
            FileNewDefaultFileName = Util.Local.Strings.STR_FILE_DEFAULTNAME;
            FileNewDefaultFileExtension = ".txt";

            ExplorerSettings = new ExplorerSettingsModel(true);

            mIsDirty = false;
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

            mThemesManager = copyThis.mThemesManager;
            EditorTextOptions = copyThis.EditorTextOptions;
            mWordWarpText = copyThis.mWordWarpText;

            mDocumentZoomUnit = copyThis.mDocumentZoomUnit;     // Zoom View in Percent
            mDocumentZoomView = copyThis.mDocumentZoomView;     // Font Size 12 is 100 %

            mReloadOpenFilesOnAppStart = copyThis.mReloadOpenFilesOnAppStart;
            mRunSingleInstance = copyThis.mRunSingleInstance;
            mCurrentTheme = copyThis.mCurrentTheme;
            mLanguageSelected = copyThis.mLanguageSelected;

            mIsDirty = copyThis.mIsDirty;
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
                return mWordWarpText;
            }

            set
            {
                if (mWordWarpText != value)
                {
                    mWordWarpText = value;
                    IsDirty = true;
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
                return mDocumentZoomView;
            }

            set
            {
                if (mDocumentZoomView != value)
                {
                    mDocumentZoomView = value;
                    IsDirty = true;
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
                return mDocumentZoomUnit;
            }

            set
            {
                if (mDocumentZoomUnit != value)
                {
                    mDocumentZoomUnit = value;
                    IsDirty = true;
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
                return mReloadOpenFilesOnAppStart;
            }

            set
            {
                if (mReloadOpenFilesOnAppStart != value)
                {
                    mReloadOpenFilesOnAppStart = value;
                    IsDirty = true;
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
                return mRunSingleInstance;
            }

            set
            {
                if (mRunSingleInstance != value)
                {
                    mRunSingleInstance = value;
                    IsDirty = true;
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
                return mCurrentTheme;
            }

            set
            {
                if (mCurrentTheme != value)
                {
                    mCurrentTheme = value;
                    IsDirty = true;
                }
            }
        }

        [XmlElement("LanguageSelected")]
        public string LanguageSelected
        {
            get
            {
                return mLanguageSelected;
            }

            set
            {
                if (mLanguageSelected != value)
                {
                    mLanguageSelected = value;
                    IsDirty = true;
                }
            }
        }

        #region HTML Export
        [XmlElement("TextToHTML_ShowLineNumbers")]
        public bool TextToHTML_ShowLineNumbers
        {
            get
            {
                return mTextToHTML_ShowLineNumbers;
            }

            set
            {
                if (mTextToHTML_ShowLineNumbers != value)
                {
                    mTextToHTML_ShowLineNumbers = value;
                    IsDirty = true;
                }
            }
        }

        [XmlElement("TextToHTML_AlternateLineBackground")]
        public bool TextToHTML_AlternateLineBackground
        {
            get
            {
                return mTextToHTML_TextToHTML_AlternateLineBackground;
            }

            set
            {
                if (mTextToHTML_TextToHTML_AlternateLineBackground != value)
                {
                    mTextToHTML_TextToHTML_AlternateLineBackground = value;
                    IsDirty = true;
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
                return mIsDirty;
            }

            set
            {
                if (mIsDirty != value)
                    mIsDirty = value;
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

            unitList.Add(new UnitComboLib.Models.ListItem(Itemkey.ScreenPercent, Util.Local.Strings.STR_SCALE_VIEW_PERCENT, Util.Local.Strings.STR_SCALE_VIEW_PERCENT_SHORT, percentDefaults));
            unitList.Add(new UnitComboLib.Models.ListItem(Itemkey.ScreenFontPoints, Util.Local.Strings.STR_SCALE_VIEW_POINT, Util.Local.Strings.STR_SCALE_VIEW_POINT_SHORT, pointsDefaults));

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
            return mThemesManager.GetTextEditorHighlighting(hlThemeName);
        }

        /// <summary>
        /// Reset the dirty flag (e.g. after saving program options when they where edit).
        /// </summary>
        /// <param name="flag"></param>
        internal void SetDirtyFlag(bool flag)
        {
            IsDirty = flag;
        }
        #endregion methods
    }
}
