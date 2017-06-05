namespace MiniUML.Model.ViewModels.Shapes
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Runtime;
  using System.Windows;
  using System.Xml;
  using System.Xml.Linq;
  using MiniUML.Framework;
  using MiniUML.Framework.Command;
  using MiniUML.Model.ViewModels.Document;

  /// <summary>
  /// Base class to manage data items for each shape that is visible on the canvas.
  /// </summary>
  public abstract class ShapeViewModelBase : BaseViewModel
  {
    #region fields
    private readonly ObservableCollection<ShapeViewModelBase> mElements = new ObservableCollection<ShapeViewModelBase>();

    private string mID = string.Empty;
    private string mTypeKey = string.Empty;
    private string mUmlControlKey = string.Empty;
    private string mName = string.Empty;
    private bool mIsSelected = false;

    private double mLeft = 0;
    private double mTop = 0;

    private RelayCommand<object> mBringToFront = null;
    private RelayCommand<object> mSendToBack = null;

    private IShapeParent mParent = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard contructor
    /// </summary>
    public ShapeViewModelBase(IShapeParent parent)
    {
      this.mParent = parent;
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
        return this.mID;
      }
      
      set
      {
        if (this.mID != value)
        {
          this.mID = value;
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
        return this.mName;
      }

      set
      {
        if (this.mName != value)
        {
          this.mName = value;
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
    /// Get/set X,Y-positon of this shape.
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
    #endregion shape position

    /// <summary>
    /// Get/set whether this shape is currently selected or not.
    /// </summary>
    public bool IsSelected
    {
      get
      {
        return this.mIsSelected;
      }

      internal set
      {
        if (this.mIsSelected != value)
        {
          this.mIsSelected = value;
          this.NotifyPropertyChanged(() => this.IsSelected);
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
        if (this.mBringToFront == null)
          this.mBringToFront = new RelayCommand<object>(p => this.BringToFront_Executed());

        return this.mBringToFront;
      }
    }

    /// <summary>
    /// Get ICommand that implements Send to Back functinoality for this shape.
    /// </summary>
    public RelayCommand<object> SendToBack
    {
      get
      {
        if (this.mSendToBack == null)
          this.mSendToBack = new RelayCommand<object>(p => this.SendToBack_Executed());

        return this.mSendToBack;
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
        return this.mParent;
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
      if (this.mParent != null)
        this.mParent.Remove(this);
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

    public IEnumerable<ShapeViewModelBase> Elements()
    {
      return (IEnumerable<ShapeViewModelBase>)this.mElements;
    }

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
      if (this.mParent != null)
        this.mParent.BringToFront(this);
    }

    /// <summary>
    /// Brings the shape into the back of the canvas view
    /// (moves shape to the bottom of virtual Z-axis)
    /// </summary>
    protected void SendToBack_Executed()
    {
      if (this.mParent != null)
        this.mParent.SendToBack(this);
    }
    #endregion methods
  }
}
