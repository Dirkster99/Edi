namespace ICSharpCode.AvalonEdit.Edi
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using System.Windows;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Windows.Threading;
  using BracketRenderer;
  using BlockSurround;
  using Editing;
  using Rendering;
  using Utils;

  /// <summary>
  /// </summary>
  public partial class EdiTextEditor : TextEditor
  {
    #region fields
    static readonly List<CommandBinding> CmdBindings = new List<CommandBinding>();
    ////static readonly List<InputBinding> InputBindings = new List<InputBinding>();

    // Highlight opening and closing brackets in editor
    BracketHighlightRenderer mBracketRenderer = null;
    EdiBracketSearcher FindBrackets = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// Static class constructor to register style key and commands
    /// </summary>
    static EdiTextEditor()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(EdiTextEditor), new FrameworkPropertyMetadata(typeof(EdiTextEditor)));
      FocusableProperty.OverrideMetadata(typeof(EdiTextEditor), new FrameworkPropertyMetadata(Boxes.True));

      CmdBindings.Add(new CommandBinding(EdiTextEditorCommands.FoldsCollapseAll, FoldsCollapseAll, FoldsColapseExpandCanExecute));
      CmdBindings.Add(new CommandBinding(EdiTextEditorCommands.FoldsExpandAll, FoldsExpandAll, FoldsColapseExpandCanExecute));
      CmdBindings.Add(new CommandBinding(EdiTextEditorCommands.PrintDocument, PrintDocument, PrintDocumentCanExecute));
    }

    /// <summary>
    /// Class Constructor
    /// (review the OnApplyTemplate() method for most construction routines)
    /// </summary>
    public EdiTextEditor()
      : base()
    {
      // This is default list of viewing scales that is bound by the default XAML skin in EdiTextEditor.xaml
      // Consumers can use other lists by binding to their viewmodel when re-styling the control in the
      // applications resource dictionary (therefore, this list is not localized)
      ScaleList = new ObservableCollection<string>(){ "20 %",
                                                           "50 %",
                                                           "75 %",
                                                           "100 %",
                                                           "125 %",
                                                           "150 %",
                                                           "175 %",
                                                           "200 %",
                                                           "300 %",
                                                           "400 %"};

      // Copy static collection of commands to collection of commands of this instance
      CommandBindings.AddRange(CmdBindings);      
    }
    #endregion constructor

    #region properties
    #region ScaleViewOutput
    /// <summary>
    /// Get/Set viewmodel to display a customizable font size
    /// This is default list of viewing scales that is bound by the default XAML skin in EdiTextEditor.xaml
    /// Consumers can use other lists by binding to their viewmodel when re-styling the control in the
    /// applications resource dictionary
    /// </summary>
    public ObservableCollection<string> ScaleList { get; set; }
    #endregion ScaleViewOutput
    #endregion properties

    #region methods
    /// <summary>
    /// Standard <seealso cref="System.Windows.Controls.Control"/> method to be executed when the
    /// framework applies the XAML definition of the lookless control.
    /// </summary>
		public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      Loaded += new RoutedEventHandler(OnLoaded);
      Unloaded += new RoutedEventHandler(OnUnloaded);

      // Highlight current line in editor (even if editor is not focused) via themable dp-property
      AdjustCurrentLineBackground(EditorCurrentLineBackground);

      // Update highlighting of current line when caret position is changed
      TextArea.Caret.PositionChanged += Caret_PositionChanged;
    }

    /// <summary>
    /// Hock event handlers and restore editor states (if any) or defaults
    /// when the control is fully loaded.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="args"></param>
    private void OnLoaded(object obj, RoutedEventArgs args)
    {
      // Call folding once upon loading so that first run is executed right away
      foldingUpdateTimer_Tick(null, null);

      mFoldingUpdateTimer = new DispatcherTimer();
      mFoldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
      mFoldingUpdateTimer.Tick += foldingUpdateTimer_Tick;
      mFoldingUpdateTimer.Start();

      // Connect CompletionWindow Listners
      try
      {
        EdiTextEditor_OptionChanged(null, null);
        OptionChanged += EdiTextEditor_OptionChanged;
      }
      catch
      {
      }

      try
      {
        Focus();
        ForceCursor = true;

        // Restore cusor position for CTRL-TAB Support http://avalondock.codeplex.com/workitem/15079
        ScrollToHorizontalOffset(EditorScrollOffsetX);
        ScrollToVerticalOffset(EditorScrollOffsetY);
      }
      catch
      {
      }

      try
      {
        if (EditorIsRectangleSelection == true)
        {
          CaretOffset = EditorCaretOffset;

          SelectionStart = EditorSelectionStart;
          SelectionLength = EditorSelectionLength;

          // See OnMouseCaretBoxSelection in Editing\CaretNavigationCommandHandler.cs
          // Convert normal selection to rectangle selection
          TextArea.Selection = new RectangleSelection(TextArea,
                                                                                          TextArea.Selection.StartPosition,
                                                                                          TextArea.Selection.EndPosition);
        }
        else
        {
          CaretOffset = EditorCaretOffset;

          // http://www.codeproject.com/Articles/42490/Using-AvalonEdit-WPF-Text-Editor?msg=4388281#xx4388281xx
          Select(EditorSelectionStart, EditorSelectionLength);
        }
      }
      catch
      {
      }

      // Attach mouse wheel CTRL-key zoom support
      PreviewMouseWheel += new MouseWheelEventHandler(textEditor_PreviewMouseWheel);
      KeyDown += new KeyEventHandler(textEditor_KeyDown);
    }

    /// <summary>
    /// Setup options and parameters that may depend on chnaging options.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EdiTextEditor_OptionChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (Options != null)
      {
        TextArea.TextEntering -= TextEditorTextAreaTextEntering;
        TextArea.TextEntered -= TextEditorTextAreaTextEntered;

        if (Options.EnableCodeCompletion == true)
        {
          TextArea.TextEntering += TextEditorTextAreaTextEntering;
          TextArea.TextEntered += TextEditorTextAreaTextEntered;
        }
      }
    }

    /// <summary>
    /// Add/Remove surrounding tags when pressing a certain key sequence (eg.: Ctrl+1)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void textEditor_KeyDown(object sender, KeyEventArgs e)
    {
      ModifierKeys mod = Keyboard.Modifiers;

      if (InsertBlocks != null)
      {
        IEnumerable<BlockDefinition> sel = InsertBlocks.Where(i => i.Key == e.Key && e.KeyboardDevice.Modifiers == i.Modifier);
      
        if (sel == null)
          return;

        foreach (var item in sel)           // Press 'Ctrl+1' to add remove tags surrounding current selection
        {
          //// if (e.Key == System.Windows.Input.Key.D1 &&
          //// (System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Control) ==
          ////     System.Windows.Input.ModifierKeys.Control)
          {
            try
            {
              // Make sure that this change is in one UNDO/Redo step
              BeginChange();
              AvalonHelper.SurroundSelectionWithBlockComment(this, item);
            }
            catch (Exception exp)
            {
              Console.WriteLine(exp.ToString());
            }
            finally
            {
              // Make sure that this change is in one UNDO/Redo step
              EndChange();
              e.Handled = true;
            }

            return;
          }
        }
      }
    }

    /// <summary>
    /// Unhock event handlers and save editor states (to be recovered later)
    /// when the control is unloaded.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="args"></param>
    private void OnUnloaded(object obj, RoutedEventArgs args)
    {
      if (mFoldingUpdateTimer != null)
        mFoldingUpdateTimer = null;

      TextArea.TextEntering -= TextEditorTextAreaTextEntering;
      TextArea.TextEntered -= TextEditorTextAreaTextEntered;

      // Save cusor position for CTRL-TAB Support http://avalondock.codeplex.com/workitem/15079
      EditorCaretOffset = CaretOffset;
      EditorSelectionStart = SelectionStart;
      EditorSelectionLength = SelectionLength;
      EditorIsRectangleSelection = TextArea.Selection is RectangleSelection;

      // http://stackoverflow.com/questions/11863273/avalonedit-how-to-get-the-top-visible-line
      EditorScrollOffsetX = TextArea.TextView.ScrollOffset.X;
      EditorScrollOffsetY = TextArea.TextView.ScrollOffset.Y;

      // Detach mouse wheel CTRL-key zoom support
      // This does not work when doing mouse zoom and CTRL-TAB between two documents and trying to do mouse zoom???
      PreviewMouseWheel -= textEditor_PreviewMouseWheel;
    }

    /// <summary>
    /// Reset the <seealso cref="SolidColorBrush"/> to be used for highlighting the current editor line.
    /// </summary>
    /// <param name="newValue"></param>
    private void AdjustCurrentLineBackground(SolidColorBrush newValue)
    {
      if (newValue != null)
      {
        HighlightCurrentLineBackgroundRenderer oldRenderer = null;

        // Make sure there is only one of this type of background renderer
        // Otherwise, we might keep adding and WPF keeps drawing them on top of each other
        foreach (var item in TextArea.TextView.BackgroundRenderers)
        {
          if (item != null)
          {
            if (item is HighlightCurrentLineBackgroundRenderer)
            {
              oldRenderer = item as HighlightCurrentLineBackgroundRenderer;
            }
          }
        }

        TextArea.TextView.BackgroundRenderers.Remove(oldRenderer);

        TextArea.TextView.BackgroundRenderers.Add(new HighlightCurrentLineBackgroundRenderer(this, newValue.Clone()));

        // Remove reference to old background renderer instance (if any) and construct BracketRenderer from scratch
        if (mBracketRenderer != null)
        {
          TextArea.TextView.BackgroundRenderers.Remove(mBracketRenderer);
          mBracketRenderer = null;
        }

        mBracketRenderer = new BracketHighlightRenderer(TextArea.TextView);
        TextArea.TextView.BackgroundRenderers.Add(mBracketRenderer);
      }
    }

    /// <summary>
    /// Update Column and Line position properties when caret position is changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Caret_PositionChanged(object sender, EventArgs e)
    {
      // Highlight opening and closing brackets when the carret position changes
      try
      {
        HighlightBrackets(sender, e);
      }
      catch
      {
      }

      TextArea.TextView.InvalidateLayer(KnownLayer.Background); //Update current line highlighting

      if (TextArea != null)
      {
        Column = TextArea.Caret.Column;
        Line = TextArea.Caret.Line;
      }
      else
        Column = Line = 0;
    }

    /// <summary>
    /// Highlights matching brackets.
    /// </summary>
    private void HighlightBrackets(object sender, EventArgs e)
    {
      if (TextArea.Options.EnableHighlightBrackets == true)
      {
        if (FindBrackets == null)
          FindBrackets = new EdiBracketSearcher();

        var bracketSearchResult = FindBrackets.SearchBracket(Document, TextArea.Caret.Offset);
        mBracketRenderer.SetHighlight(bracketSearchResult);
      }
      else
      {
        mBracketRenderer.SetHighlight(null);
      }
    }

    /// <summary>
    /// This method is triggered on a MouseWheel preview event to check if the user
    /// is also holding down the CTRL Key and adjust the current font size if so.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void textEditor_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
      if (Keyboard.Modifiers == ModifierKeys.Control)
      {
        double fontSize = FontSize + e.Delta / 25.0;

        if (fontSize < 6)
          FontSize = 6;
        else
        {
          if (fontSize > 200)
            FontSize = 200;
          else
            FontSize = fontSize;
        }

        e.Handled = true;
      }
    }
    #endregion methods
  }
}
