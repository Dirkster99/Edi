using System.Windows;
using System.Windows.Controls;
using MiniUML.Model;

namespace MiniUML.Plugins.UmlClassDiagram.TemplateSelector
{
    public class ArrowTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
                return null;

            DataTemplate d = null;


            if (PluginManager.GetPluginModel(PluginModel.ModelName) is PluginModel)
            {
                var m = PluginManager.GetPluginModel(PluginModel.ModelName) as PluginModel;

                d = m.Resources[item.ToString()] as DataTemplate;
            }

            return d;
            ////return PluginManager.PluginResources[item.ToString()] as DataTemplate;
        }
    }
}
