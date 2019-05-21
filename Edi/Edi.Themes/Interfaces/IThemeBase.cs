namespace Edi.Themes.Interfaces
{
    using ICSharpCode.AvalonEdit.Edi.Interfaces;
    using System.Collections.Generic;

    public interface IThemeBase
    {
        #region properties
        /// <summary>
        /// Get a list of Uri formatted resource strings that point to all relevant resources.
        /// </summary>
        List<string> Resources { get; }

        /// <summary>
        /// WPF Application skin theme (e.g. Metro)
        /// </summary>
        string WPFThemeName { get; }

        /// <summary>
        /// Get/set the name of the highlighting theme
        /// (eg.: DeepBlack, Bright Standard, True Blue)
        /// </summary>
        string EditorThemeName { get; }

        /// <summary>
        /// Get/set the location of the current highlighting theme (eg.: C:\DeepBlack.xml)
        /// </summary>
        string EditorThemeFileName { get; }

        /// <summary>
        /// Get the human read-able name of this WPF/Editor theme.
        /// </summary>
        string HlThemeName { get; }

        /// <summary>
        /// This property exposes a collection of highlighting themes for different file types
        /// (color and style definitions for keywords in SQL, C# and so forth)
        /// </summary>
        IHighlightingThemes HighlightingStyles { get; }

        /// <summary>
        /// Determine whether this theme is currently selected or not.
        /// </summary>
        bool IsSelected { get; }
        #endregion properties
    }
}
