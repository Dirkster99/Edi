namespace Edi.Core.View.Pane
{
  using System;
  using System.Collections.Generic;
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  /// Select a tool window style for an instance of its view.
  /// 
  /// 1) Call RegisterStyle() method to initialize a new association
  ///    between view and viewmodel.
  ///
  /// 2) Call SelectStyle to determine the next best View
  ///    for a given content (viewmodel).
  /// 
  /// </summary>
  public class PanesStyleSelector : StyleSelector
  {
    #region fields
    private readonly Dictionary<Type, Style> _StyleDirectory = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    public PanesStyleSelector()
    {
      _StyleDirectory = new Dictionary<Type, Style>();
    }
    #endregion constructor

    #region methods
    /// <summary>
    /// Returns a System.Windows.Style based on custom logic.
    /// </summary>
    /// <param name="item">The content (usually a viewmodel).</param>
    /// <param name="container">The element to which the style will be applied.</param>
    /// <returns>Returns an application-specific style to apply; otherwise, null.</returns>
    public override System.Windows.Style SelectStyle(object item,
                                                     System.Windows.DependencyObject container)
    {
      if (item == null)
        return null;

      Style o;
      Type t = item.GetType();
      _StyleDirectory.TryGetValue(t, out o);

      if (o != null)
        return o;

      // Traverse backwards in the inheritance chain to find a mapping there
      //
      // https://stackoverflow.com/questions/8699053/how-to-check-if-a-class-inherits-another-class-without-instantiating-it
      // Lets use .net to check up the inheritance chain to determine
      // if we can return a style for an inheritated viewmodel instead
      // of using the direct viewmodel <-> style association.
      foreach (var vmItem in _StyleDirectory.Keys)
      {
          if (t.IsSubclassOf(vmItem) == true)
          {
              _StyleDirectory.TryGetValue(vmItem, out o);
              return o;
          }
      }

      return base.SelectStyle(item, container);
    }

    /// <summary>
    /// Register a (viewmodel) class type with a <seealso cref="Style"/> for a view.
    /// </summary>
    /// <param name="typeOfViewmodel"></param>
    /// <param name="styleOfView"></param>
    public void RegisterStyle(Type typeOfViewmodel, Style styleOfView)
    {
      _StyleDirectory.Add(typeOfViewmodel, styleOfView);
    }
    #endregion methods
  }
}
