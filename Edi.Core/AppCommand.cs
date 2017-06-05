namespace Edi.Core
{
	using System.Windows.Input;
	using Util.Local;

	public class AppCommand
	{
		#region CommandFramework Fields
		private static RoutedUICommand exit;
		private static RoutedUICommand about;
		private static RoutedUICommand programSettings;
		private static RoutedUICommand showToolWindow;

		private static RoutedUICommand loadFile;
		private static RoutedUICommand saveAll;
		private static RoutedUICommand exportUMLToImage;
		private static RoutedUICommand exportTextToHTML;

		private static RoutedUICommand pinUnpin;
		private static RoutedUICommand addMruEntry;
		private static RoutedUICommand removeMruEntry;
		private static RoutedUICommand closeFile;
		private static RoutedUICommand viewTheme;

		private static RoutedUICommand browseURL;
		private static RoutedUICommand showStartPage;

		private static RoutedUICommand toggleOptimizeWorkspace;

		#region Text Edit Commands
		private static RoutedUICommand gotoLine;
		private static RoutedUICommand findText;
		private static RoutedUICommand findNextText;
		private static RoutedUICommand findPreviousText;
		private static RoutedUICommand replaceText;

		private static RoutedUICommand disableHighlighting;

		#endregion Text Edit Commands
		#endregion CommandFramework Fields

		#region Static Constructor (Constructs static application commands)
		/// <summary>
		/// Define custom commands and their key gestures
		/// </summary>
		static AppCommand()
		{
			InputGestureCollection inputs = null;

			// Initialize the exit command
			inputs = new InputGestureCollection();
			inputs.Add(new KeyGesture(Key.F4, ModifierKeys.Alt, "Alt+F4"));
			AppCommand.exit = new RoutedUICommand(Strings.CMD_App_Exit_Describtion, "Exit", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			AppCommand.about = new RoutedUICommand(Strings.CMD_APP_About_Description, "About", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			AppCommand.programSettings = new RoutedUICommand("Edit or Review your program settings", "ProgramSettings", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			AppCommand.showToolWindow = new RoutedUICommand("Hide or display toolwindow", "ShowToolWindow", typeof(AppCommand), inputs);

			// Execute file open command (without user interaction)
			inputs = new InputGestureCollection();
			AppCommand.loadFile = new RoutedUICommand(Strings.CMD_APP_Open_Description, "LoadFile", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			AppCommand.saveAll = new RoutedUICommand(Strings.CMD_APP_SaveAll_Description, "SaveAll", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			AppCommand.exportUMLToImage = new RoutedUICommand(Strings.CMD_APP_ExportUMLToImage_Description, "ExportUMLToImage", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			AppCommand.exportTextToHTML = new RoutedUICommand(Strings.CMD_APP_ExportTextToHTML_Description, "ExportTextToHTML", typeof(AppCommand), inputs);

			// Initialize pin command (to set or unset a pin in MRU and re-sort list accordingly)
			inputs = new InputGestureCollection();
			AppCommand.pinUnpin = new RoutedUICommand(Strings.CMD_MRU_Pin_Description, "Pin", typeof(AppCommand), inputs);

			// Execute add recent files list etnry pin command (to add another MRU entry into the list)
			inputs = new InputGestureCollection();
			AppCommand.addMruEntry = new RoutedUICommand(Strings.CMD_MRU_AddEntry_Description, "AddEntry", typeof(AppCommand), inputs);

			// Execute remove pin command (remove a pin from a recent files list entry)
			inputs = new InputGestureCollection();
			AppCommand.removeMruEntry = new RoutedUICommand(Strings.CMD_MRU_RemoveEntry_Description, "RemoveEntry", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			inputs.Add(new KeyGesture(Key.F4, ModifierKeys.Control, "Ctrl+F4"));
			inputs.Add(new KeyGesture(Key.W, ModifierKeys.Control, "Ctrl+W"));
			AppCommand.closeFile = new RoutedUICommand(Strings.CMD_APP_CloseDoc_Description, "Close", typeof(AppCommand), inputs);

			// Initialize the viewTheme command
			inputs = new InputGestureCollection();
			AppCommand.viewTheme = new RoutedUICommand(Strings.CMD_APP_ViewTheme_Description, "ViewTheme", typeof(AppCommand), inputs);

			// Execute browse Internt URL (without user interaction)
			inputs = new InputGestureCollection();
			AppCommand.browseURL = new RoutedUICommand(Strings.CMD_APP_OpenURL_Description, "OpenURL", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			AppCommand.showStartPage = new RoutedUICommand(Strings.CMD_APP_ShowStartPage_Description, "StartPage", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			AppCommand.toggleOptimizeWorkspace = new RoutedUICommand(Strings.CMD_APP_ToggleOptimizeWorkspace_Description, "ToggleOptimizeWorkspace", typeof(AppCommand), inputs);

			#region Text Edit Commands
			inputs = new InputGestureCollection();
			AppCommand.disableHighlighting = new RoutedUICommand(Strings.CMD_TXT_DisableHighlighting_Description, "DisableHighlighting", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();                                     // Goto Line n in the current document
			inputs.Add(new KeyGesture(Key.G, ModifierKeys.Control, "Ctrl+G"));
			AppCommand.gotoLine = new RoutedUICommand(Strings.CMD_TXT_GotoLine_Description, "GotoLine", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			inputs.Add(new KeyGesture(Key.F, ModifierKeys.Control, "Ctrl+F"));
			AppCommand.findText = new RoutedUICommand(Strings.CMD_TXT_FindNext_Description, "FindText", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			inputs.Add(new KeyGesture(Key.F3, ModifierKeys.None, "F3"));
			AppCommand.findNextText = new RoutedUICommand(Strings.CMD_TXT_FindNextText_Description, "FindNextText", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			inputs.Add(new KeyGesture(Key.F3, ModifierKeys.Shift, "Shift+F3"));
			AppCommand.findPreviousText = new RoutedUICommand(Strings.CMD_TXT_FindPreviousText_Description, "FindPreviousText", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			inputs.Add(new KeyGesture(Key.H, ModifierKeys.Control, "Ctrl+H"));
			AppCommand.replaceText = new RoutedUICommand(Strings.CMD_TXT_FindReplaceText_Description, "FindReplaceText", typeof(AppCommand), inputs);
			#endregion Text Edit Commands
		}
		#endregion Static Constructor

		#region CommandFramwork Properties (Exposes Commands to which the UI can bind to)
		/// <summary>
		/// Static property of the correspondong <seealso cref="System.Windows.Input.RoutedUICommand"/>
		/// </summary>
		public static RoutedUICommand Exit
		{
			get { return AppCommand.exit; }
		}

		public static RoutedUICommand About
		{
			get { return AppCommand.about; }
		}

		public static RoutedUICommand ProgramSettings
		{
			get
			{
				return AppCommand.programSettings;
			}
		}

		public static RoutedUICommand ShowToolWindow
		{
			get
			{
				return AppCommand.showToolWindow;
			}
		}

		/// <summary>
		/// Execute file open command (without user interaction)
		/// </summary>
		public static RoutedUICommand LoadFile
		{
			get { return AppCommand.loadFile; }
		}

		/// <summary>
		/// Execute a command to save all edited files and current program settings
		/// </summary>
		public static RoutedUICommand SaveAll
		{
			get { return AppCommand.saveAll; }
		}

		/// <summary>
		/// Execute a command to export the currently loaded UML Diagram (XML based data)
		/// into an image based data format (png, jpeg, wmf)
		/// </summary>
		public static RoutedUICommand ExportUMLToImage
		{
			get { return AppCommand.exportUMLToImage; }
		}

		/// <summary>
		/// Execute a command to export the currently loaded and highlighted text (XML, C# ...)
		/// into an HTML data format (*.htm, *.html ...)
		/// </summary>
		public static RoutedUICommand ExportTextToHTML
		{
			get { return AppCommand.exportTextToHTML; }
		}

		/// <summary>
		/// Execute pin/unpin command (to set or unset a pin in MRU and re-sort list accordingly)
		/// </summary>
		public static RoutedUICommand PinUnpin
		{
			get { return AppCommand.pinUnpin; }
		}

		/// <summary>
		/// Execute add recent files list etnry pin command (to add another MRU entry into the list)
		/// </summary>
		public static RoutedUICommand AddMruEntry
		{
			get { return AppCommand.addMruEntry; }
		}

		/// <summary>
		/// Execute remove pin command (remove a pin from a recent files list entry)
		/// </summary>
		public static RoutedUICommand RemoveMruEntry
		{
			get { return AppCommand.removeMruEntry; }
		}

		public static RoutedUICommand CloseFile
		{
			get { return AppCommand.closeFile; }
		}

		/// <summary>
		/// Static property of the correspondong <seealso cref="System.Windows.Input.RoutedUICommand"/>
		/// </summary>
		public static RoutedUICommand ViewTheme
		{
			get { return AppCommand.viewTheme; }
		}

		/// <summary>
		/// Browse to an Internet URL via default web browser configured in Windows
		/// </summary>
		public static RoutedUICommand BrowseURL
		{
			get { return AppCommand.browseURL; }
		}

		/// <summary>
		/// Static property of the correspondong <seealso cref="System.Windows.Input.RoutedUICommand"/>
		/// </summary>
		public static RoutedUICommand ShowStartPage
		{
			get { return AppCommand.showStartPage; }
		}

		/// <summary>
		/// Static property of the correspondong <seealso cref="System.Windows.Input.RoutedUICommand"/>
		/// </summary>
		public static RoutedUICommand ToggleOptimizeWorkspace
		{
			get { return AppCommand.toggleOptimizeWorkspace; }
		}

		#region Text Edit Commands
		/// <summary>
		/// Goto line n in a given document
		/// </summary>
		public static RoutedUICommand GotoLine
		{
			get { return AppCommand.gotoLine; }
		}

		/// <summary>
		/// Find text in a given document
		/// </summary>
		public static RoutedUICommand FindText
		{
			get { return AppCommand.findText; }
		}

		public static RoutedUICommand FindNextText
		{
			get { return AppCommand.findNextText; }
		}

		public static RoutedUICommand FindPreviousText
		{
			get { return AppCommand.findPreviousText; }
		}

		/// <summary>
		/// Find and replace text in a given document
		/// </summary>
		public static RoutedUICommand ReplaceText
		{
			get { return AppCommand.replaceText; }
		}

		public static RoutedUICommand DisableHighlighting
		{
			get { return AppCommand.disableHighlighting; }
		}
		#endregion Text Edit Commands
		#endregion CommandFramwork_Properties
	}
}
