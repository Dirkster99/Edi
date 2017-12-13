namespace MWindowInterfacesLib.Interfaces
{
    using System.Windows;

    /// <summary>
    /// Defines an interfaces to a service component.
    /// This service component creates instances of Metro Windows
    /// and supports utillity functions ...
    /// </summary>
    public interface IMetroWindowService
    {
        /// <summary>
        /// Creates another metro window instance with the given (default) parameters.
        /// </summary>
        /// <param name="showInTaskbar"></param>
        /// <param name="showActivated"></param>
        /// <param name="topmost"></param>
        /// <param name="resizeMode"></param>
        /// <param name="windowStyle"></param>
        /// <param name="windowStartupLocation"></param>
        /// <param name="showTitleBar"></param>
        /// <param name="showTitle"></param>
        /// <param name="showMinButton"></param>
        /// <param name="showMaxButton"></param>
        /// <param name="showCloseButton"></param>
        /// <returns></returns>
        Window CreateExternalWindow(
            bool showInTaskbar = false,
            bool showActivated = true,
            bool topmost = true,
            ResizeMode resizeMode = ResizeMode.NoResize,
            WindowStyle windowStyle = WindowStyle.None,
            WindowStartupLocation windowStartupLocation = WindowStartupLocation.CenterScreen,
            bool showTitleBar = false,

            bool showTitle = false,
            bool showMinButton = false,
            bool showMaxButton = false,
            bool showCloseButton = false
////            bool windowTransitionsEnabled = false
        );
    }
}