using Edi.Core.Interfaces;
namespace Edi.Core.ViewModels
{
	public delegate void DocumentChangedEventHandler(object sender, DocumentChangedEventArgs e);

	/// <summary>
	/// This kind of event should be fired by the document container when a new document becomes active.
	/// 
	/// The initial design follows this article:
	/// http://www.codeproject.com/Articles/5043/Step-by-Step-Event-handling-in-C
	/// </summary>
	public class DocumentChangedEventArgs : System.EventArgs
	{
		#region fields
		private IFileBaseViewModel mActiveDocument;
		#endregion fields

		#region constrcutor
		public DocumentChangedEventArgs(IFileBaseViewModel activeDocument)
		{
			this.mActiveDocument = activeDocument;
		}
		#endregion constrcutor

		#region methods
		/// <summary>
		/// Get the active document that is active now (as of this event).
		/// </summary>
		public IFileBaseViewModel ActiveDocument
		{
			get
			{
				return this.mActiveDocument;
			}
		}
		#endregion methods
	}
}
