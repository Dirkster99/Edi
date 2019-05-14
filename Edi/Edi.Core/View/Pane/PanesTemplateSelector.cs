namespace Edi.Core.View.Pane
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Data template selector that can be used to select a <seealso cref="DataTemplate"/>
    /// for view instance based on a viewmodel (class) type.
    /// </summary>
    public class PanesTemplateSelector : DataTemplateSelector
    {
        #region fields
        private readonly Dictionary<Type, DataTemplate> _templateDirectory;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public PanesTemplateSelector()
        {
            _templateDirectory = new Dictionary<Type, DataTemplate>();
        }
        #endregion constructor

        #region methods
        /// <summary>
        /// Returns a System.Windows.DataTemplate based on custom logic.
        /// </summary>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>Returns a System.Windows.DataTemplate or null. The default value is null.</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (_templateDirectory == null)
                return null;

            if (item == null)
                return null;

            DataTemplate o;
            _templateDirectory.TryGetValue(item.GetType(), out o);


            return o ?? base.SelectTemplate(item, container);
        }

        /// <summary>
        /// Register a (viewmodel) class type with a <seealso cref="DataTemplate"/> for a view.
        /// </summary>
        /// <param name="typeOfViewmodel"></param>
        /// <param name="view"></param>
        public void RegisterDataTemplate(Type typeOfViewmodel, DataTemplate view)
        {
            _templateDirectory.Add(typeOfViewmodel, view);
        }
        #endregion methods
    }
}