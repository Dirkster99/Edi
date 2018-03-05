namespace Files.Views.FileExplorer
{
    using Files.ViewModels.FileExplorer;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for FileExplorerView.xaml
    /// </summary>
    public partial class FileExplorerView : UserControl
    {
        public FileExplorerView()
        {
            this.InitializeComponent();

            Loaded += FileExplorerView_Loaded;
        }

        private void FileExplorerView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Loaded -= FileExplorerView_Loaded;

            var vm = DataContext as FileExplorerViewModel;

            if (vm == null)
                return;

            vm.InitialzeOnLoad();
        }
    }
}
