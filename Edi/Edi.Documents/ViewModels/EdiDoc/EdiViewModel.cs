using ICSharpCode.AvalonEdit;

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
    using Core.Interfaces;
    using Core.Interfaces.Documents;
    using Core.Interfaces.Enums;
    using Core.ViewModels.Command;
    using Core.ViewModels.Events;
    using Process;
    using ICSharpCode.AvalonEdit.Document;
    using ICSharpCode.AvalonEdit.Edi.BlockSurround;
    using ICSharpCode.AvalonEdit.Edi.TextBoxControl;
    using ICSharpCode.AvalonEdit.Highlighting;
    using ICSharpCode.AvalonEdit.Utils;
    using Microsoft.Win32;
    using MsgBox;
    using Settings.Interfaces;
    using Settings.ProgramSettings;
    using UnitComboLib.Models.Unit.Screen;
    using UnitComboLib.ViewModels;
    using CommonServiceLocator;

    public interface IDocumentEdi : IFileBaseViewModel
    {
        #region methods
        /// <summary>
        /// Initialize viewmodel with data that should not be initialized in constructor
        /// but is usually necessary after creating default object.
        /// </summary>
        /// <param name="settingData"></param>
        void InitInstance(Options settingData);

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
    public class EdiViewModel : Core.ViewModels.FileBaseViewModel,
                              Dialogs.FindReplace.ViewModel.IEditor,
                                                            IDocumentEdi,
                                                            IDocumentFileWatcher
    {
        #region Fields
        public const string DocumentKey = "EdiTextEditor";
        public const string Description = "Text files";
        public const string FileFilterName = "All Files";
        public const string DefaultFilter = "*";

        private static int _iNewFileCounter;
        private string _mDefaultFileName = Util.Local.Strings.STR_FILE_DEFAULTNAME;
        private string _mDefaultFileType = ".txt";

        private TextDocument _mDocument;
        private ICSharpCode.AvalonEdit.TextEditorOptions _mTextOptions;
        private IHighlightingDefinition _mHighlightingDefinition;

        private string _mFilePath;
        private bool _mIsDirty;

        private readonly object _lockThis = new object();

        private bool _mWordWrap;            // Toggle state command
        private bool _mShowLineNumbers = true;     // Toggle state command
        private Encoding _mFileEncoding = Encoding.UTF8;

        private int _mLine;      // These properties are used to display the current column/line
        private int _mColumn;    // of the cursor in the user interface

        // These properties are used to save and restore the editor state when CTRL+TABing between documents
        private int _mTextEditorCaretOffset;
        private int _mTextEditorSelectionStart;
        private int _mTextEditorSelectionLength;
        private bool _mTextEditorIsRectangularSelection;
        private double _mTextEditorScrollOffsetX;
        private double _mTextEditorScrollOffsetY;

        private TextBoxController _mTxtControl;

        private bool _mIsReadOnly = true;
        private string _mIsReadOnlyReason = string.Empty;

        private FileLoader _mAsyncProcessor;

        RelayCommand<object> _mCloseCommand;
        #endregion Fields

        #region constructor
        /// <summary>
        /// Class constructor from <seealso cref="IDocumentModel"/> parameter.
        /// </summary>
        /// <param name="documentModel"></param>
        public EdiViewModel(IDocumentModel documentModel)
            : this()
        {
            MDocumentModel.SetFileNamePath(documentModel.FileNamePath, documentModel.IsReal);
        }

        /// <summary>
        /// Standard constructor. See also static <seealso cref="LoadFile"/> method
        /// for construction from file saved on disk.
        /// </summary>
        protected EdiViewModel()
            : base(DocumentKey)
        {
            CloseOnErrorWithoutMessage = false;

            // Copy text editor settings from settingsmanager by default
            TextOptions = new ICSharpCode.AvalonEdit.TextEditorOptions();
            WordWrap = false;

            var items = new ObservableCollection<UnitComboLib.Models.ListItem>(Options.GenerateScreenUnitList());
            SizeUnitLabel =
                UnitComboLib.UnitViewModeService.CreateInstance(items,
                                                                new ScreenConverter(),
                                                                0);

            TxtControl = new TextBoxController();

            FilePath = GetDefaultFileNewName();

            IsDirty = false;
            _mHighlightingDefinition = null;

            _mDocument = null; //new TextDocument();

            TextEditorSelectionStart = 0;
            TextEditorSelectionLength = 0;

            InsertBlocks = null;
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
                if (string.IsNullOrEmpty(_mFilePath))
                    return GetDefaultFileNewName();

                return _mFilePath;
            }

            protected set
            {
                if (_mFilePath != value)
                {
                    _mFilePath = value;

                    RaisePropertyChanged(() => FilePath);
                    RaisePropertyChanged(() => FileName);
                    RaisePropertyChanged(() => Title);

                    HighlightingDefinition = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(_mFilePath));
                }
            }
        }
        #endregion

        #region Title
        /// <summary>
        /// Title is the string that is usually displayed - with or without dirty mark '*' - in the docking environment
        /// </summary>
        public override string Title => FileName + (IsDirty ? "*" : string.Empty);

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
                    return GetDefaultFileNewName();

                return Path.GetFileName(FilePath);
            }
        }

        public override Uri IconSource => new Uri("pack://application:,,,/Edi.Themes;component/Images/Documents/document.png", UriKind.RelativeOrAbsolute);

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
                lock (_lockThis)
                {
                    return _mIsReadOnly;
                }
            }

            protected set
            {
                lock (_lockThis)
                {
                    if (_mIsReadOnly != value)
                    {
                        if (value == false)
                            IsReadOnlyReason = string.Empty;

                        _mIsReadOnly = value;
                        RaisePropertyChanged(() => IsReadOnly);
                    }
                }
            }
        }

        public string IsReadOnlyReason
        {
            get { return _mIsReadOnlyReason; }

            protected set
            {
                if (_mIsReadOnlyReason != value)
                {
                    _mIsReadOnlyReason = value;
                    RaisePropertyChanged(() => IsReadOnlyReason);
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
            get { return _mDocument; }

            set
            {
                if (_mDocument != value)
                {
                    _mDocument = value;
                    RaisePropertyChanged(() => Document);
                }
            }
        }
        #endregion

        #region IsDirty
        /// <summary>
        /// IsDirty indicates whether the file currently loaded
        /// in the editor was modified by the user or not.
        /// </summary>
        public override bool IsDirty
        {
            get { return _mIsDirty; }

            set
            {
                if (_mIsDirty != value)
                {
                    _mIsDirty = value;

                    RaisePropertyChanged(() => IsDirty);
                    RaisePropertyChanged(() => Title);
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
        public override bool CanSaveData => true;

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
                lock (_lockThis)
                {
                    return _mHighlightingDefinition;
                }
            }

            set
            {
                lock (_lockThis)
                {
                    if (_mHighlightingDefinition != value)
                    {
                        _mHighlightingDefinition = value;

                        RaisePropertyChanged(() => HighlightingDefinition);
                    }
                }
            }
        }

        /// <summary>
        /// Get/set whether word wrap is currently activated or not.
        /// </summary>
        public bool WordWrap
        {
            get { return _mWordWrap; }

            set
            {
                if (_mWordWrap != value)
                {
                    _mWordWrap = value;
                    RaisePropertyChanged(() => WordWrap);
                }
            }
        }

        /// <summary>
        /// Get/set whether line numbers are currently shown or not.
        /// </summary>
        public bool ShowLineNumbers
        {
            get { return _mShowLineNumbers; }

            set
            {
                if (_mShowLineNumbers != value)
                {
                    _mShowLineNumbers = value;
                    RaisePropertyChanged(() => ShowLineNumbers);
                }
            }
        }

        /// <summary>
        /// Get/set whether the end of each line is currently shown or not.
        /// </summary>
        public bool ShowEndOfLine               // Toggle state command
        {
            get { return TextOptions.ShowEndOfLine; }

            set
            {
                if (TextOptions.ShowEndOfLine != value)
                {
                    TextOptions.ShowEndOfLine = value;
                    RaisePropertyChanged(() => ShowEndOfLine);
                }
            }
        }

        /// <summary>
        /// Get/set whether the spaces are highlighted or not.
        /// </summary>
        public bool ShowSpaces               // Toggle state command
        {
            get { return TextOptions.ShowSpaces; }

            set
            {
                if (TextOptions.ShowSpaces != value)
                {
                    TextOptions.ShowSpaces = value;
                    RaisePropertyChanged(() => ShowSpaces);
                }
            }
        }

        /// <summary>
        /// Get/set whether the tabulator characters are highlighted or not.
        /// </summary>
        public bool ShowTabs               // Toggle state command
        {
            get { return TextOptions.ShowTabs; }

            set
            {
                if (TextOptions.ShowTabs != value)
                {
                    TextOptions.ShowTabs = value;
                    RaisePropertyChanged(() => ShowTabs);
                }
            }
        }

        /// <summary>
        /// Get/Set texteditor options frmo <see cref="AvalonEdit"/> editor as <see cref="TextEditorOptions"/> instance.
        /// </summary>
        public TextEditorOptions TextOptions
        {
            get { return _mTextOptions; }

            set
            {
                if (_mTextOptions != value)
                {
                    _mTextOptions = value;
                    RaisePropertyChanged(() => TextOptions);
                }
            }
        }
        #endregion AvalonEdit properties

        #region SaveCommand SaveAsCommand
        /// <summary>
        /// Indicate whether there is something to save in the document
        /// currently viewed in through this viewmodel.
        /// </summary>
        public override bool CanSave()
        {
            if (Document == null)
                return false;

            return true;
        }

        /// <summary>
        /// Write text content to disk and (re-)set associated properties
        /// </summary>
        /// <param name="filePath"></param>
        public override bool SaveFile(string filePath)
        {
            File.WriteAllText(filePath, Document.Text);

            // Set new file name in viewmodel and model
            FilePath = filePath;
            ContentId = filePath;
            MDocumentModel.SetFileNamePath(filePath, true);

            IsDirty = false;

            return true;
        }

        /// <summary>
        /// Indicate whether there is something to save as ... in the document
        /// currently viewed in through this viewmodel.
        /// </summary>
        /// <returns></returns>
        public override bool CanSaveAs()
        {
            return CanSave();
        }
        #endregion SaveCommand SaveAsCommand

        #region CloseCommand
        /// <summary>
        /// This command cloases a single file. The binding for this is in the AvalonDock LayoutPanel Style.
        /// </summary>
        public override ICommand CloseCommand
        {
            get
            {
                return _mCloseCommand ?? (_mCloseCommand = new RelayCommand<object>((p) => OnClose(),
                           (p) => CanClose()));
            }
        }

        /// <summary>
        /// Determine whether document can be closed or not.
        /// </summary>
        /// <returns></returns>
        public new bool CanClose()
        {
            if (State == DocumentState.IsLoading)
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
            get { return _mFileEncoding; }

            set
            {
                if (!Equals(_mFileEncoding, value))
                {
                    _mFileEncoding = value;
                    RaisePropertyChanged(() => _mFileEncoding);
                }
            }
        }
        #endregion Encoding

        #region ScaleView
        /// <summary>
        /// Scale view of text in percentage of font size
        /// </summary>
        public IUnitViewModel SizeUnitLabel { get; set; }
        #endregion ScaleView

        #region CaretPosition
        /// <summary>
        /// Get/set property to indicate the current line
        /// of the cursor in the user interface.
        /// </summary>
        public int Line
        {
            get { return _mLine; }

            set
            {
                if (_mLine != value)
                {
                    _mLine = value;
                    RaisePropertyChanged(() => Line);
                }
            }
        }

        /// <summary>
        /// Get/set property to indicate the current column
        /// of the cursor in the user interface.
        /// </summary>
        public int Column
        {
            get { return _mColumn; }

            set
            {
                if (_mColumn != value)
                {
                    _mColumn = value;
                    RaisePropertyChanged(() => Column);
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
            get { return _mTextEditorCaretOffset; }

            set
            {
                if (_mTextEditorCaretOffset != value)
                {
                    _mTextEditorCaretOffset = value;
                    RaisePropertyChanged(() => TextEditorCaretOffset);
                }
            }
        }

        /// <summary>
        /// Get/set editor start of selection
        /// for CTRL-TAB Support http://avalondock.codeplex.com/workitem/15079
        /// </summary>
        public int TextEditorSelectionStart
        {
            get { return _mTextEditorSelectionStart; }

            set
            {
                if (_mTextEditorSelectionStart != value)
                {
                    _mTextEditorSelectionStart = value;
                    RaisePropertyChanged(() => TextEditorSelectionStart);
                }
            }
        }

        /// <summary>
        /// Get/set editor length of selection
        /// for CTRL-TAB Support http://avalondock.codeplex.com/workitem/15079
        /// </summary>
        public int TextEditorSelectionLength
        {
            get { return _mTextEditorSelectionLength; }

            set
            {
                if (_mTextEditorSelectionLength != value)
                {
                    _mTextEditorSelectionLength = value;
                    RaisePropertyChanged(() => TextEditorSelectionLength);
                }
            }
        }

        public bool TextEditorIsRectangularSelection
        {
            get { return _mTextEditorIsRectangularSelection; }

            set
            {
                if (_mTextEditorIsRectangularSelection != value)
                {
                    _mTextEditorIsRectangularSelection = value;
                    RaisePropertyChanged(() => TextEditorIsRectangularSelection);
                }
            }
        }

        #region EditorScrollOffsetXY
        /// <summary>
        /// Current editor view scroll X position
        /// </summary>
        public double TextEditorScrollOffsetX
        {
            get { return _mTextEditorScrollOffsetX; }

            set
            {
                if (_mTextEditorScrollOffsetX != value)
                {
                    _mTextEditorScrollOffsetX = value;
                    RaisePropertyChanged(() => TextEditorScrollOffsetX);
                }
            }
        }

        /// <summary>
        /// Current editor view scroll Y position
        /// </summary>
        public double TextEditorScrollOffsetY
        {
            get { return _mTextEditorScrollOffsetY; }

            set
            {
                if (_mTextEditorScrollOffsetY != value)
                {
                    _mTextEditorScrollOffsetY = value;
                    RaisePropertyChanged(() => TextEditorScrollOffsetY);
                }
            }
        }
        #endregion EditorScrollOffsetXY
        #endregion EditorStateProperties

        #region TxtControl
        public TextBoxController TxtControl
        {
            get { return _mTxtControl; }

            private set
            {
                if (_mTxtControl != value)
                {
                    _mTxtControl = value;
                    RaisePropertyChanged(() => TxtControl);
                }
            }
        }
        #endregion TxtControl

        #region IEditorInterface
        public string Text
        {
            get
            {
                if (Document == null)
                    return string.Empty;

                return Document.Text;
            }
        }

        public int SelectionStart
        {
            get
            {
                int start = 0, length = 0;
                bool IsRectSelect = false;

                if (TxtControl != null)
                    TxtControl.CurrentSelection(out start, out length, out IsRectSelect);

                return start;
            }
        }

        public int SelectionLength
        {
            get
            {
                int start = 0, length = 0;
                bool IsRectSelect = false;

                if (TxtControl != null)
                    TxtControl.CurrentSelection(out start, out length, out IsRectSelect);

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
            TxtControl?.SelectText(start, length);
        }

        public void Replace(int start, int length, string replaceWith)
        {
            Document?.Replace(start, length, replaceWith);
        }

        /// <summary>
        /// This method is called before a replace all operation.
        /// </summary>
        public void BeginChange()
        {
            TxtControl?.BeginChange();
        }

        /// <summary>
        /// This method is called after a replace all operation.
        /// </summary>
        public void EndChange()
        {
            TxtControl?.EndChange();
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
            return LoadFile(dm, o as ISettingsManager);
        }

        /// <summary>
        /// Load a files contents into the viewmodel for viewing and editing.
        /// </summary>
        /// <param name="dm"></param>
        /// <param name="settings"></param>
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
                MDocumentModel.SetFileNamePath(filePath, isReal);

                if (IsFilePathReal)
                {
                    MDocumentModel.SetIsReal(IsFilePathReal);
                    FilePath = filePath;
                    ContentId = _mFilePath;
                    IsDirty = false; // Mark document loaded from persistence as unedited copy (display without dirty mark '*' in name)

                    // Check file attributes and set to read-only if file attributes indicate that
                    if ((File.GetAttributes(filePath) & FileAttributes.ReadOnly) != 0)
                    {
                        IsReadOnly = true;
                        IsReadOnlyReason = Util.Local.Strings.STR_FILE_READONLY_REASON_NO_WRITE_PERMISSION;
                    }

                    try
                    {
                        using (FileStream fs = new FileStream(_mFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            using (StreamReader reader = FileReader.OpenStream(fs, Encoding.UTF8))
                            {
                                TextDocument doc = new TextDocument(reader.ReadToEnd());
                                doc.SetOwnerThread(Application.Current.Dispatcher.Thread);
                                Application.Current.Dispatcher.BeginInvoke(
                                            new Action(
                                                    delegate
                                                    {
                                                        Document = doc;
                                                    }), DispatcherPriority.Normal);

                                FileEncoding = reader.CurrentEncoding; // assign encoding after ReadToEnd() so that the StreamReader can autodetect the encoding
                            }
                        }

                        // Set the correct actualy state of the model into the viewmodel
                        // to either allow editing or continue to block editing depending on what the model says
                        IsReadOnly = MDocumentModel.IsReadonly;

                        State = DocumentState.IsEditing;
                    }
                    catch                 // File may be blocked by another process
                    {                    // Try read-only shared method and set file access to read-only
                        try
                        {
                            IsReadOnly = true;  // Open file in readonly mode
                            IsReadOnlyReason = Util.Local.Strings.STR_FILE_READONLY_REASON_USED_BY_OTHER_PROCESS;

                            using (FileStream fs = new FileStream(_mFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                using (StreamReader reader = FileReader.OpenStream(fs, Encoding.UTF8))
                                {
                                    TextDocument doc = new TextDocument(reader.ReadToEnd());
                                    doc.SetOwnerThread(Application.Current.Dispatcher.Thread);
                                    Application.Current.Dispatcher.BeginInvoke(
                                                new Action(
                                                        delegate
                                                        {
                                                            Document = doc;
                                                        }), DispatcherPriority.Normal);

                                    FileEncoding = reader.CurrentEncoding; // assign encoding after ReadToEnd() so that the StreamReader can autodetect the encoding
                                }
                            }

                            State = DocumentState.IsEditing;
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

        /// <inheritdoc />
        /// <summary>
        /// Initialize viewmodel with data that should not be initialized in constructor
        /// but is usually necessary after creating default object.
        /// </summary>
        /// <param name="settingData"></param>
        public void InitInstance(Options settingData)
        {
            if (settingData != null)
            {
                FilePath = GetDefaultFileNewName(settingData.FileNewDefaultFileName,
                                                                                                     settingData.FileNewDefaultFileExtension);

                TextOptions = new ICSharpCode.AvalonEdit.TextEditorOptions(settingData.EditorTextOptions);
                HighlightingDefinition = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(_mFilePath));
            }

            // TODO: This should be moved into Settings project?
            InsertBlocks = new ObservableCollection<BlockDefinition>(SettingsView.Config.ViewModels.ConfigViewModel.GetDefaultBlockDefinitions());

            if (settingData != null) WordWrap = settingData.WordWarpText;
        }

        /// <inheritdoc />
        /// <summary>
        /// Reloads/Refresh's the current document content with the content
        /// of the from disc.
        /// </summary>
        public override void ReOpen()
        {
            base.ReOpen();

            LoadFileAsync(FilePath);
        }

        /// <inheritdoc />
        /// <summary>
        /// Can be called when executing File&gt;New for this document type.
        /// The method changes all document states such that users can start
        /// editing and be creating new content.
        /// </summary>
        public void CreateNewDocument()
        {
            Document = new TextDocument();
            State = DocumentState.IsEditing;
            IsReadOnly = false;
        }

        /// <summary>
        /// Export the current content of the text editor as HTML.
        /// </summary>
        /// <param name="defaultFileName"></param>
        /// <param name="showLineNumbers"></param>
        /// <param name="alternateLineBackground"></param>
        public void ExportToHtml(string defaultFileName = "",
                                                         bool showLineNumbers = true,
                                                         bool alternateLineBackground = true)
        {
            string exportHtmlFileFilter = Util.Local.Strings.STR_ExportHTMLFileFilter;

            // Create and configure SaveFileDialog.
            FileDialog dlg = new SaveFileDialog()
            {
                ValidateNames = true,
                AddExtension = true,
                Filter = exportHtmlFileFilter,
                FileName = defaultFileName
            };

            // Show dialog; return if canceled.
            if (!dlg.ShowDialog(Application.Current.MainWindow).GetValueOrDefault())
                return;

            defaultFileName = dlg.FileName;

            IHighlightingDefinition highlightDefinition = HighlightingDefinition;

            HtmlWriter w = new HtmlWriter()
            {
                ShowLineNumbers = showLineNumbers,
                AlternateLineBackground = alternateLineBackground
            };
            string html = w.GenerateHtml(Text, highlightDefinition);
            File.WriteAllText(defaultFileName, @"<html><body>" + html + @"</body></html>");

            System.Diagnostics.Process.Start(defaultFileName); // view in browser
        }

        /// <inheritdoc />
        /// <summary>
        /// Get the path of the file or empty string if file does not exists on disk.
        /// </summary>
        /// <returns></returns>
        public override string GetFilePath()
        {
            try
            {
                if (File.Exists(FilePath))
                    return Path.GetDirectoryName(FilePath);
            }
            catch
            {
                // ignored
            }

            return string.Empty;
        }

        /// <summary>
        /// Switch off text highlighting to display the current document in regular
        /// black and white or white and black foreground/background colors.
        /// </summary>
        public void DisableHighlighting()
        {
            HighlightingDefinition = null;
        }

        /// <summary>
        /// Increase the document counter for new documents created via New command.
        /// </summary>
        public void IncreaseNewCounter()
        {
            _iNewFileCounter += 1;
        }

        /// <summary>
        /// Set a file specific value to determine whether file
        /// watching is enabled/disabled for this file.
        /// </summary>
        /// <param name="isEnabled"></param>
        /// <returns></returns>
        public bool EnableDocumentFileWatcher(bool isEnabled)
        {
            // Activate file watcher for this document
            return MDocumentModel.EnableDocumentFileWatcher(true);
        }

        #region ScaleView methods
        /// <summary>
        /// Initialize scale view of content to indicated value and unit.
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="defaultValue"></param>
        public void InitScaleView(ZoomUnit unit, double defaultValue)
        {
            var unitList = new ObservableCollection<UnitComboLib.Models.ListItem>(Options.GenerateScreenUnitList());

            SizeUnitLabel =
                UnitComboLib.UnitViewModeService.CreateInstance(unitList,
                                                                new ScreenConverter(),
                                                                (int)unit, defaultValue);
        }
        #endregion ScaleView methods

        /*
        private bool CommandCancelProcessingCanExecute(object obj)
        {
            return (mAsyncProcessor != null);
        }

        private object CommandCancelProcessingExecuted(object arg)
        {
            if (mAsyncProcessor != null)
                mAsyncProcessor.Cancel();

            return null;
        }
        */

        /// <summary>
        /// Load a file asynchronously to display its content through this ViewModel.
        /// http://yalvlib.codeplex.com/SourceControl/latest#src/YalvLib/ViewModel/YalvViewModel.cs
        /// </summary>
        /// <param name="path">file path</param>
        private void LoadFileAsync(string path)
        {
            if (_mAsyncProcessor != null)
            {
                var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
                if (msgBox.Show("An operation is currently in progress. Would you like to cancel the current process?",
                                "Processing...",
                                MsgBoxButtons.YesNo, MsgBoxImage.Question, MsgBoxResult.No) == MsgBoxResult.Yes)
                {
                    _mAsyncProcessor.Cancel();
                }
            }

            _mAsyncProcessor = new FileLoader();

            _mAsyncProcessor.ProcessingResultEvent += FileLoaderLoadResultEvent;

            State = DocumentState.IsLoading;

            _mAsyncProcessor.ExecuteAsynchronously(delegate
                                                                                                {
                                                                                                    try
                                                                                                    {
                                                                                                        OpenFile(path);

                                                                                                    }
                                                                                                    finally
                                                                                                    {
                                                                                                        // Set this to invalid if viewmodel still things its loading...
                                                                                                        if (State == DocumentState.IsLoading)
                                                                                                            State = DocumentState.IsInvalid;
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
            _mAsyncProcessor.ProcessingResultEvent -= FileLoaderLoadResultEvent;
            _mAsyncProcessor = null;

            CommandManager.InvalidateRequerySuggested();

            // close documents automatically without message when re-loading on startup
            if (State == DocumentState.IsInvalid && CloseOnErrorWithoutMessage)
            {
                OnClose();
                return;
            }

            // Continue processing in parent of this viewmodel if there is any such requested
            FireFileProcessingResultEvent(e, TypeOfResult.FileLoad);
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
                _mDefaultFileName = defaultFileName;

            if (defaultFileExtension != null)
                _mDefaultFileType = defaultFileExtension;

            return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}",
                            _mDefaultFileName,
                            (_iNewFileCounter == 0 ? string.Empty : " " + _iNewFileCounter.ToString()),
                            _mDefaultFileType);
        }
        #endregion methods
    }
}
