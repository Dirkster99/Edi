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
		private readonly TextViewModel mText;

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

			this.mText = new TextViewModel();

			this.ContentId = OutputViewModel.ToolContentId;
		}
		#endregion constructors

		#region properties
		public override Uri IconSource
		{
			get
			{
				return new Uri("pack://application:,,,/Output;component/Images/MetroLight/appbar.monitor.png", UriKind.RelativeOrAbsolute);
			}
		}

		/// <summary>
		/// Implements the <seealso cref="IOutput"/> interface.
		/// </summary>
		public TextWriter Writer { get { return _writer; } }

		public TextViewModel Text
		{
			get
			{
				return this.mText;
			}
		}

		public override PaneLocation PreferredLocation
		{
			get { return PaneLocation.Bottom; }
		}
		#endregion properties

		#region methods
		/// <summary>
		/// Implements the <seealso cref="IOutput"/> interface.
		/// </summary>
		public void Clear()
		{
			this.mText.Clear();
		}

		/// <summary>
		/// Implements the <seealso cref="IOutput"/> interface.
		/// </summary>
		public void AppendLine(string text)
		{
			this.mText.Append(text + Environment.NewLine);
		}

		/// <summary>
		/// Implements the <seealso cref="IOutput"/> interface.
		/// </summary>
		public void Append(string text)
		{
			this.mText.Append(text);
		}
		#endregion methods
	}
}