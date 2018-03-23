using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using Edi.Core.Interfaces;
using log4net;

namespace Edi.Core.Models
{
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
		public string IssueTrackerLink
		{
			get
			{
				return AppHelpers.IssueTrackerLink;
			}
		}

		/// <summary>
		/// Gets the file name of the layout file that is useful for AvalonDock.
		/// </summary>
		public string LayoutFileName
		{
			get
			{
				return "Layout.config";
			}
		}

		/// <summary>
		/// Get a path to the directory where the application
		/// can persist/load user data on session exit and re-start.
		/// </summary>
		public string DirAppData
		{
			get
			{
				return AppHelpers.DirAppData;
			}
		}

		/// <summary>
		/// Get a path to the directory where the user store his documents
		/// </summary>
		public string MyDocumentsUserDir
		{
			get
			{
				return AppHelpers.MyDocumentsUserDir;
			}
		}

		/// <summary>
		/// Get the name of the executing assembly (usually name of *.exe file)
		/// </summary>
		public string AssemblyTitle
		{
			get
			{
				return AppHelpers.AssemblyTitle;
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
				return AppHelpers.AssemblyEntryLocation;
			}
		}

		/// <summary>
		/// Gets the company string of this application.
		/// </summary>
		public string Company
		{
			get
			{
				return AppHelpers.Company;
			}
		}

		/// <summary>
		/// Get path and file name to application specific session file
		/// </summary>
		public string DirFileAppSessionData
		{
			get
			{
				return AppHelpers.DirFileAppSessionData;
			}
		}

		/// <summary>
		/// Get path and file name to application specific settings file
		/// </summary>
		public string DirFileAppSettingsData
		{
			get
			{
				return AppHelpers.DirFileAppSettingsData;
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
				if (Directory.Exists(DirAppData) == false)
					Directory.CreateDirectory(DirAppData);
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
