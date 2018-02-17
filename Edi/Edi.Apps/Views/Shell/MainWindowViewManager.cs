namespace Edi.Apps.Views.Shell
{
	/// <summary>
	/// This class manages global settings such as a
	/// 
	/// 1> MainMenu control,
	/// </summary>
	public static class MainWindowViewManager
	{
		#region fields
		private static MainMenu mMainMenu;
		private static StatusBar mMainWindowStatusBar;
		#endregion fields

		#region constructor

	    #endregion constructor

		#region properties
		/// <summary>
		/// Get the currently available main menu to be displayed in the main menu.
		/// </summary>
		public static MainMenu MainWindowMenu => mMainMenu ?? (mMainMenu = new MainMenu());

	    public static StatusBar MainWindowStatusBar => mMainWindowStatusBar ?? (mMainWindowStatusBar = new StatusBar());

	    #endregion properties
	}
}
