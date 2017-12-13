namespace MWindowLib.Themes
{
    using System.Windows;

    public static class ResourceKeys
    {
        #region Accent Keys
        // Accent Color Key and Accent Brush Key
        // These keys are used to accent elements in the UI
        // (e.g.: Color of Activated Normal Window Frame, ResizeGrip, Focus or MouseOver input elements)
        public static readonly ComponentResourceKey ControlAccentColorKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlAccentColorKey");
        public static readonly ComponentResourceKey ControlAccentBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlAccentBrushKey");
        #endregion Accent Keys

        #region Normal Control Foreground and Background Keys
        // Color Keys
        public static readonly ComponentResourceKey ControlNormalForegroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlNormalForegroundKey");
        public static readonly ComponentResourceKey ControlNormalBackgroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlNormalBackgroundKey");

        // Brush Keys for colors defined above
        public static readonly ComponentResourceKey ControlNormalForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlNormalForegroundBrushKey");
        public static readonly ComponentResourceKey ControlNormalBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlNormalBackgroundBrushKey");
        #endregion Normal Control Foreground and Background Keys

        #region MouseOver Keys
        public static readonly ComponentResourceKey ControlMouseOverBackgroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlMouseOverBackgroundKey");
        public static readonly ComponentResourceKey ControlMouseOverBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlMouseOverBackgroundBrushKey");
        #endregion

        // Black Color Definition
        public static readonly ComponentResourceKey OverlayColorKey = new ComponentResourceKey(typeof(ResourceKeys), "OverlayColorKey");
        public static readonly ComponentResourceKey OverlayBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "OverlayBrushKey");

        // Non-Color Keys
        public static readonly ComponentResourceKey WindowButtonStyleKey = new ComponentResourceKey(typeof(ResourceKeys), "WindowButtonStyleKey");
    }
}
