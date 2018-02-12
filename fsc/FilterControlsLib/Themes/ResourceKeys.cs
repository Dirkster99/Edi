namespace FilterControlsLib.Themes
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
        public static readonly ComponentResourceKey FilterCmbDarkBrushKey               = new ComponentResourceKey(typeof(ResourceKeys), "FilterCmbDarkBrushKey");
        public static readonly ComponentResourceKey FilterCmbHighlighColorBrushKey      = new ComponentResourceKey(typeof(ResourceKeys), "FilterCmbHighlighColorBrushKey");
        public static readonly ComponentResourceKey FilterCmbPressedBrushKey            = new ComponentResourceKey(typeof(ResourceKeys), "FilterCmbPressedBrushKey");
        public static readonly ComponentResourceKey FilterCmbDisabledForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "FilterCmbDisabledForegroundBrushKey");
        public static readonly ComponentResourceKey FilterCmbDisabledBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "FilterCmbDisabledBackgroundBrushKey");
        public static readonly ComponentResourceKey FilterCmbWindowBackgroundBrushKey   = new ComponentResourceKey(typeof(ResourceKeys), "FilterCmbWindowBackgroundBrushKey");
        public static readonly ComponentResourceKey FilterCmbNormalBorderBrushKey       = new ComponentResourceKey(typeof(ResourceKeys), "FilterCmbNormalBorderBrushKey");
        public static readonly ComponentResourceKey FilterCmbDisabledBorderBrushKey     = new ComponentResourceKey(typeof(ResourceKeys), "FilterCmbDisabledBorderBrushKey");
        public static readonly ComponentResourceKey FilterCmbSolidBorderBrushKey        = new ComponentResourceKey(typeof(ResourceKeys), "FilterCmbSolidBorderBrushKey");
        public static readonly ComponentResourceKey FilterCmbGlyphBrushKey              = new ComponentResourceKey(typeof(ResourceKeys), "FilterCmbGlyphBrushKey");
        #endregion Brush Keys
    }
}
