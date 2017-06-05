namespace MiniUML.Framework
{
  using System;
  using System.ComponentModel;
  using System.Diagnostics;
  using System.Linq.Expressions;
  using System.Windows.Threading;

  /// <summary>
  /// Base class for data models. All public methods must be called on the instantiating thread only.
  /// </summary>
  public abstract class DataModel : DispatcherObject, INotifyPropertyChanged
  {
    #region fields
    private ModelState mState = ModelState.Invalid;
    private PropertyChangedEventHandler mPropertyChangedEvent;
    #endregion fields

    /// <summary>
    /// PropertyChanged event for INotifyPropertyChanged implementation.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged
    {
      add
      {
        this.VerifyAccess();
        this.mPropertyChangedEvent += value;
      }

      remove
      {
        this.VerifyAccess();
        this.mPropertyChangedEvent -= value;
      }
    }

    /// <summary>
    /// Possible states for a DataModel.
    /// </summary>
    public enum ModelState
    {
      Invalid,    // The model is in an invalid state
      Loading,    // The model is loading data
      Saving,     // The model is saving data
      Ready,      // The model is ready
      Busy        // The model is busy (currently in the middle of being updated)
    }

    #region methods
    /// <summary>
    /// Gets or sets current state of the model.
    /// </summary>
    public ModelState State
    {
      get
      {
        this.VerifyAccess();
        return this.mState;
      }

      set
      {
        this.VerifyAccess();

        // Ensure that the state cannot be set to Ready while the model is invalid.
        if (value == ModelState.Ready && this.IsValid() == false)
          value = ModelState.Invalid;

        if (value != this.mState)
        {
          this.mState = value;
          this.SendPropertyChanged("State");
        }
      }
    }

    /// <summary>
    /// Utility method for use by subclasses to notify that a property has changed.
    /// </summary>
    /// <param name="propertyName">The names of the properties.</param>
    protected void SendPropertyChanged(params string[] propertyNames)
    {
      this.VerifyAccess();
      if (this.mPropertyChangedEvent != null)
      {
        foreach (string propertyName in propertyNames)
          this.mPropertyChangedEvent(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    protected void NotifyPropertyChanged<TProperty>(Expression<Func<TProperty>> property)
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

      this.SendPropertyChanged(memberExpression.Member.Name);
    }

    /// <summary>
    /// Determines if the model is valid. Override to provide custom behavior.
    /// Do not call the base version when overriding.
    /// </summary>
    /// <returns>True if the model is valid, otherwise false.</returns>
    protected virtual bool IsValid()
    {
      return true;
    }

    /// <summary>
    /// Verifies that the model is in an acceptable state.
    /// Throws InvalidOperationException if validation fails.
    /// </summary>
    /// <param name="acceptedStates">The acceptable states.</param>
    [DebuggerNonUserCode]
    protected void VerifyState(params ModelState[] acceptedStates)
    {
      foreach (ModelState s in acceptedStates)
      {
        if (this.State == s)
          return; // OK.
      }

      string msg = "The model state must be ";
      for (int i = 0; i < acceptedStates.Length; i++)
      {
        if (i > 0) msg += (i == acceptedStates.Length - 1 ? " or " : ", ");
        msg += acceptedStates[i];
      }

      throw new InvalidOperationException(msg);
    }
  }
  #endregion methods
}
