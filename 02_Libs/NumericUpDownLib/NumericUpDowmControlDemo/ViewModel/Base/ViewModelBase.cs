namespace NumericUpDowmControlDemo.ViewModel.Base
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.ComponentModel;
  using System.Linq.Expressions;

  public class ViewModelBase : INotifyPropertyChanged
  {

    protected virtual void RaisePropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }


    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Tell bound controls (via WPF binding) to refresh their display.
    /// 
    /// Sample call: this.NotifyPropertyChanged(() => this.IsSelected);
    /// where 'this' is derived from <seealso cref="BaseViewModel"/>
    /// and IsSelected is a property.
    /// </summary>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="property"></param>
    public void NotifyPropertyChanged<TProperty>(Expression<Func<TProperty>> property)
    {
      var lambda = (LambdaExpression)property;
      MemberExpression memberExpression;

      if (lambda.Body is UnaryExpression)
      {
        var unaryExpression = (UnaryExpression)lambda.Body;
        memberExpression = (MemberExpression)unaryExpression.Operand;
      }
      else
        memberExpression = (MemberExpression)lambda.Body;

      this.RaisePropertyChanged(memberExpression.Member.Name);
    }
  }
}
