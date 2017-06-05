namespace MiniUML.Plugins.UmlClassDiagram.Controls.View.Shapes.Base
{
  using System;
  using System.Windows.Controls;
  using System.Windows.Media.Imaging;

  internal enum IconCommand
  {
    SendToBack,
    BringToFront,

    Cut,
    Copy,
    Paste,
    Delete,

    AlignObjectsBottom,
    AlignObjectsCenteredHorizontal,
    AlignObjectsCenteredVertical,
    AlignObjectsLeft,
    AlignObjectsRight,
    AlignObjectsTop
  }

  static class IconImageFactory
  {
    /// <summary>
    /// Return a corresponding image for a given type of command.
    /// </summary>
    /// <param name="typeOfIcon"></param>
    /// <returns></returns>
    internal static Image Get(IconCommand typeOfIcon)
    {
      string source = string.Empty;

      switch (typeOfIcon)
      {
        case IconCommand.SendToBack:
          source = "Images/BrightBackground/BringForward.png";
          break;

        case IconCommand.BringToFront:
          source = "Images/BrightBackground/BringToFront.png";
          break;

        case IconCommand.Cut:
          source = "Images/BrightBackground/Cut.png";
          break;
        case IconCommand.Copy:
          source = "Images/BrightBackground/Copy.png";
          break;
        case IconCommand.Paste:
          source = "Images/BrightBackground/Paste.png";
          break;
        case IconCommand.Delete:
          source = "Images/BrightBackground/Delete.png";
          break;

        case IconCommand.AlignObjectsBottom:
          source = "Images/BrightBackground/AlignObjectsBottom.png";
          break;

        case IconCommand.AlignObjectsCenteredHorizontal:
          source = "Images/BrightBackground/AlignObjectsCenteredHorizontal.png";
          break;

        case IconCommand.AlignObjectsCenteredVertical:
          source = "Images/BrightBackground/AlignObjectsCenteredVertical.png";
          break;

        case IconCommand.AlignObjectsLeft:
          source = "Images/BrightBackground/AlignObjectsLeft.png";
          break;

        case IconCommand.AlignObjectsRight:
          source = "Images/BrightBackground/AlignObjectsRight.png";
          break;

        case IconCommand.AlignObjectsTop:
          source = "Images/BrightBackground/AlignObjectsTop.png";
          break;

        default:
          throw new NotImplementedException(typeOfIcon.ToString());
      }

      source = string.Format("pack://application:,,,/{0};component/{1}", "MiniUML.View", source);

      return new Image()
                 {
                   Source = BitmapFrame.Create(new Uri(source))
                 };
    }
  }
}
