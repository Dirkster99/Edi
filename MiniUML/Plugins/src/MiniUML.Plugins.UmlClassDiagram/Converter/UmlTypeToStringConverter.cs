using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Xml;
using MiniUML.Model.Model;
using MiniUML.Model.ViewModels;
using MiniUML.Model.ViewModels.Document;
using MiniUML.Model.ViewModels.Shapes;
using MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel;
using MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.UmlElements;

namespace MiniUML.Plugins.UmlClassDiagram.Converter
{
    /// <summary>
    ///     Standard converter with static instance to convert a
    ///     <seealso cref="UmlTypes" /> enumeration member into a string or
    ///     a string into a <seealso cref="UmlTypes" /> enumeration member.
    /// </summary>
    [ValueConversion(typeof(UmlTypes), typeof(string))]
    public class UmlTypeToStringConverter : UmlTypeToStringConverterBase
    {
        #region fields

        public const double DefaultX = 100, DefaultY = 100;

        private readonly Tuple<UmlTypes, string>[] mMap =
        {
            new Tuple<UmlTypes, string>(UmlTypes.Undefined, "Undefined"),
            new Tuple<UmlTypes, string>(UmlTypes.Primitive, "Primitive"),
            new Tuple<UmlTypes, string>(UmlTypes.DataType, "DataType"),
            new Tuple<UmlTypes, string>(UmlTypes.Signal, "Signal"),
            new Tuple<UmlTypes, string>(UmlTypes.Class, "Class"),
            new Tuple<UmlTypes, string>(UmlTypes.Interface, "Interface"),
            new Tuple<UmlTypes, string>(UmlTypes.Table, "Table"),
            new Tuple<UmlTypes, string>(UmlTypes.Enumeration, "Enumeration"),
            new Tuple<UmlTypes, string>(UmlTypes.Component, "Component"),
            new Tuple<UmlTypes, string>(UmlTypes.Node, "Node"),
            new Tuple<UmlTypes, string>(UmlTypes.Device, "Device"),
            new Tuple<UmlTypes, string>(UmlTypes.DeploymentSpec, "DeploymentSpec"),
            new Tuple<UmlTypes, string>(UmlTypes.Decision, "Decision"),

            new Tuple<UmlTypes, string>(UmlTypes.Note, "Note"),

            new Tuple<UmlTypes, string>(UmlTypes.Package, "Package"),

            new Tuple<UmlTypes, string>(UmlTypes.Boundary, "Boundary"),

            new Tuple<UmlTypes, string>(UmlTypes.UseCase, "UseCase"),
            new Tuple<UmlTypes, string>(UmlTypes.Collaboration, "Collaboration"),

            new Tuple<UmlTypes, string>(UmlTypes.CanvasShape, "CanvasShape"),

            // CanvasShape elements XXX TODO
            new Tuple<UmlTypes, string>(UmlTypes.Actor, "Actor"),
            new Tuple<UmlTypes, string>(UmlTypes.Actor1, "Actor1"),
            new Tuple<UmlTypes, string>(UmlTypes.ActivityInitial, "ActivityInitial"),
            new Tuple<UmlTypes, string>(UmlTypes.ActivityFinal, "ActivityFinal"),
            new Tuple<UmlTypes, string>(UmlTypes.ActivityFlowFinal, "ActivityFlowFinal"),
            new Tuple<UmlTypes, string>(UmlTypes.ActivitySync, "ActivitySync"),
            new Tuple<UmlTypes, string>(UmlTypes.Event1, "Event1"),
            new Tuple<UmlTypes, string>(UmlTypes.Event2, "Event2"),
            new Tuple<UmlTypes, string>(UmlTypes.Action1, "Action1"),
            new Tuple<UmlTypes, string>(UmlTypes.Action2, "Action2"),

            new Tuple<UmlTypes, string>(UmlTypes.ExecutionEnvironment, "ExecutionEnvironment"),

            // Connection shapes
            new Tuple<UmlTypes, string>(UmlTypes.ConnectorAggregation, "Aggregation"),
            new Tuple<UmlTypes, string>(UmlTypes.ConnectorAssociation, "Association"),
            new Tuple<UmlTypes, string>(UmlTypes.ConnectorComposition, "Composition"),
            new Tuple<UmlTypes, string>(UmlTypes.ConnectorInheritance, "Inheritance")
        };

