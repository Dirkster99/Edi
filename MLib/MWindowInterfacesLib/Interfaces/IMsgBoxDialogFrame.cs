namespace MWindowInterfacesLib.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Controls.Primitives;

    public static class DialogIntResults
    {
        /// <summary>
        /// This is mostly technically needed for properties that implement
        /// automatic magic, such as, setting a useful default button. This
        /// magic occurs only if this default parameter is set in the
        /// constructor/interface - otherwise the button set by the caller
        /// is used.
        /// </summary>
        public const int NONE = 0;

        /// <summary>
        /// This can be used to tell the dialog sub-system explicitly to not
        /// set any default button (which is rather un-uasual but possible if
        /// the user needs to determine somthing that has a real 50:50 chance
        /// of being ansered or if there is no button to click on...).
        /// 
        /// This member should only be set in the defaultbutton parameter of the
        /// constructor but should never appear as actual result of a dialog display.
        /// </summary>
        public const int NO_DEFAULT_BUTTON = 1;


        /*** A value greater one is a value that repesents an ectual button that can close the dialog ***/

        /// <summary>
        /// The result value of the dialog  is OK.
        /// </summary>
        public const int OK = 2;

        /// <summary>
        /// The result value of the dialog is Cancel.
        /// </summary>
        public const int CANCEL = 3;
    }

    /// <summary>
    /// This interface defines a custom dialog interface that defines the type
    /// (and hence its values) of the dialog result via the <TResult> template
    /// parameter.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IMsgBoxDialogFrame<TResult> : IBaseMetroDialogFrame
    {
        /// <summary>
        /// This event is invoked to signal subscribers when the dialog is about to be closed.
        /// </summary>
        event EventHandler DialogCloseResultEvent;

        /// <summary>
        /// Bind this property between view and viemodel to have the viewmodel tell
        /// the view whether it OK to close without picking a choice (eg. yes) or not.
        /// </summary>
        bool DialogCanCloseViaChrome { get; set; }

        /// <summary>
        /// Bind this property between view and viemodel to have the viewmodel tell
        /// the view that it is time to disappear (eg. user has clicked a choice button).
        /// </summary>
        bool? DialogCloseResult { get; set; }

        Thumb DialogThumb { get; }

        /// <summary>
        /// The method keeps the dialog open until a user or process has signalled
        /// that we can close this with a result...
        /// </summary>
        /// <returns></returns>
        Task<TResult> WaitForButtonPressAsync();
    }
}
