namespace Edi.Apps.ViewModels
{
    using System;
    using System.Windows;
    using Core.Interfaces;
    using Files.ViewModels.FileExplorer;
    using Settings.Interfaces;
    using Settings.ProgramSettings;
    using Settings.UserProfile;
    using Themes.Interfaces;

    public partial class ApplicationViewModel
    {
        /// <summary>
        /// Save application settings when the application is being closed down
        /// </summary>
        public void SaveConfigOnAppClosed()
        {
            try
            {
                mAppCore.CreateAppDataFolder();

                // Save current explorer settings and user profile data
                // Query for an explorer tool window and return it
                // Query for an explorer tool window and return it
                var explorerTW = GetToolWindowVM<IExplorer>();

                if (explorerTW != null)
                    FileExplorerViewModel.SaveSettings(mSettingsManager, explorerTW);

                // Save program options only if there are un-saved changes that need persistence
                // This can be caused when WPF theme was changed or something else
                // but should normally not occur as often as saving session data
                if (mSettingsManager.SettingData.IsDirty)
                {
                    mSettingsManager.SaveOptions(mAppCore.DirFileAppSettingsData, mSettingsManager.SettingData);
                }

                // Convert viewmodel data into model for persistance layer...
                var mruVM = ServiceLocator.Current.GetInstance<IMRUListViewModel>();
                MRUEntrySerializer.ConvertToModel(mruVM, mSettingsManager.SessionData.MruList);

                mSettingsManager.SaveSessionData(mAppCore.DirFileAppSessionData, mSettingsManager.SessionData);
            }
            catch (Exception exp)
            {
                _MsgBox.Show(exp, "Unhandled Exception", MsgBoxButtons.OK, MsgBoxImage.Error);
            }
        }

        /// <summary>
        /// Load configuration from persistence on startup of application
        /// </summary>
        /// <param name="programSettings"></param>
        /// <param name="settings"></param>
        /// <param name="themes"></param>
        public void LoadConfigOnAppStartup(Options programSettings,
                                            ISettingsManager settings,
                                            IThemesManager themes)
        {
            // Re/Load program options and user profile session data to control global behaviour of program
            settings.LoadOptions(mAppCore.DirFileAppSettingsData, themes, programSettings);
            settings.LoadSessionData(mAppCore.DirFileAppSessionData);

            settings.CheckSettingsOnLoad(SystemParameters.VirtualScreenLeft, SystemParameters.VirtualScreenTop);

            // Convert Session model into viewmodel instance
            var mruVM = ServiceLocator.Current.GetInstance<IMRUListViewModel>();
            MRUEntrySerializer.ConvertToViewModel(settings.SessionData.MruList, mruVM);

            // Initialize skinning engine with this current skin
            // standard skins defined in class enum
            // plus configured skins with highlighting
            themes.SetSelectedTheme(settings.SettingData.CurrentTheme);
            ResetTheme();                       // Initialize theme in process
        }

        /// <summary>
        /// Save session data on closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (Exit_CheckConditions(sender))      // Close all open files and check whether application is ready to close
                {
                    OnRequestClose();                          // (other than exception and error handling)

                    e.Cancel = false;
                    //if (wsVM != null)
                    //  wsVM.SaveConfigOnAppClosed(); // Save application layout
                }
                else
                    e.Cancel = ShutDownInProgress_Cancel = true;
            }
            catch (Exception exp)
            {
                logger.Error(exp);
            }
        }

        /// <summary>
        /// Execute closing function and persist session data to be reloaded on next restart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnClosed(Window win)
        {
            try
            {
                EnableMainWindowActivated(false);

                // Persist window position, width and height from this session
                mSettingsManager.SessionData.MainWindowPosSz =
                    new ViewPosSizeModel(win.Left, win.Top, win.Width, win.Height,
                                                             (win.WindowState == WindowState.Maximized ? true : false));

                mSettingsManager.SessionData.IsWorkspaceAreaOptimized = IsWorkspaceAreaOptimized;

                // Save/initialize program options that determine global programm behaviour
                SaveConfigOnAppClosed();

                DisposeResources();
            }
            catch (Exception exp)
            {
                logger.Error(exp);
                _MsgBox.Show(exp.ToString(),
                             Util.Local.Strings.STR_MSG_UnknownError_InShutDownProcess,
                             MsgBoxButtons.OK, MsgBoxImage.Error);
            }
        }

        /// <summary>
        /// Disposes all reserved resources when the application is in its last phase of shuttng down.
        /// </summary>
        private void DisposeResources()
        {
            try
            {
                foreach (var item in Files)
                {
                    try
                    {
                        item.Dispose();
                    }
                    catch (Exception exp)
                    {
                        logger.ErrorFormat("Error disposing file; {0}", item.FileName);
                        logger.Error(exp);
                    }
                }
            }
            catch (Exception exp)
            {
                logger.Error(exp);
            }
        }
    }
}
