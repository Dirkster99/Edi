namespace Edi.Core.Resources
{
	using System;
	using System.Linq;
	using System.Windows;

	/// <summary>
	/// Locate resources ín any assembly and return their reference.
	/// This class can, for example, be used to load a DataTemplate instance from an XAML reference.
	/// That is, the XAML is referenced as URI string (and the XAML itself can live in an extra assembly).
	/// The returned instance can be consumed in a 'code behind' context.
	/// </summary>
	public static class ResourceLocator
	{
		private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Gets the first matching resource of the type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="assemblyName">Name of the assembly.</param>
		/// <param name="resourceFilename">The resource filename.</param>
		/// <returns></returns>
		public static T GetResource<T>(string assemblyName, string resourceFilename) where T : class
		{
			return GetResource<T>(assemblyName, resourceFilename, string.Empty);
		}

		/// <summary>
		/// Gets the resource by name
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="assemblyName">Name of the assembly.</param>
		/// <param name="resourceFilename">The resource filename.</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public static T GetResource<T>(string assemblyName, string resourceFilename, string name) where T : class
		{
			try
			{
				if (string.IsNullOrEmpty(assemblyName) || string.IsNullOrEmpty(resourceFilename))
					return default(T);

				string uriPath = string.Format("/{0};component/{1}", assemblyName, resourceFilename);
				Uri uri = new Uri(uriPath, UriKind.Relative);
				ResourceDictionary resource = Application.LoadComponent(uri) as ResourceDictionary;

				if (resource == null)
					return default(T);

				if (!string.IsNullOrEmpty(name))
				{
					if (resource.Contains(name))
						return resource[name] as T;

					return default(T);
				}

				return resource.Values.OfType<T>().FirstOrDefault();
			}
			catch (Exception exp)
			{
				logger.Error(string.Format("Error Loading resource '{0}': {1}", "Exception:", exp.Message, exp));

				MsgBox.Msg.Show(exp, "Error loading internal resource.", MsgBox.MsgBoxButtons.OK);
			}

			return default(T);
		}
	}
}
