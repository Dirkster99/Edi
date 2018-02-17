namespace Output.ViewModels
{
	using System.IO;
	using System.Text;
	using Edi.Core.Interfaces;

	internal class OutputWriter : TextWriter
	{
		private readonly IOutput _output;

		public override Encoding Encoding => Encoding.Default;

	    public OutputWriter(IOutput output)
		{
			_output = output;
		}

		public override void WriteLine()
		{
			_output.AppendLine(string.Empty);
		}

		public override void WriteLine(string value)
		{
			_output.AppendLine(value);
		}

		public override void Write(string value)
		{
			_output.Append(value);
		}
	}
}