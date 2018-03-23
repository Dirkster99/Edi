using System.Windows;
using Edi.Core.Resources;
using Edi.Core.View.Pane;

namespace Edi.Core.ViewModels
{
	/// <summary>
    /// This class exports properties that are relevant to viewing, styling and templating
    /// of document and tool winodw items managed by AvalonDock.
    /// </summary>
    public class AvalonDockViewProperties
	{
		#region fields
		private DataTemplate _mDocumentHeaderTemplate;
		private readonly LayoutInitializer _mLayoutInitializer;
		private readonly PanesStyleSelector _mSelectPanesStyle;
		private readonly PanesTemplateSelector _mSelectPanesTemplate;
		#endregion fields

		#region constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		public AvalonDockViewProperties()
		{
			_mDocumentHeaderTemplate = null;
			_mLayoutInitializer = new LayoutInitializer();
			_mSelectPanesStyle = new PanesStyleSelector();
			_mSelectPanesTemplate = new PanesTemplateSelector();
		}
		#endregion constructors

		#region properties
		/// <summary>
		/// Gets the AvalonDock DocumentHeaderTemplate from resource definitions.
		/// </summary>
		public DataTemplate DocumentHeaderTemplate
		{
			get => _mDocumentHeaderTemplate;

			private set => _mDocumentHeaderTemplate = value;
		}

		public LayoutInitializer LayoutInitializer => _mLayoutInitializer;

		public PanesStyleSelector SelectPanesStyle => _mSelectPanesStyle;

		public PanesTemplateSelector SelectPanesTemplate => _mSelectPanesTemplate;

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
		private PanesStyleSelector LoadPanesStyleSelector(PanesStyleSelector panesStyleSelector)
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

			return panesStyleSelector;
		}
		#endregion methods
	}
}
