namespace FolderBrowser.ViewModels
{
    using FileSystemModels;
    using FileSystemModels.ViewModels.Base;
    using FolderBrowser.Interfaces;

    /// <summary>
    /// Wrapper class for <seealso cref="System.Environment.SpecialFolder"/> items.
    /// </summary>
    internal class CustomFolderItemViewModel : ViewModelBase, ICustomFolderItemViewModel
    {
        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="specialFolder"></param>
        public CustomFolderItemViewModel(System.Environment.SpecialFolder specialFolder)
        {
            SpecialFolder = specialFolder;

            Path = PathFactory.SpecialFolderHasPath(specialFolder);
        }

        /// <summary>
        /// Protected standard class constructor
        /// </summary>
        protected CustomFolderItemViewModel()
        {
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Gets the file system path of this custom folder item.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Gets the <seealso cref="System.Environment.SpecialFolder"/> enumeration member
        /// associated with this class.
        /// </summary>
        public System.Environment.SpecialFolder SpecialFolder { get; private set; }
        #endregion properties
    }
}
