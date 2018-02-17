namespace Edi.Settings.Interfaces
{
	using System.Xml.Serialization;
	using FileSystemModels.Models;
	using ICSharpCode.AvalonEdit;
	using ProgramSettings;

	public interface IOptions
	{
		#region properties
		/// <summary>
		/// Get/set whether WordWarp should be applied in editor (by default) or not.
		/// </summary>
		[XmlElement(ElementName = "WordWarpText")]
		bool WordWarpText { get; set; }

		/// <summary>
		/// Get/set options that are applicable to the texteditor which is based on AvalonEdit.
		/// </summary>
		TextEditorOptions EditorTextOptions { get; set; }

		/// <summary>
		/// Percentage Size of data to be viewed by default
		/// </summary>
		[XmlAttribute(AttributeName = "DocumentZoomView")]
		int DocumentZoomView { get; set; }

		/// <summary>
		/// Get/set standard size of display for text editor.
		/// </summary>
		[XmlAttribute(AttributeName = "DocumentZoomUnit")]
		ZoomUnit DocumentZoomUnit { get; set; }

		/// <summary>
		/// Get/set whether application re-loads files open in last sesssion or not
		/// </summary>
		[XmlAttribute(AttributeName = "ReloadOpenFilesFromLastSession")]
		bool ReloadOpenFilesOnAppStart { get; set; }

		/// <summary>
		/// Get/set whether application can be started more than once.
		/// </summary>
		[XmlElement(ElementName = "RunSingleInstance")]
		bool RunSingleInstance { get; set; }

		/// <summary>
		/// Get/set WPF theme configured for the complete Application
		/// </summary>
		[XmlElement("CurrentTheme")]
		string CurrentTheme { get; set; }

		[XmlElement("LanguageSelected")]
		string LanguageSelected { get; set; }

		#region HTML Export
		[XmlElement("TextToHTML_ShowLineNumbers")]
		bool TextToHTML_ShowLineNumbers { get; set; }

		[XmlElement("TextToHTML_AlternateLineBackground")]
		bool TextToHTML_AlternateLineBackground { get; set; }
		#endregion HTML Export

		#region New File Default options
		/// <summary>
		/// Determine whether a file created with File>New should be highlighted or not.
		/// </summary>
		[XmlElement("NewFile_HighlightOnFileNew")]
		bool HighlightOnFileNew { get; set; }

		/// <summary>
		/// Get/set default name of file that is created via File>New.
		/// </summary>
		[XmlElement("NewFile_DefaultFileName")]
		string FileNewDefaultFileName { get; set; }

		/// <summary>
		/// Get/set default string of file extension (including '.' character)
		/// that is created via File>New.
		/// </summary>
		[XmlElement("NewFile_DefaultFileExtension")]
		string FileNewDefaultFileExtension { get; set; }
		#endregion New File Default options

		[XmlElement(ElementName = "ExplorerSettings")]
		ExplorerSettingsModel ExplorerSettings { get; set; }

		/// <summary>
		/// Get/set whether the settings stored in this instance have been
		/// changed and need to be saved when program exits (at the latest).
		/// </summary>
		[XmlIgnore]
		bool IsDirty { get; set; }
		#endregion properties
	}
}
