namespace SimpleControls.Hyperlink
{
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using MsgBox;
    using CommonServiceLocator;

    /// <summary>
    /// Interaction logic for WebHyperlink.xaml
    /// </summary>
    public class WebHyperlink : UserControl
    {
        #region fields
        private static readonly DependencyProperty NavigateUriProperty =
          DependencyProperty.Register("NavigateUri", typeof(System.Uri), typeof(WebHyperlink));

        private static readonly DependencyProperty TextProperty =
          DependencyProperty.Register("Text", typeof(string), typeof(WebHyperlink));

        private static readonly RoutedCommand _mCopyUri;
        private static readonly RoutedCommand _mNavigateToUri;

        private System.Windows.Documents.Hyperlink _mHypLink;
        #endregion fields

        #region constructor
        static WebHyperlink()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WebHyperlink),
                      new FrameworkPropertyMetadata(typeof(WebHyperlink)));

            _mCopyUri = new RoutedCommand("CopyUri", typeof(WebHyperlink));

            CommandManager.RegisterClassCommandBinding(typeof(WebHyperlink), new CommandBinding(_mCopyUri, CopyHyperlinkUri));
            CommandManager.RegisterClassInputBinding(typeof(WebHyperlink), new InputBinding(_mCopyUri, new KeyGesture(Key.C, ModifierKeys.Control, "Ctrl-C")));

            _mNavigateToUri = new RoutedCommand("NavigateToUri", typeof(WebHyperlink));
            CommandManager.RegisterClassCommandBinding(typeof(WebHyperlink), new CommandBinding(_mNavigateToUri, Hyperlink_CommandNavigateTo));
            ////CommandManager.RegisterClassInputBinding(typeof(WebHyperlink), new InputBinding(mCopyUri, new KeyGesture(Key.C, ModifierKeys.Control, "Ctrl-C")));
        }

        public WebHyperlink()
        {
            _mHypLink = null;
        }
        #endregion constructor

        #region properties
        public static RoutedCommand CopyUri => _mCopyUri;

        public static RoutedCommand NavigateToUri => _mNavigateToUri;

        /// <summary>
        /// Declare NavigateUri property to allow a user who clicked
        /// on the dispalyed Hyperlink to navigate their with their installed browser...
        /// </summary>
        public System.Uri NavigateUri
        {
            get => (System.Uri)GetValue(NavigateUriProperty);
            set => SetValue(NavigateUriProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        #endregion

        #region Methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _mHypLink = GetTemplateChild("PART_Hyperlink") as System.Windows.Documents.Hyperlink;
            Debug.Assert(_mHypLink != null, "No Hyperlink in ControlTemplate!");

            // Attach hyperlink event clicked event handler to Hyperlink ControlTemplate if there is no command defined
            // Commanding allows calling commands that are external to the control (application commands) with parameters
            // that can differ from whats available in this control (using converters and what not)
            //
            // Therefore, commanding overrules the Hyperlink.Clicked event when it is defined.
            if (_mHypLink != null)
            {
                if (_mHypLink.Command == null)
                    _mHypLink.RequestNavigate += Hyperlink_RequestNavigate;
            }
        }

        /// <summary>
        /// Process command when a hyperlink has been clicked.
        /// Start a web browser and let it browse to where this points to...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Hyperlink_CommandNavigateTo(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender == null || e == null) return;

            e.Handled = true;

            if (!(sender is WebHyperlink whLink)) return;

            try
            {
                Process.Start(new ProcessStartInfo(whLink.NavigateUri.AbsoluteUri));
            }
            catch (System.Exception ex)
            {
                var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
                msgBox.Show(string.Format(CultureInfo.CurrentCulture, "{0}.", ex.Message),
                            Local.Strings.STR_MSG_ERROR_FINDING_RESOURCE,
                            MsgBoxButtons.OK, MsgBoxImage.Error);
            }
        }

        /// <summary>
        /// A hyperlink has been clicked. Start a web browser and let it browse to where this points to...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CopyHyperlinkUri(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender == null || e == null) return;

            e.Handled = true;

            if (!(sender is WebHyperlink whLink)) return;

            try
            {
                Clipboard.SetText(whLink.NavigateUri.AbsoluteUri);
            }
            catch
            {
                Clipboard.SetText(whLink.NavigateUri.OriginalString);
            }
        }

        /// <summary>
        /// A hyperlink has been clicked. Start a web browser and let it browse to where this points to...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            }
            catch (System.Exception ex)
            {
                var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
                msgBox.Show(string.Format(CultureInfo.CurrentCulture, "{0}.", ex.Message),
                            Local.Strings.STR_MSG_ERROR_FINDING_RESOURCE,
                            MsgBoxButtons.OK, MsgBoxImage.Error);
            }
        }
        #endregion
    }
}
