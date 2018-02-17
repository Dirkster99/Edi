namespace Edi.Core.ViewModels.Base
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Windows.Input;
	using Command;

	/// <summary>
	/// ViewModel base class to support dialog based views
	/// (or views in general that support OK/Cancel functions)
	/// </summary>
	public class DialogViewModelBase : ViewModelBase
	{
		#region fields
		private bool? mDialogCloseResult;

		private bool mShutDownInProgress;
		private bool mIsReadyToClose;

		private RelayCommand mCancelCommand;
		private RelayCommand mOKCommand;

		private ObservableCollection<Msg> mProblems;

		private bool mFoundErrorsInLastRun = true;
		#endregion fields

		#region constructor
		/// <summary>
		/// Standard constructor
		/// </summary>
		public DialogViewModelBase()
		{
			InitializeDialogState();
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="copyThis"></param>
		public DialogViewModelBase(DialogViewModelBase copyThis)
		{
			if (copyThis == null) return;

			mDialogCloseResult = copyThis.mDialogCloseResult;

			mShutDownInProgress = copyThis.mShutDownInProgress;
			mIsReadyToClose = copyThis.mIsReadyToClose;

			// Commands cannot be copied (but must be initialized again) because
			// we otherwise end up calling the the source of the copy instead of a method in this.
			mOKCommand = mCancelCommand = null;

			mProblems = new ObservableCollection<Msg>(copyThis.mProblems);

			mFoundErrorsInLastRun = copyThis.mFoundErrorsInLastRun;
		}
		#endregion constructor

		#region delegates
		/// <summary>
		/// This type of method delegation is used when user input is evaluated.
		/// 
		/// The user input is available in the class that instantiates the <seealso cref="DialogViewModelBase"/> class.
		/// Therefore, the <seealso cref="DialogViewModelBase"/> class calls a delegate method to retrieve whether input
		/// is OK or not plus a list messages describing problem details.
		/// </summary>
		/// <param name="outMsg"></param>
		/// <returns></returns>
		public delegate bool EvaluateInput(out List<Msg> outMsg);
		#endregion delegates

		#region event
		/// <summary>
		/// Raised when this workspace should be removed from the UI.
		/// </summary>
		public event EventHandler RequestClose;
		#endregion event

		#region properties
		/// <summary>
		/// This can be used to close the attached view via ViewModel
		/// 
		/// Source: http://stackoverflow.com/questions/501886/wpf-mvvm-newbie-how-should-the-viewmodel-close-the-form
		/// </summary>
		public bool? WindowCloseResult
		{
			get
			{
				return mDialogCloseResult;
			}

			private set
			{
				if (mDialogCloseResult != value)
				{
					mDialogCloseResult = value;
					RaisePropertyChanged(() => WindowCloseResult);
				}
			}
		}

		/// <summary>
		/// Get/set proprety to determine whether application is ready to close
		/// (the setter is public here to bind it to a checkbox - in a normal
		/// application that setter would more likely be private and be set via
		/// a corresponding method call that manages/overrides the properties' value).
		/// </summary>
		public bool IsReadyToClose
		{
			get
			{
				return mIsReadyToClose;
			}

			set
			{
				if (mIsReadyToClose != value)
				{
					mIsReadyToClose = value;
					RaisePropertyChanged(() => IsReadyToClose);
				}
			}
		}

		/// <summary>
		/// This property can be used to delegate the test of user input to the class that instantiates this class.
		/// User input is then, based on the external method, evaluated whenver a user executes the Cancel or OK command.
		/// 
		/// The user input is available in the class that instantiates the <seealso cref="DialogViewModelBase"/> class.
		/// Therefore, the <seealso cref="DialogViewModelBase"/> class calls a delegate method to retrieve whether input
		/// is OK or not plus a list messages describing problem details.
		/// </summary>
		public EvaluateInput EvaluateInputData
		{
			get;
			set;
		}

		/// <summary>
		/// Execute the cancel command (occurs typically when a user clicks cancel in the dialog)
		/// </summary>
		public ICommand CancelCommand
		{
			get
			{
				if (mCancelCommand == null)
					mCancelCommand = new RelayCommand(() =>
					{
						OnRequestClose(false);
					});

				return mCancelCommand;
			}
		}

		/// <summary>
		/// Execute the OK command (occurs typically when a user clicks OK in the dialog)
		/// </summary>
		public ICommand OKCommand
		{
			get
			{
				if (mOKCommand == null)
					mOKCommand = new RelayCommand(() =>
					{
						// Check user input and perform exit if data input is OK
						PerformInputDataEvaluation();

						if (IsReadyToClose)
						{
							OnRequestClose(true);
						}
					});

				return mOKCommand;
			}
		}

		/// <summary>
		/// This string can be displayed when the list of problems <seealso cref="ListMessages"/> is displayed.
		/// </summary>
		public string ProblemCaption
		{
			get;
			set;
		}

		public ObservableCollection<Msg> ListMessages
		{
			get
			{
				if (mProblems == null)
					mProblems = new ObservableCollection<Msg>();

				return mProblems;
			}
		}
		#endregion properties

		#region methods
		/// <summary>
		/// Reset the viewmodel such that opening a view (dialog) is realized with known states.
		/// </summary>
		public void InitializeDialogState()
		{
			ProblemCaption = Util.Local.Strings.STR_DIALOG_INPUT_PROBLEM_CAPTION;

			EvaluateInputData = null;

			mIsReadyToClose = true;
			mShutDownInProgress = false;
			mDialogCloseResult = null;

			RequestClose = null;

			mProblems = new ObservableCollection<Msg>();
		}

		/// <summary>
		/// Method to be executed when user (or program) tries to close the application
		/// </summary>
		public void OnRequestClose(bool setWindowCloseResult)
		{
			try
			{
				if (mShutDownInProgress == false)
				{
					WindowCloseResult = setWindowCloseResult;

					mShutDownInProgress = true;
					EventHandler handler = RequestClose;

					if (handler != null)
						handler(this, EventArgs.Empty);
				}
			}
			catch (Exception exp)
			{
				Console.WriteLine("Exception occurred in OnRequestClose\n{0}", exp.ToString());
				mShutDownInProgress = false;
			}
		}

		public void ClearMessages()
		{
			if (mProblems == null)
				mProblems = new ObservableCollection<Msg>();

			mProblems.Clear();
		}

		public void AddMessage(string strMessage, Msg.MsgCategory categoryOfMsg = Msg.MsgCategory.Error)
		{
			AddMessage(new Msg(strMessage, categoryOfMsg));
		}

		public void AddMessage(Msg inputMsg)
		{
			if (mProblems == null)
				mProblems = new ObservableCollection<Msg>();

			mProblems.Add(new Msg(inputMsg));
		}

		/// <summary>
		/// Determine whether Dialog is ready to close down or
		/// whether close down should be cancelled - and cancel it.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (WindowCloseResult == null) // Process window close button as Cancel (IsReadyToClose is not evaluated)
				return;

			if (WindowCloseResult == false)
			{
				e.Cancel = false;
				return;
			}

			e.Cancel = !IsReadyToClose; // Cancel close down request if ViewModel is not ready, yet
		}

		/// <summary>
		/// Call the external method delegation (if any) to verify whether user input is valid or not.
		/// </summary>
		private void PerformInputDataEvaluation()
		{
			if (EvaluateInputData != null)
			{
				List<Msg> msgs;
				bool bResult = EvaluateInputData(out msgs);
				bool bFoundErrors = false;

				// Copy messages from delegate method (if any)
				ClearMessages();

				if (msgs != null)
				{
					foreach (Msg m in msgs)
					{
						if (m.CategoryOfMsg != Msg.MsgCategory.Information && m.CategoryOfMsg != Msg.MsgCategory.Warning)
							bFoundErrors = true;

						AddMessage(m);
					}
				}

				if (bFoundErrors == false)
				{
					if (mFoundErrorsInLastRun == false)
					{
						// Found only Information or Warnings for the second time -> lets get over it!
						IsReadyToClose = true;
						return;
					}
					else
					{
						mFoundErrorsInLastRun = false;
						IsReadyToClose = bResult;
						return;
					}
				}

				mFoundErrorsInLastRun = true;
				IsReadyToClose = bResult;
			}
		}
		#endregion methods
	}
}
