namespace Edi.SettingsView.Config
{
  using System.Windows;

  /// <summary>
  /// Interaction logic for GotoLine.xaml
  /// </summary>
  public partial class ConfigDlg : FirstFloor.ModernUI.Windows.Controls.ModernWindow
  {
    /// <summary>
    /// Class constructor
    /// </summary>
    public ConfigDlg()
    {
      InitializeComponent();

      Loaded += ConfigDlg_Loaded;
    }

    /// <summary>
    /// Standard load event fired when view becomes visible for the first time.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void ConfigDlg_Loaded(object sender, RoutedEventArgs e)
    {
      // Ensure that dialog will keep its initial size no matter the
      // containing content in Tabitem (or what not) does after load
      ResizeMode = ResizeMode.NoResize;
      SizeToContent = SizeToContent.Manual;
    }
  }
}
