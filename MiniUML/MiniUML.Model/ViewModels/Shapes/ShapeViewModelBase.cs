namespace MiniUML.Model.ViewModels.Shapes
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Xml;
    using MiniUML.Framework;
    using MiniUML.Framework.Command;
    using MiniUML.Model.ViewModels.Interfaces;

    /// <summary>
    /// Base class to manage data items for
    /// each shape that is visible and resizeable on the canvas.
    /// </summary>
    public abstract class ShapeViewModelBase : BaseViewModel, IShapeViewModelBase
    {
        #region fields
        private readonly ObservableCollection<ShapeViewModelBase> mElements = new ObservableCollection<ShapeViewModelBase>();

        private string _ID = string.Empty;
        private string _TypeKey = string.Empty;
        private string _UmlControlKey = string.Empty;
        private string _Name = string.Empty;
        private bool _IsSelected = false;

        private double _Left = 0;
        private double _Top = 0;

        private RelayCommand<object> _BringToFront = null;
        private RelayCommand<object> _SendToBack = null;

        private IShapeParent _Parent = null;
        #endregion fields

        #region constructor
        /// <summary>
        /// Standard contructor
        /// </summary>
        public ShapeViewModelBase(IShapeParent parent)
        {
            this._Parent = parent;
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Name of XML element that will repesent this object in data.
        /// </summary>
        public abstract string XElementName
        {
            get;
        }

        /// <summary>
        /// Get/set unique identifier for this shape.
        /// </summary>
        public string ID
        {
            get
            {
                return this._ID;
            }

            set
            {
                if (this._ID != value)
                {
                    this._ID = value;
                    this.NotifyPropertyChanged(() => this.ID);
                }
            }
        }

        public abstract string TypeKey { get; }

        /// <summary>
        /// Get/set label (short string that is usually
        /// displayed below shape) for this shape.
        /// </summary>
        public string Name
        {
            get
            {
                return this._Name;
            }

            set
            {
                if (this._Name != value)
                {
                    this._Name = value;
                    this.NotifyPropertyChanged(() => this.Name);
                }
            }
        }

        #region shape position
        /// <summary>
        /// Get/set X-positon of this shape.
        /// </summary>
        public double Left
        {
            get
            {
                return this._Left;
            }

            set
            {
                if (_Left != value)
                {
                    _Left = value;
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
                return this._Top;
            }

            set
            {
                if (_Top != value)
                {
                    _Top = value;
                    NotifyPropertyChanged(() => Top);
                    NotifyPropertyChanged(() => Position);
                }
            }
        }

        /// <summary>
        /// Get/set X,Y-positon of this shape.
        /// </summary>
        public Point Position
        {
            get
            {
                return new Point(Left, Top);
            }

            set
            {
                if (Point.Equals(value, new Point(Left, Top)) == false)
                {
                    Left = value.X;
                    Top = value.Y;

                    NotifyPropertyChanged(() => Position);
                    NotifyPropertyChanged(() => Top);
                    NotifyPropertyChanged(() => Left);
                }
            }
        }
        #endregion shape position

        /// <summary>
        /// Get/set whether this shape is currently selected or not.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }

            internal set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    NotifyPropertyChanged(() => IsSelected);
                }
            }
        }

        #region Commands
        /// <summary>
        /// Get ICommand that implements Bring to Front functinoality for this shape.
        /// </summary>
        public RelayCommand<object> BringToFront
        {
            get
            {
                if (_BringToFront == null)
                    _BringToFront = new RelayCommand<object>(p => this.BringToFront_Executed());

                return _BringToFront;
            }
        }

        /// <summary>
        /// Get ICommand that implements Send to Back functinoality for this shape.
        /// </summary>
        public RelayCommand<object> SendToBack
        {
            get
            {
                if (this._SendToBack == null)
                    this._SendToBack = new RelayCommand<object>(p => this.SendToBack_Executed());

                return this._SendToBack;
            }
        }
        #endregion Commands

        /// <summary>
        /// Get first child node (if any) or null.
        /// (Simple clr - Non-viewmodel property)
        /// </summary>
        /// <returns></returns>
        public ShapeViewModelBase FirstNode
        {
            get
            {
                if (this.mElements.Count == 0)
                    return null;

                return this.mElements[0];
            }
        }

        /// <summary>
        /// Get last child node (if any) or null.
        /// (Simple clr - Non-viewmodel property)
        /// </summary>
        /// <returns></returns>
        public ShapeViewModelBase LastNode
        {
            get
            {
                if (this.mElements.Count == 0)
                    return null;

                return this.mElements[this.mElements.Count - 1];
            }
        }

        protected IShapeParent Parent
        {
            get
            {
                return this._Parent;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Add child objects
        /// into the collection of child objects.
        /// </summary>
        /// <param name="shape"></param>
        public void Add(ShapeViewModelBase shape)
        {
            this.mElements.Add(shape);
        }

        /// <summary>
        /// Add child objects from a given collection
        /// into the collection of child objects.
        /// </summary>
        /// <param name="shapes"></param>
        public void Add(IEnumerable<ShapeViewModelBase> shapes)
        {
            if (shapes != null)
            {
                foreach (var item in shapes)
                {
                    this.mElements.Add(item);
                }
            }
        }

        /// <summary>
        /// Remove this object instance from the parent (if any)
        /// </summary>
        public void Remove()
        {
            if (this._Parent != null)
                this._Parent.Remove(this);
        }

        /// <summary>
        /// Persist the contents of this object into the given
        /// parameter <paramref name="writer"/> object.
        /// 
        /// Inheriting classes have to overwrite this method to provide
        /// their custom persistence method.
        /// </summary>
        /// <param name="writer"></param>
        public abstract void SaveDocument(XmlWriter writer, IEnumerable<ShapeViewModelBase> root);

        /// <summary>
        /// Gets a collection of shapes stored in this shapes object.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ShapeViewModelBase> Elements()
        {
            return (IEnumerable<ShapeViewModelBase>)this.mElements;
        }

        /// <summary>
        /// Gets a count of the elements in the <see cref="Elements"/> collection.
        /// </summary>
        /// <returns></returns>
        public int ElementsCount()
        {
            return this.mElements.Count;
        }

        /// <summary>
        /// Brings the shape into front of the canvas view
        /// (moves shape on top of virtual Z-axis)
        /// </summary>
        protected void BringToFront_Executed()
        {
            if (this._Parent != null)
                this._Parent.BringToFront(this);
        }

        /// <summary>
        /// Brings the shape into the back of the canvas view
        /// (moves shape to the bottom of virtual Z-axis)
        /// </summary>
        protected void SendToBack_Executed()
        {
            if (this._Parent != null)
                this._Parent.SendToBack(this);
        }
        #endregion methods
    }
}
