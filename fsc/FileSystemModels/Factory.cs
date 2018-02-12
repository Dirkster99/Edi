namespace FileSystemModels
{
    using FileSystemModels.Interfaces.Bookmark;
    using FileSystemModels.ViewModels;

    /// <summary>
    /// Implements a factory for core models and viemodels that can be implemented
    /// by clients and controls that are based on this library.
    /// </summary>
    public sealed class Factory
    {
        private Factory(){}

        /// <summary>
        /// Factory pattern that can create objects to manage
        /// recently visited file system folder entries.
        /// </summary>
        /// <returns></returns>
        public static IBookmarksViewModel CreateBookmarksViewModel()
        {
            return new BookmarkesViewModel() as IBookmarksViewModel;
        }
    }
}