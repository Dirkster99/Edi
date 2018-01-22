
Change Log in comparison to initial version

- Refactored towards navigation model in Models namespaces
  Added FileListView.Models.Interfaces.IBrowseNavigation

- Extracted separator character and changed it into ';'

- Fixed performance problem with not showing any icons
  (The ACB behaviour was somehow slowing down the view in this case.
   So, I replaced this with standard input bindings)

- Added images for Generic, Metro Dark and Metro Light
  See FileListView/App.xaml for switching these to a particular theme

- Added Refresh Command

- Added File System Commands Context Menu with Open..., Open in Windows, ...

- Added FilterDisplayName into FilterItemViewModel to associate each filter with a name

- Refoactored Main ViewModels Added
  FileListView.ViewModels.Interfaces.IFileListViewModel
  FileListView.ViewModels.Interfaces.IFolderListViewModel

- Localization in FileListView.Local
  FileListView/Local/Strings.rex

- FileSystemModels.Models.ExplorerSettingsModel and
  FileSystemModels.Models.ExplorerUserProfile class and
  FileSystemModels.Interfaces.IConfigExplorerSettings

  to support persistence and reload of user defined settings (see edi.codeplex.com for more details)

- Added Recent Folder Add/Remove commands in context menu

- BugFixes in browse forward and backward history

- Added ToggleIsFilteredCommand and IsFiltered property in FileListView.ViewModels.FileListViewViewModel

- Added LastSelectedRecentFolder into ExplorerSettingsModel class