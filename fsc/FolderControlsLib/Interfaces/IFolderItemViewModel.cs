namespace FolderControlsLib.Interfaces
{
    using FileSystemModels.Interfaces;
    using FileSystemModels.Models.FSItems.Base;
    using System.Windows.Media;

    /// <summary>
    /// Defines properties and methods of an item that is used to display folder
    /// related information in a list like fashion with indentation if possible.
    /// (combobox, etc...).
    /// </summary>
    public interface IFolderItemViewModel : IListItemViewModel
    {
        #region properties
        /// <summary>
        /// Gets an indendation (if any) for this item.
        /// An indendation allows the display of path
        /// items
        ///      in
        ///        stair
        ///             like
        ///                 display
        ///                        fashion.
        /// </summary>
        int Indentation { get; }
        #endregion properties
    }
}