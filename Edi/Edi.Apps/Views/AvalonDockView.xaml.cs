namespace Edi.Apps.Views
{
	using System;
	using System.IO;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Input;
	using System.Windows.Threading;
	using Events;
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
		protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private DockingManager mDockManager = null;

		private DataTemplateSelector mLayoutItemTemplateSelector = null;
		private DataTemplate mDocumentHeaderTemplate = null;
		private StyleSelector mLayoutItemContainerStyleSelector = null;
		private ILayoutUpdateStrategy mLayoutUpdateStrategy = null;

		private string mOnLoadXmlLayout = null;
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
			LayoutID = Guid.NewGuid();
		}
		#endregion constructor

		#region properties
		/// <summary>
		/// Gets/Sets the LayoutId of the AvalonDocking Manager layout used to manage
		/// the positions and layout of documents and tool windows within the AvalonDock
		/// view.
		/// </summary>
		public Guid LayoutID
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the current AvalonDockManager Xml layout and returns it as a string.
		/// </summary>
		public string CurrentADLayout
		{
			get
			{
				if (mDockManager == null)
					return String.Empty;

				string xmlLayoutString = string.Empty;
				try
				{
					using (StringWriter fs = new StringWriter())
					{
						XmlLayoutSerializer xmlLayout = new XmlLayoutSerializer(mDockManager);

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

			mDockManager = Template.FindName("PART_DockView", this) as DockingManager;

			SetCustomLayoutItems();
			////this.LoadXmlLayout(this.mOnLoadXmlLayout);
		}


		/// <summary>
		/// Class Constructor
		/// </summary>
		/// <param name="paneSel"></param>
		/// <param name="documentHeaderTemplate"></param>
		/// <param name="panesStyleSelector"></param>
		/// <param name="layoutInitializer"></param>
		/// <param name="layoutID"></param>
		public void SetTemplates(DataTemplateSelector paneSel,
															DataTemplate documentHeaderTemplate,
															StyleSelector panesStyleSelector,
															ILayoutUpdateStrategy layoutInitializer,
															Guid layoutID
															)
		{
			mLayoutItemTemplateSelector = paneSel;
			mDocumentHeaderTemplate = documentHeaderTemplate;
			mLayoutItemContainerStyleSelector = panesStyleSelector;
			mLayoutUpdateStrategy = layoutInitializer;
			LayoutID = layoutID;

			if (mDockManager == null)
				return;

			SetCustomLayoutItems();
		}

		#region Workspace Layout Management
		/// <summary>
		/// Is executed when PRISM sends an Xml layout string notification
		/// via a sender which could be a viewmodel that wants to receive
		/// the load the <seealso cref="LoadLayoutEvent"/>.
		/// 
		/// Save layout is triggered by the containing window onClosed event.
		/// </summary>
		/// <param name="args"></param>
		public void OnLoadLayout(LoadLayoutEventArgs args)
		{
			if (args == null)
				return;

			if (string.IsNullOrEmpty(args.XmlLayout))
				return;

			mOnLoadXmlLayout = args.XmlLayout;

			if (mDockManager == null)
				return;

			LoadXmlLayout(mOnLoadXmlLayout);
		}

		private void LoadXmlLayout(string xmlLayout)
		{
			try
			{
				StringReader sr = new StringReader(xmlLayout);

				XmlLayoutSerializer layoutSerializer = null;

				Application.Current.Dispatcher.BeginInvoke(new Action(() =>
				{
					try
					{
						layoutSerializer = new XmlLayoutSerializer(mDockManager);
						layoutSerializer.LayoutSerializationCallback += UpdateLayout;
						layoutSerializer.Deserialize(sr);
					}
					catch (Exception exp)
					{
						logger.ErrorFormat("Error Loading Layout: {0}\n\n{1}", exp.Message, xmlLayout);
					}

				}), DispatcherPriority.Background);
			}
			catch (Exception exp)
			{
				logger.ErrorFormat("Error Loading Layout: {0}\n\n{1}", exp.Message, xmlLayout);
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
				Core.Interfaces.IViewModelResolver resolver = null;

				resolver = DataContext as Core.Interfaces.IViewModelResolver;

				if (resolver == null)
					return;

				// Get a matching viewmodel for a view through DataContext of this view
				var content_view_model = resolver.ContentViewModelFromID(args.Model.ContentId);

				if (content_view_model == null)
					args.Cancel = true;

				// found a match - return it
				args.Content = content_view_model;
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
			if (mDockManager == null)
				return;

			if (mLayoutItemTemplateSelector != null)
				mDockManager.LayoutItemTemplateSelector = mLayoutItemTemplateSelector;

			if (mDocumentHeaderTemplate != null)
				mDockManager.DocumentHeaderTemplate = mDocumentHeaderTemplate;

			if (mLayoutItemContainerStyleSelector != null)
				mDockManager.LayoutItemContainerStyleSelector = mLayoutItemContainerStyleSelector;

			if (mLayoutUpdateStrategy != null)
				mDockManager.LayoutUpdateStrategy = mLayoutUpdateStrategy;
		}
		#endregion methods

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
		}
	}
}
