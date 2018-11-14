namespace Edi.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Edi.Core.Interfaces;
    using Edi.Core.ViewModels;
    using Edi.Interfaces.MessageManager;

    /// <summary>
    /// Class to register and manage all tool windows in one common place.
    /// </summary>
    public class ToolWindowRegistry : IToolWindowRegistry
    {
        #region fields
        private readonly List<ToolViewModel> _mTodoTools;
        #endregion fields

        #region contructors
        /// <summary>
        /// Class constructor from <paramref name="messageManager"/> parameter
        /// to output progress registration of tool windows.
        /// </summary>
        public ToolWindowRegistry(IMessageManager messageManager)
            :this()
        {
            this.Messaging = messageManager;
        }

        /// <summary>
        /// Class constructor.
        /// </summary>
        public ToolWindowRegistry()
        {
            Tools = new ObservableCollection<ToolViewModel>();
            _mTodoTools = new List<ToolViewModel>();
        }
        #endregion contructors

        /// <summary>
        /// Implements an event that is raised when a new tool window is registered.
        /// </summary>
        public event EventHandler<RegisterToolWindowEventArgs> RegisterToolWindowEvent;

        #region properties
        /// <summary>
        /// Gets an observable collection of the available tool window viewmodels
        /// in this application.
        /// </summary>
        public ObservableCollection<ToolViewModel> Tools { get; }

        /// <summary>
        /// Gets a reference to the <see cref="IMessageManager"/> service which
        /// can be used to output information about the progress of the
        /// tool window registration.
        /// </summary>
        protected IMessageManager Messaging { get; }
		#endregion properties

		#region methods
		/// <summary>
		/// Publishs all registered tool window definitions into an observable collection.
		/// (Which in turn will execute the LayoutInitializer that takes care of default positions etc).
		/// </summary>
		public void PublishTools()
		{
			foreach (var item in _mTodoTools)
			{
				Tools.Add(item);

                // Publish the fact that we have registered a new tool window instance
                RegisterToolWindowEvent?.Invoke(this, new RegisterToolWindowEventArgs(item));
            }

            _mTodoTools.Clear();
		}

		/// <summary>
		/// Register a new tool window definition for usage in this program.
		/// </summary>
		/// <param name="newTool"></param>
		public void RegisterTool(ToolViewModel newTool)
		{
			try
			{
                Messaging.Output.Append(string.Format("{0} Registering tool window: {1} ...",
                    DateTime.Now.ToLongTimeString(), newTool.Name));

                _mTodoTools.Add(newTool);
            }
            catch (Exception exp)
			{
                Messaging.Output.AppendLine(exp.Message);
                Messaging.Output.AppendLine(exp.StackTrace);
                throw new Exception("Tool window registration failed in ToolWindowRegistry.", exp);
			}
            finally
            {
                Messaging.Output.AppendLine("Done.");
            }
        }
		#endregion methods
	}
}
