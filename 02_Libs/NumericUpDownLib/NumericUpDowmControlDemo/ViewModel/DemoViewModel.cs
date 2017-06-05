using System.ComponentModel;
namespace NumericUpDowmControlDemo.ViewModel
{
  /// <summary>
  /// Viewmodel class to demonstrate the usage
  /// of a bound numeric up/down control.
  /// </summary>
  public class DemoViewModel : Base.ViewModelBase
  {
    #region fields
    private int mMyIntValue = 98;
    private int mMyIntMinimumValue = -3;
    private int mMyIntMaximumValue = 105;
    #endregion fields

    #region properties
    /// <summary>
    /// Get/set integer Value to be displayed in numeric up/down control
    /// </summary>
    public int MyIntValue
    {
      get
      {
        return this.mMyIntValue;
      }
      
      set
      {
        if (this.mMyIntValue != value)
        {
          this.mMyIntValue = value;
          this.NotifyPropertyChanged(() => this.MyIntValue);
        }
      }
    }

    /// <summary>
    /// Get/set minimum integer Value to be displayed in numeric up/down control
    /// </summary>
    public int MyIntMinimumValue
    {
      get
      {
        return this.mMyIntMinimumValue;
      }
      
      set
      {
        if (this.mMyIntMinimumValue != value)
        {
          this.mMyIntMinimumValue = value;
          this.NotifyPropertyChanged(() => this.MyIntMinimumValue);
        }
      }
    }

    /// <summary>
    /// Get/set maximum integer Value to be displayed in numeric up/down control
    /// </summary>
    public int MyIntMaximumValue
    {
      get
      {
        return this.mMyIntMaximumValue;
      }
      
      set
      {
        if (this.mMyIntMaximumValue != value)
        {
          this.mMyIntMaximumValue = value;
          this.NotifyPropertyChanged(() => this.MyIntMaximumValue);
        }
      }
    }

    /// <summary>
    /// Get/set maximum integer Value to be displayed in numeric up/down control
    /// </summary>
    public string MyToolTip
    {
      get
      {
        return string.Format("Enter a value between {0} and {1}", this.mMyIntMinimumValue, this.MyIntMaximumValue);
      }
    }
    #endregion properties
  }
}
