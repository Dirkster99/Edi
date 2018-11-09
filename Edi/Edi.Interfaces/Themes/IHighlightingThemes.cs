namespace Edi.Interfaces.Themes
{
    using System.Collections.Generic;

    public interface IHighlightingThemes
    {
        #region properties
        /// <summary>
        /// Name of highlighting theme
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Path and file name from which this highlighting theme was loaded from (if any)
        /// </summary>
        string FileNamePath { get; set; }

        /// <summary>
        /// Returns an (empty) collection of IEnumerable style objects.
        /// </summary>
        IEnumerable<IWidgetStyle> GlobalStyles { get; }
        #endregion properties

        #region methods

        /// <summary>
        /// Add a highlighting theme into the collection
        /// of highlighting themes stored in this object.
        /// </summary>
        /// <param name="styleName"></param>
        /// <param name="highlightingTheme"></param>
        void AddTheme(string styleName, IHighlightingTheme highlightingTheme);

        /// <summary>
        /// Search a highlighting theme by its name and return a
        /// <see cref="HighlightingTheme"/> object or null.
        /// </summary>
        /// <param name="themeName"></param>
        /// <returns></returns>
        IHighlightingTheme FindTheme(string themeName);

        /// <summary>
        /// Add a highlighting theme into the collection
        /// of highlighting themes stored in this object.
        /// </summary>
        /// <param name="styleName"></param>
        /// <param name="style"></param>
        void AddWidgetStyle(string styleName, IWidgetStyle style);
        #endregion methods
    }
}