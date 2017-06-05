namespace MiniUML.Plugins.UmlClassDiagram.Controls.View.Shapes
{
  using System.Windows;
  using System.Windows.Controls;
  using MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel;
  
  /// <summary>
  /// This class selects a canvas/path shape for a enumeration based parameter.
  /// </summary>
  public class UmlCanvasShapeSelector : DataTemplateSelector
  {
    public DataTemplate UmlManPathShape
    {
      get;
      set;
    }

    public DataTemplate ErrorPathShape
    {
      get;
      set;
    }

    public DataTemplate ActivityInitial
    {
      get;
      set;
    }

    public DataTemplate ActivityFinal
    {
      get;
      set;
    }

    public DataTemplate ActivityFlowFinal
    {
      get;
      set;
    }

    public DataTemplate ActivitySync
    {
      get;
      set;
    }

    public DataTemplate Event1
    {
      get;
      set;
    }

    public DataTemplate Event2
    {
      get;
      set;
    }

    /// <summary>
    /// Method is invoked with the Content property bound to a ContentControl
    /// if the <seealso cref="DataTemplateSelector"/> is assigned to the
    /// ContentTemplateSelector property of a ContentControl.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="container"></param>
    /// <returns></returns>
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      if (item is ShapeViewModelSubKeys)
      {
        ShapeViewModelSubKeys selectItemSourceType = (ShapeViewModelSubKeys)item;

        if (selectItemSourceType == ShapeViewModelSubKeys.CanvasUmlMan)
          return this.UmlManPathShape;

        if (selectItemSourceType == ShapeViewModelSubKeys.CanvasActivityInitial)
          return this.ActivityInitial;

        if (selectItemSourceType == ShapeViewModelSubKeys.CanvasActivityFinal)
          return this.ActivityFinal;

        if (selectItemSourceType == ShapeViewModelSubKeys.CanvasActivityFlowFinal)
          return this.ActivityFlowFinal;

        if (selectItemSourceType == ShapeViewModelSubKeys.CanvasActivitySync)
          return this.ActivitySync;

        if (selectItemSourceType == ShapeViewModelSubKeys.CanvasEvent1)
          return this.Event1;

        if (selectItemSourceType == ShapeViewModelSubKeys.CanvasEvent2)
          return this.Event2;
      }

      return this.ErrorPathShape;
    }
  }
}
