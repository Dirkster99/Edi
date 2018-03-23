using System.ComponentModel.Composition;
using Edi.Core.Interfaces;
using MsgBox;

namespace Edi.Core.Models
{
	/// <summary>
	/// Class registers and manages output stream channels:
	/// - MessageBox Service
	/// - Ouptput text service
	/// - (Todo) Classified (error, warning, information) message output service.
	/// </summary>
	[Export(typeof(IMessageManager))]
	public class MessageManager : IMessageManager
	{
		#region fields
		IMessageBoxService _messageBox;
		IOutput _mOutput;
		#endregion fields

		#region constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		public MessageManager()
		{
            _messageBox = new MessageBoxService();
		}
		#endregion constructors

		#region properties
        /// <summary>
        /// Gets a reference to a message box service implementation.
        /// This service should be used if user interaction is required
        /// (e.g. user is requested to click ok or yes, no etc...).
        /// </summary>
		public IMessageBoxService MessageBox
        {
			get { return _messageBox; }
		}

        /// <summary>
        /// Gets a reference to the output message servive implementation.
        /// This service can be used to output warnings or imformation
        /// that does not require user interaction.
        /// </summary>
		public IOutput Output
		{
			get { return _mOutput; }
		}
		#endregion properties

		#region Methods
        /// <summary>
        /// This method can be used to register an instance that can act as an
        /// <seealso cref="IOutput"/> service.
        /// </summary>
        /// <param name="output"></param>
		public void RegisterOutputStream(IOutput output)
		{
			_mOutput = output;
		}
		#endregion Methods
	}
}
