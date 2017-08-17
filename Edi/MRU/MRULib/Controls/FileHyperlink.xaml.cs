namespace MRULib.Controls
{
    using MRULib.MRU.Models;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Navigation;

    /// <summary>
    /// Interaction logic for FileHyperlink.xaml
    /// </summary>
    public partial class FileHyperlink : UserControl
    {
        #region fields
        private static RoutedCommand mCopyUri;
        private static RoutedCommand mNavigateToUri;
        private static RoutedCommand mOpenContainingFolder;

        /// <summary>
        /// Relay NavigateUri dependency property to outside of FileHyperlink Control.
        /// Use this dp if you can offer a Uri compliant string that represents the target destination.
        /// "file:///c:\\" or "https://mysite.com"
        /// 
        /// !!! A standard string object "c:\\" is not suitable for binding to NavigateUri !!!
        /// </summary>
        private static readonly DependencyProperty NavigateUriProperty =
          DependencyProperty.Register("NavigateUri",
              typeof(string),
              typeof(FileHyperlink),
              new PropertyMetadata(string.Empty));

        private static readonly DependencyProperty TextProperty =
          DependencyProperty.Register("Text",
              typeof(string),
              typeof(FileHyperlink),
              new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets a command to associate with the FileHyperlink.
        /// 
        /// A command to associate with the System.Windows.Documents.Hyperlink. The default is null.
        /// </summary>
        [Bindable(true)]
        [Category("Action")]
        [Localizability(LocalizationCategory.NeverLocalize)]
        public ICommand NavigateUriCommand
        {
            get { return (ICommand)GetValue(NavigateUriCommandProperty); }
            set { SetValue(NavigateUriCommandProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for NavigateUriCommand.
        /// </summary>
        private static readonly DependencyProperty NavigateUriCommandProperty =
            DependencyProperty.Register("NavigateUriCommand",
                typeof(ICommand),
                typeof(FileHyperlink),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets command parameters associated with the command specified by the
        /// FileHyperlink.Command property.
        /// 
        /// An object specifying parameters for the command specified by the System.Windows.Documents.Hyperlink.Command
        /// property. The default is null.
        /// </summary>
        [Bindable(true)]
        [Category("Action")]
        [Localizability(LocalizationCategory.NeverLocalize)]
        public object NavigateUriCommandParameter
        {
            get { return (object)GetValue(NavigateUriCommandParameterProperty); }
            set { SetValue(NavigateUriCommandParameterProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for NavigateUriCommandParameter.
        /// </summary>
        private static readonly DependencyProperty NavigateUriCommandParameterProperty =
            DependencyProperty.Register("NavigateUriCommandParameter",
                typeof(object),
                typeof(FileHyperlink),
                new PropertyMetadata(null));
        #endregion fields

        #region constructors
        /// <summary>
        /// Static class constructor.
        /// </summary>
        static FileHyperlink()
        {
            FileHyperlink.mCopyUri = new RoutedCommand("CopyUri", typeof(FileHyperlink));

            CommandManager.RegisterClassCommandBinding(typeof(FileHyperlink), new CommandBinding(mCopyUri, CopyHyperlinkUri));
            CommandManager.RegisterClassInputBinding(typeof(FileHyperlink), new InputBinding(mCopyUri, new KeyGesture(Key.C, ModifierKeys.Control, "Ctrl-C")));

            FileHyperlink.mNavigateToUri = new RoutedCommand("NavigateToUri", typeof(FileHyperlink));
            CommandManager.RegisterClassCommandBinding(typeof(FileHyperlink), new CommandBinding(mNavigateToUri, Hyperlink_CommandNavigateTo));
            ////CommandManager.RegisterClassInputBinding(typeof(FileHyperlink), new InputBinding(mCopyUri, new KeyGesture(Key.C, ModifierKeys.Control, "Ctrl-C")));

            FileHyperlink.mOpenContainingFolder = new RoutedCommand("OpenContainingFolder", typeof(FileHyperlink));
            CommandManager.RegisterClassCommandBinding(typeof(FileHyperlink), new CommandBinding(mOpenContainingFolder, Hyperlink_OpenContainingFolder));
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public FileHyperlink()
        {
            InitializeComponent();
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets the CopyUri command of the <seealso cref="FileHyperlink"/> control.
        /// This command copies the current Uri (path) into the Windows clipboard.
        /// </summary>
        public static RoutedCommand CopyUri
        {
            get
            {
                return FileHyperlink.mCopyUri;
            }
        }

        /// <summary>
        /// Gets the NavigateUri command of the <seealso cref="FileHyperlink"/> control.
        /// This command opens the file with the application that is associated in the Windows Explorer.
        /// </summary>
        public static RoutedCommand NavigateToUri
        {
            get
            {
                return FileHyperlink.mNavigateToUri;
            }
        }

        /// <summary>
        /// Gets the OpenContainingFolder command of the <seealso cref="FileHyperlink"/> control.
        /// This command opens the requested folder in Windows Explorer.
        /// </summary>
        public static RoutedCommand OpenContainingFolder
        {
            get
            {
                return FileHyperlink.mOpenContainingFolder;
            }
        }

        /// <summary>
        /// Declare NavigateUri property to allow a user who clicked
        /// on the dispalyed Hyperlink to navigate to it...
        /// 
        /// Relay NavigateUri dependency property to outside of FileHyperlink Control.
        /// Use this dp if you can offer a Uri compliant string that represents the target destination.
        /// "file:///c:\\" or "https://mysite.com"
        /// 
        /// !!! A standard string object "c:\\" is not suitable for binding to NavigateUri !!!
        /// </summary>
        public string NavigateUri
        {
            get { return (string)this.GetValue(FileHyperlink.NavigateUriProperty); }
            set { this.SetValue(FileHyperlink.NavigateUriProperty, value); }
        }

        /// <summary>
        /// Dependency property that denotes the text that is shown in the hyperlink
        /// (while NavigateUri denotes the linked destination).
        /// </summary>
        public string Text
        {
            get { return (string)this.GetValue(FileHyperlink.TextProperty); }
            set { this.SetValue(FileHyperlink.TextProperty, value); }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Standard method that is executed when control template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Debug.Assert(this.PART_Hyperlink != null, "No Hyperlink object found!");

            // Attach hyperlink event clicked event handler to Hyperlink ControlTemplate if there is no command defined
            // Commanding allows calling commands that are external to the control (application commands) with parameters
            // that can differ from whats available in this control (using converters and what not)
            //
            // Therefore, commanding overrules the Hyperlink.Clicked event when it is defined.
            if (this.PART_Hyperlink != null)
            {
                if (this.NavigateUriCommand == null)
                    this.PART_Hyperlink.RequestNavigate += this.Hyperlink_RequestNavigate;
            }
        }

        private static void Hyperlink_OpenContainingFolder(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (sender == null || e == null) return;

                e.Handled = true;

                FileHyperlink whLink = sender as FileHyperlink;

                if (whLink == null) return;

                FileSystemCommands.OpenContainingFolder(whLink.NavigateUri);
            }
            catch {}
        }

        /// <summary>
        /// Process command when a hyperlink has been clicked.
        /// Start a web browser and let it browse to where this points to...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Hyperlink_CommandNavigateTo(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (sender == null || e == null) return;

                e.Handled = true;

                FileHyperlink whLink = sender as FileHyperlink;

                if (whLink == null) return;

                FileSystemCommands.OpenInWindows(whLink.NavigateUri);
            }
            catch {}
        }

        private static void CopyHyperlinkUri(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (sender == null || e == null) return;

                e.Handled = true;

                FileHyperlink whLink = sender as FileHyperlink;

                if (whLink == null) return;

                FileSystemCommands.CopyPath(whLink.NavigateUri);
            }
            catch { }
        }

        /// <summary>
        /// A hyperlink has been clicked. Start a web browser and let it browse to where this points to...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(this.NavigateUri));
            }
            catch
            {
////           var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
////           msgBox.Show(string.Format(CultureInfo.CurrentCulture, "{0}\n'{1}'.", ex.Message, (this.NavigateUri == null ? string.Empty : this.NavigateUri.ToString())),
////                       Local.Strings.STR_MSG_ERROR_FINDING_RESOURCE,
////                       MsgBoxButtons.OK, MsgBoxImage.Error);
            }
        }
        #endregion methods
    }
}
