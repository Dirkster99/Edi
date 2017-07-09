namespace Edi
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Threading;
	using System.Windows;
	using System.Windows.Threading;
	using Edi.Core.Models;
	using Edi.Apps.ViewModels;
	using Edi.Apps.Views.Shell;
	using log4net;
	using log4net.Config;
	using MsgBox;
	using Edi.Settings;
	using Edi.Settings.ProgramSettings;
	using SimpleControls.Local;
	using Edi.Themes;
	using Edi.Themes.Interfaces;
	using Edi.Util;
	using Edi.Util.ActivateWindow;

	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		#region fields
		readonly SingletonApplicationEnforcer enforcer = new SingletonApplicationEnforcer(ProcessSecondInstance, WindowLister.ActivateMainWindow, "Edi");

		protected static log4net.ILog Logger;

		static App()
		{
			XmlConfigurator.Configure();
			Logger = LogManager.GetLogger("default");
		}

		private Bootstapper mBoot;
		#endregion fields

		#region constructor
		public App()
		{
			this.InitializeComponent();
			this.AppIsShuttingDown = false;

			this.SessionEnding += App_SessionEnding;
		}

		void App_SessionEnding(object sender, SessionEndingCancelEventArgs e)
		{
			this.AppIsShuttingDown = true;
		}
		#endregion constructor

		public bool AppIsShuttingDown { get; set; }

		#region methods
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
											 Application.ResourceAssembly.GetName(), e.ReasonSessionEnding.ToString()));
				}
				catch
				{
				}

				var mainWin = App.GetMainWindow();
				var appVM = App.GetWorkSpace();

				if (mainWin != null && appVM != null)
				{
					if (mainWin.DataContext != null && appVM.Files != null)
					{
						// Close all open files and check whether application is ready to close
						if (appVM.Exit_CheckConditions(mainWin) == true)
							e.Cancel = false;
						else
							e.Cancel = appVM.ShutDownInProgress_Cancel = true;
					}
				}
			}
			catch (Exception exp)
			{
				Logger.Error(exp);
			}
		}

		/// <summary>
		/// This is the first bit of code being executed when the application is invoked (main entry point).
		/// 
		/// Use the <paramref name="e"/> parameter to evaluate command line options.
		/// Invoking a program with an associated file type extension (eg.: *.txt) in Explorer
		/// results, for example, in executing this function with the path and filename being
		/// supplied in <paramref name="e"/>.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			try
			{
				// Set shutdown mode here (and reset further below) to enable showing custom dialogs (messageboxes)
				// durring start-up without shutting down application when the custom dialogs (messagebox) closes
				this.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
			}
			catch
			{
			}

			Options options = null;
			IThemesManager themesManager = new ThemesManager();

			try
			{
				options = SettingsManager.LoadOptions(AppHelpers.DirFileAppSettingsData, themesManager);

				Thread.CurrentThread.CurrentCulture = new CultureInfo(options.LanguageSelected);
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(options.LanguageSelected);

				if (options.RunSingleInstance == true)
				{
					if (enforcer.ShouldApplicationExit() == true)
					{
						if (this.AppIsShuttingDown == false)
						{
							this.AppIsShuttingDown = true;
							this.Shutdown();
						}
					}
				}
			}
			catch (Exception exp)
			{
				Logger.Error(exp);
			}

			try
			{
				this.mBoot = new Bootstapper(this, e, options, themesManager);
				this.mBoot.Run();
			}
			catch (Exception exp)
			{
				Logger.Error(exp);

				// Cannot set shutdown mode when application is already shuttong down
				if (this.AppIsShuttingDown == false)
					this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

				// 1) Application hangs when this is set to null while MainWindow is visible
				// 2) Application throws exception when this is set as owner of window when it
				//    was never visible.
				//
				if (Application.Current.MainWindow != null)
				{
					if (Application.Current.MainWindow.IsVisible == false)
						Application.Current.MainWindow = null;
				}
				
				MsgBox.Msg.Show(exp, Strings.STR_MSG_ERROR_FINDING_RESOURCE, MsgBoxButtons.OKCopy);

				if (this.AppIsShuttingDown == false)
					this.Shutdown();
			}
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
					message = Edi.Util.Local.Strings.STR_Msg_UnknownError;

				Logger.Error(message);

				Msg.Show(e.Exception, Edi.Util.Local.Strings.STR_MSG_UnknownError_Caption,
									MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
									AppHelpers.IssueTrackerLink,
									AppHelpers.IssueTrackerLink,
                                    Edi.Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);

				e.Handled = true;
			}
			catch (Exception exp)
			{
				Logger.Error(Edi.Util.Local.Strings.STR_MSG_UnknownError_InErrorDispatcher, exp);
			}
		}

		/// <summary>
		/// Interpret command line parameters and process their content
		/// </summary>
		/// <param name="args"></param>
		private static void ProcessCmdLine(IEnumerable<string> args, ApplicationViewModel appVM)
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
		/// Process command line args and window activation when switching from 1st to 2nd instance
		/// </summary>
		/// <param name="args"></param>
		public static void ProcessSecondInstance(IEnumerable<string> args)
		{
			var dispatcher = Application.Current.Dispatcher;
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
						AppHelpers.RestoreCurrentMainWindow();

						var mainWindow = Application.Current.MainWindow as MainWindow;

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
                        if (mainWindow.DataContext is ApplicationViewModel appVM)
                        {
                            if (args != null)
                                ProcessCmdLine(App.FilterAssemblyName(args), appVM);
                        }

                    }));
			}
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

		private static MainWindow GetMainWindow()
		{
			return Application.Current.MainWindow as MainWindow;
		}

		private static ApplicationViewModel GetWorkSpace()
		{

            if (Application.Current.MainWindow is MainWindow mainWindow)
                return mainWindow.DataContext as ApplicationViewModel;

            return null;
		}
		#endregion methods
	}
}
