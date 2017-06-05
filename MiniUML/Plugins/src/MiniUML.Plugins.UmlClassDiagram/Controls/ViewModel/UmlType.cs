namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel
{
  /// <summary>
  /// This enumeration represents the type of each element that
  /// can be constructed and displayed with this Uml Element framework.
  /// </summary>
  public enum UmlTypes
  {
    Undefined,
    Primitive,
    DataType,
    Signal,
    Class,
    Interface,
    Table,
    Enumeration,
    Component,
    Node,
    Device,
    DeploymentSpec,
    Decision,

    Note,

    Package,

    Boundary,

    UseCase,
    Collaboration,

    CanvasShape,

    // CanvasShape elements XXX TODO
    Actor,
    Actor1,
    ActivityInitial,
    ActivityFinal,
    ActivityFlowFinal,
    ActivitySync,
    Event1,
    Event2,
    Action1,
    Action2,

    ExecutionEnvironment,

    // Connection shapes
    ConnectorAggregation,
    ConnectorAssociation,
    ConnectorComposition,
    ConnectorInheritance
  }
}
