namespace FolderBrowser.Views
{
  using System.Windows.Controls;

  /// <summary>
  /// Interaction logic for FolderBrowserView.xaml
  /// </summary>
  public partial class FolderBrowserView : UserControl
  {
    /// <summary>
    /// Standard class constructor
    /// </summary>
    public FolderBrowserView()
    {
      this.InitializeComponent();

      TreeView t = new TreeView();
    }
  }
}
