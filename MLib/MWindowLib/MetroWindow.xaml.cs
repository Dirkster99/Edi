namespace MWindowLib
{
    using Controls;  //Extensions
    using Microsoft.Windows.Shell.Standard;
    using MWindowInterfacesLib.Interfaces;
    using Native;
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media.Animation;

    /// <summary>
    /// The <seealso cref="MetroWindow"/> class is a CustonControl that inherites from Window.
    /// 
    /// Remarks:
    /// CustomControl Window is based on this source:
    /// http://stackoverflow.com/questions/13592326/making-wpf-applications-look-metro-styled-even-in-windows-7-window-chrome-t
    /// 
    /// and of course MahApps.Metro http://mahapps.com/
    /// </summary>
    [TemplatePart(Name = PART_OverlayBox, Type = typeof(Grid))]
    [TemplatePart(Name = PART_MetroActiveDialogContainer, Type = typeof(Grid))]
    [TemplatePart(Name = PART_MetroInactiveDialogsContainer, Type = typeof(Grid))]
    [TemplatePart(Name = PART_WindowTitleThumb, Type = typeof(Thumb))]
    public class MetroWindow : Window, IMetroWindow
    {
        #region fields
        internal Grid _OverlayBox;
        internal Grid _MetroActiveDialogContainer;
        internal Grid _MetroInactiveDialogContainer;

        private const string PART_OverlayBox = "PART_OverlayBox";
        private const string PART_MetroActiveDialogContainer = "PART_MetroActiveDialogContainer";
        private const string PART_MetroInactiveDialogsContainer = "PART_MetroInactiveDialogsContainer";
        private const string PART_WindowTitleThumb = "PART_WindowTitleThumb";

        private IInputElement _RestoreFocus;
        private Storyboard _OverlayStoryboard;
        private Thumb _WindowTitleThumb;
        #endregion fields

        #region ctor
        /// <summary>
        /// Static constructor
        /// </summary>
        static MetroWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroWindow), new FrameworkPropertyMetadata(typeof(MetroWindow)));
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public MetroWindow()
        {
            this.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, this.OnCloseWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, this.OnMaximizeWindow, this.OnCanResizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, this.OnMinimizeWindow, this.OnCanMinimizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, this.OnRestoreWindow, this.OnCanResizeWindow));

            this.SourceInitialized += new EventHandler(Window1_SourceInitialized);
        }
        #endregion ctor

        #region properties
        public Grid OverlayBox
        {
            get { return _OverlayBox; }
        }

        public Grid MetroActiveDialogContainer
        {
            get { return _MetroActiveDialogContainer; }
        }

        public Grid MetroInactiveDialogContainer
        {
            get { return _MetroInactiveDialogContainer; }
        }

