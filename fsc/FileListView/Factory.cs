namespace FileListView
{
    using FileListView.Interfaces;
    using FileListView.ViewModels;
    using FileSystemModels.Interfaces;
    using FileSystemModels.Models.FSItems.Base;

    /// <summary>
    /// Implements factory methods that creates library objects that are accessible
    /// through interfaces but are otherwise invisible for the outside world.
    /// </summary>
    public sealed class Factory
    {
        private Factory(){ }

        public static IFileListViewModel CreateFileListViewModel(IBrowseNavigation browseNavigation)
        {
            return new FileListViewModel(browseNavigation);
        }

        public static ILVItemViewModel CreateItem(
              string path
            , FSItemType type
            , string displayName)
        {
            return new LVItemViewModel(path, type, displayName);
        }
    }
}
