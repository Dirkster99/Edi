namespace MiniUML.View.Views.ResizeAdorner.Adorners
{
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Documents;
  using System.Windows.Media;
  using MiniUML.Model.Events;

  /// <summary>
  /// The resize adorner is a ContentControl that can host a number of different
  /// WPF contents and implements its adorners around this content to allow users
  /// shape adjustments via simple point and click operations.
  /// 
  /// The resize adorner view design is baed on
  /// http://www.codeproject.com/Articles/22952/WPF-Diagram-Designer-Part-1
  /// </summary>
  public class ResizeAdorner : Adorner
  {
    #region fields
    private VisualCollection mVisuals=null;
    private ResizeChrome mChrome = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    /// <param name="designerItem"></param>
    public ResizeAdorner(ContentControl designerItem,
                         DragDeltaThumbEventHandler dragDeltaAction_DelegateFunction = null)
      : base(designerItem)
    {
      this.mChrome = new ResizeChrome(dragDeltaAction_DelegateFunction);
      this.mVisuals = new VisualCollection(this);

      this.mVisuals.Add(this.mChrome);
      this.mChrome.DataContext = designerItem;  // Apply data context from this object to chrome object
    }
    #endregion constructor

    #region properties
    protected override int VisualChildrenCount
    {
      get
      {
        return this.mVisuals.Count;
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Positions child elements and determines a size for a
    /// System.Windows.FrameworkElement derived class.
    ///
    /// Parameters:
    ///   finalSize:
    ///     The final area within the parent that this element
    ///     should use to arrange itself and its children.
    ///
    /// Returns:
    ///     The actual size used.
    /// </summary>
    /// <param name="arrangeBounds"></param>
    /// <returns><seealso cref="Size"/></returns>
    protected override Size ArrangeOverride(Size arrangeBounds)
    {
      this.mChrome.Arrange(new Rect(arrangeBounds));

      return arrangeBounds;
    }

    /// <summary>
    /// Returns a child at the specified index from a collection of child elements.
    ///
    /// Parameters:
    ///   index:
    ///     The zero-based index of the requested child element in the collection.
    ///
    /// Returns:
    ///     The requested child element. This should not return null; if the provided
    ///     index is out of range, an exception is thrown.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    protected override Visual GetVisualChild(int index)
    {
      return this.mVisuals[index];
    }
    #endregion methods
  }
}
