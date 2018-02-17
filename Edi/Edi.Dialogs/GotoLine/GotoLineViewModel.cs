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
		private int mMin, mMax, iCurrentLine;
		private string mSelectedText = string.Empty;
		private string mLineNumberInput;
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
			mMin = iMin;
			mMax = iMax;
			this.iCurrentLine = iCurrentLine;

			mLineNumberInput = iCurrentLine.ToString();

			EvaluateInputData = ValidateData;
		}
		#endregion constructor

		#region properties
		/// <summary>
		/// Get the title string of the view - to be displayed in the associated view
		/// (e.g. as dialog title)
		/// </summary>
		public string WindowTitle
		{
			get
			{
				return Util.Local.Strings.STR_GOTO_LINE_CAPTION;
			}
		}

		/// <summary>
		/// Get/set string representing the input line number.
		/// </summary>
		public string LineNumberInput
		{
			get
			{
				return (mLineNumberInput ?? string.Empty);
			}

			set
			{
				if (mLineNumberInput != value)
				{
					mLineNumberInput = value;

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
					iNumber = int.Parse(mLineNumberInput);
				}
				catch
				{
				}

				return iNumber;
			}
		}

		/// <summary>
		/// Get avalaiable range of line numbers that a user can choose from.
		/// </summary>
		public string MinMaxRange
		{
			get
			{
				return "(" + mMin.ToString() + " - " + mMax.ToString() + ")";
			}
		}

		public string SelectedText
		{
			get
			{
				return mSelectedText;
			}

			set
			{
				if (mSelectedText != value)
				{
					mSelectedText = value;
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
			bool Error = true;
			listMsgs = new List<Core.Msg>();

			try
			{
				int iNumber = 0;
				try
				{
					iNumber = int.Parse(mLineNumberInput);
				}
				catch
				{
					listMsgs.Add(new Core.Msg(string.Format(CultureInfo.CurrentCulture, "The entered number '{0}' is not valid. Enter a valid number.", mLineNumberInput),
																				Core.Msg.MsgCategory.Error));

					Error = !(listMsgs.Count > 0);
					return Error;
				}

				if (iNumber < mMin || iNumber > mMax)
					listMsgs.Add(new Core.Msg(string.Format(CultureInfo.CurrentCulture, "The entered number '{0}' is not within expected range ({1} - {2}).", iNumber, mMin, mMax),
																				Core.Msg.MsgCategory.Error));

				Error = !(listMsgs.Count > 0);
			}
			finally
			{
				if (Error == false)                          // Select complete text on error such that user can re-type string from scratch
					SelectedText = LineNumberInput;
			}

			return Error;
		}
		#endregion methods
	}
}
