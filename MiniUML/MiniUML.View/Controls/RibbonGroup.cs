namespace MiniUML.View.Controls
{
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  /// Interaction logic for RibbonGroup.xaml
  /// </summary>
  public partial class RibbonGroup : GroupBox
  {
    static RibbonGroup()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonGroup), new FrameworkPropertyMetadata(typeof(RibbonGroup)));
    }
  }
}
