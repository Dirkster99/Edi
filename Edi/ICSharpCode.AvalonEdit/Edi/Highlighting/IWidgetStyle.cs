namespace ICSharpCode.AvalonEdit.Edi.Interfaces
{
    using System.Windows.Media;

    public interface IWidgetStyle
    {
        #region properties
        /// <summary>
        /// Name of <seealso cref="WidgetStyle"/> object
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
        /// Get/set brush definition for the border used in this style
        /// </summary>
        SolidColorBrush borderColor { get; set; }
        #endregion properties
    }
}