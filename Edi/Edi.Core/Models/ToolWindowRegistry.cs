namespace Edi.Core.Models
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.ComponentModel.Composition;
	using Interfaces;
	using ViewModels;
	using EdiApp.Events;

	/// <summary>
	/// Class to register and manage all tool windows in one common place.
	/// </summary>
	[Export(typeof(IToolWindowRegistry))]
	public class ToolWindowRegistry : IToolWindowRegistry
	{
		#region fields
		private readonly ObservableCollection<ToolViewModel> mItems = null;

		private readonly List<ToolViewModel> mTodoTools = null;
		#endregion fields

		#region contructors
		public ToolWindowRegistry()
		{
			mItems = new ObservableCollection<ToolViewModel>();
			mTodoTools = new List<ToolViewModel>();
		}
		#endregion contructors

		#region properties
		public ObservableCollection<ToolViewModel> Tools
		{
			get
			{
				return mItems;
			}
		}

		public IOutput Output { get; set; }
		#endregion properties

		#region methods
		/// <summary>
		/// Publishs all registered tool window definitions into an observable collection.
		/// (Which in turn will execute the LayoutInitializer that takes care of default positions etc).
		/// </summary>
		public void PublishTools()
		{
			foreach (var item in mTodoTools)
			{
				mItems.Add(item);
			}

			mTodoTools.Clear();
		}

		/// <summary>
		/// Register a new tool window definition for usage in this program.
		/// </summary>
		/// <param name="newTool"></param>
		public void RegisterTool(ToolViewModel newTool)
		{
			try
			{
				mTodoTools.Add(newTool);

				// Publish the fact that we have registered a new tool window instance
				RegisterToolWindowEvent.Instance.Publish(new RegisterToolWindowEventArgs(newTool));
			}
			catch (Exception exp)
			{
				throw new Exception("Tool window registration failed in ToolWindowRegistry.", exp);
			}
		}
		#endregion methods
	}
}
