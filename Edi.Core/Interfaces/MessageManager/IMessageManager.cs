namespace Edi.Core.Interfaces
{
	public interface IMessageManager
	{
		#region properties
		MsgBox.IMessage MessageBox { get; }

		IOutput Output { get; }
		#endregion properties

		#region methods
		void RegisterOutputStream(IOutput output);

		void RegisterMessagebox(MsgBox.IMessage message);
		#endregion methods
	}
}
