namespace Edi.Core.Interfaces
{
	/// <summary>
	/// Interface to a class that maintains and helps access to core facts of this application.
	/// Core facts are installation directory, name of application etc.
	/// </summary>
	public interface IAppCoreModel
	{

		#region properties
		string IssueTrackerLink { get; }

		/// <summary>
		/// Get a path to the directory where the user store his documents
		/// </summary>
		string MyDocumentsUserDir { get; }

		/// <summary>
		/// Get the name of the executing assembly (usually name of *.exe file)
		/// </summary>
		string AssemblyTitle { get; }

		//
		// Summary:
		//     Gets the path or UNC location of the loaded file that contains the manifest.
		//
		// Returns:
		//     The location of the loaded file that contains the manifest. If the loaded
		//     file was shadow-copied, the location is that of the file after being shadow-copied.
		//     If the assembly is loaded from a byte array, such as when using the System.Reflection.Assembly.Load(System.Byte[])
		//     method overload, the value returned is an empty string ("").
		string AssemblyEntryLocation { get; }

		string Company { get; }

		/// <summary>
		/// Get path and file name to application specific session file
		/// </summary>
		string DirFileAppSessionData { get; }

		/// <summary>
		/// Get path and file name to application specific settings file
		/// </summary>
		string DirFileAppSettingsData { get; }
		#endregion properties

		#region methods
		/// <summary>
		/// Create a dedicated directory to store program settings and session data
		/// </summary>
		/// <returns></returns>
		bool CreateAppDataFolder();

		/// <summary>
		/// Restore the applications window from minimized state
		/// into non-minimzed state and send it to the top to make
		/// sure its visible for the user.
		/// </summary>
		void RestoreCurrentMainWindow();
		#endregion methods
	}
}
