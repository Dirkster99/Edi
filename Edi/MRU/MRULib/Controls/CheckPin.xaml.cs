namespace MRULib.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// A CheckPin is inheritat from a <seealso cref="CheckBox"/> in order to use
    /// a custom ui (XAML) definition and a slightly extended MouseOver behaviour.
    /// 
    /// The control shows a faint unpinned pin on:
    /// - MouseOver or
    /// - on MouseOver on the control bound to IsMouseOverListViewItem - use someting
    ///   like the following XAML in the ControlTemplate of the hosting control:
    ///   
    ///  &lt;ControlTemplate.Triggers>
    ///    &lt;Trigger Property = "IsMouseOver" Value="True" >
    ///      &lt;Setter TargetName = "checkPin"  Property="IsMouseOverListViewItem" Value="True" />
    ///    &lt;/Trigger>
    ///  &lt;/ControlTemplate.Triggers>
    /// </summary>
    public class CheckPin : CheckBox
    {
        /// <summary>
        /// Static Class constructor
        /// </summary>
        static CheckPin()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckPin), new FrameworkPropertyMetadata(typeof(CheckPin)));
        }

        #region IsMouseOverListViewItem Dependency Property
        /// <summary>
        /// Bind this dependeny property to the item where the pin is placed into (eg. ListViewItem).
        /// The CheckPin ControlTemplate Trigger is setup to show a faint pin to inform the user via
        /// mouseover that this list is actually pinnable.
        /// </summary>
        public bool IsMouseOverListViewItem
        {
            get { return (bool)GetValue(IsMouseOverListViewItemProperty); }
            set { SetValue(IsMouseOverListViewItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMouseOverListViewItem.
        private static readonly DependencyProperty IsMouseOverListViewItemProperty =
            DependencyProperty.Register("IsMouseOverListViewItem",
                typeof(bool),
                typeof(CheckPin), new PropertyMetadata(false));
        #endregion IsMouseOverListViewItem Dependency Property
    }
}
