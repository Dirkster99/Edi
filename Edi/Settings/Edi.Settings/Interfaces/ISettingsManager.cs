namespace Edi.Settings.Interfaces
{
    using Edi.Settings.UserProfile;
    using Edi.Themes.Interfaces;
    using System.Threading.Tasks;

    public interface ISettingsManager
	{
        /// <summary>
        /// Gets the programm settings (preferences) for this manager instance.
        /// </summary>
		IOptions SettingData { get; }

		IProfile SessionData { get; }

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
		bool SaveOptions(string settingsFileName, IOptions optionsModel);

        /// <summary>
        /// Save program options into persistence.
        /// See <seealso cref="SaveOptions"/> to save program options on program end.
        /// </summary>
        /// <param name="settingsFileName"></param>
        /// <returns></returns>
        Task<IOptions> LoadOptionsAsync(string settingsFileName,
                                        IThemesManager themesManager,
                                        IOptions programSettings = null);

        /// <summary>
        /// Save program options into persistence.
        /// See <seealso cref="LoadOptions"/> to load program options on program start.
        /// </summary>
        /// <param name="sessionDataFileName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        bool SaveSessionData(string sessionDataFileName, IProfile model);

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
