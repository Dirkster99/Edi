namespace Edi.Themes
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using Edi.Themes.Definition;
    using MLib.Interfaces;
    using System;
    using System.Windows.Media;
    using System.Windows;
    using Edi.Interfaces.Themes;

    /// <summary>
    /// This class manages a list of WPF themes (Aero, Metro etc) which
    /// can be combined with TextEditorThemes (True Blue, Deep Black).
    /// 
    /// The class implements a service that can be accessed via Instance property.
    /// The exposed methodes and properties can be used to display a list available
    /// themes, determine the currently selected theme, and set the currently selected
    /// theme.
    /// </summary>
    internal class ThemesManager : IThemesManager, IParentSelectedTheme
    {
        #region fields
        #region WPF Themes
        #region Dark theme resources
        const string MetroDarkThemeName = "Metro Dark";
        static readonly string[] MetroDarkResources =
        {
          "/Mlib;component/Themes/DarkTheme.xaml",
          "/MWindowLib;component/Themes/DarkTheme.xaml",

          "/Edi.Themes;component/MetroDark/Theme.xaml",
          "/Xceed.Wpf.AvalonDock.Themes.VS2013;component/DarkTheme.xaml",

          "/DropDownButtonLib;component/Themes/MetroDark.xaml",  // DropDownButtonLib theming
          "/MsgBox;component/Themes/DarkBrushes.xaml",
          "/MsgBox;component/Themes/DarkIcons.xaml",
          "/UnitComboLib;component/Themes/DarkBrushs.xaml",
          "/NumericUpDownLib;component/Themes/DarkBrushs.xaml",

          "/DropDownButtonLib;component/Themes/MetroDark.xaml",
          "/WatermarkControlsLib;component/Themes/DarkBrushs.xaml",
          "/FolderBrowser;component/Themes/MetroDark.xaml",
          "/FileListView;component/Images/MetroDarkIcons.xaml",
          "/HistoryControlLib;component/Themes/DarkTheme.xaml",

          "/Edi.Themes;component/BindToMLib/MiniUML.View/LightBrushs.xaml",

          "/Edi.Themes;component/BindToMLib/MWindowLib/DarkBrushs.xaml",
          "/Edi.Themes;component/BindToMLib/AvalonDock_DarkLightBrushs.xaml",
          "/Edi.Themes;component/BindToMLib/DropDownButtonLib_DarkLightBrushs.xaml",
          "/Edi.Themes;component/BindToMLib/HistoryControlLib_DarkLightBrushs.xaml",
          "/Edi.Themes;component/BindToMLib/WatermarkControlsLib_DarkLightBrushs.xaml",
          "/Edi.Themes;component/BindToMLib/NumericUpDownLib_DarkLightBrushs.xaml",
          "/Edi.Themes;component/BindToMLib/UnitComboLib_DarkLightBrushs.xaml"
        };
        #endregion Dark theme resources

        #region Generic theme resources
        const string GenericThemeName = "Generic";
        static readonly string[] GenericResources =
        {
          "/Edi.Themes;component/Generic/Theme.xaml",
          "/FileListView;component/Images/GenericIcons.xaml"
        };
        #endregion Generic theme resources

        #region Light Metro theme resources
        const string MetroLightThemeName = "Metro Light";
        static readonly string[] MetroResources =
        {
          "/Mlib;component/Themes/LightTheme.xaml",
          "/MWindowLib;component/Themes/LightTheme.xaml",

          // This is required to style the dropdown button and frame of the control
          "/Edi.Themes;component/MetroLight/Theme.xaml",
          "/Xceed.Wpf.AvalonDock.Themes.VS2013;component/LightTheme.xaml",

          "/DropDownButtonLib;component/Themes/MetroLight.xaml",  // DropDownButtonLib theming
          "/MsgBox;component/Themes/LightBrushes.xaml",
          "/MsgBox;component/Themes/LightIcons.xaml",
          "/UnitComboLib;component/Themes/LightBrushs.xaml",
          "/NumericUpDownLib;component/Themes/LightBrushs.xaml",

          "/DropDownButtonLib;component/Themes/MetroLight.xaml",
          "/WatermarkControlsLib;component/Themes/LightBrushs.xaml",
          "/FolderBrowser;component/Themes/MetroLight.xaml",
          "/FileListView;component/Images/MetroLightIcons.xaml",
          "/HistoryControlLib;component/Themes/LightTheme.xaml",

          "/Edi.Themes;component/BindToMLib/MiniUML.View/LightBrushs.xaml",

          "/Edi.Themes;component/BindToMLib/MWindowLib/LightBrushs.xaml",
          "/Edi.Themes;component/BindToMLib/AvalonDock_DarkLightBrushs.xaml",
          "/Edi.Themes;component/BindToMLib/DropDownButtonLib_DarkLightBrushs.xaml",
          "/Edi.Themes;component/BindToMLib/HistoryControlLib_DarkLightBrushs.xaml",
          "/Edi.Themes;component/BindToMLib/WatermarkControlsLib_DarkLightBrushs.xaml",
          "/Edi.Themes;component/BindToMLib/NumericUpDownLib_DarkLightBrushs.xaml",
          "/Edi.Themes;component/BindToMLib/UnitComboLib_DarkLightBrushs.xaml"
        };
        #endregion Light Metro theme resources
        #endregion WPF Themes

        #region Text Editor Themes
        const string EditorThemeBrightStandard = "Bright Standard";
        const string EditorThemeBrightStandardLocation = @"AvalonEdit\HighLighting_Themes\BrightStandard.xshd";

        const string EditorThemeTrueBlue = "True Blue";
        const string EditorThemeTrueBlueLocation = @"AvalonEdit\HighLighting_Themes\TrueBlue.xshd";

        const string EditorThemeDeepBlack = "Deep Black";
        const string EditorThemeDeepBlackLocation = @"AvalonEdit\HighLighting_Themes\DeepBlack.xshd";
        #endregion Text Editor Themes

        public const string DefaultThemeNameString = ThemesManager.MetroLightThemeName;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IAppearanceManager _AppearanceManager;
        private readonly SortedDictionary<string, IThemeBase> _TextEditorThemes;
        private readonly ObservableCollection<IThemeBase> _ListOfAllThemes;
        private string _SelectedThemeName = string.Empty;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public ThemesManager()
        {
            _SelectedThemeName = ThemesManager.DefaultThemeNameString;

            _AppearanceManager = MLib.AppearanceManager.GetInstance();
            _TextEditorThemes = new SortedDictionary<string, IThemeBase>();
            _ListOfAllThemes = new ObservableCollection<IThemeBase>();
        }
        #endregion constructor

        #region properties
        public string DefaultThemeName { get { return DefaultThemeNameString; } }

        /// <summary>
        /// Get the name of the currently seelcted theme.
        /// </summary>
        public string SelectedThemeName
        {
            get
            {
                return _SelectedThemeName;
            }
        }

        /// <summary>
        /// Get the object that has links to all resources for the currently selected WPF theme.
        /// </summary>
        public IThemeBase SelectedTheme
        {
            get
            {
                // Build theme references to resources on demand
                if (_TextEditorThemes.Count == 0 || _ListOfAllThemes.Count == 0)
                    BuildThemeCollections();

                IThemeBase theme;
                _TextEditorThemes.TryGetValue(_SelectedThemeName, out theme);

                // Fall back to default if all else fails
                if (theme == null)
                {
                    _TextEditorThemes.TryGetValue(ThemesManager.DefaultThemeNameString, out theme);
                    _SelectedThemeName = theme.HlThemeName;
                }

                return theme;
            }
        }

        /// <summary>
        /// Get a list of all available themes (This property can typically be used to bind
        /// menuitems or other resources to let the user select a theme in the user interface).
        /// </summary>
        public ObservableCollection<IThemeBase> ListAllThemes
        {
            get
            {
                // Build theme references to resources on demand
                if (_TextEditorThemes.Count == 0 || _ListOfAllThemes.Count == 0)
                    BuildThemeCollections();

                return _ListOfAllThemes;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Change the WPF/EditorHighlightingTheme to the <paramref name="themeName"/> theme.
        /// </summary>
        /// <param name="themeName"></param>
        /// <returns>True if new theme is succesfully selected (was available), otherwise false</returns>
        public bool SetSelectedTheme(string themeName)
        {
            // Build theme references to resources on demand
            if (_TextEditorThemes.Count == 0 || _ListOfAllThemes.Count == 0)
                BuildThemeCollections();


            // Lets try to get the requested theme
            IThemeBase theme;
            _TextEditorThemes.TryGetValue(themeName, out theme);

            // Fall back to default if all else fails
            if (theme == null)
                return false;

            _SelectedThemeName = themeName;

            _AppearanceManager.SetAccentColor(GetCurrentAccentColor());

            return true;
        }

        private Color GetCurrentAccentColor()  // ISettingsManager settings
        {
            Color AccentColor = default(Color);

////            if (settings.Options.GetOptionValue<bool>("Appearance", "ApplyWindowsDefaultAccent"))
////            {
                try
                {
                    AccentColor = SystemParameters.WindowGlassColor;
                }
                catch
                {
                }

                // This may be black on Windows 7 and the experience is black & white then :-(
                if (AccentColor == default(Color) || AccentColor == Colors.Black || AccentColor.A == 0)
                {
                    // default blue accent color
                    AccentColor = Color.FromRgb(0x1b, 0xa1, 0xe2);
                }
////            else
////                AccentColor = settings.Options.GetOptionValue<Color>("Appearance", "AccentColor");

            return AccentColor;
        }


        /// <summary>
        /// Get a text editor highlighting theme associated with the given WPF Theme Name.
        /// </summary>
        /// <param name="themeName"></param>
        /// <returns></returns>
        public IHighlightingThemes GetTextEditorHighlighting(string themeName)
        {
            // Is this WPF theme configured with a highlighting theme???
            IThemeBase cfg = null;

            _TextEditorThemes.TryGetValue(themeName, out cfg);

            if (cfg != null)
                return cfg.HighlightingStyles;

            return null;
        }

        /// <summary>
        /// Resets the standard themes available through the theme settings interface.
        /// </summary>
        /// <param name="themes"></param>
        public void SetDefaultThemes(IThemeInfos themes)
        {
            themes.RemoveAllThemeInfos();

            // Add dark theme resource items
            var resources = new List<Uri>();
            foreach (var item in MetroDarkResources)
                resources.Add(new Uri(item, UriKind.RelativeOrAbsolute));

            themes.AddThemeInfo(MetroDarkThemeName, resources);

            // Add light theme resource items
            resources = new List<Uri>();
            foreach (var item in ThemesManager.MetroResources)
                resources.Add(new Uri(item, UriKind.RelativeOrAbsolute));

            themes.AddThemeInfo(MetroLightThemeName, resources);

            // Add generic theme resource items
            resources = new List<Uri>();
            foreach (var item in ThemesManager.GenericResources)
                resources.Add(new Uri(item, UriKind.RelativeOrAbsolute));

            themes.AddThemeInfo(GenericThemeName, resources);
        }

        /// <summary>
        /// Build sorted dictionary and observable collection for WPF themes.
        /// </summary>
        private void BuildThemeCollections()
        {
            _TextEditorThemes.Clear();
            _ListOfAllThemes.Clear();

            foreach (var item in BuildThemeDictionary())
                _TextEditorThemes.Add(item.Key, item.Value);

            foreach (KeyValuePair<string, IThemeBase> t in _TextEditorThemes)
                _ListOfAllThemes.Add(t.Value);
        }

        /// <summary>
        /// Build a sorted structure of all default themes and their resources.
        /// </summary>
        /// <returns></returns>
        private SortedDictionary<string, ThemeBase> BuildThemeDictionary()
        {
            SortedDictionary<string, ThemeBase> ret = new SortedDictionary<string, ThemeBase>();

            ThemeBase t = null;
            string themeName = null;
            List<string> wpfTheme = null;

            try
            {
                string appLocation = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

                // ExpressionDark Theme
                themeName = MetroDarkThemeName;
                wpfTheme = new List<string>(MetroDarkResources);

                t = new ThemeBase(this, wpfTheme, themeName, null, null, null);
                ret.Add(t.HlThemeName, t);

                t = new ThemeBase(this, wpfTheme, themeName, EditorThemeDeepBlack,
                                  appLocation, EditorThemeDeepBlackLocation);
                ret.Add(t.HlThemeName, t);

                t = new ThemeBase(this, wpfTheme, themeName, EditorThemeTrueBlue,
                                  appLocation, EditorThemeTrueBlueLocation);
                ret.Add(t.HlThemeName, t);

                // Generic Theme
                themeName = GenericThemeName;
                wpfTheme = new List<string>(GenericResources);

                t = new ThemeBase(this, wpfTheme, themeName, null, null, null);
                ret.Add(t.HlThemeName, t);

                // Metro Theme
                themeName = MetroLightThemeName;
                wpfTheme = new List<string>(MetroResources);

                t = new ThemeBase(this, wpfTheme, themeName, null, null, null);
                ret.Add(t.HlThemeName, t);

                t = new ThemeBase(this, wpfTheme, themeName, EditorThemeBrightStandard,
                                   appLocation, EditorThemeBrightStandardLocation);
                ret.Add(t.HlThemeName, t);

                t = new ThemeBase(this, wpfTheme, themeName, EditorThemeTrueBlue,
                                    appLocation, EditorThemeTrueBlueLocation);
                ret.Add(t.HlThemeName, t);
            }
            catch (System.Exception exp)
            {
                string msg = string.Format("Error registering application theme '{0}' -> '{1}'",
                                           themeName == null ? "(null)" : themeName,
                                           t == null ? "(null)" : t.HlThemeName);

                // Log an error message and let the system boot up with default theme instead of re-throwing this
                logger.Fatal(new System.Exception(msg, exp));
            }

            return ret;
        }
        #endregion methods
    }
}
