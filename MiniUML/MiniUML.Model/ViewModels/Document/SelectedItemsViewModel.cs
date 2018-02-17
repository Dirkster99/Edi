﻿namespace MiniUML.Model.ViewModels.Document
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.Linq;
  using System.Windows.Input;
  using Framework;
  using Shapes;

  /// <summary>
  /// Viewmodel to manage selected items and exposes all
  /// selected items through readonly colleciton.
  /// </summary>
  public class SelectedItems : BaseViewModel
  {
    #region fields
    private ObservableCollection<ShapeViewModelBase> mSelectedShapes;
    private ReadOnlyObservableCollection<ShapeViewModelBase> mCollectionReadOnly;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    public SelectedItems()
    {
      mSelectedShapes = new ObservableCollection<ShapeViewModelBase>();
      mCollectionReadOnly = new ReadOnlyObservableCollection<ShapeViewModelBase>(mSelectedShapes);
    }
    #endregion constructor

    public event EventHandler SelectionChanged;

    #region properties
    /// <summary>
    /// Readonly collection property to expose selected items.
    /// </summary>
    public ReadOnlyObservableCollection<ShapeViewModelBase> Shapes
    {
      get
      {
        return mCollectionReadOnly;
      }

      set
      {
        SelectShapes(value);
      }
    }

    /// <summary>
    /// Count the number of selected items in the collection.
    /// </summary>
    public int Count
    {
      get
      {
        return mSelectedShapes.Count;
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Set the supplied <paramref name="shape"/> as selected.
    /// </summary>
    /// <param name="shape"></param>
    public void SelectShape(ShapeViewModelBase shape)
    {
      mSelectedShapes.CollectionChanged -= selectedShapes_CollectionChanged;

      // Reset all IsSelected properties to false
      mSelectedShapes.Select(c => { c.IsSelected = false; return c; }).ToList();
      mSelectedShapes.Clear();

      if (shape != null)
      {
        mSelectedShapes.Add(shape);
        shape.IsSelected = true;
      }

      mSelectedShapes.CollectionChanged += selectedShapes_CollectionChanged;
      selectedShapes_CollectionChanged(null, null);
    }

    /// <summary>
    /// Set the supplied <paramref name="shapes"/> as selected
    /// (is called by set method of property).
    /// </summary>
    /// <param name="shapes"></param>
    public void SelectShapes(IEnumerable<ShapeViewModelBase> shapes)
    {
      mSelectedShapes.CollectionChanged -= selectedShapes_CollectionChanged;

      // Reset all IsSelected properties to false
      mSelectedShapes.Select(c => { c.IsSelected = false; return c; }).ToList();
      mSelectedShapes.Clear();

      if (shapes != null)
      {
        foreach (ShapeViewModelBase shape in shapes)
        {
          mSelectedShapes.Add(shape);
          shape.IsSelected = true;
        }
      }

      mSelectedShapes.CollectionChanged += selectedShapes_CollectionChanged;
      selectedShapes_CollectionChanged(null, null);
    }

    /// <summary>
    /// Check whether a certain shape is selected or not
    /// (An alternative solution for implementing this function is to check the IsSelected property).
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public bool Contains(ShapeViewModelBase s)
    {
      return mSelectedShapes.Contains(s);
    }

    /// <summary>
    /// Clear the collection of selected items.
    /// </summary>
    public void Clear()
    {
      // Reset all IsSelected properties to false
      mSelectedShapes.Select(c => { c.IsSelected = false; return c; }).ToList();

      // clear this collection
      mSelectedShapes.Clear();
    }

    /// <summary>
    /// Add an item inteo the collection of selected items.
    /// </summary>
    public void Add(ShapeViewModelBase s)
    {
      mSelectedShapes.Add(s);
      s.IsSelected = true;
    }

    /// <summary>
    /// Remove an item from the collection of selected items.
    /// </summary>
    public void Remove(ShapeViewModelBase shape)
    {
      mSelectedShapes.Remove(shape);
      shape.IsSelected = false;
    }

    private void selectedShapes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      NotifyPropertyChanged(() => Shapes);

      if (SelectionChanged != null)
        SelectionChanged(this, new EventArgs());

      CommandManager.InvalidateRequerySuggested();
    }
    #endregion methods
  }
}
