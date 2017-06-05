/***
See also:
EdiViews\Tools\FileExplorer\FileExplorerView.xaml

          <!-- Format sample: 'All Files (*.*)' -->
          <fview:FilterComboBox.Text>
            <Binding Path="CurrentFilter" UpdateSourceTrigger="PropertyChanged" />
          </fview:FilterComboBox.Text>
    
***/    
public interface IExplorer_Settings
{
  /// <summary>
  /// Gets the currently viewed path.
  /// Use this property to save/re-restore data when the application
  /// starts or shutsdown.
  /// </summary>
  string GetCurrentPath();

  /// <summary>
  /// Sets the currently viewed filter in the filter combobox.
  /// Use this property to save/re-restore data when the application
  /// starts or shutsdown.
  /// </summary>
  string SetCurrentFilter(FilterItemViewModel newFilter);

  IEnumerate<string> GetRecentFolders(FilterItemViewModel newFilter);
  bool SetRecentFolders(IEnumerate<string> recentFolders);

  #region filter settings
  IEnumerate<FilterItemViewModel> GetFilterCollection(FilterItemViewModel newFilter);
  bool SetFilterCollection(List<IEnumerate> recentFolders);
  
  /// <summary>
  /// Sets the currently viewed path.
  /// Use this property to save/re-restore data when the application
  /// starts or shutsdown.
  /// </summary>
  string SetCurrentFilter(string newPath);
  
  /// <summary>
  /// Sets the currently viewed filter in the filter combobox.
  /// Use this property to save/re-restore data when the application
  /// starts or shutsdown.
  /// </summary>
  FilterItemViewModel GetCurrentFilter();
  #endregion filter settings
  
  bool ShowIcons {get; set;}
  bool ShowFolders {get; set;}
  bool ShowHiddenFiles {get; set;}
  
  IEnumerate<CustomFolderItemModel> SpecialFolders {get; set;}
}
