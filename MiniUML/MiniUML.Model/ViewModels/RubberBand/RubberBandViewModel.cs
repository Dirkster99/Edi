namespace MiniUML.Model.ViewModels.RubberBand
{
  using System;
  using System.Windows;
  using MiniUML.Framework;
  using MiniUML.View.Views.RubberBand;

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
      this.Top = this.Left = 100;
      this.Height = this.Width = 200;
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
        return this.mLeft;
      }

      set
      {
        if (this.mLeft != value)
        {
          this.mLeft = value;
          this.NotifyPropertyChanged(() => this.Left);
          this.NotifyPropertyChanged(() => this.Position);
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
        return this.mTop;
      }

      set
      {
        if (this.mTop != value)
        {
          this.mTop = value;
          this.NotifyPropertyChanged(() => this.Top);
          this.NotifyPropertyChanged(() => this.Position);
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
        return new Point(this.Left, this.Top);
      }

      set
      {
        if (value != new Point(this.Left, this.Top))
        {
          this.Left = value.X;
          this.Top = value.Y;

          this.NotifyPropertyChanged(() => this.Position);
          this.NotifyPropertyChanged(() => this.Top);
          this.NotifyPropertyChanged(() => this.Left);
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
        return this.mHeight;
      }

      set
      {
        if (this.mHeight != value)
        {
          this.mHeight = value;
          this.NotifyPropertyChanged(() => this.Height);
          this.NotifyPropertyChanged(() => this.EndPosition);
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
        return this.mWidth;
      }

      set
      {
        if (this.mWidth != value)
        {
          this.mWidth = value;
          this.NotifyPropertyChanged(() => this.Width);
          this.NotifyPropertyChanged(() => this.EndPosition);
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
        return new Point(this.Left + this.Width, this.Top + this.Height);
      }

      set
      {
        if (value != new Point(this.Left, this.Top))
        {
          this.Width = value.X - this.Left;
          this.Height = value.Y - this.Top;

          this.NotifyPropertyChanged(() => this.EndPosition);
          this.NotifyPropertyChanged(() => this.Height);
          this.NotifyPropertyChanged(() => this.Width);
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
        return this.mIsVisible;
      }

      set
      {
        if (this.mIsVisible != value)
        {
          this.mIsVisible = value;
          this.NotifyPropertyChanged(() => this.IsVisible);
        }
      }
    }

    public MouseSelection Select
    {
      get
      {
        return this.mSelect;
      }

      set
      {
        if (this.mSelect != value)
        {
          this.mSelect = value;
          this.NotifyPropertyChanged(() => this.Select);
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
      return new RubberBandSelectionEventArgs(this.Position.X, this.Position.Y,
                                              this.EndPosition.X, this.EndPosition.Y, this.Select);
    }
    #endregion methods
  }
}
