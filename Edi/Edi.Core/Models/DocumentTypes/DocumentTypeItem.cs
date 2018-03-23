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
		public List<string> DocFileTypeExtensions { get; private set; }

		public string Description { get; private set; }

		public int SortPriority { get; private set; }
		#endregion properties

		#region methods
		#endregion methods
	}
}
