namespace MiniUML.Model.ViewModels.Shapes
{
  using System.Windows;
  using System.Windows.Controls.Primitives;
  using System.Windows.Input;
  using MiniUML.Framework.Command;
  using MiniUML.Model.Events;

  /// <summary>
  /// Base class to manage data items for each shape that is visible on the canvas.
  /// </summary>
  public abstract class ShapeSizeViewModelBase : ShapeViewModelBase
  {
    #region fields
    private double mWidth = 0;
    private double mHeight = 0;
    private double mMinWidth = 10;
    private double mMinHeight = 10;

    private RelayCommand<DragDeltaThumbEvent> mResizeSelectedShapes = null;
    private RelayCommand<object> mAlignObjectsBottom = null;
    private RelayCommand<object> mAdjustShapesToSameSize = null;
    private RelayCommand<object> mDestributeShapes = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard contructor
    /// </summary>
    public ShapeSizeViewModelBase(IShapeParent parent)
      : base(parent)
    {
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get/set width of the bound shape.
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
    /// Get/set height of the bound shape.
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
    /// Get/set minimum width of shape.
    /// </summary>
    public double MinWidth
    {
      get
      {
        return this.mMinWidth;
      }

      set
      {
        if (this.mMinWidth != value)
        {
          this.mMinWidth = value;
          this.NotifyPropertyChanged(() => this.MinWidth);
        }
      }
    }

    /// <summary>
    /// Get/set minimum height of shape.
    /// </summary>
    public double MinHeight
    {
      get
      {
        return this.mMinHeight;
      }

      set
      {
        if (this.mMinHeight != value)
        {
          this.mMinHeight = value;
          this.NotifyPropertyChanged(() => this.MinHeight);
        }
      }
    }

    /// <summary>
    /// Get/set bottom righ position of shape
    /// (use this with care as it will adjust
    /// the width and height of a shape).
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
    /// View elements (DesignerItem) can bind to this property to get a resize shape
    /// command across into the viewmodel. THis command resizes all currently selected
    /// shapes view the relative movement passed into this command.
    /// </summary>
    public ICommand ResizeSelectedShapesCommand
    {
      get
      {
        if (this.mResizeSelectedShapes == null)
          this.mResizeSelectedShapes = new RelayCommand<DragDeltaThumbEvent>((p) => this.ResizeSelectedShapes_Executed(p),
                                                                             (p) => this.ResizeSelectedShapes_CanExecute());

        return this.mResizeSelectedShapes;
      }
    }

    /// <summary>
    /// Align all selected shapes (if any) to a given shape (this).
    /// The actual alignment operation performed is defined by the command
    /// parameter of <seealso cref="AlignShapes"/> type.
    /// </summary>
    public RelayCommand<object> AlignShapes
    {
      get
      {
        if (this.mAlignObjectsBottom == null)
          this.mAlignObjectsBottom = new RelayCommand<object>(p =>
          {
            if (p is AlignShapes)
            {
              AlignShapes alignmentOption = (AlignShapes)p;

              if (this.Parent != null)
                this.Parent.AlignShapes(this, alignmentOption);
            }
          });

        return this.mAlignObjectsBottom;
      }
    }
  
    /// <summary>
    /// Get command to adjust the width and/or height of all selected shapes
    /// to the size of a given (this) shape. Command requires 3 parameters:
    /// 
    /// this            - A shape to adjust the size of all other shapes to (implizit parameter)
    /// SameSize        - enumeration member to determine actual sizing request (explizit)
    /// Selected shapes - shapes that are selected via CNTRL+Click or rubberband selection (implizit parameter)
    /// </summary>
    public RelayCommand<object> AdjustShapesToSameSize
    {
      get
      {
        if (this.mAdjustShapesToSameSize == null)
          this.mAdjustShapesToSameSize = new RelayCommand<object>(p =>
          {
            if (p is SameSize)
            {
              SameSize sameSize = (SameSize)p;

              if (this.Parent != null)
                this.Parent.AdjustShapesToSameSize(this, sameSize);
            }
          });

        return this.mAdjustShapesToSameSize;
      }
    }

    /// <summary>
    /// Distribute all selected shapes evenly over X or Y space on canvas.
    /// Command Parameter: <seealso cref="Destribute"/> enumeration member
    /// to request actual X or Y spacing option.
    /// </summary>
    public RelayCommand<object> DestributeShapes
    {
      get
      {
        if (this.mDestributeShapes == null)
          this.mDestributeShapes = new RelayCommand<object>(p =>
          {
            if (p is Destribute)
            {
              Destribute distibOption = (Destribute)p;

              if (this.Parent != null)
                this.Parent.DistributeShapes(distibOption);
            }
          });

        return this.mDestributeShapes;
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Move this shape based on its bottom right corner position coordinates.
    /// </summary>
    /// <param name="point"></param>
    internal void MoveEndPosition(Point point)
    {
      if (point == null)
        return;

      this.Left = point.X - this.Width;
      this.Top = point.Y - this.Height;
    }

    /// <summary>
    /// Move this shape based on its top left position coordinates.
    /// </summary>
    /// <param name="point"></param>
    internal void MovePosition(Point point)
    {
      if (point == null)
        return;

      this.Left = point.X;
      this.Top = point.Y;
    }

    private void ResizeSelectedShapes_Executed(DragDeltaThumbEvent e)
    {
      if (this.Parent != null)
        this.Parent.ResizeSelectedShapes(e);
    }

    private bool ResizeSelectedShapes_CanExecute()
    {
      if (this.Parent != null)
        return true;
      else
        return false;
    }
    #endregion methods
  }
}
