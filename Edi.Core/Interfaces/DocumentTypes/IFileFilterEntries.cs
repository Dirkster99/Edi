namespace Edi.Core.Interfaces.DocumentTypes
{
	public interface IFileFilterEntries
	{
		string GetFilterString();

		FileOpenDelegate GetFileOpenMethod(int idx);
	}
}
