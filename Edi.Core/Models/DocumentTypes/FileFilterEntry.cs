namespace Edi.Core.Models.DocumentTypes
{
	using System;
	using Edi.Core.Interfaces.DocumentTypes;

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
			this.FileFilter = fileFilter;
			this.FileOpenMethod = fileOpenMethod;
		}
		#endregion constructors

		#region properties
		public string FileFilter { get; private set; }

		public FileOpenDelegate FileOpenMethod { get; private set; }
		#endregion properties
	}
}
