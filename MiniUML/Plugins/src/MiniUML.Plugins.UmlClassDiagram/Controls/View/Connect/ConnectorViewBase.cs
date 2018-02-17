namespace MiniUML.Plugins.UmlClassDiagram.Controls.View.Connect
{
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Input;
  using Model.ViewModels.Shapes;
  using Shapes.Base;

  /// <summary>
  /// Helper class that contains comman methods that can be called by all shape view controls
  /// (is used by controls positioned on the canvas).
  /// </summary>
  public static class ConnectorViewBase
  {
    /// <summary>
    /// Create a new context menu with standard entries like
    /// Copy, Cut, Paste, Undo, Redo, Bring To Front etc..
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static ContextMenu CreateContextMenu(ShapeViewModelBase element)
    {
      ContextMenu retMenu = new ContextMenu();

      AddCopyCutPasteMenuItems(retMenu);

      retMenu.Items.Add(new Separator());
      AddUndoRedoMenuItems(retMenu);

      retMenu.Items.Add(new Separator());
      AddZOrderMenuItems(retMenu, element);

      return retMenu;
    }

    /// <summary>
    /// Add "Bring To Front" and "Send To Back" menu entries into the given menu.
    /// </summary>
    /// <param name="menu"></param>
    /// <param name="element"></param>
    public static void AddZOrderMenuItems(MenuBase menu, ShapeViewModelBase element)
    {
      try
      {
        if (element != null)
        {
          menu.Items.Add(new MenuItem()
          {
            Header = Framework.Local.Strings.STR_MENUITEM_BringToFront,
            Command = element.BringToFront,
            Icon = IconImageFactory.Get(IconCommand.SendToBack)
          });

          menu.Items.Add(new MenuItem()
          {
            Header = Framework.Local.Strings.STR_MENUITEM_SendToBack,
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
    /// Add standard application Cut, Copy, Paste, and Deleete menu entries.
    /// </summary>
    /// <param name="menu"></param>
    public static void AddCopyCutPasteMenuItems(MenuBase menu)
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
    public static void AddUndoRedoMenuItems(MenuBase menu)
    {
      menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Undo });
      menu.Items.Add(new MenuItem() { Command = ApplicationCommands.Redo });
    }
  }
}
