namespace MiniUML.View.TemplateSelector
{
  using System.Windows;
  using System.Windows.Controls;
  using MiniUML.Model;
  using MiniUML.Model.ViewModels;
  using MiniUML.Model.ViewModels.Shapes;

  /// <summary>
  /// Select a view for a given viewmodel by loading the DataTemplate as Resource.
  /// </summary>
  public class ShapeTemplateSelector : DataTemplateSelector
  {
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      ShapeViewModelBase el = item as ShapeViewModelBase;

      if (el != null)
      {
        DataTemplate template = PluginManager.PluginResources[el.TypeKey] as DataTemplate;

        if (template != null)
          return template;
      }

      return PluginManager.PluginResources["MiniUML.UnknownShape"] as DataTemplate;
    }
  }
}
