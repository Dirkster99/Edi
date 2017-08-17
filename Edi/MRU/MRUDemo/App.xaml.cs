namespace MRU
{
    using System.Globalization;
    using System.Threading;
    using System.Windows;
    using System.Windows.Markup;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Ensure the current culture passed into bindings is the OS culture.
            // By default, WPF uses en-US as the culture, regardless of the system settings.
////            FrameworkElement.LanguageProperty.OverrideMetadata(
////                  typeof(FrameworkElement),
////                  new FrameworkPropertyMetadata(
////                      XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            // Setup WPF to show dates in German formatting
            FrameworkElement.LanguageProperty.OverrideMetadata(
                  typeof(FrameworkElement),
                  new FrameworkPropertyMetadata(
                      XmlLanguage.GetLanguage("de-DE")));

            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");

            var window = new MainWindow();

            window.Show();
        }
    }
}

