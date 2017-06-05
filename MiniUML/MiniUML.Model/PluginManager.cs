namespace MiniUML.Model
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using System.Windows;

  /// <summary>
  /// Manage the root of each plug-in in a static plug-in manager class.
  /// </summary>
  public static class PluginManager
  {
    #region fields
    private static Dictionary<string, PluginModelBase> mPluginModelColl = new Dictionary<string, PluginModelBase>();

    private static ResourceDictionary mPluginResources = new ResourceDictionary();
    #endregion fields

    #region properties
    /// <summary>
    /// Get readonly collection of plugin-models. Use the 'AddPluginModel' method
    /// if you want to add a plug-in model into the collection of models.
    /// </summary>
    public static ReadOnlyCollection<PluginModelBase> PluginModels
    {
      get
      {
        return new ReadOnlyCollection<PluginModelBase>(PluginManager.mPluginModelColl.Values.ToArray());
      }
    }

    /// <summary>
    /// Get a resource dictionary that keeps track of resources that belong to this plug-in.
    /// </summary>
    public static ResourceDictionary PluginResources
    {
      get
      {
        return PluginManager.mPluginResources;
      }
    }
    #endregion properties

    #region methods
    public static PluginModelBase GetPluginModel(string modelName)
    {
      PluginModelBase o;

      PluginManager.mPluginModelColl.TryGetValue(modelName, out o);

      return o;
    }

    /// <summary>
    /// Adds a plugin into the managed collection of plugin models.
    /// </summary>
    /// <param name="pluginModel"></param>
    /// <returns></returns>
    public static bool AddPluginModel(PluginModelBase pluginModel)
    {
      try
      {
        if (pluginModel != null)
        {
          PluginManager.mPluginModelColl.Add(pluginModel.Name, pluginModel);
          return true;
        }

        return false;
      }
      catch (System.Exception)
      {
        throw;
      }
    }
    #endregion methods
  }
}
