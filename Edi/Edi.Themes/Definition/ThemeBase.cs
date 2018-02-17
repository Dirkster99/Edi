namespace Edi.Themes.Definition
{
	using System.Collections.Generic;
	using System.Reflection;
	using ICSharpCode.AvalonEdit.Highlighting.Themes;
	using Interfaces;

	public class ThemeBase
	{
		#region fields
		private HighlightingThemes mStyles;
		private string mPathLocation;
		private IParentSelectedTheme mParent = null;
		#endregion fields

		#region constructor
		/// <summary>
		/// Parameterized constructor
		/// </summary>
		internal ThemeBase(IParentSelectedTheme parent,
											 List<string> resources,
											 string wpfThemeName,
											 string editorThemeName,
											 string editorThemePathLocation,
											 string editorThemeFileName)
			: base()
		{
			mParent = parent;
			Resources = new List<string>(resources);
			WPFThemeName = wpfThemeName;

			EditorThemeName = editorThemeName;
			mPathLocation = editorThemePathLocation;
			EditorThemeFileName = editorThemeFileName;

			mStyles = null;
		}

		/// <summary>
		/// Hidden constructor
		/// </summary>
		protected ThemeBase()
			: base()
		{
			mPathLocation = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			mStyles = null;
		}
		#endregion constructor

		#region properties
		/// <summary>
		/// Get a list of Uri formatted resource strings that point to all relevant resources.
		/// </summary>
		public List<string> Resources { get; private set; }

		/// <summary>
		/// WPF Application skin theme (e.g. Metro)
		/// </summary>
		public string WPFThemeName { get; private set; }

		/// <summary>
		/// Get/set the name of the highlighting theme
		/// (eg.: DeepBlack, Bright Standard, True Blue)
		/// </summary>
		public string EditorThemeName { get; private set; }

		/// <summary>
		/// Get/set the location of the current highlighting theme (eg.: C:\DeepBlack.xml)
		/// </summary>
		public string EditorThemeFileName { get; private set; }

		/// <summary>
		/// Get the human read-able name of this WPF/Editor theme.
		/// </summary>
		public string HlThemeName
		{
			get
			{
				return string.Format("{0}{1}", WPFThemeName,
																			 EditorThemeName == null ? string.Empty : " (" + EditorThemeName + ")");
			}
		}

		/// <summary>
		/// This property exposes a collection of highlighting themes for different file types
		/// (color and style definitions for keywords in SQL, C# and so forth)
		/// </summary>
		public HighlightingThemes HighlightingStyles
		{
			get
			{
				// Lazy load this content when it is needed for the first time ever
				if (mStyles == null)
					mStyles = ICSharpCode.AvalonEdit.Highlighting.Themes.XML.Read.ReadXML(mPathLocation, EditorThemeFileName);

				return mStyles;
			}
		}

		/// <summary>
		/// Determine whether this theme is currently selected or not.
		/// </summary>
		public bool IsSelected
		{
			get
			{
				if (mParent != null)
				{
					if (mParent.SelectedThemeName != null)
					{
						return mParent.SelectedThemeName.Equals(HlThemeName);
					}
				}
				return false;
			}
		}
		#endregion properties
	}
}
