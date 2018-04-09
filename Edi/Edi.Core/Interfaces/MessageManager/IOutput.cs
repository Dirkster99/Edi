namespace Edi.Core.Interfaces
{
    using System.IO;

    public interface IOutput
	{
		TextWriter Writer { get; }
		void AppendLine(string text);
		void Append(string text);
		void Clear();
	}
}