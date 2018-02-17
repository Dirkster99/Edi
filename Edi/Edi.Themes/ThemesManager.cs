namespace Edi.Themes
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using ICSharpCode.AvalonEdit.Highlighting.Themes;
    using Definition;
    using Interfaces;

    /// <summary>
    /// This class manages a list of WPF themes (Aero, Metro etc) which
    /// can be combined with TextEditorThemes (True Blue, Deep Black).
    /// 
    /// The class implements a service that can be accessed via Instance property.
    /// The exposed methodes and properties can be used to display a list available
    /// themes, determine the currently selected theme, and set the currently selected
    /// theme.
    /// </summary>
    public class ThemesManager : IThemesManager, IParentSelectedTheme
    {
        #region fields
        #region WPF Themes
        #region Expression Dark theme resources
        const string MetroDarkThemeName = "Metro Dark";
        static readonly string[] MetroDarkResources =
    {
      "/Edi.Dialogs;component/Themes/ModernDialogEx.xaml",

      // "/FirstFloor.ModernUI;component/Assets/Button.xaml",

      "/FirstFloor.ModernUI;component/Assets/Calendar.xaml",
      "/FirstFloor.ModernUI;component/Assets/CheckBox.xaml",

      // For some reason these are applied in the Explorer (TW) but nowhere else(?)
      "/FirstFloor.ModernUI;component/Assets/ComboBox.xaml",
      // "/FirstFloor.ModernUI;component/Assets/ContextMenu.xaml",
      "/FirstFloor.ModernUI;component/Assets/DataGrid.xaml",
      "/FirstFloor.ModernUI;component/Assets/DatePicker.xaml",
      "/FirstFloor.ModernUI;component/Assets/GridSplitter.xaml",
      "/FirstFloor.ModernUI;component/Assets/Hyperlink.xaml",
      "/FirstFloor.ModernUI;component/Assets/Label.xaml",
      "/FirstFloor.ModernUI;component/Assets/ListBox.xaml",
      "/FirstFloor.ModernUI;component/Assets/ListView.xaml",
      "/FirstFloor.ModernUI;component/Assets/MenuItem.xaml",
      "/FirstFloor.ModernUI;component/Assets/ContextMenu.xaml",
      "/FirstFloor.ModernUI;component/Assets/PasswordBox.xaml",
      "/FirstFloor.ModernUI;component/Assets/ProgressBar.xaml",
      "/FirstFloor.ModernUI;component/Assets/RadioButton.xaml",
      "/FirstFloor.ModernUI;component/Assets/ScrollBar.xaml",
      "/FirstFloor.ModernUI;component/Assets/Slider.xaml",
      "/FirstFloor.ModernUI;component/Assets/TextBlock.xaml",
      "/FirstFloor.ModernUI;component/Assets/TextBox.xaml",
      "/FirstFloor.ModernUI;component/Assets/ToolTip.xaml",
      "/FirstFloor.ModernUI;component/Assets/TreeView.xaml",

      "/DropDownButtonLib;component/Themes/MetroDark.xaml",  // DropDownButtonLib theming
      "/MsgBox;component/Themes/DarkBrushes.xaml",
      "/MsgBox;component/Themes/DarkIcons.xaml",
      "/UnitComboLib;component/Themes/DarkBrushs.xaml",
      "/NumericUpDownLib;component/Themes/DarkBrushs.xaml",

      // This is required to style the dropdown button and frame of the control
      "/Edi.Themes;component/FileListView/Combobox.xaml",
      "/Edi.Themes;component/MetroDark/Theme.xaml",

      //"/Xceed.Wpf.AvalonDock.Themes.Expression;component/DarkTheme.xaml",
      "/Xceed.Wpf.AvalonDock.Themes.VS2013;component/DarkTheme.xaml",

      "/Edi.Apps;component/Themes/MetroDark.xaml",
      "/FileListView;component/Images/MetroDarkIcons.xaml"
    };
        #endregion Expression Dark theme resources

        #region Generic theme resources
        const string GenericThemeName = "Generic";
        static readonly string[] GenericResources =
    {
      "/FirstFloor.ModernUI;component/Assets/ModernUI.Light.xaml",
      "/Edi.Dialogs;component/Themes/ModernDialogEx.xaml",

      "/Edi.Themes;component/Generic/Theme.xaml",
      "/Edi.Apps;component/Themes/MetroLight.xaml",
      "/FileListView;component/Images/GenericIcons.xaml"
    };
        #endregion Generic theme resources

        #region Light Metro theme resources
        const string MetroLightThemeName = "Metro Light";
        static readonly string[] MetroResources =
    {
      "/Edi.Dialogs;component/Themes/ModernDialogEx.xaml",

      // "/FirstFloor.ModernUI;component/Assets/Button.xaml",

      "/FirstFloor.ModernUI;component/Assets/Calendar.xaml",
      "/FirstFloor.ModernUI;component/Assets/CheckBox.xaml",
      
      // For some reason these are applied in the Explorer (TW) but nowhere else(?)
      "/FirstFloor.ModernUI;component/Assets/ComboBox.xaml",
      //// "/FirstFloor.ModernUI;component/Assets/ContextMenu.xaml",
      "/FirstFloor.ModernUI;component/Assets/DataGrid.xaml",
      "/FirstFloor.ModernUI;component/Assets/DatePicker.xaml",
      "/FirstFloor.ModernUI;component/Assets/GridSplitter.xaml",
      "/FirstFloor.ModernUI;component/Assets/Hyperlink.xaml",
      "/FirstFloor.ModernUI;component/Assets/Label.xaml",
      "/FirstFloor.ModernUI;component/Assets/ListBox.xaml",
      "/FirstFloor.ModernUI;component/Assets/ListView.xaml",
      //"/FirstFloor.ModernUI;component/Assets/MenuItem.xaml",
      "/FirstFloor.ModernUI;component/Assets/PasswordBox.xaml",
      "/FirstFloor.ModernUI;component/Assets/ProgressBar.xaml",
      "/FirstFloor.ModernUI;component/Assets/RadioButton.xaml",
      "/FirstFloor.ModernUI;component/Assets/ScrollBar.xaml",
      "/FirstFloor.ModernUI;component/Assets/Slider.xaml",
      "/FirstFloor.ModernUI;component/Assets/TextBlock.xaml",
      "/FirstFloor.ModernUI;component/Assets/TextBox.xaml",
      "/FirstFloor.ModernUI;component/Assets/ToolTip.xaml",
      "/FirstFloor.ModernUI;component/Assets/TreeView.xaml",

      "/DropDownButtonLib;component/Themes/MetroLight.xaml",  // DropDownButtonLib theming
      "/MsgBox;component/Themes/LightBrushes.xaml",
      "/MsgBox;component/Themes/LightIcons.xaml",
      "/UnitComboLib;component/Themes/LightBrushs.xaml",
      "/NumericUpDownLib;component/Themes/LightBrushs.xaml",

      // This is required to style the dropdown button and frame of the control
      "/Edi.Themes;component/FileListView/Combobox.xaml",
      "/Edi.Themes;component/MetroLight/Theme.xaml",

      //"/Xceed.Wpf.AvalonDock.Themes.Expression;component/LightTheme.xaml",
      "/Xceed.Wpf.AvalonDock.Themes.VS2013;component/LightTheme.xaml",

      "/Edi.Apps;component/Themes/MetroLight.xaml",
      "/FileListView;component/Images/MetroLightIcons.xaml"
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

        public const string DefaultThemeName = MetroLightThemeName;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private SortedDictionary<string, ThemeBase> mTextEditorThemes = null;
        private ObservableCollection<ThemeBase> mListOfAllThemes = null;
        private string mSelectedThemeName = string.Empty;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public ThemesManager()
        {
            mSelectedThemeName = DefaultThemeName;
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Get the name of the currently seelcted theme.
        /// </summary>
        public string SelectedThemeName
        {
            get
            {
                return mSelectedThemeName;
            }
        }

        /// <summary>
        /// Get the object that has links to all resources for the currently selected WPF theme.
        /// </summary>
        public ThemeBase SelectedTheme
        {
            get
            {
                if (mTextEditorThemes == null || mListOfAllThemes == null)
                    BuildThemeCollections();

                ThemeBase theme;
                mTextEditorThemes.TryGetValue(mSelectedThemeName, out theme);

                // Fall back to default if all else fails
                if (theme == null)
                {
                    mTextEditorThemes.TryGetValue(DefaultThemeName, out theme);
                    mSelectedThemeName = theme.HlThemeName;
                }

                return theme;
            }
        }

        /// <summary>
        /// Get a list of all available themes (This property can typically be used to bind
        /// menuitems or other resources to let the user select a theme in the user interface).
        /// </summary>
        public ObservableCollection<ThemeBase> ListAllThemes
        {
            get
            {
                if (mTextEditorThemes == null || mListOfAllThemes == null)
                    BuildThemeCollections();

                return mListOfAllThemes;
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
            if (mTextEditorThemes == null || mListOfAllThemes == null)
                BuildThemeCollections();

            ThemeBase theme;
            mTextEditorThemes.TryGetValue(themeName, out theme);

            // Fall back to default if all else fails
            if (theme == null)
                return false;

            mSelectedThemeName = themeName;

            return true;
        }

        /// <summary>
        /// Get a text editor highlighting theme associated with the given WPF Theme Name.
        /// </summary>
        /// <param name="themeName"></param>
        /// <returns></returns>
        public HighlightingThemes GetTextEditorHighlighting(string themeName)
        {
            // Is this WPF theme configured with a highlighting theme???
            ThemeBase cfg = null;

            mTextEditorThemes.TryGetValue(themeName, out cfg);

            if (cfg != null)
                return cfg.HighlightingStyles;

            return null;
        }

        /// <summary>
        /// Build sorted dictionary and observable collection for WPF themes.
        /// </summary>
        private void BuildThemeCollections()
        {
            mTextEditorThemes = BuildThemeDictionary();
            mListOfAllThemes = new ObservableCollection<ThemeBase>();

            foreach (KeyValuePair<string, ThemeBase> t in mTextEditorThemes)
            {
                mListOfAllThemes.Add(t.Value);
            }
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
