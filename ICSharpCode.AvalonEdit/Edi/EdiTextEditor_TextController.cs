namespace ICSharpCode.AvalonEdit.Edi
{
  using System;
  using System.Collections.Generic;
  using System.Windows;

  using ICSharpCode.AvalonEdit;
  using ICSharpCode.AvalonEdit.Document;
  using TextBoxControl;
  using System.Windows.Controls;

  public partial class EdiTextEditor : TextEditor
  {
    #region ITextBoxControllerFields
    private static readonly Dictionary<ITextBoxController, TextEditor> elements = new Dictionary<ITextBoxController, TextEditor>();

    private static readonly DependencyProperty TextBoxControllerProperty =
                            DependencyProperty.Register(
                              "TextBoxController",
                              typeof(ITextBoxController),
                              typeof(EdiTextEditor),
                              new FrameworkPropertyMetadata(null, EdiTextEditor.OnTextBoxControllerChanged));
    #endregion ITextBoxControllerFields

    #region ITextBoxController_Properties
    /// <summary>
    /// Get method for TextBoxController dependency property is set.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="value"></param>
    public static void SetTextBoxController(UIElement element, ITextBoxController value)
    {
      element.SetValue(EdiTextEditor.TextBoxControllerProperty, value);
    }

    /// <summary>
    /// Set method for TextBoxController dependency property is set.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static ITextBoxController GetTextBoxController(UIElement element)
    {
      return (ITextBoxController)element.GetValue(EdiTextEditor.TextBoxControllerProperty);
    }
    #endregion ITextBoxController_Properties

    #region ITextBoxController_methods
    /// <summary>
    /// Call corresponding on changed method when the depependency property
    /// for this <seealso cref="ITextBoxController"/> is changed.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void OnTextBoxControllerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var fileDoc = d as EdiTextEditor;
      if (fileDoc == null)
        return;
        ////throw new ArgumentNullException("Object of type FileDocument is not available!");

      var txtBox = fileDoc as EdiTextEditor;

      // Remove event handler from old if OldValue is available
      var oldController = e.OldValue as ITextBoxController;
      if (oldController != null)
      {
        elements.Remove(oldController);
        oldController.SelectAll -= SelectAll;
        oldController.Select -= Select;
        oldController.ScrollToLineEvent -= ScrollToLine;
        oldController.CurrentSelectionEvent -= CurrentSelection;
        oldController.BeginChangeEvent -= EdiTextEditor.BeginChange;
        oldController.EndChangeEvent -= EdiTextEditor.EndChange;
        oldController.GetSelectedTextEvent -= EdiTextEditor.GetSelectedText;
      }        

      // Add new eventhandler for each event declared in the interface declaration
      var newController = e.NewValue as ITextBoxController;
      if (newController != null)
      {
        // Sometime the newController is already there but the event handling is not working
        // Remove controller and event handling and install a new one instead.
        TextEditor test;
        if (elements.TryGetValue(newController, out test) == true)
        {
          elements.Remove(newController);

          newController.SelectAll -= EdiTextEditor.SelectAll;
          newController.Select -= EdiTextEditor.Select;
          newController.ScrollToLineEvent -= EdiTextEditor.ScrollToLine;
          newController.CurrentSelectionEvent -= EdiTextEditor.CurrentSelection;
          newController.BeginChangeEvent -= EdiTextEditor.BeginChange;
          newController.EndChangeEvent -= EdiTextEditor.EndChange;
          newController.GetSelectedTextEvent -= EdiTextEditor.GetSelectedText;
        }

        elements.Add(newController, txtBox);
        newController.SelectAll += SelectAll;
        newController.Select += Select;
        newController.ScrollToLineEvent += ScrollToLine;
        newController.CurrentSelectionEvent += CurrentSelection;
        newController.BeginChangeEvent += EdiTextEditor.BeginChange;
        newController.EndChangeEvent += EdiTextEditor.EndChange;
        newController.GetSelectedTextEvent += EdiTextEditor.GetSelectedText;
      }        
    }

    /// <summary>
    /// Select all text in the editor
    /// </summary>
    /// <param name="sender"></param>
    private static void SelectAll(ITextBoxController sender)
    {
      TextEditor element;
      if (!elements.TryGetValue(sender, out element))
        throw new ArgumentException("sender");

      element.Focus();
      element.SelectAll();
    }

    /// <summary>
    /// Select the text in the editor as indicated by <paramref name="start"/>
    /// and <paramref name="length"/>.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="start"></param>
    /// <param name="length"></param>
    private static void Select(ITextBoxController sender, int start, int length)
    {
      TextEditor element;
      if (!elements.TryGetValue(sender, out element))
        throw new ArgumentException("sender");

      // element.Focus();

      element.Select(start, length);
      TextLocation loc = element.Document.GetLocation(start);
      element.ScrollTo(loc.Line, loc.Column);
    }

    /// <summary>
    /// Scroll to a specific line in the currently displayed editor text
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="line"></param>
    private static void ScrollToLine(ITextBoxController sender, int line)
    {
      TextEditor element;
      if (!elements.TryGetValue(sender, out element))
        throw new ArgumentException("sender");

      element.Focus();
      element.ScrollToLine(line);
    }

    /// <summary>
    /// Get the state of the current selection start, end and whether its rectangular or not.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="start"></param>
    /// <param name="length"></param>
    /// <param name="IsRectangularSelection"></param>
    private static void CurrentSelection(ITextBoxController sender,
                                         out int start, out int length, out bool IsRectangularSelection)
    {
      TextEditor element;

      if (!elements.TryGetValue(sender, out element))
        throw new ArgumentException("sender");

      start = element.SelectionStart;
      length = element.SelectionLength;
      IsRectangularSelection = element.TextArea.Selection.EnableVirtualSpace;

      // element.TextArea.Selection = RectangleSelection.Create(element.TextArea, start, length);
    }

    private static void BeginChange(ITextBoxController sender)
    {
      TextEditor element;

      if (!elements.TryGetValue(sender, out element))
        throw new ArgumentException("sender");

      element.BeginChange();
    }

    private static void EndChange(ITextBoxController sender)
    {
      TextEditor element;

      if (!elements.TryGetValue(sender, out element))
        throw new ArgumentException("sender");

      element.EndChange();
    }

    private static void GetSelectedText(ITextBoxController sender, out string selectedText)
    {
      TextEditor element;
      selectedText = string.Empty;

      if (!elements.TryGetValue(sender, out element))
        throw new ArgumentException("sender");

      selectedText = element.SelectedText;
    }
    #endregion ITextBoxController_methods
  }
}
