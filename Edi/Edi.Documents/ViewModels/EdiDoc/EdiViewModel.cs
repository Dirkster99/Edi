namespace Edi.Documents.ViewModels.EdiDoc
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Edi.Core.Interfaces;
    using Edi.Core.Interfaces.Documents;
    using Edi.Core.Interfaces.Enums;
    using Edi.Core.ViewModels.Command;
    using Edi.Core.ViewModels.Events;
    using Edi.Documents.Process;
    using ICSharpCode.AvalonEdit.Document;
    using ICSharpCode.AvalonEdit.Edi.BlockSurround;
    using ICSharpCode.AvalonEdit.Edi.TextBoxControl;
    using ICSharpCode.AvalonEdit.Highlighting;
    using ICSharpCode.AvalonEdit.Utils;
    using Microsoft.Win32;
    using MsgBox;
    using Settings.Interfaces;
    using Settings.ProgramSettings;
    using UnitComboLib.Unit.Screen;
    using UnitComboLib.ViewModel;

    public interface IDocumentEdi : IFileBaseViewModel
    {
        #region methods
        /// <summary>
        /// Initialize viewmodel with data that should not be initialized in constructor
        /// but is usually necessary after creating default object.
        /// </summary>
        /// <param name="SettingData"></param>
        void InitInstance(Options SettingData);

        /// <summary>
        /// Increase the document counter for new documents created via New command.
        /// </summary>
        void IncreaseNewCounter();

        /// <summary>
        /// Can be called when executing File>New for this document type.
        /// The method changes all document states such that users can start
        /// editing and be creating new content.
        /// </summary>
        void CreateNewDocument();

        /// <summary>
        /// Initialize scale view of content to indicated value and unit.
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="defaultValue"></param>
        void InitScaleView(ZoomUnit unit, double defaultValue);
        #endregion methods
    }

    /// <summary>
    /// This viewmodel class represents the business logic of the text editor.
    /// Each text editor document instance is associated with a <seealso cref="EdiViewModel"/> instance.
    /// </summary>
    public class EdiViewModel : Edi.Core.ViewModels.FileBaseViewModel,
                              Edi.Dialogs.FindReplace.ViewModel.IEditor,
                                                            IDocumentEdi,
                                                            IDocumentFileWatcher
    {
        #region Fields
        public const string DocumentKey = "EdiTextEditor";
        public const string Description = "Text files";
        public const string FileFilterName = "All Files";
        public const string DefaultFilter = "*";

        private static int iNewFileCounter = 0;
        private string mDefaultFileName = Edi.Util.Local.Strings.STR_FILE_DEFAULTNAME;
        private string mDefaultFileType = ".txt";

        private TextDocument mDocument;
        private ICSharpCode.AvalonEdit.TextEditorOptions mTextOptions;
        private IHighlightingDefinition mHighlightingDefinition;

        private string mFilePath = null;
        private bool mIsDirty = false;

        private object lockThis = new object();

        private bool mWordWrap = false;            // Toggle state command
        private bool mShowLineNumbers = true;     // Toggle state command
        private Encoding mFileEncoding = Encoding.UTF8;

        private int mLine = 0;      // These properties are used to display the current column/line
        private int mColumn = 0;    // of the cursor in the user interface

        // These properties are used to save and restore the editor state when CTRL+TABing between documents
        private int mTextEditorCaretOffset = 0;
        private int mTextEditorSelectionStart = 0;
        private int mTextEditorSelectionLength = 0;
        private bool mTextEditorIsRectangularSelection = false;
        private double mTextEditorScrollOffsetX = 0;
        private double mTextEditorScrollOffsetY = 0;

        private TextBoxController mTxtControl = null;

        private bool mIsReadOnly = true;
        private string mIsReadOnlyReason = string.Empty;

        private FileLoader mAsyncProcessor;

        RelayCommand<object> mCloseCommand = null;
        #endregion Fields

        #region constructor
        /// <summary>
        /// Class constructor from <seealso cref="IDocumentModel"/> parameter.
        /// </summary>
        /// <param name="documentModel"></param>
        public EdiViewModel(IDocumentModel documentModel)
            : this()
        {
            this.mDocumentModel.SetFileNamePath(documentModel.FileNamePath, documentModel.IsReal);
        }

        /// <summary>
        /// Standard constructor. See also static <seealso cref="LoadFile"/> method
        /// for construction from file saved on disk.
        /// </summary>
        protected EdiViewModel()
            : base(EdiViewModel.DocumentKey)
        {
            this.CloseOnErrorWithoutMessage = false;

            // Copy text editor settings from settingsmanager by default
            this.TextOptions = new ICSharpCode.AvalonEdit.TextEditorOptions();
            this.WordWrap = false;

            var items = new ObservableCollection<ListItem>(Options.GenerateScreenUnitList());
            this.SizeUnitLabel = new UnitViewModel(items, new ScreenConverter(), 0);

            this.TxtControl = new TextBoxController();

            this.FilePath = this.GetDefaultFileNewName();

            this.IsDirty = false;
            this.mHighlightingDefinition = null;

            this.mDocument = null; //new TextDocument();

            this.TextEditorSelectionStart = 0;
            this.TextEditorSelectionLength = 0;

            this.InsertBlocks = null;
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Indicate whether error on load is displayed to user or not.
        /// </summary>
        protected bool CloseOnErrorWithoutMessage { get; set; }

        public ObservableCollection<BlockDefinition> InsertBlocks { get; set; }

        #region FilePath
        /// <summary>
        /// Get/set complete path including file name to where this stored.
        /// This string is never null or empty.
        /// </summary>
        public override string FilePath
        {
            get
            {
                if (string.IsNullOrEmpty(this.mFilePath))
                    return this.GetDefaultFileNewName();

                return this.mFilePath;
            }

            protected set
            {
                if (this.mFilePath != value)
                {
                    this.mFilePath = value;

                    this.RaisePropertyChanged(() => this.FilePath);
                    this.RaisePropertyChanged(() => this.FileName);
                    this.RaisePropertyChanged(() => this.Title);

                    this.HighlightingDefinition = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(this.mFilePath));
                }
            }
        }
        #endregion

        #region Title
        /// <summary>
        /// Title is the string that is usually displayed - with or without dirty mark '*' - in the docking environment
        /// </summary>
        public override string Title
        {
            get
            {
                return this.FileName + (this.IsDirty == true ? "*" : string.Empty);
            }
        }
        #endregion

        #region FileName
        /// <summary>
        /// FileName is the string that is displayed whenever the application refers to this file, as in:
        /// string.Format(CultureInfo.CurrentCulture, "Would you like to save the '{0}' file", FileName)
        /// 
        /// Note the absense of the dirty mark '*'. Use the Title property if you want to display the file
        /// name with or without dirty mark when the user has edited content.
        /// </summary>
        public override string FileName
        {
            get
            {
                // This option should never happen - its an emergency break for those cases that never occur
                if (string.IsNullOrEmpty(FilePath))
                    return this.GetDefaultFileNewName();

                return System.IO.Path.GetFileName(FilePath);
            }
        }

        public override Uri IconSource
        {
            get
            {
                // This icon is visible in AvalonDock's Document Navigator window
                return new Uri("pack://application:,,,/Edi.Themes;component/Images/Documents/document.png", UriKind.RelativeOrAbsolute);
            }
        }
        #endregion FileName

        #region IsReadOnly
        /// <summary>
        /// Gets/sets whether document can currently be edit by user
        /// (through attached UI) or not. Also resets IsReadOnlyReason
        /// to string.empty if value set is false.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                lock (this.lockThis)
                {
                    return this.mIsReadOnly;
                }
            }

            protected set
            {
                lock (this.lockThis)
                {
                    if (this.mIsReadOnly != value)
                    {
                        if (value == false)
                            this.IsReadOnlyReason = string.Empty;

                        this.mIsReadOnly = value;
                        this.RaisePropertyChanged(() => this.IsReadOnly);
                    }
                }
            }
        }

        public string IsReadOnlyReason
        {
            get
            {
                return this.mIsReadOnlyReason;
            }

            protected set
            {
                if (this.mIsReadOnlyReason != value)
                {
                    this.mIsReadOnlyReason = value;
                    this.RaisePropertyChanged(() => this.IsReadOnlyReason);
                }
            }
        }
        #endregion IsReadOnly

        #region TextContent
        /// <summary>
        /// This property wraps the document class provided by AvalonEdit. The actual text is inside
        /// the document and can be accessed at save, load or other processing times.
        /// 
        /// The Text property itself cannot be bound in AvalonEdit since binding this would mResult
        /// in updating the text (via binding) each time a user enters a key on the keyboard
        /// (which would be a design error resulting in huge performance problems)
        /// </summary>
        public TextDocument Document
        {
            get
            {
                return this.mDocument;
            }

            set
            {
                if (this.mDocument != value)
                {
                    this.mDocument = value;
                    this.RaisePropertyChanged(() => this.Document);
                }
            }
        }
        #endregion

        #region IsDirty
        /// <summary>
        /// IsDirty indicates whether the file currently loaded
        /// in the editor was modified by the user or not.
        /// </summary>
        override public bool IsDirty
        {
            get
            {
                return mIsDirty;
            }

            set
            {
                if (mIsDirty != value)
                {
                    mIsDirty = value;

                    this.RaisePropertyChanged(() => this.IsDirty);
                    this.RaisePropertyChanged(() => this.Title);
                }
            }
        }
        #endregion

        #region CanSaveData
        /// <summary>
        /// Get whether edited data can be saved or not.
        /// This type of document does not have a save
        /// data implementation if this property returns false.
        /// (this is document specific and should always be overriden by descendents)
        /// </summary>
        override public bool CanSaveData
        {
            get
            {
                return true;
            }
        }
        #endregion CanSaveData

        #region AvalonEdit properties
        /// <summary>
        /// AvalonEdit exposes a Highlighting property that controls whether keywords,
        /// comments and other interesting text parts are colored or highlighted in any
        /// other visual way. This property exposes the highlighting information for the
        /// text file managed in this viewmodel class.
        /// </summary>
        public IHighlightingDefinition HighlightingDefinition
        {
            get
            {
                lock (lockThis)
                {
                    return this.mHighlightingDefinition;
                }
            }

            set
            {
                lock (lockThis)
                {
                    if (this.mHighlightingDefinition != value)
                    {
                        this.mHighlightingDefinition = value;

                        this.RaisePropertyChanged(() => this.HighlightingDefinition);
                    }
                }
            }
        }

        /// <summary>
        /// Get/set whether word wrap is currently activated or not.
        /// </summary>
        public bool WordWrap
        {
            get
            {
                return this.mWordWrap;
            }

            set
            {
                if (this.mWordWrap != value)
                {
                    this.mWordWrap = value;
                    this.RaisePropertyChanged(() => this.WordWrap);
                }
            }
        }

        /// <summary>
        /// Get/set whether line numbers are currently shown or not.
        /// </summary>
        public bool ShowLineNumbers
        {
            get
            {
                return this.mShowLineNumbers;
            }

            set
            {
                if (this.mShowLineNumbers != value)
                {
                    this.mShowLineNumbers = value;
                    this.RaisePropertyChanged(() => this.ShowLineNumbers);
                }
            }
        }

        /// <summary>
        /// Get/set whether the end of each line is currently shown or not.
        /// </summary>
        public bool ShowEndOfLine               // Toggle state command
        {
            get
            {
                return this.TextOptions.ShowEndOfLine;
            }

            set
            {
                if (this.TextOptions.ShowEndOfLine != value)
                {
                    this.TextOptions.ShowEndOfLine = value;
                    this.RaisePropertyChanged(() => this.ShowEndOfLine);
                }
            }
        }

        /// <summary>
        /// Get/set whether the spaces are highlighted or not.
        /// </summary>
        public bool ShowSpaces               // Toggle state command
        {
            get
            {
                return this.TextOptions.ShowSpaces;
            }

            set
            {
                if (this.TextOptions.ShowSpaces != value)
                {
                    this.TextOptions.ShowSpaces = value;
                    this.RaisePropertyChanged(() => this.ShowSpaces);
                }
            }
        }

        /// <summary>
        /// Get/set whether the tabulator characters are highlighted or not.
        /// </summary>
        public bool ShowTabs               // Toggle state command
        {
            get
            {
                return this.TextOptions.ShowTabs;
            }

            set
            {
                if (this.TextOptions.ShowTabs != value)
                {
                    this.TextOptions.ShowTabs = value;
                    this.RaisePropertyChanged(() => this.ShowTabs);
                }
            }
        }

        /// <summary>
        /// Get/Set texteditor options frmo <see cref="AvalonEdit"/> editor as <see cref="TextEditorOptions"/> instance.
        /// </summary>
        public ICSharpCode.AvalonEdit.TextEditorOptions TextOptions
        {
            get
            {
                return this.mTextOptions;
            }

            set
            {
                if (this.mTextOptions != value)
                {
                    this.mTextOptions = value;
                    this.RaisePropertyChanged(() => this.TextOptions);
                }
            }
        }
        #endregion AvalonEdit properties

        #region SaveCommand SaveAsCommand
        /// <summary>
        /// Indicate whether there is something to save in the document
        /// currently viewed in through this viewmodel.
        /// </summary>
        override public bool CanSave()
        {
            if (this.Document == null)
                return false;

            return true;
        }

        /// <summary>
        /// Write text content to disk and (re-)set associated properties
        /// </summary>
        /// <param name="filePath"></param>
        override public bool SaveFile(string filePath)
        {
            try
            {
                File.WriteAllText(filePath, this.Document.Text);

                // Set new file name in viewmodel and model
                this.FilePath = filePath;
                this.ContentId = filePath;
                this.mDocumentModel.SetFileNamePath(filePath, true);

                this.IsDirty = false;

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Indicate whether there is something to save as ... in the document
        /// currently viewed in through this viewmodel.
        /// </summary>
        /// <returns></returns>
        override public bool CanSaveAs()
        {
            return this.CanSave();
        }
        #endregion SaveCommand SaveAsCommand

        #region CloseCommand
        /// <summary>
        /// This command cloases a single file. The binding for this is in the AvalonDock LayoutPanel Style.
        /// </summary>
        override public ICommand CloseCommand
        {
            get
            {
                if (mCloseCommand == null)
                {
                    mCloseCommand = new RelayCommand<object>((p) => this.OnClose(),
                                                                                                     (p) => this.CanClose());
                }

                return mCloseCommand;
            }
        }

        /// <summary>
        /// Determine whether document can be closed or not.
        /// </summary>
        /// <returns></returns>
        public new bool CanClose()
        {
            if (this.State == DocumentState.IsLoading)
                return false;

            return base.CanClose();
        }
        #endregion

        #region Encoding
        /// <summary>
        /// Get/set file encoding of current text file.
        /// </summary>
        public Encoding FileEncoding
        {
            get
            {
                return this.mFileEncoding;
            }

            set
            {
                if (this.mFileEncoding != value)
                {
                    this.mFileEncoding = value;
                    this.RaisePropertyChanged(() => this.mFileEncoding);
                }
            }
        }
        #endregion Encoding

        #region ScaleView
        /// <summary>
        /// Scale view of text in percentage of font size
        /// </summary>
        public UnitViewModel SizeUnitLabel { get; set; }
        #endregion ScaleView

        #region CaretPosition
        /// <summary>
        /// Get/set property to indicate the current line
        /// of the cursor in the user interface.
        /// </summary>
        public int Line
        {
            get
            {
                return this.mLine;
            }

            set
            {
                if (this.mLine != value)
                {
                    this.mLine = value;
                    this.RaisePropertyChanged(() => this.Line);
                }
            }
        }

        /// <summary>
        /// Get/set property to indicate the current column
        /// of the cursor in the user interface.
        /// </summary>
        public int Column
        {
            get
            {
                return this.mColumn;
            }

            set
            {
                if (this.mColumn != value)
                {
                    this.mColumn = value;
                    this.RaisePropertyChanged(() => this.Column);
                }
            }
        }
        #endregion CaretPosition

        #region EditorStateProperties
        /// <summary>
        /// Get/set editor carret position
        /// for CTRL-TAB Support http://avalondock.codeplex.com/workitem/15079
        /// </summary>
        public int TextEditorCaretOffset
        {
            get
            {
                return this.mTextEditorCaretOffset;
            }

            set
            {
                if (this.mTextEditorCaretOffset != value)
                {
                    this.mTextEditorCaretOffset = value;
                    this.RaisePropertyChanged(() => this.TextEditorCaretOffset);
                }
            }
        }

        /// <summary>
        /// Get/set editor start of selection
        /// for CTRL-TAB Support http://avalondock.codeplex.com/workitem/15079
        /// </summary>
        public int TextEditorSelectionStart
        {
            get
            {
                return this.mTextEditorSelectionStart;
            }

            set
            {
                if (this.mTextEditorSelectionStart != value)
                {
                    this.mTextEditorSelectionStart = value;
                    this.RaisePropertyChanged(() => this.TextEditorSelectionStart);
                }
            }
        }

        /// <summary>
        /// Get/set editor length of selection
        /// for CTRL-TAB Support http://avalondock.codeplex.com/workitem/15079
        /// </summary>
        public int TextEditorSelectionLength
        {
            get
            {
                return this.mTextEditorSelectionLength;
            }

            set
            {
                if (this.mTextEditorSelectionLength != value)
                {
                    this.mTextEditorSelectionLength = value;
                    this.RaisePropertyChanged(() => this.TextEditorSelectionLength);
                }
            }
        }

        public bool TextEditorIsRectangularSelection
        {
            get
            {
                return this.mTextEditorIsRectangularSelection;
            }

            set
            {
                if (this.mTextEditorIsRectangularSelection != value)
                {
                    this.mTextEditorIsRectangularSelection = value;
                    this.RaisePropertyChanged(() => this.TextEditorIsRectangularSelection);
                }
            }
        }

        #region EditorScrollOffsetXY
        /// <summary>
        /// Current editor view scroll X position
        /// </summary>
        public double TextEditorScrollOffsetX
        {
            get
            {
                return this.mTextEditorScrollOffsetX;
            }

            set
            {
                if (this.mTextEditorScrollOffsetX != value)
                {
                    this.mTextEditorScrollOffsetX = value;
                    this.RaisePropertyChanged(() => this.TextEditorScrollOffsetX);
                }
            }
        }

        /// <summary>
        /// Current editor view scroll Y position
        /// </summary>
        public double TextEditorScrollOffsetY
        {
            get
            {
                return this.mTextEditorScrollOffsetY;
            }

            set
            {
                if (this.mTextEditorScrollOffsetY != value)
                {
                    this.mTextEditorScrollOffsetY = value;
                    this.RaisePropertyChanged(() => this.TextEditorScrollOffsetY);
                }
            }
        }
        #endregion EditorScrollOffsetXY
        #endregion EditorStateProperties

        #region TxtControl
        public TextBoxController TxtControl
        {
            get
            {
                return this.mTxtControl;
            }

            private set
            {
                if (this.mTxtControl != value)
                {
                    this.mTxtControl = value;
                    this.RaisePropertyChanged(() => this.TxtControl);
                }
            }
        }
        #endregion TxtControl

        #region IEditorInterface
        public string Text
        {
            get
            {
                if (this.Document == null)
                    return string.Empty;

                return this.Document.Text;
            }
        }

        public int SelectionStart
        {
            get
            {
                int start = 0, length = 0;
                bool IsRectSelect = false;

                if (this.TxtControl != null)
                    this.TxtControl.CurrentSelection(out start, out length, out IsRectSelect);

                return start;
            }
        }

        public int SelectionLength
        {
            get
            {
                int start = 0, length = 0;
                bool IsRectSelect = false;

                if (this.TxtControl != null)
                    this.TxtControl.CurrentSelection(out start, out length, out IsRectSelect);

                return length;
            }
        }

        /// <summary>
        /// Selects the specified portion of Text and scrolls that part into view.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        public void Select(int start, int length)
        {
            if (this.TxtControl != null)
                this.TxtControl.SelectText(start, length);
        }

        public void Replace(int start, int length, string ReplaceWith)
        {
            if (this.Document != null)
                this.Document.Replace(start, length, ReplaceWith);
        }

        /// <summary>
        /// This method is called before a replace all operation.
        /// </summary>
        public void BeginChange()
        {
            if (this.TxtControl != null)
                this.TxtControl.BeginChange();
        }

        /// <summary>
        /// This method is called after a replace all operation.
        /// </summary>
        public void EndChange()
        {
            if (this.TxtControl != null)
                this.TxtControl.EndChange();
        }
        #endregion IEditorInterface
        #endregion properties

        #region methods
        public static IFileBaseViewModel CreateNewDocument(IDocumentModel documentModel)
        {
            return new EdiViewModel(documentModel);
        }

        #region LoadFile
        /// <summary>
        /// Load an Edi text editor file based on an <seealso cref="IDocumentModel"/>
        /// representation and a <seealso cref="ISettingsManager"/> instance.
        /// </summary>
        /// <param name="dm"></param>
        /// <param name="o">Should point to a <seealso cref="ISettingsManager"/> instance.</param>
        /// <returns></returns>
        public static EdiViewModel LoadFile(IDocumentModel dm,
                                                                                object o)
        {
            return EdiViewModel.LoadFile(dm, o as ISettingsManager);
        }

        /// <summary>
        /// Load a files contents into the viewmodel for viewing and editing.
        /// </summary>
        /// <param name="dm"></param>
        /// <param name="closeOnErrorWithoutMessage"></param>
        /// <returns></returns>
        public static EdiViewModel LoadFile(IDocumentModel dm,
                                                                                ISettingsManager settings,
                                                                                bool closeOnErrorWithoutMessage = false)
        {
            EdiViewModel vm = new EdiViewModel();
            vm.InitInstance(settings.SettingData);
            vm.FilePath = dm.FileNamePath;
            vm.CloseOnErrorWithoutMessage = closeOnErrorWithoutMessage;

            vm.LoadFileAsync(vm.FilePath);
            ////vm.OpenFile(filePath);   // Non-async file open version

            return vm;
        }

        /// <summary>
        /// Attempt to open a file and load it into the viewmodel if it exists.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>True if file exists and was succesfully loaded. Otherwise false.</returns>
        protected bool OpenFile(string filePath)
        {
            try
            {
                var isReal = File.Exists(filePath);
                this.mDocumentModel.SetFileNamePath(filePath, isReal);

                if (this.IsFilePathReal == true)
                {
                    this.mDocumentModel.SetIsReal(this.IsFilePathReal);
                    this.FilePath = filePath;
                    this.ContentId = this.mFilePath;
                    this.IsDirty = false; // Mark document loaded from persistence as unedited copy (display without dirty mark '*' in name)

                    // Check file attributes and set to read-only if file attributes indicate that
                    if ((System.IO.File.GetAttributes(filePath) & FileAttributes.ReadOnly) != 0)
                    {
                        this.IsReadOnly = true;
                        this.IsReadOnlyReason = Edi.Util.Local.Strings.STR_FILE_READONLY_REASON_NO_WRITE_PERMISSION;
                    }

                    try
                    {
                        using (FileStream fs = new FileStream(this.mFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            using (StreamReader reader = FileReader.OpenStream(fs, Encoding.UTF8))
                            {
                                TextDocument doc = new TextDocument(reader.ReadToEnd());
                                doc.SetOwnerThread(Application.Current.Dispatcher.Thread);
                                Application.Current.Dispatcher.BeginInvoke(
                                            new Action(
                                                    delegate
                                                    {
                                                        this.Document = doc;
                                                    }), DispatcherPriority.Normal);

                                this.FileEncoding = reader.CurrentEncoding; // assign encoding after ReadToEnd() so that the StreamReader can autodetect the encoding
                            }
                        }

                        // Set the correct actualy state of the model into the viewmodel
                        // to either allow editing or continue to block editing depending on what the model says
                        this.IsReadOnly = this.mDocumentModel.IsReadonly;

                        this.State = DocumentState.IsEditing;
                    }
                    catch                 // File may be blocked by another process
                    {                    // Try read-only shared method and set file access to read-only
                        try
                        {
                            this.IsReadOnly = true;  // Open file in readonly mode
                            this.IsReadOnlyReason = Edi.Util.Local.Strings.STR_FILE_READONLY_REASON_USED_BY_OTHER_PROCESS;

                            using (FileStream fs = new FileStream(this.mFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                using (StreamReader reader = FileReader.OpenStream(fs, Encoding.UTF8))
                                {
                                    TextDocument doc = new TextDocument(reader.ReadToEnd());
                                    doc.SetOwnerThread(Application.Current.Dispatcher.Thread);
                                    Application.Current.Dispatcher.BeginInvoke(
                                                new Action(
                                                        delegate
                                                        {
                                                            this.Document = doc;
                                                        }), DispatcherPriority.Normal);

                                    this.FileEncoding = reader.CurrentEncoding; // assign encoding after ReadToEnd() so that the StreamReader can autodetect the encoding
                                }
                            }

                            this.State = DocumentState.IsEditing;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Util.Local.Strings.STR_FILE_OPEN_ERROR_MSG_CAPTION, ex);
                        }
                    }
                }
                else
                    throw new FileNotFoundException(filePath);   // File does not exist
            }
            catch (Exception exp)
            {
                throw new Exception(Util.Local.Strings.STR_FILE_OPEN_ERROR_MSG_CAPTION, exp);
            }

            return true;
        }
        #endregion LoadFile

        /// <summary>
        /// Initialize viewmodel with data that should not be initialized in constructor
        /// but is usually necessary after creating default object.
        /// </summary>
        /// <param name="SettingData"></param>
        public void InitInstance(Options SettingData)
        {
            if (SettingData != null)
            {
                this.FilePath = this.GetDefaultFileNewName(SettingData.FileNewDefaultFileName,
                                                                                                     SettingData.FileNewDefaultFileExtension);

                this.TextOptions = new ICSharpCode.AvalonEdit.TextEditorOptions(SettingData.EditorTextOptions);
                this.HighlightingDefinition = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(this.mFilePath));
            }

            // TODO: This should be moved into Settings project?
            this.InsertBlocks = new ObservableCollection<BlockDefinition>(SettingsView.Config.ViewModels.ConfigViewModel.GetDefaultBlockDefinitions());

            this.WordWrap = SettingData.WordWarpText;
        }

        /// <summary>
        /// Reloads/Refresh's the current document content with the content
        /// of the from disc.
        /// </summary>
        public override void ReOpen()
        {
            base.ReOpen();

            this.LoadFileAsync(this.FilePath);
        }

        /// <summary>
        /// Can be called when executing File>New for this document type.
        /// The method changes all document states such that users can start
        /// editing and be creating new content.
        /// </summary>
        public void CreateNewDocument()
        {
            this.Document = new TextDocument();
            this.State = DocumentState.IsEditing;
            this.IsReadOnly = false;
        }

        /// <summary>
        /// Export the current content of the text editor as HTML.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <param name="filePath"></param>
        public void ExportToHTML(string defaultFileName = "",
                                                         bool showLineNumbers = true,
                                                         bool alternateLineBackground = true)
        {
            string ExportHTMLFileFilter = Edi.Util.Local.Strings.STR_ExportHTMLFileFilter;

            // Create and configure SaveFileDialog.
            FileDialog dlg = new SaveFileDialog()
            {
                ValidateNames = true,
                AddExtension = true,
                Filter = ExportHTMLFileFilter,
                FileName = defaultFileName
            };

            // Show dialog; return if canceled.
            if (!dlg.ShowDialog(Application.Current.MainWindow).GetValueOrDefault())
                return;

            defaultFileName = dlg.FileName;

            IHighlightingDefinition highlightDefinition = this.HighlightingDefinition;

            HtmlWriter w = new HtmlWriter()
            {
                ShowLineNumbers = showLineNumbers,
                AlternateLineBackground = alternateLineBackground
            };
            string html = w.GenerateHtml(this.Text, highlightDefinition);
            File.WriteAllText(defaultFileName, "<html><body>" + html + "</body></html>");

            System.Diagnostics.Process.Start(defaultFileName); // view in browser
        }

        /// <summary>
        /// Get the path of the file or empty string if file does not exists on disk.
        /// </summary>
        /// <returns></returns>
        override public string GetFilePath()
        {
            try
            {
                if (System.IO.File.Exists(this.FilePath))
                    return System.IO.Path.GetDirectoryName(this.FilePath);
            }
            catch
            {
            }

            return string.Empty;
        }

        /// <summary>
        /// Switch off text highlighting to display the current document in regular
        /// black and white or white and black foreground/background colors.
        /// </summary>
        public void DisableHighlighting()
        {
            this.HighlightingDefinition = null;
        }

        /// <summary>
        /// Increase the document counter for new documents created via New command.
        /// </summary>
        public void IncreaseNewCounter()
        {
            EdiViewModel.iNewFileCounter += 1;
        }

        /// <summary>
        /// Set a file specific value to determine whether file
        /// watching is enabled/disabled for this file.
        /// </summary>
        /// <param name="IsEnabled"></param>
        /// <returns></returns>
        public bool EnableDocumentFileWatcher(bool IsEnabled)
        {
            // Activate file watcher for this document
            return this.mDocumentModel.EnableDocumentFileWatcher(true);
        }

        #region ScaleView methods
        /// <summary>
        /// Initialize scale view of content to indicated value and unit.
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="defaultValue"></param>
        public void InitScaleView(ZoomUnit unit, double defaultValue)
        {
            var unitList = new ObservableCollection<ListItem>(Options.GenerateScreenUnitList());

            this.SizeUnitLabel = new UnitViewModel(unitList, new ScreenConverter(), (int)unit, defaultValue);
        }
        #endregion ScaleView methods

        private bool CommandCancelProcessingCanExecute(object obj)
        {
            return (this.mAsyncProcessor != null);
        }

        private object CommandCancelProcessingExecuted(object arg)
        {
            if (this.mAsyncProcessor != null)
                this.mAsyncProcessor.Cancel();

            return null;
        }

        /// <summary>
        /// Load a file asynchronously to display its content through this ViewModel.
        /// http://yalvlib.codeplex.com/SourceControl/latest#src/YalvLib/ViewModel/YalvViewModel.cs
        /// </summary>
        /// <param name="path">file path</param>
        private void LoadFileAsync(string path)
        {
            if (this.mAsyncProcessor != null)
            {
                if (Msg.Show(
                        "An operation is currently in progress. Would you like to cancel the current process?",
                        "Processing...",
                        MsgBoxButtons.YesNo, MsgBoxImage.Question, MsgBoxResult.No) == MsgBoxResult.Yes)
                {
                    this.mAsyncProcessor.Cancel();
                }
            }

            this.mAsyncProcessor = new FileLoader();

            this.mAsyncProcessor.ProcessingResultEvent += FileLoaderLoadResultEvent;

            this.State = DocumentState.IsLoading;

            this.mAsyncProcessor.ExecuteAsynchronously(delegate
                                                                                                {
                                                                                                    try
                                                                                                    {
                                                                                                        this.OpenFile(path);

                                                                                                    }
                                                                                                    finally
                                                                                                    {
                                                                                                        // Set this to invalid if viewmodel still things its loading...
                                                                                                        if (this.State == DocumentState.IsLoading)
                                                                                                            this.State = DocumentState.IsInvalid;
                                                                                                    }
                                                                                                },
                                                                                                true);
        }

        /// <summary>
        /// Method is executed when the background process finishes and returns here
        /// because it was cancelled or is done processing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileLoaderLoadResultEvent(object sender, ResultEvent e)
        {
            this.mAsyncProcessor.ProcessingResultEvent -= FileLoaderLoadResultEvent;
            this.mAsyncProcessor = null;

            CommandManager.InvalidateRequerySuggested();

            // close documents automatically without message when re-loading on startup
            if (this.State == DocumentState.IsInvalid && this.CloseOnErrorWithoutMessage == true)
            {
                this.OnClose();
                return;
            }

            // Continue processing in parent of this viewmodel if there is any such requested
            base.FireFileProcessingResultEvent(e, TypeOfResult.FileLoad);
        }

        /// <summary>
        /// Generates the default file name (with counter and extension)
        /// for File>New text document.
        /// </summary>
        /// <returns></returns>
        private string GetDefaultFileNewName(string defaultFileName = null,
                                                                                 string defaultFileExtension = null)
        {
            if (defaultFileName != null)
                this.mDefaultFileName = defaultFileName;

            if (defaultFileExtension != null)
                this.mDefaultFileType = defaultFileExtension;

            return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}",
                            this.mDefaultFileName,
                            (EdiViewModel.iNewFileCounter == 0 ? string.Empty : " " + EdiViewModel.iNewFileCounter.ToString()),
                            this.mDefaultFileType);
        }
        #endregion methods
    }
}
