using System.Collections.Generic;
using MiniUML.Framework.helpers;

namespace MiniUML.Plugins.UmlClassDiagram.ToolBox.ViewModel
{
    /// <summary>
    ///     Interface definition that ensures that toolbox data from viewmodel to view is provided.
    /// </summary>
    public interface IShapBox
    {
        List<ToolBoxData> ToolBoxItems { get; }
    }
}
