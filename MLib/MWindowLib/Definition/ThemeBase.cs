namespace MWindowLib.Definition
{
    using System.Collections.Generic;

    /// <summary>
    /// A theme base contains all information that is relevant to a given name.
    /// Thats at least a list of resource files drawn from various modules plus
    /// a displayable unique name that can be used to select/display the theme
    /// in the application's UI.
    /// </summary>
    public class ThemeBase
    {
        #region fields
        #endregion fields

        #region constructor
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        internal ThemeBase(List<string> resources
                         , string wpfThemeName
            )
            : this()
        {
            this.Resources = new List<string>(resources);
            this.WPFThemeName = wpfThemeName;
        }

        /// <summary>
        /// Hidden constructor
        /// </summary>
        protected ThemeBase()
        {
            this.Resources = new List<string>();
            this.WPFThemeName = string.Empty;
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Get a list of Uri formatted resource strings that point to all relevant resources.
        /// </summary>
        public List<string> Resources { get; private set; }

        /// <summary>
        /// WPF Application skin theme (e.g. Metro)
        /// </summary>
        public string WPFThemeName { get; private set; }
        #endregion properties
    }
}
