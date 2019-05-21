namespace ICSharpCode.AvalonEdit.Edi.Interfaces
{
    using System.Windows;
    using System.Windows.Media;

    public interface IWordsStyle
    {
        #region properties
        /// <summary>
        /// Name of <seealso cref="WordsStyle"/> object
        /// 
        /// (this is usually the key in a collection of these)
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Get/set brush definition for the foreground used in this style
        /// </summary>
        SolidColorBrush fgColor { get; set; }

        /// <summary>
        /// Get/set brush definition for the background used in this style
        /// </summary>
        SolidColorBrush bgColor { get; set; }

        /// <summary>
        /// Get/set brush definition for the fontweight used in this style
        /// </summary>
        FontWeight? fontWeight { get; set; }

        /// <summary>
        /// Get/set fontStyle used in this WordsStyle
        /// </summary>
        FontStyle? fontStyle { get; set; }
        #endregion properties
    }
}