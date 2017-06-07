namespace SimpleControls.MRU.View
{
  using System;
  using System.Collections.Generic;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Media;

  /// <summary>
  /// ListViewColumnStretch
  /// </summary>
  public class ListViewColumns : DependencyObject
  {
    /// <summary>
    /// IsStretched dependency property which can be attached to GridView columns.
    /// </summary>
    public static readonly DependencyProperty StretchProperty =
        DependencyProperty.RegisterAttached("Stretch",
        typeof(bool),
        typeof(ListViewColumns),
        new UIPropertyMetadata(true, null, OnCoerceStretch));

    /// <summary>
    /// Gets the stretch.
    ///
    /// The <paramref name="obj"/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool GetStretch(DependencyObject obj)
    {
      return (bool)obj.GetValue(StretchProperty);
    }

    /// <summary>
    /// Sets the stretch.
    ///
    /// The obj.
    /// if set to true [value].
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetStretch(DependencyObject obj, bool value)
    {
      obj.SetValue(StretchProperty, value);
    }

    /// <summary>
    /// Called when [coerce stretch].
    ///
    /// If this callback seems unfamilar then please read
    /// the great blog post by Paul jackson found here. 
    /// http://compilewith.net/2007/08/wpf-dependency-properties.html
    /// The source.
    /// The value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static object OnCoerceStretch(DependencyObject source, object value)
    {
      ListView lv = (source as ListView);

      // Ensure we dont have an invalid dependancy object of type ListView.
      if (lv == null)
        throw new ArgumentException("This property may only be used on ListViews");

      // Setup our event handlers for this list view.
      lv.Loaded += new RoutedEventHandler(lv_Loaded);
      lv.SizeChanged += new SizeChangedEventHandler(lv_SizeChanged);
      return value;
    }

    /// <summary>
    /// Handles the SizeChanged event of the lv control.
    ///
    /// The source of the event.
    /// The instance containing the event data.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void lv_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      ListView lv = (sender as ListView);
      if (lv.IsLoaded)
      {
        // Set our initial widths.
        SetColumnWidths(lv);
      }
    }

    /// <summary>
    /// Handles the Loaded event of the lv control.
    ///
    /// The source of the event.
    /// The instance containing the event data.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void lv_Loaded(object sender, RoutedEventArgs e)
    {
      ListView lv = (sender as ListView);

      // Set our initial widths.
      SetColumnWidths(lv);
    }

    /// <summary>
    /// Sets the column widths.
    /// </summary>
    /// <param name="listView"></param>
    private static void SetColumnWidths(ListView listView)
    {
      // Pull the stretch columns fromt the tag property.
      List<GridViewColumn> columns = (listView.Tag as List<GridViewColumn>);
      double specifiedWidth = 0;
            if (listView.View is GridView gridView)
            {
                if (columns == null)
                {
                    // Instance if its our first run.
                    columns = new List<GridViewColumn>();

                    // Get all columns with no width having been set.
                    foreach (GridViewColumn column in gridView.Columns)
                    {
                        if (!(column.Width >= 0))
                            columns.Add(column);
                        else specifiedWidth += column.ActualWidth;
                    }
                }
                else
                {
                    // Get all columns with no width having been set.
                    foreach (GridViewColumn column in gridView.Columns)
                        if (!columns.Contains(column))
                            specifiedWidth += column.ActualWidth;
                }

                // Allocate remaining space equally.
                foreach (GridViewColumn column in columns)
                {
                    double newWidth = (listView.ActualWidth - specifiedWidth) / columns.Count;

                    if (newWidth - 10 >= 0)
                        column.Width = newWidth - 10;
                }

                // Store the columns in the TAG property for later use. 
                listView.Tag = columns;
            }
        }
  }
}
