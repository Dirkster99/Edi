namespace MiniUML.Framework
{
  using System;
  using System.ComponentModel;
  using System.Linq.Expressions;

  public abstract class BaseViewModel : INotifyPropertyChanged
  {
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

      SendPropertyChanged(memberExpression.Member.Name);
    }

    /// <summary>
    /// Utility method for use by subclasses to notify that a property has changed.
    /// </summary>
    /// <param name="propertyName">The names of the properties.</param>
    protected void SendPropertyChanged(params string[] propertyNames)
    {
      if (PropertyChanged != null)
      {
        foreach (string propertyName in propertyNames)
          PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}
