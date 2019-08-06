namespace Edi.Apps.Views.Shell
{
    using System;
    using Core.Interfaces;
    using MWindowLib;
    using Edi.Apps.Interfaces;
    using System.ComponentModel;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : SimpleMetroWindow, ILayoutableWindow
    {
        #region fields
        private readonly IAvalonDockLayoutViewModel _av;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="av"></param>
        /// <param name="appVm"></param>
        public MainWindow(IAvalonDockLayoutViewModel av,
                          IApplicationViewModel appVm
                          )
            : this()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            _av = av;

            dockView.InitTemplates(_av.ViewProperties.SelectPanesTemplate,
                                   _av.ViewProperties.DocumentHeaderTemplate,
                                   _av.ViewProperties.SelectPanesStyle,
                                   _av.ViewProperties.LayoutInitializer,
                                   _av.LayoutId);

            // Register this methods to receive event notifications about
            // load and save of avalondock layouts
            _av.LoadLayout += dockView.OnLoadLayout;

            // subscribe to close event messing to application viewmodel
            Closing += appVm.OnClosing;

            // When the ViewModel asks to be closed, close the window.
            // Source: http://msdn.microsoft.com/en-us/magazine/dd419663.aspx
            appVm.RequestClose += delegate
            {
                // Save session data and close application
                appVm.OnClosed(this);
            };
        }

        protected MainWindow()
        {
            InitializeComponent();
        }
        #endregion constructors

        #region methods
        /// <summary>
        /// Call this method to clean-up resource references on exit.
        /// </summary>
        public void ReleaseResources()
        {
            // Remove event notifications about load and save of avalondock layouts
            _av.LoadLayout -= dockView.OnLoadLayout;
        }
        #endregion methods

        #region properties
        /// <summary>
        /// Gets/Sets the LayoutId of the AvalonDocking Manager layout used to manage
        /// the positions and layout of documents and tool windows within the AvalonDock
        /// view.
        /// </summary>
        public Guid LayoutId
        {
            get
            {
                return (dockView != null ? dockView.LayoutId : Guid.Empty);
            }
        }

        /// <summary>
        /// Gets the current AvalonDockManager Xml layout and returns it as a string.
        /// </summary>
        public string CurrentAdLayout
        {
            get
            {
                return (dockView != null ? dockView.CurrentAdLayout : string.Empty);
            }
        }
        #endregion properties
    }
}
