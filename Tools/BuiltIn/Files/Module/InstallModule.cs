namespace Files.Module
{
    using Edi.Core.Interfaces;
    using Edi.Core.Resources;
    using Edi.Core.View.Pane;
    using Edi.Settings.Interfaces;
    using Files.ViewModels.FileExplorer;
    using Files.ViewModels.FileStats;
    using Files.ViewModels.RecentFiles;
    using FileSystemModels.Models;
    using MRULib.MRU.Interfaces;
    using System.Reflection;
    using System.Windows;

    /// <summary>
    /// This class installs the moduls contained in this library
    /// using the constructor parameters supplied by the container.
    /// </summary>
    internal class InstallModule
    {
        #region fields
        private readonly IAvalonDockLayoutViewModel _avLayout;
        private readonly ISettingsManager _programSettings;
        private readonly IToolWindowRegistry _toolRegistry;
        private readonly IFileOpenService _fileOpenService;
        private readonly IMRUListViewModel _mruListViewModel;
        #endregion fields

        #region ctors
        /// <summary>
        /// Parameterized PUBLIC class constructor
        /// </summary>
        /// <param name="avLayout"></param>
        /// <param name="programSettings"></param>
        /// <param name="toolRegistry"></param>
        /// <param name="fileOpenService"></param>
        /// <param name="mruListViewModel"></param>
        public InstallModule(IAvalonDockLayoutViewModel avLayout,
                             ISettingsManager programSettings,
                             IToolWindowRegistry toolRegistry,
                             IFileOpenService fileOpenService,
                             IMRUListViewModel mruListViewModel
                            ) : this()
        {
            _avLayout = avLayout;
            _programSettings = programSettings;
            _toolRegistry = toolRegistry;
            _fileOpenService = fileOpenService;
            _mruListViewModel = mruListViewModel;
        }

        /// <summary>
        /// HIDDEN standard class constructor
        /// </summary>
        protected InstallModule()
        {
        }
        #endregion ctors

        #region methods
        /// <summary>
        /// Initialize this module via standard init procedure
        /// </summary>
        internal void Initialize()
        {
            RegisterDataTemplates(_avLayout.ViewProperties.SelectPanesTemplate);

            _toolRegistry.RegisterTool(new RecentFilesTWViewModel(_mruListViewModel));

            _toolRegistry.RegisterTool(new FileStatsViewModel());
            RegisterFileExplorerViewModel(_programSettings,
                                          _toolRegistry,
                                          _fileOpenService);
        }

        private void RegisterFileExplorerViewModel(ISettingsManager programSettings,
                                                   IToolWindowRegistry toolRegistry
                                                 , IFileOpenService fileOpenService)
        {
            var FileExplorer = new FileExplorerViewModel(programSettings, fileOpenService);

            ExplorerSettingsModel settings = null;

            settings = programSettings.SettingData.ExplorerSettings;

            if (settings == null)
                settings = new ExplorerSettingsModel();

            settings.SetUserProfile(programSettings.SessionData.LastActiveExplorer);

            // (re-)configure previous explorer settings and (re-)activate current location
            FileExplorer.ConfigureExplorerSettings(settings);

            toolRegistry.RegisterTool(FileExplorer);
        }

        /// <summary>
        /// Register viewmodel types with <seealso cref="DataTemplate"/> for a view
        /// and return all definitions with a <seealso cref="PanesTemplateSelector"/> instance.
        /// </summary>
        /// <param name="paneSel"></param>
        /// <returns></returns>
        private PanesTemplateSelector RegisterDataTemplates(PanesTemplateSelector paneSel)
        {
            // FileStatsView
            var template = ResourceLocator.GetResource<DataTemplate>(
                                            Assembly.GetAssembly(typeof(FileStatsViewModel)).GetName().Name,
                                            "DataTemplates/FileStatsViewDataTemplate.xaml",
                                            "FileStatsViewTemplate") as DataTemplate;

            paneSel.RegisterDataTemplate(typeof(FileStatsViewModel), template);

            // RecentFiles
            template = ResourceLocator.GetResource<DataTemplate>(
                                    Assembly.GetAssembly(typeof(RecentFilesTWViewModel)).GetName().Name,
                                    "DataTemplates/RecentFilesViewDataTemplate.xaml",
                                    "RecentFilesViewDataTemplate") as DataTemplate;

            paneSel.RegisterDataTemplate(typeof(RecentFilesTWViewModel), template);

            // FileExplorer
            template = ResourceLocator.GetResource<DataTemplate>(
                                    Assembly.GetAssembly(typeof(FileExplorerViewModel)).GetName().Name,
                                    "DataTemplates/FileExplorerViewDataTemplate.xaml",
                                    "FileExplorerViewDataTemplate") as DataTemplate;

            paneSel.RegisterDataTemplate(typeof(FileExplorerViewModel), template);

            return paneSel;
        }
        #endregion methods
    }
}
