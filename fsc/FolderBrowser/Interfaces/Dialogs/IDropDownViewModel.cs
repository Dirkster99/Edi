namespace FolderBrowser.Dialogs.Interfaces
{
    using FileSystemModels.Interfaces.Bookmark;
    using System.Windows.Input;

    public delegate string UpdateCurrentPath();
    public delegate IBookmarksViewModel UpdateBookmarks();

    /// <summary>
    /// Specifies the event on which the drop had been closed.
    /// </summary>
    public enum Result
    {
        /// <summary>
        /// Indicates that the drop down was closed without either
        /// clicking cancel or OK button. This is possible if the
        /// user clicks into a non-drop down area.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Drop down was cloased with the OK button.
        /// </summary>
        OK = 1,

        /// <summary>
        /// Drop down was cloased with Cancel button.
        /// </summary>
        Cancel = 2
    }

    /// <summary>
    /// Defines a delegate callback method that is invoked when the drop down
    /// has closed on click OK or Cancel or otherwise (user clicks in non-drop down area)
    /// </summary>
    /// <param name="bookmarks"></param>
    /// <param name="selectedPath"></param>
    /// <param name="result"></param>
    public delegate void DropDownClosedResult(IBookmarksViewModel bookmarks,
                                              string selectedPath,
                                              Result result);

    /// <summary>
    /// Defines an interface to a viewmodel that can be used to manage a
    /// view that is visible via drop down button.
    /// </summary>
    public interface IDropDownViewModel
    {
        /// <summary>
        /// Gets/sets the label of the drop down button.
        /// </summary>
        string ButtonLabel { get; set; }

        /// <summary>
        /// Gets/sets the (initial) height of the drop down button.
        /// </summary>
        double Height { get; set; }

        /// <summary>
        /// Gets/sets the (initial) width of the drop down button.
        /// </summary>
        double Width { get; set; }

        /// <summary>
        /// Gets/sets whether the drop down element is currently open or not.
        /// (relevant for binding to view)
        /// </summary>
        bool IsOpen { get; set; }

        /// <summary>
        /// Gets/sets a command that cancels the display of the drop down element.
        /// (relevant for binding to view)
        /// </summary>
        ICommand CancelCommand { get; }

        /// <summary>
        /// Gets/sets a command that OK'eys the display of the drop down element.
        /// (relevant for binding to view)
        /// </summary>
        ICommand OKCommand { get; }

        DropDownClosedResult ResultCallback { get; }

        UpdateCurrentPath UpdateInitialPath { get; set; }

        UpdateBookmarks UpdateInitialBookmarks { get; set; }
    }
}
