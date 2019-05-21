namespace ICSharpCode.AvalonEdit.Highlighting.Themes
{
    using ICSharpCode.AvalonEdit.Edi.Interfaces;
    using System.Collections.Generic;
    using System.Windows.Media;

    /// <summary>
    /// This class manages one highlighting theme for one highlighting pattern (eg.: 'HTML')
    /// </summary>
    public class HighlightingTheme : IHighlightingTheme
    {
        #region fields
        private Dictionary<string, IWordsStyle> mHlThemes;
        #endregion fields

        #region constructor
        /// <summary>
        /// Constructor constructor with human
        /// readable name and description for this highlighting theme
        /// </summary>
        /// <param name="hlName">Human read-able name of highlighting (eg.: 'html')</param>
        /// <param name="description">Human read-able description for this highlighting pattern</param>
        public HighlightingTheme(string hlName, string description = "")
          : this()
        {
            this.HlName = hlName;
            this.HlDesc = description;
        }

        /// <summary>
        /// Hidden constructor in favour of constructor with human
        /// readable name and description for this highlighting theme
        /// </summary>
        protected HighlightingTheme()
        {
            this.HlDesc = this.HlName = string.Empty;
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Human read-able name of highlighting (eg.: 'html')
        /// </summary>
        public string HlName { get; private set; }

        /// <summary>
        /// Human read-able description for this highlighting pattern
        /// </summary>
        public string HlDesc { get; set; }
        #endregion properties

        #region methods
        /// <summary>
        /// Add another style (foreground color, background color, bold etc...) to the collection
        /// of styles that make up this highlighting theme.
        /// </summary>
        /// <param name="brushName"></param>
        /// <param name="wordStyle">color and brush representation (eg.: "#FF00FFFF" etc)</param>
        public void AddWordStyle(string brushName, IWordsStyle wordStyle)
        {
            if (this.mHlThemes == null)
                this.mHlThemes = new Dictionary<string, IWordsStyle>();

            this.mHlThemes.Add(brushName, wordStyle);
        }

        /// <summary>
        /// Attempt to retrieve a <seealso cref="SolidColorBrush"/> from a word style name
        /// </summary>
        /// <param name="wordStyleName"></param>
        /// <returns></returns>
        public SolidColorBrush GetFgColorBrush(string wordStyleName)
        {
            if (this.mHlThemes != null)
            {
                IWordsStyle s;
                this.mHlThemes.TryGetValue(wordStyleName, out s);

                if (s != null)
                    return s.fgColor;
            }

            return null;
        }

        /// <summary>
        /// Attempt to retrieve a <seealso cref="WordsStyle"/> from a word style name
        /// </summary>
        /// <param name="BrushName"></param>
        /// <returns></returns>
        public IWordsStyle GetWordsStyle(string BrushName)
        {
            if (this.mHlThemes != null)
            {
                IWordsStyle s;
                this.mHlThemes.TryGetValue(BrushName, out s);

                return s;
            }

            return null;
        }
        #endregion methods
    }
}
