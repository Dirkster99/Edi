namespace MiniUML.Plugins.UmlClassDiagram.ToolBox.ViewModel
{
  using System.Collections.Generic;
  using MiniUML.Framework.helpers;

  /// <summary>
  /// Interface definition that ensures that toolbox data from viewmodel to view is provided.
  /// </summary>
  public interface IShapBox
  {
    List<ToolBoxData> ToolBoxItems { get; }
  }
}
