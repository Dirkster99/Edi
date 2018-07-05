namespace MiniUML.View.Themes
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
        #region RubberBand
        public static readonly ComponentResourceKey Rubberband_OuterBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "Rubberband_OuterBorderBrushKey");
        public static readonly ComponentResourceKey Rubberband_InnerBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "Rubberband_InnerBorderBrushKey");
        public static readonly ComponentResourceKey Rubberband_BackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "Rubberband_BackgroundBrushKey");
        #endregion RubberBand

        #region Anchor Point Movable Thumb at Start and End of Line
        public static readonly ComponentResourceKey AchorPoint_BorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "AchorPoint_BorderBrushKey");
        public static readonly ComponentResourceKey AchorPoint_BackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "AchorPoint_BackgroundBrushKey");
        #endregion Anchor Point Movable Thumb at Start and End of Line

        #region ResizeChrome in DesignItem
        public static readonly ComponentResourceKey ThumbCorner_BorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ThumbCorner_BorderBrushKey");
        public static readonly ComponentResourceKey ThumbCorner_BackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ThumbCorner_BackgroundBrushKey");

        public static readonly ComponentResourceKey ResizeChrome_BorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ResizeChrome_BorderBrushKey");
        #endregion ResizeChrome in DesignItem
        #endregion Brush Keys
    }
}
