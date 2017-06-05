
1 Version

This Readme file has the Version 1.0.14.

2 Edi 1.0

Edit is a WPF text editor application written and published by
Dirkster99 on CodeProject (http://www.codeproject.com/Articles/Dirkster99#Articles)
and the contributing authors listed in the credits section.

2.0 Building Edi from Sources

- Building Edi from sources requires to build the AvalonDock sources first:
  Change into sub-folder: edi\02_Libs\AvalonDock
  and compile the test application project.
  
  Copy the resulting AvalonDock DLLs into the sub-folder:
  edi\02_Libs\00_bin\Xceed.Wpf.AvalonDock

- Open the Edi project and check whether the AvalonDock references appear OK

- Rebuild the Edi project.

2.1 Introduction

As an absolute beginner in WPF - I have often been looking for a
Model-View-ViewModel (MVVM, http://de.wikipedia.org/wiki/Model_View_ViewModel) compliant implementation
of a Most Recent Files (MRU, http://www.codeproject.com/Articles/8741/Most-Recently-Used-MRU-Component)
implementation. Those implementations that I found where either too old or too complicated or both.

So, when I felt confident enough to do this I implemented my own version as part of my SimpleControls
library project documented here: http://www.codeproject.com/Articles/332541/WPFControlCompositionPart2of2.

Having implemented an MRU of my liking I also had to find a useful application framework
(http://waf.codeplex.com/) to give people a practical understanding of why I implemented
this that way and that the other way.

To my own surprise, I ended up implementing Edi using the awesome
open source components AvalonDock and AvalonEdit (see Credits section for more details).

3 Requirements

This section lists all components and their respective requirements.

3.0 General Requirements

§ 3.0.1 Every aspect of the application is skinable. The User can change the current theme (skin)
        via View>Themes>... menu entry.
        (Themes: Aero, Expression Dark, VS2010, and Generic)

        On Application Start:
        The application restores the theme that was applied in the last session.
		The current editor line can be highlighted and customized via theme
		(use different current line highlighting colors).

§ 3.0.1 The application supports a true/false option to decide whether a second editor
        is started when a user invokes the same executable twice.
        The editor implements a single instance mode if this option is true.

§ 3.0.2 The application offers an about dialog and icons branding to inform users about its purpose and origin.

3.1 Most Recent Files (MRU)

§ 3.1.1 TODO
        The user can change between pinned and unpinned mode via Tools>Options dialog.

§ 3.1.2 Clicking on a file in the recent file list activates that document
        if it is already open in the AvalonDock.

§ 3.1.4 Opening a file from a non-existing location (folder has been renamed or file is removed)
        results in an error message and the program offers an option to remove the MRU file entry
        (if an entry is already present in the MRU).

§ 3.1.3 Changed documents are saved on close of application

      - Close button in upper left corner of Main Window
   		- File>Exit in Main Menu
        - Keyboard shortcut: Alt+F4
        
   		- File>Close in Main Menu
        - Close Button on document tab window (in docked or undocked mode)
        - Keyboard shortcut: Ctrl+F4 or Ctrl+W

§ 3.1.1.20 The MRU is visible in:

           - a seperate tool window called Recent Files
           - in the main menu under File>Recent Files

           The MRU supports two different modes:

           - PinnedEntriesFirst
           - UnsortedFavourites

           These modes are detailled in the following sub-sections.

3.1.1 PinnedEntriesFirst

§ 3.1.1.0 The MRU list displayed in the tool window contains two sub-lists:
        - Pinned Entries   (entries that are displayed with a pin next to the file name)
        - Unpinned Entries (entries that are displayed without a pin next to the file name)
        
        All Pinned Entries are displayed before the unpinned entries.

§ 3.1.1.1 The MRU list in the tool window displays a pin icon to pin entries at the beginning of a MRU list.

§ 3.1.1.2 The user can click on the pin icon and the application displays the corresponding entry in the list
          of pinned entries (at the beginning of the list).

3.1.2 UnsortedFavourites

§ 3.1.2.1 TODO
          The MRU list in the tool window displays a Favourite-Star icon to pin entries at
          their current position of the MRU list.

§ 3.1.2.2 The user can click on the Favourite-Star icon and the application displays the
          corresponding entry in the list (the pinned entry remains where the unpinned entry was).

3.2 AvalonDock 2.0

See Build "*.txt" file in "AvalonDock" sub-folder for details about version and source of origin.

3.2.1 Requirements

This section lists the requirements that should (at least in part) be implemented by AvalonDock.

§ 3.1.0 Executing the Save command with a new file (file has never been saved) executes the Save As command.

        The Open/SaveAs file dialog opens in the location of the currently active document (if any).

        Otherwise, if there is no active document or the active document has never been saved before,
        the location of the last file open or file save/save as (which ever was last)
        is displayed in the Open/SaveAs File dialog.
        
        The Open/SaveAs file dialog opens in the MyDocuments Windows user folder
        if none of the above conditions are true. (eg.: Open file for the very first
        time or last location does not exist).

        The Open/Save/SaveAs file dialog opens in "C:\" if none of the above requirements
        can be implemented (eg.: MyDocuments folder does not exist or user has no access).

        The last Open/Save/SaveAs file location used is stored and recovered between user sessions.

§ 3.1.1 The Application offers a Save All function that can be used to save all changed documents
        plus user program settings (MRU etc).

§ 3.1.2 The Application can be associated with file extensions and will open these files
        via command line parameter ("Edi.exe C:\Readme.txt").

§ 3.2.1 Editing a document results in showing a dirty mark ("*") next to the file name of the
        of the corresponding document tab.
       
§ 3.2.2 The dirty mark ("*") is not shown when the user opens or saves a file.

§ 3.2.3 On exit of application:
        The application asks the user whether edited documents (documents displayed with a
        dirty mark ("*") in the document tab shall be saved or not -before exit of application
        continues.

        The user can cancel the process of exciting the application if a document contains a dirty mark.

        This function is also invoced when Windows shuts down and there are still documents with a question mark.

§ 3.2.4 New documents do not contain a dirty mark (and most therefore not be saved), unless they are
        actually edited by the user.
        
        Files saved via File>New and File>Save or File>Save As are added into the MRU list.

§ 3.1.5 CTRL-Tabbing, between two or more documents, and returning to a previously shown document
        does not change the previously shown cursor position or text selection.
        
        The current view (X/Y scroll position is recovered if it is further down the view
        (eg.: line 90, column 120).
        
        Bug: The rectangular selection (unix box mode) is not re-covered correctly if the selection
             goes beyond the end of a line.
             http://www.codeproject.com/Articles/42490/Using-AvalonEdit-WPF-Text-Editor?msg=4388281#xx4388281xx

        ALT-Tabbing away from the editor and returning to it restores the focus and cursor position
        or selection of the editor that was previously used (if any).
        
        Bug
        Getting ALT+Tab (Application Activation/Deactivation) to work requires event handling in the
        MainWindow class and recording/restoring focus on Deactivation/Activation. This solution does
        not work if a document/tool window is undocked.

        See Issue for more details
        http://avalondock.codeplex.com/workitem/15134?ProjectName=avalondock

        Bug
        The current solution does not show the MainWindow bar with the activated color
        when doing ALT+Tab AND ALT+Tab back to editor.

§ 3.2.6 TODO
        The application has an option to choose whether a *.bak file is created
        (in a dedicated folder) before editing takes place. The user can enable
        or disable this option to be on the save side when saving edits to disk.

§ 3.2.6 Performing Drag&Drop of file(s) over the main window opens those file(s)
        with the standard open file command.
        
        http://www.codeproject.com/Articles/422537/Patterns-in-Attached-Behaviours

§ 3.2.6 TODO
        Check whether a file exists or not when the application is restored from minimized state.
        Check whether a file exists or not when exciting the application.

§ 3.2.7 The document tab in AvalonDock templates the AvalonEdit component such that a
        scale combobox is provided (at the lower left corner). The scale of the content
        shown in the document is changed when the user selects a different scale in percentage.
        
        The application provides a setting to determine the size of a 100% scale (eg.: 12pt).
        This default setting is stored and recovered between user sessions. The default scale
        is applied when new files are viewed (Bug File>New, File>Open)

        TODO: The user can change the default scale in an options view.

§ 3.2.8 File>Open (Read-Only files)
        The application indicates a read-only mode (a lock symbol in the document tab next to the title)
        when a file cannot be opened with read & write access.
        
        Reasons for read-only mode:
        - The file attribute Read-Only is set in Windows Explorer file properties dialog
        - A file is locked by another process

3.2.2 Known Problems

§ 3.2.1 Bug
        Some commands (Edit, Copy, Past, Delete) do not work with documents in undocked mode,
        unless the function is invocked through the document context menu. It is therefore,
        recommended either to:
        - use only the context menu in undocked more or
        - use documents in docked mode only
          (until further notice).

§ 3.2.2 Bug
        File>New
        File>Save As
        
        Adjusts the file name shown in the document tab
        Adjust the highlighting pattern for the type of file saved
        Removes the dirty mark if saving was succesful.

§ 3.3.0 Every controls shall show its whether its disabled or enabled.
        Implemented in ExpressionDark theme

        TODO ToolTipService.ShowOnDisabled="True" should be stated in all corresponding styles in all themes

§ 3.4.0 Edi does not support localization at this time. Therefore, localization always defaults to:

		Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

        in App.Application_Startup()
        
        TODO
        Store all GUI relevant strings in a resource and add a language option
        Test whether AvalonEdit can support languages that require FlowDirection="LeftToRight" (Persian)

3.3 AvalonEdit

See Build "*.txt" file in "ICSharpCode.AvalonEdit" sub-folder for details about version
and source of origin.

Local fixes are marked with Dirkster comments.

3.3.1 Editing

§ 3.3.1.0 The editor supports Unix style editing with block selection and copy & past operations
          To select text boxes:
          Hold shift key and move the mouse
          OR
          Hold Shift-Alt+(left, right, up, down) Cursor key

§ 3.2.1.1 TODO
          Usage of Tabs and Spaces

          The editor offers an option to display tabs with different numbers of spaces (default 4).
          Tab characters can replaced with the configured number of spaces (via explicit menu function
          or on file load).
          
          Tabs can be displayed with a certain number of spaces (the tab character is not used at all).

§ 3.2.1.2 Code Completion can be customized and used
          Put a file with keywords into: 
          
          AvalonDock.MVVMTestApp\Intellisense\Keywords\ (set propertiew to copy always)
          Press Space to open Pop-up completion window
          (Download http://dotnetnotepad.codeplex.com/ and press space when editin C# file)

§ 3.2.1.3 The current line is highlighted with a background color (even if the editor has no focus)
          http://stackoverflow.com/questions/5072761/avalonedit-highlight-current-line-even-when-not-focused

§ 3.2.1.4 AvalonEdit supports:
          - Find (CTRL-F) and
          - Find Next (F3)
          - Find/Replace (CTRL-H) functionality
          - Find Previous (Shift+F3)

          for current document

          TODO 
          Refactor Find/Repalce and implement Find/Replace over more than one document
          Implement Find/Replace over a directory/sub-directory and their files (based on file pattern merging)
          
          Find/Replace function is based on (its a hack right now)
          (By Thomas Willwacher - http://www.codeproject.com/Articles/173509/A-Universal-WPF-Find-Replace-Dialog

		  AvalonEdit from SharpDevelop Wiki
		  (By Daniel Grunwald - http://wiki.sharpdevelop.net/AvalonEdit.ashx)

§ 3.2.1.4 AvalonEdit supports Goto Line functionality

          Workflow
          The User enters a short-cut (default CTRL+G) in an open document
          The System displays a Goto Line view that shows:
           - the current line number
           - the minimum and maximum valid line number for the current document

          The user can cancel the process with the Escape key or click on Cancel.

          The user can enter a valid line number and press Enter or click on OK.

          On valid line number:
            - The System shows an error message if the entered number is invalid
          
          On valid line number:
            - The System displays the entered line in the current document

§ 3.2.1.4 The Editor supports Insert/Overtype Mode
          Default: Insert Mode
          Press Insert Key to switch to Overtype Mode
          Press Insert Key to switch back Insert Mode
          
          The System displays "INS" or "OVR" to indicate the current mode of typing
          in the lower right corner of the Application MainWindow StatusBar.

§ 3.2.1.4 The Editor supports a line/column indicator in the lower right corner
          of the application MainWindow. The Indicator reads
          "Line <x> Column <y>" where <x> and <y> are the current line and
          column coordinates of the cursor in the currently active document.

§ 3.2.1.5 The Editor supports customizable syntax highlighting colors which can be
          configured per file type. Highlighting colors and search patterns are configured
          through XSHD files stored in a dedicated sub-folder.

          The syntax highlighting sub-folder is currently the Highlighting sub-folder of the binary.

          ToDo:
          Make highlighting folder a program option to allow customizable location of these files.

§ 3.2.1.6 The editor supports configurable highlighting themes that can be configured per:
          
          - file type highlighting definition (eg.: XSHD definition for SQL) 
          - and WPF theme (eg.: Dark Expression)

          A highlighting theme can include global styles (editor background or hyperlink color)

§ 3.2.1.7 The application supports a disable highlighting button to switch off
          syntax highlighting for the current file.


§ 3.2.1.20 The editor attempts to detect a file encoding and shows the encoding currently used
           (see bottom right corner of main window).

           TODO:
           > The user can set the encoding when opening a new file.
           > The user can set the encoding when editing a file in order to save it with a different encoding.

§ 3.3.0.0 The editor supports highlighting of links and (CTRL+)MouseClick execution. Strings, such as:

          "C:\"
          "\\YDrive"
          "\\YDrive\MyFile.log"
          "\\YDrive\MyFile sdf\345345~1.log"
          "C:\MyFile.log"
          "C:\MyFile\sdf sdf.log"
          "F:\YDrive\"
          "F:\XDirvte\Note.txt"
          (Links are not displayed for MS-DOS and UNC paths unless they are within quotation marks)

          "file://sdfsfd/htr.html"
          "https://edi.codeplex.com/"
          "http://edi.codeplex.com/"
          "www.docs.google.com"
          "mailto:Blah@nowhere.com"

          are underlined and theme colored as hyperlink. The hyperlink is evaluated and executed
          when user (CTRL+)MouseClicks it. Errors (eg.: on dead-links) are not displayed in Edi.

          Sample string that is not highlighted (since quotation marks are missing): F:\XDirvte\Note.txt

§ 3.3.3.0 The text editor supports zooming of text view via
          - CTRL + MouseWheel,
          - Zoom Gesture, and
          - Zoom Combobox control (in lower left corner of editor control).

3.4 Application Framework

This section contains requirements on the application framework, which is in this case the outer shell
wrapped around AvalonDock. Thats the MainWindow, the App Class, and other such things.

2.2.1 Requirements

This section lists the requirements that should (at least in part) be implemented by the Application Framework.

§ 3.4.1 The main application window opens with position, width, and height recovered from the last user session.

§ 3.4.2 Application settings are saved into the user's AppData folder when the application session ends.
        
        Application settings are loaded from the user's AppData folder when the application session starts. This includes an option that determines whether, or not, open files
        are re-loaded and displayed on next application start-up.
        
        TODO
        Review XML Serialization of Observable Collection in MRU control.

§ 3.4.3 TODO
        Application short-cuts should be configurable
        Nice to have - The application (and the editor) should accept more than
        one keyboard short-cut to implement the same function
        (Sample: Firefox closes window tabs with Ctrl-W or Ctrl-F4 [is already implemented])

§ 3.4.10 The application creates and writes to a log file to support (stabability)
         error report/resolution problems in production.
         
         The logging file is located at: %TEMP%\Edi_Log.xml
         The Edi.exe.config file can be used to configure whether logging is enabled or not.
         http://logging.apache.org/log4net/

3.1 Terminology

-------------------+-------------------------+----------------------------------------------------
English            | German                  | Description
-------------------+-------------------------+----------------------------------------------------
Most Recent Files  | Zuletzt genutzte        | Most Recent Files (MRU) are recently accessed via
                   | Dateien                 | File>Open, drag & drop etc. These files are
                   | Dateien                 | displayed at the top of the MRU list.
-------------------+-------------------------+----------------------------------------------------
Program Settings   | Programmeinstellungen   | Refers to those options that can be configured to
                   |                         | adjust a program to a user's needs.
-------------------+-------------------------+----------------------------------------------------

3.1.1 Typical XAML Constructs to Localize

Perform regular expression search with:
Header="[A-Z]
ToolTip="[A-Z]
Content="[A-Z]
Text="[A-Z]

4 Licensing

This editor is published under the MIT license.

5 Credits

* AvalonEdit written by Daniel Grunewald:
- www.avalonedit.net/
- http://www.codeproject.com/Articles/42490/Using-AvalonEdit-WPF-Text-Editor
- http://wiki.sharpdevelop.net/AvalonEdit.ashx

AvalonEdit is among the best open source WPF editors world wide.
It is build for extensibility and scalability. It has features like highlighting
and rivals many comercial and non-comercial editors.

* AvalonDock 2.0
  By Adolfo Marinucci - http://avalondock.codeplex.com/

* Highlighting and Code Completion framework (loading definitions at start-up of application)
  .NET Notepad
  By tym32167 . http://dotnetnotepad.codeplex.com/
  
  TODO
  Review A WPF TikZ Editor (TikzEdt)
  By Thomas Willwacher, Julian Ohrt - http://www.codeproject.com/Articles/154371/A-WPF-TikZ-Editor-TikzEdt

* Find/Replace function is based on
  By Thomas Willwacher - http://www.codeproject.com/Articles/173509/A-Universal-WPF-Find-Replace-Dialog

The AvalonDock 2.0 and AvalonEdit components are in developed as we speak -
so they may not function completely. But the available functions are already very impressive
- and absolutely sufficient to show case the usage of WPF. This very file was in fact written
and edited using Edi with a custom highlighting pattern for *.txt files.

* Log4Net is an effort from the Apache Foundation
  http://logging.apache.org/log4net/
