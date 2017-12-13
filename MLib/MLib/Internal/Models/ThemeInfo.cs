namespace MLib.Internal.Models
{
    using Interfaces;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a WPF theme by its name and Uri sources.
    /// </summary>
    internal class ThemeInfo : IThemeInfo
    {
        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="themeName"></param>
        /// <param name="themeSources"></param>
        public ThemeInfo(string themeName,
                         List<Uri> themeSources)
            : this()
        {
            DisplayName = themeName;

            if (themeSources != null)
            {
                foreach (var item in themeSources)
                    ThemeSources.Add(new Uri(item.OriginalString, UriKind.Relative));
            }
        }

        /// <summary>
        /// Hidden standard constructor
        /// </summary>
        protected ThemeInfo()
        {
            DisplayName = string.Empty;
            ThemeSources = new List<Uri>();
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets the displayable (localized) name for this theme.
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets the Uri sources for this theme.
        /// </summary>
        public List<Uri> ThemeSources { get; private set; }
        #endregion properties

        #region methods
        /// <summary>
        /// Adds additional resource file references into the existing theme definition.
        /// </summary>
        /// <param name="additionalResource"></param>
        public void AddResources(List<Uri> additionalResource)
        {
            foreach (var item in additionalResource)
                ThemeSources.Add(item);
        }
        #endregion methods
    }
}
