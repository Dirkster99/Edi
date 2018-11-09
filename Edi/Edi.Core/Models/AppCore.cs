namespace Edi.Core.Models
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using Edi.Interfaces.App;
    using log4net;

    /// <summary>
    /// Class maintains and helps access to core facts of this application.
    /// Core facts are installation directory, name of application etc.
    /// 
    /// This class should not be used directly unless it is realy necessary.
    /// Use the <seealso cref="AppCoreModel"/> through its interface and
    /// constructor dependency injection to avoid unnecessary dependencies
    /// and problems when refactoring later on.
    /// </summary>
    public class AppCore : IAppCore //  Installs in Edi.Core.Installer
    {
        #region fields
        protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion fields

        #region properties
        /// <summary>
        /// Link to public site where issues can be reported.
        /// </summary>
        public string IssueTrackerLink { get { return "https://github.com/Dirkster99/Edi/issues"; } }

        /// <summary>
        /// Get a path to the directory where the application
        /// can persist/load user data on session exit and re-start.
        /// </summary>
        public string DirAppData
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                    Path.DirectorySeparatorChar +
                                    Company;
            }
        }

		/// <summary>
		/// Get a path to the directory where the user store his documents
		/// </summary>
		public string MyDocumentsUserDir
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }

		/// <summary>
		/// Get the name of the executing assembly (usually name of *.exe file)
		/// </summary>
		public string AssemblyTitle
        {
            get
            {
                return Assembly.GetEntryAssembly().GetName().Name;
            }
        }

		//
		// Summary:
		//     Gets the path or UNC location of the loaded file that contains the manifest.
		//
		// Returns:
		//     The location of the loaded file that contains the manifest. If the loaded
		//     file was shadow-copied, the location is that of the file after being shadow-copied.
		//     If the assembly is loaded from a byte array, such as when using the System.Reflection.Assembly.Load(System.Byte[])
		//     method overload, the value returned is an empty string ("").
		public string AssemblyEntryLocation
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
        }

		public string Company { get { return "Edi"; } }

		/// <summary>
		/// Get path and file name to application specific session file
		/// </summary>
		public string DirFileAppSessionData
        {
            get
            {
                return Path.Combine(DirAppData,
                                    string.Format(CultureInfo.InvariantCulture, "{0}.App.session",
                                        AssemblyTitle));
            }
        }

		/// <summary>
		/// Get path and file name to application specific settings file
		/// </summary>
		public string DirFileAppSettingsData
        {
            get
            {
                return Path.Combine(DirAppData,
                                    string.Format(CultureInfo.InvariantCulture, "{0}.App.settings",
                                        AssemblyTitle));
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Create a dedicated directory to store program settings and session data
        /// </summary>
        /// <returns></returns>
        public bool CreateAppDataFolder()
        {
            try
            {
                var path = this.DirAppData;

                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Restore the applications window from minimized state
        /// into non-minimzed state and send it to the top to make
        /// sure its visible for the user.
        /// </summary>
        public void RestoreCurrentMainWindow()
		{
			if (Application.Current != null)
			{
				if (Application.Current.MainWindow != null)
				{
					Window win = Application.Current.MainWindow;

					if (win.IsVisible == false)
						win.Show();

					if (win.WindowState == WindowState.Minimized)
						win.WindowState = WindowState.Normal;

					win.Topmost = true;
					win.Topmost = false;
				}
			}
		}
		#endregion methods
	}
}
