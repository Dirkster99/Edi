namespace FolderControlsLib.Themes
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
        public static readonly ComponentResourceKey FolderCmbDarkBrushKey               = new ComponentResourceKey(typeof(ResourceKeys), "FolderCmbDarkBrushKey");
        public static readonly ComponentResourceKey FolderCmbHighlighColorBrushKey      = new ComponentResourceKey(typeof(ResourceKeys), "FolderCmbHighlighColorBrushKey");
        public static readonly ComponentResourceKey FolderCmbPressedBrushKey            = new ComponentResourceKey(typeof(ResourceKeys), "FolderCmbPressedBrushKey");
        public static readonly ComponentResourceKey FolderCmbDisabledForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "FolderCmbDisabledForegroundBrushKey");
        public static readonly ComponentResourceKey FolderCmbDisabledBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "FolderCmbDisabledBackgroundBrushKey");
        public static readonly ComponentResourceKey FolderCmbWindowBackgroundBrushKey   = new ComponentResourceKey(typeof(ResourceKeys), "FolderCmbWindowBackgroundBrushKey");
        public static readonly ComponentResourceKey FolderCmbNormalBorderBrushKey       = new ComponentResourceKey(typeof(ResourceKeys), "FolderCmbNormalBorderBrushKey");
        public static readonly ComponentResourceKey FolderCmbDisabledBorderBrushKey     = new ComponentResourceKey(typeof(ResourceKeys), "FolderCmbDisabledBorderBrushKey");
        public static readonly ComponentResourceKey FolderCmbSolidBorderBrushKey        = new ComponentResourceKey(typeof(ResourceKeys), "FolderCmbSolidBorderBrushKey");
        public static readonly ComponentResourceKey FolderCmbGlyphBrushKey              = new ComponentResourceKey(typeof(ResourceKeys), "FolderCmbGlyphBrushKey");
        #endregion Brush Keys
    }
}
