namespace Edi.Dialogs.GotoLine
{
	using System.Collections.Generic;
	using System.Globalization;
	using Core.ViewModels.Base;

	/// <summary>
	/// This viewmodel organizes the backend of a goto line (dialog) functionality
	/// which lets the user type a number and skips the (texteditor) view to that line.
	/// 
	/// The class verifies the length of the current document and whether the entered
	/// text is a number and whether that number is within avaliable limits etc...
	/// </summary>
	public class GotoLineViewModel : DialogViewModelBase
	{
		#region fields
		private readonly int _mMin;
		private readonly int _mMax;
		private int _iCurrentLine;
		private string _mSelectedText = string.Empty;
		private string _mLineNumberInput;
		#endregion fields

		#region constructor
		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="iMin"></param>
		/// <param name="iMax"></param>
		/// <param name="iCurrentLine"></param>
		public GotoLineViewModel(int iMin, int iMax, int iCurrentLine)
		{
			_mMin = iMin;
			_mMax = iMax;
			_iCurrentLine = iCurrentLine;

			_mLineNumberInput = iCurrentLine.ToString();

			EvaluateInputData = ValidateData;
		}
		#endregion constructor

		#region properties
		/// <summary>
		/// Get the title string of the view - to be displayed in the associated view
		/// (e.g. as dialog title)
		/// </summary>
		public string WindowTitle => Util.Local.Strings.STR_GOTO_LINE_CAPTION;

		/// <summary>
		/// Get/set string representing the input line number.
		/// </summary>
		public string LineNumberInput
		{
			get => (_mLineNumberInput ?? string.Empty);

			set
			{
				if (_mLineNumberInput != value)
				{
					_mLineNumberInput = value;

					RaisePropertyChanged(() => LineNumberInput);
				}
			}
		}

		/// <summary>
		/// Get integer representing the input line number or -1 if input is invalid.
		/// </summary>
		public int LineNumber
		{
			get
			{
				int iNumber = -1;

				try
				{
					iNumber = int.Parse(_mLineNumberInput);
				}
				catch
				{
					// ignored
				}

				return iNumber;
			}
		}

		/// <summary>
		/// Get avalaiable range of line numbers that a user can choose from.
		/// </summary>
		public string MinMaxRange => "(" + _mMin.ToString() + " - " + _mMax.ToString() + ")";

		public string SelectedText
		{
			get => _mSelectedText;

			set
			{
				if (_mSelectedText != value)
				{
					_mSelectedText = value;
					RaisePropertyChanged(() => SelectedText);
				}
			}
		}
		#endregion properties

		#region methods
		/// <summary>
		/// Delegate method that is called whenever a user OKs or Cancels
		/// the view that is bound to this viewmodel.
		/// </summary>
		/// <param name="listMsgs"></param>
		/// <returns></returns>
		private bool ValidateData(out List<Core.Msg> listMsgs)
		{
			bool error = true;
			listMsgs = new List<Core.Msg>();

			try
			{
				int iNumber = 0;
				try
				{
					iNumber = int.Parse(_mLineNumberInput);
				}
				catch
				{
					listMsgs.Add(new Core.Msg(string.Format(CultureInfo.CurrentCulture, "The entered number '{0}' is not valid. Enter a valid number.", _mLineNumberInput),
																				Core.Msg.MsgCategory.Error));

					error = !(listMsgs.Count > 0);
					return error;
				}

				if (iNumber < _mMin || iNumber > _mMax)
					listMsgs.Add(new Core.Msg(string.Format(CultureInfo.CurrentCulture, "The entered number '{0}' is not within expected range ({1} - {2}).", iNumber, _mMin, _mMax),
																				Core.Msg.MsgCategory.Error));

				error = !(listMsgs.Count > 0);
			}
			finally
			{
				if (error == false)                          // Select complete text on error such that user can re-type string from scratch
					SelectedText = LineNumberInput;
			}

			return error;
		}
		#endregion methods
	}
}
