namespace FileSystemModels.ViewModels.Base
{
  using System;
  using System.ComponentModel;
  using System.Linq.Expressions;

  /// <summary>
  /// Base of Viewmodel classes implemented in this assembly.
  /// </summary>
  public class ViewModelBase : INotifyPropertyChanged
  {
    #region constructor
    /// <summary>
    /// Standard <seealso cref="ViewModelBase"/> class constructor
    /// </summary>
    public ViewModelBase()
    {
    }
    #endregion constructor

    /// <summary>
    /// Standard event of the <seealso cref="INotifyPropertyChanged"/> interface.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #region methods
    /// <summary>
    /// Tell bound controls (via WPF binding) to refresh their display
    /// for the viewmodel property indicated as string.
    /// </summary>
    /// <param name="propName"></param>
    protected virtual void RaisePropertyChanged(string propName)
    {
      if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
    }

    /// <summary>
    /// Tell bound controls (via WPF binding) to refresh their display.
    /// 
    /// Sample call: this.OnPropertyChanged(() => this.IsSelected);
    /// where 'this' is derived from <seealso cref="ViewModelBase"/>
    /// and IsSelected is a property.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyExpression"></param>
    protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
    {
      if (propertyExpression.Body.NodeType == ExpressionType.MemberAccess)
      {
        var memberExpr = propertyExpression.Body as MemberExpression;
        string propertyName = memberExpr.Member.Name;
                RaisePropertyChanged(propertyName);
      }
    }
    #endregion methods
  }
}
