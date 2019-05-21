namespace Edi.Themes.Interfaces
{
	/// <summary>
	/// This interface specifies a property which is required to determine
	/// whether a theme is selected ot not (the answer is determined per
	/// single property in parent but the theme itself exposes the corresponding 
	/// property to answer the question for itself - could also be solved
	/// without link but with multivalue converter...
	/// </summary>
	public interface IParentSelectedTheme
	{
		string SelectedThemeName { get; }
	}
}
