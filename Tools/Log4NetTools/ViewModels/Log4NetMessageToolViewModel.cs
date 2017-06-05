namespace Log4NetTools.ViewModels
{
	using System;
	using Edi.Core.Interfaces;
	using Edi.Core.Interfaces.Enums;
	using Edi.Core.ViewModels;

	/// <summary>
	/// This viewmodel manages the functions of the Log4Net Message Tool Window    
	/// which contains controls that display details on the Message and Throwable log4net fields.
	/// </summary>
	public class Log4NetMessageToolViewModel : ToolViewModel, IRegisterableToolWindow
	{
		#region fields
		public const string ToolContentId = "<Log4NetMessageTool>";
		private Log4NetViewModel mLog4NetVM = null;

		private IDocumentParent mParent = null;
		#endregion fields

		#region constructor
		/// <summary>
		/// Default class constructor
		/// </summary>
		public Log4NetMessageToolViewModel()
			: base("Log4Net Messages")
		{
			// Check if active document is a log4net document to display data for...
			this.OnActiveDocumentChanged(null, null);

			////Workspace.This.ActiveDocumentChanged += new EventHandler(OnActiveDocumentChanged);
			this.ContentId = ToolContentId;
		}
		#endregion constructor

		#region properties
		public override Uri IconSource
		{
			get
			{
				return new Uri("pack://application:,,,/Themes;component/Images/Documents/Log4net.png", UriKind.RelativeOrAbsolute);
			}
		}

		public Log4NetViewModel Log4NetVM
		{
			get
			{
				return this.mLog4NetVM;
			}

			set
			{
				if (this.mLog4NetVM != value)
				{
					this.mLog4NetVM = value;
					this.RaisePropertyChanged(() => this.Log4NetVM);
					this.RaisePropertyChanged(() => this.IsOnline);
				}
			}
		}

		public bool IsOnline
		{
			get
			{
				return (this.Log4NetVM != null);
			}
		}

		public override PaneLocation PreferredLocation
		{
			get { return PaneLocation.Bottom; }
		}
		#endregion properties

		#region methods
		/// <summary>
		/// Set the document parent handling object to deactivation and activation
		/// of documents with content relevant to this tool window viewmodel.
		/// </summary>
		/// <param name="parent"></param>
		public void SetDocumentParent(IDocumentParent parent)
		{
			if (parent != null)
				parent.ActiveDocumentChanged -= this.OnActiveDocumentChanged;

			this.mParent = parent;

			// Check if active document is a log4net document to display data for...
			if (this.mParent != null)
				parent.ActiveDocumentChanged += new DocumentChangedEventHandler(this.OnActiveDocumentChanged);
			else
				this.OnActiveDocumentChanged(null, null);
		}

		/// <summary>
		/// Set the document parent handling object and visibility
		/// to enable tool window to react on deactivation and activation
		/// of documents with content relevant to this tool window viewmodel.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="isVisible"></param>
		public void SetToolWindowVisibility(IDocumentParent parent,
																				bool isVisible = true)
		{
			if (IsVisible == true)
				this.SetDocumentParent(parent);
			else
				this.SetDocumentParent(null);

			base.SetToolWindowVisibility(isVisible);
		}

		/// <summary>
		/// Executes event based when the active (AvalonDock) document changes.
		/// Determine whether tool window can show corresponding state or not
		/// and update viewmodel reference accordingly.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnActiveDocumentChanged(object sender, DocumentChangedEventArgs e)
		{
			if (e != null)
			{
				if (e.ActiveDocument != null)
				{
					Log4NetViewModel log4NetVM = e.ActiveDocument as Log4NetViewModel;

					if (log4NetVM != null)
						this.Log4NetVM = log4NetVM;  // There is an active Log4Net document -> display corresponding content
					else
						this.Log4NetVM = null;
				}
			}
			else // There is no active document hence we do not have corresponding content to display
			{
				this.Log4NetVM = null;
			}
		}
		#endregion methods
	}
}
