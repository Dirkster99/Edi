namespace Edi.Core.Models
{
	using System.ComponentModel.Composition;
	using Edi.Core.Interfaces;

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
		MsgBox.IMessage mMessageBox = null;
		IOutput mOutput = null;
		#endregion fields

		#region constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		public MessageManager()
		{
			this.mMessageBox = new MsgBox.Message();
		}
		#endregion constructors

		#region properties
		public MsgBox.IMessage MessageBox
		{
			get { return this.MessageBox; }
		}

		public IOutput Output
		{
			get { return this.mOutput; }
		}
		#endregion properties

		#region Methods
		public void RegisterOutputStream(IOutput output)
		{
			this.mOutput = output;
		}

		public void RegisterMessagebox(MsgBox.IMessage message)
		{
			this.mMessageBox = message;
		}
		#endregion Methods
	}
}
