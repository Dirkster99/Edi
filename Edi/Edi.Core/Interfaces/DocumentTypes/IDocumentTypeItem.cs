using System.Collections.Generic;

namespace Edi.Core.Interfaces.DocumentTypes
{
	/// <summary>
	/// This interface specifies the data items necessary to descripe a document type
	/// in terms of prefered filters, a human readable short description etc.
	/// </summary>
	public interface IDocumentTypeItem
	{
		/// <summary>
		/// Gets a list of prefered file extension filter(s) for this document type.
		/// eg: "*.*"
		/// </summary>
		List<string> DocFileTypeExtensions { get; }

		/// <summary>
		/// Gets the description of this document type.
		/// eg: 'All Files'
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Gets the sort priority which can be used to cluster similar
		/// document types with a similar priority.
		/// </summary>
		int SortPriority { get; }
	}
}
