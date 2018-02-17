namespace MiniUML.Framework.helpers
{
  using Model.ViewModels.Command;

  /// <summary>
  /// Manage data necessary to display and create a new canvas item from a toolbox item.
  /// 
  /// Source: http://www.codeproject.com/Articles/484616/MVVM-Diagram-Designer?msg=4413242#Drag-And-Drop-To-The-Design-Surface
  /// </summary>
  public class ToolBoxData
  {
    public ToolBoxData(string imageUrl, CommandModelBase command)
    {
      ImageUrl = imageUrl;
      CreateShapeCommand = command;
    }

    public string ImageUrl { get; private set; }

    public CommandModelBase CreateShapeCommand { get; private set; }
  }
}
