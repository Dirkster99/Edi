namespace MiniUML.Model.ViewModels.Document
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Windows.Input;
  using ICSharpCode.AvalonEdit.Document;
  using ICSharpCode.AvalonEdit.Edi.TextBoxControl;
  using ICSharpCode.AvalonEdit.Highlighting;
  using Framework;
  using Framework.Command;
  using Model;
  using Shapes;

    /// <summary>
  /// Class to manage XML text for editor/view displayed in the drawing canvas.
  /// </summary>
  public class XmlViewModel : BaseViewModel
  {
    #region fields
    private RelayCommand<object> mUpdateDesignerCommand = null;
    private RelayCommand<object> mExpandCollapseEditorCommand = null;
    private bool mDocumentChanged;

    private TextDocument mDocument;
    private bool mIsExpanded;
    private bool mIsCanvasExpanded;

    private double mTextEditorHeight;
    private IHighlightingDefinition mHighlightingDefinition;
    private string mXmlStatusMessage = "Ready.";
    private object mLockThis = new object();

    private bool mIsReadOnly = false;
    private int mLine = 0;
    private int mColumn = 0;

    private TextBoxController mTxtControl = null;

    // These properties are used to save and restore the editor state when CTRL+TABing between documents
    private int mTextEditorCaretOffset = 0;
    private int mTextEditorSelectionStart = 0;
    private int mTextEditorSelectionLength = 0;
    private bool mTextEditorIsRectangularSelection = false;
    private double mTextEditorScrollOffsetX = 0;
    private double mTextEditorScrollOffsetY = 0;

    private bool mWordWrap = false;            // Toggle state command
    private bool mShowLineNumbers = true;     // Toggle state command

    private ICSharpCode.AvalonEdit.TextEditorOptions mTextOptions
            = new ICSharpCode.AvalonEdit.TextEditorOptions() { IndentationSize = 2, ConvertTabsToSpaces = true };

    private bool mIsDirty = false;
    #endregion fields

    #region constructor
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="documentViewModel"></param>
    public XmlViewModel(DocumentViewModel documentViewModel)
    {
      mIsExpanded = false;
      mIsCanvasExpanded = true;

      mTextEditorHeight = 220;

      // Store a reference to the parent view model.
      DocumentViewModel = documentViewModel;

      SizeUnitLabel = UnitComboLib.UnitViewModeService.CreateInstance(
        GenerateScreenUnitList(),
        new ScreenConverter(),
        0);

      TxtControl = new TextBoxController();

      mDocument = new TextDocument();

      TextEditorSelectionStart = 0;
      TextEditorSelectionLength = 0;

      // Set XML Highlighting for XML split view part of the UML document viewer
      mHighlightingDefinition = HighlightingManager.Instance.GetDefinitionByExtension(".xml");
    }
    #endregion constructor

    #region properties
    #region AvalonEdit properties
    #region IsReadOnly
    public bool IsReadOnly
    {
      get
      {
        return mIsReadOnly;
      }

      protected set
      {
        if (mIsReadOnly != value)
        {
          mIsReadOnly = value;
          NotifyPropertyChanged(() => IsReadOnly);
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
        return mDocument;
      }

      set
      {
        if (mDocument != value)
        {
          mDocument = value;
          NotifyPropertyChanged(() => Document);
        }
      }
    }
    #endregion

    #region ScaleView
    /// <summary>
    /// Scale view of text in percentage of font size
    /// </summary>
    public IUnitViewModel SizeUnitLabel { get; set; }
    #endregion ScaleView

    #region CaretPosition
    // These properties are used to display the current column/line
    // of the cursor in the user interface
    public int Line
    {
      get
      {
        return mLine;
      }

      set
      {
        if (mLine != value)
        {
          mLine = value;
          NotifyPropertyChanged(() => Line);
        }
      }
    }

    public int Column
    {
      get
      {
        return mColumn;
      }

      set
      {
        if (mColumn != value)
        {
          mColumn = value;
          NotifyPropertyChanged(() => Column);
        }
      }
    }
    #endregion CaretPosition

    #region TxtControl
    public TextBoxController TxtControl
    {
      get
      {
        return mTxtControl;
      }

      private set
      {
        if (mTxtControl != value)
        {
          mTxtControl = value;
          NotifyPropertyChanged(() => TxtControl);
        }
      }
    }
    #endregion TxtControl

    #region EditorStateProperties
    /// <summary>
    /// Get/set editor carret position
    /// for CTRL-TAB Support http://avalondock.codeplex.com/workitem/15079
    /// </summary>
    public int TextEditorCaretOffset
    {
      get
      {
        return mTextEditorCaretOffset;
      }

      set
      {
        if (mTextEditorCaretOffset != value)
        {
          mTextEditorCaretOffset = value;
          NotifyPropertyChanged(() => TextEditorCaretOffset);
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
        return mTextEditorSelectionStart;
      }

      set
      {
        if (mTextEditorSelectionStart != value)
        {
          mTextEditorSelectionStart = value;
          NotifyPropertyChanged(() => TextEditorSelectionStart);
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
        return mTextEditorSelectionLength;
      }

      set
      {
        if (mTextEditorSelectionLength != value)
        {
          mTextEditorSelectionLength = value;
          NotifyPropertyChanged(() => TextEditorSelectionLength);
        }
      }
    }

    public bool TextEditorIsRectangularSelection
    {
      get
      {
        return mTextEditorIsRectangularSelection;
      }

      set
      {
        if (mTextEditorIsRectangularSelection != value)
        {
          mTextEditorIsRectangularSelection = value;
          NotifyPropertyChanged(() => TextEditorIsRectangularSelection);
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
        return mTextEditorScrollOffsetX;
      }

      set
      {
        if (mTextEditorScrollOffsetX != value)
        {
          mTextEditorScrollOffsetX = value;
          NotifyPropertyChanged(() => TextEditorScrollOffsetX);
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
        return mTextEditorScrollOffsetY;
      }

      set
      {
        if (mTextEditorScrollOffsetY != value)
        {
          mTextEditorScrollOffsetY = value;
          NotifyPropertyChanged(() => TextEditorScrollOffsetY);
        }
      }
    }
    #endregion EditorScrollOffsetXY
    #endregion EditorStateProperties

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
        lock (mLockThis)
        {
          return mHighlightingDefinition;
        }
      }

      set
      {
        lock (mLockThis)
        {
          if (mHighlightingDefinition != value)
          {
            mHighlightingDefinition = value;

            NotifyPropertyChanged(() => HighlightingDefinition);
          }
        }
      }
    }

    public bool WordWrap
    {
      get
      {
        return mWordWrap;
      }

      set
      {
        if (mWordWrap != value)
        {
          mWordWrap = value;
          NotifyPropertyChanged(() => WordWrap);
        }
      }
    }

    public bool ShowLineNumbers
    {
      get
      {
        return mShowLineNumbers;
      }

      set
      {
        if (mShowLineNumbers != value)
        {
          mShowLineNumbers = value;
          NotifyPropertyChanged(() => ShowLineNumbers);
        }
      }
    }

    public bool ShowEndOfLine               // Toggle state command
    {
      get
      {
        return TextOptions.ShowEndOfLine;
      }

      set
      {
        if (TextOptions.ShowEndOfLine != value)
        {
          TextOptions.ShowEndOfLine = value;
          NotifyPropertyChanged(() => ShowEndOfLine);
        }
      }
    }

    public bool ShowSpaces               // Toggle state command
    {
      get
      {
        return TextOptions.ShowSpaces;
      }

      set
      {
        if (TextOptions.ShowSpaces != value)
        {
          TextOptions.ShowSpaces = value;
          NotifyPropertyChanged(() => ShowSpaces);
        }
      }
    }

    public bool ShowTabs               // Toggle state command
    {
      get
      {
        return TextOptions.ShowTabs;
      }

      set
      {
        if (TextOptions.ShowTabs != value)
        {
          TextOptions.ShowTabs = value;
          NotifyPropertyChanged(() => ShowTabs);
        }
      }
    }

    public ICSharpCode.AvalonEdit.TextEditorOptions TextOptions
    {
      get
      {
        return mTextOptions;
      }

      set
      {
        if (mTextOptions != value)
        {
          mTextOptions = value;
          NotifyPropertyChanged(() => TextOptions);
        }
      }
    }
    #endregion AvalonEdit properties

    #region IsDirty
    /// <summary>
    /// Get whether the current information was edit and needs to be saved or not.
    /// </summary>
    public bool IsDirty
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
          NotifyPropertyChanged(() => IsDirty);
        }
      }
    }
    #endregion IsDirty

    /// <summary>
    /// Get an error message to show in UI when parsing XML text failed.
    /// </summary>
    public string XmlStatusMessage
    {
      get
      {
        return mXmlStatusMessage;
      }

      set
      {
        if (mXmlStatusMessage != value)
        {
          mXmlStatusMessage = value;
          NotifyPropertyChanged(() => XmlStatusMessage);
        }
      }
    }

    public bool prop_DocumentChanged
    {
      get
      {
        return mDocumentChanged;
      }

      set
      {
        mDocumentChanged = value;

        NotifyPropertyChanged(() => prop_DocumentChanged);
      }
    }

    public DocumentViewModel DocumentViewModel { get; private set; }

    public bool IsExpanded
    {
      get
      {
        return mIsExpanded;
      }

      set
      {
        if (mIsExpanded != value)
        {
          mIsExpanded = value;
          NotifyPropertyChanged(() => IsExpanded);
        }
      }
    }

    public bool IsCanvasExpanded
    {
      get
      {
        return mIsCanvasExpanded;
      }

      set
      {
        if (mIsCanvasExpanded != value)
        {
          mIsCanvasExpanded = value;
          NotifyPropertyChanged(() => mIsCanvasExpanded);
        }
      }
    }

    public double TextEditorHeight
    {
      get
      {
        return mTextEditorHeight;
      }

      set
      {
        if (mTextEditorHeight != value)
        {
          mTextEditorHeight = value;
          NotifyPropertyChanged(() => TextEditorHeight);
        }
      }
    }

    #region command
    /// <summary>
    /// Update the canvas with text input from XML (in XML editor).
    /// </summary>
    public ICommand UpdateDesignerCommand
    {
      get
      {
        if (mUpdateDesignerCommand == null)
          mUpdateDesignerCommand = new RelayCommand<object>((p) => OnUpdateDesignerCommand_Execute(p),
                                                                 (p) => OnUpdateDesignerCommand_CanExecute());

        return mUpdateDesignerCommand;
      }
    }

    public ICommand ExpandCollapseEditorCommand
    {
      get
      {
        if (mExpandCollapseEditorCommand == null)
          mExpandCollapseEditorCommand = new RelayCommand<object>((p) => OnExpandCollapseTextEditorCommand_Execute(),
                                                                        (p) => OnExpandCollapseTextEditorCommand_CanExecute());

        return mExpandCollapseEditorCommand;
      }
    }
    #endregion command
    #endregion properties

    #region methods
    #region ScaleView methods
    /// <summary>
    /// Initialize scale view of content to indicated value and unit.
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="defaultValue"></param>
    public void InitScaleView(int unit, double defaultValue)
    {
      SizeUnitLabel = UnitComboLib.UnitViewModeService.CreateInstance(
                GenerateScreenUnitList(),
                new ScreenConverter(),
                unit,
                defaultValue);
    }
    #endregion ScaleView methods
    
    /// <summary>
    /// Implementation of drag delta event, which occurs
    /// when dragging the grid splitter between text editor and canvas
    /// 
    ///     Canvas
    ///  - - - - - - - (GridSplitter)
    ///   Text Editor
    /// </summary>
    /// <param name="verticalChange"></param>
    /// <param name="canvasActualHeight"></param>
    public void GridSplitter_DragDelta(double verticalChange, double canvasActualHeight)
    {
      if (verticalChange <= 0)
      {
        if (canvasActualHeight <= 0)
        {
          // Make canvas invisible if its height is zero or below
          IsCanvasExpanded = false;
          return;
        }
      }

      // Switching the visibility of the canvas ensures that it is measured
      // and siszs adjusted correctly if when it becomes visible again
      IsCanvasExpanded = true;

      double height = TextEditorHeight - verticalChange;

      if (height < 0)
      {
        height = 0;
        IsExpanded = false;
      }
      else
      {
        // Load text for display if not available, yet
        if (IsExpanded == false)
          OnExpandCollapseTextEditorCommand_Execute();
      }

      TextEditorHeight = height;
    }

    #region Update Canvas from XML command
    /// <summary>
    /// Determine whether update on canvas with the content
    /// of the XML editor/view can be processed or not.
    /// </summary>
    /// <returns></returns>
    private bool OnUpdateDesignerCommand_CanExecute()
    {
      return (DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready ||
              DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Invalid) &&
              IsDirty;
    }

    /// <summary>
    /// Update the canvas with the content of the XML editor/view
    /// (updates from canvas to viewmodel are initiated in the <seealso cref="XmlView"/> class).
    /// </summary>
    /// <param name="p"></param>
    private void OnUpdateDesignerCommand_Execute(object p)
    {
      try
      {
        // Look-up plugin model
        string plugin = DocumentViewModel.dm_DocumentDataModel.PluginModelName;
        PluginModelBase m = PluginManager.GetPluginModel(plugin);

        // Look-up shape converter
        UmlTypeToStringConverterBase conv = null;
        conv = m.ShapeConverter;

        // Convert Xml document into a list of shapes and page definition
        List<ShapeViewModelBase> coll;
        PageViewModelBase page = conv.ReadDocument(Document.Text,
                                                   DocumentViewModel.vm_CanvasViewModel, out coll);

        // Apply new page and shape definitions to data model
        DocumentViewModel.dm_DocumentDataModel.LoadFileFromCollection(page, coll);
      }
      catch (Exception exp)
      {
        XmlStatusMessage = string.Format("The document is in an invalid state: '{0}'.", exp.Message);
        DocumentViewModel.dm_DocumentDataModel.State = DataModel.ModelState.Invalid;
        //// TODO XXX this.DocumentViewModel.dm_DocumentDataModel.DocumentRoot = new XElement("InvalidDocument");
      }
    }
    #endregion Update Canvas from XML command

    #region Expand Collapse text editor command
    private bool OnExpandCollapseTextEditorCommand_CanExecute()
    {
      return true;
    }

    /// <summary>
    /// Command implementation of text editor collapse/expand command.
    /// </summary>
    private void OnExpandCollapseTextEditorCommand_Execute()
    {
      try
      {
        if (IsExpanded == false)
        {
          IsExpanded = true;

          Document.Text = DocumentViewModel.dm_DocumentDataModel.SaveDocument();
          IsDirty = false;
          DocumentViewModel.dm_DocumentDataModel.PropertyChanged += documentDataModel_PropertyChanged;
        }
        else
        {
          IsExpanded = false;
          IsCanvasExpanded = true; // Ensure that canvas is visible when text editor is not

          Document.Text = string.Empty;
          IsDirty = false;
          DocumentViewModel.dm_DocumentDataModel.PropertyChanged -= documentDataModel_PropertyChanged;
        }
      }
      catch (Exception exp)
      {
        XmlStatusMessage = string.Format("The document is in an invalid state: '{0}'.", exp.Message);
        DocumentViewModel.dm_DocumentDataModel.State = DataModel.ModelState.Invalid;
      }
    }

    private void documentDataModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      DocumentDataModel dataModel = sender as DocumentDataModel;

      if (dataModel.State == DataModel.ModelState.Invalid && prop_DocumentChanged)
        return;

      Document.Text = dataModel.SaveDocument();
      IsDirty = false;

      prop_DocumentChanged = false;
    }
    #endregion Expand Collapse text editor command

    #region ScaleView methods
    /// <summary>
    /// Initialize Scale View with useful units in percent and font point size
    /// </summary>
    /// <returns></returns>
    private ObservableCollection<UnitComboLib.Models.ListItem> GenerateScreenUnitList()
    {
      ObservableCollection<UnitComboLib.Models.ListItem> unitList = new ObservableCollection<UnitComboLib.Models.ListItem>();

      var percentDefaults = new ObservableCollection<string>() { "25", "50", "75", "100", "125", "150", "175", "200", "300", "400", "500" };
      var pointsDefaults = new ObservableCollection<string>() { "3", "6", "8", "9", "10", "12", "14", "16", "18", "20", "24", "26", "32", "48", "60" };

      unitList.Add(new UnitComboLib.Models.ListItem(Itemkey.ScreenPercent, "percent", "%", percentDefaults));
      unitList.Add(new UnitComboLib.Models.ListItem(Itemkey.ScreenFontPoints, "point", "pt", pointsDefaults));

      return unitList;
    }
    #endregion ScaleView methods
    #endregion methods
  }
}
