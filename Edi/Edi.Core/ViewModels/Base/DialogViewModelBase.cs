namespace Edi.Core.ViewModels.Base
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Input;
    using Edi.Core.ViewModels.Command;
    using Edi.Util.Local;

    /// <summary>
    /// ViewModel base class to support dialog based views
    /// (or views in general that support OK/Cancel functions)
    /// </summary>
    public class DialogViewModelBase : ViewModelBase
	{
		#region fields
		private bool? _mDialogCloseResult;

		private bool _mShutDownInProgress;
		private bool _mIsReadyToClose;

		private RelayCommand _mCancelCommand;
		private RelayCommand _mOkCommand;

		private ObservableCollection<Msg> _mProblems;

		private bool _mFoundErrorsInLastRun = true;
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

			_mDialogCloseResult = copyThis._mDialogCloseResult;

			_mShutDownInProgress = copyThis._mShutDownInProgress;
			_mIsReadyToClose = copyThis._mIsReadyToClose;

			// Commands cannot be copied (but must be initialized again) because
			// we otherwise end up calling the the source of the copy instead of a method in this.
			_mOkCommand = _mCancelCommand = null;

			_mProblems = new ObservableCollection<Msg>(copyThis._mProblems);

			_mFoundErrorsInLastRun = copyThis._mFoundErrorsInLastRun;
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
            get { return _mDialogCloseResult; }

			private set
			{
				if (_mDialogCloseResult != value)
				{
					_mDialogCloseResult = value;
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
            get { return _mIsReadyToClose; }

			set
			{
				if (_mIsReadyToClose != value)
				{
					_mIsReadyToClose = value;
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
			{ return _mCancelCommand ?? (_mCancelCommand = new RelayCommand(() => { OnRequestClose(false); })); }
		}

		/// <summary>
		/// Execute the OK command (occurs typically when a user clicks OK in the dialog)
		/// </summary>
		public ICommand OkCommand
		{
			get
			{
				return _mOkCommand ?? (_mOkCommand = new RelayCommand(() =>
				{
					// Check user input and perform exit if data input is OK
					PerformInputDataEvaluation();

					if (IsReadyToClose)
					{
						OnRequestClose(true);
					}
				}));
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

		public ObservableCollection<Msg> ListMessages => _mProblems ?? (_mProblems = new ObservableCollection<Msg>());

		#endregion properties

		#region methods
		/// <summary>
		/// Reset the viewmodel such that opening a view (dialog) is realized with known states.
		/// </summary>
		public void InitializeDialogState()
		{
			ProblemCaption = Strings.STR_DIALOG_INPUT_PROBLEM_CAPTION;

			EvaluateInputData = null;

			_mIsReadyToClose = true;
			_mShutDownInProgress = false;
			_mDialogCloseResult = null;

			RequestClose = null;

			_mProblems = new ObservableCollection<Msg>();
		}

		/// <summary>
		/// Method to be executed when user (or program) tries to close the application
		/// </summary>
		public void OnRequestClose(bool setWindowCloseResult)
		{
			try
			{
				if (_mShutDownInProgress == false)
				{
					WindowCloseResult = setWindowCloseResult;

					_mShutDownInProgress = true;
					EventHandler handler = RequestClose;

					handler?.Invoke(this, EventArgs.Empty);
				}
			}
			catch (Exception exp)
			{
				Console.WriteLine(@"Exception occurred in OnRequestClose
{0}", exp);
				_mShutDownInProgress = false;
			}
		}

		public void ClearMessages()
		{
			if (_mProblems == null)
				_mProblems = new ObservableCollection<Msg>();

			_mProblems.Clear();
		}

		public void AddMessage(string strMessage, Msg.MsgCategory categoryOfMsg = Msg.MsgCategory.Error)
		{
			AddMessage(new Msg(strMessage, categoryOfMsg));
		}

		public void AddMessage(Msg inputMsg)
		{
			if (_mProblems == null)
				_mProblems = new ObservableCollection<Msg>();

			_mProblems.Add(new Msg(inputMsg));
		}

		/// <summary>
		/// Determine whether Dialog is ready to close down or
		/// whether close down should be cancelled - and cancel it.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnClosing(object sender, CancelEventArgs e)
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
            if (this.EvaluateInputData != null)
            {
                List<Edi.Core.Msg> msgs;
                bool bResult = this.EvaluateInputData(out msgs);
                bool bFoundErrors = false;

                // Copy messages from delegate method (if any)
                this.ClearMessages();

                if (msgs != null)
                {
                    foreach (Edi.Core.Msg m in msgs)
                    {
                        if (m.CategoryOfMsg != Edi.Core.Msg.MsgCategory.Information && m.CategoryOfMsg != Edi.Core.Msg.MsgCategory.Warning)
                            bFoundErrors = true;

                        this.AddMessage(m);
                    }
                }

                if (bFoundErrors == false)
                {
                    if (_mFoundErrorsInLastRun == false)
                    {
                        // Found only Information or Warnings for the second time -> lets get over it!
                        IsReadyToClose = true;
                        return;
                    }
                    else
                    {
                        _mFoundErrorsInLastRun = false;
                        IsReadyToClose = bResult;
                        return;
                    }
                }

                _mFoundErrorsInLastRun = true;
                IsReadyToClose = bResult;
            }
        }
        #endregion methods
    }
}
