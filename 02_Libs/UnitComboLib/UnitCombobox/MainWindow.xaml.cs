namespace UnitCombobox
{
  using System.Windows;
  using UnitCombobox.ViewModel;

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      this.InitializeComponent();

      this.DataContext = new AppViewModel();
    }
  }
}
