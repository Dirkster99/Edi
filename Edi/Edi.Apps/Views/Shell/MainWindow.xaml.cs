namespace Edi.Apps.Views.Shell
{
	using System;
	using System.ComponentModel.Composition;
	using Edi.Core.Interfaces;
	using Edi.Apps.Events;
	using Edi.Apps.Interfaces.ViewModel;
	using Prism.Events;

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
 [Export]
  public partial class MainWindow : FirstFloor.ModernUI.Windows.Controls.ModernWindow, Edi.Core.Interfaces.ILayoutableWindow
  {
    #region constructors
		[ImportingConstructor]
    public MainWindow(IAvalonDockLayoutViewModel av, IApplicationViewModel appVM)
    {
      this.InitializeComponent();

      this.dockView.SetTemplates(av.ViewProperties.SelectPanesTemplate,
																 av.ViewProperties.DocumentHeaderTemplate,
																 av.ViewProperties.SelectPanesStyle,
																 av.ViewProperties.LayoutInitializer,
																 av.LayoutID);

      // Register these methods to receive PRISM event notifications about load and save of avalondock layouts
			LoadLayoutEvent.Instance.Subscribe(this.dockView.OnLoadLayout, ThreadOption.PublisherThread,
			                                   true,
                                         s => s.LayoutID == av.LayoutID);

			// subscribe to close event messing to application viewmodel
			this.Closing += appVM.OnClosing;

			// When the ViewModel asks to be closed, close the window.
			// Source: http://msdn.microsoft.com/en-us/magazine/dd419663.aspx
			appVM.RequestClose += delegate
			{
				// Save session data and close application
				appVM.OnClosed(this);
			};
    }
    #endregion constructors

    #region properties
    /// <summary>
    /// Gets/Sets the LayoutId of the AvalonDocking Manager layout used to manage
    /// the positions and layout of documents and tool windows within the AvalonDock
    /// view.
    /// </summary>
    public Guid LayoutID
    {
      get
      {
        return (this.dockView != null ? this.dockView.LayoutID : Guid.Empty);
      }
    }

    /// <summary>
    /// Gets the current AvalonDockManager Xml layout and returns it as a string.
    /// </summary>
    public string CurrentADLayout
    {
      get
      {
        return (this.dockView != null ? this.dockView.CurrentADLayout : string.Empty);
      }
    }
    #endregion properties
  }
}
