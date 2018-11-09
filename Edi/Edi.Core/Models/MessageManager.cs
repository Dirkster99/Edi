namespace Edi.Core.Models
{
    using Edi.Interfaces.MessageManager;
    using MsgBox;

    /// <summary>
    /// Class registers and manages output stream channels:
    /// - MessageBox Service
    /// - Ouptput text service
    /// - (Todo) Classified (error, warning, information) message output service.
    /// </summary>
	public class MessageManager : IMessageManager
	{
		#region fields

		#endregion fields

		#region constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		public MessageManager(IMessageBoxService msgBox)
            : this()
		{
            _MsgBox = msgBox;
		}

        protected MessageManager()
        {
        }
		#endregion constructors

		#region properties
        /// <summary>
        /// Gets a reference to a message box service implementation.
        /// This service should be used if user interaction is required
        /// (e.g. user is requested to click ok or yes, no etc...).
        /// </summary>
		public IMessageBoxService _MsgBox { get; }

		/// <summary>
        /// Gets a reference to the output message servive implementation.
        /// This service can be used to output warnings or imformation
        /// that does not require user interaction.
        /// </summary>
		public IOutput Output { get; private set; }

		#endregion properties

		#region Methods
        /// <summary>
        /// This method can be used to register an instance that can act as an
        /// <seealso cref="IOutput"/> service.
        /// </summary>
        /// <param name="output"></param>
		public void RegisterOutputStream(IOutput output)
		{
			Output = output;
		}
		#endregion Methods
	}
}
