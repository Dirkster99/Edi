namespace Edi.Apps.ViewModels
{
    using System;
    using System.Windows;
    using Edi.Core.Interfaces;
    using Files.ViewModels.FileExplorer;
    using MsgBox;
    using Edi.Settings.Interfaces;
    using Edi.Settings.ProgramSettings;
    using Edi.Settings.UserProfile;
    using Edi.Themes.Interfaces;
    using MRULib.MRU.Interfaces;
    using MRULib.MRU.Models.Persist;
    using CommonServiceLocator;
    using MLib.Interfaces;
    using System.Windows.Media;

    public partial class ApplicationViewModel
    {
        /// <summary>
        /// Save application settings when the application is being closed down
        /// </summary>
        public void SaveConfigOnAppClosed()
        {
            try
            {
                this._mAppCore.CreateAppDataFolder();

                // Save current explorer settings and user profile data
                // Query for an explorer tool window and return it
                // Query for an explorer tool window and return it
                var explorerTw = this.GetToolWindowVm<IExplorer>();

                if (explorerTw != null)
                    FileExplorerViewModel.SaveSettings(this._mSettingsManager, explorerTw);

                // Save program options only if there are un-saved changes that need persistence
                // This can be caused when WPF theme was changed or something else
                // but should normally not occur as often as saving session data
                if (this._mSettingsManager.SettingData.IsDirty == true)
                {
                    this._mSettingsManager.SaveOptions(this._mAppCore.DirFileAppSettingsData, this._mSettingsManager.SettingData);
                }

                // Convert viewmodel data into model for persistance layer...
                var mruVm = ServiceLocator.Current.GetInstance<IMRUListViewModel>();
                MRUEntrySerializer.ConvertToModel(mruVm, this._mSettingsManager.SessionData.MruList);

                this._mSettingsManager.SaveSessionData(this._mAppCore.DirFileAppSessionData, this._mSettingsManager.SessionData);
            }
            catch (Exception exp)
            {
                _msgBox.Show(exp, "Unhandled Exception", MsgBoxButtons.OK, MsgBoxImage.Error);
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
            settings.LoadOptions(this._mAppCore.DirFileAppSettingsData, themes, programSettings);
            settings.LoadSessionData(this._mAppCore.DirFileAppSessionData);

            settings.CheckSettingsOnLoad(SystemParameters.VirtualScreenLeft, SystemParameters.VirtualScreenTop);

            // Convert Session model into viewmodel instance
            var mruVm = ServiceLocator.Current.GetInstance<IMRUListViewModel>();
            MRUEntrySerializer.ConvertToViewModel(settings.SessionData.MruList, mruVm);

            // Initialize skinning engine with this current skin
            // standard skins defined in class enum PLUS
            // configured skins with highlighting
            themes.SetSelectedTheme(settings.SettingData.CurrentTheme);
            this.ResetTheme();                       // Initialize theme in process
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
                if (this.Exit_CheckConditions(sender) == true)      // Close all open files and check whether application is ready to close
                {
                    this.OnRequestClose();                          // (other than exception and error handling)

                    e.Cancel = false;
                    //if (wsVM != null)
                    //  wsVM.SaveConfigOnAppClosed(); // Save application layout
                }
                else
                    e.Cancel = this.ShutDownInProgressCancel = true;
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
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
                this.EnableMainWindowActivated(false);

                // Persist window position, width and height from this session
                this._mSettingsManager.SessionData.MainWindowPosSz =
                    new ViewPosSizeModel(win.Left, win.Top, win.Width, win.Height,
                                                             (win.WindowState == WindowState.Maximized ? true : false));

                this._mSettingsManager.SessionData.IsWorkspaceAreaOptimized = this.IsWorkspaceAreaOptimized;

                // Save/initialize program options that determine global programm behaviour
                this.SaveConfigOnAppClosed();

                this.DisposeResources();
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
                _msgBox.Show(exp.ToString(),
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
                foreach (var item in this.Files)
                {
                    try
                    {
                        item.Dispose();
                    }
                    catch (Exception exp)
                    {
                        Logger.ErrorFormat("Error disposing file; {0}", item.FileName);
                        Logger.Error(exp);
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
            }
        }
    }
}
