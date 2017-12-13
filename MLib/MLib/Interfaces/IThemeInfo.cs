namespace MLib.Interfaces
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a WPF theme by its name and Uri source.
    /// </summary>
    public interface IThemeInfo
    {
        #region properties
        /// <summary>
        /// Gets the displayable (localized) name for this theme.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets the Uri source for this theme.
        /// </summary>
        List<Uri> ThemeSources { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Adds additional resource file references into the existing theme definition.
        /// </summary>
        /// <param name="additionalResource"></param>
        void AddResources(List<Uri> additionalResource);
        #endregion methods
    }
}
