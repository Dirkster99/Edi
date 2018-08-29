namespace Edi.Core.Models
{
    using System;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Reflection;
    using Edi.Core.Interfaces;
    using log4net;

    /// <summary>
    /// Class maintains and helps access to core facts of this application.
    /// Core facts are installation directory, name of application etc.
    /// </summary>
    [Export(typeof(IAppCoreModel))]
	public class AppCoreModel : IAppCoreModel
	{
		protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		#region constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		[ImportingConstructor]
		public AppCoreModel()
		{
		}
		#endregion constructors

		#region properties
		/// <summary>
		/// Gets a string that denotes an internet link to
		/// a web site where users can enter their issues.
		/// </summary>
		public string IssueTrackerLink => AppHelpers.IssueTrackerLink;

		/// <summary>
		/// Get a path to the directory where the user store his documents
		/// </summary>
		public string MyDocumentsUserDir => AppHelpers.MyDocumentsUserDir;

		/// <summary>
		/// Get the name of the executing assembly (usually name of *.exe file)
		/// </summary>
		public string AssemblyTitle => AppHelpers.AssemblyTitle;

		//
		// Summary:
		//     Gets the path or UNC location of the loaded file that contains the manifest.
		//
		// Returns:
		//     The location of the loaded file that contains the manifest. If the loaded
		//     file was shadow-copied, the location is that of the file after being shadow-copied.
		//     If the assembly is loaded from a byte array, such as when using the System.Reflection.Assembly.Load(System.Byte[])
		//     method overload, the value returned is an empty string ("").
		public string AssemblyEntryLocation => AppHelpers.AssemblyEntryLocation;

		/// <summary>
		/// Gets the company string of this application.
		/// </summary>
		public string Company => AppHelpers.Company;

		/// <summary>
		/// Get path and file name to application specific session file
		/// </summary>
		public string DirFileAppSessionData => AppHelpers.DirFileAppSessionData;

		/// <summary>
		/// Get path and file name to application specific settings file
		/// </summary>
		public string DirFileAppSettingsData => AppHelpers.DirFileAppSettingsData;
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
                var path = AppHelpers.DirAppData;

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
			AppHelpers.RestoreCurrentMainWindow();
		}
		#endregion methods
	}
}
