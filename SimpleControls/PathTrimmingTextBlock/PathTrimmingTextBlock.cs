namespace SimpleControls.PathTrimmingTextBlock
{
  using System;
  using System.ComponentModel;
  using System.Globalization;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Media;

  /// <summary>
  /// This PathTrimmingTextBlock textblock attaches itself to the events of a parent container and
  /// displays a trimmed path text when the size of the parent (container) is changed.
  /// 
  /// http://www.codeproject.com/Tips/467054/WPF-PathTrimmingTextBlock
  /// </summary>
  public class PathTrimmingTextBlock : TextBlock
  {
    #region fields
    /// <summary>
    /// Path dependency property that stores the trimmed path
    /// </summary>
    private static readonly DependencyProperty PathProperty =
        DependencyProperty.Register("Path",
                                    typeof(string),
                                    typeof(PathTrimmingTextBlock),
                                    new UIPropertyMetadata(string.Empty));

    private FrameworkElement mContainer;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class Constructor
    /// </summary>
    public PathTrimmingTextBlock()
    {
      this.mContainer = null;

      this.Loaded += new RoutedEventHandler(this.PathTrimmingTextBlock_Loaded);
      this.Unloaded += new RoutedEventHandler(this.PathTrimmingTextBlock_Unloaded);
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Path dependency property that stores the trimmed path
    /// </summary>
    public string Path
    {
      get { return (string)this.GetValue(PathProperty); }
      set { this.SetValue(PathProperty, value); }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Textblock is constructed and start its live - lets attach to the
    /// size changed event handler of the containing parent.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PathTrimmingTextBlock_Loaded(object sender, RoutedEventArgs e)
    {

            if (this.Parent is FrameworkElement p)
            {
                this.mContainer = p;
            }
            else
            {

                if (this.Parent is DependencyObject dp)
                {
                    for (DependencyObject parent = LogicalTreeHelper.GetParent(dp as DependencyObject);
                         parent != null;
                         parent = LogicalTreeHelper.GetParent(parent as DependencyObject))
                    {
                        p = parent as FrameworkElement;

                        if (p != null)
                            break;
                    }

                    this.mContainer = p;
                }
            }

            if (this.mContainer != null)
      {
        this.mContainer.SizeChanged += new SizeChangedEventHandler(this.container_SizeChanged);

        this.Text = this.GetTrimmedPath(this.mContainer.ActualWidth);
      }
      //// else
      ////  throw new InvalidOperationException("PathTrimmingTextBlock must have a container such as a Grid.");
    }

    /// <summary>
    /// Remove custom event handlers and clean-up on unload.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PathTrimmingTextBlock_Unloaded(object sender, RoutedEventArgs e)
    {
      if (this.mContainer != null)
        this.mContainer.SizeChanged -= this.container_SizeChanged;
    }

    /// <summary>
    /// Trim the containing text (path) accordingly whenever the parent container chnages its size.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void container_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (this.mContainer != null)
        this.Text = this.GetTrimmedPath(this.mContainer.ActualWidth);
    }

    /// <summary>
    /// Compute the text to display (with ellipsis) that fits the ActualWidth of the container
    /// </summary>
    /// <param name="width"></param>
    /// <returns></returns>
    private string GetTrimmedPath(double width)
    {
      string filename = string.Empty;
      string directory = string.Empty;

      try
      {
        filename = System.IO.Path.GetFileName(this.Path);
        directory = System.IO.Path.GetDirectoryName(this.Path);
      }
      catch (Exception)
      {
        directory = this.Path;
        filename = string.Empty;
      }

      bool widthOK = false;
      bool changedWidth = false;

      TextBlock block = new TextBlock();
      block.Style = this.Style;
      block.FontWeight = this.FontWeight;
      block.FontStyle = this.FontStyle;
      block.FontStretch = this.FontStretch;
      block.FontSize = this.FontSize;
      block.FontFamily = this.FontFamily;

      do
      {
        block.Text = "{0}...{1}".FormatWith(directory, filename);
        block.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

        widthOK = block.DesiredSize.Width < width;
        ////widthOK = formatted.Width < width;

        if (!widthOK)
        {
          if (directory.Length == 0)
            return "...\\" + filename;

          changedWidth = true;
          directory = directory.Substring(0, directory.Length - 1);
        }
      }
      while (!widthOK);

      if (!changedWidth)
      {
        return this.Path;
      }

      if (block != null)   // Optimize for speed
        return block.Text;

      return "{0}...{1}".FormatWith(directory, filename);
    }
    #endregion constructor
  }
}