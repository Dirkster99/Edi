namespace Log4NetTools.ViewModels
{
	using System;
	using Edi.Core.Interfaces;
	using Edi.Core.Interfaces.Enums;
	using Edi.Core.ViewModels;

	/// <summary>
	/// This viewmodel manages the functions of the Log4Net Tool Window which contains
	/// filter function to adjust the display of log4net information.
	/// </summary>
	public class Log4NetToolViewModel : ToolViewModel, IRegisterableToolWindow
	{
		#region fields
		public const string ToolContentId = "<Log4NetTool>";
		private Log4NetViewModel mLog4NetVM = null;
		private IDocumentParent mParent = null;
		#endregion fields

		#region constructor
		/// <summary>
		/// Class constructor
		/// </summary>
		public Log4NetToolViewModel()
			: base("Log4Net")
		{
			// Check if active document is a log4net document to display data for...
			OnActiveDocumentChanged(null, null);

			////Workspace.This.ActiveDocumentChanged += new EventHandler(OnActiveDocumentChanged);
			ContentId = ToolContentId;
		}
		#endregion constructor

		#region properties
		public override Uri IconSource
		{
			get
			{
				return new Uri("pack://application:,,,/Edi.Themes;component/Images/Documents/Log4net.png", UriKind.RelativeOrAbsolute);
			}
		}

		public Log4NetViewModel Log4NetVM
		{
			get
			{
				return mLog4NetVM;
			}

			protected set
			{
				if (mLog4NetVM != value)
				{
					mLog4NetVM = value;
					RaisePropertyChanged(() => Log4NetVM);
					RaisePropertyChanged(() => IsOnline);
				}
			}
		}

		public bool IsOnline
		{
			get
			{
				return (Log4NetVM != null);
			}
		}

		public override PaneLocation PreferredLocation
		{
			get { return PaneLocation.Right; }
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
				parent.ActiveDocumentChanged -= OnActiveDocumentChanged;

			mParent = parent;

			// Check if active document is a log4net document to display data for...
			if (mParent != null)
				parent.ActiveDocumentChanged += new DocumentChangedEventHandler(OnActiveDocumentChanged);
			else
				OnActiveDocumentChanged(null, null);
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
			if (IsVisible)
				SetDocumentParent(parent);
			else
				SetDocumentParent(null);

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

                    if (e.ActiveDocument is Log4NetViewModel)
                    {
                       Log4NetViewModel log4NetVM = e.ActiveDocument as Log4NetViewModel;
                        Log4NetVM = log4NetVM;  // There is an active Log4Net document -> display corresponding content
                    }
                    else
                        Log4NetVM = null;
                }
			}
			else // There is no active document hence we do not have corresponding content to display
			{
				Log4NetVM = null;
			}
		}
		#endregion methods
	}
}
