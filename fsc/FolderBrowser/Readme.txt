
Change Log in comparison to initial version

- Moved all Views into FolderBrowser.Views
- Extracted FolderBrowserView.xaml/cs out of BrowseDirectoriesView.xaml/cs

- Improved Environment.SpecialFolder handling in view binding with SpecialFolderMarkUpExtension

- Added Views.Behaviour.TreeViewSelectionChangedBehavior to get rid of parent reference between
  ViewModels.FolderViewModel and ViewModels.BrowserViewModel

- Breaking Change Renamed
  Renamed Behaviour namespace into Views.Behaviours
  Renamed ViewModel namespace into ViewModels

  ViewModel.BrowserViewModel.PathSelectionEvent     into ViewModels.BrowserViewModel.FinalPathSelectionEvent
  ViewModel.BrowserViewModel.SelectDirectoryCommand into ViewModels.BrowserViewModel.FinalSelectDirectoryCommand

- Added TextChangedCommand to map change of test events into a Command binding

- FolderBrowser integration with FileListViewModel (synchronize current folder via events)
  See FileListViewTest extension with Tab Control demonstrating:
   - FileListView integrated with FolderBrowser, and
   - FileListView by itself

- Localization in FileListView.Local
  FolderBrowser and FileListView project are localizable via:
  FolderBrowser/Local/Strings.rex

  file.

- FileSystemModels.Models.ExplorerSettingsModel and
  FileSystemModels.Models.ExplorerUserProfile class and
  FileSystemModels.Interfaces.IExplorerSettings

  to support persistence and reload of user defined settings (see edi.codeplex.com for more details)

- Added Recent Folder Add/Remove commands in context menu

- Added DisplayItemString in FolderBrowser.ViewModels.FolderViewModel
                             FolderBrowser.ViewModels.Interfaces.IFolderViewModel
                             FolderBrowser.Views.FolderBrowserView.xaml

  To display drives as "C:\ (Windows)" or
                       "D:\ (Device not ready)" -> when CD-Rom 'D:\' drive does not contain a CD

 - Added ToolTip FolderPath for all FolderItems

Todo (in order of priority):

- Filter via CollectionView instead of re-query
- Drag & Drop item in FListView
- Multiselect in FListView
- Async Queries when changing folder