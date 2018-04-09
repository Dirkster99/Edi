using Edi.Apps.Interfaces;

namespace Edi
{
	using Apps.ViewModels;
    using Apps.Views.Shell;
    using Core;
    using Core.Interfaces;
    using Documents.Module;
    using Settings;
    using Settings.Interfaces;
    using Settings.ProgramSettings;
    using Themes.Interfaces;
    using Files.Module;
    using MLib.Interfaces;
    using MRULib.MRU.Interfaces;
    using MsgBox;
    using Output.Views;
    using Prism.Mef;
    using Prism.Modularity;
    using SimpleControls.Local;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Windows;

    public class Bootstapper : MefBootstrapper
    {
        #region fields
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private MainWindow mMainWin = null;
        private IApplicationViewModel appVM = null;

        private readonly StartupEventArgs mEventArgs;
        private readonly App mApp = null;

        private readonly IMessageBoxService _MsgBox = null;
        private readonly IMRUListViewModel _MruVM = null;

        private readonly Options mOptions = null;
        private readonly IThemesManager mThemes = null;
        private readonly ISettingsManager mProgramSettingsManager = null;
        #endregion fields

        /// <summary>
        /// Initializes static members of the <see cref="Bootstapper"/> class.
        /// This constructor will be called before MEF begins to take over.
        /// </summary>
        static Bootstapper()
        {
            System.IO.Directory.CreateDirectory(@".\Plugins");
        }

        #region constructors
        public Bootstapper(App app,
                           StartupEventArgs eventArgs,
                           Options programSettings,
                           IThemesManager themesManager)
            : this()
        {
            mThemes = themesManager;
            mProgramSettingsManager = new SettingsManager(mThemes);

            mEventArgs = eventArgs;
            mApp = app;
            mOptions = programSettings;
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
                return appVM;
            }
        }

        private MainWindow MainWindow
        {
            get
            {
                return MainWindow;
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
            try
            {
                base.Run(runWithDefaultConfiguration);

                // Register imported tool window definitions with Avalondock
                var toolWindowRegistry = Container.GetExportedValue<IToolWindowRegistry>();

                MEFLoadFiles.Initialize(appVM.AdLayout,
                                        mProgramSettingsManager,
                                        toolWindowRegistry,
                                        appVM as IFileOpenService);

                toolWindowRegistry.PublishTools();


                // Show the startpage if application starts for the very first time
                // (This requires that command binding was succesfully done before this line)
                if (appVM.AdLayout.LayoutSoure == Core.Models.Enums.LayoutLoaded.FromDefault)
                    AppCommand.ShowStartPage.Execute(null, null);

                if (mEventArgs != null)
                    ProcessCmdLine(mEventArgs.Args, appVM);

                // PRISM modules (and everything else) have been initialized if we got here
                var output = Container.GetExportedValue<IMessageManager>();
                output.Output.AppendLine("Get involved at: https://github.com/Dirkster99/Edi");

                appVM.EnableMainWindowActivated(true);
            }
            catch (Exception exp)
            {
                logger.Error(exp);
            }
        }

        protected override void ConfigureAggregateCatalog()
        {
            // Loading these items is equivalent to using static construction, except MEF runs the
            // decorated class and method to initialize each module
            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(IOutputView).Assembly));

            // These module register services (e.g.: file open) and are required for other plug-ins to work
////            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(MEFLoadFiles).Assembly));
            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(MefLoadEdiDocuments).Assembly));

            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(IAppCoreModel).Assembly));

            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(AvalonDockLayoutViewModel).Assembly));
            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstapper).Assembly));

            ////this.AggregateCatalog.Catalogs.Add(new PluginModuleCatalog(@".\Plugins"));

        }

        protected override DependencyObject CreateShell()
        {
            try
            {
                var appCore = Container.GetExportedValue<IAppCoreModel>();

                // Setup localtion of config files
                mProgramSettingsManager.AppDir = appCore.DirAppData;
                mProgramSettingsManager.LayoutFileName = appCore.LayoutFileName;

                var avLayout = Container.GetExportedValue<IAvalonDockLayoutViewModel>();
                appVM = Container.GetExportedValue<IApplicationViewModel>();

                var toolWindowRegistry = Container.GetExportedValue<IToolWindowRegistry>();

                appVM.LoadConfigOnAppStartup(mOptions, mProgramSettingsManager, mThemes);

                // Attempt to load a MiniUML plugin via the model class
                MiniUML.Model.MiniUmlPluginLoader.LoadPlugins(appCore.AssemblyEntryLocation + @".\Plugins\MiniUML.Plugins.UmlClassDiagram\", AppViewModel); // discover via Plugin folder instead

                mMainWin = Container.GetExportedValue<MainWindow>();

                appCore.CreateAppDataFolder();

                if (mMainWin != null)
                {
                    ConstructMainWindowSession(appVM, mMainWin, mProgramSettingsManager);

                    if (mApp.AppIsShuttingDown == false)
                        mApp.ShutdownMode = ShutdownMode.OnLastWindowClose;
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
                    if (mApp.AppIsShuttingDown == false)
                        mApp.ShutdownMode = ShutdownMode.OnExplicitShutdown;
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

                if (mApp.AppIsShuttingDown == false)
                    mApp.Shutdown();
            }

            return mMainWin;
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            Application.Current.MainWindow = (MainWindow)Shell;
            Application.Current.MainWindow.Show();
        }

        protected override Prism.Regions.IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
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
            return new PluginModuleCatalog(@".\Plugins", new ConfigurationModuleCatalog());
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            // Register MessageBox Service for usage in other modules
            // use the following statement to resolve queries towards the IMessageBoxService
            // var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
            Container.ComposeExportedValue<IMessageBoxService>(_MsgBox);

            Container.ComposeExportedValue<IMRUListViewModel>(_MruVM);

            // Because we created the SettingsManager and it needs to be used immediately
            // we compose it to satisfy any imports it has.
            //
            // http://msdn.microsoft.com/en-us/library/ff921140%28v=pandp.40%29.aspx
            //
            Container.ComposeExportedValue<ISettingsManager>(mProgramSettingsManager);
            Container.ComposeExportedValue<IThemesManager>(mThemes);
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
