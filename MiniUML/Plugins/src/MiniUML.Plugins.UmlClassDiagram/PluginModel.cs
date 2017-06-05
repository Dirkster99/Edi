namespace MiniUML.Plugins.UmlClassDiagram
{
  using System.Windows;
  using MiniUML.Model.Model;
  using MiniUML.Model.ViewModels;
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

    private FrameworkElement mPluginView;

    private UmlTypeToStringConverter mShapeConverter = null;
    #endregion field

    #region constructor
    /// <summary>
    /// Class constructor from document viewmodel
    /// </summary>
    /// <param name="windowViewModel"></param>
    public PluginModel(IMiniUMLDocument windowViewModel)
    {
      this.mShapeConverter = new UmlTypeToStringConverter();
      this.mPluginView = new PluginView();
      this.mPluginView.DataContext = new PluginViewModel(windowViewModel);
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
        return this.mPluginView;
      }
    }

    /// <summary>
    /// Get shape converter from unique string to shape viewmodel instance.
    /// </summary>
    public override UmlTypeToStringConverterBase ShapeConverter
    {
      get
      {
        return this.mShapeConverter;
      }
    }
    #endregion properties

    #region methods
    #endregion methods
  }
}
