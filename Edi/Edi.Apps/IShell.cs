namespace Edi.Apps
{
    using Edi.Apps.Interfaces;
    using Edi.Settings.Interfaces;

    /// <summary>
    /// Credit to:
    /// https://www.codeproject.com/articles/683632/a-wpf-template-solution-using-mvvm-and-castle-wind#_articleTop
    /// 
    /// Castle Windsor Tutorial
    /// http://app-code.net/wordpress/?p=676
    /// </summary>
    public interface IShell<T> where T : class
    {
        /// <summary>
        /// Gets or sets the window.
        /// </summary>
        /// <value>
        /// The window.
        /// </value>
        T window { get; }

        /// <summary>
        /// Configure MainWindow and attach datacontext to it.
        /// </summary>
        /// <param name="workSpace"></param>
        /// <param name="win"></param>
        void ConfigureSession(IApplicationViewModel workSpace,
                              ISettingsManager settings);

        /// <summary>
        /// Runs this instance.
        /// </summary>
        void Run();
    }

}
