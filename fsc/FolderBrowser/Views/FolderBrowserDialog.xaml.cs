namespace FolderBrowser.Views
{
  using System.Windows;
  using System.Windows.Input;
  using FolderBrowser.ViewModels;

  /// <summary>
  /// Interaction logic for FolderBrowserDialog.xaml
  /// </summary>
  public partial class FolderBrowserDialog : Window
  {
    #region constructor
    /// <summary>
    /// Standard <seealso cref="FolderBrowserDialog"/> constructor
    /// </summary>
    public FolderBrowserDialog()
    {
      this.InitializeComponent();
    }
    #endregion constructor

    #region methods
    private void Ok_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
    }
    #endregion methods
  }
}
