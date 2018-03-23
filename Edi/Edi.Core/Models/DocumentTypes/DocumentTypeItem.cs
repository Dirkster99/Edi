using System.Collections.Generic;
using Edi.Core.Interfaces.DocumentTypes;

namespace Edi.Core.Models.DocumentTypes
{
	internal class DocumentTypeItem : IDocumentTypeItem
	{
		#region fields
		#endregion fields

		#region constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		public DocumentTypeItem(string description, List<string> extensions, int sortPriority = 0)
		{
			Description = description;
			DocFileTypeExtensions = extensions;
			SortPriority = sortPriority;
		}
		#endregion constructors

		#region properties
		public List<string> DocFileTypeExtensions { get; }

		public string Description { get; }

		public int SortPriority { get; }
		#endregion properties

		#region methods
		#endregion methods
	}
}
