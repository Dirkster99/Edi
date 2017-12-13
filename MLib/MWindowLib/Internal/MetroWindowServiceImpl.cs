namespace MWindowLib.Internal
{
    using System.Windows;

    /// <summary>
    /// Implements the service component according to the interface
    /// defintion in <see cref="Interfaces.IMetroWindowService"/>.
    /// 
    /// This service component creates instances of Metro Windows
    /// and supports utillity functions ...
    /// </summary>
    internal class MetroWindowServiceImpl : MWindowInterfacesLib.Interfaces.IMetroWindowService
    {
        /// <summary>
        /// Creates another metro window instance with the given (default) parameters.
        /// </summary>
        /// <returns></returns>
        public Window CreateExternalWindow(
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
        )
        {
            return new MetroWindow
            {
                ShowInTaskbar = showInTaskbar,
                ShowActivated = showActivated,
                Topmost = topmost,
                ResizeMode = resizeMode,
                WindowStyle = windowStyle,
                WindowStartupLocation = windowStartupLocation,
                ShowTitleBar = showTitleBar,

                ShowTitle = showTitle,
                ShowMinButton = showMinButton,
                ShowMaxButton = showMaxButton,
                ShowCloseButton = showCloseButton,
                ////                WindowTransitionsEnabled = windowTransitionsEnabled
            };
        }
    }
}
