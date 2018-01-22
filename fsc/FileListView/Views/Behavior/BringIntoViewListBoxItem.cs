namespace FileListView.Views.Behavior
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;

  /// <summary>
  /// Class implements an attached behaviour to bring a selected ListBox item
  /// into view when selection is driven by the viewmodel (not the user).
  ///
  /// Source: http://stackoverflow.com/questions/8827489/scroll-wpf-listbox-to-the-selecteditem-set-in-code-in-a-view-model
  ///
  ///  &lt;ListBox ItemsSource="{Binding Path=MyList}"
  ///  SelectedItem="{Binding Path=MyItem, Mode=TwoWay}"
  ///  SelectionMode="Single" 
  ///  behaviors:BringIntoViewListBoxItem.ScrollSelectedIntoView="True">
  ///
  /// </summary>
  public static class BringIntoViewListBoxItem
  {
    /// <summary>
    /// Attached dependency property of this behaviour.
    /// </summary>
    public static readonly DependencyProperty ScrollSelectedIntoViewProperty =
        DependencyProperty.RegisterAttached("ScrollSelectedIntoView", typeof(bool), typeof(BringIntoViewListBoxItem),
                                            new UIPropertyMetadata(false, OnScrollSelectedIntoViewChanged));

    /// <summary>
    /// Gets the attached dependency property of this behaviour.
    /// </summary>
    public static bool GetScrollSelectedIntoView(ListBox listBox)
    {
      return (bool)listBox.GetValue(ScrollSelectedIntoViewProperty);
    }

    /// <summary>
    /// Sets the attached dependency property of this behaviour.
    /// </summary>
    public static void SetScrollSelectedIntoView(ListBox listBox, bool value)
    {
      listBox.SetValue(ScrollSelectedIntoViewProperty, value);
    }

    private static void OnScrollSelectedIntoViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var selector = d as Selector;
      if (selector == null) return;

      if (e.NewValue is bool == false)
        return;

      if ((bool)e.NewValue)
      {
        selector.AddHandler(Selector.SelectionChangedEvent, new RoutedEventHandler(ListBoxSelectionChangedHandler));
      }
      else
      {
        selector.RemoveHandler(Selector.SelectionChangedEvent, new RoutedEventHandler(ListBoxSelectionChangedHandler));
      }
    }

    private static void ListBoxSelectionChangedHandler(object sender, RoutedEventArgs e)
    {
      if (!(sender is ListBox)) return;

      var listBox = (sender as ListBox);
      if (listBox.SelectedItem != null)
      {
        listBox.Dispatcher.BeginInvoke(
            (Action)(() =>
                {
                  listBox.UpdateLayout();
                  if (listBox.SelectedItem != null)
                    listBox.ScrollIntoView(listBox.SelectedItem);
                }));
      }
    }
  }
}
