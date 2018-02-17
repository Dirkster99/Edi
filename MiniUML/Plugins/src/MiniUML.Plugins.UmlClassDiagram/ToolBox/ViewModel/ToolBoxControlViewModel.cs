namespace MiniUML.Plugins.UmlClassDiagram.ToolBox.ViewModel
{
    using System.Collections.Generic;
    using Framework.helpers;
    using Controls.ViewModel.UmlElements;

  /// <summary>
  /// A viewmodel class to manage the viewmodel for a toolbox of a canvas.
  /// 
  /// Source: http://www.codeproject.com/Articles/484616/MVVM-Diagram-Designer?msg=4413242#Drag-And-Drop-To-The-Design-Surface
  /// </summary>
  public class ToolBoxControlViewModel
  {
    private List<ToolBoxData> toolBoxItems = new List<ToolBoxData>();

      /// <summary>
      /// Class constructor to create all shapes that belong to one
      /// UML type of diagram plus common diagram items.
      /// </summary>
      /// <param name="pluginViewModel"></param>
      /// <param name="umlDiagram"></param>
      public ToolBoxControlViewModel(PluginViewModel pluginViewModel, UmlDiagrams umlDiagram)
    {
      var shapeCommandModels = UmlElementsManager.Instance.GetUmlDiagramElements(pluginViewModel, umlDiagram);

      foreach (var item in shapeCommandModels)
        toolBoxItems.Add(new ToolBoxData(item.ToolBoxImageUrl, item));

      if (umlDiagram != UmlDiagrams.Connector)
      {
        shapeCommandModels = UmlElementsManager.Instance.GetUmlDiagramElements(pluginViewModel, UmlDiagrams.Common);

        foreach (var item in shapeCommandModels)
          toolBoxItems.Add(new ToolBoxData(item.ToolBoxImageUrl, item));
      }
    }

    /// <summary>
    /// Get tool box items managed in this viewmodel
    /// </summary>
    public List<ToolBoxData> ToolBoxItems => toolBoxItems;
  }
}
