namespace Edi.Dialogs.FindReplace
{
	/// <summary>
	/// Define enumeration to state whether current document, all open documents
	/// or any other scope is to be searched.
	/// </summary>
	public enum SearchScope
	{
		/// <summary>
		/// Current document in editor (if any) is to be searched.
		/// </summary>
		CurrentDocument,

		/// <summary>
		/// All open documents in editor (if any) are to be searched.
		/// </summary>
		AllDocuments
	}
}
