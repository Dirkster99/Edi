namespace Edi.SettingsView.Config
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for GotoLine.xaml
    /// </summary>
    public partial class ConfigDlg : MWindowLib.SimpleMetroWindow
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        public ConfigDlg()
        {
            this.InitializeComponent();

            this.Loaded += ConfigDlg_Loaded;
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
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
            this.SizeToContent = System.Windows.SizeToContent.Manual;
        }
    }
}
