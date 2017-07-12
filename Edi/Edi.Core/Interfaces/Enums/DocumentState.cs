namespace Edi.Core.Interfaces.Enums
{
	/// <summary>
	/// Enumerate the state of the document to enable a corresponding dynamic display.
	/// </summary>
	public enum DocumentState
	{
		/// <summary>
		/// Identifies a state that was probably not fully initialized.
		/// Getting of state can indicate a defect in the software.
		/// </summary>
		IsInvalid = -1,

		/// <summary>
		/// Document is loading and cannot be view, yet
		/// </summary>
		IsLoading = 0,

		/// <summary>
		/// Document is loaded and can either be viewed (readonly) or edited
		/// </summary>
		IsEditing = 1
	}
}