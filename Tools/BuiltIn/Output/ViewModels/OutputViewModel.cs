namespace Output.ViewModels
{
	using System;
	using System.ComponentModel.Composition;
	using System.IO;
	using Edi.Core.Interfaces;
	using Edi.Core.Interfaces.Enums;
	using Edi.Core.ViewModels;

	/// <summary>
	/// Implementation is based on Output Tool Window from Gemini project:
	/// https://github.com/tgjones/gemini
	/// </summary>
	[Export(typeof(IOutput))]
	public class OutputViewModel : ToolViewModel, IOutput
	{
		#region fields
		private readonly OutputWriter _writer;

	    public const string ToolContentId = "<OutputToolWindow>";
		#endregion fields

		#region constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		public OutputViewModel()
		 : base ("Output")
		{
			_writer = new OutputWriter(this);

			Text = new TextViewModel();

			ContentId = ToolContentId;
		}
		#endregion constructors

		#region properties
		public override Uri IconSource => new Uri("pack://application:,,,/Output;component/Images/MetroLight/appbar.monitor.png", UriKind.RelativeOrAbsolute);

	    /// <summary>
		/// Implements the <seealso cref="IOutput"/> interface.
		/// </summary>
		public TextWriter Writer => _writer;

	    public TextViewModel Text { get; }

	    public override PaneLocation PreferredLocation => PaneLocation.Bottom;

	    #endregion properties

		#region methods
		/// <summary>
		/// Implements the <seealso cref="IOutput"/> interface.
		/// </summary>
		public void Clear()
		{
			Text.Clear();
		}

		/// <summary>
		/// Implements the <seealso cref="IOutput"/> interface.
		/// </summary>
		public void AppendLine(string text)
		{
			Text.Append(text + Environment.NewLine);
		}

		/// <summary>
		/// Implements the <seealso cref="IOutput"/> interface.
		/// </summary>
		public void Append(string text)
		{
			Text.Append(text);
		}
		#endregion methods
	}
}