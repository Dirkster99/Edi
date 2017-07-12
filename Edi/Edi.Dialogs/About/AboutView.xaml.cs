namespace Edi.Dialogs.About
{
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  ///
  /// </summary>
  public class AboutView : Control
  {
    static AboutView()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(AboutView), new FrameworkPropertyMetadata(typeof(AboutView)));
    }
  }
}
