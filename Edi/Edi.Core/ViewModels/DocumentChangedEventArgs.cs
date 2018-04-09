namespace Edi.Core.ViewModels
{
    using System;
    using Edi.Core.Interfaces;

    public delegate void DocumentChangedEventHandler(object sender, DocumentChangedEventArgs e);

	/// <summary>
	/// This kind of event should be fired by the document container when a new document becomes active.
	/// 
	/// The initial design follows this article:
	/// http://www.codeproject.com/Articles/5043/Step-by-Step-Event-handling-in-C
	/// </summary>
	public class DocumentChangedEventArgs : EventArgs
	{
		#region fields

		#endregion fields

		#region constrcutor
		public DocumentChangedEventArgs(IFileBaseViewModel activeDocument)
		{
			ActiveDocument = activeDocument;
		}
		#endregion constrcutor

		#region methods
		/// <summary>
		/// Get the active document that is active now (as of this event).
		/// </summary>
		public IFileBaseViewModel ActiveDocument { get; }

		#endregion methods
	}
}
