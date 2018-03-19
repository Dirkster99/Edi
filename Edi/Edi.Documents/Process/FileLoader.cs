namespace Edi.Documents.Process
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Threading;
    using Core.ViewModels.Events;

    /// <summary>
    /// This class implements a task based log4net loader
    /// that can run in an async task and generate an event
    /// when loading is done. The class can be used as a template
    /// for processing other tasks asynchronously.
    /// </summary>
    internal class FileLoader
    {
        #region fields
        public const string KeyLogItems = "LogItems";

        private bool _mAbortedWithCancel;
        private bool _mAbortedWithErrors;
        private CancellationTokenSource _mCancelTokenSource;

        private ApplicationException _mInnerException;
        private Dictionary<string, object> _mObjColl;

        /// <summary>
        /// This property is used to tell the context of the thread that started this thread
        /// originally. We use this to generate callbacks in the correct context when the long
        /// running task is done
        /// 
        /// Quote:
        /// One of the most important parts of this pattern is calling the MethodNameCompleted
        /// method on the same thread that called the MethodNameAsync method to begin with. You
        /// could do this using WPF fairly easily, by storing CurrentDispatcher—but then the
        /// nongraphical component could only be used in WPF applications, not in Windows Forms
        /// or ASP.NET programs. 
        /// 
        /// The DispatcherSynchronizationContext class addresses this need—think of it as a
        /// simplified version of Dispatcher that works with other UI frameworks as well.
        /// 
        /// http://msdn.microsoft.com/en-us/library/ms741870.aspx
        /// </summary>
        private DispatcherSynchronizationContext _mRequestingContext;
        #endregion fields

        #region Constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public FileLoader()
        {
            _mAbortedWithErrors = _mAbortedWithCancel = false;
            _mInnerException = null;
            _mObjColl = new Dictionary<string, object>();
        }
        #endregion Constructor

        /// <summary>
        /// This mResult is fired when the assigned processing task is finished.
        /// </summary>
        public event EventHandler<ResultEvent> ProcessingResultEvent;

        #region Properties
        public Dictionary<string, object> ResultObjects => (_mObjColl == null
            ? new Dictionary<string, object>()
            : new Dictionary<string, object>(_mObjColl));

        protected ApplicationException InnerException
        {
            get => _mInnerException;
            set => _mInnerException = value;
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Cancel Asynchronous processing (if there is any right now)
        /// </summary>
        public void Cancel()
        {
            _mCancelTokenSource?.Cancel();
        }

        /// <summary>
        /// Process an asynchronous function
        /// </summary>
        /// <param name="execFunc"></param>
        /// <param name="async"></param>
        internal void ExecuteAsynchronously(Action execFunc,
                                                                                bool async)
        {
            SaveThreadContext(async);

            _mCancelTokenSource = new CancellationTokenSource();
            CancellationToken cancelToken = _mCancelTokenSource.Token;

            Task taskToProcess = Task.Factory.StartNew(stateObj =>
                    {
                        _mAbortedWithErrors = _mAbortedWithCancel = false;
                        _mObjColl = new Dictionary<string, object>();
                        var processResults = new ObservableCollection<string>();

                        // This is not run on the UI thread.
                        try
                        {
                            cancelToken.ThrowIfCancellationRequested();
                            execFunc();
                        }
                        catch (OperationCanceledException exp)
                        {
                            _mAbortedWithCancel = true;
                            processResults.Add(exp.Message);
                        }
                        catch (Exception exp)
                        {
                            _mInnerException = new ApplicationException("Error occured", exp);
                            _mAbortedWithErrors = true;

                            processResults.Add(exp.ToString());
                        }

                        return processResults;
                        // End of async task with summary list of mResult strings
                    },
            cancelToken, cancelToken).ContinueWith(ant =>
                                                                            {
                                                                                try
                                                                                {
                                                                                    ReportBatchResultEvent(async);
                                                                                }
                                                                                catch (AggregateException aggExp)
                                                                                {
                                                                                    _mAbortedWithErrors = true;
                                                                                    throw new Exception(aggExp.Message, aggExp);
                                                                                }
                                                                            }, cancelToken);

            if (async == false) // Execute 'synchronously' via wait/block method
                taskToProcess.Wait(cancelToken);
        }

        /// <summary>
        /// Analyze AggregateException (typically returned from Task class) and return human-readable
        /// string for display in GUI.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="agg"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        protected static string PrintException(Task task, AggregateException agg, string taskName)
        {
            var sResult = string.Empty;

            foreach (Exception ex in agg.InnerExceptions)
            {
                sResult += $"{taskName} Caught exception '{ex.Message}'" + Environment.NewLine;
            }

            sResult += $"{taskName} cancelled? {task.IsCanceled}" + Environment.NewLine;

            return sResult;
        }

        /// <summary>
        /// Report a mResult to the attached eventhalders (if any) on whether execution succeded or not.
        /// </summary>
        protected void ReportBatchResultEvent(bool bAsnc)
        {
            // non-Asnyc threads are simply blocked until they finish
            // hence completed event is not required to fire
            if (bAsnc == false)
                return;

            SendOrPostCallback callback = ReportTaskCompletedEvent;
            _mRequestingContext.Post(callback, null);
            _mRequestingContext = null;
        }

        /// <summary>
        /// Save the threading context of a calling thread to enable event completion handling
        /// in original context when async task has finished (WPF, Winforms and co require this)
        /// </summary>
        /// <param name="bAsnc"></param>
        protected void SaveThreadContext(bool bAsnc)
        {
            // non-Asnyc threads are simply blocked until they finish
            // hence completed event is not required to fire
            if (bAsnc == false) return;

            if (_mRequestingContext != null)
                throw new InvalidOperationException("This component can handle only 1 processing request at a time");

            _mRequestingContext = (DispatcherSynchronizationContext)SynchronizationContext.Current;
        }

        /// <summary>
        /// Report the asynchronous task as having completed
        /// </summary>
        /// <param name="e"></param>
        private void ReportTaskCompletedEvent(object e)
        {
            if (ProcessingResultEvent != null)
            {
                if (_mAbortedWithErrors == false && _mAbortedWithCancel == false)
                    ProcessingResultEvent(this, new ResultEvent("Execution succeeded", false, false, _mObjColl));
                else
                    ProcessingResultEvent(this, new ResultEvent("Execution was not successfull", _mAbortedWithErrors,
                                                                                                _mAbortedWithCancel, _mObjColl, _mInnerException));
            }
        }
        #endregion Methods
    }
}
