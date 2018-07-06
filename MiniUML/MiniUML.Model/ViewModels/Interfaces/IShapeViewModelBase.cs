namespace MiniUML.Model.ViewModels.Interfaces
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime;
    using System.Windows;
    using System.Xml;
    using System.Xml.Linq;
    using MiniUML.Framework;
    using MiniUML.Framework.Command;
    using MiniUML.Model.ViewModels.Document;
    using MiniUML.Model.ViewModels.Shapes;

    /// <summary>
    /// Base interface to manage data items for
    /// each shape that is visible on the canvas.
    /// </summary>
    public interface IShapeViewModelBase : INotifyPropertyChanged
    {
        #region properties
        /// <summary>
        /// Name of XML element that will repesent this object in data.
        /// </summary>
        string XElementName { get; }

        /// <summary>
        /// Get/set unique identifier for this shape.
        /// </summary>
        string ID { get; set; }

        string TypeKey { get; }

        /// <summary>
        /// Get/set label (short string that is usually
        /// displayed below shape) for this shape.
        /// </summary>
        string Name { get; set; }

        #region shape position
        /// <summary>
        /// Get/set X-positon of this shape.
        /// </summary>
        double Left { get; set; }

        /// <summary>
        /// Get/set Y-positon of this shape.
        /// </summary>
        double Top { get; set; }

        /// <summary>
        /// Get/set X,Y-positon of this shape.
        /// </summary>
        Point Position { get; set; }
        #endregion shape position

        /// <summary>
        /// Get/set whether this shape is currently selected or not.
        /// </summary>
        bool IsSelected { get; }
        #region Commands
        /// <summary>
        /// Get ICommand that implements Bring to Front functinoality for this shape.
        /// </summary>
        RelayCommand<object> BringToFront { get; }

        /// <summary>
        /// Get ICommand that implements Send to Back functinoality for this shape.
        /// </summary>
        RelayCommand<object> SendToBack { get; }
        #endregion Commands

        /// <summary>
        /// Get first child node (if any) or null.
        /// (Simple clr - Non-viewmodel property)
        /// </summary>
        /// <returns></returns>
        ShapeViewModelBase FirstNode { get; }

        /// <summary>
        /// Get last child node (if any) or null.
        /// (Simple clr - Non-viewmodel property)
        /// </summary>
        /// <returns></returns>
        ShapeViewModelBase LastNode { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Add child objects
        /// into the collection of child objects.
        /// </summary>
        /// <param name="shape"></param>
        void Add(ShapeViewModelBase shape);

        /// <summary>
        /// Add child objects from a given collection
        /// into the collection of child objects.
        /// </summary>
        /// <param name="shapes"></param>
        void Add(IEnumerable<ShapeViewModelBase> shapes);

        /// <summary>
        /// Remove this object instance from the parent (if any)
        /// </summary>
        void Remove();

        /// <summary>
        /// Persist the contents of this object into the given
        /// parameter <paramref name="writer"/> object.
        /// 
        /// Inheriting classes have to overwrite this method to provide
        /// their custom persistence method.
        /// </summary>
        /// <param name="writer"></param>
        void SaveDocument(XmlWriter writer, IEnumerable<ShapeViewModelBase> root);

        /// <summary>
        /// Gets a collection of shapes stored in this shapes object.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ShapeViewModelBase> Elements();

        /// <summary>
        /// Gets a count of the elements in the <see cref="Elements"/> collection.
        /// </summary>
        /// <returns></returns>
        int ElementsCount();
        #endregion methods
    }
}
