namespace Edi.Themes.Definition
{
	using System.Collections.Generic;
	using System.Reflection;
	using ICSharpCode.AvalonEdit.Highlighting.Themes;
	using Edi.Themes.Interfaces;

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
			this.mParent = parent;
			this.Resources = new List<string>(resources);
			this.WPFThemeName = wpfThemeName;

			this.EditorThemeName = editorThemeName;
			this.mPathLocation = editorThemePathLocation;
			this.EditorThemeFileName = editorThemeFileName;

			this.mStyles = null;
		}

		/// <summary>
		/// Hidden constructor
		/// </summary>
		protected ThemeBase()
			: base()
		{
			this.mPathLocation = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			this.mStyles = null;
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
				return string.Format("{0}{1}", this.WPFThemeName,
																			 this.EditorThemeName == null ? string.Empty : " (" + this.EditorThemeName + ")");
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
				if (this.mStyles == null)
					this.mStyles = ICSharpCode.AvalonEdit.Highlighting.Themes.XML.Read.ReadXML(this.mPathLocation, this.EditorThemeFileName);

				return this.mStyles;
			}
		}

		/// <summary>
		/// Determine whether this theme is currently selected or not.
		/// </summary>
		public bool IsSelected
		{
			get
			{
				if (this.mParent != null)
				{
					if (this.mParent.SelectedThemeName != null)
					{
						return this.mParent.SelectedThemeName.Equals(this.HlThemeName);
					}
				}
				return false;
			}
		}
		#endregion properties
	}
}
