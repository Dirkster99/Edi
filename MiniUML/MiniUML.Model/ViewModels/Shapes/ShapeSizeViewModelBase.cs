namespace MiniUML.Model.ViewModels.Shapes
{
  using System.Windows;
  using System.Windows.Input;
  using Framework.Command;
  using Events;

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
    /// Get/set height of the bound shape.
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
    /// Get/set minimum width of shape.
    /// </summary>
    public double MinWidth
    {
      get
      {
        return mMinWidth;
      }

      set
      {
        if (mMinWidth != value)
        {
          mMinWidth = value;
          NotifyPropertyChanged(() => MinWidth);
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
        return mMinHeight;
      }

      set
      {
        if (mMinHeight != value)
        {
          mMinHeight = value;
          NotifyPropertyChanged(() => MinHeight);
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
    /// View elements (DesignerItem) can bind to this property to get a resize shape
    /// command across into the viewmodel. THis command resizes all currently selected
    /// shapes view the relative movement passed into this command.
    /// </summary>
    public ICommand ResizeSelectedShapesCommand
    {
      get
      {
        if (mResizeSelectedShapes == null)
          mResizeSelectedShapes = new RelayCommand<DragDeltaThumbEvent>((p) => ResizeSelectedShapes_Executed(p),
                                                                             (p) => ResizeSelectedShapes_CanExecute());

        return mResizeSelectedShapes;
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
        if (mAlignObjectsBottom == null)
          mAlignObjectsBottom = new RelayCommand<object>(p =>
          {
            if (p is AlignShapes)
            {
              AlignShapes alignmentOption = (AlignShapes)p;

              if (Parent != null)
                Parent.AlignShapes(this, alignmentOption);
            }
          });

        return mAlignObjectsBottom;
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
        if (mAdjustShapesToSameSize == null)
          mAdjustShapesToSameSize = new RelayCommand<object>(p =>
          {
            if (p is SameSize)
            {
              SameSize sameSize = (SameSize)p;

              if (Parent != null)
                Parent.AdjustShapesToSameSize(this, sameSize);
            }
          });

        return mAdjustShapesToSameSize;
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
        if (mDestributeShapes == null)
          mDestributeShapes = new RelayCommand<object>(p =>
          {
            if (p is Destribute)
            {
              Destribute distibOption = (Destribute)p;

              if (Parent != null)
                Parent.DistributeShapes(distibOption);
            }
          });

        return mDestributeShapes;
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

      Left = point.X - Width;
      Top = point.Y - Height;
    }

    /// <summary>
    /// Move this shape based on its top left position coordinates.
    /// </summary>
    /// <param name="point"></param>
    internal void MovePosition(Point point)
    {
      if (point == null)
        return;

      Left = point.X;
      Top = point.Y;
    }

    private void ResizeSelectedShapes_Executed(DragDeltaThumbEvent e)
    {
      if (Parent != null)
        Parent.ResizeSelectedShapes(e);
    }

    private bool ResizeSelectedShapes_CanExecute()
    {
      if (Parent != null)
        return true;
      else
        return false;
    }
    #endregion methods
  }
}
