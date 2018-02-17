using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml;
using MiniUML.Framework.Local;
using MiniUML.Model.ViewModels;
using MiniUML.Model.ViewModels.Command;
using MiniUML.Model.ViewModels.Shapes;
using MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Connect;
using MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.ConnectCreate;
using MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Shape;
using MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.ShapeCreate;

namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.UmlElements
{
    internal class UmlElementDataDef
  {
    #region fields
    // Character code for '«' much smaller symbol typically used as lead-in on display of stereotype name
    public const int StereotypeLeadIn = 171;

    // Character code for '»' much greater symbol typically used as lead-out on display of stereotype name
    public const int StereotypeLeadOut = 187;

    private static readonly Dictionary<UmlTypes, DataDef> mUmlElements = new Dictionary<UmlTypes, DataDef>
    {
      // Common diagram shapes
      {
        UmlTypes.Package,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Common/Package.png",
        ShapeViewModelKey.PackageShape,
        "Package",
        null,
        "Package",
        "Creates a package shape",
        null,
        false,
        125, 100)
      },
      {
        UmlTypes.Boundary,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Common/Boundary.png",
        ShapeViewModelKey.BoundaryShape,
        "Boundary",
        null,
        "Boundary",
        "Creates a boundary shape",
        null,
        false,
        700, 500)
      },
      {
        UmlTypes.Note,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Common/Note.png",
        ShapeViewModelKey.NoteShape,
        "Note",
        null,
        "Note",
        "Creates a note shape")
      },
      {
        UmlTypes.Primitive,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/Primitive.png",
        ShapeViewModelKey.SquareShape,
        "PrimitiveType1",
        $"{(char) StereotypeLeadIn}primitive{(char) StereotypeLeadOut}",
        "Primitive",
        "Creates a primitive shape",
        null,
        false,
        125.0, 75.0)
      },
      {
        UmlTypes.DataType,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/DataType.png",
        ShapeViewModelKey.SquareShape,
        "DataType1",
        $"{(char) StereotypeLeadIn}datatype{(char) StereotypeLeadOut}",
        "Data Type",
        "Creates a data type shape",
        null)
      },
      {
        UmlTypes.Signal,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/Signal.png",
        ShapeViewModelKey.SquareShape,
        "Signal1",
        $"{(char) StereotypeLeadIn}signal{(char) StereotypeLeadOut}",
        "Signal",
        "Creates a signal shape",
        null)
      },
      {
        UmlTypes.Class,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/Class.png",
        ShapeViewModelKey.SquareShape,
        "Class1",
        null,
        "Class",
        "Creates a class shape",
        null, true)
      },
      {
        UmlTypes.Table,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/TableToolBox.png",
        ShapeViewModelKey.SquareShape,
        "Table1",
        null,
        "Table",
        "Creates a table shape",
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/Table.png", true)
      },
      {
        UmlTypes.Enumeration,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/Enumeration.png",
        ShapeViewModelKey.SquareShape,
        "Enumeration1",
        $"{(char) StereotypeLeadIn}enumeration{(char) StereotypeLeadOut}",
        "Enumeration",
        "Creates an enumeration shape",
        null, true,
        125, 75)
      },
      {
        UmlTypes.Interface,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/Interface.png",
        ShapeViewModelKey.SquareShape,
        "Interface1",
        $"{(char) StereotypeLeadIn}interface{(char) StereotypeLeadOut}",
        "Interface",
        "Creates an interface shape",
        null, true)
      },
      {
        UmlTypes.Decision,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/Association.png",
        ShapeViewModelKey.DecisionShape,
        null,
        null,
        "Association",
        "Creates an association shape")
      },

      // Activitiy diagram shapes
      {
        UmlTypes.ActivityFinal,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Activity/ActivityFinalToolbox.png",
        ShapeViewModelKey.CanvasShape,
        "Activity Final",
        null,              // stereotype
        "Activity Final",
        "Creates an final activity shape",
        null, false,
        80, 105)
        { ShapeViewModelSubKey = ShapeViewModelSubKeys.CanvasActivityFinal }
      },
      {
        UmlTypes.ActivityFlowFinal,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Activity/ActivityFlowFinalToolbox.png",
        ShapeViewModelKey.CanvasShape,
        "Activity Flow Final",
        null,
        "Activity Flow Final",
        "Creates an final activity flow shape",
        null, false,
        90, 105)
        { ShapeViewModelSubKey = ShapeViewModelSubKeys.CanvasActivityFlowFinal }
      },
      {
        UmlTypes.ActivityInitial,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Activity/ActivityInitialToolbox.png",
        ShapeViewModelKey.CanvasShape,
        "Activity Initial",
        null,
        "Activity Initial",
        "Creates an initial activity shape",
        null, false,
        80, 105)
        { ShapeViewModelSubKey = ShapeViewModelSubKeys.CanvasActivityInitial }
      },
      {
        UmlTypes.ActivitySync,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Activity/ActivitySyncToolbox.png",
        ShapeViewModelKey.CanvasShape,
        "Activity Sync",
        null,
        "Activity Sync",
        "Creates an activity sync (man symbol)",
        null, false,
        80, 105)
        {
          ShapeViewModelSubKey = ShapeViewModelSubKeys.CanvasActivitySync
        }
      },
      {
        UmlTypes.Event1,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Activity/Event1Toolbox.png",
        ShapeViewModelKey.CanvasShape,
        "Event",
        null,
        "Event",
        "Creates an event shape",
        null, false,
        105, 50)
        {
          ShapeViewModelSubKey = ShapeViewModelSubKeys.CanvasEvent1
        }
      },
      {
        UmlTypes.Event2,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Activity/EventToolbox.png",
        ShapeViewModelKey.CanvasShape,
        "Event",
        null,
        "Event",
        "Creates an event shape",
        null, false,
        105, 50)
        {
          ShapeViewModelSubKey = ShapeViewModelSubKeys.CanvasEvent2
        }
      },
      {
        UmlTypes.Action1,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Activity/Event1Toolbox.png",
        ShapeViewModelKey.CanvasShape,
        "Action",
        null,
        "Action",
        "Creates an action shape",
        null, false,
        105, 50)
        {
          ShapeViewModelSubKey = ShapeViewModelSubKeys.CanvasEvent1
        }
      },
      {
        UmlTypes.Action2,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Activity/EventToolbox.png",
        ShapeViewModelKey.CanvasShape,
        "Action",
        null,
        "Action",
        "Creates an action shape",
        null, false,
        105, 50)
        {
          ShapeViewModelSubKey = ShapeViewModelSubKeys.CanvasEvent2
        }
      },
      {
        UmlTypes.Component,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Deployment/ComponentToolBox.png",
        ShapeViewModelKey.SquareShape,
        "Component",
        null,
        "Component",
        "Creates a component shape",
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Deployment/Component.png",
        false,
        125, 75)
      },
      {
        UmlTypes.Node,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Deployment/NodeToolBox.png",
        ShapeViewModelKey.NodeShape,
        "Node",
        null,
        "Node",
        "Creates a node shape",
        null,
        false,
        150, 75)
      },
      {
        UmlTypes.Device,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Deployment/DeviceToolBox.png",
        ShapeViewModelKey.NodeShape,
        "Device",
        $"{(char) StereotypeLeadIn}device{(char) StereotypeLeadOut}",
        "Device",
        "Creates a device shape",
        null,
        false,
        150, 75)
      },
      {
        UmlTypes.ExecutionEnvironment,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Deployment/ExecutionEnvironmentToolBox.png",
        ShapeViewModelKey.NodeShape,
        "ExecutionEnvironment",
        $"{(char) StereotypeLeadIn}execution environment{(char) StereotypeLeadOut}",
        "ExecutionEnvironment",
        "Creates a execution environment shape",
        null,
        false,
        200, 75)
      },
      {
        UmlTypes.DeploymentSpec,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Deployment/DeploymentSpecificationToolBox.png",
        ShapeViewModelKey.SquareShape,
        "Deployment Specification",
        $"{(char) StereotypeLeadIn}deployment spec{(char) StereotypeLeadOut}",
        "Deployment Specification",
        "Creates a deployment specification shape",
        null, false,
        175, 125)
      },
      {
        UmlTypes.UseCase,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/UseCase/UseCaseToolBox.png",
        ShapeViewModelKey.UseCaseShape,
        "Use Case 1",
        null,
        "Use Case",
        "Creates a use case shape",
        null, false,
        125, 75)
      },
      {
        UmlTypes.Collaboration,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/UseCase/CollaborationToolBox.png",
        ShapeViewModelKey.UseCaseShape,
        "Collaboration 1",
        null,
        "Collaboration",
        "Creates a collaboration shape",
        null, false,
        125, 75)
        { StrokeDashArray = "5,3" }
      },
      {
        UmlTypes.Actor,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/UseCase/ActorSquareToolbox.png",
        ShapeViewModelKey.SquareShape,
        "Actor1",
        $"{(char) StereotypeLeadIn}actor{(char) StereotypeLeadOut}",
        "Actor",
        "Creates an actor shape (square with stereotype)")
      },
      {
        UmlTypes.Actor1,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/UseCase/ActorToolbox.png",
        ShapeViewModelKey.CanvasShape,
        "Actor1",
        null,
        "Actor",
        "Creates an actor shape (man symbol)",
        null, false,
        60, 105)
        { ShapeViewModelSubKey = ShapeViewModelSubKeys.CanvasUmlMan }
      },
      {
        UmlTypes.ConnectorAggregation,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Connect/Command.CreateAggregationShape.png",
        ShapeViewModelKey.AssocationShape,
        null,
        null,
        Strings.STR_CMD_Association,
        Strings.STR_CMD_Association_description)
      },
      {
        UmlTypes.ConnectorAssociation,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Connect/Command.CreateAssociationShape.png",
        ShapeViewModelKey.AssocationShape,
        null,
        null,
        Strings.STR_CMD_Aggregation,
        Strings.STR_CMD_Aggregation_description)
      },
      {
        UmlTypes.ConnectorComposition,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Connect/Command.CreateCompositionShape.png",
        ShapeViewModelKey.AssocationShape,
        null,
        null,
        Strings.STR_CMD_Composition,
        Strings.STR_CMD_Composition_description)
      },
      {
        UmlTypes.ConnectorInheritance,

        new DataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Connect/Command.CreateInheritanceShape.png",
        ShapeViewModelKey.AssocationShape,
        null,
        null,
        Strings.STR_CMD_InheritanceAssociation,
        Strings.STR_CMD_InheritanceAssociation_description)
      }
    };
    #endregion fields

    #region Methods
    /// <summary>
    /// Create a shape viewmodel and return it.
    /// </summary>
    /// <param name="umlType"></param>
    /// <param name="dropPoint"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static ShapeViewModelBase CreateShape(UmlTypes umlType, Point dropPoint, IShapeParent parent)
    {
        if (mUmlElements.TryGetValue(umlType, out var item))
      {
        switch (item.ImplementingViewModel)
        {
          case ShapeViewModelKey.SquareShape:
            return CreateSquareShapeViewModel(dropPoint, parent, item, umlType);

          case ShapeViewModelKey.DecisionShape:
            return CreateDecisionShapeViewModel(dropPoint, parent, item, umlType);

          case ShapeViewModelKey.PackageShape:
            return CreatePackageShapeViewModel(dropPoint, parent, item, umlType);

          case ShapeViewModelKey.BoundaryShape:
            return CreateBoundaryShapeViewModel(dropPoint, parent, item, umlType);

          case ShapeViewModelKey.NoteShape:
            return CreateNoteShapeViewModel(dropPoint, parent, item, umlType);

          case ShapeViewModelKey.NodeShape:
            return CreateNodeShapeViewModel(dropPoint, parent, item, umlType);

          case ShapeViewModelKey.UseCaseShape:
            return CreateUseCaseShapeViewModel(dropPoint, parent, item, umlType);

          case ShapeViewModelKey.CanvasShape:
            return CreateCanvasShapeViewModel(dropPoint, parent, item, umlType);

          case ShapeViewModelKey.AssocationShape:
            return CreateAssocationShapeViewModel(dropPoint, parent, item, umlType);

            default:
            throw new NotImplementedException($"System error: '{umlType}' not supported in CreateShape.");
        }
      }

      throw new NotImplementedException(umlType.ToString());
    }

    /// <summary>
    /// Read a shapes configiration information from persistence and return the new shape view model.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="umlType"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static ShapeViewModelBase ReadShape(XmlReader reader, UmlTypes umlType, IShapeParent parent)
    {
      switch (umlType)
      {
        case UmlTypes.Primitive:
        case UmlTypes.DataType:
        case UmlTypes.Signal:
        case UmlTypes.Class:
        case UmlTypes.Interface:
        case UmlTypes.Table:
        case UmlTypes.Enumeration:
        case UmlTypes.Component:
          return UmlSquareShapeViewModel.ReadDocument(reader.ReadSubtree(), parent, umlType);

        case UmlTypes.Decision:
          return UmlDecisionShapeViewModel.ReadDocument(reader.ReadSubtree(), parent, umlType);

        case UmlTypes.Package:
          return UmlPackageShapeViewModel.ReadDocument(reader.ReadSubtree(), parent, umlType);

        case UmlTypes.Boundary:
          return UmlBoundaryShapeViewModel.ReadDocument(reader.ReadSubtree(), parent, umlType);

        case UmlTypes.Note:
          return UmlNoteShapeViewModel.ReadDocument(reader.ReadSubtree(), parent, umlType);

        case UmlTypes.Node:
        case UmlTypes.Device:
        case UmlTypes.DeploymentSpec:
          return UmlNodeShapeViewModel.ReadDocument(reader.ReadSubtree(), parent, umlType);

        case UmlTypes.Collaboration:
        case UmlTypes.UseCase:
          return UmlUseCaseShapeViewModel.ReadDocument(reader.ReadSubtree(), parent, umlType);

        case UmlTypes.CanvasShape:
        case UmlTypes.Actor:
        case UmlTypes.Actor1:
        case UmlTypes.ActivityInitial:
        case UmlTypes.ActivityFinal:
        case UmlTypes.ActivityFlowFinal:
        case UmlTypes.ActivitySync:
        case UmlTypes.Event1:
        case UmlTypes.Event2:
        case UmlTypes.Action1:
        case UmlTypes.Action2:
        case UmlTypes.ExecutionEnvironment:
          return UmlCanvasShapeViewModel.ReadDocument(reader.ReadSubtree(), parent, umlType);

        case UmlTypes.ConnectorAggregation:
        case UmlTypes.ConnectorAssociation:
        case UmlTypes.ConnectorComposition:
        case UmlTypes.ConnectorInheritance:
          return UmlAssociationShapeViewModel.ReadDocument(reader.ReadSubtree(), parent, umlType);

        case UmlTypes.Undefined:
        default:
          throw new NotImplementedException($"System error: '{umlType}' not supported in ReadShape.");
      }
    }

    /// <summary>
    /// Create a command viewmodel that has the command functionality
    /// to create a corresponding shape viewmodel.
    /// </summary>
    /// <param name="viewModel"></param>
    /// <param name="umlType"></param>
    /// <returns></returns>
    public CommandModelBase GetShapeCreateCommand(PluginViewModel viewModel, UmlTypes umlType)
    {
        if (mUmlElements.TryGetValue(umlType, out var item))
      {
        switch (item.ImplementingViewModel)
        {
          case ShapeViewModelKey.SquareShape:
          case ShapeViewModelKey.DecisionShape:
          case ShapeViewModelKey.PackageShape:
          case ShapeViewModelKey.BoundaryShape:
          case ShapeViewModelKey.NoteShape:
          case ShapeViewModelKey.NodeShape:
          case ShapeViewModelKey.UseCaseShape:
          case ShapeViewModelKey.CanvasShape:
            return new CreateShapeCommandModel(viewModel, umlType,
                                               item.ToolBoxDescription, item.ToolboxName, item.ToolboxImageUrl);

          // Create connector command viewmodels
          case ShapeViewModelKey.AssocationShape:
            return new CreateAssociationCommandModel(viewModel,
                                                     item.ToolboxImageUrl,
                                                     umlType,
                                                     item.ToolboxName,
                                                     item.ToolBoxDescription);

          case ShapeViewModelKey.Undefined:
          default:
            throw new NotImplementedException(umlType.ToString());
        }
      }

      throw new NotImplementedException($"System error: '{umlType}' not supported in CreateCommand.");
    }

    #region create shapeviewmodel

      /// <summary>
      /// Create a shape viewwmodel that represents a canvas element.
      /// </summary>
      /// <param name="dropPoint"></param>
      /// <param name="parent"></param>
      /// <param name="item"></param>
      /// <param name="umlType"></param>
      /// <returns></returns>
      private static ShapeViewModelBase CreateSquareShapeViewModel(Point dropPoint,
                                                                IShapeParent parent,
                                                                DataDef item,
                                                                UmlTypes umlType)
    {
        var element = new UmlSquareShapeViewModel(parent, umlType)
        {
            Stereotype = item.ShapeStereotype ?? string.Empty,
            Name = item.ShapeName,
            ShapeImageUrl = item.ShapeImageUrl ?? string.Empty,
            HorizontalLine = item.ShapeHorizontalLine,
            Top = dropPoint.Y,
            Left = dropPoint.X,
            Width = item.DefaultWidth,
            Height = item.DefaultHeight
        };


        return element;
    }

      /// <summary>
      /// Create a shape viewwmodel that represents a canvas element.
      /// </summary>
      /// <param name="dropPoint"></param>
      /// <param name="parent"></param>
      /// <param name="item"></param>
      /// <param name="umlType"></param>
      /// <returns></returns>
      private static ShapeViewModelBase CreateDecisionShapeViewModel(Point dropPoint,
                                                                   IShapeParent parent,
                                                                   DataDef item,
                                                                   UmlTypes umlType)
    {
        var element = new UmlDecisionShapeViewModel(parent, umlType)
        {
            Name = item.ShapeName,
            Top = dropPoint.Y,
            Left = dropPoint.X,
            Width = item.DefaultWidth,
            Height = item.DefaultHeight
        };

        return element;
    }

      /// <summary>
      /// Create a shape viewwmodel that represents a canvas element.
      /// </summary>
      /// <param name="dropPoint"></param>
      /// <param name="parent"></param>
      /// <param name="item"></param>
      /// <param name="umlType"></param>
      /// <returns></returns>
      private static ShapeViewModelBase CreatePackageShapeViewModel(Point dropPoint,
                                                                  IShapeParent parent,
                                                                  DataDef item,
                                                                  UmlTypes umlType)
    {
        var element = new UmlPackageShapeViewModel(parent, umlType)
        {
            Name = item.ShapeName,
            Top = dropPoint.Y,
            Left = dropPoint.X,
            Width = item.DefaultWidth,
            Height = item.DefaultHeight
        };


        return element;
    }

      /// <summary>
      /// Create a shape viewwmodel that represents a canvas element.
      /// </summary>
      /// <param name="dropPoint"></param>
      /// <param name="parent"></param>
      /// <param name="item"></param>
      /// <param name="umlType"></param>
      /// <returns></returns>
      private static ShapeViewModelBase CreateBoundaryShapeViewModel(Point dropPoint,
                                                                   IShapeParent parent,
                                                                   DataDef item,
                                                                   UmlTypes umlType)
    {
        var element = new UmlBoundaryShapeViewModel(parent, umlType)
        {
            Name = item.ShapeName,
            Top = dropPoint.Y,
            Left = dropPoint.X,
            Width = item.DefaultWidth,
            Height = item.DefaultHeight
        };


        return element;
    }

      /// <summary>
      /// Create a shape viewwmodel that represents a canvas element.
      /// </summary>
      /// <param name="dropPoint"></param>
      /// <param name="parent"></param>
      /// <param name="item"></param>
      /// <param name="umlType"></param>
      /// <returns></returns>
      private static ShapeViewModelBase CreateNoteShapeViewModel(Point dropPoint,
                                                               IShapeParent parent,
                                                               DataDef item,
                                                               UmlTypes umlType)
    {
        var element = new UmlNoteShapeViewModel(parent, umlType)
        {
            Text = item.ToolboxName,
            Top = dropPoint.Y,
            Left = dropPoint.X,
            Width = item.DefaultWidth,
            Height = item.DefaultHeight
        };


        return element;
    }

      /// <summary>
      /// Create a shape viewwmodel that represents a canvas element.
      /// </summary>
      /// <param name="dropPoint"></param>
      /// <param name="parent"></param>
      /// <param name="item"></param>
      /// <param name="umlType"></param>
      /// <returns></returns>
      private static ShapeViewModelBase CreateNodeShapeViewModel(Point dropPoint,
                                                               IShapeParent parent,
                                                               DataDef item,
                                                               UmlTypes umlType)
    {
        var element = new UmlNodeShapeViewModel(parent, umlType)
        {
            Stereotype = item.ShapeStereotype ?? string.Empty,
            Name = item.ShapeName,
            ShapeImageUrl = item.ShapeImageUrl ?? string.Empty,
            Top = dropPoint.Y,
            Left = dropPoint.X,
            Width = item.DefaultWidth,
            Height = item.DefaultHeight
        };


        return element;
    }

      /// <summary>
      /// Create a shape viewwmodel that represents a canvas element.
      /// </summary>
      /// <param name="dropPoint"></param>
      /// <param name="parent"></param>
      /// <param name="item"></param>
      /// <param name="umlType"></param>
      /// <returns></returns>
      private static ShapeViewModelBase CreateUseCaseShapeViewModel(Point dropPoint,
                                                                  IShapeParent parent,
                                                                  DataDef item,
                                                                  UmlTypes umlType)
    {
        var element = new UmlUseCaseShapeViewModel(parent, umlType)
        {
            Name = item.ShapeName,
            Top = dropPoint.Y,
            Left = dropPoint.X,
            Width = item.DefaultWidth,
            Height = item.DefaultHeight,
            StrokeDashArray = item.StrokeDashArray ?? string.Empty
        };


        return element;
    }

      /// <summary>
      /// Create a shape viewwmodel that represents a canvas element.
      /// </summary>
      /// <param name="dropPoint"></param>
      /// <param name="parent"></param>
      /// <param name="item"></param>
      /// <param name="umlType"></param>
      /// <returns></returns>
      private static ShapeViewModelBase CreateAssocationShapeViewModel(Point dropPoint,
                                                                      IShapeParent parent,
                                                                      DataDef item,
                                                                      UmlTypes umlType)
    {
      var element = new UmlUseCaseShapeViewModel(parent, umlType);

      switch (umlType)
      {
        case UmlTypes.ConnectorAggregation:
          return new UmlAssociationShapeViewModel(parent, ConnectorKeys.WhiteDiamond, ConnectorKeys.None, UmlTypes.ConnectorAggregation);

        case UmlTypes.ConnectorAssociation:
          return new UmlAssociationShapeViewModel(parent, ConnectorKeys.None, ConnectorKeys.None, UmlTypes.ConnectorAssociation);

        case UmlTypes.ConnectorComposition:
          return new UmlAssociationShapeViewModel(parent, ConnectorKeys.BlackDiamond, ConnectorKeys.None, UmlTypes.ConnectorComposition);

        case UmlTypes.ConnectorInheritance:
          return new UmlAssociationShapeViewModel(parent, ConnectorKeys.None, ConnectorKeys.Triangle, UmlTypes.ConnectorInheritance);

        default:
          throw new NotImplementedException(umlType.ToString());
      }
    }

      /// <summary>
      /// Create a shape viewwmodel that represents a canvas element.
      /// </summary>
      /// <param name="dropPoint"></param>
      /// <param name="parent"></param>
      /// <param name="item"></param>
      /// <param name="umlType"></param>
      /// <returns></returns>
      private static ShapeViewModelBase CreateCanvasShapeViewModel(Point dropPoint,
                                                                 IShapeParent parent,
                                                                 DataDef item,
                                                                 UmlTypes umlType)
    {
        var element = new UmlCanvasShapeViewModel(parent, item.ShapeViewModelSubKey, umlType)
        {
            Name = item.ShapeName,
            Top = dropPoint.Y,
            Left = dropPoint.X,
            Width = item.DefaultWidth,
            Height = item.DefaultHeight
        };


        return element;
    }
    #endregion create shapeviewmodel
    #endregion Methods

    #region private class
    private class DataDef
    {
      public DataDef(string toolboxImageUrl,
                     ShapeViewModelKey implementingViewModel,
                     string shapeName,
                     string shapeStereotype,
                     string toolboxName,
                     string description,
                     string shapeImageUrl = "",
                     bool shapeHorizontalLine = false,
                     double defaultWidth = 95,
                     double defaultHeight = 75)
      {
        ToolboxImageUrl = toolboxImageUrl;
        ImplementingViewModel = implementingViewModel;
        ShapeName = shapeName;
        ShapeStereotype = shapeStereotype;
        ToolboxName = toolboxName;
        ToolBoxDescription = description;
        ShapeImageUrl = shapeImageUrl;
        ShapeHorizontalLine = shapeHorizontalLine;

        DefaultHeight = defaultHeight;
        DefaultWidth = defaultWidth;
      }

        public string ToolboxImageUrl { get; }

        public ShapeViewModelKey ImplementingViewModel { get; }

        public ShapeViewModelSubKeys ShapeViewModelSubKey { get; set; }

        public string ShapeName { get; }

        public string ShapeStereotype { get; }

        public string ToolboxName { get; }

        public string ToolBoxDescription { get; }

        public string ShapeImageUrl { get; }

        public bool ShapeHorizontalLine { get; }

        public double DefaultWidth { get; }

        public double DefaultHeight { get; }

        public string StrokeDashArray { get; set; }
    }
    #endregion private class
  }
}
