namespace ICSharpCode.AvalonEdit.Highlighting.Themes
{
    using System.Windows.Media;
    using global::Edi.Interfaces.Themes;

    /// <summary>
    /// Class to manage a global editor style (eg.: Default color of background and foreground)
    /// </summary>
    public class WidgetStyle : IWidgetStyle
    {
        #region constructors
        /// <summary>
        /// Construct a named (eg. 'Comment') WordStyle object
        /// </summary>
        /// <param name="name"></param>
        public WidgetStyle(string name)
          : this()
        {
            this.Name = name;
        }

        /// <summary>
        /// Hidden standard constructor in favour of named element cosntructor
        /// </summary>
        protected WidgetStyle()
        {
            this.Name = string.Empty;

            this.fgColor = null;
            this.bgColor = null;
            this.borderColor = null;
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Name of <seealso cref="WidgetStyle"/> object
        /// 
        /// (this is usually the key in a collection of these)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get/set brush definition for the foreground used in this style
        /// </summary>
        public SolidColorBrush fgColor { get; set; }

        /// <summary>
        /// Get/set brush definition for the background used in this style
        /// </summary>
        public SolidColorBrush bgColor { get; set; }

        /// <summary>
        /// Get/set brush definition for the border used in this style
        /// </summary>
        public SolidColorBrush borderColor { get; set; }
        #endregion properties
    }
}
