namespace FolderBrowser.Interfaces
{
    /// <summary>
    /// Models an interfaces to an item that can
    /// tell is parent (whether it has a paren or not).
    /// </summary>
    public interface IParent
    {
        /// <summary>
        /// Gets the parent object where this object is the child in the treeview.
        /// </summary>
        ITreeItemViewModel Parent { get; }
    }
}
