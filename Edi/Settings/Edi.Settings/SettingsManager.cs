﻿namespace Edi.Settings
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Xml.Serialization;
	using Interfaces;
	using ProgramSettings;
	using UserProfile;
	using Themes.Interfaces;

	/// <summary>
	/// This class keeps track of program options and user profile (session) data.
	/// Both data items can be added and are loaded on application start to restore
	/// the program state of the last user session or to implement the default
	/// application state when starting the application for the very first time.
	/// </summary>
	public class SettingsManager : ISettingsManager
	{
		#region fields
		protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private Options mSettingData = null;
		private Profile mSessionData = null;

		private readonly IThemesManager mThemesManager = null;
		#endregion fields

		#region constructor
		/// <summary>
		/// Class constructor
		/// </summary>
		public SettingsManager(IThemesManager themesManager)
		{
			mThemesManager = themesManager;

			SettingData = new Options(mThemesManager);
			SessionData = new Profile();
		}
		#endregion constructor

		#region properties
		/// <summary>
		/// Gets the program options for the complete application.
		/// Program options are user specific settings that are rarelly
		/// changed and can be customized per user.
		/// </summary>
		public Options SettingData
		{
			get
			{
				if (mSettingData == null)
					mSettingData = new Options(mThemesManager);

				return mSettingData;
			}

			private set
			{
				if (mSettingData != value)
					mSettingData = value;
			}
		}

		/// <summary>
		/// Gets the user session data that is almost always bound to change
		/// everytime the user starts up the application, does something with it,
		/// and shuts it down. Typically, window position, recent files list,
		/// and such things are stored in here.
		/// </summary>
		public Profile SessionData
		{
			get
			{
				if (mSessionData == null)
					mSessionData = new Profile();

				return mSessionData;
			}

			private set
			{
				if (mSessionData != value)
					mSessionData = value;
			}
		}

		public string LayoutFileName { get; set; }

		public string AppDir { get; set; }
		#endregion properties

		#region methods
		/// <summary>
		/// Determine whether program options are valid and corrext
		/// settings if they appear to be invalid on current system
		/// </summary>
		public void CheckSettingsOnLoad(double SystemParameters_VirtualScreenLeft,
																		double SystemParameters_VirtualScreenTop)
		{
			//// Dirkster: Not sure whether this is working correctly yet...
			//// this.SessionData.CheckSettingsOnLoad(SystemParameters_VirtualScreenLeft,
			////                                      SystemParameters_VirtualScreenTop);
		}

		#region Load Save ProgramOptions
		/// <summary>
		/// Save program options into persistence.
		/// See <seealso cref="SaveOptions"/> to save program options on program end.
		/// </summary>
		/// <param name="settingsFileName"></param>
		/// <param name="themesManager"></param>
		/// <returns></returns>
		public void LoadOptions(string settingsFileName,
														IThemesManager themesManager,
														Options programSettings = null)
		{
			Options loadedModel = null;

			if (programSettings != null)
				loadedModel = programSettings;
			else                                     // Get a fresh copy from persistence
				loadedModel = LoadOptions(settingsFileName, themesManager);

			loadedModel.SetDirtyFlag(false);  // Data has just been loaded from persistence (or default) so its not dirty for sure
			SettingData = loadedModel;
		}

		/// <summary>
		/// Save program options into persistence.
		/// See <seealso cref="SaveOptions"/> to save program options on program end.
		/// </summary>
		/// <param name="settingsFileName"></param>
		/// <param name="themesManager"></param>
		/// <returns></returns>
		public static Options LoadOptions(string settingsFileName, IThemesManager themesManager)
		{
			Options loadedModel = null;

			try
			{
				if (File.Exists(settingsFileName))
				{
					// Create a new file stream for reading the XML file
					using (FileStream readFileStream = new FileStream(settingsFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
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

			loadedModel.SetDirtyFlag(false);  // Data has just been loaded from persistence (or default) so its not dirty for sure

			return loadedModel;
		}

		/// <summary>
		/// Save program options into persistence.
		/// See <seealso cref="LoadOptions"/> to load program options on program start.
		/// </summary>
		/// <param name="settingsFileName"></param>
		/// <param name="optionsModel"></param>
		/// <returns></returns>
		public bool SaveOptions(string settingsFileName, Options optionsModel)
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
		public void LoadSessionData(string sessionDataFileName)
		{
			Profile profileDataModel = null;

			try
			{
				if (File.Exists(sessionDataFileName))
				{
					// Create a new file stream for reading the XML file
					using (FileStream readFileStream = new FileStream(sessionDataFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
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

				SessionData = profileDataModel;
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
		/// See <seealso cref="LoadOptions"/> to load program options on program start.
		/// </summary>
		/// <param name="sessionDataFileName"></param>
		/// <param name="model"></param>
		/// <returns></returns>
		public bool SaveSessionData(string sessionDataFileName, Profile model)
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
		#endregion methods
	}
}
