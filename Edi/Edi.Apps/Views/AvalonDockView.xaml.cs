namespace Edi.Apps.Views
{
	using System;
	using System.IO;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Input;
	using System.Windows.Threading;
	using Edi.Apps.Events;
	using Xceed.Wpf.AvalonDock;
	using Xceed.Wpf.AvalonDock.Layout;
	using Xceed.Wpf.AvalonDock.Layout.Serialization;

	/// <summary>
	/// Interaction logic for AvalonDockView.xaml
	/// </summary>
	[TemplatePartAttribute(Name = "PART_DockView", Type = typeof(DockingManager))]
	public partial class AvalonDockView : UserControl
	{
		#region fields
		protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private DockingManager _mDockManager = null;

		private DataTemplateSelector _mLayoutItemTemplateSelector = null;
		private DataTemplate _mDocumentHeaderTemplate = null;
		private StyleSelector _mLayoutItemContainerStyleSelector = null;
		private ILayoutUpdateStrategy _mLayoutUpdateStrategy = null;

		private string _mOnLoadXmlLayout = null;
		#endregion fields

		#region constructor
		/// <summary>
		/// Static constructor
		/// </summary>
		static AvalonDockView()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(AvalonDockView),
								new FrameworkPropertyMetadata(typeof(AvalonDockView)));
		}

		/// <summary>
		/// Class Constructor
		/// </summary>
		public AvalonDockView()
		{
			//// this.InitializeComponent();
			this.LayoutId = Guid.NewGuid();
		}
		#endregion constructor

		#region properties
		/// <summary>
		/// Gets/Sets the LayoutId of the AvalonDocking Manager layout used to manage
		/// the positions and layout of documents and tool windows within the AvalonDock
		/// view.
		/// </summary>
		public Guid LayoutId
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the current AvalonDockManager Xml layout and returns it as a string.
		/// </summary>
		public string CurrentAdLayout
		{
			get
			{
				if (this._mDockManager == null)
					return String.Empty;

				string xmlLayoutString = string.Empty;
				try
				{
					using (StringWriter fs = new StringWriter())
					{
						XmlLayoutSerializer xmlLayout = new XmlLayoutSerializer(this._mDockManager);

						xmlLayout.Serialize(fs);

						xmlLayoutString = fs.ToString();
					}
				}
				catch
				{
				}

				return xmlLayoutString;
			}
		}
		#endregion properties

		#region methods
		/// <summary>
		/// Standard method is executed when control template is applied to lookless control.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this._mDockManager = this.Template.FindName("PART_DockView", this) as DockingManager;

			this.SetCustomLayoutItems();
			////this.LoadXmlLayout(this.mOnLoadXmlLayout);
		}


		/// <summary>
		/// Class Constructor
		/// </summary>
		/// <param name="paneSel"></param>
		/// <param name="documentHeaderTemplate"></param>
		/// <param name="panesStyleSelector"></param>
		/// <param name="layoutInitializer"></param>
		/// <param name="layoutId"></param>
		public void SetTemplates(DataTemplateSelector paneSel,
								 DataTemplate documentHeaderTemplate,
								 StyleSelector panesStyleSelector,
								 ILayoutUpdateStrategy layoutInitializer,
								 Guid layoutId
								)
		{
			this._mLayoutItemTemplateSelector = paneSel;
			this._mDocumentHeaderTemplate = documentHeaderTemplate;
			this._mLayoutItemContainerStyleSelector = panesStyleSelector;
			this._mLayoutUpdateStrategy = layoutInitializer;
			this.LayoutId = layoutId;

			if (this._mDockManager == null)
				return;

			this.SetCustomLayoutItems();
		}

		#region Workspace Layout Management
		/// <summary>
		/// Is executed when PRISM sends an Xml layout string notification
		/// via a sender which could be a viewmodel that wants to receive
		/// the load <seealso cref="LoadLayoutEvent"/>.
		/// 
		/// Save layout is triggered by the containing window onClosed event.
		/// </summary>
		/// <param name="args"></param>
		public void OnLoadLayout(LoadLayoutEventArgs args)
		{
			if (args == null)
				return;

			if (string.IsNullOrEmpty(args.XmlLayout) == true)
				return;

			this._mOnLoadXmlLayout = args.XmlLayout;

			if (this._mDockManager == null)
				return;

			this.LoadXmlLayout(this._mOnLoadXmlLayout);
		}

		private void LoadXmlLayout(string xmlLayout)
		{
			try
			{
				StringReader sr = new StringReader(xmlLayout);

				XmlLayoutSerializer layoutSerializer = null;

				Application.Current.Dispatcher.Invoke(new Action(() =>
				{
					try
					{
						layoutSerializer = new XmlLayoutSerializer(this._mDockManager);
						layoutSerializer.LayoutSerializationCallback += this.UpdateLayout;
						layoutSerializer.Deserialize(sr);
					}
					catch (Exception exp)
					{
						Logger.ErrorFormat("Error Loading Layout: {0}\n\n{1}", exp.Message, xmlLayout);
					}

				}), DispatcherPriority.Background);
			}
			catch (Exception exp)
			{
				Logger.ErrorFormat("Error Loading Layout: {0}\n\n{1}", exp.Message, xmlLayout);
			}
		}

		/// <summary>
		/// Convert a Avalondock ContentId into a viewmodel instance
		/// that represents a document or tool window. The re-load of
		/// this component is cancelled if the Id cannot be resolved.
		/// 
		/// The result is (viewmodel Id or Cancel) is returned in <paramref name="args"/>.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void UpdateLayout(object sender, LayoutSerializationCallbackEventArgs args)
		{
			try
			{
				Edi.Core.Interfaces.IViewModelResolver resolver = null;

				resolver = this.DataContext as Edi.Core.Interfaces.IViewModelResolver;

				if (resolver == null)
					return;

				// Get a matching viewmodel for a view through DataContext of this view
				var contentViewModel = resolver.ContentViewModelFromID(args.Model.ContentId);

				if (contentViewModel == null)
					args.Cancel = true;

				// found a match - return it
				args.Content = contentViewModel;
			}
			catch (Exception exp)
			{
				Console.WriteLine(exp.Message);
			}
		}
		#endregion Workspace Layout Management

		/// <summary>
		/// Assigns the currently assigned custom layout controls to the AvalonDock DockingManager.
		/// </summary>
		private void SetCustomLayoutItems()
		{
			if (this._mDockManager == null)
				return;

			if (this._mLayoutItemTemplateSelector != null)
				this._mDockManager.LayoutItemTemplateSelector = this._mLayoutItemTemplateSelector;

			if (this._mDocumentHeaderTemplate != null)
				this._mDockManager.DocumentHeaderTemplate = this._mDocumentHeaderTemplate;

			if (this._mLayoutItemContainerStyleSelector != null)
				this._mDockManager.LayoutItemContainerStyleSelector = this._mLayoutItemContainerStyleSelector;

			if (this._mLayoutUpdateStrategy != null)
				this._mDockManager.LayoutUpdateStrategy = this._mLayoutUpdateStrategy;
		}
		#endregion methods

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
		}
	}
}
