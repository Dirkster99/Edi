namespace MiniUML.Plugins.UmlClassDiagram.Controls.View.Shapes
{
  using System.Windows;
  using Model.ViewModels.Shapes;
  using Base;

  /// <summary>
  /// Interaction logic for UmlSquareShape.xaml
  /// </summary>
  public partial class UmlSquareShape : ShapeViewBase
  {
    #region constructor
    /// <summary>
    /// static class constructor
    /// </summary>
    static UmlSquareShape()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(UmlSquareShape),
      new FrameworkPropertyMetadata(typeof(UmlSquareShape)));
    }

    /// <summary>
    /// class constructor
    /// </summary>
    public UmlSquareShape()
    {
    }
    #endregion constructor

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
