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
		private static MainMenu mMainMenu = null;
		private static StatusBar mMainWindowStatusBar = null;
		#endregion fields

		#region constructor
		/// <summary>
		/// Staic class constructor
		/// </summary>
		static MainWindowViewManager()
		{
		}
		#endregion constructor

		#region properties
		/// <summary>
		/// Get the currently available main menu to be displayed in the main menu.
		/// </summary>
		public static MainMenu MainWindowMenu
		{
			get
			{
				if (MainWindowViewManager.mMainMenu == null)
					MainWindowViewManager.mMainMenu = new MainMenu();

				return MainWindowViewManager.mMainMenu;
			}
		}

		public static StatusBar MainWindowStatusBar
		{
			get
			{
				if (MainWindowViewManager.mMainWindowStatusBar == null)
					MainWindowViewManager.mMainWindowStatusBar = new StatusBar();

				return MainWindowViewManager.mMainWindowStatusBar;
			}
		}
		#endregion properties
	}
}
