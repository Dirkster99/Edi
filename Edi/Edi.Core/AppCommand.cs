using System.Windows.Input;
using Edi.Util.Local;

namespace Edi.Core
{
	public class AppCommand
	{
		#region CommandFramework Fields
		private static RoutedUICommand _exit;
		private static RoutedUICommand _about;
		private static RoutedUICommand _programSettings;
		private static RoutedUICommand _showToolWindow;

		private static RoutedUICommand _loadFile;
		private static RoutedUICommand _saveAll;
		private static RoutedUICommand _exportUmlToImage;
		private static RoutedUICommand _exportTextToHtml;

        private static RoutedUICommand _clearAllMruItemsCommand;
        private static RoutedUICommand _removeItemsOlderThanThisCommand;
        private static RoutedUICommand _movePinnedMruItemUpCommand;
        private static RoutedUICommand _movePinnedMruItemDownCommand;
        private static RoutedUICommand _pinItemCommand;
        private static RoutedUICommand _unPinItemCommand;
        private static RoutedUICommand _pinUnpin;
		private static RoutedUICommand _addMruEntry;
		private static RoutedUICommand _removeMruEntry;

		private static RoutedUICommand _closeFile;
		private static RoutedUICommand _viewTheme;

		private static RoutedUICommand _browseUrl;
		private static RoutedUICommand _showStartPage;

		private static RoutedUICommand _toggleOptimizeWorkspace;

		#region Text Edit Commands
		private static RoutedUICommand _gotoLine;
		private static RoutedUICommand _findText;
		private static RoutedUICommand _findNextText;
		private static RoutedUICommand _findPreviousText;
		private static RoutedUICommand _replaceText;

