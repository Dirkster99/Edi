namespace SimpleControls.Hyperlink
{
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Edi.Core;
    using MsgBox;

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

        private static readonly RoutedCommand _CopyUri;
        private static readonly RoutedCommand _mNavigateToUri;

        private System.Windows.Documents.Hyperlink _mHypLink;
        #endregion fields

        #region constructor
        static WebHyperlink()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WebHyperlink),
                      new FrameworkPropertyMetadata(typeof(WebHyperlink)));

            _CopyUri = new RoutedCommand("CopyUri", typeof(WebHyperlink));

            CommandManager.RegisterClassCommandBinding(typeof(WebHyperlink), new CommandBinding(_CopyUri, CopyHyperlinkUri));
            CommandManager.RegisterClassInputBinding(typeof(WebHyperlink), new InputBinding(_CopyUri, new KeyGesture(Key.C, ModifierKeys.Control, "Ctrl-C")));

            _mNavigateToUri = new RoutedCommand("NavigateToUri", typeof(WebHyperlink));
            CommandManager.RegisterClassCommandBinding(typeof(WebHyperlink), new CommandBinding(_mNavigateToUri, Hyperlink_CommandNavigateTo));
            ////CommandManager.RegisterClassInputBinding(typeof(WebHyperlink), new InputBinding(mCopyUri, new KeyGesture(Key.C, ModifierKeys.Control, "Ctrl-C")));
        }

        /// <summary>
        /// Hidden standard constructor
        /// </summary>
        public WebHyperlink()
        {
            _mHypLink = null;
        }
        #endregion constructor

        #region properties
        public static RoutedCommand CopyUri => _CopyUri;

        public static RoutedCommand NavigateToUri => _mNavigateToUri;

        /// <summary>
        /// Declare NavigateUri property to allow a user who clicked
        /// on the dispalyed Hyperlink to navigate their with their installed browser...
        /// </summary>
        public System.Uri NavigateUri
        {
            get { return (System.Uri)GetValue(NavigateUriProperty); }
            set { SetValue(NavigateUriProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Is invoked whenever application code or internal
        /// processes call System.Windows.FrameworkElement.ApplyTemplate.
        /// </summary>
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

            WebHyperlink whLink = sender as WebHyperlink;

            if (whLink == null) return;

            whLink.NavigateTo(whLink.NavigateUri.AbsoluteUri);
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

            WebHyperlink whLink = sender as WebHyperlink;

            if (whLink == null) return;

            try
            {
                System.Windows.Clipboard.SetText(whLink.NavigateUri.AbsoluteUri);
            }
            catch
            {
                System.Windows.Clipboard.SetText(whLink.NavigateUri.OriginalString);
            }
        }

        /// <summary>
        /// A hyperlink has been clicked. Start an OS configured web browser and let it browse
        /// to where this points to...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (e == null)
                return;

            if (e.Uri == null)
                return;

            if (e.Uri.AbsoluteUri == null)
                return;

            NavigateTo(e.Uri.AbsoluteUri);
        }

        /// <summary>
        /// Start an OS configured web browser and let it browse to
        /// where <paramref name="absoluteUri"/> points to...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavigateTo(string absoluteUri)
        {
            try
            {
                Process.Start(new ProcessStartInfo(absoluteUri));
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "{0} -> {1}.", ex.Message, ex.StackTrace));
            }
        }
        #endregion
    }
}
