namespace Edi.Apps
{
    using Edi.Apps.Interfaces;
    using Edi.Apps.Views.Shell;
    using Edi.Core.Interfaces;
    using Edi.Settings.Interfaces;
    using System.Windows;

    public class Shell : IShell<MainWindow>
    {
        public Shell(IAvalonDockLayoutViewModel av,
                     IApplicationViewModel appVm)
        {
            window = new MainWindow(av, appVm);
        }

        /// <summary>
        /// Gets or sets the window.
        /// </summary>
        /// <value>
        /// The window.
        /// </value>
        public MainWindow window { get; }

        /// <summary>
        /// Configure MainWindow and attach datacontext to it.
        /// </summary>
        /// <param name="workSpace"></param>
        /// <param name="win"></param>
        public void ConfigureSession(IApplicationViewModel workSpace,
                                     ISettingsManager settings)
        {
            window.DataContext = workSpace;

            // Establish command binding to accept user input via commanding framework
            workSpace.InitCommandBinding(window);

            window.Left = settings.SessionData.MainWindowPosSz.X;
            window.Top = settings.SessionData.MainWindowPosSz.Y;
            window.Width = settings.SessionData.MainWindowPosSz.Width;
            window.Height = settings.SessionData.MainWindowPosSz.Height;
            window.WindowState = (settings.SessionData.MainWindowPosSz.IsMaximized == true ? WindowState.Maximized : WindowState.Normal);

            // Initialize Window State in viewmodel to show resize grip when window is not maximized
            if (window.WindowState == WindowState.Maximized)
                workSpace.IsNotMaximized = false;
            else
                workSpace.IsNotMaximized = true;

            workSpace.IsWorkspaceAreaOptimized = settings.SessionData.IsWorkspaceAreaOptimized;
//            string lastActiveFile = settings.SessionData.LastActiveFile;
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        public void Run()
        {
            window.Show();
        }
    }
}
