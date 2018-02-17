namespace MiniUML.Model.ViewModels.Shapes
{
    public abstract class AnchorBaseViewModel : ShapeViewModelBase
  {
    #region constructor
    /// <summary>
    /// Standard contructor hidding XElement constructor
    /// </summary>
    /// <param name="parent"></param>
    public AnchorBaseViewModel(IShapeParent parent)
      : base(parent)
    {
    }
    #endregion constructor
  }
}
