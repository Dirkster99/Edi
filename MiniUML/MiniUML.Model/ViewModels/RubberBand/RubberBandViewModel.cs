namespace MiniUML.Model.ViewModels.RubberBand
{
    using System.Windows;
  using Framework;
  using View.Views.RubberBand;

  /// <summary>
  /// Base class to manage data items for each shape that is visible on the canvas.
  /// </summary>
  public class RubberBandViewModel : BaseViewModel
  {
    #region fields
    private double mLeft = 0;
    private double mTop = 0;
    private double mWidth = 0;
    private double mHeight = 0;

    private bool mIsVisible = false;
    private MouseSelection mSelect = MouseSelection.ReducedToNewSelection;
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard contructor
    /// </summary>
    public RubberBandViewModel()
    {
      Top = Left = 100;
      Height = Width = 200;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get/set X-positon of this shape.
    /// </summary>
    public double Left
    {
      get
      {
        return mLeft;
      }

      set
      {
        if (mLeft != value)
        {
          mLeft = value;
          NotifyPropertyChanged(() => Left);
          NotifyPropertyChanged(() => Position);
        }
      }
    }

    /// <summary>
    /// Get/set Y-positon of this shape.
    /// </summary>
    public double Top
    {
      get
      {
        return mTop;
      }

      set
      {
        if (mTop != value)
        {
          mTop = value;
          NotifyPropertyChanged(() => Top);
          NotifyPropertyChanged(() => Position);
        }
      }
    }

    /// <summary>
    /// Get/set X,Y-positon of top right corner of this shape.
    /// </summary>
    public Point Position
    {
      get
      {
        return new Point(Left, Top);
      }

      set
      {
        if (value != new Point(Left, Top))
        {
          Left = value.X;
          Top = value.Y;

          NotifyPropertyChanged(() => Position);
          NotifyPropertyChanged(() => Top);
          NotifyPropertyChanged(() => Left);
        }
      }
    }

    /// <summary>
    /// Get/set height of rubber band selection.
    /// </summary>
    public double Height
    {
      get
      {
        return mHeight;
      }

      set
      {
        if (mHeight != value)
        {
          mHeight = value;
          NotifyPropertyChanged(() => Height);
          NotifyPropertyChanged(() => EndPosition);
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public double Width
    {
      get
      {
        return mWidth;
      }

      set
      {
        if (mWidth != value)
        {
          mWidth = value;
          NotifyPropertyChanged(() => Width);
          NotifyPropertyChanged(() => EndPosition);
        }
      }
    }

    /// <summary>
    /// Get/set X,Y-positon of lower right corner of this shape.
    /// </summary>
    public Point EndPosition
    {
      get
      {
        return new Point(Left + Width, Top + Height);
      }

      set
      {
        if (value != new Point(Left, Top))
        {
          Width = value.X - Left;
          Height = value.Y - Top;

          NotifyPropertyChanged(() => EndPosition);
          NotifyPropertyChanged(() => Height);
          NotifyPropertyChanged(() => Width);
        }
      }
    }

    /// <summary>
    /// Get/set whether rubber band is visible on canvas or not.
    /// </summary>
    public bool IsVisible
    {
      get
      {
        return mIsVisible;
      }

      set
      {
        if (mIsVisible != value)
        {
          mIsVisible = value;
          NotifyPropertyChanged(() => IsVisible);
        }
      }
    }

    public MouseSelection Select
    {
      get
      {
        return mSelect;
      }

      set
      {
        if (mSelect != value)
        {
          mSelect = value;
          NotifyPropertyChanged(() => Select);
        }
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Retrun a new rubber band selection event object
    /// to be passed on to whom it may concern.
    /// </summary>
    /// <returns></returns>
    public RubberBandSelectionEventArgs GetSelectionEvent()
    {
      return new RubberBandSelectionEventArgs(Position.X, Position.Y,
                                              EndPosition.X, EndPosition.Y, Select);
    }
    #endregion methods
  }
}
