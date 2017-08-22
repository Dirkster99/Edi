namespace Edi
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Windows;
    using Edi.Core;
    using Edi.Core.Interfaces;
    using Edi.Apps.Interfaces.ViewModel;
    using Edi.Apps.ViewModels;
    using Edi.Apps.Views.Shell;
    using Microsoft.Practices.Prism.MefExtensions;
    using Microsoft.Practices.Prism.Modularity;
    using MsgBox;
    using Output.Views;
    using Edi.Settings;
    using Edi.Settings.Interfaces;
    using Edi.Settings.ProgramSettings;
    using SimpleControls.Local;
    using Edi.Themes.Interfaces;
    using MRULib.MRU.Interfaces;

    public class Bootstapper : MefBootstrapper
    {
        #region fields
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private MainWindow mMainWin = null;
        private IApplicationViewModel appVM = null;

        private readonly StartupEventArgs mEventArgs;
        private readonly App mApp = null;

        private readonly MsgBox.IMessageBoxService _MsgBox = null;
        private readonly IMRUListViewModel _MruVM = null;

        private readonly Options mOptions = null;
        private readonly IThemesManager mThemes = null;
        private readonly ISettingsManager mProgramSettingsManager = null;
        #endregion fields

        #region constructors
        public Bootstapper(App app,
                           StartupEventArgs eventArgs,
                           Options programSettings,
                           IThemesManager themesManager)
            : this()
        {
            this.mThemes = themesManager;
            this.mProgramSettingsManager = new SettingsManager(this.mThemes);

            this.mEventArgs = eventArgs;
            this.mApp = app;
            this.mOptions = programSettings;
        }

        protected Bootstapper()
        {
            _MsgBox = new MessageBoxService();

            _MruVM = MRULib.MRU_Service.Create_List();
        }
        #endregion constructors

        #region properties
        private IApplicationViewModel AppViewModel
        {
            get
            {
                return this.appVM;
            }
        }

        private MainWindow MainWindow
        {
            get
            {
                return this.MainWindow;
            }
        }
        #endregion properties

        #region Methods
        /// <summary>
        /// Executes the processing necessary to bootstrap the application
        /// including module detection, registration, and loading.
        /// http://stackoverflow.com/questions/10466304/event-upon-initialization-complete-in-wpf-prism-app
        /// </summary>
        public override void Run(bool runWithDefaultConfiguration)
        {
            base.Run(runWithDefaultConfiguration);

            // Register imported tool window definitions with Avalondock
            var toolWindowRegistry = this.Container.GetExportedValue<IToolWindowRegistry>();
            toolWindowRegistry.PublishTools();

            // Show the startpage if application starts for the very first time
            // (This requires that command binding was succesfully done before this line)
            if (this.appVM.ADLayout.LayoutSoure == Core.Models.Enums.LayoutLoaded.FromDefault)
                AppCommand.ShowStartPage.Execute(null, null);

            if (this.mEventArgs != null)
                ProcessCmdLine(this.mEventArgs.Args, this.appVM);

            // PRISM modules (and everything else) have been initialized if we got here
            var output = this.Container.GetExportedValue<IMessageManager>();
            output.Output.AppendLine("Get involved at: https://github.com/Dirkster99/Edi");

            this.appVM.EnableMainWindowActivated(true);
        }

        protected override void ConfigureAggregateCatalog()
        {
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(IOutputView).Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(IAppCoreModel).Assembly));

            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(AvalonDockLayoutViewModel).Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstapper).Assembly));

            ////Scan directory for content
            ////DirectoryCatalog catalog = new DirectoryCatalog("Plugins");
            ////this.AggregateCatalog.Catalogs.Add(catalog);
        }

        protected override DependencyObject CreateShell()
        {
            try
            {
                var appCore = this.Container.GetExportedValue<IAppCoreModel>();

                // Setup localtion of config files
                this.mProgramSettingsManager.AppDir = appCore.DirAppData;
                this.mProgramSettingsManager.LayoutFileName = appCore.LayoutFileName;

                var avLayout = this.Container.GetExportedValue<IAvalonDockLayoutViewModel>();
                this.appVM = this.Container.GetExportedValue<IApplicationViewModel>();

                appVM.LoadConfigOnAppStartup(this.mOptions, this.mProgramSettingsManager, this.mThemes);

                // Attempt to load a MiniUML plugin via the model class
                MiniUML.Model.MiniUmlPluginLoader.LoadPlugins(appCore.AssemblyEntryLocation + @".\Plugins\UML\", this.AppViewModel);

                this.mMainWin = this.Container.GetExportedValue<MainWindow>();

                appCore.CreateAppDataFolder();

                if (this.mMainWin != null)
                {
                    this.ConstructMainWindowSession(this.appVM, this.mMainWin, this.mProgramSettingsManager);

                    if (this.mApp.AppIsShuttingDown == false)
                        this.mApp.ShutdownMode = System.Windows.ShutdownMode.OnLastWindowClose;
                    ////this.mMainWin.Show();
                }
                else
                    throw new Exception("Main Window construction failed in application boot strapper class.");
            }
            catch (Exception exp)
            {
                logger.Error(exp);

                try
                {
                    // Cannot set shutdown mode when application is already shuttong down
                    if (this.mApp.AppIsShuttingDown == false)
                        this.mApp.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                }
                catch (Exception exp1)
                {
                    logger.Error(exp1);
                }

                try
                {
                    // 1) Application hangs when this is set to null while MainWindow is visible
                    // 2) Application throws exception when this is set as owner of window when it
                    //    was never visible.
                    //
                    if (Application.Current.MainWindow != null)
                    {
                        if (Application.Current.MainWindow.IsVisible == false)
                            Application.Current.MainWindow = null;
                    }
                }
                catch (Exception exp2)
                {
                    logger.Error(exp2);
                }

                _MsgBox.Show(exp, Strings.STR_MSG_ERROR_FINDING_RESOURCE
                           , MsgBoxButtons.OKCopy, MsgBoxImage.Error);

                if (this.mApp.AppIsShuttingDown == false)
                    this.mApp.Shutdown();
            }

            return this.mMainWin;
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            Application.Current.MainWindow = (MainWindow)this.Shell;
            Application.Current.MainWindow.Show();
        }

        protected override Microsoft.Practices.Prism.Regions.IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            var factory = base.ConfigureDefaultRegionBehaviors();
            return factory;
        }

        /// <summary>
        /// Creates the <see cref="IModuleCatalog"/> used by Prism.
        /// 
        /// The base implementation returns a new ModuleCatalog.
        /// </summary>
        /// <returns>
        /// A ConfigurationModuleCatalog.
        /// </returns>
        protected override IModuleCatalog CreateModuleCatalog()
        {
            // Configure Prism ModuleCatalog via app.config configuration file
            return new ConfigurationModuleCatalog();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            // Register MessageBox Service for usage in other modules
            // use the following statement to resolve queries towards the IMessageBoxService
            // var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
            this.Container.ComposeExportedValue<IMessageBoxService>(_MsgBox);

            this.Container.ComposeExportedValue<IMRUListViewModel>(_MruVM);

            // Because we created the SettingsManager and it needs to be used immediately
            // we compose it to satisfy any imports it has.
            //
            // http://msdn.microsoft.com/en-us/library/ff921140%28v=pandp.40%29.aspx
            //
            this.Container.ComposeExportedValue<ISettingsManager>(this.mProgramSettingsManager);
            this.Container.ComposeExportedValue<IThemesManager>(this.mThemes);
        }
        #endregion Methods

        #region shell handling methods
        /// <summary>
        /// Interpret command line parameters and process their content
        /// </summary>
        /// <param name="args"></param>
        private static void ProcessCmdLine(IEnumerable<string> args, IApplicationViewModel appVM)
        {
            if (args != null)
            {
                logger.InfoFormat("TRACE Processing command line 'args' in App.ProcessCmdLine");

                foreach (string sPath in args)
                {
                    logger.InfoFormat("TRACE Processing CMD param: '{0}'", sPath);

                    // Command may not be bound yet so we do this via direct call
                    appVM.Open(sPath);
                }
            }
            else
                logger.InfoFormat("TRACE There are no command line 'args' to process in App.ProcessCmdLine");
        }

        /// <summary>
        /// COnstruct MainWindow an attach datacontext to it.
        /// </summary>
        /// <param name="workSpace"></param>
        /// <param name="win"></param>
        private void ConstructMainWindowSession(IApplicationViewModel workSpace,
                                                Window win,
                                                ISettingsManager settings)
        {
            win.DataContext = workSpace;

            // Establish command binding to accept user input via commanding framework
            workSpace.InitCommandBinding(win);

            win.Left = settings.SessionData.MainWindowPosSz.X;
            win.Top = settings.SessionData.MainWindowPosSz.Y;
            win.Width = settings.SessionData.MainWindowPosSz.Width;
            win.Height = settings.SessionData.MainWindowPosSz.Height;
            win.WindowState = (settings.SessionData.MainWindowPosSz.IsMaximized == true ? WindowState.Maximized : WindowState.Normal);

            // Initialize Window State in viewmodel to show resize grip when window is not maximized
            if (win.WindowState == WindowState.Maximized)
                workSpace.IsNotMaximized = false;
            else
                workSpace.IsNotMaximized = true;

            workSpace.IsWorkspaceAreaOptimized = settings.SessionData.IsWorkspaceAreaOptimized;

            string lastActiveFile = settings.SessionData.LastActiveFile;

            MainWindow mainWin = win as MainWindow;
        }
        #endregion shell handling methods
    }
}
