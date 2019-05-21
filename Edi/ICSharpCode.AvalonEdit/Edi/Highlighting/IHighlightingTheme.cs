namespace ICSharpCode.AvalonEdit.Edi.Interfaces
{
    using System.Windows.Media;

    public interface IHighlightingTheme
    {
        #region properties
        /// <summary>
        /// Human read-able name of highlighting (eg.: 'html')
        /// </summary>
        string HlName { get; }

        /// <summary>
        /// Human read-able description for this highlighting pattern
        /// </summary>
        string HlDesc { get; set; }
        #endregion properties

        #region methods
        /// <summary>
        /// Add another style (foreground color, background color, bold etc...) to the collection
        /// of styles that make up this highlighting theme.
        /// </summary>
        /// <param name="brushName"></param>
        /// <param name="wordStyle">color and brush representation (eg.: "#FF00FFFF" etc)</param>
        void AddWordStyle(string brushName, IWordsStyle wordStyle);

        /// <summary>
        /// Attempt to retrieve a <seealso cref="SolidColorBrush"/> from a word style name
        /// </summary>
        /// <param name="wordStyleName"></param>
        /// <returns></returns>
        SolidColorBrush GetFgColorBrush(string wordStyleName);

        /// <summary>
        /// Attempt to retrieve a <seealso cref="WordsStyle"/> from a word style name
        /// </summary>
        /// <param name="BrushName"></param>
        /// <returns></returns>
        IWordsStyle GetWordsStyle(string BrushName);
        #endregion methods
    }
}