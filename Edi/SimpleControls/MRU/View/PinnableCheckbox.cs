namespace SimpleControls.MRU.View
{
  using System.Windows;
  using System.Windows.Controls;

  public class PinnableCheckbox : CheckBox
  {
    #region constructor
    static PinnableCheckbox()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PinnableCheckbox),
                new FrameworkPropertyMetadata(typeof(PinnableCheckbox)));
    }

    public PinnableCheckbox()
    {
    }
    #endregion constructor

    #region methods
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
    }
    #endregion methods
  }
}
