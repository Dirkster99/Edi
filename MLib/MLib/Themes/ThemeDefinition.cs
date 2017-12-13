namespace MLib.Themes
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a theme by its name, source etc...
    /// </summary>
    public class ThemeDefinition
    {
        /// <summary>
        /// Hidden standard constructor.
        /// </summary>
        private ThemeDefinition()
        {
        }

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="source"></param>
        public ThemeDefinition(string name, List<string> sources)
        {
            this.Name = (name != null ? name : string.Empty);
            this.Sources = (sources != null ? new List<string>(sources) : new List<string>());
        }

        /// <summary>
        /// Identifies a theme by a Name that can be used as a key.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Uri formatted source for this theme.
        /// </summary>
        public List<string> Sources { get; private set; }
    }
}