        private readonly Dictionary<UmlTypes, string> mMapUmlTypeToString;
        private readonly Dictionary<string, UmlTypes> mMapStringToUmlType;

        #endregion fields

        #region constructor

        /// <summary>
        ///     Static class constructor
        /// </summary>
        static UmlTypeToStringConverter()
        {
            Instance = new UmlTypeToStringConverter();
        }

        /// <summary>
        ///     Standard Class constructor
        /// </summary>
        public UmlTypeToStringConverter()
        {
            // Construct lookup tables and load them for both ways
            mMapStringToUmlType = new Dictionary<string, UmlTypes>();
            mMapUmlTypeToString = new Dictionary<UmlTypes, string>();

            foreach (var item in mMap)
            {
                mMapStringToUmlType.Add(item.Item2, item.Item1);
                mMapUmlTypeToString.Add(item.Item1, item.Item2);
            }
        }

        #endregion constructor

        #region properties

        /// <summary>
        ///     Get a static instance of this converter
        /// </summary>
        public static UmlTypeToStringConverter Instance { get; }

        #endregion properties

        #region methods

        /// <summary>
        ///     Comverts an <seealso cref="UmlTypes" /> enumeration member into a string or
        ///     the equivalent string for <seealso cref="UmlTypes" />.Undefined if conversion failed.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (value is UmlTypes == false)
                return null;

            if (mMapUmlTypeToString.TryGetValue((UmlTypes) value, out var str) == false)
                mMapUmlTypeToString.TryGetValue(UmlTypes.Undefined, out str);

            return str;
        }

        /// <summary>
        ///     Converts a string into an <seealso cref="UmlTypes" /> enumeration member or
        ///     the Undefined memeber if the conversion failed.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (value is string == false)
                return null;

            if (mMapStringToUmlType.TryGetValue((string) value, out var umlTypes) == false)
                return UmlTypes.Undefined;

            return umlTypes;
        }

        public override PageViewModelBase ReadDocument(string xml,
            IShapeParent docDataModel,
            out List<ShapeViewModelBase> docRoot)
        {
            docRoot = new List<ShapeViewModelBase>();

            var root = new PageViewModel();

            using (var sw = new StringReader(xml))
            {
                using (var reader = XmlReader.Create(sw))
                {
                    return ReadXML(reader, docDataModel, root, out docRoot);
                }
            }
        }

        public override PageViewModelBase LoadDocument(string fileName,
            IShapeParent docDataModel,
            out List<ShapeViewModelBase> docRoot)
        {
            docRoot = new List<ShapeViewModelBase>();

            try
            {
                var root = new PageViewModel();

                try
                {
                    using (var reader = XmlReader.Create(fileName))
                    {
                        return ReadXML(reader, docDataModel, root, out docRoot);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private PageViewModel ReadXML(XmlReader reader,
            IShapeParent docDataModel,
            PageViewModel root,
            out List<ShapeViewModelBase> docRoot)
        {
            docRoot = new List<ShapeViewModelBase>();

            reader.ReadToNextSibling(PageViewModel.XmlElementName);
            while (reader.MoveToNextAttribute())
                if (root.ReadAttribute(reader.Name, reader.Value) == false)
                    return root;

            // Read child elements of this XML node
            while (reader.Read())
                if (reader.NodeType == XmlNodeType.Element)
                {
                    var nodeName = reader.Name;

                    var o = ConvertBack(nodeName, nodeName.GetType(), null, CultureInfo.InvariantCulture);

                    if (o is UmlTypes == false)
                        throw new ArgumentException("Node name: '" + nodeName + "' is not supported.");

                    var umlType = (UmlTypes) o;

                    if (umlType == UmlTypes.Undefined)
                        throw new ArgumentException("Undefined node name: '" + nodeName +
                                                    "' in conversion is not supported.");

                    ShapeViewModelBase s = null;

                    try
                    {
                        s = UmlElementDataDef.ReadShape(reader, umlType, docDataModel);

                        if (s != null)
                            docRoot.Add(s);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

            return root;
        }

        #endregion methods
    }
}
