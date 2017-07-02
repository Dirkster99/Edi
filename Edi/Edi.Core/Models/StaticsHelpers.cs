namespace Edi.Core.Models
{
	using System;
	using System.Globalization;
	using System.Reflection;
	using System.Windows;

	/// <summary>
	/// Class maintains and helps access to core facts of this application.
	/// Core facts are installation directory, name of application etc.
	/// 
	/// This class should not be used directly unless it is realy necessary.
	/// Use the <seealso cref="AppCoreModel"/> through its interface and
	/// constructor dependency injection to avoid unnecessary dependencies
	/// and problems when refactoring later on.
	/// </summary>
	public class AppHelpers
	{
		/// <summary>
		/// Link to public site where issues can be reported.
		/// </summary>
		public const string IssueTrackerLink = "https://edi.codeplex.com/workitem/list/basic";

		protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		#region properties
		/// <summary>
		/// Get a path to the directory where the application
		/// can persist/load user data on session exit and re-start.
		/// </summary>
		public static string DirAppData
		{
			get
			{
				return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
																				 System.IO.Path.DirectorySeparatorChar +
																				 AppHelpers.Company;
			}
		}

		/// <summary>
		/// Get a path to the directory where the user store his documents
		/// </summary>
		public static string MyDocumentsUserDir
		{
			get
			{
				return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			}
		}

		/// <summary>
		/// Get the name of the executing assembly (usually name of *.exe file)
		/// </summary>
		public static string AssemblyTitle
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
		public static string AssemblyEntryLocation
		{
			get
			{
				return System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			}
		}

		public static string Company
		{
			get
			{
				return "Edi";
			}
		}

		/// <summary>
		/// Get path and file name to application specific session file
		/// </summary>
		public static string DirFileAppSessionData
		{
			get
			{
				return System.IO.Path.Combine(AppHelpers.DirAppData,
																			string.Format(CultureInfo.InvariantCulture, "{0}.App.session",
																										AppHelpers.AssemblyTitle));
			}
		}

		/// <summary>
		/// Get path and file name to application specific settings file
		/// </summary>
		public static string DirFileAppSettingsData
		{
			get
			{
				return System.IO.Path.Combine(AppHelpers.DirAppData,
																			string.Format(CultureInfo.InvariantCulture, "{0}.App.settings",
																										AppHelpers.AssemblyTitle));
			}
		}
		#endregion properties

		#region methods
		/// <summary>
		/// Restore the applications window from minimized state
		/// into non-minimzed state and send it to the top to make
		/// sure its visible for the user.
		/// </summary>
		public static void RestoreCurrentMainWindow()
		{
			if (System.Windows.Application.Current != null)
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
