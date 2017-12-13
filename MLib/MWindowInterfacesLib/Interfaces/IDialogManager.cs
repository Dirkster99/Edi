namespace MWindowInterfacesLib.Interfaces
{
    using Events;
    using MsgBox.Enums;
    using System;
    using System.Threading.Tasks;

    public interface IDialogManager
    {
        #region events
        event EventHandler<DialogStateChangedEventArgs> DialogOpened;
        event EventHandler<DialogStateChangedEventArgs> DialogClosed;
        #endregion events

        #region methods
        Task HideMetroDialogAsync(IMetroWindow metroWindow
                                , IBaseMetroDialogFrame dialog
                                , IMetroDialogFrameSettings settings = null);

        /// <summary>
        /// Creates a MsgBoxDialog inside of the current window.
        /// </summary>
        /// <param name="window">The MetroWindow</param>
        /// <param name="title">The title of the MessageDialog.</param>
        /// <param name="message">The message contained within the MessageDialog.</param>
        /// <param name="settings">Optional settings that override the global metro dialog settings.</param>
        /// <returns>A task promising the result of which button was pressed.</returns>
        Task<MsgBoxResult> ShowMsgBoxAsync(
              IMetroWindow metroWindow
            , IMsgBoxDialogFrame<MsgBoxResult> dialog
            , IMetroDialogFrameSettings settings = null);

        Task ShowMetroDialogAsync(IMetroWindow metroWindow
                                , IBaseMetroDialogFrame dialog
                                , IMetroDialogFrameSettings settings = null);

        Task<int> ShowMetroDialogAsync(IMetroWindow metroWindow
                                        , IMsgBoxDialogFrame<int> dialog
                                        , IMetroDialogFrameSettings settings = null);

        /// <summary>
        /// Creates a custom dialog outside of the current window.
        /// </summary>
        /// <param name="metroWindow">The MetroWindow</param>
        /// <param name="dialog">The outside modal window to be owned by a given <seealso cref="MetroWindow"/></param>
        /// <param name="settings">Optional settings that override the global metro dialog settings.</param>
        /// <returns>The result event that was generated to close the dialog (button click).</returns>
        int ShowModalDialogExternal(
              IMetroWindow metroWindow
            , IMsgBoxDialogFrame<int> dialog
            , IMetroDialogFrameSettings settings = null);

        /// <summary>
        /// Creates an External MsgBox dialog outside of the current window.
        /// </summary>
        /// <param name="metroWindow">The MetroWindow</param>
        /// <param name="dialog">The outside modal window to be owned by a given <seealso cref="MetroWindow"/></param>
        /// <param name="settings">Optional settings that override the global metro dialog settings.</param>
        /// <returns>The result event that was generated to close the dialog (button click).</returns>
        MsgBoxResult ShowModalDialogExternal(
              IMetroWindow metroWindow
            , IMsgBoxDialogFrame<MsgBoxResult> dialog
            , IMetroDialogFrameSettings settings = null);
        #endregion methods
    }
}