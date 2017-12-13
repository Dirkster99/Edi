namespace MWindowInterfacesLib.Interfaces
{
    using MWindowInterfacesLib.Enums;

    /// <summary>
    /// Defines the properties that can be set to control the behaviour
    /// and features of a content dialog.
    /// </summary>
    public interface IMetroDialogFrameSettings
    {
        /// <summary>
        /// Gets/sets whether an animation is shown upon closing a dialog.
        /// </summary>
        bool AnimateHide { get; set; }

        /// <summary>
        /// Gets/sets whether an animation is shown upon opening a dialog.
        /// </summary>
        bool AnimateShow { get; set; }

        /// <summary>
        /// Gets/sets whether static (non-async) message boxes are shown
        /// as (fixed, moveable) external message box or not.
        /// </summary>
        StaticMsgBoxModes MsgBoxMode { get; set; }
    }
}
