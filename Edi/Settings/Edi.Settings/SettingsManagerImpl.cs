namespace Edi.Settings
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;
    using Edi.Core.Models;
    using Edi.Settings.Interfaces;
    using Edi.Settings.ProgramSettings;
    using Edi.Settings.UserProfile;
    using Edi.Themes.Interfaces;

    /// <summary>
    /// This class keeps track of program options and user profile (session) data.
    /// Both data items can be added and are loaded on application start to restore
    /// the program state of the last user session or to implement the default
    /// application state when starting the application for the very first time.
    /// </summary>
    internal class SettingsManagerImpl : ISettingsManager
	{
		#region fields
		protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private IOptions _SettingData = null;
		private IProfile _SessionData = null;

		private readonly IThemesManager _ThemesManager = null;
		#endregion fields

		#region constructor
		/// <summary>
		/// Class constructor
		/// </summary>
		public SettingsManagerImpl(IThemesManager themesManager)
		{
			_ThemesManager = themesManager;

			_SettingData = new Options(_ThemesManager);
			_SessionData = new Profile();
		}
		#endregion constructor

		#region properties
		/// <summary>
		/// Gets the program options for the complete application.
		/// Program options are user specific settings that are rarelly
		/// changed and can be customized per user.
		/// </summary>
		IOptions ISettingsManager.SettingData
		{
			get
			{
				if (_SettingData == null)
					_SettingData = new Options(_ThemesManager);

				return _SettingData;
			}
		}

		/// <summary>
		/// Gets the user session data that is almost always bound to change
		/// everytime the user starts up the application, does something with it,
		/// and shuts it down. Typically, window position, recent files list,
		/// and such things are stored in here.
		/// </summary>
		IProfile ISettingsManager.SessionData
		{
			get
			{
				if (_SessionData == null)
					_SessionData = new Profile();

				return _SessionData;
			}
		}

        /// <summary>
        /// Gets a string that denotes an internet link to
        /// a web site where users can enter their issues.
        /// </summary>
        public string LayoutFileName { get { return "Layout.config"; } }


        /// <summary>
        /// Get a path to the directory where the application
        /// can persist/load user data on session exit and re-start.
        /// </summary>
        public string AppDir { get { return AppHelpers.DirAppData; } }
		#endregion properties

		#region methods
		/// <summary>
		/// Determine whether program options are valid and corrext
		/// settings if they appear to be invalid on current system
		/// </summary>
		void ISettingsManager.CheckSettingsOnLoad(double SystemParameters_VirtualScreenLeft,
									             double SystemParameters_VirtualScreenTop)
		{
			//// Dirkster: Not sure whether this is working correctly yet...
			//// this.SessionData.CheckSettingsOnLoad(SystemParameters_VirtualScreenLeft,
			////                                      SystemParameters_VirtualScreenTop);
		}

        #region Load Save ProgramOptions
        /// <summary>
        /// Async method to load program options from persistence (without blocking UI thread).
        /// See <seealso cref="SaveOptions"/> to load program options on program start.
        /// </summary>
        /// <param name="settingsFileName"></param>
        /// <param name="themesManager"></param>
        /// <returns></returns>
        Task<IOptions> ISettingsManager.LoadOptionsAsync(string settingsFileName,
                                                         IThemesManager themesManager,
                                                         IOptions programSettings = null)
        {
            return Task.Run(() => { return LoadOptions(settingsFileName, themesManager, programSettings); });
        }

        /// <summary>
        /// Save program options into persistence.
        /// See <seealso cref="LoadOptionsAsync"/> to load program options on program start.
        /// </summary>
        /// <param name="settingsFileName"></param>
        /// <param name="optionsModel"></param>
        /// <returns></returns>
        bool ISettingsManager.SaveOptions(string settingsFileName, IOptions optionsModel)
		{
			try
			{
                XmlWriterSettings xws = new XmlWriterSettings()
                {
                    NewLineOnAttributes = true,
                    Indent = true,
                    IndentChars = "  ",
                    Encoding = System.Text.Encoding.UTF8
                };

                // Create a new file stream to write the serialized object to a file
                using (XmlWriter xw = XmlWriter.Create(settingsFileName, xws))
				{
					// Create a new XmlSerializer instance with the type of the test class
					XmlSerializer serializerObj = new XmlSerializer(typeof(Options));

					serializerObj.Serialize(xw, optionsModel);

					optionsModel.SetDirtyFlag(false);
				}

				return true;
			}
			catch
			{
				throw;
			}
		}
		#endregion Load Save ProgramOptions

		#region Load Save UserSessionData
		/// <summary>
		/// Save program options into persistence.
		/// See <seealso cref="SaveOptions"/> to save program options on program end.
		/// </summary>
		/// <param name="sessionDataFileName"></param>
		/// <returns></returns>
		void ISettingsManager.LoadSessionData(string sessionDataFileName)
		{
			Profile profileDataModel = null;

			try
			{
				if (System.IO.File.Exists(sessionDataFileName))
				{
					// Create a new file stream for reading the XML file
					using (FileStream readFileStream = new System.IO.FileStream(sessionDataFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						try
						{
							// Create a new XmlSerializer instance with the type of the test class
							XmlSerializer serializerObj = new XmlSerializer(typeof(Profile));

							// Load the object saved above by using the Deserialize function
							profileDataModel = (Profile)serializerObj.Deserialize(readFileStream);
						}
						catch (Exception e)
						{
							logger.Error(e);
						}

						// Cleanup
						readFileStream.Close();
					}
				}

				_SessionData = profileDataModel;
			}
			catch (Exception exp)
			{
				logger.Error(exp);
			}
			finally
			{
				if (profileDataModel == null)
					profileDataModel = new Profile();  // Just get the defaults if serilization wasn't working here...
			}
		}

        /// <summary>
        /// Save program options into persistence.
        /// See <seealso cref="LoadOptionsAsync"/> to load program options on program start.
        /// </summary>
        /// <param name="sessionDataFileName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        bool ISettingsManager.SaveSessionData(string sessionDataFileName, IProfile model)
		{
			try
			{
                XmlWriterSettings xws = new XmlWriterSettings()
                {
                    NewLineOnAttributes = true,
                    Indent = true,
                    IndentChars = "  ",
                    Encoding = System.Text.Encoding.UTF8
                };

                // Create a new file stream to write the serialized object to a file
                using (XmlWriter xw = XmlWriter.Create(sessionDataFileName, xws))
				{
					// Create a new XmlSerializer instance with the type of the test class
					XmlSerializer serializerObj = new XmlSerializer(typeof(Profile));

					serializerObj.Serialize(xw, model);

					xw.Close(); // Cleanup

					return true;
				}
			}
			catch
			{
				throw;
			}
		}
        #endregion Load Save UserSessionData

        #region Load Options methods
        /// <summary>
        /// Load program options from persistence.
        /// See <seealso cref="SaveOptions"/> to save program options on program end.
        /// </summary>
        /// <param name="settingsFileName"></param>
        /// <param name="themesManager"></param>
        /// <returns></returns>
        private IOptions LoadOptions(string settingsFileName,
                                     IThemesManager themesManager,
                                     IOptions programSettings = null)
        {
            if (programSettings != null)
                _SettingData = programSettings;
            else                                     // Get a fresh copy from persistence
                _SettingData = LoadOptionsImpl(settingsFileName, themesManager);

            return _SettingData;
        }

        /// <summary>
        /// Load program options from persistence.
        /// See <seealso cref="SaveOptions"/> to save program options on program end.
        /// </summary>
        /// <param name="settingsFileName"></param>
        /// <param name="themesManager"></param>
        /// <returns></returns>
        private IOptions LoadOptionsImpl(string settingsFileName
                                       , IThemesManager themesManager)
        {
            Options loadedModel = null;

            try
            {
                if (System.IO.File.Exists(settingsFileName))
                {
                    // Create a new file stream for reading the XML file
                    using (FileStream readFileStream = new System.IO.FileStream(settingsFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        try
                        {
                            // Create a new XmlSerializer instance with the type of the test class
                            XmlSerializer serializerObj = new XmlSerializer(typeof(Options));

                            // Load the object saved above by using the Deserialize function
                            loadedModel = (Options)serializerObj.Deserialize(readFileStream);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e);
                        }

                        // Cleanup
                        readFileStream.Close();
                    }
                }
            }
            catch (Exception exp)
            {
                logger.Error(exp);
            }
            finally
            {
                // Just get the defaults if serilization wasn't working here...
                if (loadedModel == null)
                    loadedModel = new Options(themesManager);
            }

            // Data has just been loaded from persistence (or default) so its not dirty for sure
            loadedModel.SetDirtyFlag(false);

            return loadedModel;
        }
        #endregion Load Options methods
        #endregion methods
    }
}
