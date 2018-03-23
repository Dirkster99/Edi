using Edi.Core.ViewModels;

namespace Edi.Core.Interfaces
{
	/// <summary>
	/// A document parent is a viewmodel that holds the collection of documents
	/// and can inform other objects when the active document changes.
	/// </summary>
	public interface IDocumentParent
	{
		/// <summary>
		/// This event is raised when the active document changes
		/// (a new/different or no document becomes active).
		/// </summary>
		event DocumentChangedEventHandler ActiveDocumentChanged;

		/// <summary>
		/// Gets the viewmodel of the currently selected document.
		/// </summary>
		IFileBaseViewModel ActiveDocument
		{
			get;
			set;
		}

		////void Close(IDocument fileToClose);
		////void Save(IDocument fileToSave, bool saveAsFlag = false);
	}
}
