namespace MWindowInterfacesLib.Interfaces
{
    using MsgBox;

    /// <summary>
    /// This service is the root item for all other content dialog
    /// related services in this assembly.
    /// </summary>
    public interface IContentDialogService
    {
        #region properties
        /// <summary>
        /// Gets the default dialog settings that are applied when invoking
        /// a dialog from this service.
        /// 
        /// The message box service methodes take care of this property, automatically,
        /// the methodes in all other services, <seealso cref="DialogManager"/> and
        /// <seealso cref="DialogCoordinator"/> should be invoked with this property
        /// as parameter (or will be invoked with default settings).
        /// </summary>
        IMetroDialogFrameSettings DialogSettings { get; }

        /// <summary>
        /// Gets an instance of the <seealso cref="IDialogManager"/> object.
        /// </summary>
        IDialogManager Manager { get; }

        /// <summary>
        /// Gets an instance of the <seealso cref="IDialogCoordinator"/> object.
        /// </summary>
        IDialogCoordinator Coordinator { get; }

        /// <summary>
        /// Gets a message box service that can display message boxes
        /// in a variety of different configurations.
        /// </summary>
        IMessageBoxService MsgBox { get; }
        #endregion properties
    }
}
