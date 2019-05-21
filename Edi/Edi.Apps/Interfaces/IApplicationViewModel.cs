namespace Edi.Apps.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using Edi.Apps.Enums;
    using Edi.Core.Interfaces;
    using Edi.Settings.Interfaces;
    using Edi.Themes.Interfaces;
    using MiniUML.Model.ViewModels.Document;

    /// <summary>
    /// This interface models the viewmodel that manages the complete
    /// application life cyle from start to end. It publishes the methodes,
    /// properties, and events necessary to integrate the application into
    /// a given shell (BootStrapper, App.xaml.cs etc).
    /// </summary>
    public interface IApplicationViewModel : IMiniUMLDocument
	{
		/// <summary>
		/// Raised when this workspace should be removed from the UI.
		/// </summary>
		event EventHandler RequestClose;

		#region properties
		/// <summary>
		/// Get/set property to determine whether window is in maximized state or not.
		/// (this can be handy to determine when a resize grip should be shown or not)
		/// </summary>
		bool? IsNotMaximized { get; set; }

		/// <summary>
		/// Gets/sets whether the workspace area is optimized or not.
		/// The optimized workspace is distructive free and does not
		/// show optional stuff like toolbar and status bar.
		/// </summary>
		bool IsWorkspaceAreaOptimized { get; set; }

		/// <summary>
		/// Gets/sets whether the current application shut down process
		/// is cancelled or not.
		/// </summary>
		bool ShutDownInProgressCancel { get; set; }

		/// <summary>
		/// Expose command to load/save AvalonDock layout on application startup and shut-down.
		/// </summary>
		IAvalonDockLayoutViewModel AdLayout { get; }
		#endregion properties

		#region methods
		/// <summary>
		/// Method to be executed when user (or program) tries to close the application
		/// </summary>
		void OnRequestClose();

		/// <summary>
		/// Save session data on closing
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnClosing(object sender, System.ComponentModel.CancelEventArgs e);

		/// <summary>
		/// Execute closing function and persist session data to be reloaded on next restart
		/// </summary>
		/// <param name="win"></param>
		void OnClosed(Window win);

		/// <summary>
		/// Check if pre-requisites for closing application are available.
		/// Save session data on closing and cancel closing process if necessary.
		/// </summary>
		/// <returns>true if application is OK to proceed closing with closed, otherwise false.</returns>
		bool Exit_CheckConditions(object sender);

        /// <summary>
        /// Load configuration from persistence on startup of application
        /// </summary>
        Task<IOptions> LoadConfigOnAppStartupAsync(IOptions programSettings,
									               ISettingsManager settings,
									               IThemesManager themes);

		/// <summary>
		/// Save application settings when the application is being closed down
		/// </summary>
		void SaveConfigOnAppClosed();

		/// <summary>
		/// Bind a window to some commands to be executed by the viewmodel.
		/// </summary>
		/// <param name="win"></param>
		void InitCommandBinding(Window win);

		/// <summary>
		/// Open a file supplied in <paramref name="filePath"/> (without displaying a file open dialog).
		/// </summary>
		/// <param name="filePath">file to open</param>
		/// <param name="closeDocumentWithoutMessageOnError"></param>
		/// <param name="addIntoMru">indicate whether file is to be added into MRU or not</param>
		/// <param name="typeOfDocument"></param>
		/// <returns></returns>
		IFileBaseViewModel Open(string filePath,
								CloseDocOnError closeDocumentWithoutMessageOnError = CloseDocOnError.WithUserNotification,
								bool addIntoMru = true,
								string typeOfDocument = "EdiTextEditor");

		/// <summary>
		/// Activates/deactivates processing of the mainwindow activated event.
		/// </summary>
		/// <param name="bActivate"></param>
		void EnableMainWindowActivated(bool bActivate);

        void ShowStartPage();
        #endregion methods
    }
}
