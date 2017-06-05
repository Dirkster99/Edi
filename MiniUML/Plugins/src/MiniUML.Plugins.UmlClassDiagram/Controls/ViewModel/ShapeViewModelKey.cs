namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel
{
  using System;

  #region shape viewmodel view
  /// <summary>
  /// This key is used to create the right commandviewmodel
  /// inside a diagran viewmodel factory class.
  /// </summary>
  public enum ShapeViewModelKey
  {
    /// <summary>
    /// Shape is undefined (this can indicate an error)
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// Shape with rectangle border (eg uml class or table symbole)
    /// </summary>
    SquareShape = 1,

    /// <summary>
    /// Diamond symbol
    /// </summary>
    DecisionShape = 2,

    /// <summary>
    /// Uml package symbol
    /// </summary>
    PackageShape = 3,

    /// <summary>
    /// Uml boundary symbol
    /// </summary>
    BoundaryShape = 4,

    /// <summary>
    /// Uml comment symbol
    /// </summary>
    NoteShape = 5,

    /// <summary>
    /// Uml node symbol
    /// </summary>
    NodeShape = 6,

    /// <summary>
    /// Use case ellipse symbol
    /// </summary>
    UseCaseShape = 7,

    /// <summary>
    /// Use case symbol with sub-symbol types assigned at run-time
    /// </summary>
    CanvasShape = 8,

    /// <summary>
    /// Connector symbol (recycled for association, generalization etc.)
    /// </summary>
    AssocationShape = 9
  }

  /// <summary>
  /// CanvasShape elements
  /// </summary>
  public enum ShapeViewModelSubKeys
  {
    /// <summary>
    /// Value has not been set
    /// - its likely a bug if this value is present after class construction.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// This shape has no further sub-classes of other shape representation
    /// in view or data persistence.
    /// </summary>
    None = 1,

    /// <summary>
    /// UML man actor symbol
    /// </summary>
    CanvasUmlMan = 2,

    /// <summary>
    /// Initial activity symbol
    /// </summary>
    CanvasActivityInitial = 3,

    /// <summary>
    /// Final activity symbol
    /// </summary>
    CanvasActivityFinal = 4,

    /// <summary>
    /// Final flow activity symbol
    /// </summary>
    CanvasActivityFlowFinal = 5,

    /// <summary>
    /// Sync activity symbol
    /// </summary>
    CanvasActivitySync = 6,

    /// <summary>
    /// Activity event symbol
    /// </summary>
    CanvasEvent1 = 7,

    /// <summary>
    /// Activity event symbol
    /// </summary>
    CanvasEvent2 = 8
  }

  /// <summary>
  /// Presentation keys used for template selector to associate the right
  /// view to the right viewmodel.
  /// </summary>
  public static class ShapeViewModelKeyStrings
  {
    public const string ShapeUndefined = "ShapeUndefined";
    public const string ShapeSquare = "ShapeSquare";
    public const string ShapeDecision = "ShapeDecision";
    public const string ShapePackage = "ShapePackage";
    public const string ShapeBoundary = "ShapeBoundary";
    public const string ShapeNote = "ShapeNote";
    public const string ShapeNode = "ShapeNode";
    public const string ShapeUseCase = "ShapeUseCase";
    public const string ShapeCanvas = "ShapeCanvas";

    public const string ShapeAssociation = "Association";

    /// <summary>
    /// Get string identifier that represents the ImplementingViewModel
    /// as <seealso cref="ShapeViewModelKey"/> does. This identifier can
    /// be used by the TemplateSelector to find the implementing view
    /// (this extra property avoids an extra string cast).
    /// </summary>
    /// <param name="implementingViewModel"></param>
    /// <returns></returns>
    public static string GetPresentationStringKey(ShapeViewModelKey implementingViewModel)
    {
      switch (implementingViewModel)
      {
        case ShapeViewModelKey.Undefined:
          return ShapeViewModelKeyStrings.ShapeUndefined;

        case ShapeViewModelKey.SquareShape:
          return ShapeViewModelKeyStrings.ShapeSquare;

        case ShapeViewModelKey.DecisionShape:
          return ShapeViewModelKeyStrings.ShapeDecision;

        case ShapeViewModelKey.PackageShape:
          return ShapeViewModelKeyStrings.ShapePackage;

        case ShapeViewModelKey.BoundaryShape:
          return ShapeViewModelKeyStrings.ShapeBoundary;

        case ShapeViewModelKey.NoteShape:
          return ShapeViewModelKeyStrings.ShapeNote;

        case ShapeViewModelKey.NodeShape:
          return ShapeViewModelKeyStrings.ShapeNode;

        case ShapeViewModelKey.UseCaseShape:
          return ShapeViewModelKeyStrings.ShapeUseCase;

        case ShapeViewModelKey.CanvasShape:
          return ShapeViewModelKeyStrings.ShapeCanvas;

        case ShapeViewModelKey.AssocationShape:
          return ShapeViewModelKeyStrings.ShapeAssociation;

        default:
          throw new NotImplementedException();
      }
    }
  }
  #endregion shape viewmodel view
}
