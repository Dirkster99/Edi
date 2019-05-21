namespace Edi.Themes
{
    using Edi.Themes.Interfaces;

    /// <summary>
    /// Implements a <seealso cref="ThemesManager"/> factory.
    /// </summary>
    public sealed class Factory
    {
        private Factory()
        {
        }

        /// <summary>
        /// Creates a new theme manager instances and returns it
        /// </summary>
        /// <returns></returns>
        public static IThemesManager CreateThemeManager()
        {
            return new ThemesManager();
        }

        /// <summary>
        /// Gets the name of the default theme of this themes manager.
        /// </summary>
        public static string DefaultThemeName
        {
            get
            {
                return ThemesManager.DefaultThemeNameString;
            }
        }
    }
}
