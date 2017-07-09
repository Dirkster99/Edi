namespace SimpleControls.WatermarkTextBox
{
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  /// Interaction logic for TextBoxWithWaterMark.xaml
  /// 
  /// Source: http://www.dotnetspark.com/kb/1716-create-watermark-textbox-wpf-application.aspx
  /// </summary>
  public partial class TextBoxWithWatermark : UserControl
  {
    #region fields
    private static readonly DependencyProperty LabelTextBoxProperty =
      DependencyProperty.Register("LabelTextBox", typeof(string), typeof(TextBoxWithWatermark));

    private static readonly DependencyProperty TextProperty =
      TextBox.TextProperty.AddOwner(typeof(TextBoxWithWatermark));

    private static readonly DependencyProperty WatermarkProperty =
      DependencyProperty.Register("Watermark", typeof(string), typeof(TextBoxWithWatermark));
    #endregion fields

    #region Static Constructor
    /// <summary>
    /// Static constructor
    /// </summary>
    static TextBoxWithWatermark()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxWithWatermark),
          new FrameworkPropertyMetadata(typeof(TextBoxWithWatermark)));
    }
    #endregion

    #region properties
    /// <summary>
    /// Declare a TextBox label dependency property
    /// </summary>
    public string LabelTextBox
    {
      // These proeprties can be bound to. The XAML for this control binds the Label's content to this.
      get { return (string)this.GetValue(TextBoxWithWatermark.LabelTextBoxProperty); }
      set { this.SetValue(TextBoxWithWatermark.LabelTextBoxProperty, value); }
    }

    /// <summary>
    /// Declare a TextBox Text dependency property
    /// </summary>
    public string Text
    {
      // These proeprties can be bound to. The XAML for this control binds the Label's content to this.
      get { return (string)this.GetValue(TextBoxWithWatermark.TextProperty); }
      set { this.SetValue(TextBoxWithWatermark.TextProperty, value); }
    }

    /// <summary>
    /// Declare a TextBox Watermark label dependency property
    /// </summary>
    public string Watermark
    {
      // These proeprties can be bound to. The XAML for this control binds the Watermark's content to this.
      get { return (string)this.GetValue(TextBoxWithWatermark.WatermarkProperty); }
      set { this.SetValue(TextBoxWithWatermark.WatermarkProperty, value); }
    }
    #endregion properties
  }
}
