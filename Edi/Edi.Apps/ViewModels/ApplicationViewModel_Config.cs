namespace Edi.Apps.ViewModels
{
    using System;
    using System.Windows;
    using Core.Interfaces;
    using Files.ViewModels.FileExplorer;
    using MsgBox;
    using Settings.Interfaces;
    using MRULib.MRU.Models.Persist;
    using System.Threading.Tasks;
    using Edi.Settings;
    using Edi.Themes.Interfaces;

    public partial class ApplicationViewModel
    {
        /// <summary>
        /// Save application settings when the application is being closed down
        /// </summary>
        public void SaveConfigOnAppClosed()
        {
            try
            {
                _AppCore.CreateAppDataFolder();

                // Save current explorer settings and user profile data
                // Query for an explorer tool window and return it
                // Query for an explorer tool window and return it
                var explorerTw = GetToolWindowVm<IExplorer>();

                if (explorerTw != null)
                    FileExplorerViewModel.SaveSettings(_SettingsManager, explorerTw);

                // Save program options only if there are un-saved changes that need persistence
                // This can be caused when WPF theme was changed or something else
                // but should normally not occur as often as saving session data
                if (_SettingsManager.SettingData.IsDirty)
                {
                    _SettingsManager.SaveOptions(_AppCore.DirFileAppSettingsData, _SettingsManager.SettingData);
                }

                // Convert viewmodel data into model for persistance layer...
                MRUEntrySerializer.ConvertToModel(_MruVM, _SettingsManager.SessionData.MruList);

                _SettingsManager.SaveSessionData(_AppCore.DirFileAppSessionData, _SettingsManager.SessionData);
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
        public async Task<IOptions> LoadConfigOnAppStartupAsync(IOptions programSettings,
                                                                ISettingsManager settings,
                                                                IThemesManager themes)
        {
            // Re/Load program options and user profile session data to control global behaviour of program
            await settings.LoadOptionsAsync(_AppCore.DirFileAppSettingsData, themes, programSettings);
            settings.LoadSessionData(_AppCore.DirFileAppSessionData);

            settings.CheckSettingsOnLoad(SystemParameters.VirtualScreenLeft, SystemParameters.VirtualScreenTop);

            // Initialize skinning engine with this current skin
            // standard skins defined in class enum PLUS
            // configured skins with highlighting
            themes.SetSelectedTheme(settings.SettingData.CurrentTheme);
            ResetTheme();                       // Initialize theme in process

            // Convert Session model into viewmodel instance
            MRUEntrySerializer.ConvertToViewModel(settings.SessionData.MruList, _MruVM);

            return programSettings;
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
                    e.Cancel = ShutDownInProgressCancel = true;
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
            }
        }

	    /// <summary>
	    /// Execute closing function and persist session data to be reloaded on next restart
	    /// </summary>
	    /// <param name="win"></param>
	    public void OnClosed(ILayoutableWindow win)
        {
            try
            {
                EnableMainWindowActivated(false);

                // Persist window position, width and height from this session
                _SettingsManager.SessionData.MainWindowPosSz =
                    SettingsFactory.GetViewPosition(win.Left, win.Top, win.Width, win.Height,
                                                    (win.WindowState == WindowState.Maximized));

                _SettingsManager.SessionData.IsWorkspaceAreaOptimized = IsWorkspaceAreaOptimized;

                // Save/initialize program options that determine global programm behaviour
                SaveConfigOnAppClosed();

                win.ReleaseResources();
                DisposeResources();
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
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
            if (_ToolRegistry != null)
                _ToolRegistry.PublishToolWindows -= OnPublisToolWindows;

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