namespace Edi.Dialogs.About
{
	using System.Reflection;
	using Edi.Core.ViewModels.Base;
	using System.Collections.Generic;

	/// <summary>
	/// Organize the viewmodel for an about program information presentation
	/// (e.g. About dialog)
	/// </summary>
	public class AboutViewModel : DialogViewModelBase
	{
		#region constructor
		/// <summary>
		/// Class constructor
		/// </summary>
		public AboutViewModel()
			: base()
		{
		}
		#endregion constructor

		#region properties
		/// <summary>
		/// Get the title string of the view - to be displayed in the associated view
		/// (e.g. as dialog title)
		/// </summary>
		public string WindowTitle
		{
			get
			{
				return "About Edi";
			}
		}

		/// <summary>
		/// Get title of application for display in About view.
		/// </summary>
		public string AppTitle
		{
			get
			{
				return Assembly.GetEntryAssembly().GetName().Name;
			}
		}

		public string SubTitle
		{
			get
			{
				return Edi.Util.Local.Strings.STR_ABOUT_MSG;
			}
		}

		/// <summary>
		/// Gets the assembly copyright.
		/// </summary>
		/// <value>The assembly copyright.</value>
		public string AssemblyCopyright
		{
			get
			{
				// Get all Copyright attributes on this assembly
				object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);

				// If there aren't any Copyright attributes, return an empty string
				if (attributes.Length == 0)
					return string.Empty;

				// If there is a Copyright attribute, return its value
				return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
			}
		}

		/// <summary>
		/// Get URL of application for reference of source and display in About view.
		/// </summary>
		public string AppUrl
		{
			get
			{
				return "https://github.com/Dirkster99/Edi";
			}
		}

		/// <summary>
		/// Get URL string to display for reference of source and display in About view.
		/// </summary>
		public string AppUrlDisplayString
		{
			get
			{
				return "https://github.com/Dirkster99/Edi";
			}
		}

		/// <summary>
		/// Get application version for display in About view.
		/// </summary>
		public string AppVersion
		{
			get
			{
				return Assembly.GetEntryAssembly().GetName().Version.ToString();
			}
		}

		/// <summary>
		/// Get version of runtime for display in About view.
		/// </summary>
		public string RunTimeVersion
		{
			get
			{
				return Assembly.GetEntryAssembly().ImageRuntimeVersion;
			}
		}

		/// <summary>
		/// Get list of modules (referenced from EntryAssembly) and their version for display in About view.
		/// </summary>
		public SortedList<string, string> Modules
		{
			get
			{
				SortedList<string, string> l = new SortedList<string, string>();

				var name = Assembly.GetEntryAssembly().FullName;

				foreach (AssemblyName assembly in Assembly.GetEntryAssembly().GetReferencedAssemblies())
				{
					try
					{
                        string val = string.Empty;

                        if (l.TryGetValue(assembly.Name, out val) == false)
                            l.Add(assembly.Name, string.Format("{0}, {1}={2}", assembly.Name,
                                                        Edi.Util.Local.Strings.STR_ABOUT_Version,
                                                        assembly.Version));
                    }
					catch (System.Exception)
					{
					}
				}

				return l;
			}
		}
		#endregion properties
	}
}
