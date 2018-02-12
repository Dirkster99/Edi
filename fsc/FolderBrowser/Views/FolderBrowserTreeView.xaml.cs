namespace FolderBrowser.Views
{
    using FileSystemModels;
    using FileSystemModels.Interfaces;
    using FolderBrowser.Interfaces;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for FolderBrowserTreeView.xaml
    /// </summary>
    public partial class FolderBrowserTreeView : UserControl
    {
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Standard class constructor
        /// </summary>
        public FolderBrowserTreeView()
        {
            InitializeComponent();
            Loaded += FolderBrowserTreeView_Loaded;
        }

        /// <summary>
        /// Initializes the folder browser viewmodel and view as soon as the view is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FolderBrowserTreeView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Loaded -= FolderBrowserTreeView_Loaded;

            var vm = DataContext as IBrowserViewModel;

            if (vm != null)
            {
                IPathModel location = null;
                try
                {
                    location = PathFactory.Create(vm.InitialPath);

                    // XXX Todo Keep task reference, support cancel, and remove on end?
                    var t = vm.NavigateToAsync(location);
                }
                catch
                {
                }
            }
            else
            {
                if (DataContext != null)
                    logger.DebugFormat("FolderBrowserTreeView: Attached vm is: {0}", DataContext.ToString());
                else
                {
                    logger.DebugFormat("FolderBrowserTreeView: No Vm Attached!");
                    this.DataContextChanged += FolderBrowserTreeView_DataContextChangedAsync;
                }
            }
        }

        private async void FolderBrowserTreeView_DataContextChangedAsync(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            this.DataContextChanged -= FolderBrowserTreeView_DataContextChangedAsync;

            var vm = e.NewValue as IBrowserViewModel;

            if (vm != null)
            {
                if (string.IsNullOrEmpty(vm.InitialPath) == false)
                {
                    logger.DebugFormat("FolderBrowserTreeView: Browsing Path on DataContextChanged: '{0}'", vm.InitialPath);

                    await vm.NavigateToAsync(PathFactory.Create(vm.InitialPath));
                }
            }
        }
    }
}
