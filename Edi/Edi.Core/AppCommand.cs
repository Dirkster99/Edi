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

        private static RoutedUICommand clearAllMruItemsCommand;
        private static RoutedUICommand removeItemsOlderThanThisCommand;
        private static RoutedUICommand movePinnedMruItemUPCommand;
        private static RoutedUICommand movePinnedMruItemDownCommand;
        private static RoutedUICommand pinItemCommand;
        private static RoutedUICommand unPinItemCommand;
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
			exit = new RoutedUICommand(Strings.CMD_App_Exit_Describtion, "Exit", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			about = new RoutedUICommand(Strings.CMD_APP_About_Description, "About", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			programSettings = new RoutedUICommand("Edit or Review your program settings", "ProgramSettings", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			showToolWindow = new RoutedUICommand("Hide or display toolwindow", "ShowToolWindow", typeof(AppCommand), inputs);

			// Execute file open command (without user interaction)
			inputs = new InputGestureCollection();
			loadFile = new RoutedUICommand(Strings.CMD_APP_Open_Description, "LoadFile", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			saveAll = new RoutedUICommand(Strings.CMD_APP_SaveAll_Description, "SaveAll", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			exportUMLToImage = new RoutedUICommand(Strings.CMD_APP_ExportUMLToImage_Description, "ExportUMLToImage", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			exportTextToHTML = new RoutedUICommand(Strings.CMD_APP_ExportTextToHTML_Description, "ExportTextToHTML", typeof(AppCommand), inputs);

            // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
            // MRU Commands
            inputs = new InputGestureCollection();
            clearAllMruItemsCommand = new RoutedUICommand(string.Empty, "Remove All MRU Entries", typeof(AppCommand), inputs);

            inputs = new InputGestureCollection();
            removeItemsOlderThanThisCommand = new RoutedUICommand(string.Empty, "Remove Entries older than this", typeof(AppCommand), inputs);

            inputs = new InputGestureCollection();
            movePinnedMruItemUPCommand = new RoutedUICommand(string.Empty, "Move Up", typeof(AppCommand), inputs);

            inputs = new InputGestureCollection();
            movePinnedMruItemDownCommand = new RoutedUICommand(string.Empty, "Move Down", typeof(AppCommand), inputs);

            inputs = new InputGestureCollection();
            pinItemCommand = new RoutedUICommand(string.Empty, "Pin to this list", typeof(AppCommand), inputs);

            inputs = new InputGestureCollection();
            unPinItemCommand = new RoutedUICommand(string.Empty, "Unpin from this list", typeof(AppCommand), inputs);

            // Initialize pin command (to set or unset a pin in MRU and re-sort list accordingly)
            inputs = new InputGestureCollection();
			pinUnpin = new RoutedUICommand(Strings.CMD_MRU_Pin_Description, "Pin", typeof(AppCommand), inputs);

			// Execute add recent files list etnry pin command (to add another MRU entry into the list)
			inputs = new InputGestureCollection();
			addMruEntry = new RoutedUICommand(Strings.CMD_MRU_AddEntry_Description, "AddEntry", typeof(AppCommand), inputs);

			// Execute remove pin command (remove a pin from a recent files list entry)
			inputs = new InputGestureCollection();
			removeMruEntry = new RoutedUICommand(Strings.CMD_MRU_RemoveEntry_Description, "RemoveEntry", typeof(AppCommand), inputs);

            // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
            // Window Close Command
			inputs = new InputGestureCollection();
			inputs.Add(new KeyGesture(Key.F4, ModifierKeys.Control, "Ctrl+F4"));
			inputs.Add(new KeyGesture(Key.W, ModifierKeys.Control, "Ctrl+W"));
			closeFile = new RoutedUICommand(Strings.CMD_APP_CloseDoc_Description, "Close", typeof(AppCommand), inputs);

			// Initialize the viewTheme command
			inputs = new InputGestureCollection();
			viewTheme = new RoutedUICommand(Strings.CMD_APP_ViewTheme_Description, "ViewTheme", typeof(AppCommand), inputs);

			// Execute browse Internt URL (without user interaction)
			inputs = new InputGestureCollection();
			browseURL = new RoutedUICommand(Strings.CMD_APP_OpenURL_Description, "OpenURL", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			showStartPage = new RoutedUICommand(Strings.CMD_APP_ShowStartPage_Description, "StartPage", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			toggleOptimizeWorkspace = new RoutedUICommand(Strings.CMD_APP_ToggleOptimizeWorkspace_Description, "ToggleOptimizeWorkspace", typeof(AppCommand), inputs);

			#region Text Edit Commands
			inputs = new InputGestureCollection();
			disableHighlighting = new RoutedUICommand(Strings.CMD_TXT_DisableHighlighting_Description, "DisableHighlighting", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();                                     // Goto Line n in the current document
			inputs.Add(new KeyGesture(Key.G, ModifierKeys.Control, "Ctrl+G"));
			gotoLine = new RoutedUICommand(Strings.CMD_TXT_GotoLine_Description, "GotoLine", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			inputs.Add(new KeyGesture(Key.F, ModifierKeys.Control, "Ctrl+F"));
			findText = new RoutedUICommand(Strings.CMD_TXT_FindNext_Description, "FindText", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			inputs.Add(new KeyGesture(Key.F3, ModifierKeys.None, "F3"));
			findNextText = new RoutedUICommand(Strings.CMD_TXT_FindNextText_Description, "FindNextText", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			inputs.Add(new KeyGesture(Key.F3, ModifierKeys.Shift, "Shift+F3"));
			findPreviousText = new RoutedUICommand(Strings.CMD_TXT_FindPreviousText_Description, "FindPreviousText", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			inputs.Add(new KeyGesture(Key.H, ModifierKeys.Control, "Ctrl+H"));
			replaceText = new RoutedUICommand(Strings.CMD_TXT_FindReplaceText_Description, "FindReplaceText", typeof(AppCommand), inputs);
			#endregion Text Edit Commands
		}
		#endregion Static Constructor

		#region CommandFramwork Properties (Exposes Commands to which the UI can bind to)
		/// <summary>
		/// Static property of the correspondong <seealso cref="System.Windows.Input.RoutedUICommand"/>
		/// </summary>
		public static RoutedUICommand Exit
		{
			get { return exit; }
		}

		public static RoutedUICommand About
		{
			get { return about; }
		}

		public static RoutedUICommand ProgramSettings
		{
			get
			{
				return programSettings;
			}
		}

		public static RoutedUICommand ShowToolWindow
		{
			get
			{
				return showToolWindow;
			}
		}

		/// <summary>
		/// Execute file open command (without user interaction)
		/// </summary>
		public static RoutedUICommand LoadFile
		{
			get { return loadFile; }
		}

		/// <summary>
		/// Execute a command to save all edited files and current program settings
		/// </summary>
		public static RoutedUICommand SaveAll
		{
			get { return saveAll; }
		}

		/// <summary>
		/// Execute a command to export the currently loaded UML Diagram (XML based data)
		/// into an image based data format (png, jpeg, wmf)
		/// </summary>
		public static RoutedUICommand ExportUMLToImage
		{
			get { return exportUMLToImage; }
		}

		/// <summary>
		/// Execute a command to export the currently loaded and highlighted text (XML, C# ...)
		/// into an HTML data format (*.htm, *.html ...)
		/// </summary>
		public static RoutedUICommand ExportTextToHTML
		{
			get { return exportTextToHTML; }
		}


        public static RoutedUICommand ClearAllMruItemsCommand
        {
            get { return clearAllMruItemsCommand; }
        }


        public static RoutedUICommand RemoveItemsOlderThanThisCommand
        {
            get { return removeItemsOlderThanThisCommand; }
        }

        public static RoutedUICommand MovePinnedMruItemUPCommand
        {
            get { return movePinnedMruItemUPCommand; }
        }

        public static RoutedUICommand MovePinnedMruItemDownCommand
        {
            get { return movePinnedMruItemDownCommand; }
        }

        public static RoutedUICommand PinItemCommand
        {
            get { return pinItemCommand; }
        }

        public static RoutedUICommand UnPinItemCommand
        {
            get { return unPinItemCommand; }
        }


        /// <summary>
        /// Execute pin/unpin command (to set or unset a pin in MRU and re-sort list accordingly)
        /// </summary>
        public static RoutedUICommand PinUnpin
		{
			get { return pinUnpin; }
		}

		/// <summary>
		/// Execute add recent files list etnry pin command (to add another MRU entry into the list)
		/// </summary>
		public static RoutedUICommand AddMruEntry
		{
			get { return addMruEntry; }
		}

		/// <summary>
		/// Execute remove pin command (remove a pin from a recent files list entry)
		/// </summary>
		public static RoutedUICommand RemoveMruEntry
		{
			get { return removeMruEntry; }
		}

		public static RoutedUICommand CloseFile
		{
			get { return closeFile; }
		}

		/// <summary>
		/// Static property of the correspondong <seealso cref="System.Windows.Input.RoutedUICommand"/>
		/// </summary>
		public static RoutedUICommand ViewTheme
		{
			get { return viewTheme; }
		}

		/// <summary>
		/// Browse to an Internet URL via default web browser configured in Windows
		/// </summary>
		public static RoutedUICommand BrowseURL
		{
			get { return browseURL; }
		}

		/// <summary>
		/// Static property of the correspondong <seealso cref="System.Windows.Input.RoutedUICommand"/>
		/// </summary>
		public static RoutedUICommand ShowStartPage
		{
			get { return showStartPage; }
		}

		/// <summary>
		/// Static property of the correspondong <seealso cref="System.Windows.Input.RoutedUICommand"/>
		/// </summary>
		public static RoutedUICommand ToggleOptimizeWorkspace
		{
			get { return toggleOptimizeWorkspace; }
		}

		#region Text Edit Commands
		/// <summary>
		/// Goto line n in a given document
		/// </summary>
		public static RoutedUICommand GotoLine
		{
			get { return gotoLine; }
		}

		/// <summary>
		/// Find text in a given document
		/// </summary>
		public static RoutedUICommand FindText
		{
			get { return findText; }
		}

		public static RoutedUICommand FindNextText
		{
			get { return findNextText; }
		}

		public static RoutedUICommand FindPreviousText
		{
			get { return findPreviousText; }
		}

		/// <summary>
		/// Find and replace text in a given document
		/// </summary>
		public static RoutedUICommand ReplaceText
		{
			get { return replaceText; }
		}

		public static RoutedUICommand DisableHighlighting
		{
			get { return disableHighlighting; }
		}
		#endregion Text Edit Commands
		#endregion CommandFramwork_Properties
	}
}
