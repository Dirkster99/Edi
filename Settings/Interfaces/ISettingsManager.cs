namespace Settings.Interfaces
{
	using Settings.ProgramSettings;
	using Settings.UserProfile;
	using Edi.Themes.Interfaces;

	public interface ISettingsManager
	{
		Options SettingData { get; }

		Profile SessionData { get; }

		string LayoutFileName { get; set; }

		string AppDir { get; set; }

		#region methods
		/// <summary>
		/// Save program options into persistence.
		/// See <seealso cref="LoadOptions"/> to load program options on program start.
		/// </summary>
		/// <param name="settingsFileName"></param>
		/// <param name="optionsModel"></param>
		/// <returns></returns>
		bool SaveOptions(string settingsFileName, Options optionsModel);

		/// <summary>
		/// Save program options into persistence.
		/// See <seealso cref="SaveOptions"/> to save program options on program end.
		/// </summary>
		/// <param name="settingsFileName"></param>
		/// <returns></returns>
		void LoadOptions(string settingsFileName,
										 IThemesManager themesManager,
										 Options programSettings = null);

		/// <summary>
		/// Save program options into persistence.
		/// See <seealso cref="LoadOptions"/> to load program options on program start.
		/// </summary>
		/// <param name="sessionDataFileName"></param>
		/// <param name="model"></param>
		/// <returns></returns>
		bool SaveSessionData(string sessionDataFileName, Profile model);

		/// <summary>
		/// Save program options into persistence.
		/// See <seealso cref="SaveOptions"/> to save program options on program end.
		/// </summary>
		/// <param name="sessionDataFileName"></param>
		/// <returns></returns>
		void LoadSessionData(string sessionDataFileName);

		/// <summary>
		/// Determine whether program options are valid and corrext
		/// settings if they appear to be invalid on current system
		/// </summary>
		void CheckSettingsOnLoad(double SystemParameters_VirtualScreenLeft,
														 double SystemParameters_VirtualScreenTop);
		#endregion methods
	}
}
