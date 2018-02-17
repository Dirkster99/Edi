namespace Edi.SettingsView.Config.ViewModels
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using Core.ViewModels.Base;
	using ICSharpCode.AvalonEdit;
	using ICSharpCode.AvalonEdit.Edi.BlockSurround;
	using Settings.ProgramSettings;

    public class ConfigViewModel : DialogViewModelBase
	{
		#region fields
		private bool mWordWrapText;
		private bool mReloadOpenFilesOnAppStart;
		private bool mRunSingleInstance;

		private LanguageCollection mLanguageSelected;

		private bool mTextToHTML_ShowLineNumbers = true;
		private bool mTextToHTML_TextToHTML_AlternateLineBackground = true;

		private bool mHighlightOnFileNew = true;
		private string mFileNewDefaultFileName = Util.Local.Strings.STR_FILE_DEFAULTNAME;
		private string mFileNewDefaultFileExtension = ".txt";
		#endregion fields

		#region constructor
		/// <summary>
		/// Class constructor
		/// </summary>
		public ConfigViewModel()
			: base()
		{
			// Setup default values here - real values are loaded in a specific method of this class (!)
			mWordWrapText = false;
			mReloadOpenFilesOnAppStart = false;
			mRunSingleInstance = true;

			WordWrapText = false;

			// Get default list of units from settings manager
			var unitList = new ObservableCollection<UnitComboLib.Models.ListItem>(Options.GenerateScreenUnitList());
			SizeUnitLabel =
                UnitComboLib.UnitViewModeService.CreateInstance(
                    unitList,
                    new ScreenConverter(),
                    (int)ZoomUnit.Percentage, 100);

			EditorTextOptions = new TextEditorOptions();

			// Initialize localization settings
			Languages = new List<LanguageCollection>(Options.GetSupportedLanguages());

			// Set default language to make sure app neutral is selected and available for sure
			// (this is a fallback if all else fails)
			try
			{
				LanguageSelected = Languages.FirstOrDefault(lang => lang.BCP47 == Options.DefaultLocal);
			}
			catch
			{
			}
		}
		#endregion constructor

		#region properties
		/// <summary>
		/// Get/set whether WordWarp should be applied in editor (by default) or not.
		/// </summary>
		public bool WordWrapText
		{
			get
			{
				return mWordWrapText;
			}

			set
			{
				if (mWordWrapText != value)
				{
					mWordWrapText = value;
					RaisePropertyChanged(() => WordWrapText);
				}
			}
		}

		/// <summary>
		/// Expose AvalonEdit Text Editing options for editing in program settings view.
		/// </summary>
		public TextEditorOptions EditorTextOptions { get; set; }

		#region Application Behaviour
		/// <summary>
		/// Get/set whether application re-loads files open in last sesssion or not
		/// </summary>
		public bool ReloadOpenFilesOnAppStart
		{
			get
			{
				return mReloadOpenFilesOnAppStart;
			}

			set
			{
				if (mReloadOpenFilesOnAppStart != value)
				{
					mReloadOpenFilesOnAppStart = value;
					RaisePropertyChanged(() => ReloadOpenFilesOnAppStart);
				}
			}
		}

		/// <summary>
		/// Get/set whether application can be started more than once.
		/// </summary>
		public bool RunSingleInstance
		{
			get
			{
				return mRunSingleInstance;
			}

			set
			{
				if (mRunSingleInstance != value)
				{
					mRunSingleInstance = value;
					RaisePropertyChanged(() => RunSingleInstance);
				}
			}
		}
		#endregion Application Behaviour

		#region ScaleView
		/// <summary>
		/// Scale view of text in percentage of font size
		/// </summary>
		public IUnitViewModel SizeUnitLabel { get; set; }

		/// <summary>
		/// Get/set unit of document zoom unit.
		/// </summary>
		public ZoomUnit DocumentZoomUnit
		{
			get
			{
				if (SizeUnitLabel.SelectedItem.Key == Itemkey.ScreenFontPoints)
					return ZoomUnit.Points;

				return ZoomUnit.Percentage;
			}

			set
			{
				if (ConvertZoomUnit(value) != SizeUnitLabel.SelectedItem.Key)
				{
					if (value == ZoomUnit.Points)
						SizeUnitLabel.SetSelectedItemCommand.Execute(Itemkey.ScreenFontPoints);
					else
						SizeUnitLabel.SetSelectedItemCommand.Execute(Itemkey.ScreenPercent);

					RaisePropertyChanged(() => DocumentZoomUnit);
				}
			}
		}

		/// <summary>
		/// Get the title string of the view - to be displayed in the associated view
		/// (e.g. as dialog title)
		/// </summary>
		public string WindowTitle
		{
			get
			{
				return Util.Local.Strings.STR_ProgramSettings_Caption;
			}
		}
		#endregion ScaleView

		#region Language Localization Support
		/// <summary>
		/// Get list of GUI languages supported in this application.
		/// </summary>
		public List<LanguageCollection> Languages { get; private set; }

		/// <summary>
		/// Get/set language of message box buttons for display in localized form.
		/// </summary>
		public LanguageCollection LanguageSelected
		{
			get
			{
				return mLanguageSelected;
			}

			set
			{
				if (mLanguageSelected != value)
				{
					mLanguageSelected = value;
					RaisePropertyChanged(() => LanguageSelected);
				}
			}
		}
		#endregion Language Localization Support

		#region Text To HTML Export
		/// <summary>
		/// Get/set whether Text to HTML should contain line numbers or not.
		/// </summary>
		public bool TextToHTML_ShowLineNumbers
		{
			get
			{
				return mTextToHTML_ShowLineNumbers;
			}

			set
			{
				if (mTextToHTML_ShowLineNumbers != value)
				{
					mTextToHTML_ShowLineNumbers = value;
					RaisePropertyChanged(() => TextToHTML_ShowLineNumbers);
				}
			}
		}

		/// <summary>
		/// Get/set whether Text to HTML should contain an alternating background.
		/// </summary>
		public bool TextToHTML_AlternateLineBackground
		{
			get
			{
				return mTextToHTML_TextToHTML_AlternateLineBackground;
			}

			set
			{
				if (mTextToHTML_TextToHTML_AlternateLineBackground != value)
				{
					mTextToHTML_TextToHTML_AlternateLineBackground = value;
					RaisePropertyChanged(() => TextToHTML_AlternateLineBackground);
				}
			}
		}
		#endregion Text To HTML Export

		#region NewFileDefaults
		/// <summary>
		/// Determine whether a file created with File>New should be highlighted or not.
		/// </summary>
		public bool HighlightOnFileNew
		{
			get
			{
				return mHighlightOnFileNew;
			}

			set
			{
				if (mHighlightOnFileNew != value)
				{
					mHighlightOnFileNew = value;
					RaisePropertyChanged(() => HighlightOnFileNew);
				}
			}
		}

		/// <summary>
		/// Get/set default name of file that is created via File>New.
		/// </summary>
		public string FileNewDefaultFileName
		{
			get
			{
				return mFileNewDefaultFileName;
			}

			set
			{
				if (mFileNewDefaultFileName != value)
				{
					mFileNewDefaultFileName = value;
					RaisePropertyChanged(() => FileNewDefaultFileName);
				}
			}
		}

		/// <summary>
		/// Get/set default string of file extension (including '.' character)
		/// that is created via File>New.
		/// </summary>
		public string FileNewDefaultFileExtension
		{
			get
			{
				return mFileNewDefaultFileExtension;
			}

			set
			{
				if (mFileNewDefaultFileExtension != value)
				{
					mFileNewDefaultFileExtension = value;
					RaisePropertyChanged(() => FileNewDefaultFileExtension);
				}
			}
		}
		#endregion NewFileDefaults
		#endregion properties

		#region methods
		/// <summary>
		/// Get default block definitions to add / remove surrounding selection blocks in text.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<BlockDefinition> GetDefaultBlockDefinitions()
		{
			List<BlockDefinition> ret = new List<BlockDefinition>();

			ret.Add(new BlockDefinition("/**", "**/", BlockDefinition.BlockAt.StartAndEnd,
																	string.Empty,   // Default BlockSurrond
																	System.Windows.Input.Key.D1,
																	System.Windows.Input.ModifierKeys.Control
																));

			ret.Add(new BlockDefinition("////", string.Empty, BlockDefinition.BlockAt.Start,
																	string.Empty,   // Default BlockSurrond
																	System.Windows.Input.Key.D2,
																	System.Windows.Input.ModifierKeys.Control
																));

			ret.Add(new BlockDefinition(string.Empty, "<<<<", BlockDefinition.BlockAt.End,
																	string.Empty,   // Default BlockSurrond
																	System.Windows.Input.Key.D3,
																	System.Windows.Input.ModifierKeys.Control
																));

			ret.Add(new BlockDefinition("/**", "**/", BlockDefinition.BlockAt.StartAndEnd,
																	"*.txt",
																	System.Windows.Input.Key.D1,
																	System.Windows.Input.ModifierKeys.Control
																));

			ret.Add(new BlockDefinition("/**", "**/", BlockDefinition.BlockAt.StartAndEnd,
																	"*.cs",
																	System.Windows.Input.Key.D1,
																	System.Windows.Input.ModifierKeys.Control
																));

			ret.Add(new BlockDefinition("<!--", "-->", BlockDefinition.BlockAt.StartAndEnd,
																	"*.xml;*.html;*.htm",
																	System.Windows.Input.Key.D1,
																	System.Windows.Input.ModifierKeys.Control
																));

			return ret;
		}

		/// <summary>
		/// Reset the view model to those options that are going to be presented for editing.
		/// </summary>
		/// <param name="settingData"></param>
		public void LoadOptionsFromModel(Options settingData)
		{
			ReloadOpenFilesOnAppStart = settingData.ReloadOpenFilesOnAppStart;
			RunSingleInstance = settingData.RunSingleInstance;

			WordWrapText = settingData.WordWarpText;

			EditorTextOptions = new TextEditorOptions(settingData.EditorTextOptions);
			SizeUnitLabel = UnitComboLib.UnitViewModeService.CreateInstance(
                new ObservableCollection<UnitComboLib.Models.ListItem>(
                    Options.GenerateScreenUnitList()),
						new ScreenConverter(),
					(int)settingData.DocumentZoomUnit, settingData.DocumentZoomView);

			try
			{
				LanguageSelected = Languages.FirstOrDefault(lang => lang.BCP47 == settingData.LanguageSelected);
			}
			catch
			{
			}

			TextToHTML_ShowLineNumbers = settingData.TextToHTML_ShowLineNumbers;
			TextToHTML_AlternateLineBackground = settingData.TextToHTML_AlternateLineBackground;

			HighlightOnFileNew = settingData.HighlightOnFileNew;
			FileNewDefaultFileName = settingData.FileNewDefaultFileName;
			FileNewDefaultFileExtension = settingData.FileNewDefaultFileExtension;
		}

		/// <summary>
		/// Save changed settings back to model for further
		/// application and persistence in file system.
		/// </summary>
		/// <param name="settingData"></param>
		public void SaveOptionsToModel(Options settingData)
		{
			settingData.ReloadOpenFilesOnAppStart = ReloadOpenFilesOnAppStart;
			settingData.RunSingleInstance = RunSingleInstance;

			settingData.WordWarpText = WordWrapText;

			settingData.EditorTextOptions = new TextEditorOptions(EditorTextOptions);
			if (SizeUnitLabel.SelectedItem.Key == UnitComboLib.Models.Unit.Itemkey.ScreenFontPoints)
				settingData.DocumentZoomUnit = ZoomUnit.Points;
			else
				settingData.DocumentZoomUnit = ZoomUnit.Percentage;

			settingData.DocumentZoomView = (int)SizeUnitLabel.Value;

			settingData.LanguageSelected = LanguageSelected.BCP47;

			settingData.TextToHTML_ShowLineNumbers = TextToHTML_ShowLineNumbers;
			settingData.TextToHTML_AlternateLineBackground = TextToHTML_AlternateLineBackground;

			settingData.HighlightOnFileNew = HighlightOnFileNew;
			settingData.FileNewDefaultFileName = FileNewDefaultFileName;
			settingData.FileNewDefaultFileExtension = FileNewDefaultFileExtension;

			settingData.IsDirty = true;
		}

		/// <summary>
		/// Convert between local zoom unit enumeration and remote zoom unit enumeration.
		/// </summary>
		/// <param name="unit"></param>
		/// <returns></returns>
		private UnitComboLib.Models.Unit.Itemkey ConvertZoomUnit(ZoomUnit unit)
		{
			switch (unit)
			{
				case ZoomUnit.Percentage:
					return UnitComboLib.Models.Unit.Itemkey.ScreenPercent;

				case ZoomUnit.Points:
					return UnitComboLib.Models.Unit.Itemkey.ScreenFontPoints;

				default:
					throw new System.NotImplementedException(unit.ToString());
			}
		}
		#endregion methods
	}
}