		private static RoutedUICommand _disableHighlighting;

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
			_exit = new RoutedUICommand(Strings.CMD_App_Exit_Describtion, "Exit", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			_about = new RoutedUICommand(Strings.CMD_APP_About_Description, "About", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			_programSettings = new RoutedUICommand("Edit or Review your program settings", "ProgramSettings", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			_showToolWindow = new RoutedUICommand("Hide or display toolwindow", "ShowToolWindow", typeof(AppCommand), inputs);

			// Execute file open command (without user interaction)
			inputs = new InputGestureCollection();
			_loadFile = new RoutedUICommand(Strings.CMD_APP_Open_Description, "LoadFile", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			_saveAll = new RoutedUICommand(Strings.CMD_APP_SaveAll_Description, "SaveAll", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			_exportUmlToImage = new RoutedUICommand(Strings.CMD_APP_ExportUMLToImage_Description, "ExportUMLToImage", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			_exportTextToHtml = new RoutedUICommand(Strings.CMD_APP_ExportTextToHTML_Description, "ExportTextToHTML", typeof(AppCommand), inputs);

            // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
            // MRU Commands
            inputs = new InputGestureCollection();
            _clearAllMruItemsCommand = new RoutedUICommand(string.Empty, "Remove All MRU Entries", typeof(AppCommand), inputs);

            inputs = new InputGestureCollection();
            _removeItemsOlderThanThisCommand = new RoutedUICommand(string.Empty, "Remove Entries older than this", typeof(AppCommand), inputs);

            inputs = new InputGestureCollection();
            _movePinnedMruItemUpCommand = new RoutedUICommand(string.Empty, "Move Up", typeof(AppCommand), inputs);

            inputs = new InputGestureCollection();
            _movePinnedMruItemDownCommand = new RoutedUICommand(string.Empty, "Move Down", typeof(AppCommand), inputs);

            inputs = new InputGestureCollection();
            _pinItemCommand = new RoutedUICommand(string.Empty, "Pin to this list", typeof(AppCommand), inputs);

            inputs = new InputGestureCollection();
            _unPinItemCommand = new RoutedUICommand(string.Empty, "Unpin from this list", typeof(AppCommand), inputs);

            // Initialize pin command (to set or unset a pin in MRU and re-sort list accordingly)
            inputs = new InputGestureCollection();
			_pinUnpin = new RoutedUICommand(Strings.CMD_MRU_Pin_Description, "Pin", typeof(AppCommand), inputs);

			// Execute add recent files list etnry pin command (to add another MRU entry into the list)
			inputs = new InputGestureCollection();
			_addMruEntry = new RoutedUICommand(Strings.CMD_MRU_AddEntry_Description, "AddEntry", typeof(AppCommand), inputs);

			// Execute remove pin command (remove a pin from a recent files list entry)
			inputs = new InputGestureCollection();
			_removeMruEntry = new RoutedUICommand(Strings.CMD_MRU_RemoveEntry_Description, "RemoveEntry", typeof(AppCommand), inputs);

            // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
            // Window Close Command
			inputs = new InputGestureCollection();
			inputs.Add(new KeyGesture(Key.F4, ModifierKeys.Control, "Ctrl+F4"));
			inputs.Add(new KeyGesture(Key.W, ModifierKeys.Control, "Ctrl+W"));
			_closeFile = new RoutedUICommand(Strings.CMD_APP_CloseDoc_Description, "Close", typeof(AppCommand), inputs);

			// Initialize the viewTheme command
			inputs = new InputGestureCollection();
			_viewTheme = new RoutedUICommand(Strings.CMD_APP_ViewTheme_Description, "ViewTheme", typeof(AppCommand), inputs);

			// Execute browse Internt URL (without user interaction)
			inputs = new InputGestureCollection();
			_browseUrl = new RoutedUICommand(Strings.CMD_APP_OpenURL_Description, "OpenURL", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			_showStartPage = new RoutedUICommand(Strings.CMD_APP_ShowStartPage_Description, "StartPage", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			_toggleOptimizeWorkspace = new RoutedUICommand(Strings.CMD_APP_ToggleOptimizeWorkspace_Description, "ToggleOptimizeWorkspace", typeof(AppCommand), inputs);

			#region Text Edit Commands
			inputs = new InputGestureCollection();
			_disableHighlighting = new RoutedUICommand(Strings.CMD_TXT_DisableHighlighting_Description, "DisableHighlighting", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();                                     // Goto Line n in the current document
			inputs.Add(new KeyGesture(Key.G, ModifierKeys.Control, "Ctrl+G"));
			_gotoLine = new RoutedUICommand(Strings.CMD_TXT_GotoLine_Description, "GotoLine", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			inputs.Add(new KeyGesture(Key.F, ModifierKeys.Control, "Ctrl+F"));
			_findText = new RoutedUICommand(Strings.CMD_TXT_FindNext_Description, "FindText", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			inputs.Add(new KeyGesture(Key.F3, ModifierKeys.None, "F3"));
			_findNextText = new RoutedUICommand(Strings.CMD_TXT_FindNextText_Description, "FindNextText", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			inputs.Add(new KeyGesture(Key.F3, ModifierKeys.Shift, "Shift+F3"));
			_findPreviousText = new RoutedUICommand(Strings.CMD_TXT_FindPreviousText_Description, "FindPreviousText", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			inputs.Add(new KeyGesture(Key.H, ModifierKeys.Control, "Ctrl+H"));
			_replaceText = new RoutedUICommand(Strings.CMD_TXT_FindReplaceText_Description, "FindReplaceText", typeof(AppCommand), inputs);
			#endregion Text Edit Commands
		}
		#endregion Static Constructor

		#region CommandFramwork Properties (Exposes Commands to which the UI can bind to)
		/// <summary>
		/// Static property of the correspondong <seealso cref="System.Windows.Input.RoutedUICommand"/>
		/// </summary>
		public static RoutedUICommand Exit
		{
			get { return _exit; }
		}

		public static RoutedUICommand About
		{
			get { return _about; }
		}

		public static RoutedUICommand ProgramSettings
		{
			get
			{
				return _programSettings;
			}
		}

		public static RoutedUICommand ShowToolWindow
		{
			get
			{
				return _showToolWindow;
			}
		}

		/// <summary>
		/// Execute file open command (without user interaction)
		/// </summary>
		public static RoutedUICommand LoadFile
		{
			get { return _loadFile; }
		}

		/// <summary>
		/// Execute a command to save all edited files and current program settings
		/// </summary>
		public static RoutedUICommand SaveAll
		{
			get { return _saveAll; }
		}

		/// <summary>
		/// Execute a command to export the currently loaded UML Diagram (XML based data)
		/// into an image based data format (png, jpeg, wmf)
		/// </summary>
		public static RoutedUICommand ExportUmlToImage
		{
			get { return _exportUmlToImage; }
		}

		/// <summary>
		/// Execute a command to export the currently loaded and highlighted text (XML, C# ...)
		/// into an HTML data format (*.htm, *.html ...)
		/// </summary>
		public static RoutedUICommand ExportTextToHtml
		{
			get { return _exportTextToHtml; }
		}


        public static RoutedUICommand ClearAllMruItemsCommand
        {
            get { return _clearAllMruItemsCommand; }
        }


        public static RoutedUICommand RemoveItemsOlderThanThisCommand
        {
            get { return _removeItemsOlderThanThisCommand; }
        }

        public static RoutedUICommand MovePinnedMruItemUpCommand
        {
            get { return _movePinnedMruItemUpCommand; }
        }

        public static RoutedUICommand MovePinnedMruItemDownCommand
        {
            get { return _movePinnedMruItemDownCommand; }
        }

        public static RoutedUICommand PinItemCommand
        {
            get { return _pinItemCommand; }
        }

        public static RoutedUICommand UnPinItemCommand
        {
            get { return _unPinItemCommand; }
        }


        /// <summary>
        /// Execute pin/unpin command (to set or unset a pin in MRU and re-sort list accordingly)
        /// </summary>
        public static RoutedUICommand PinUnpin
		{
			get { return _pinUnpin; }
		}

		/// <summary>
		/// Execute add recent files list etnry pin command (to add another MRU entry into the list)
		/// </summary>
		public static RoutedUICommand AddMruEntry
		{
			get { return _addMruEntry; }
		}

		/// <summary>
		/// Execute remove pin command (remove a pin from a recent files list entry)
		/// </summary>
		public static RoutedUICommand RemoveMruEntry
		{
			get { return _removeMruEntry; }
		}

		public static RoutedUICommand CloseFile
		{
			get { return _closeFile; }
		}

		/// <summary>
		/// Static property of the correspondong <seealso cref="System.Windows.Input.RoutedUICommand"/>
		/// </summary>
		public static RoutedUICommand ViewTheme
		{
			get { return _viewTheme; }
		}

		/// <summary>
		/// Browse to an Internet URL via default web browser configured in Windows
		/// </summary>
		public static RoutedUICommand BrowseUrl
		{
			get { return _browseUrl; }
		}

		/// <summary>
		/// Static property of the correspondong <seealso cref="System.Windows.Input.RoutedUICommand"/>
		/// </summary>
		public static RoutedUICommand ShowStartPage
		{
			get { return _showStartPage; }
		}

		/// <summary>
		/// Static property of the correspondong <seealso cref="System.Windows.Input.RoutedUICommand"/>
		/// </summary>
		public static RoutedUICommand ToggleOptimizeWorkspace
		{
			get { return _toggleOptimizeWorkspace; }
		}

		#region Text Edit Commands
		/// <summary>
		/// Goto line n in a given document
		/// </summary>
		public static RoutedUICommand GotoLine
		{
			get { return _gotoLine; }
		}

		/// <summary>
		/// Find text in a given document
		/// </summary>
		public static RoutedUICommand FindText
		{
			get { return _findText; }
		}

		public static RoutedUICommand FindNextText
		{
			get { return _findNextText; }
		}

		public static RoutedUICommand FindPreviousText
		{
			get { return _findPreviousText; }
		}

		/// <summary>
		/// Find and replace text in a given document
		/// </summary>
		public static RoutedUICommand ReplaceText
		{
			get { return _replaceText; }
		}

		public static RoutedUICommand DisableHighlighting
		{
			get { return _disableHighlighting; }
		}
		#endregion Text Edit Commands
		#endregion CommandFramwork_Properties
	}
}
