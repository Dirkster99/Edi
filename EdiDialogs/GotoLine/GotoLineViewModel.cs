namespace EdiDialogs.GotoLine
{
	using System.Collections.Generic;
	using System.Globalization;
	using Edi.Core.ViewModels.Base;

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
			this.mMin = iMin;
			this.mMax = iMax;
			this.iCurrentLine = iCurrentLine;

			this.mLineNumberInput = iCurrentLine.ToString();

			this.EvaluateInputData = this.ValidateData;
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
				return (this.mLineNumberInput == null ? string.Empty : this.mLineNumberInput);
			}

			set
			{
				if (this.mLineNumberInput != value)
				{
					this.mLineNumberInput = value;

					this.RaisePropertyChanged(() => this.LineNumberInput);
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
					iNumber = int.Parse(this.mLineNumberInput);
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
				return "(" + this.mMin.ToString() + " - " + this.mMax.ToString() + ")";
			}
		}

		public string SelectedText
		{
			get
			{
				return this.mSelectedText;
			}

			set
			{
				if (this.mSelectedText != value)
				{
					this.mSelectedText = value;
					this.RaisePropertyChanged(() => this.SelectedText);
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
		private bool ValidateData(out List<Edi.Core.Msg> listMsgs)
		{
			bool Error = true;
			listMsgs = new List<Edi.Core.Msg>();

			try
			{
				int iNumber = 0;
				try
				{
					iNumber = int.Parse(this.mLineNumberInput);
				}
				catch
				{
					listMsgs.Add(new Edi.Core.Msg(string.Format(CultureInfo.CurrentCulture, "The entered number '{0}' is not valid. Enter a valid number.", this.mLineNumberInput),
																				Edi.Core.Msg.MsgCategory.Error));

					Error = !(listMsgs.Count > 0);
					return Error;
				}

				if (iNumber < this.mMin || iNumber > this.mMax)
					listMsgs.Add(new Edi.Core.Msg(string.Format(CultureInfo.CurrentCulture, "The entered number '{0}' is not within expected range ({1} - {2}).", iNumber, this.mMin, this.mMax),
																				Edi.Core.Msg.MsgCategory.Error));

				Error = !(listMsgs.Count > 0);
			}
			finally
			{
				if (Error == false)                          // Select complete text on error such that user can re-type string from scratch
					this.SelectedText = this.LineNumberInput;
			}

			return Error;
		}
		#endregion methods
	}
}
