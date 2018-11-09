namespace Output.ViewModels
{
    using System;
    using Edi.Core.Interfaces;
    using Edi.Core.Interfaces.Enums;
    using Edi.Core.ViewModels;
    using Edi.Interfaces.MessageManager;

    /// <summary>
    /// Implementation is based on Output Tool Window from Gemini project:
    /// https://github.com/tgjones/gemini
    /// </summary>
    public class OutputTWViewModel : ToolViewModel, IOutput
	{
		#region fields
		private readonly TextViewModel _Text;

		public const string ToolContentId = "<OutputToolWindow>";
		#endregion fields

		#region constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		public OutputTWViewModel()
		 : base ("Output")
		{
			_Text = new TextViewModel();

			ContentId = OutputTWViewModel.ToolContentId;
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

		public TextViewModel Text
		{
			get
			{
				return _Text;
			}
		}

        /// <summary>
        /// Gets the preferred panel location of this tool window.
        /// </summary>
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
			_Text.Clear();
		}

		/// <summary>
		/// Implements the <seealso cref="IOutput"/> interface.
		/// </summary>
		public void AppendLine(string text)
		{
			_Text.Append(text + Environment.NewLine);
		}

		/// <summary>
		/// Implements the <seealso cref="IOutput"/> interface.
		/// </summary>
		public void Append(string text)
		{
			_Text.Append(text);
		}
		#endregion methods
	}
}