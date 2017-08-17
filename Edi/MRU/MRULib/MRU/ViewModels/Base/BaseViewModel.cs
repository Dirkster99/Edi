namespace MRULib.MRU.ViewModels.Base
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;

    /// <summary>
    /// Every ViewModel class is required to implement the INotifyPropertyChanged
    /// interface in order to tell WPF when a property changed (for instance, when
    /// a method or setter is executed).
    /// 
    /// Therefore, the PropertyChanged methode has to be called when data changes,
    /// because the relevant properties may or may not be bound to GUI elements,
    /// which in turn have to refresh their display.
    /// 
    /// The PropertyChanged method is to be called by the members and properties of
    /// the class that derives from this class. Each call contains the name of the
    /// property that has to be refreshed.
    /// 
    /// The BaseViewModel is derived from from System.Windows.DependencyObject to allow
    /// resulting ViewModels the implemantion of dependency properties. Dependency properties
    /// in turn are useful when working with IValueConverter and ConverterParameters.
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Standard event handler of the <seealso cref="INotifyPropertyChanged"/> interface
        /// </summary>
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
        public void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> property)
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

            this.OnPropertyChanged(memberExpression.Member.Name);
        }

        /// <summary>
        /// Tell bound controls (via WPF binding) to refresh their display.
        /// 
        /// Sample call: this.OnPropertyChanged("IsSelected");
        /// where 'this' is derived from <seealso cref="BaseViewModel"/>
        /// and IsSelected is a property.
        /// </summary>
        /// <param name="propertyName">Name of property to refresh</param>
        private void OnPropertyChanged(string propertyName)
        {
            try
            {
                var handler = this.PropertyChanged;

                if (handler != null)
                    handler(this, new PropertyChangedEventArgs(propertyName));
            }
            catch
            {
            }
        }
    }
}
