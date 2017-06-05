namespace MiniUML.View.Views.RubberBand
{
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  /// Class to manage the resize view elements that are
  /// actually visible for the user.
  /// 
  /// This view design is based on the rubberband adorner published in
  /// http://www.codeproject.com/Articles/23871/WPF-Diagram-Designer-Part-3
  /// </summary>
  public class RubberBandChrome : Control
  {
    #region fields
    #endregion fields

    #region constructor
    /// <summary>
    /// Static class constructor
    /// </summary>
    static RubberBandChrome()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(RubberBandChrome),
       new FrameworkPropertyMetadata(typeof(RubberBandChrome)));
    }

    /// <summary>
    /// Class constructor
    /// </summary>
    public RubberBandChrome()
    {
    }
    #endregion constructor

    #region methods
    /// <summary>
    /// Standard method that is executed when the template 'skin' is applied
    /// (by the WPF framework) on the look-less control.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
    }
    #endregion methods
  }
}
