namespace MiniUML.View.Views.ResizeAdorner
{
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using MiniUML.Model.Events;
  using MiniUML.View.Views.ResizeAdorner.Decorators;

  /// <summary>
  /// The <seealso cref="DesignerItem"/> class organizes a contentcontrol with a resize
  /// adorner such that resize adorner elements can be applied to any WPF view displayed on a canvas.
  /// 
  /// The resize adorner view design is baed on
  /// http://www.codeproject.com/Articles/22952/WPF-Diagram-Designer-Part-1
  /// </summary>
  public class DesignerItem : ContentControl
  {
    #region fields
    public static readonly DependencyProperty IsSelectedProperty =
      DependencyProperty.Register("IsSelected", typeof(bool),
                                  typeof(DesignerItem),
                                  new FrameworkPropertyMetadata(false));

    public static readonly DependencyProperty ResizeSelectedShapesProperty =
        DependencyProperty.Register("ResizeSelectedShapes",
                                    typeof(ICommand),
                                    typeof(DesignerItem),
                                    new PropertyMetadata(null, ChangedResizeSelectedShapes));

    /// <summary>
    /// This command is invoked in the bound (viewmodel) object when
    /// the user has resized the shape (via resize adorner).
    /// </summary>
    private ICommand mResizeSelectedShapes = null;

    ////public static readonly DependencyProperty MoveThumbTemplateProperty =
    ////    DependencyProperty.RegisterAttached("MoveThumbTemplate", typeof(ControlTemplate), typeof(DesignerItem));

    private ContentPresenter mContentPresenter = null;
    private ResizeDecorator mResizeDecorator = null;
    ////private MoveThumb mThumb = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// Static class constructor (is required to register control within WPF framework)
    /// </summary>
    static DesignerItem()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerItem),
                                                                new FrameworkPropertyMetadata(typeof(DesignerItem)));
    }

    /// <summary>
    /// Class constructor
    /// </summary>
    public DesignerItem()
    {
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// This dependency property command is invoked in the bound (viewmodel) object when
    /// the user has resized the shape (via resize adorner).
    /// </summary>
    public ICommand ResizeSelectedShapes
    {
      get { return (ICommand)GetValue(ResizeSelectedShapesProperty); }
      set { this.SetValue(ResizeSelectedShapesProperty, value); }
    }

    /// <summary>
    /// Get/set dependency property to determine whether shape should be displayed as being selected or not.
    /// </summary>
    public bool IsSelected
    {
      get { return (bool)GetValue(IsSelectedProperty); }
      set { this.SetValue(IsSelectedProperty, value); }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Standard method that is executed when the template 'skin' is applied
    /// (by the WPF framework) on the look-less control.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if (this.Template != null)
      {
        // Get links to required child elements inside this control.
        this.mContentPresenter = this.GetTemplateChild("PART_ContentPresenter") as ContentPresenter;
        this.mResizeDecorator = this.GetTemplateChild("PART_DesignerItemDecorator") as ResizeDecorator;
        ////this.mThumb = this.GetTemplateChild("PART_MoveThumb") as MoveThumb;

        if (this.mResizeDecorator != null)
          this.mResizeDecorator.AssignDragDeltaEvent(this.ResizeThumb_DragDelta);
      }
    }

    public void ResizeThumb_DragDelta(object sender, DragDeltaThumbEvent e)
    {
      if (this.mResizeSelectedShapes != null)
      {
        // Call the bound (ShapeSizeViewModel) to implement this change via (XAML) command binding
        if (this.mResizeSelectedShapes.CanExecute(null))
          this.mResizeSelectedShapes.Execute(e);
      }
    }

    #region MoveThumb DP
    /// <summary>
    /// Get MoveThumb dependency property field.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    ////public static ControlTemplate GetMoveThumbTemplate(UIElement element)
    ////{
    ////  return (ControlTemplate)element.GetValue(MoveThumbTemplateProperty);
    ////}

    /// <summary>
    /// Set MoveThumb dependency property field.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="value"></param>
    ////public static void SetMoveThumbTemplate(UIElement element, ControlTemplate value)
    ////{
    ////  element.SetValue(MoveThumbTemplateProperty, value);
    ////}
    #endregion MoveThumb DP

    protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseDown(e);
/***
      DesignerCanvas designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;

      if (designer != null)
      {
        if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
        {
          this.IsSelected = !this.IsSelected;
        }
        else
        {
          if (!this.IsSelected)
          {
            designer.DeselectAll();
            this.IsSelected = true;
          }
        }
      }
      e.Handled = false;
***/
    }

    private static void ChangedResizeSelectedShapes(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      DesignerItem di = d as DesignerItem;

      di.ResetChangedResizeSelectedShapesCommand(e.NewValue as ICommand);
    }

    private void ResetChangedResizeSelectedShapesCommand(ICommand resizeSelectedShapes)
    {
      this.mResizeSelectedShapes = resizeSelectedShapes;
    }
    #endregion methods
  }
}
