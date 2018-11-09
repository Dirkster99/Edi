namespace Edi
{
    using Castle.Windsor;
    using Castle.Windsor.Installer;
    using Edi.Apps;
    using Edi.Apps.Interfaces;
    using Edi.Apps.ViewModels;
    using Edi.Apps.Views.Shell;
    using Edi.Core;
    using Edi.Core.Interfaces;
    using Edi.Interfaces.App;
    using Edi.Interfaces.MessageManager;
    using Edi.Interfaces.Themes;
    using Edi.Settings.Interfaces;
    using Edi.Util;
    using Edi.Util.ActivateWindow;
    using log4net;
    using log4net.Config;
    using MsgBox;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region fields
        private readonly SingletonApplicationEnforcer enforcer =
                            new SingletonApplicationEnforcer(ProcessSecondInstance,
                                                             WindowLister.ActivateMainWindow, "Edi");

        protected static ILog Logger;
        private IWindsorContainer _Container;
        private static IAppCore _AppCore;
        #endregion fields

        #region constructors
        /// <summary>
        /// Static class constructor
        /// </summary>
        static App()
        {
            XmlConfigurator.Configure();
            Logger = LogManager.GetLogger("default");
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public App()
        {
            InitializeComponent();
            AppIsShuttingDown = false;

            SessionEnding += App_SessionEnding;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets whether the application is currently in the process of shutting down or not.
        /// </summary>
        public bool AppIsShuttingDown { get; set; }
        #endregion properties

        #region methods
        /// <summary>
        /// Process command line args and window activation when switching from 1st to 2nd instance
        /// </summary>
        /// <param name="args"></param>
        public static void ProcessSecondInstance(IEnumerable<string> args)
        {
            var dispatcher = Current.Dispatcher;
            if (dispatcher.CheckAccess())
            {
                // The current application is the first
                // This case is already handled via start-up code in App.cs
                // ShowArgs(args);
            }
            else
            {
                dispatcher.BeginInvoke(
                    new Action(delegate
                    {
                        var mainWindow = GetMainWindow();
                        _AppCore.RestoreCurrentMainWindow();

                        if (mainWindow != null)
                        {
                            if (mainWindow.IsVisible == false)
                                mainWindow.Show();

                            if (mainWindow.WindowState == WindowState.Minimized)
                                mainWindow.WindowState = WindowState.Normal;

                            mainWindow.Topmost = true;
                            //mainWindow.Show();

                            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (Action)delegate
                            {
                                bool bActive = mainWindow.Activate();

                                mainWindow.Topmost = false;
                            });
                        }


                        // Filter name of executeable if present in command line args
                        if (mainWindow.DataContext is ApplicationViewModel)
                        {
                            ApplicationViewModel appVM = mainWindow.DataContext as ApplicationViewModel;

                            if (args != null)
                                ProcessCmdLine(FilterAssemblyName(args), appVM);
                        }
                    }));
            }
        }

        /// <summary>
        /// Method executes as application entry point - that is -
        /// this bit of code executes before anything else in this
        /// class and application.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Set shutdown mode here (and reset further below) to enable showing custom dialogs (messageboxes)
                // durring start-up without shutting down application when the custom dialogs (messagebox) closes
                ShutdownMode = ShutdownMode.OnExplicitShutdown;
            }
            catch
            {
            }

            IOptions options = null;
            ISettingsManager settingsManager = null;
            IThemesManager themesManager = null;
            try
            {
                _Container = new WindsorContainer();

                // This allows castle to look at the current assembly and look for implementations
                // of the IWindsorInstaller interface
                _Container.Install(FromAssembly.This());                         // Register

                // Resolve SettingsManager to retrieve app settings/session data
                // and start with correct parameters from last session (theme, window pos etc...)
                settingsManager = _Container.Resolve<ISettingsManager>();
                themesManager = _Container.Resolve<IThemesManager>();
                _AppCore = _Container.Resolve<IAppCore>();

                var task = Task.Run(async () => // Off Loading Load Programm Settings to non-UI thread
                {
                    options = await settingsManager.LoadOptionsAsync(_AppCore.DirFileAppSettingsData, themesManager);
                });
                task.Wait(); // Block this to ensure that results are usable in next steps of sequence

                Thread.CurrentThread.CurrentCulture = new CultureInfo(options.LanguageSelected);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(options.LanguageSelected);

                if (options.RunSingleInstance == true)
                {
                    if (enforcer.ShouldApplicationExit() == true)
                    {
                        if (AppIsShuttingDown == false)
                        {
                            AppIsShuttingDown = true;
                            Shutdown();
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
                Console.WriteLine("");
                Console.WriteLine("Unexpected Error 1 in App.Application_Startup()");
                Console.WriteLine("   Message:{0}", exp.Message);
                Console.WriteLine("StackTrace:{0}", exp.StackTrace);
                Console.WriteLine("");
            }

            var AppViewModel = _Container.Resolve<IApplicationViewModel>();
            var task1 = Task.Run(async () => // Off Loading Load Programm Settings to non-UI thread
            {
                await AppViewModel.LoadConfigOnAppStartupAsync(options, settingsManager, themesManager);
            });
            task1.Wait(); // Block this to ensure that results are usable in next steps of sequence

            var start = _Container.Resolve<IShell<MainWindow>>();     // Resolve

            //resolve our shell to start the application.
            if (start != null)
            {
                start.ConfigureSession(AppViewModel, settingsManager);
                AppViewModel.EnableMainWindowActivated(true);

/////                var toolWindowRegistry = _Container.Resolve<IToolWindowRegistry>();
/////                toolWindowRegistry.PublishTools();

                if (this.AppIsShuttingDown == false)
                    this.ShutdownMode = ShutdownMode.OnLastWindowClose;
            }
            else
                throw new Exception("Main Window construction failed in application boot strapper class.");

            // Show the startpage if application starts for the very first time
            // (This requires that command binding was succesfully done before this line)
            if (string.IsNullOrEmpty(settingsManager.SessionData.LastActiveFile))
                AppViewModel.ShowStartPage();

            start.Run();                                              // Show the mainWindow

            var msgBox = _Container.Resolve<IMessageBoxService>();

            // discover via Plugin folder instead
            MiniUML.Model.MiniUmlPluginLoader.LoadPlugins(
                _AppCore.AssemblyEntryLocation + @".\Plugins\MiniUML.Plugins.UmlClassDiagram\",
                AppViewModel, msgBox);


            if (e != null)
                ProcessCmdLine(e.Args, AppViewModel);

            // Modules (and everything else) have been initialized if we got here
            var output = _Container.Resolve<IMessageManager>();
            output.Output.AppendLine("Get involved at: https://github.com/Dirkster99/Edi");

            _AppCore.CreateAppDataFolder();

            // Cannot set shutdown mode when application is already shuttong down
//            try
//            {
//                if (AppIsShuttingDown == false)
//                    ShutdownMode = ShutdownMode.OnExplicitShutdown;
//            }
//            catch
//            {
//            }

            // 1) Application hangs when this is set to null while MainWindow is visible
            // 2) Application throws exception when this is set as owner of window when it
            //    was never visible.
            //
            if (Current.MainWindow != null)
            {
                if (Current.MainWindow.IsVisible == false)
                    Current.MainWindow = null;
            }

/////            if (AppIsShuttingDown == false)
/////                Shutdown();
        }

        /// <summary>
        /// Check if end of application session should be canceled or not
        /// (we may have gotten here through unhandled exceptions - so we
        /// display it and attempt CONTINUE so user can save his data.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            base.OnSessionEnding(e);

            try
            {
                try
                {
                    Logger.Error(string.Format(CultureInfo.InvariantCulture,
                                             "The {0} application received request to shutdown: {1}.",
                                             ResourceAssembly.GetName(), e.ReasonSessionEnding.ToString()));
                }
                catch
                {
                }

                var mainWin = GetMainWindow();
                var appVM = GetWorkSpace();

                if (mainWin != null && appVM != null)
                {
                    if (mainWin.DataContext != null && appVM.Files != null)
                    {
                        // Close all open files and check whether application is ready to close
                        if (appVM.Exit_CheckConditions(mainWin) == true)
                            e.Cancel = false;
                        else
                            e.Cancel = appVM.ShutDownInProgressCancel = true;
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
            }

            try
            {
                _Container.Dispose();
                _Container = null;
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
            }
        }

        /// <summary>
        /// Interpret command line parameters and process their content
        /// </summary>
        /// <param name="args"></param>
        private static void ProcessCmdLine(IEnumerable<string> args,
                                           IApplicationViewModel appVM)
        {
            if (args != null)
            {
                Logger.InfoFormat("TRACE Processing command line 'args' in App.ProcessCmdLine");

                foreach (string sPath in args)
                {
                    Logger.InfoFormat("TRACE Processing CMD param: '{0}'", sPath);

                    // Command may not be bound yet so we do this via direct call
                    appVM.Open(sPath);
                }
            }
            else
                Logger.InfoFormat("TRACE There are no command line 'args' to process in App.ProcessCmdLine");
        }

        /// <summary>
        /// Filter name of executeable if present in command line args
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static List<string> FilterAssemblyName(IEnumerable<string> args)
        {
            List<string> filterCmdLineArgs = new List<string>();

            if (args != null)
            {
                int Cnt = 0;
                foreach (string s in args)
                {
                    Cnt++;

                    if (Cnt == 1)  // Always remove first command line parameter
                        continue;    // since this is the assembly entry name (Edi.exe)

                    filterCmdLineArgs.Add(s);
                }
            }

            return filterCmdLineArgs;
        }

        private static Window GetMainWindow()
        {
            return Current.MainWindow as Window;
        }

        private static ApplicationViewModel GetWorkSpace()
        {

            if (Current.MainWindow is Window)
            {
                var mainWindow = Current.MainWindow as Window;
                return mainWindow.DataContext as ApplicationViewModel;
            }

            return null;
        }

        /// <summary>
        /// Handle unhandled exception here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string message = string.Empty;

            try
            {
                if (e.Exception != null)
                {
                    message = string.Format(CultureInfo.CurrentCulture, "{0}\n\n{1}", e.Exception.Message, e.Exception.ToString());
                }
                else
                    message = Util.Local.Strings.STR_Msg_UnknownError;

                Logger.Error(message);

                var msgBox = _Container.Resolve<IMessageBoxService>();
                var appCore = _Container.Resolve<IAppCore>();

                msgBox.Show(e.Exception, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                            MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                            appCore.IssueTrackerLink, appCore.IssueTrackerLink,
                            Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);

                e.Handled = true;
            }
            catch (Exception exp)
            {
                Logger.Error(Util.Local.Strings.STR_MSG_UnknownError_InErrorDispatcher, exp);
            }
        }

        /// <summary>
        /// Method executes event based when the user ends the Windows
        /// session by logging off or shutting down the operating system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            AppIsShuttingDown = true;
        }
        #endregion methods
    }
}
