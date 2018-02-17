﻿using System.Collections.Generic;
using MiniUML.Model.ViewModels.Command;

namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.UmlElements
{
    /// <summary>
  /// Enumerate over all types of diagrams managed in this framework
  /// </summary>
  public enum UmlDiagrams
  {
    Undefined,
    Activity,
    Class,
    Common,
    Deployment,
    UseCase,
    Connector
  }

  /// <summary>
  /// Implement a service to generate command models that represent high level entry points for each UML diagram.
  /// </summary>
  public partial class UmlElementsManager
  {
    private class UmlDiagramsDataDef
    {
      #region fields
      private Dictionary<UmlDiagrams, UmlTypes[]> mDiagrams = new Dictionary<UmlDiagrams, UmlTypes[]>
      {
        {
          UmlDiagrams.Activity,
          new[] {
            UmlTypes.ActivityFinal,
            UmlTypes.ActivityFlowFinal,
            UmlTypes.ActivityInitial,
            UmlTypes.ActivitySync,
            UmlTypes.Event1,
            UmlTypes.Event2,
            UmlTypes.Action1,
            UmlTypes.Action2
          }
        },
        {
          UmlDiagrams.Class,
          new[] {
            UmlTypes.Primitive,
            UmlTypes.DataType,
            UmlTypes.Signal,
            UmlTypes.Class,
            UmlTypes.Table,
            UmlTypes.Enumeration,
            UmlTypes.Interface,
            UmlTypes.Decision
          }
        },
        {
          UmlDiagrams.Common,
          new[] {
            UmlTypes.Package,
            UmlTypes.Boundary,
            UmlTypes.Note
          }
        },
        {
          UmlDiagrams.Deployment,
          new[] {
            UmlTypes.Component,
            UmlTypes.Node,
            UmlTypes.Device,
            UmlTypes.ExecutionEnvironment,
            UmlTypes.Interface,
            UmlTypes.DeploymentSpec
          }
        },
        {
          UmlDiagrams.UseCase,
          new[] {
            UmlTypes.UseCase,
            UmlTypes.Collaboration,
            UmlTypes.Actor,
            UmlTypes.Actor1
          }
        },
        {
          UmlDiagrams.Connector,
          new[] {
            // Connection shapes
            UmlTypes.ConnectorAggregation,
            UmlTypes.ConnectorAssociation,
            UmlTypes.ConnectorComposition,
            UmlTypes.ConnectorInheritance
          }
        }
      };
      #endregion fields

      public IEnumerable<CommandModelBase> GetUmlDiagramDataDef(PluginViewModel viewModel, UmlDiagrams umlDiagram)
      {
        List<CommandModelBase> ret = new List<CommandModelBase>();

          mDiagrams.TryGetValue(umlDiagram, out var list);

        foreach (var item in list)
          ret.Add(Instance.GetCreateUmlShapeCommandModel(viewModel, item));

        return ret;
      }
    }
  }
}
