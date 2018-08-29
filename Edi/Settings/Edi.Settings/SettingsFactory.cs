namespace Edi.Settings
{
    using Edi.Settings.Interfaces;
    using Edi.Settings.ProgramSettings;
    using Edi.Settings.UserProfile;
    using Edi.Themes.Interfaces;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UnitComboLib.Models.Unit;

    public sealed class SettingsFactory
    {
        public const string DefaultLocal = "en-US";

        private SettingsFactory()
        {
        }

        /// <summary>
        /// Class cosntructor from coordinates of control
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="isMaximized"></param>
        public static ViewPosSizeModel GetViewPosition(double x,
                                                       double y,
                                                       double width,
                                                       double height,
                                                       bool isMaximized = false)
        {
            return new ViewPosSizeModel(x, y, width, height, isMaximized);
        }

        /// <summary>
        /// Get a list of all supported languages in Edi.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ILang> GetSupportedLanguages()
        {
            List<Lang> ret = new List<Lang>();

            ret.Add(new Lang("de", "DE", "Deutsch (German)"));
            ret.Add(new Lang("en", "US", "English (English)" ));
            ret.Add(new Lang("es", "ES", "Español (Spanish)" ));
            ret.Add(new Lang("fr", "FR", "Français (French)" ));
            ret.Add(new Lang("it", "IT", "Italiano (Italian)" ));
            ret.Add(new Lang("ru", "RU", "Русский (Russian)" ));
            ret.Add(new Lang("id", "ID", "Bahasa Indonesia(Indonesian)" ));
            ret.Add(new Lang("ja", "JP", "日本語 (Japanese)" ));
            ret.Add(new Lang("zh-Hans", "", "简体中文 (Simplified)" ));
            ret.Add(new Lang("pt", "PT", "Português (Portuguese)" ));
            ret.Add(new Lang("hi", "IN", "हिंदी (Hindi)" ));

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
        /// Creates a new Settings Manager instance
        /// given a themes manager instance is available.
        /// </summary>
        /// <param name="themesManager"></param>
        /// <returns></returns>
        public static ISettingsManager CreateSettingsManager(IThemesManager themesManager)
        {
            return new SettingsManagerImpl(themesManager);
        }
    }
}
