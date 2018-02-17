namespace MiniUML.View.Views.ResizeAdorner.Adorners
{
  using System.Windows;
  using System.Windows.Controls;
  using Model.Events;
  using Thumbs;

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

    private DragDeltaThumbEventHandler mDragDeltaAction_DelegateFunction;
    #endregion fields

    #region constructor
    /// <summary>
    /// Static class constructor
    /// </summary>
    static ResizeChrome()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeChrome),
                                  new FrameworkPropertyMetadata(typeof(ResizeChrome)));
    }

    /// <summary>
    /// Class constructor
    /// </summary>
    /// <param name="dragDeltaAction_DelegateFunction"></param>
    public ResizeChrome(DragDeltaThumbEventHandler dragDeltaAction_DelegateFunction = null)
    {
      mDragDeltaAction_DelegateFunction = dragDeltaAction_DelegateFunction;
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

      mPART_TopRSThumb = GetTemplateChild("PART_TopRSThumb") as ResizeThumb;
      mPART_LeftRSThumb = GetTemplateChild("PART_LeftRSThumb") as ResizeThumb;
      mPART_RightRSThumb = GetTemplateChild("PART_RightRSThumb") as ResizeThumb;
      mPART_BottomRSThumb = GetTemplateChild("PART_BottomRSThumb") as ResizeThumb;
      mPART_TopLeftRSThumb = GetTemplateChild("PART_TopLeftRSThumb") as ResizeThumb;
      mPART_TopRightRSThumb = GetTemplateChild("PART_TopRightRSThumb") as ResizeThumb;
      mPART_BottomLeftRSThumb = GetTemplateChild("PART_BottomLeftRSThumb") as ResizeThumb;
      mPART_BottomRightRSThumb = GetTemplateChild("PART_BottomRightRSThumb") as ResizeThumb;

      if (mDragDeltaAction_DelegateFunction != null)
      {
        mPART_TopRSThumb.DragDeltaEvent += mDragDeltaAction_DelegateFunction;

        mPART_TopRSThumb.DragDeltaEvent += mDragDeltaAction_DelegateFunction;
        mPART_LeftRSThumb.DragDeltaEvent += mDragDeltaAction_DelegateFunction;
        mPART_RightRSThumb.DragDeltaEvent += mDragDeltaAction_DelegateFunction;
        mPART_BottomRSThumb.DragDeltaEvent += mDragDeltaAction_DelegateFunction;
        mPART_TopLeftRSThumb.DragDeltaEvent += mDragDeltaAction_DelegateFunction;
        mPART_TopRightRSThumb.DragDeltaEvent += mDragDeltaAction_DelegateFunction;
        mPART_BottomLeftRSThumb.DragDeltaEvent += mDragDeltaAction_DelegateFunction;
        mPART_BottomRightRSThumb.DragDeltaEvent += mDragDeltaAction_DelegateFunction;
      }
    }
    #endregion methods
  }
}
