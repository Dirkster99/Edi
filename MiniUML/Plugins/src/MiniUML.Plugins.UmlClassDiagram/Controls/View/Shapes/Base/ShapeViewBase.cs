namespace MiniUML.Plugins.UmlClassDiagram.Controls.View.Shapes.Base
{
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Input;
  using MiniUML.Model.ViewModels;
  using MiniUML.Model.ViewModels.Shapes;
  using MiniUML.View.Controls;
  using MiniUML.View.Views;

  /// <summary>
  /// Base class for all UML view elements that can be position on the canvas
  /// (except for connection type view elements).
  /// </summary>
  public class ShapeViewBase : Control, ISnapTarget
  {
    #region constructor
    /// <summary>
    /// class constructor
    /// </summary>
    public ShapeViewBase()
      : base()
    {
    }
    #endregion constructor

    public event SnapTargetUpdateHandler SnapTargetUpdate;

    #region methods
    public void SnapPoint(ref Point snapPosition, out double snapAngle)
    {
      snapAngle = 0;

      var cv = CanvasView.GetCanvasView(this);
      if (cv == null)
        return;

      Point pos = cv.ElementFromControl(this).Position;

      // An array of line segments, each line segment represented as four double values.
      double[] shape = {
                pos.X, pos.Y,
                pos.X, pos.Y + ActualHeight,

                pos.X, pos.Y + ActualHeight,
                pos.X + ActualWidth, pos.Y + ActualHeight,

                pos.X + ActualWidth, pos.Y + ActualHeight,
                pos.X + ActualWidth, pos.Y,

                pos.X + ActualWidth, pos.Y,
                pos.X, pos.Y,
            };

      Point bestSnapPoint = new Point(double.NaN, double.NaN);
      double bestSnapLengthSq = double.PositiveInfinity;

      for (int i = 0; i < shape.Length; i += 4)
      {
        Point from = new Point(shape[i], shape[i + 1]);
        Point to = new Point(shape[i + 2], shape[i + 3]);

        AnchorPoint.SnapToLineSegment(from, to, snapPosition, ref bestSnapLengthSq, ref bestSnapPoint, ref snapAngle);
      }

      snapPosition = bestSnapPoint;
    }

    public void NotifySnapTargetUpdate(SnapTargetUpdateEventArgs e)
    {
      if (this.SnapTargetUpdate != null)
        this.SnapTargetUpdate(this, e);
    }

    /// <summary>
    /// Use mouse click event to acquire focus
    /// (ApplicationCommands.Copy, Cut, Paste will not work otherwise) 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      base.OnMouseDown(e);

      if (this.IsFocused == false)
        this.Focus();

      e.Handled = true;
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
      base.OnRenderSizeChanged(sizeInfo);

      this.NotifySnapTargetUpdate(new SnapTargetUpdateEventArgs());
    }

    #region protected contextmenu creation methods
    /// <summary>
    /// Create a new context menu with standard entries like
    /// Copy, Cut, Paste, Undo, Redo, Bring To Front etc..
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    protected ContextMenu CreateContextMenu(ShapeViewModelBase element)
    {
      ContextMenu retMenu = new ContextMenu();

      this.AddCopyCutPasteMenuItems(retMenu);

      retMenu.Items.Add(new Separator());
      this.AddUndoRedoMenuItems(retMenu);

      this.AddZOrderMenuItems(retMenu, element);

      this.SameSizeMenuItems(retMenu, element as ShapeSizeViewModelBase);

      this.AlignMenuItems(retMenu, element as ShapeSizeViewModelBase);

      this.DistributeMenuItems(retMenu, element as ShapeSizeViewModelBase);

      return retMenu;
    }

    /// <summary>
    /// Add standard application Cut, Copy, Paste, and Deleete menu entries.
    /// </summary>
    /// <param name="menu"></param>
    protected void AddCopyCutPasteMenuItems(MenuBase menu)
    {
      menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Cut, Icon = IconImageFactory.Get(IconCommand.Cut) });
      menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Copy, Icon = IconImageFactory.Get(IconCommand.Copy) });
      menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Paste, Icon = IconImageFactory.Get(IconCommand.Paste) });
      menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Delete, Icon = IconImageFactory.Get(IconCommand.Delete) });
    }

    /// <summary>
    /// Add standard application Undo and Redo menu entries.
    /// </summary>
    /// <param name="menu"></param>
    protected void AddUndoRedoMenuItems(MenuBase menu)
    {
      menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Undo });
      menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Redo });
    }

    /// <summary>
    /// Add "Bring To Front" and "Send To Back" menu entries into the given menu.
    /// </summary>
    /// <param name="menu"></param>
    /// <param name="element"></param>
    protected void AddZOrderMenuItems(MenuBase menu, ShapeViewModelBase element)
    {
      try
      {
        if (element != null)
        {
          MenuItem submenu = new MenuItem() { Header = "Z-Order" };
          menu.Items.Add(submenu);

          submenu.Items.Add(new MenuItem()
          {
            Header = MiniUML.Framework.Local.Strings.STR_MENUITEM_BringToFront,
            Command = element.BringToFront,
            Icon = IconImageFactory.Get(IconCommand.SendToBack)
          });

          submenu.Items.Add(new MenuItem()
          {
            Header = MiniUML.Framework.Local.Strings.STR_MENUITEM_SendToBack,
            Command = element.SendToBack,
            Icon = IconImageFactory.Get(IconCommand.BringToFront)
          });
        }
      }
      catch
      {
      }
    }

    /// <summary>
    /// Construct commands, bindings, and menu items for shape alignment functions.
    /// (Align Top, Align Center etc).
    /// </summary>
    /// <param name="menu"></param>
    /// <param name="element"></param>
    protected void SameSizeMenuItems(MenuBase menu, ShapeSizeViewModelBase element)
    {
      try
      {
        if (element != null)
        {
          MenuItem submenu = new MenuItem() { Header = "Same Size" };
          menu.Items.Add(submenu);

          submenu.Items.Add(new MenuItem()
          {
            Header = "Same Width",
            Command = element.AdjustShapesToSameSize,
            CommandParameter = SameSize.SameWidth,
            Icon = IconImageFactory.Get(IconCommand.AlignObjectsLeft)
          });
          submenu.Items.Add(new MenuItem()
          {
            Header = "Same Height",
            Command = element.AdjustShapesToSameSize,
            CommandParameter = SameSize.SameHeight,
            Icon = IconImageFactory.Get(IconCommand.AlignObjectsTop)
          });
          submenu.Items.Add(new MenuItem()
          {
            Header = "Same Width and Height",
            Command = element.AdjustShapesToSameSize,
            CommandParameter = SameSize.SameWidthandHeight,
            Icon = IconImageFactory.Get(IconCommand.AlignObjectsRight)
          });
        }
      }
      catch
      {
      }
    }

    /// <summary>
    /// Construct commands, bindings, and menu items for shape alignment functions.
    /// (Align Top, Align Center etc).
    /// </summary>
    /// <param name="menu"></param>
    /// <param name="element"></param>
    protected void AlignMenuItems(MenuBase menu, ShapeSizeViewModelBase element)
    {
      try
      {
        if (element != null)
        {
          MenuItem submenu = new MenuItem() { Header = "Align" };
          menu.Items.Add(submenu);

          submenu.Items.Add(new MenuItem()
          {
            Header = "Align Objects Left",
            Command = element.AlignShapes,
            CommandParameter = AlignShapes.Left,
            Icon = IconImageFactory.Get(IconCommand.AlignObjectsLeft)
          });
          submenu.Items.Add(new MenuItem()
          {
            Header = "Align Objects Top",
            Command = element.AlignShapes,
            CommandParameter = AlignShapes.Top,
            Icon = IconImageFactory.Get(IconCommand.AlignObjectsTop)
          });
          submenu.Items.Add(new MenuItem()
          {
            Header = "Align Objects Right",
            Command = element.AlignShapes,
            CommandParameter = AlignShapes.Right,
            Icon = IconImageFactory.Get(IconCommand.AlignObjectsRight)
          });
          submenu.Items.Add(new MenuItem()
          {
            Header = "Align Objects Bottom",
            Command = element.AlignShapes,
            CommandParameter = AlignShapes.Bottom,
            Icon = IconImageFactory.Get(IconCommand.AlignObjectsBottom)
          });
          submenu.Items.Add(new Separator());
          submenu.Items.Add(new MenuItem()
          {
            Header = "Align Objects Centered Horizontal",
            Command = element.AlignShapes,
            CommandParameter = AlignShapes.CenteredHorizontal,
            Icon = IconImageFactory.Get(IconCommand.AlignObjectsCenteredHorizontal)
          });
          submenu.Items.Add(new MenuItem()
          {
            Header = "Align Objects Centered Vertical",
            Command = element.AlignShapes,
            CommandParameter = AlignShapes.CenteredVertical,
            Icon = IconImageFactory.Get(IconCommand.AlignObjectsCenteredVertical)
          });
        }
      }
      catch
      {
      }
    }

    /// <summary>
    /// Create menu entries to distribute shapes evenly over horizontal or vertical space.
    /// </summary>
    /// <param name="menu"></param>
    /// <param name="element"></param>
    protected void DistributeMenuItems(MenuBase menu, ShapeSizeViewModelBase element)
    {
      try
      {
        if (element != null)
        {
          MenuItem submenu = new MenuItem() { Header = "Distribute" };
          menu.Items.Add(submenu);

          submenu.Items.Add(new MenuItem()
          {
            Header = "Distribute Horizontally",
            Command = element.DestributeShapes,
            CommandParameter = Destribute.Horizontally
          });
          submenu.Items.Add(new MenuItem()
          {
            Header = "Distribute Vertically",
            Command = element.DestributeShapes,
            CommandParameter = Destribute.Vertically
          });
        }
      }
      catch
      {
      }
    }
    #endregion protected contextmenu creation methods
    #endregion methods
  }
}
