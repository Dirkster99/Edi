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
  using MiniUML.Framework;
  using MiniUML.Framework.Command;
  using MiniUML.Model.Model;
  using MiniUML.Model.ViewModels.Shapes;
  using UnitComboLib.Models.Unit;
  using UnitComboLib.Models.Unit.Screen;
  using UnitComboLib.ViewModels;

  /// <summary>
  /// Class to manage XML text for editor/view displayed in the drawing canvas.
  /// </summary>
  public class XmlViewModel : MiniUML.Framework.BaseViewModel
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
      this.mIsExpanded = false;
      this.mIsCanvasExpanded = true;

      this.mTextEditorHeight = 220;

      // Store a reference to the parent view model.
      this.DocumentViewModel = documentViewModel;

      this.SizeUnitLabel = new UnitViewModel(this.GenerateScreenUnitList(), new ScreenConverter(), 0);

      this.TxtControl = new TextBoxController();

      this.mDocument = new TextDocument();

      this.TextEditorSelectionStart = 0;
      this.TextEditorSelectionLength = 0;

      // Set XML Highlighting for XML split view part of the UML document viewer
      this.mHighlightingDefinition = HighlightingManager.Instance.GetDefinitionByExtension(".xml");
    }
    #endregion constructor

    #region properties
    #region AvalonEdit properties
    #region IsReadOnly
    public bool IsReadOnly
    {
      get
      {
        return this.mIsReadOnly;
      }

      protected set
      {
        if (this.mIsReadOnly != value)
        {
          this.mIsReadOnly = value;
          this.NotifyPropertyChanged(() => this.IsReadOnly);
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
          this.NotifyPropertyChanged(() => this.Document);
        }
      }
    }
    #endregion

    #region ScaleView
    /// <summary>
    /// Scale view of text in percentage of font size
    /// </summary>
    public UnitViewModel SizeUnitLabel { get; set; }
    #endregion ScaleView

    #region CaretPosition
    // These properties are used to display the current column/line
    // of the cursor in the user interface
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
          this.NotifyPropertyChanged(() => this.Line);
        }
      }
    }

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
          this.NotifyPropertyChanged(() => this.Column);
        }
      }
    }
    #endregion CaretPosition

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
          this.NotifyPropertyChanged(() => this.TxtControl);
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
        return this.mTextEditorCaretOffset;
      }

      set
      {
        if (this.mTextEditorCaretOffset != value)
        {
          this.mTextEditorCaretOffset = value;
          this.NotifyPropertyChanged(() => this.TextEditorCaretOffset);
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
          this.NotifyPropertyChanged(() => this.TextEditorSelectionStart);
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
          this.NotifyPropertyChanged(() => this.TextEditorSelectionLength);
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
          this.NotifyPropertyChanged(() => this.TextEditorIsRectangularSelection);
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
          this.NotifyPropertyChanged(() => this.TextEditorScrollOffsetX);
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
          this.NotifyPropertyChanged(() => this.TextEditorScrollOffsetY);
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
        lock (this.mLockThis)
        {
          return this.mHighlightingDefinition;
        }
      }

      set
      {
        lock (this.mLockThis)
        {
          if (this.mHighlightingDefinition != value)
          {
            this.mHighlightingDefinition = value;

            this.NotifyPropertyChanged(() => this.HighlightingDefinition);
          }
        }
      }
    }

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
          this.NotifyPropertyChanged(() => this.WordWrap);
        }
      }
    }

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
          this.NotifyPropertyChanged(() => this.ShowLineNumbers);
        }
      }
    }

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
          this.NotifyPropertyChanged(() => this.ShowEndOfLine);
        }
      }
    }

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
          this.NotifyPropertyChanged(() => this.ShowSpaces);
        }
      }
    }

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
          this.NotifyPropertyChanged(() => this.ShowTabs);
        }
      }
    }

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
          this.NotifyPropertyChanged(() => this.TextOptions);
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
        return this.mIsDirty;
      }

      set
      {
        if (this.mIsDirty != value)
        {
          this.mIsDirty = value;
          this.NotifyPropertyChanged(() => this.IsDirty);
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
        return this.mXmlStatusMessage;
      }

      set
      {
        if (this.mXmlStatusMessage != value)
        {
          this.mXmlStatusMessage = value;
          this.NotifyPropertyChanged(() => this.XmlStatusMessage);
        }
      }
    }

    public bool prop_DocumentChanged
    {
      get
      {
        return this.mDocumentChanged;
      }

      set
      {
        this.mDocumentChanged = value;

        this.NotifyPropertyChanged(() => this.prop_DocumentChanged);
      }
    }

    public DocumentViewModel DocumentViewModel { get; private set; }

    public bool IsExpanded
    {
      get
      {
        return this.mIsExpanded;
      }

      set
      {
        if (this.mIsExpanded != value)
        {
          this.mIsExpanded = value;
          this.NotifyPropertyChanged(() => this.IsExpanded);
        }
      }
    }

    public bool IsCanvasExpanded
    {
      get
      {
        return this.mIsCanvasExpanded;
      }

      set
      {
        if (this.mIsCanvasExpanded != value)
        {
          this.mIsCanvasExpanded = value;
          this.NotifyPropertyChanged(() => this.mIsCanvasExpanded);
        }
      }
    }

    public double TextEditorHeight
    {
      get
      {
        return this.mTextEditorHeight;
      }

      set
      {
        if (this.mTextEditorHeight != value)
        {
          this.mTextEditorHeight = value;
          this.NotifyPropertyChanged(() => this.TextEditorHeight);
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
        if (this.mUpdateDesignerCommand == null)
          this.mUpdateDesignerCommand = new RelayCommand<object>((p) => this.OnUpdateDesignerCommand_Execute(p),
                                                                 (p) => this.OnUpdateDesignerCommand_CanExecute());

        return this.mUpdateDesignerCommand;
      }
    }

    public ICommand ExpandCollapseEditorCommand
    {
      get
      {
        if (this.mExpandCollapseEditorCommand == null)
          this.mExpandCollapseEditorCommand = new RelayCommand<object>((p) => this.OnExpandCollapseTextEditorCommand_Execute(),
                                                                        (p) => this.OnExpandCollapseTextEditorCommand_CanExecute());

        return this.mExpandCollapseEditorCommand;
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
      this.SizeUnitLabel = new UnitViewModel(this.GenerateScreenUnitList(), new ScreenConverter(), unit, defaultValue);
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
          this.IsCanvasExpanded = false;
          return;
        }
      }

      // Switching the visibility of the canvas ensures that it is measured
      // and siszs adjusted correctly if when it becomes visible again
      this.IsCanvasExpanded = true;

      double height = this.TextEditorHeight - verticalChange;

      if (height < 0)
      {
        height = 0;
        this.IsExpanded = false;
      }
      else
      {
        // Load text for display if not available, yet
        if (this.IsExpanded == false)
          this.OnExpandCollapseTextEditorCommand_Execute();
      }

      this.TextEditorHeight = height;
    }

    #region Update Canvas from XML command
    /// <summary>
    /// Determine whether update on canvas with the content
    /// of the XML editor/view can be processed or not.
    /// </summary>
    /// <returns></returns>
    private bool OnUpdateDesignerCommand_CanExecute()
    {
      return (this.DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready ||
              this.DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Invalid) &&
              (this.IsDirty == true);
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
        string plugin = this.DocumentViewModel.dm_DocumentDataModel.PluginModelName;
        PluginModelBase m = PluginManager.GetPluginModel(plugin);

        // Look-up shape converter
        UmlTypeToStringConverterBase conv = null;
        conv = m.ShapeConverter;

        // Convert Xml document into a list of shapes and page definition
        List<ShapeViewModelBase> coll;
        PageViewModelBase page = conv.ReadDocument(this.Document.Text,
                                                   this.DocumentViewModel.vm_CanvasViewModel, out coll);

        // Apply new page and shape definitions to data model
        this.DocumentViewModel.dm_DocumentDataModel.LoadFileFromCollection(page, coll);
      }
      catch (Exception exp)
      {
        this.XmlStatusMessage = string.Format("The document is in an invalid state: '{0}'.", exp.Message);
        this.DocumentViewModel.dm_DocumentDataModel.State = DataModel.ModelState.Invalid;
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
        if (this.IsExpanded == false)
        {
          this.IsExpanded = true;

          this.Document.Text = this.DocumentViewModel.dm_DocumentDataModel.SaveDocument();
          this.IsDirty = false;
          this.DocumentViewModel.dm_DocumentDataModel.PropertyChanged += this.documentDataModel_PropertyChanged;
        }
        else
        {
          this.IsExpanded = false;
          this.IsCanvasExpanded = true; // Ensure that canvas is visible when text editor is not

          this.Document.Text = string.Empty;
          this.IsDirty = false;
          this.DocumentViewModel.dm_DocumentDataModel.PropertyChanged -= this.documentDataModel_PropertyChanged;
        }
      }
      catch (Exception exp)
      {
        this.XmlStatusMessage = string.Format("The document is in an invalid state: '{0}'.", exp.Message);
        this.DocumentViewModel.dm_DocumentDataModel.State = DataModel.ModelState.Invalid;
      }
    }

    private void documentDataModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      DocumentDataModel dataModel = sender as DocumentDataModel;

      if (dataModel.State == DataModel.ModelState.Invalid && this.prop_DocumentChanged)
        return;

      this.Document.Text = dataModel.SaveDocument();
      this.IsDirty = false;

      this.prop_DocumentChanged = false;
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
