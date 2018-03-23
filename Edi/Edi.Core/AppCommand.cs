using System.Windows.Input;
using Edi.Util.Local;

namespace Edi.Core
{
	public class AppCommand
	{
		#region CommandFramework Fields

		#region Text Edit Commands

		#endregion Text Edit Commands
		#endregion CommandFramework Fields

		#region Static Constructor (Constructs static application commands)
		/// <summary>
		/// Define custom commands and their key gestures
		/// </summary>
		static AppCommand()
		{
			// Initialize the exit command
			var inputs = new InputGestureCollection {new KeyGesture(Key.F4, ModifierKeys.Alt, "Alt+F4")};
			Exit = new RoutedUICommand(Strings.CMD_App_Exit_Describtion, "Exit", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			About = new RoutedUICommand(Strings.CMD_APP_About_Description, "About", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			ProgramSettings = new RoutedUICommand("Edit or Review your program settings", "ProgramSettings", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			ShowToolWindow = new RoutedUICommand("Hide or display toolwindow", "ShowToolWindow", typeof(AppCommand), inputs);

			// Execute file open command (without user interaction)
			inputs = new InputGestureCollection();
			LoadFile = new RoutedUICommand(Strings.CMD_APP_Open_Description, "LoadFile", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			SaveAll = new RoutedUICommand(Strings.CMD_APP_SaveAll_Description, "SaveAll", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			ExportUmlToImage = new RoutedUICommand(Strings.CMD_APP_ExportUMLToImage_Description, "ExportUMLToImage", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			ExportTextToHtml = new RoutedUICommand(Strings.CMD_APP_ExportTextToHTML_Description, "ExportTextToHTML", typeof(AppCommand), inputs);

            // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
            // MRU Commands
            inputs = new InputGestureCollection();
            ClearAllMruItemsCommand = new RoutedUICommand(string.Empty, "Remove All MRU Entries", typeof(AppCommand), inputs);

            inputs = new InputGestureCollection();
            RemoveItemsOlderThanThisCommand = new RoutedUICommand(string.Empty, "Remove Entries older than this", typeof(AppCommand), inputs);

            inputs = new InputGestureCollection();
            MovePinnedMruItemUpCommand = new RoutedUICommand(string.Empty, "Move Up", typeof(AppCommand), inputs);

            inputs = new InputGestureCollection();
            MovePinnedMruItemDownCommand = new RoutedUICommand(string.Empty, "Move Down", typeof(AppCommand), inputs);

            inputs = new InputGestureCollection();
            PinItemCommand = new RoutedUICommand(string.Empty, "Pin to this list", typeof(AppCommand), inputs);

            inputs = new InputGestureCollection();
            UnPinItemCommand = new RoutedUICommand(string.Empty, "Unpin from this list", typeof(AppCommand), inputs);

            // Initialize pin command (to set or unset a pin in MRU and re-sort list accordingly)
            inputs = new InputGestureCollection();
			PinUnpin = new RoutedUICommand(Strings.CMD_MRU_Pin_Description, "Pin", typeof(AppCommand), inputs);

			// Execute add recent files list etnry pin command (to add another MRU entry into the list)
			inputs = new InputGestureCollection();
			AddMruEntry = new RoutedUICommand(Strings.CMD_MRU_AddEntry_Description, "AddEntry", typeof(AppCommand), inputs);

			// Execute remove pin command (remove a pin from a recent files list entry)
			inputs = new InputGestureCollection();
			RemoveMruEntry = new RoutedUICommand(Strings.CMD_MRU_RemoveEntry_Description, "RemoveEntry", typeof(AppCommand), inputs);

            // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
            // Window Close Command
			inputs = new InputGestureCollection
			{
				new KeyGesture(Key.F4, ModifierKeys.Control, "Ctrl+F4"),
				new KeyGesture(Key.W, ModifierKeys.Control, "Ctrl+W")
			};
			CloseFile = new RoutedUICommand(Strings.CMD_APP_CloseDoc_Description, "Close", typeof(AppCommand), inputs);

			// Initialize the viewTheme command
			inputs = new InputGestureCollection();
			ViewTheme = new RoutedUICommand(Strings.CMD_APP_ViewTheme_Description, "ViewTheme", typeof(AppCommand), inputs);

			// Execute browse Internt URL (without user interaction)
			inputs = new InputGestureCollection();
			BrowseUrl = new RoutedUICommand(Strings.CMD_APP_OpenURL_Description, "OpenURL", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			ShowStartPage = new RoutedUICommand(Strings.CMD_APP_ShowStartPage_Description, "StartPage", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection();
			ToggleOptimizeWorkspace = new RoutedUICommand(Strings.CMD_APP_ToggleOptimizeWorkspace_Description, "ToggleOptimizeWorkspace", typeof(AppCommand), inputs);

			#region Text Edit Commands
			inputs = new InputGestureCollection();
			DisableHighlighting = new RoutedUICommand(Strings.CMD_TXT_DisableHighlighting_Description, "DisableHighlighting", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection
			{
				new KeyGesture(Key.G, ModifierKeys.Control, "Ctrl+G")
			}; // Goto Line n in the current document
			GotoLine = new RoutedUICommand(Strings.CMD_TXT_GotoLine_Description, "GotoLine", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection {new KeyGesture(Key.F, ModifierKeys.Control, "Ctrl+F")};
			FindText = new RoutedUICommand(Strings.CMD_TXT_FindNext_Description, "FindText", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection {new KeyGesture(Key.F3, ModifierKeys.None, "F3")};
			FindNextText = new RoutedUICommand(Strings.CMD_TXT_FindNextText_Description, "FindNextText", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection {new KeyGesture(Key.F3, ModifierKeys.Shift, "Shift+F3")};
			FindPreviousText = new RoutedUICommand(Strings.CMD_TXT_FindPreviousText_Description, "FindPreviousText", typeof(AppCommand), inputs);

			inputs = new InputGestureCollection {new KeyGesture(Key.H, ModifierKeys.Control, "Ctrl+H")};
			ReplaceText = new RoutedUICommand(Strings.CMD_TXT_FindReplaceText_Description, "FindReplaceText", typeof(AppCommand), inputs);
			#endregion Text Edit Commands
		}
		#endregion Static Constructor

		#region CommandFramwork Properties (Exposes Commands to which the UI can bind to)
		/// <summary>
		/// Static property of the correspondong <seealso cref="System.Windows.Input.RoutedUICommand"/>
		/// </summary>
		public static RoutedUICommand Exit { get; }

		public static RoutedUICommand About { get; }

		public static RoutedUICommand ProgramSettings { get; }

		public static RoutedUICommand ShowToolWindow { get; }

		/// <summary>
		/// Execute file open command (without user interaction)
		/// </summary>
		public static RoutedUICommand LoadFile { get; }

		/// <summary>
		/// Execute a command to save all edited files and current program settings
		/// </summary>
		public static RoutedUICommand SaveAll { get; }

		/// <summary>
		/// Execute a command to export the currently loaded UML Diagram (XML based data)
		/// into an image based data format (png, jpeg, wmf)
		/// </summary>
		public static RoutedUICommand ExportUmlToImage { get; }

		/// <summary>
		/// Execute a command to export the currently loaded and highlighted text (XML, C# ...)
		/// into an HTML data format (*.htm, *.html ...)
		/// </summary>
		public static RoutedUICommand ExportTextToHtml { get; }


		public static RoutedUICommand ClearAllMruItemsCommand { get; }


		public static RoutedUICommand RemoveItemsOlderThanThisCommand { get; }

		public static RoutedUICommand MovePinnedMruItemUpCommand { get; }

		public static RoutedUICommand MovePinnedMruItemDownCommand { get; }

		public static RoutedUICommand PinItemCommand { get; }

		public static RoutedUICommand UnPinItemCommand { get; }


		/// <summary>
        /// Execute pin/unpin command (to set or unset a pin in MRU and re-sort list accordingly)
        /// </summary>
        public static RoutedUICommand PinUnpin { get; }

		/// <summary>
		/// Execute add recent files list etnry pin command (to add another MRU entry into the list)
		/// </summary>
		public static RoutedUICommand AddMruEntry { get; }

		/// <summary>
		/// Execute remove pin command (remove a pin from a recent files list entry)
		/// </summary>
		public static RoutedUICommand RemoveMruEntry { get; }

		public static RoutedUICommand CloseFile { get; }

		/// <summary>
		/// Static property of the correspondong <seealso cref="System.Windows.Input.RoutedUICommand"/>
		/// </summary>
		public static RoutedUICommand ViewTheme { get; }

		/// <summary>
		/// Browse to an Internet URL via default web browser configured in Windows
		/// </summary>
		public static RoutedUICommand BrowseUrl { get; }

		/// <summary>
		/// Static property of the correspondong <seealso cref="System.Windows.Input.RoutedUICommand"/>
		/// </summary>
		public static RoutedUICommand ShowStartPage { get; }

		/// <summary>
		/// Static property of the correspondong <seealso cref="System.Windows.Input.RoutedUICommand"/>
		/// </summary>
		public static RoutedUICommand ToggleOptimizeWorkspace { get; }

		#region Text Edit Commands
		/// <summary>
		/// Goto line n in a given document
		/// </summary>
		public static RoutedUICommand GotoLine { get; }

		/// <summary>
		/// Find text in a given document
		/// </summary>
		public static RoutedUICommand FindText { get; }

		public static RoutedUICommand FindNextText { get; }

		public static RoutedUICommand FindPreviousText { get; }

		/// <summary>
		/// Find and replace text in a given document
		/// </summary>
		public static RoutedUICommand ReplaceText { get; }

		public static RoutedUICommand DisableHighlighting { get; }

		#endregion Text Edit Commands
		#endregion CommandFramwork_Properties
	}
}
