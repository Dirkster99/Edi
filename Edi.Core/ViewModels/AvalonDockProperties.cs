namespace Edi.Core.ViewModels
{
	using System.Windows;
	using Edi.Core.Resources;
	using Edi.Core.View.Pane;
	using Edi.Core.ViewModels;

	/// <summary>
	/// This class exports properties that are relevant to viewing, styling and templating
	/// of document and tool winodw items managed by AvalonDock.
	/// </summary>
	public class AvalonDockViewProperties
	{
		#region fields
		private DataTemplate mDocumentHeaderTemplate;
		readonly private Edi.Core.View.Pane.LayoutInitializer mLayoutInitializer;
		readonly private PanesStyleSelector mSelectPanesStyle;
		readonly private PanesTemplateSelector mSelectPanesTemplate;
		#endregion fields

		#region constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		public AvalonDockViewProperties()
		{
			this.mDocumentHeaderTemplate = null;
			this.mLayoutInitializer = new LayoutInitializer();
			this.mSelectPanesStyle = new PanesStyleSelector();
			this.mSelectPanesTemplate = new PanesTemplateSelector();
		}
		#endregion constructors

		#region properties
		/// <summary>
		/// Gets the AvalonDock DocumentHeaderTemplate from resource definitions.
		/// </summary>
		public DataTemplate DocumentHeaderTemplate
		{
			get
			{
				return this.mDocumentHeaderTemplate;
			}

			private set
			{
				this.mDocumentHeaderTemplate = value;
			}
		}

		public LayoutInitializer LayoutInitializer
		{
			get
			{
				return this.mLayoutInitializer;
			}
		}

		public PanesStyleSelector SelectPanesStyle
		{
			get { return this.mSelectPanesStyle; }
		}

		public PanesTemplateSelector SelectPanesTemplate
		{
			get { return this.mSelectPanesTemplate; }
		}
		#endregion properties

		#region methods
		/// <summary>
		/// Initialize and return a new class of this type.
		/// </summary>
		/// <returns></returns>
		public AvalonDockViewProperties InitialzeInstance()
		{
			this.DocumentHeaderTemplate = this.LoadDocumentHeaderTemplate();
			this.LoadPanesStyleSelector(this.SelectPanesStyle);

			return this;
		}

		/// <summary>
		/// Load an AvalonDock DocumentHeaderTemplate from resources.
		/// </summary>
		/// <returns></returns>
		private DataTemplate LoadDocumentHeaderTemplate()
		{
			return
				ResourceLocator.GetResource<DataTemplate>(
					"EdiApp",
					"Resources/DocumentHeaderDataTemplate.xaml",
					"AvalonDock_DocumentHeader") as DataTemplate;
		}

		/// <summary>
		/// Load an PanestayleSelector with initial styles from resources.
		/// </summary>
		/// <returns></returns>
		private PanesStyleSelector LoadPanesStyleSelector(PanesStyleSelector panesStyleSelector)
		{
			var newStyle = ResourceLocator.GetResource<Style>(
										 "EdiApp",
										 "Resources/Styles/AvalonDockStyles.xaml",
										 "FileStyle") as Style;

			panesStyleSelector.RegisterStyle(typeof(FileBaseViewModel), newStyle);

			newStyle = ResourceLocator.GetResource<Style>(
									"EdiApp",
									"Resources/Styles/AvalonDockStyles.xaml",
									"ToolStyle") as Style;

			panesStyleSelector.RegisterStyle(typeof(ToolViewModel), newStyle);

			return panesStyleSelector;
		}
		#endregion methods
	}
}
