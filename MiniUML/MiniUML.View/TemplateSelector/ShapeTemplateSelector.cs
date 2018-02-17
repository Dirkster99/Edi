namespace MiniUML.View.TemplateSelector
{
  using System.Windows;
  using System.Windows.Controls;
  using Model;
  using Model.ViewModels.Shapes;

  /// <summary>
  /// Select a view for a given viewmodel by loading the DataTemplate as Resource.
  /// </summary>
  public class ShapeTemplateSelector : DataTemplateSelector
  {
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {

            if (item is ShapeViewModelBase)
            {
                ShapeViewModelBase el = item as ShapeViewModelBase;

                if (PluginManager.PluginResources[el.TypeKey] is DataTemplate)
                {
                    DataTemplate template = PluginManager.PluginResources[el.TypeKey] as DataTemplate;

                    return template;
                }
            }

            return PluginManager.PluginResources["MiniUML.UnknownShape"] as DataTemplate;
    }
  }
}
