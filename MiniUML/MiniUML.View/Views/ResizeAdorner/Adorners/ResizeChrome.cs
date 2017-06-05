namespace MiniUML.View.Views.ResizeAdorner.Adorners
{
  using System.Windows;
  using System.Windows.Controls;
  using MiniUML.Model.Events;
  using MiniUML.View.Views.ResizeAdorner.Thumbs;

  /// <summary>
  /// Class to manage the resize view elements that are
  /// actually visible (and can be manipulated) for the user.
  /// 
  /// The resize adorner view design is baed on
  /// http://www.codeproject.com/Articles/22952/WPF-Diagram-Designer-Part-1
  /// </summary>
  public class ResizeChrome : Control
  {
    #region fields
    private ResizeThumb mPART_TopRSThumb = null;
    private ResizeThumb mPART_LeftRSThumb = null;
    private ResizeThumb mPART_RightRSThumb = null;
    private ResizeThumb mPART_BottomRSThumb = null;
    private ResizeThumb mPART_TopLeftRSThumb = null;
    private ResizeThumb mPART_TopRightRSThumb = null;
    private ResizeThumb mPART_BottomLeftRSThumb = null;
    private ResizeThumb mPART_BottomRightRSThumb = null;

    private MiniUML.Model.Events.DragDeltaThumbEventHandler mDragDeltaAction_DelegateFunction;
    #endregion fields

    #region constructor
    /// <summary>
    /// Static class constructor
    /// </summary>
    static ResizeChrome()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeChrome),
                                  new FrameworkPropertyMetadata(typeof(ResizeChrome)));
    }

    /// <summary>
    /// Class constructor
    /// </summary>
    /// <param name="dragDeltaAction_DelegateFunction"></param>
    public ResizeChrome(DragDeltaThumbEventHandler dragDeltaAction_DelegateFunction = null)
    {
      this.mDragDeltaAction_DelegateFunction = dragDeltaAction_DelegateFunction;
    }
    #endregion constructor

    #region methods
    /// <summary>
    /// Standard method that is executed when the template 'skin' is applied
    /// (by the WPF framework) on the look-less control.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      this.mPART_TopRSThumb = this.GetTemplateChild("PART_TopRSThumb") as ResizeThumb;
      this.mPART_LeftRSThumb = this.GetTemplateChild("PART_LeftRSThumb") as ResizeThumb;
      this.mPART_RightRSThumb = this.GetTemplateChild("PART_RightRSThumb") as ResizeThumb;
      this.mPART_BottomRSThumb = this.GetTemplateChild("PART_BottomRSThumb") as ResizeThumb;
      this.mPART_TopLeftRSThumb = this.GetTemplateChild("PART_TopLeftRSThumb") as ResizeThumb;
      this.mPART_TopRightRSThumb = this.GetTemplateChild("PART_TopRightRSThumb") as ResizeThumb;
      this.mPART_BottomLeftRSThumb = this.GetTemplateChild("PART_BottomLeftRSThumb") as ResizeThumb;
      this.mPART_BottomRightRSThumb = this.GetTemplateChild("PART_BottomRightRSThumb") as ResizeThumb;

      if (this.mDragDeltaAction_DelegateFunction != null)
      {
        this.mPART_TopRSThumb.DragDeltaEvent += this.mDragDeltaAction_DelegateFunction;

        this.mPART_TopRSThumb.DragDeltaEvent += this.mDragDeltaAction_DelegateFunction;
        this.mPART_LeftRSThumb.DragDeltaEvent += this.mDragDeltaAction_DelegateFunction;
        this.mPART_RightRSThumb.DragDeltaEvent += this.mDragDeltaAction_DelegateFunction;
        this.mPART_BottomRSThumb.DragDeltaEvent += this.mDragDeltaAction_DelegateFunction;
        this.mPART_TopLeftRSThumb.DragDeltaEvent += this.mDragDeltaAction_DelegateFunction;
        this.mPART_TopRightRSThumb.DragDeltaEvent += this.mDragDeltaAction_DelegateFunction;
        this.mPART_BottomLeftRSThumb.DragDeltaEvent += this.mDragDeltaAction_DelegateFunction;
        this.mPART_BottomRightRSThumb.DragDeltaEvent += this.mDragDeltaAction_DelegateFunction;
      }
    }
    #endregion methods
  }
}
