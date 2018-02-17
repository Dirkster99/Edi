namespace MiniUML.View.Views.ResizeAdorner.Decorators
{
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Documents;
  using Model.Events;
  using Adorners;

  /// <summary>
  /// The <seealso cref="ResizeDecorator"/> class connects the <seealso cref="DesignerItem"/>
  /// class with the <seealso cref="ResizeAdorner"/> class and manages the visibility of the Adorner.
  /// 
  /// The ShowDecorator dependency property can be used to show and hide the resizeadorner
  /// whenever a bound viewmodel property indicates either direction.
  /// 
  /// The resize adorner view design is baed on
  /// http://www.codeproject.com/Articles/22952/WPF-Diagram-Designer-Part-1
  /// </summary>
  public class ResizeDecorator : Control
  {
    #region fields
    /// <summary>
    /// Back storage field of ShowDecorator dependency property
    /// </summary>
    public static readonly DependencyProperty ShowDecoratorProperty =
        DependencyProperty.Register("ShowDecorator", typeof(bool), typeof(ResizeDecorator),
        new FrameworkPropertyMetadata(false, new PropertyChangedCallback(ShowDecoratorProperty_Changed)));

    private DragDeltaThumbEventHandler mDragDeltaAction_DelegateFunction;

    private Adorner mAdorner = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    public ResizeDecorator()
    {
      Unloaded += new RoutedEventHandler(ResizeDecorator_Unloaded);
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get/set show decorator dependency property
    /// which determines whether the resize decorator is shown or not.
    /// </summary>
    public bool ShowDecorator
    {
      get { return (bool)GetValue(ShowDecoratorProperty); }
      set { SetValue(ShowDecoratorProperty, value); }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Assign an event handler to handle the event in which
    /// a user drags a resize thumb causing a delta drag event.
    /// </summary>
    /// <param name="functionToAssign"></param>
    public void AssignDragDeltaEvent(DragDeltaThumbEventHandler functionToAssign)
    {
      mDragDeltaAction_DelegateFunction = functionToAssign;
    }

    /// <summary>
    /// Method is executed on change of the ShowDecorator dependency property.
    /// It calls the HideAdorner or ShowAdorner decorator method depending on
    /// the actual value that has changed in the ShowDecorator dependency property.
    /// dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void ShowDecoratorProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ResizeDecorator decorator = (ResizeDecorator)d;
      bool showDecorator = (bool)e.NewValue;

      if (showDecorator)
      {
        decorator.ShowAdorner();
      }
      else
      {
        decorator.HideAdorner();
      }
    }

    /// <summary>
    /// Switch visibility of adorner to Hidden.
    /// </summary>
    private void HideAdorner()
    {
      if (mAdorner != null)
      {
        mAdorner.Visibility = Visibility.Hidden;
      }
    }

    /// <summary>
    /// Switch the resize adorner to be visible
    /// (even constructs the adorner if not already constructed).
    /// </summary>
    private void ShowAdorner()
    {
      if (mAdorner == null)
      {
        AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);

        if (adornerLayer != null)
        {
          ContentControl designerItem = DataContext as ContentControl;
          
          ////Canvas canvas = VisualTreeHelper.GetParent(designerItem) as Canvas;
          mAdorner = new ResizeAdorner(designerItem, mDragDeltaAction_DelegateFunction);

          adornerLayer.Add(mAdorner);

          if (ShowDecorator)
            mAdorner.Visibility = Visibility.Visible;
          else
            mAdorner.Visibility = Visibility.Hidden;
        }
      }
      else
      {
        mAdorner.Visibility = Visibility.Visible;
      }
    }

    private void ResizeDecorator_Unloaded(object sender, RoutedEventArgs e)
    {
      if (mAdorner != null)
      {
        AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);

        if (adornerLayer != null)
          adornerLayer.Remove(mAdorner);

        mAdorner = null;
      }
    }
    #endregion methods
  }
}
