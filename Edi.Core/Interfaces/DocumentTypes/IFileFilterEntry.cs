namespace Edi.Core.Interfaces.DocumentTypes
{
	using System;
	using Edi.Core.ViewModels;

	public interface IFileFilterEntry
	{
		string FileFilter { get; }

		FileOpenDelegate FileOpenMethod { get; }
	}
}
