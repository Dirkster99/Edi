namespace Edi.Core.ViewModels
{
    using System.Windows;
    using Edi.Core.Resources;
    using Edi.Core.View.Pane;

    /// <summary>
    /// This class exports properties that are relevant to viewing, styling and templating
    /// of document and tool winodw items managed by AvalonDock.
    /// </summary>
    public class AvalonDockViewProperties
	{
		#region fields

		#endregion fields

		#region constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		public AvalonDockViewProperties()
		{
			DocumentHeaderTemplate = null;
			LayoutInitializer = new LayoutInitializer();
			SelectPanesStyle = new PanesStyleSelector();
			SelectPanesTemplate = new PanesTemplateSelector();
		}
		#endregion constructors

		#region properties
		/// <summary>
		/// Gets the AvalonDock DocumentHeaderTemplate from resource definitions.
		/// </summary>
		public DataTemplate DocumentHeaderTemplate { get; private set; }

		public LayoutInitializer LayoutInitializer { get; }

		public PanesStyleSelector SelectPanesStyle { get; }

		public PanesTemplateSelector SelectPanesTemplate { get; }

		#endregion properties

		#region methods
		/// <summary>
		/// Initialize and return a new class of this type.
		/// </summary>
		/// <returns></returns>
		public AvalonDockViewProperties InitialzeInstance()
		{
			DocumentHeaderTemplate = LoadDocumentHeaderTemplate();
			LoadPanesStyleSelector(SelectPanesStyle);

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
					"Edi.Apps",
					"Resources/DocumentHeaderDataTemplate.xaml",
					"AvalonDock_DocumentHeader");
		}

		/// <summary>
		/// Load an PanestayleSelector with initial styles from resources.
		/// </summary>
		/// <returns></returns>
		private void LoadPanesStyleSelector(PanesStyleSelector panesStyleSelector)
		{
			var newStyle = ResourceLocator.GetResource<Style>(
				"Edi.Apps",
				"Resources/Styles/AvalonDockStyles.xaml",
				"FileStyle");

			panesStyleSelector.RegisterStyle(typeof(FileBaseViewModel), newStyle);

			newStyle = ResourceLocator.GetResource<Style>(
				"Edi.Apps",
				"Resources/Styles/AvalonDockStyles.xaml",
				"ToolStyle");

			panesStyleSelector.RegisterStyle(typeof(ToolViewModel), newStyle);
		}
		#endregion methods
	}
}