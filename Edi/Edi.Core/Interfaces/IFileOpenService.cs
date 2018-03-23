namespace Edi.Core.Interfaces
{
	public interface IFileOpenService
	{
		/// <summary>
		/// Wrapper method for file open
		/// - is executed when a file open is requested from external party such as tool window.
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		bool FileOpen(string file);
	}
}