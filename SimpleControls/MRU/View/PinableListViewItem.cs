namespace SimpleControls.MRU.View
{
  using System.Windows;
  using System.Windows.Controls;

  public class PinableListViewItem : ListViewItem
  {
    private static readonly DependencyProperty IsMouseOverListViewItemProperty =
        DependencyProperty.Register("IsMouseOverListViewItem",
                                    typeof(bool),
                                    typeof(PinableListViewItem),
                                    new FrameworkPropertyMetadata(IsMouseOverListViewItemChanged));

    public bool IsMouseOverListViewItem
    {
      get { return (bool)this.GetValue(IsMouseOverListViewItemProperty); }

      set { this.SetValue(IsMouseOverListViewItemProperty, value); }
    }

    protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
    {
      base.OnMouseEnter(e);

      this.IsMouseOverListViewItem = true;
    }

    protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
    {
      base.OnMouseEnter(e);

      this.IsMouseOverListViewItem = false;
    }

    private static void IsMouseOverListViewItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      PinableListViewItem item = d as PinableListViewItem;

      if (item != null)
      {
        item.IsMouseOverListViewItem = (bool)e.NewValue;
      }
    }
  }
}
