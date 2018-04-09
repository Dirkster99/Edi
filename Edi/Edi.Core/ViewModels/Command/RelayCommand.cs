namespace Edi.Core.ViewModels.Command
{
    using System;
    using System.Diagnostics;
    using System.Windows.Input;

    /// <summary>
    /// A command whose sole purpose is to 
    /// relay its functionality to other
    /// objects by invoking delegates. The
    /// default return value for the CanExecute
    /// method is 'true'.
    /// 
    /// Source: http://www.codeproject.com/Articles/31837/Creating-an-Internationalized-Wizard-in-WPF
    /// </summary>
    public class RelayCommand<T> : ICommand
	{
		#region Fields
		private readonly Action<T> _mExecute;
		private readonly Predicate<T> _mCanExecute;
		#endregion // Fields

		#region Constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="execute"></param>
		public RelayCommand(Action<T> execute)
			: this(execute, null)
		{
		}

		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		/// <param name="canExecute">The execution status logic.</param>
		public RelayCommand(Action<T> execute, Predicate<T> canExecute)
		{
			_mExecute = execute ?? throw new ArgumentNullException(nameof(execute));
			_mCanExecute = canExecute;
		}

		#endregion // Constructors

		#region events
		/// <summary>
		/// Eventhandler to re-evaluate whether this command can execute or not
		/// </summary>
		public event EventHandler CanExecuteChanged
		{
			add
			{
				if (_mCanExecute != null)
					CommandManager.RequerySuggested += value;
			}

			remove
			{
				if (_mCanExecute != null)
					CommandManager.RequerySuggested -= value;
			}
		}
		#endregion

		#region methods
		/// <summary>
		/// Determine whether this pre-requisites to execute this command are given or not.
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			return _mCanExecute == null ? true : _mCanExecute((T)parameter);
		}

		/// <summary>
		/// Execute the command method managed in this class.
		/// </summary>
		/// <param name="parameter"></param>
		public void Execute(object parameter)
		{
			_mExecute((T)parameter);
		}
		#endregion methods
	}

	/// <summary>
	/// A command whose sole purpose is to 
	/// relay its functionality to other
	/// objects by invoking delegates. The
	/// default return value for the CanExecute
	/// method is 'true'.
	/// </summary>
	public class RelayCommand : ICommand
	{
		#region Fields
		private readonly Action _mExecute;
		private readonly Func<bool> _mCanExecute;
		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new command that can always execute.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		public RelayCommand(Action execute)
			: this(execute, null)
		{
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="inputRc"></param>
		public RelayCommand(RelayCommand inputRc)
			: this(inputRc._mExecute, inputRc._mCanExecute)
		{
		}

		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		/// <param name="canExecute">The execution status logic.</param>
		public RelayCommand(Action execute, Func<bool> canExecute)
		{
			_mExecute = execute ?? throw new ArgumentNullException(nameof(execute));
			_mCanExecute = canExecute;
		}

		#endregion Constructors

		#region Events
		/// <summary>
		/// Eventhandler to re-evaluate whether this command can execute or not
		/// </summary>
		public event EventHandler CanExecuteChanged
		{
			add
			{
				if (_mCanExecute != null)
					CommandManager.RequerySuggested += value;
			}

			remove
			{
				if (_mCanExecute != null)
					CommandManager.RequerySuggested -= value;
			}
		}
		#endregion Events

		#region Methods
		/// <summary>
		/// Execute the attached CanExecute methode delegate (or always return true)
		/// to determine whether the command managed in this object can execute or not.
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			return _mCanExecute?.Invoke() ?? true;
		}

		/// <summary>
		/// Return the attached delegate method.
		/// </summary>
		/// <param name="parameter"></param>
		public void Execute(object parameter)
		{
			_mExecute();
		}
		#endregion Methods
	}
}