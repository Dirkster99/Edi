namespace UnitComboLib.Themes
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
        public static readonly ComponentResourceKey NormalBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "NormalBackgroundBrushKey");
        public static readonly ComponentResourceKey UnitCmbDarkBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "UnitCmbDarkBrushKey");

        /// <summary>
        /// This Brush is used to highlight the ToggleButton on MouseOver
        /// </summary>
        public static readonly ComponentResourceKey UnitCmbHighlighColorBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "UnitCmbHighlighColorBrushKey");

        public static readonly ComponentResourceKey UnitCmbPressedBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "UnitCmbPressedBrushKey");
        public static readonly ComponentResourceKey UnitCmbDisabledForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "UnitCmbDisabledForegroundBrushKey");
        public static readonly ComponentResourceKey UnitCmbDisabledBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "UnitCmbDisabledBackgroundBrushKey");
        public static readonly ComponentResourceKey UnitCmbWindowBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "UnitCmbWindowBackgroundBrushKey");

        // Border Brushes
        public static readonly ComponentResourceKey UnitCmbNormalBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "UnitCmbNormalBorderBrushKey");
        public static readonly ComponentResourceKey UnitCmbDisabledBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "UnitCmbDisabledBorderBrushKey");
        public static readonly ComponentResourceKey UnitCmbSolidBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "UnitCmbSolidBorderBrushKey");

        // Miscellaneous Brushes
        public static readonly ComponentResourceKey UnitCmbGlyphBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "UnitCmbGlyphBrushKey");
        #endregion Brush Keys
    }
}
