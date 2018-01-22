namespace FileSystemModels.Interfaces
{
  using FileSystemModels.Models;

  /// <summary>
  /// Define an interface for configuring and reading setting
  /// and user profile data for the explorer (tool window) viewmodel.
  ///
  /// This interface includes usage of the <see cref="ExplorerSettingsModel"/> class.
  /// </summary>
  public interface IConfigExplorerSettings
  {
    /// <summary>
    /// Configure this viewmodel (+ attached browser viewmodel) with the given settings.
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    bool ConfigureExplorerSettings(ExplorerSettingsModel settings);

    /// <summary>
    /// Compare given <paramref name="input"/> settings with current settings
    /// and return a new settings model if the current settings
    /// changed in comparison to the given <paramref name="input"/> settings.
    /// 
    /// Always return current settings if <paramref name="input"/> settings is null.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    ExplorerSettingsModel GetExplorerSettings(ExplorerSettingsModel input);
  }
}