namespace Edi.Interfaces.MessageManager
{
    using MsgBox;

    /// <summary>
    /// Defines an interface to register and manage output stream channels:
    /// - MessageBox Service
    /// - Ouptput text service
    /// </summary>
    public interface IMessageManager
    {
        #region properties
        /// <summary>
        /// Gets a reference to a message box service implementation.
        /// This service should be used if user interaction is required
        /// (e.g. user is requested to click ok or yes, no etc...).
        /// </summary>
        IMessageBoxService _MsgBox { get; }

        /// <summary>
        /// Gets a reference to the output message servive implementation.
        /// This service can be used to output warnings or imformation
        /// that does not require user interaction.
        /// </summary>
        IOutput Output { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// This method can be used to register an instance that can act as an
        /// <seealso cref="IOutput"/> service.
        /// </summary>
        /// <param name="output"></param>
        void RegisterOutputStream(IOutput output);

////    void RegisterMessagebox(MsgBox.IMessageBoxService message);
        #endregion methods
    }
}