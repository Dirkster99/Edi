using System;
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
	public class DocumentChangedEventArgs : EventArgs
	{
		#region fields
		private IFileBaseViewModel _mActiveDocument;
		#endregion fields

		#region constrcutor
		public DocumentChangedEventArgs(IFileBaseViewModel activeDocument)
		{
			_mActiveDocument = activeDocument;
		}
		#endregion constrcutor

		#region methods
		/// <summary>
		/// Get the active document that is active now (as of this event).
		/// </summary>
		public IFileBaseViewModel ActiveDocument => _mActiveDocument;

		#endregion methods
	}
}
