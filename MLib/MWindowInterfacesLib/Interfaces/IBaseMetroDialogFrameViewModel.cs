namespace MWindowInterfacesLib.Interfaces
{
    using System.Windows.Input;

    /// <summary>
    /// Defines the base interface items that should be implemented by any
    /// viewmodel that supports a content dialog of any type. This type of
    /// viewmodel can return a <seealso cref="TResult"/> to give a callers
    /// more details than just true/false for Cancel or OK.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IBaseMetroDialogFrameViewModel<TResult>
    {
        /// <summary>
        /// Gets/sets the title (if any) of the dialog to be displayed.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Use this property to determine whether the dialog can be closed
        /// without picking a choice (e.g. OK or Cancel) or not.
        /// </summary>
        bool DialogCanCloseViaChrome { get; }

        /// <summary>
        /// Use this property to tell the view that the viewmodel would like to close now.
        /// </summary>
        bool? DialogCloseResult { get; }

        /// <summary>
        /// Gets the close command that is invoked to close this dialog.
        /// The close command is invoked when the user clicks:
        /// 1) the dialogs (x) button or
        /// 2) a button that is, for example, labelled |Close|
        /// 
        /// - if any of the above is visible
        /// - if |Close| button is bound
        /// </summary>
        ICommand CloseCommand { get; }

        /// <summary>
        /// Determines whether the dialog's close (x) button is visible or not.
        /// </summary>
        bool CloseWindowButtonVisibility { get; }

        /// <summary>
        /// Get the resulting button (that has been clicked
        /// by the user) or result event when working with the dialog.
        /// </summary>
        TResult Result { get; }

        /// <summary>
        /// Gets the default value for the result datatype.
        /// </summary>
        TResult DefaultResult { get; }

        /// <summary>
        /// Gets property to determine dialog result when user closes it
        /// via F4 or Window Close (X) button when using window chrome.
        /// </summary>
        TResult DefaultCloseResult { get; }
    }
}
