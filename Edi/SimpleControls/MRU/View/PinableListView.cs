namespace SimpleControls.MRU.View
{
  using System.Windows;
  using System.Windows.Controls;

  public class PinableListView : ListView
  {
    /// <summary>
    /// Static Standard Constructor
    /// 
    /// Getting CustomControl style from Themes/Generic.xaml does not work ???
    /// </summary>
    static PinableListView()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PinableListView),
                new FrameworkPropertyMetadata(typeof(PinableListView)));
    }

    /// <summary>
    /// Standard method is executed when control template is applied to lookless control.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
      return new PinableListViewItem();
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return item is PinableListViewItem;
    }
  }
}
