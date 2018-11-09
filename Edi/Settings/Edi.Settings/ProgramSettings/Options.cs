namespace Edi.Settings.ProgramSettings
{
    using System;
    using System.Xml.Serialization;
    using FileSystemModels.Models;
    using ICSharpCode.AvalonEdit;
    using Edi.Settings.Interfaces;
    using Edi.Interfaces.Themes;

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
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool _WordWarpText;
        private int _DocumentZoomView;
        private ZoomUnit _DocumentZoomUnit;

        private bool _ReloadOpenFilesOnAppStart;
        private bool _RunSingleInstance;

        private string _CurrentTheme;

        private string _LanguageSelected;
        private bool _IsDirty = false;

        private bool _TextToHTML_ShowLineNumbers = true;                        // Text to HTML export
        private bool _TextToHTML_TextToHTML_AlternateLineBackground = true;

        private readonly IThemesManager _ThemesManager = null;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor from <paramref name="themesManager"/> parameter.
        /// </summary>
        /// <param name="themesManager"></param>
        public Options(IThemesManager themesManager)
            : this()
        {
            this._ThemesManager = themesManager;
            this._CurrentTheme = themesManager.DefaultThemeName;
        }

        /// <summary>
        /// Hidden class Constructor
        /// </summary>
        protected Options()
        {
            this._ThemesManager = null;

            this.EditorTextOptions = new TextEditorOptions();
            this._WordWarpText = false;

            this._DocumentZoomUnit = ZoomUnit.Percentage;     // Zoom View in Percent
            this._DocumentZoomView = 100;                     // Font Size 12 is 100 %

            this._ReloadOpenFilesOnAppStart = true;
            this._RunSingleInstance = true;
            this._CurrentTheme = null;
            this._LanguageSelected = SettingsFactory.DefaultLocal;

            this.HighlightOnFileNew = true;
            this.FileNewDefaultFileName = Edi.Util.Local.Strings.STR_FILE_DEFAULTNAME;
            this.FileNewDefaultFileExtension = ".txt";

            this.ExplorerSettings = new ExplorerSettingsModel(true);

            this._IsDirty = false;
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

            this._ThemesManager = copyThis._ThemesManager;
            this.EditorTextOptions = copyThis.EditorTextOptions;
            this._WordWarpText = copyThis._WordWarpText;

            this._DocumentZoomUnit = copyThis._DocumentZoomUnit;     // Zoom View in Percent
            this._DocumentZoomView = copyThis._DocumentZoomView;     // Font Size 12 is 100 %

            this._ReloadOpenFilesOnAppStart = copyThis._ReloadOpenFilesOnAppStart;
            this._RunSingleInstance = copyThis._RunSingleInstance;
            this._CurrentTheme = copyThis._CurrentTheme;
            this._LanguageSelected = copyThis._LanguageSelected;

            this._IsDirty = copyThis._IsDirty;
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
                return this._WordWarpText;
            }

            set
            {
                if (this._WordWarpText != value)
                {
                    this._WordWarpText = value;
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
                return this._DocumentZoomView;
            }

            set
            {
                if (this._DocumentZoomView != value)
                {
                    this._DocumentZoomView = value;
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
                return this._DocumentZoomUnit;
            }

            set
            {
                if (this._DocumentZoomUnit != value)
                {
                    this._DocumentZoomUnit = value;
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
                return this._ReloadOpenFilesOnAppStart;
            }

            set
            {
                if (this._ReloadOpenFilesOnAppStart != value)
                {
                    this._ReloadOpenFilesOnAppStart = value;
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
                return this._RunSingleInstance;
            }

            set
            {
                if (this._RunSingleInstance != value)
                {
                    this._RunSingleInstance = value;
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
                return this._CurrentTheme;
            }

            set
            {
                if (this._CurrentTheme != value)
                {
                    this._CurrentTheme = value;
                    this.IsDirty = true;
                }
            }
        }

        [XmlElement("LanguageSelected")]
        public string LanguageSelected
        {
            get
            {
                return this._LanguageSelected;
            }

            set
            {
                if (this._LanguageSelected != value)
                {
                    this._LanguageSelected = value;
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
                return this._TextToHTML_ShowLineNumbers;
            }

            set
            {
                if (this._TextToHTML_ShowLineNumbers != value)
                {
                    this._TextToHTML_ShowLineNumbers = value;
                    this.IsDirty = true;
                }
            }
        }

        [XmlElement("TextToHTML_AlternateLineBackground")]
        public bool TextToHTML_AlternateLineBackground
        {
            get
            {
                return this._TextToHTML_TextToHTML_AlternateLineBackground;
            }

            set
            {
                if (this._TextToHTML_TextToHTML_AlternateLineBackground != value)
                {
                    this._TextToHTML_TextToHTML_AlternateLineBackground = value;
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
                return this._IsDirty;
            }

            set
            {
                if (this._IsDirty != value)
                    this._IsDirty = value;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Check whether the <paramref name="hlThemeName"/> is configured
        /// with a highlighting theme and return it if that is the case.
        /// </summary>
        /// <param name="hlThemeName"></param>
        /// <returns>List of highlighting themes that should be applied for this WPF theme</returns>
        public IHighlightingThemes FindHighlightingTheme(string hlThemeName)
        {
            return this._ThemesManager.GetTextEditorHighlighting(hlThemeName);
        }

        /// <summary>
        /// Reset the dirty flag (e.g. after saving program options when they where edit).
        /// </summary>
        /// <param name="flag"></param>
        public void SetDirtyFlag(bool flag)
        {
            this.IsDirty = flag;
        }
        #endregion methods
    }
}