////        #region MetroDialogOptionsMyRegion
////        public static readonly DependencyProperty MetroDialogOptionsProperty =
////            DependencyProperty.Register("MetroDialogOptions"
////                  , typeof(MWindowInterfacesLib.Interfaces.IMessageDialogSettings)
////                  , typeof(MetroWindow)
////                , new PropertyMetadata(new MWindowInterfacesLib.MetroDialogSettings()));
////
////        /// <summary>
////        /// Gets/sets the standard <seealso cref="IMessageDialogSettings"/> that are used for any
////        /// dialog that opens below this windows without specifying the ContentDialog settings.
////        /// 
////        /// This property contains the default ContentDialog settings that can be set at run-time.
////        /// /// </summary>
////        public IMessageDialogSettings MetroDialogOptions
////        {
////            get { return (IMessageDialogSettings)GetValue(MetroDialogOptionsProperty); }
////            set { SetValue(MetroDialogOptionsProperty, value); }
////        }
////        #endregion MetroDialogOptions

        #region Window Icon
        private static readonly DependencyProperty ShowIconProperty = DependencyProperty.Register("ShowIcon", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        /// <summary>
        /// Gets/sets if the close button is visible.
        /// </summary>
        public bool ShowIcon
        {
            get { return (bool)GetValue(ShowIconProperty); }
            set { SetValue(ShowIconProperty, value); }
        }
        #endregion Window Icon

        #region Window Title
        private static readonly DependencyProperty ShowTitleProperty = DependencyProperty.Register("ShowTitle", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        /// <summary>
        /// Gets/sets if the close button is visible.
        /// </summary>
        public bool ShowTitle
        {
            get { return (bool)GetValue(ShowTitleProperty); }
            set { SetValue(ShowTitleProperty, value); }
        }
        #endregion Window Title

        #region Window Min Button
        private static readonly DependencyProperty ShowMinButtonProperty = DependencyProperty.Register("ShowMinButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        /// <summary>
        /// Gets/sets if the close button is visible.
        /// </summary>
        public bool ShowMinButton
        {
            get { return (bool)GetValue(ShowMinButtonProperty); }
            set { SetValue(ShowMinButtonProperty, value); }
        }
        #endregion Window Min Button

        #region Window Max Button
        private static readonly DependencyProperty ShowMaxButtonProperty = DependencyProperty.Register("ShowMaxButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        /// <summary>
        /// Gets/sets if the close button is visible.
        /// </summary>
        public bool ShowMaxButton
        {
            get { return (bool)GetValue(ShowMaxButtonProperty); }
            set { SetValue(ShowMaxButtonProperty, value); }
        }
        #endregion Window Max Button

        #region Window Close Button
        private static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.Register("ShowCloseButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        /// <summary>
        /// Gets/sets if the close button is visible.
        /// </summary>
        public bool ShowCloseButton
        {
            get { return (bool)GetValue(ShowCloseButtonProperty); }
            set { SetValue(ShowCloseButtonProperty, value); }
        }
        #endregion Window Close Button

        #region Show TitleBar
        private static readonly DependencyProperty ShowTitleBarProperty = DependencyProperty.Register("ShowTitleBar", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true, OnShowTitleBarPropertyChangedCallback, OnShowTitleBarCoerceValueCallback));

        /// <summary>
        /// Gets/sets whether the TitleBar is visible or not.
        /// </summary>
        public bool ShowTitleBar
        {
            get { return (bool)GetValue(ShowTitleBarProperty); }
            set { SetValue(ShowTitleBarProperty, value); }
        }

        private static void OnShowTitleBarPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (MetroWindow)d;
            if (e.NewValue != e.OldValue)
            {
                window.SetVisibiltyForAllTitleElements((bool)e.NewValue);
            }
        }

        private static object OnShowTitleBarCoerceValueCallback(DependencyObject d, object value)
        {
            // if UseNoneWindowStyle = true no title bar should be shown
            if (((MetroWindow)d).UseNoneWindowStyle)
            {
                return false;
            }
            return value;
        }
        #endregion Show TitleBar

        #region UseNoneWindowStyleMyRegion
        private static readonly DependencyProperty UseNoneWindowStyleProperty = DependencyProperty.Register("UseNoneWindowStyle", typeof(bool), typeof(MetroWindow), new PropertyMetadata(false, OnUseNoneWindowStylePropertyChangedCallback));

        /// <summary>
        /// Gets/sets whether the WindowStyle is None or not.
        /// Setting UseNoneWindowStyle="True" on a <seealso cref="MetroWindow"/>
        /// is equivalent to not showing the titlebar of the window.
        /// </summary>
        public bool UseNoneWindowStyle
        {
            get { return (bool)GetValue(UseNoneWindowStyleProperty); }
            set { SetValue(UseNoneWindowStyleProperty, value); }
        }

        private static void OnUseNoneWindowStylePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                // if UseNoneWindowStyle = true no title bar should be shown
                var useNoneWindowStyle = (bool)e.NewValue;
                var window = (MetroWindow)d;
                window.ToggleNoneWindowStyle(useNoneWindowStyle);
            }
        }

        private void ToggleNoneWindowStyle(bool useNoneWindowStyle)
        {
            // UseNoneWindowStyle means no title bar, window commands or min, max, close buttons
            if (useNoneWindowStyle)
            {
                ShowTitleBar = false;
            }

            ////            if (LeftWindowCommandsPresenter != null)
            ////            {
            ////                LeftWindowCommandsPresenter.Visibility = useNoneWindowStyle ? Visibility.Collapsed : Visibility.Visible;
            ////            }
            ////            if (RightWindowCommandsPresenter != null)
            ////            {
            ////                RightWindowCommandsPresenter.Visibility = useNoneWindowStyle ? Visibility.Collapsed : Visibility.Visible;
            ////            }
        }
        #endregion UseNoneWindowStyle

        #region ShowDialogsOverTitleBar
        private static readonly DependencyProperty ShowDialogsOverTitleBarProperty = DependencyProperty.Register("ShowDialogsOverTitleBar", typeof(bool), typeof(MetroWindow), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Get/sets whether dialogs show over the title bar.
        /// </summary>
        public bool ShowDialogsOverTitleBar
        {
            get { return (bool)GetValue(ShowDialogsOverTitleBarProperty); }
            set { SetValue(ShowDialogsOverTitleBarProperty, value); }
        }
        #endregion ShowDialogsOverTitleBar

        #region IsWindowDraggable
        private static readonly DependencyProperty IsWindowDraggableProperty = DependencyProperty.Register("IsWindowDraggable", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        public bool IsWindowDraggable
        {
            get { return (bool)GetValue(IsWindowDraggableProperty); }
            set { SetValue(IsWindowDraggableProperty, value); }
        }
        #endregion IsWindowDraggable

        #region ShowSystemMenuOnRightClick
        private static readonly DependencyProperty ShowSystemMenuOnRightClickProperty = DependencyProperty.Register("ShowSystemMenuOnRightClick", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        /// <summary>
        /// Gets/sets if the the system menu should popup on right click.
        /// </summary>
        public bool ShowSystemMenuOnRightClick
        {
            get { return (bool)GetValue(ShowSystemMenuOnRightClickProperty); }
            set { SetValue(ShowSystemMenuOnRightClickProperty, value); }
        }
        #endregion ShowSystemMenuOnRightClick

        #region IsContentDialogVisible
        /// <summary>
        /// Determine whether a ContentDialog is currenlty shown inside the <seealso cref="MetroWindow"/> or not.
        /// </summary>
        public bool IsContentDialogVisible
        {
            get { return (bool)GetValue(IsContentDialogVisibleProperty); }
            set { SetValue(IsContentDialogVisibleProperty, value); }
        }

        /// <summary>
        /// Determine whether a ContentDialog is currenlty shown inside the <seealso cref="MetroWindow"/> or not.
        /// </summary>
        private static readonly DependencyProperty IsContentDialogVisibleProperty =
            DependencyProperty.Register("IsContentDialogVisible", typeof(bool), typeof(MetroWindow), new PropertyMetadata(false));
        #endregion IsContentDialogVisible
        #endregion properties

        #region methodes
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _OverlayBox = GetTemplateChild(PART_OverlayBox) as Grid;
            _MetroActiveDialogContainer = GetTemplateChild(PART_MetroActiveDialogContainer) as Grid;
            _MetroInactiveDialogContainer = GetTemplateChild(PART_MetroInactiveDialogsContainer) as Grid;
            _WindowTitleThumb = GetTemplateChild(PART_WindowTitleThumb) as Thumb;

            SetVisibiltyForAllTitleElements(true); // this.TitlebarHeight > 0
        }

        #region Overlay Methods
        /// <summary>
        /// Begins to show the MetroWindow's overlay effect.
        /// </summary>
        /// <returns>A task representing the process.</returns>
        public System.Threading.Tasks.Task ShowOverlayAsync()
        {
            if (_OverlayBox == null) throw new InvalidOperationException("OverlayBox can not be founded in this MetroWindow's template. Are you calling this before the window has loaded?");

            var tcs = new System.Threading.Tasks.TaskCompletionSource<object>();

            if (IsOverlayVisible() && _OverlayStoryboard == null)
            {
                //No Task.FromResult in .NET 4.
                tcs.SetResult(null);
                return tcs.Task;
            }

            Dispatcher.VerifyAccess();

            _OverlayBox.Visibility = Visibility.Visible;

            var sb = (Storyboard)this.Template.Resources["OverlayFastSemiFadeIn"];

            sb = sb.Clone();

            EventHandler completionHandler = null;
            completionHandler = (sender, args) =>
            {
                sb.Completed -= completionHandler;

                if (_OverlayStoryboard == sb)
                {
                    _OverlayStoryboard = null;

                    if (IsContentDialogVisible == false)
                        IsContentDialogVisible = true;
                }

                tcs.TrySetResult(null);
            };

            sb.Completed += completionHandler;

            _OverlayBox.BeginStoryboard(sb);

            _OverlayStoryboard = sb;

            return tcs.Task;
        }

        /// <summary>
        /// Begins to hide the MetroWindow's overlay effect.
        /// </summary>
        /// <returns>A task representing the process.</returns>
        public System.Threading.Tasks.Task HideOverlayAsync()
        {
            if (_OverlayBox == null)
                throw new InvalidOperationException("OverlayBox can not be founded in this MetroWindow's template. Are you calling this before the window has loaded?");

            var tcs = new System.Threading.Tasks.TaskCompletionSource<object>();

            if (_OverlayBox.Visibility == Visibility.Visible && _OverlayBox.Opacity == 0.0)
            {
                //No Task.FromResult in .NET 4.
                tcs.SetResult(null);
                return tcs.Task;
            }

            Dispatcher.VerifyAccess();

            var sb = (Storyboard)this.Template.Resources["OverlayFastSemiFadeOut"];

            sb = sb.Clone();

            EventHandler completionHandler = null;
            completionHandler = (sender, args) =>
            {
                sb.Completed -= completionHandler;

                if (_OverlayStoryboard == sb)
                {
                    _OverlayBox.Visibility = Visibility.Hidden;
                    _OverlayStoryboard = null;

                    if (IsContentDialogVisible == true)
                        IsContentDialogVisible = false;
                }

                tcs.TrySetResult(null);
            };

            sb.Completed += completionHandler;

            _OverlayBox.BeginStoryboard(sb);

            _OverlayStoryboard = sb;

            return tcs.Task;
        }

        public bool IsOverlayVisible()
        {
            if (_OverlayBox == null)
                throw new InvalidOperationException("OverlayBox can not be founded in this MetroWindow's template. Are you calling this before the window has loaded?");

            return _OverlayBox.Visibility == Visibility.Visible && _OverlayBox.Opacity >= 0.7;
        }

        public void ShowOverlay()
        {
            _OverlayBox.Visibility = Visibility.Visible;
            //overlayBox.Opacity = 0.7;
            _OverlayBox.SetCurrentValue(Grid.OpacityProperty, 0.5);

            if (IsContentDialogVisible == false)
                IsContentDialogVisible = true;
        }

        public void HideOverlay()
        {
            //overlayBox.Opacity = 0.0;
            _OverlayBox.SetCurrentValue(Grid.OpacityProperty, 0.0);
            _OverlayBox.Visibility = System.Windows.Visibility.Hidden;

            if (IsContentDialogVisible == true)
                IsContentDialogVisible = false;
        }

        /// <summary>
        /// Stores the given element, or the last focused element via FocusManager, for restoring the focus after closing a dialog.
        /// </summary>
        /// <param name="thisElement">The element which will be focused again.</param>
        public void StoreFocus(IInputElement thisElement = null) // [CanBeNull] 
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                _RestoreFocus = thisElement ?? (this._RestoreFocus ?? FocusManager.GetFocusedElement(this));
            }));
        }

        public void RestoreFocus()
        {
            if (_RestoreFocus != null)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    Keyboard.Focus(_RestoreFocus);
                    _RestoreFocus = null;
                }));
            }
        }

        /// <summary>
        /// Clears the stored element which would get the focus after closing a dialog.
        /// </summary>
        public void ResetStoredFocus()
        {
            _RestoreFocus = null;
        }
        #endregion Overlay Methods

        public Task<TDialog> GetCurrentDialogAsync<TDialog>() where TDialog : IBaseMetroDialogFrame
        {
            this.Dispatcher.VerifyAccess();
            var t = new TaskCompletionSource<TDialog>();
            this.Dispatcher.Invoke((Action)(() =>
            {
                TDialog dialog = default(TDialog);

                if (this.MetroActiveDialogContainer != null)
                {
                    dialog = this.MetroActiveDialogContainer.Children.OfType<TDialog>().LastOrDefault();
                    t.TrySetResult(dialog);
                }
            }));
            return t.Task;
        }

        /// <summary>
        /// Method connects the <see cref="Thumb"/> object on the window chrome
        /// with the correct drag events to let user drag the window on the screen.
        /// </summary>
        /// <param name="windowTitleThumb"></param>
        public void SetWindowEvents(Thumb windowTitleThumb)
        {
            this.ClearWindowEvents(); // clear all event handlers first

            if (windowTitleThumb != null)
            {
                windowTitleThumb.PreviewMouseLeftButtonUp += WindowTitleThumbOnPreviewMouseLeftButtonUp;
                windowTitleThumb.DragDelta += this.WindowTitleThumbMoveOnDragDelta;
                windowTitleThumb.MouseDoubleClick += this.WindowTitleThumbChangeWindowStateOnMouseDoubleClick;
                windowTitleThumb.MouseRightButtonUp += this.WindowTitleThumbSystemMenuOnMouseRightButtonUp;

                // Replace old referenc to Thumb since we seem to be looking at another instance
                // This could be a Thumb that overlays the window, for example, in a view
                // So, the Thumb may live in a view placed into the window to support dragging from there.
                if (_WindowTitleThumb != windowTitleThumb)
                    _WindowTitleThumb = windowTitleThumb;
            }
        }

        /// <summary>
        /// Gets the template child with the given name.
        /// </summary>
        /// <typeparam name="T">The interface type inheirted from DependencyObject.</typeparam>
        /// <param name="name">The name of the template child.</param>
        internal T GetPart<T>(string name) where T : class
        {
            return GetTemplateChild(name) as T;
        }

        /// <summary>
        /// Gets the template child with the given name.
        /// </summary>
        /// <param name="name">The name of the template child.</param>
        internal DependencyObject GetPart(string name)
        {
            return GetTemplateChild(name);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // #2409: don't close window if there is a dialog still open
            e.Cancel = this.IsContentDialogVisible;
            base.OnClosing(e);
        }

        protected IntPtr CriticalHandle
        {
            get
            {
                var value = typeof(Window)
                    .GetProperty("CriticalHandle", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(this, new object[0]);
                return (IntPtr)value;
            }
        }

        private void SetVisibiltyForAllTitleElements(bool visible)
        {
            SetWindowEvents(this._WindowTitleThumb);
        }

        private void ClearWindowEvents()
        {
            // clear all event handlers first:
            if (this._WindowTitleThumb != null)
            {
                this._WindowTitleThumb.PreviewMouseLeftButtonUp -= this.WindowTitleThumbOnPreviewMouseLeftButtonUp;
                this._WindowTitleThumb.DragDelta -= this.WindowTitleThumbMoveOnDragDelta;
                this._WindowTitleThumb.MouseDoubleClick -= this.WindowTitleThumbChangeWindowStateOnMouseDoubleClick;
                this._WindowTitleThumb.MouseRightButtonUp -= this.WindowTitleThumbSystemMenuOnMouseRightButtonUp;
            }
        }

        #region WindowTitleThumbEvents
        internal static void DoWindowTitleThumbOnPreviewMouseLeftButtonUp(MetroWindow window, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (mouseButtonEventArgs.Source == mouseButtonEventArgs.OriginalSource)
            {
                Mouse.Capture(null);
            }
        }

        internal static void DoWindowTitleThumbMoveOnDragDelta(IMetroThumb thumb, MetroWindow window, DragDeltaEventArgs dragDeltaEventArgs)
        {
            if (thumb == null)
            {
                throw new ArgumentNullException(nameof(thumb));
            }
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            // drag only if IsWindowDraggable is set to true
            if (!window.IsWindowDraggable ||
                (!(Math.Abs(dragDeltaEventArgs.HorizontalChange) > 2) && !(Math.Abs(dragDeltaEventArgs.VerticalChange) > 2)))
            {
                return;
            }

            // tage from DragMove internal code
            window.VerifyAccess();

            var cursorPos = NativeMethods.GetCursorPos();

            // if the window is maximized dragging is only allowed on title bar (also if not visible)
            var windowIsMaximized = window.WindowState == WindowState.Maximized;
            ////var isMouseOnTitlebar = Mouse.GetPosition(thumb).Y <= window.TitlebarHeight && window.TitlebarHeight > 0;
            ////if (!isMouseOnTitlebar && windowIsMaximized)
            ////{
            ////    return;
            ////}

            // for the touch usage
            UnsafeNativeMethods.ReleaseCapture();

            if (windowIsMaximized)
            {
                var cursorXPos = cursorPos.x;
                EventHandler windowOnStateChanged = null;
                windowOnStateChanged = (sender, args) =>
                {
                    //window.Top = 2;
                    //window.Left = Math.Max(cursorXPos - window.RestoreBounds.Width / 2, 0);

                    window.StateChanged -= windowOnStateChanged;
                    if (window.WindowState == WindowState.Normal)
                    {
                        Mouse.Capture(thumb, CaptureMode.Element);
                    }
                };
                window.StateChanged += windowOnStateChanged;
            }

            var criticalHandle = window.CriticalHandle;
            // DragMove works too
            // window.DragMove();
            // instead this 2 lines
            NativeMethods.SendMessage(criticalHandle, WM.SYSCOMMAND, (IntPtr)SC.MOUSEMOVE, IntPtr.Zero);
            NativeMethods.SendMessage(criticalHandle, WM.LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
        }

        internal static void DoWindowTitleThumbChangeWindowStateOnMouseDoubleClick(MetroWindow window, MouseButtonEventArgs mouseButtonEventArgs)
        {
            // restore/maximize only with left button
            if (mouseButtonEventArgs.ChangedButton == MouseButton.Left)
            {
                // we can maximize or restore the window if the title bar height is set (also if title bar is hidden)
                var canResize = window.ResizeMode == ResizeMode.CanResizeWithGrip || window.ResizeMode == ResizeMode.CanResize;
                var mousePos = Mouse.GetPosition(window);
                var isMouseOnTitlebar = true; //// var isMouseOnTitlebar = mousePos.Y <= window.TitlebarHeight && window.TitlebarHeight > 0;
                if (canResize && isMouseOnTitlebar)
                {
                    if (window.WindowState == WindowState.Maximized)
                    {
                        SystemCommands.RestoreWindow(window);
                    }
                    else
                    {
                        SystemCommands.MaximizeWindow(window);
                    }
                    mouseButtonEventArgs.Handled = true;
                }
            }
        }
        private static void ShowSystemMenuPhysicalCoordinates(Window window, Point physicalScreenLocation)
        {
            if (window == null) return;

            var hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero || !UnsafeNativeMethods.IsWindow(hwnd))
                return;

            var hmenu = UnsafeNativeMethods.GetSystemMenu(hwnd, false);

            var cmd = UnsafeNativeMethods.TrackPopupMenuEx(hmenu, Constants.TPM_LEFTBUTTON | Constants.TPM_RETURNCMD,
                (int)physicalScreenLocation.X, (int)physicalScreenLocation.Y, hwnd, IntPtr.Zero);
            if (0 != cmd)
                UnsafeNativeMethods.PostMessage(hwnd, Constants.SYSCOMMAND, new IntPtr(cmd), IntPtr.Zero);
        }

        internal static void DoWindowTitleThumbSystemMenuOnMouseRightButtonUp(MetroWindow window, MouseButtonEventArgs e)
        {
            if (window.ShowSystemMenuOnRightClick)
            {
                // show menu only if mouse pos is on title bar or if we have a window with none style and no title bar
                var mousePos = e.GetPosition(window);
                ////if ((mousePos.Y <= window.TitlebarHeight && window.TitlebarHeight > 0) || (window.UseNoneWindowStyle && window.TitlebarHeight <= 0))
                ////{
                    ShowSystemMenuPhysicalCoordinates(window, window.PointToScreen(mousePos));
                ////}
            }
        }

        private void WindowTitleThumbOnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DoWindowTitleThumbOnPreviewMouseLeftButtonUp(this, e);
        }

        private void WindowTitleThumbMoveOnDragDelta(object sender, DragDeltaEventArgs dragDeltaEventArgs)
        {
            DoWindowTitleThumbMoveOnDragDelta(sender as IMetroThumb, this, dragDeltaEventArgs);
        }

        private void WindowTitleThumbChangeWindowStateOnMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            DoWindowTitleThumbChangeWindowStateOnMouseDoubleClick(this, mouseButtonEventArgs);
        }

        private void WindowTitleThumbSystemMenuOnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            DoWindowTitleThumbSystemMenuOnMouseRightButtonUp(this, e);
        }
        #endregion WindowTitleThumbEvents

        private void Window1_SourceInitialized(object sender, EventArgs e)
        {
            Util.WindowSizing.WindowInitialized(this);
        }

        private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode != ResizeMode.NoResize;
        }

        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void OnRestoreWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }
        #endregion methodes
    }
}
