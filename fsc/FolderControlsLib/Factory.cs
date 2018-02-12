namespace FolderControlsLib
{
    using FileSystemModels;
    using FileSystemModels.Models.FSItems.Base;
    using FolderControlsLib.Interfaces;
    using FolderControlsLib.ViewModels;

    /// <summary>
    /// Implements factory methods that creates library objects that are accessible
    /// through interfaces but are otherwise invisible for the outside world.
    /// </summary>
    public sealed class Factory
    {
        private Factory(){ }

        /// <summary>
        /// Public construction method to create a <see cref="ILVItemViewModel"/>
        /// object that represents a logical drive (eg 'C:\')
        /// </summary>
        /// <param name="curdir"></param>
        /// <returns></returns>
        public static IFolderItemViewModel CreateLogicalDrive(string curdir)
        {
            var item = new FolderItemViewModel(
                PathFactory.Create(curdir, FSItemType.LogicalDrive),
                string.Empty, true);

            item.SetDisplayName(item.DisplayItemString());

            return item;
        }

        /// <summary>
        /// Returns a new viewmodel that can be used to drive a folder combobox.
        /// </summary>
        /// <returns></returns>
        public static IFolderComboBoxViewModel CreateFolderComboBoxVM()
        {
            return new FolderComboBoxViewModel();
        }
    }
}
