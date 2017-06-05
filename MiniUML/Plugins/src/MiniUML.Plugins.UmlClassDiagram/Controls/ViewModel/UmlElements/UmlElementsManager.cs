namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.UmlElements
{
  using System.Collections.Generic;
  using MiniUML.Model.ViewModels.Command;

  public partial class UmlElementsManager
  {
    #region fields
    private UmlElementDataDef mUmlElementFactory = null;
    private UmlDiagramsDataDef mUmlDiagramFactory = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class Constructor
    /// </summary>
    public UmlElementsManager()
    {
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Gets the default HighlightingManager instance.
    /// The default HighlightingManager comes with built-in highlightings.
    /// </summary>
    public static UmlElementsManager Instance
    {
      get {
        return DefaultUmlElementsManager.Instance;
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Get a create commandmodel instance for the matching <seealso cref="UmlTypes"/> parameter.
    /// </summary>
    /// <param name="viewModel"></param>
    /// <param name="umlType"></param>
    /// <returns></returns>
    public CommandModelBase GetCreateUmlShapeCommandModel(PluginViewModel viewModel, UmlTypes umlType)
    {
      if (this.mUmlElementFactory == null)
        this.mUmlElementFactory = new UmlElementDataDef();

      return this.mUmlElementFactory.GetShapeCreateCommand(viewModel, umlType);
    }

    /// <summary>
    /// Get a collection of <seealso cref="CommandModelBase"/> elements that represents all commands
    /// necessary to create a toolbox entry that represents the elements for a new uml diagram.
    /// </summary>
    /// <param name="viewModel"></param>
    /// <param name="umlDiagram"></param>
    /// <returns></returns>
    public IEnumerable<CommandModelBase> GetUmlDiagramElements(PluginViewModel viewModel, UmlDiagrams umlDiagram)
    {
      if (this.mUmlDiagramFactory == null)
        this.mUmlDiagramFactory = new UmlDiagramsDataDef();

      return this.mUmlDiagramFactory.GetUmlDiagramDataDef(viewModel, umlDiagram);
    }
    #endregion methods

    #region private class
    /// <summary>
    /// Static representation of <seealso cref="UmlElementsManager"/> properties and methods.
    /// </summary>
    internal sealed class DefaultUmlElementsManager : UmlElementsManager
    {
      public static new readonly DefaultUmlElementsManager Instance = new DefaultUmlElementsManager();
    }
    #endregion private class
  }
}
