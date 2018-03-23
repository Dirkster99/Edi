using System.IO;

namespace Edi.Core.Interfaces
{
	public interface IOutput
	{
		TextWriter Writer { get; }
		void AppendLine(string text);
		void Append(string text);
		void Clear();
	}
}