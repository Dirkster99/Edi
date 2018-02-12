namespace FileSystemModels.Browse
{
    using FileSystemModels.Interfaces;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines an interface that supports the case where a controller
    /// can request a control or sub-system to browse to a certain location.
    /// </summary>
    public interface INavigateable : ICanNavigate
    {
        /// <summary>
        /// Controller can start browser process if IsBrowsing = false
        /// </summary>
        /// <param name="newPath"></param>
        /// <returns></returns>
        bool NavigateTo(IPathModel newPath);

        /// <summary>
        /// Controller can start browser process if IsBrowsing = false
        /// </summary>
        /// <param name="newPath"></param>
        /// <returns></returns>
        Task<bool> NavigateToAsync(IPathModel newPath);

        /// <summary>
        /// Sets the IsExternalBrowsing state and cleans up any running processings
        /// if any. This method should only be called by an external controll instance.
        /// </summary>
        /// <param name="isBrowsing"></param>
        void SetExternalBrowsingState(bool isBrowsing);
    }
}
