namespace FolderBrowser.Dialogs.Interfaces
{
    using FileSystemModels.Interfaces.Bookmark;
    using FolderBrowser.Interfaces;

    public interface IDialogViewModel
    {
        bool? DialogCloseResult { get; }

        /// <summary>
        /// Gets the viewmodel that drives the folder picker control.
        /// </summary>
        IBrowserViewModel TreeBrowser { get; }

        /// <summary>
        /// Gets the viewmodel that drives the folder bookmark drop down control.
        /// </summary>
        IBookmarksViewModel BookmarkedLocations  { get; }
    }
}
