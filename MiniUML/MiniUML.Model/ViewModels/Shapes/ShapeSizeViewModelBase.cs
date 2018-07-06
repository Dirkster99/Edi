namespace MiniUML.Model.ViewModels.Shapes
{
    using System.Windows;
    using System.Windows.Input;
    using MiniUML.Framework.Command;
    using MiniUML.Model.Events;
    using MiniUML.Model.ViewModels.Interfaces;

    /// <summary>
    /// Base class to manage data items for each shape that
    /// is visible on the canvas.
    /// </summary>
    public abstract class ShapeSizeViewModelBase : ShapeViewModelBase, IShapeSizeViewModelBase
    {
        #region fields
        private double _Width = 0;
        private double _Height = 0;
        private double _MinWidth = 10;
        private double _MinHeight = 10;

        private RelayCommand<DragDeltaThumbEvent> _ResizeSelectedShapes = null;
        private RelayCommand<object> _AlignObjectsBottom = null;
        private RelayCommand<object> _AdjustShapesToSameSize = null;
        private RelayCommand<object> _DestributeShapes = null;
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
                return _Width;
            }

            set
            {
                if (_Width != value)
                {
                    _Width = value;
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
                return _Height;
            }

            set
            {
                if (_Height != value)
                {
                    _Height = value;
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
                return _MinWidth;
            }

            set
            {
                if (_MinWidth != value)
                {
                    _MinWidth = value;
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
                return _MinHeight;
            }

            set
            {
                if (_MinHeight != value)
                {
                    _MinHeight = value;
                    NotifyPropertyChanged(() => this.MinHeight);
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
                if (Point.Equals(value, new Point(Left, Top)) == false)
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
                if (_ResizeSelectedShapes == null)
                    _ResizeSelectedShapes = new RelayCommand<DragDeltaThumbEvent>(
                        (p) => ResizeSelectedShapes_Executed(p),
                        (p) => ResizeSelectedShapes_CanExecute());

                return _ResizeSelectedShapes;
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
                if (_AlignObjectsBottom == null)
                    _AlignObjectsBottom = new RelayCommand<object>(p =>
                    {
                        if (p is AlignShapes)
                        {
                            AlignShapes alignmentOption = (AlignShapes)p;

                            if (this.Parent != null)
                                this.Parent.AlignShapes(this, alignmentOption);
                        }
                    });

                return _AlignObjectsBottom;
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
                if (_AdjustShapesToSameSize == null)
                    _AdjustShapesToSameSize = new RelayCommand<object>(p =>
                    {
                        if (p is SameSize)
                        {
                            SameSize sameSize = (SameSize)p;

                            if (this.Parent != null)
                                this.Parent.AdjustShapesToSameSize(this, sameSize);
                        }
                    });

                return _AdjustShapesToSameSize;
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
                if (_DestributeShapes == null)
                    _DestributeShapes = new RelayCommand<object>(p =>
                    {
                        if (p is Destribute)
                        {
                            Destribute distibOption = (Destribute)p;

                            if (this.Parent != null)
                                this.Parent.DistributeShapes(distibOption);
                        }
                    });

                return _DestributeShapes;
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
