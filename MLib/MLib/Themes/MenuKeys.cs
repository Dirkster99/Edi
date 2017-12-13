namespace MLib.Themes
{
    using System.Windows;

    public static class MenuKeys
    {
        //public static readonly ComponentResourceKey BackgroundSelectedKey = new ComponentResourceKey(typeof(ResourceKeys), "BackgroundSelectedKey");
        public static readonly ComponentResourceKey MenuSeparatorBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "MenuSeparatorBorderBrushKey");

        public static readonly ComponentResourceKey SubmenuItemBackgroundKey = new ComponentResourceKey(typeof(ResourceKeys), "SubmenuItemBackgroundKey");

        // Highlighing Top Menu Entry on Mouse over
        public static readonly ComponentResourceKey MenuItemHighlightedBackgroundKey = new ComponentResourceKey(typeof(ResourceKeys), "MenuItemHighlightedBackgroundKey");

        // Highlighting all other than Top Menu entry on mouse over
        public static readonly ComponentResourceKey SubmenuItemBackgroundHighlightedKey = new ComponentResourceKey(typeof(ResourceKeys), "SubmenuItemBackgroundHighlightedKey");

        // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
        public static readonly ComponentResourceKey FocusScrollButtonBrushKey             = new ComponentResourceKey(typeof(ResourceKeys), "FocusScrollButtonBrushKey");
        public static readonly ComponentResourceKey ScrollButtonBrushKey                  = new ComponentResourceKey(typeof(ResourceKeys), "ScrollButtonBrushKey");
        public static readonly ComponentResourceKey CheckMarkBackgroundBrushKey           = new ComponentResourceKey(typeof(ResourceKeys), "CheckMarkBackgroundBrushKey");
        public static readonly ComponentResourceKey CheckMarkBorderBrushKey               = new ComponentResourceKey(typeof(ResourceKeys), "CheckMarkBorderBrushKey");
        public static readonly ComponentResourceKey CheckMarkForegroundBrushKey           = new ComponentResourceKey(typeof(ResourceKeys), "CheckMarkForegroundBrushKey");

        public static readonly ComponentResourceKey DisabledSubMenuItemBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "DisabledSubMenuItemBackgroundBrushKey");
        public static readonly ComponentResourceKey DisabledSubMenuItemBorderBrushKey     = new ComponentResourceKey(typeof(ResourceKeys), "DisabledSubMenuItemBorderBrushKey");

        public static readonly ComponentResourceKey MenuBorderBrushKey                    = new ComponentResourceKey(typeof(ResourceKeys), "MenuBorderBrushKey");

        public static readonly ComponentResourceKey MenuBackgroundKey                     = new ComponentResourceKey(typeof(ResourceKeys), "MenuBackgroundKey");

        // Styles the background of the top level item in a menu (Files, Edit, ...)
        public static readonly ComponentResourceKey TopLevelHeaderMenuBackgroundKey       = new ComponentResourceKey(typeof(ResourceKeys), "TopLevelHeaderMenuBackgroundKey");

        // XXXXXXXXXXXXXXXXXXXXXXXXXX ControlKeys
        public static readonly ComponentResourceKey TextBrushKey              = new ComponentResourceKey(typeof(ResourceKeys), "TextBrushKey");
        public static readonly ComponentResourceKey ItemBackgroundSelectedKey = new ComponentResourceKey(typeof(ResourceKeys), "ItemBackgroundSelectedKey");
        public static readonly ComponentResourceKey ItemTextDisabledKey       = new ComponentResourceKey(typeof(ResourceKeys), "ItemTextDisabledKey");
        public static readonly ComponentResourceKey NormalBackgroundBrushKey  = new ComponentResourceKey(typeof(ResourceKeys), "NormalBackgroundBrushKey");
        public static readonly ComponentResourceKey ItemBackgroundHoverKey    = new ComponentResourceKey(typeof(ResourceKeys), "ItemBackgroundHoverKey");

        public static readonly ComponentResourceKey DropShadowEffectKey       = new ComponentResourceKey(typeof(ResourceKeys), "DropShadowEffectKey");
    }
}
