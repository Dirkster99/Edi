namespace MiniUML.Plugins.UmlClassDiagram
{
    using System.Windows;
    using MiniUML.Model.Model;
    using MiniUML.Model.ViewModels.Document;
    using MiniUML.Plugins.UmlClassDiagram.Converter;

    /// <summary>
    /// Manage a model for this plug-in.
    /// </summary>
    public class PluginModel : MiniUML.Model.PluginModelBase
    {
        #region field
        /// <summary>
        /// Unique plugin name to identify 1 MiniUml plugin among many others.
        /// Changing this requires changing strings in all applications that load this plug-in.
        /// So, never change this string (ever).
        /// </summary>
        public const string ModelName = "UMLClassDiagram";

        private FrameworkElement _PluginView;

        private UmlTypeToStringConverter _ShapeConverter = null;
        #endregion field

        #region constructor
        /// <summary>
        /// Class constructor from document viewmodel
        /// </summary>
        /// <param name="windowViewModel"></param>
        public PluginModel(IMiniUMLDocument windowViewModel)
        {
            _ShapeConverter = new UmlTypeToStringConverter();
            _PluginView = new PluginView();
            _PluginView.DataContext = new PluginViewModel(windowViewModel);
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Get a name for the plug-in model. Each plug-in must have a unique name.
        /// </summary>
        public override string Name
        {
            get
            {
                return PluginModel.ModelName;
            }
        }

        /// <summary>
        /// Get a view for this plug-in.
        /// </summary>
        public override FrameworkElement View
        {
            get
            {
                return _PluginView;
            }
        }

        /// <summary>
        /// Get shape converter from unique string to shape viewmodel instance.
        /// </summary>
        public override UmlTypeToStringConverterBase ShapeConverter
        {
            get
            {
                return _ShapeConverter;
            }
        }
        #endregion properties

        #region methods
        #endregion methods
    }
}
