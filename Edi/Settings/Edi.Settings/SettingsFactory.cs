namespace Edi.Settings
{
    using Edi.Settings.Interfaces;
    using Edi.Settings.ProgramSettings;
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
        /// Creates a new Settings Manager instance
        /// given a themes manager instance is available.
        /// </summary>
        /// <param name="themesManager"></param>
        /// <returns></returns>
        ISettingsManager CreateSettingsManager(IThemesManager themesManager)
        {
            return new SettingsManager(themesManager);
        }
    }
}
