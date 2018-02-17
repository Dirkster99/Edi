namespace MiniUML.Model.Model
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Windows.Data;
  using ViewModels;
  using ViewModels.Document;
  using ViewModels.Shapes;

  public abstract class UmlTypeToStringConverterBase : IValueConverter
  {
    // Summary:
    //     Converts a value.
    //
    // Parameters:
    //   value:
    //     The value produced by the binding source.
    //
    //   targetType:
    //     The type of the binding target property.
    //
    //   parameter:
    //     The converter parameter to use.
    //
    //   culture:
    //     The culture to use in the converter.
    //
    // Returns:
    //     A converted value. If the method returns null, the valid null value is used.
    public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

    // Summary:
    //     Converts a value.
    //
    // Parameters:
    //   value:
    //     The value that is produced by the binding target.
    //
    //   targetType:
    //     The type to convert to.
    //
    //   parameter:
    //     The converter parameter to use.
    //
    //   culture:
    //     The culture to use in the converter.
    //
    // Returns:
    //     A converted value. If the method returns null, the valid null value is used.
    public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);

    /// <summary>
    /// Load a document from string persistence.
    /// </summary>
    /// <param name="xml"></param>
    /// <param name="docDataModel"></param>
    /// <param name="docRoot"></param>
    /// <returns></returns>
    public abstract PageViewModelBase ReadDocument(string xml,
                                                   IShapeParent docDataModel,
                                                   out List<ShapeViewModelBase> docRoot);

    /// <summary>
    /// Load a document from file persistence.
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="docDataModel"></param>
    /// <param name="docRoot"></param>
    /// <returns></returns>
    public abstract PageViewModelBase LoadDocument(string filename,
                                                   IShapeParent docDataModel,
                                                   out List<ShapeViewModelBase> docRoot);
  }
}
