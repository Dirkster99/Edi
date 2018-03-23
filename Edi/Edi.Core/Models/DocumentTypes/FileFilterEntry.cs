using Edi.Core.Interfaces.DocumentTypes;

namespace Edi.Core.Models.DocumentTypes
{
	internal class FileFilterEntry : IFileFilterEntry
	{
		#region constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="fileFilter"></param>
		/// <param name="fileOpenMethod"></param>
		public FileFilterEntry(string fileFilter, FileOpenDelegate fileOpenMethod)
		{
			FileFilter = fileFilter;
			FileOpenMethod = fileOpenMethod;
		}
		#endregion constructors

		#region properties
		public string FileFilter { get; }

		public FileOpenDelegate FileOpenMethod { get; }
		#endregion properties
	}
}
