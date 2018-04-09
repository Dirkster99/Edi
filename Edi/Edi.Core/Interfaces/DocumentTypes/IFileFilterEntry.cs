namespace Edi.Core.Interfaces.DocumentTypes
{
	public interface IFileFilterEntry
	{
		string FileFilter { get; }

		FileOpenDelegate FileOpenMethod { get; }
	}
}