namespace MiniUML.Plugins.UmlClassDiagram
{
  using System.Windows;
  using Model.Model;
  using Model.ViewModels.Document;
  using Converter;

  /// <summary>
  /// Manage a model for this plug-in.
  /// </summary>
  public class PluginModel : Model.PluginModelBase
  {
    #region field
    /// <summary>
    /// Unique plugin name to identify 1 MiniUml plugin among many others.
    /// Changing this requires changing strings in all applications that load this plug-in.
    /// So, never change this string (ever).
    /// </summary>
    public const string ModelName = "UMLClassDiagram";

      private UmlTypeToStringConverter mShapeConverter;
    #endregion field

    #region constructor
    /// <summary>
    /// Class constructor from document viewmodel
    /// </summary>
    /// <param name="windowViewModel"></param>
    public PluginModel(IMiniUMLDocument windowViewModel)
    {
      mShapeConverter = new UmlTypeToStringConverter();
        View = new PluginView {DataContext = new PluginViewModel(windowViewModel)};
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get a name for the plug-in model. Each plug-in must have a unique name.
    /// </summary>
    public override string Name => ModelName;

      /// <summary>
    /// Get a view for this plug-in.
    /// </summary>
    public override FrameworkElement View { get; }

      /// <summary>
    /// Get shape converter from unique string to shape viewmodel instance.
    /// </summary>
    public override UmlTypeToStringConverterBase ShapeConverter => mShapeConverter;

      #endregion properties

    #region methods
    #endregion methods
  }
}
