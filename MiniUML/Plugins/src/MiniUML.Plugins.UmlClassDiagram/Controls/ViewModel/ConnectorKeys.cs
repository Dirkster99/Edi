namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel
{
  using System;

  /// <summary>
  /// Arrow head shapes of connectors (this is the typified end of an association
  /// or other line (view shape) that connect to objects on the canvas.
  /// </summary>
  public enum ConnectorKeys
  {
    /// <summary>
    /// Connector arrow head shape is not defined (may indicate an error)
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// This connector does not have any arrow head shape
    /// </summary>
    None = 1,

    /// <summary>
    /// This connector hase a triangle arrow head shape
    /// </summary>
    Triangle = 2,

    /// <summary>
    /// This connector hase a white diamond arrow head shape
    /// </summary>
    WhiteDiamond = 3,

    /// <summary>
    /// This connector hase a black diamond arrow head shape
    /// </summary>
    BlackDiamond = 4
  }

  /// <summary>
  /// Arrow shapes of connectors
  /// </summary>
  public static class ConnectorKeyStrings
  {
    /// <summary>
    /// Link to undefined arrow head in resource dictionary
    /// </summary>
    public const string UmlArrows_Undefined = "Uml.Arrows.Undefined";

    /// <summary>
    /// Link to triangle arrow head in resource dictionary
    /// </summary>
    public const string UmlArrows_Triangle = "Uml.Arrows.Triangle";

    /// <summary>
    /// Link to white diamond arrow head in resource dictionary
    /// </summary>
    public const string UmlArrows_WhiteDiamond = "Uml.Diamonds.White";

    /// <summary>
    /// Link to black diamond arrow head in resource dictionary
    /// </summary>
    public const string UmlArrows_BlackDiamond = "Uml.Diamonds.Black";

    /// <summary>
    /// Get a matching connector resource template string to a
    /// given <seealso cref="ConnectorKeys"/> enumeration parameter.
    /// </summary>
    /// <param name="implementingConnector"></param>
    /// <returns></returns>
    public static string GetConnectorPresentationKey(ConnectorKeys implementingConnector)
    {
      switch (implementingConnector)
      {
        case ConnectorKeys.Undefined:
          return UmlArrows_Undefined;

        case ConnectorKeys.None:
          return string.Empty;

        case ConnectorKeys.Triangle:
          return UmlArrows_Triangle;

        case ConnectorKeys.WhiteDiamond:
          return UmlArrows_WhiteDiamond;

        case ConnectorKeys.BlackDiamond:
          return UmlArrows_BlackDiamond;

        default:
          throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Convert a string resource connector key string into a <seealso cref="ConnectorKeys"/> enumeration memeber.
    /// </summary>
    /// <param name="stringConnector"></param>
    /// <returns></returns>
    public static ConnectorKeys GetConnectorEnumKey(string stringConnector)
    {
      if (stringConnector == null)
        throw new ArgumentNullException();

      switch (stringConnector)
      {
        case "":
          return ConnectorKeys.None;

        case UmlArrows_Undefined:
          return ConnectorKeys.Undefined;

        case UmlArrows_Triangle:
          return ConnectorKeys.Triangle;

        case UmlArrows_WhiteDiamond:
          return ConnectorKeys.WhiteDiamond;

        case UmlArrows_BlackDiamond:
          return ConnectorKeys.BlackDiamond;

        default:
          throw new NotImplementedException();
      }
    }
  }
}
