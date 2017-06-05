namespace MiniUML.Framework
{
  using System;
  using System.Globalization;
  using System.Windows;
  using System.Xml;
  using System.Xml.Linq;

  /// <summary>
  /// This assembly contains functionality common to all MiniUML assemblies and should hence not depend on any other MiniUML assemblies
  /// </summary>
  public static class FrameworkUtilities
  {
    public static double GetDoubleAttribute(this XElement elm, string attributeName, double fallback)
    {
      XAttribute attrib = elm.Attribute(attributeName);

      if (attrib == null)
        return fallback;

      double result;

      if (double.TryParse(attrib.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
        return result;

      return fallback;
    }

    public static FourDoubles GetFourDoubleAttributes(this XElement elm,
                                                      string attributeName,
                                                      FourDoubles fallback)
    {
      XAttribute attrib = elm.Attribute(attributeName);

      if (attrib == null)
        return fallback;

      try
      {
        string[] values = attrib.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        int resultCount = 0;
        double[] result = new double[4];
        double parseResult;

        for (int i = 0; i < values.Length; i++)
        {
          if (double.TryParse(values[i], NumberStyles.Any, CultureInfo.InvariantCulture, out parseResult))
          {
            result[i] = parseResult;
            resultCount++;
          }
        }

        if (resultCount == 4)
        {
          return new FourDoubles(result[0], result[1], result[2], result[3]);
        }
      }
      catch
      {
        return fallback;
      }

      return fallback;
    }

    public static double[] GetDoubleAttributes(string attributeValue,
                                               int numberOfDoubles,
                                               double[] fallback)
    {
      if (numberOfDoubles <= 0)
        throw new ArgumentOutOfRangeException("The number of doubles to read from attribute must be greater 0.");

      double[] ret = new double[numberOfDoubles];

      try
      {
        string[] values = attributeValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        int resultCount = 0;
        double parseResult;

        for (int i = 0; i < values.Length; i++)
        {
          if (double.TryParse(values[i], NumberStyles.Any, CultureInfo.InvariantCulture, out parseResult))
          {
            ret[i] = parseResult;
            resultCount++;
          }
        }

        // Return read values only if number of values read expected matches the results
        if (resultCount == numberOfDoubles)
          return ret;
      }
      catch
      {
        return fallback;
      }

      return fallback;
    }

    public static double GetAngularCoordinate(this Vector v)
    {
      if (v.X > 0)
        return Math.Atan(v.Y / v.X);
      else
        if (v.X < 0 && v.Y >= 0) return Math.Atan(v.Y / v.X) + Math.PI;
      else
        if (v.X < 0 && v.Y < 0) return Math.Atan(v.Y / v.X) - Math.PI;
      else
        if (v.X == 0 && v.Y > 0) return Math.PI / 2;
      else
        if (v.X == 0 && v.Y < 0) return -Math.PI / 2;
      else
        return 0; // x = y = 0
    }

    /// <summary>
    /// Computes the vector resolute or projection of v onto o.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="o"></param>
    /// <returns></returns>
    public static Vector VectorProjection(this Vector v, Vector o)
    {
      return (o * v) / o.LengthSquared * o;
    }

    /// <summary>
    /// Computes the scalar resolute or projection of v onto o.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="o"></param>
    /// <returns></returns>
    public static double ScalarProjection(this Vector v, Vector o)
    {
      return (o * v) / o.Length;
    }

    public static double NormalizeAngle(double a)
    {
      return a - ((Math.Floor(a / (2 * Math.PI))) * (2 * Math.PI));
    }

    public static double RadiansToDegrees(double rad)
    {
      return (rad * 180) / Math.PI;
    }

    public static double DegreesToRadians(double rad)
    {
      return (rad / 180) * Math.PI;
    }
  }

  /// <summary>
  /// Intermediate helper class to handle 4 double values
  /// </summary>
  public class FourDoubles
  {
    #region constructor
    /// <summary>
    /// Standard constructor
    /// </summary>
    public FourDoubles(double left, double top, double right, double bottom)
    {
      this.Left = left;
      this.Top = top;
      this.Right = right;
      this.Bottom = bottom;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    protected FourDoubles()
    {
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get left X-coordinate
    /// </summary>
    public double Left { get; protected set; }

    /// <summary>
    /// Get top Y-coordinate
    /// </summary>
    public double Top { get; protected set; }

    /// <summary>
    /// Get right X-coordinate
    /// </summary>
    public double Right { get; protected set; }

    /// <summary>
    /// Get bottom Y-coordinate
    /// </summary>
    public double Bottom { get; protected set; }
    #endregion properties
  }
}
