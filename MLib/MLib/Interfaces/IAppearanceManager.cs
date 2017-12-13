namespace MLib.Interfaces
{
    using Events;
    using MLib.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;

    /// <summary>
    /// An interface to a component that manages all WPF THeming relevant things,
    /// such as, theme names, there resources, accent color, and so forth...
    /// </summary>
    public interface IAppearanceManager
    {
        #region properties
        /// <summary>
        /// Gets the name of the currently selected theme.
        /// </summary>
        string ThemeName { get; }

        /// <summary>
        /// Gets the current theme source.
        /// </summary>
        List<Uri> ThemeSources { get; }

        Color AccentColor { get; }
        #endregion properties

        #region events
        event ColorChangedEventHandler AccentColorChanged;
        #endregion events

        #region methods
        /// <summary>
        /// Returns the default theme for the application
        /// </summary>
        /// <param name="Themes"></param>
        /// <returns></returns>
        IThemeInfo GetDefaultTheme();

        /// <summary>
        /// Set the current theme as a selection of the settings service peroperties.
        /// </summary>
        /// <param name="Themes">Collections of themes to select the new theme from.</param>
        /// <param name="themeName">Name od the theme to be set (e.g.: Dark, Light)</param>
        /// <param name="AccentColor">Apply this accent color
        /// (can be Windows default or custom accent color).
        /// Accent Color in UI elements is invisible if this is null.</param>
        void SetTheme(IThemeInfos Themes, string themeName, Color AccentColor);

        /// <summary>
        /// Resets the standard themes available through the theme settings interface.
        /// </summary>
        /// <param name="Themes"></param>
        void SetDefaultThemes(IThemeInfos Themes);

        /// <summary>
        /// Adds more resource files into the standard themes available
        /// through the theme settings interface.
        /// </summary>
        /// <param name="themeName"></param>
        /// <param name="additionalResource"></param>
        /// <param name="themes"></param>
        void AddThemeResources(string themeName
                                , List<Uri> additionalResource
                                , IThemeInfos themes);

        /// <summary>
        /// Creates a new instance of an object that adheres to the
        /// <see cref="IThemeInfos"/> interface.
        /// </summary>
        /// <returns></returns>
        IThemeInfos CreateThemeInfos();
        #endregion methods
    }
}