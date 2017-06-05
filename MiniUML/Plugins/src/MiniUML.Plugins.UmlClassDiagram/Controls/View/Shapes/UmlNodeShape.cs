namespace MiniUML.Plugins.UmlClassDiagram.Controls.View.Shapes
{
  using System.Windows;
  using MiniUML.Model.ViewModels;
  using MiniUML.Model.ViewModels.Shapes;
  using MiniUML.Plugins.UmlClassDiagram.Controls.View.Shapes.Base;

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
        return new Point((this.ActualWidth - 1 > 0 ? this.ActualWidth - 1 : this.ActualWidth), 1);
      }
    }

    public Point P3_SecondSquare
    {
      get
      {
        return new Point((this.ActualWidth - 1 > 0 ? this.ActualWidth - 1 : this.ActualWidth),
                         (this.ActualHeight - UmlNodeShape.Border3DSize > 0 ? this.ActualHeight - UmlNodeShape.Border3DSize : this.ActualHeight));
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
        return new Point((this.ActualWidth - UmlNodeShape.Border3DSize > 0 ? this.ActualWidth - UmlNodeShape.Border3DSize : this.ActualWidth),
                         (this.ActualHeight > UmlNodeShape.Border3DSize ? UmlNodeShape.Border3DSize : this.ActualHeight));
      }
    }

    public Point P3_FirstSquare
    {
      get
      {
        return new Point(1, (this.ActualHeight - 1 > 0 ? this.ActualHeight - 1 : this.ActualHeight));
      }
    }

    public Point P4_FirstSquare
    {
      get
      {
        return new Point((this.ActualWidth - UmlNodeShape.Border3DSize > 0 ? this.ActualWidth - UmlNodeShape.Border3DSize : this.ActualWidth),
                         (this.ActualHeight - 1 > 0 ? this.ActualHeight - 1 : this.ActualHeight));
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
      this.ContextMenu = this.CreateContextMenu(this.DataContext as ShapeViewModelBase);
    }
    #endregion methods
  }
}
