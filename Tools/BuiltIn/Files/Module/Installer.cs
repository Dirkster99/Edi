namespace Files.Module
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
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
    /// This class gets picked up by from Castle.Windsor because
    /// it implements the <see cref="IWindsorInstaller"/> interface.
    /// 
    /// The <see cref="IWindsorInstaller"/> interface is used by the
    /// container to resolve installers when calling
    /// <see cref="IWindsorContainer"/>.Install(FromAssembly.This()); 
    /// </summary>
    public class Installers : IWindsorInstaller
    {
        #region fields
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion fields

        /// <summary>
        /// Performs the installation in the Castle.Windsor.IWindsorContainer.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="store"></param>
        void IWindsorInstaller.Install(IWindsorContainer container,
                                       IConfigurationStore store)
        {
            Logger.InfoFormat("Registering Files.Module");

            try
            {
                var avLayout = container.Resolve<IAvalonDockLayoutViewModel>();
                var programSettings = container.Resolve<ISettingsManager>();
                var toolRegistry = container.Resolve<IToolWindowRegistry>();
                var fileOpenService = container.Resolve<IFileOpenService>();
                var mruListViewModel = container.Resolve<IMRUListViewModel>();

                Initialize(avLayout, programSettings, toolRegistry,
                           fileOpenService, mruListViewModel);
            }
            catch (System.Exception exp)
            {
                Logger.Error(exp);
            }
        }

        #region registering methods
        /// <summary>
        /// Initialize this module via standard PRISM MEF procedure
        /// </summary>
        private void Initialize(IAvalonDockLayoutViewModel avLayout,
                                ISettingsManager programSettings,
                                IToolWindowRegistry toolRegistry,
                                IFileOpenService fileOpenService,
                                IMRUListViewModel mruListViewModel)
        {
            RegisterDataTemplates(avLayout.ViewProperties.SelectPanesTemplate);

            toolRegistry.RegisterTool(new RecentFilesTWViewModel(mruListViewModel));

            toolRegistry.RegisterTool(new FileStatsViewModel());
            RegisterFileExplorerViewModel(programSettings,
                                          toolRegistry,
                                          fileOpenService);
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
        #endregion registering methods
    }
}