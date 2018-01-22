namespace FolderBrowser.Views.Behaviours
{
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  /// Class implements an attached behaviour to bring a selected TreeViewItem
  /// into view when selection is driven by the viewmodel (not the user).
  /// 
  /// Sample Usage:
  /// &lt;TreeView.ItemContainerStyle>
  ///   &lt;Style TargetType="{x:Type TreeViewItem}">
  ///     &lt;Setter Property="behav:TreeViewItemBehaviour.IsBroughtIntoViewWhenSelected" Value="True" />
  ///     &lt;Setter Property="IsExpanded" Value="{Binding IsExpanded, Mode=TwoWay}" />
  ///     &lt;Setter Property="IsSelected" Value="{Binding IsSelected, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
  ///
  ///     &lt;!-- Setter Property="FontWeight" Value="Normal" />
  ///     &lt;Style.Triggers>
  ///       &lt;Trigger Property="IsSelected" Value="True">
  ///         &lt;Setter Property="FontWeight" Value="Bold" />
  ///       &lt;/Trigger>
  ///     &lt;/Style.Triggers -->
  ///   &lt;/Style>
  /// &lt;/TreeView.ItemContainerStyle>
  /// 
  /// </summary>
  public static class TreeViewItemBehaviour
  {
    #region IsBroughtIntoViewWhenSelected
    #region IsBroughtIntoViewWhenSelectedDependencyProperty
    /// <summary>
    /// Log4net logger facility for this class.
    /// </summary>
    private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// Backing storage of the IsBroughtIntoViewWhenSelected dependency property.
    /// </summary>
    public static readonly DependencyProperty IsBroughtIntoViewWhenSelectedProperty =
        DependencyProperty.RegisterAttached(
        "IsBroughtIntoViewWhenSelected",
        typeof(bool),
        typeof(TreeViewItemBehaviour),
        new UIPropertyMetadata(false, OnIsBroughtIntoViewWhenSelectedChanged));

    /// <summary>
    /// Gets the value of the IsBroughtIntoViewWhenSelected dependency property.
    /// </summary>
    public static bool GetIsBroughtIntoViewWhenSelected(TreeViewItem treeViewItem)
    {
      return (bool)treeViewItem.GetValue(IsBroughtIntoViewWhenSelectedProperty);
    }

    /// <summary>
    /// Sets the value of the IsBroughtIntoViewWhenSelected dependency property.
    /// </summary>
    public static void SetIsBroughtIntoViewWhenSelected(TreeViewItem treeViewItem, bool value)
    {
      treeViewItem.SetValue(IsBroughtIntoViewWhenSelectedProperty, value);
    }
    #endregion IsBroughtIntoViewWhenSelectedDependencyProperty

    #region methods
    private static void OnIsBroughtIntoViewWhenSelectedChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
    {
      TreeViewItem item = depObj as TreeViewItem;
      if (item == null)
        return;

      if (e.NewValue is bool == false)
        return;

      if ((bool)e.NewValue)
      {
        item.Selected += item_Selected;
      }
      else
      {
        item.Selected -= item_Selected;
      }
    }

    private static void item_Selected(object sender, RoutedEventArgs e)
    {
      TreeViewItem item = e.OriginalSource as TreeViewItem;

      if (item != null)
      {
        Logger.Debug("Behaviour BringItem Into View");
        item.BringIntoView();
        ////item.Focus();
      }
    }
    #endregion methods
    #endregion // IsBroughtIntoViewWhenSelected
  }
}
