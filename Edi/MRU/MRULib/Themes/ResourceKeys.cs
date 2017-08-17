namespace MRULib.Themes
{
    using System.Windows;

    /// <summary>
    /// Resource key management class to keep track of all resources
    /// that can be re-styled in applications that make use of the implemented controls.
    /// </summary>
    public static class ResourceKeys
    {
        #region Accent Keys
        /// <summary>
        /// Accent Color Key - This Color key is used to accent elements in the UI
        /// (e.g.: Color of Activated Normal Window Frame, ResizeGrip, Focus or MouseOver input elements)
        /// </summary>
        public static readonly ComponentResourceKey ControlAccentColorKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlAccentColorKey");

        /// <summary>
        /// Accent Brush Key - This Brush key is used to accent elements in the UI
        /// (e.g.: Color of Activated Normal Window Frame, ResizeGrip, Focus or MouseOver input elements)
        /// </summary>
        public static readonly ComponentResourceKey ControlAccentBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlAccentBrushKey");
        #endregion Accent Keys

        #region Brush Keys
        /// <summary>
        /// Foreground Brush key of the pin shown in the listview.
        /// </summary>
        public static readonly ComponentResourceKey Pin_Foreground = new ComponentResourceKey(typeof(ResourceKeys), "Pin_Foreground");

        /// <summary>
        /// Foreground Border Brush key of the pin shown in the listview.
        /// This key outlines the pin figure and can be used to highlight the outline only -
        /// or use a different color on the outline than on the inner foreground body pixels.
        /// </summary>
        public static readonly ComponentResourceKey Pin_ForegroundBorder = new ComponentResourceKey(typeof(ResourceKeys), "Pin_ForegroundBorder");

        /// <summary>
        /// Opacity value (0.0 - 1.0) of the checkmark pin when it is shown on mouseover.
        /// </summary>
        public static readonly ComponentResourceKey Pin_NoCheckMarkOpacitiy = new ComponentResourceKey(typeof(ResourceKeys), "Pin_NoCheckMarkOpacitiy");

        /// <summary>
        /// Opacity value (0.0 - 1.0) of the checkmark pin when it is shown on mouseover.
        /// </summary>
        public static readonly ComponentResourceKey Hyperlink_Foreground = new ComponentResourceKey(typeof(ResourceKeys), "Hyperlink_Foreground");
        #endregion Brush Keys
    }
}
