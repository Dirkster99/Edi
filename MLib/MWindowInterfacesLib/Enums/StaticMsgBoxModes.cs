namespace MWindowInterfacesLib.Enums
{
    /// <summary>
    /// An enum representing the state of a MessageBox dialog when it is shown through
    /// the static API calling function.
    /// </summary>
    public enum StaticMsgBoxModes
    {
        /// <summary>
        /// Message Box is shown as overlay over the current window content.
        /// </summary>
        InternalFixed = 0,

        /// <summary>
        /// Message Box is shown as modal dialog. The modal dialog is indepentent
        /// of the current content and is hosted in a seperate modal window. But the
        /// modal window is fixed over the current window such that it raises the
        /// impression that it is a content dialog (although its not technically speaking).
        /// </summary>
        ExternalFixed = 1,

        /// <summary>
        /// Message Box is shown as modal dialog that user can drag via the title bar.
        /// This type of modal dialog is consistent with the non-conent legacy Windows
        /// dialogs of all versions prior to Windows 10.
        /// </summary>
        ExternalMoveable = 2
    }
}
