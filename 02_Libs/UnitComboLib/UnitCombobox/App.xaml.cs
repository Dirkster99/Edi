namespace UnitCombobox
{
  using System.Globalization;
  using System.Threading;
  using System.Windows;

  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public App()
    {
      ////Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
      ////Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

      Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
      Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");    
    }
  }
}
