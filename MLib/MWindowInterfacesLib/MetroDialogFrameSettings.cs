namespace MWindowInterfacesLib
{
    using Enums;
    using MWindowInterfacesLib.Interfaces;

    /// <summary>
    /// Defines the properties that can be set to control the behaviour
    /// and features of a content dialog.
    /// </summary>
    public class MetroDialogFrameSettings : IMetroDialogFrameSettings
    {
        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public MetroDialogFrameSettings()
        {
            AnimateShow = AnimateHide = false;
            MsgBoxMode = StaticMsgBoxModes.InternalFixed;
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets/sets whether an animation is shown upon closing a dialog.
        /// </summary>
        public bool AnimateHide
        {
            get; set;
        }

        /// <summary>
        /// Gets/sets whether an animation is shown upon opening a dialog.
        /// </summary>
        public bool AnimateShow
        {
            get; set;
        }

        /// <summary>
        /// Gets/sets whether static (non-async) message boxes are shown
        /// as (fixed, moveable) external message box or not.
        /// </summary>
        public StaticMsgBoxModes MsgBoxMode { get; set; }
        #endregion properties
    }
}
