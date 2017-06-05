namespace MiniUML.Plugins.UmlClassDiagram.TemplateSelector
{
  using System.Windows;
  using System.Windows.Controls;
  using MiniUML.Model;

  public class ArrowTemplateSelector : DataTemplateSelector
  {
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      if (item == null)
        return null;

      DataTemplate d = null;

      PluginModel m = PluginManager.GetPluginModel(PluginModel.ModelName) as PluginModel;
      
      if (m != null)
        d = m.Resources[item.ToString()] as DataTemplate;

      return d;
      ////return PluginManager.PluginResources[item.ToString()] as DataTemplate;
    }
  }
}
