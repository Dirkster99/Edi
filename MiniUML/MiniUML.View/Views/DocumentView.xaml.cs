namespace MiniUML.View.Views
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using MiniUML.Model.ViewModels.Document;

  /// <summary>
  /// Interaction logic for DocumentView.xaml
  /// </summary>
  public partial class DocumentView : UserControl
  {
    #region constructor
    public DocumentView()
    {
      this.InitializeComponent();
      
      this.DataContextChanged += delegate(object sender, DependencyPropertyChangedEventArgs e)
      {
        DocumentViewModel viewModel = e.NewValue as DocumentViewModel;

        if (viewModel == null)
          return;
        
        // Pass a reference to the Visual representing the document to the view model.
        viewModel.v_CanvasView = this._documentVisual;
      };

      // setup zoom support via mouse wheel
      this._scrollViewer.PreviewMouseWheel += delegate(object sender, MouseWheelEventArgs e)
      {
        if (Keyboard.Modifiers == ModifierKeys.Control)
        {
          this._zoomSlider.Value += e.Delta / 1000.0;
          e.Handled = true;
        }
      };
    }
    #endregion constructor

    #region methods
    /// <summary>
    /// Update zoom display and value when user moved the mouse wheel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _zoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      if (Math.Abs(e.NewValue - 1) < 0.09)
        this._zoomSlider.Value = 1;

      this._zoomTextBlock.Text = (int)(this._zoomSlider.Value * 100) + MiniUML.Framework.Local.Strings.STR_ZOOMSlider_PercentCharacter;
    }

    /// <summary>
    /// Process the event in which the user drags the grid splitter between
    /// canvas view (that displays the actual shapes) and XML editor.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GridSplitter_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
    {

            if (this.DataContext is DocumentViewModel)
            {
                DocumentViewModel d = this.DataContext as DocumentViewModel;

                d.GridSplitter_DragDelta(e.VerticalChange, this._scrollViewer.ActualHeight);
                e.Handled = true;
            }
        }
    #endregion methods
  }
}