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
        private ResizeThumb _PART_TopRSThumb = null;
        private ResizeThumb _PART_LeftRSThumb = null;
        private ResizeThumb _PART_RightRSThumb = null;
        private ResizeThumb _PART_BottomRSThumb = null;
        private ResizeThumb _PART_TopLeftRSThumb = null;
        private ResizeThumb _PART_TopRightRSThumb = null;
        private ResizeThumb _PART_BottomLeftRSThumb = null;
        private ResizeThumb _PART_BottomRightRSThumb = null;

        private MiniUML.Model.Events.DragDeltaThumbEventHandler _DragDeltaAction_DelegateFunction;
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
            : this()
        {
            _DragDeltaAction_DelegateFunction = dragDeltaAction_DelegateFunction;
        }

        protected ResizeChrome()
        { }
        #endregion constructor

        #region methods
        /// <summary>
        /// Standard method that is executed when the template 'skin' is applied
        /// (by the WPF framework) on the look-less control.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _PART_TopRSThumb = this.GetTemplateChild("PART_TopRSThumb") as ResizeThumb;
            _PART_LeftRSThumb = this.GetTemplateChild("PART_LeftRSThumb") as ResizeThumb;
            _PART_RightRSThumb = this.GetTemplateChild("PART_RightRSThumb") as ResizeThumb;
            _PART_BottomRSThumb = this.GetTemplateChild("PART_BottomRSThumb") as ResizeThumb;
            _PART_TopLeftRSThumb = this.GetTemplateChild("PART_TopLeftRSThumb") as ResizeThumb;
            _PART_TopRightRSThumb = this.GetTemplateChild("PART_TopRightRSThumb") as ResizeThumb;
            _PART_BottomLeftRSThumb = this.GetTemplateChild("PART_BottomLeftRSThumb") as ResizeThumb;
            _PART_BottomRightRSThumb = this.GetTemplateChild("PART_BottomRightRSThumb") as ResizeThumb;

            if (_DragDeltaAction_DelegateFunction != null)
            {
                _PART_TopRSThumb.DragDeltaEvent += _DragDeltaAction_DelegateFunction;

                _PART_TopRSThumb.DragDeltaEvent += _DragDeltaAction_DelegateFunction;
                _PART_LeftRSThumb.DragDeltaEvent += _DragDeltaAction_DelegateFunction;
                _PART_RightRSThumb.DragDeltaEvent += _DragDeltaAction_DelegateFunction;
                _PART_BottomRSThumb.DragDeltaEvent += _DragDeltaAction_DelegateFunction;
                _PART_TopLeftRSThumb.DragDeltaEvent += _DragDeltaAction_DelegateFunction;
                _PART_TopRightRSThumb.DragDeltaEvent += _DragDeltaAction_DelegateFunction;
                _PART_BottomLeftRSThumb.DragDeltaEvent += _DragDeltaAction_DelegateFunction;
                _PART_BottomRightRSThumb.DragDeltaEvent += _DragDeltaAction_DelegateFunction;
            }
        }
        #endregion methods
    }
}
