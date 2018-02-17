namespace MiniUML.Plugins.UmlClassDiagram.Controls.View.Shapes
{
  using System.Windows;
  using Model.ViewModels.Shapes;
  using Base;

  /// <summary>
  /// Interaction logic for UmlNodeShape.xaml
  /// </summary>
  public partial class UmlNodeShape : ShapeViewBase
  {
    private const double Border3DSize = 20;

    #region constructor
    /// <summary>
    /// static class constructor
    /// </summary>
    static UmlNodeShape()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(UmlNodeShape),
      new FrameworkPropertyMetadata(typeof(UmlNodeShape)));
    }

    /// <summary>
    /// class constructor
    /// </summary>
    public UmlNodeShape()
    {
    }
    #endregion constructor

    #region properties
    #region Second Square Points
    public Point P2_SecondSquare
    {
      get
      {
        return new Point((ActualWidth - 1 > 0 ? ActualWidth - 1 : ActualWidth), 1);
      }
    }

    public Point P3_SecondSquare
    {
      get
      {
        return new Point((ActualWidth - 1 > 0 ? ActualWidth - 1 : ActualWidth),
                         (ActualHeight - Border3DSize > 0 ? ActualHeight - Border3DSize : ActualHeight));
      }
    }
    #endregion Second Square Points

    #region First Square properties
    /**
     *     +--------+
     * P1/      P2 /|
     * +---------+  |
     * |         |  |
     * |         |  +
     * |         | /
     * +---------+/
     * P3       P4  FirstSquare Points
     ***/
    public Point P2_FirstSquare
    {
      get
      {
        return new Point((ActualWidth - Border3DSize > 0 ? ActualWidth - Border3DSize : ActualWidth),
                         (ActualHeight > Border3DSize ? Border3DSize : ActualHeight));
      }
    }

    public Point P3_FirstSquare
    {
      get
      {
        return new Point(1, (ActualHeight - 1 > 0 ? ActualHeight - 1 : ActualHeight));
      }
    }

    public Point P4_FirstSquare
    {
      get
      {
        return new Point((ActualWidth - Border3DSize > 0 ? ActualWidth - Border3DSize : ActualWidth),
                         (ActualHeight - 1 > 0 ? ActualHeight - 1 : ActualHeight));
      }
    }
    #endregion First Square properties
    #endregion properties

    #region methods
    /// <summary>
    ///     When overridden in a derived class, is invoked whenever application code
    ///     or internal processes call System.Windows.FrameworkElement.ApplyTemplate()
    ///     -> which in turn applies the (XAML) view definition of this control).
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      // Attach a context menu when the corresponding template is loaded
      ContextMenu = CreateContextMenu(DataContext as ShapeViewModelBase);
    }
    #endregion methods
  }
}
