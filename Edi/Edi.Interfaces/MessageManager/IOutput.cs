namespace Edi.Interfaces.MessageManager
{

    public interface IOutput
	{
        void AppendLine(string text);

		void Append(string text);

		void Clear();
	}
}