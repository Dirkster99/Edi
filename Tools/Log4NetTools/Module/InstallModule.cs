namespace Log4NetTools.Module
{
    using Edi.Core.Interfaces;
    using Edi.Core.Interfaces.DocumentTypes;
    using Edi.Core.Resources;
    using Edi.Core.View.Pane;
    using Log4NetTools.ViewModels;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows;

    /// <summary>
    /// This class installs the moduls contained in this library
    /// using the constructor parameters supplied by the container.
    /// </summary>
    internal class InstallModule
    {
        #region fields
        private readonly IAvalonDockLayoutViewModel _avLayout;
        private readonly IToolWindowRegistry _toolRegistry;
        private readonly IDocumentTypeManager _documentTypeManager;
        #endregion fields

        #region ctors
        /// <summary>
        /// Public parameterized class constructor
        /// </summary>
        public InstallModule(IAvalonDockLayoutViewModel avLayout,
                             IToolWindowRegistry toolRegistry,
                             IDocumentTypeManager documentTypeManager)
        {
            _avLayout = avLayout;
            _toolRegistry = toolRegistry;
            _documentTypeManager = documentTypeManager;
        }

        /// <summary>
        /// HIDDEN standard class constructor
        /// </summary>
        protected InstallModule()
        {
        }
        #endregion ctors

        #region methods
        /// <summary>
        /// Initialize this module via standard PRISM MEF procedure
        /// </summary>
        internal void Initialize()
        {
            this.RegisterDataTemplates(_avLayout.ViewProperties.SelectPanesTemplate);
            this.RegisterStyles(_avLayout.ViewProperties.SelectPanesStyle);

            _toolRegistry.RegisterTool(new Log4NetToolViewModel());
            _toolRegistry.RegisterTool(new Log4NetMessageToolViewModel());

            var docType = _documentTypeManager.RegisterDocumentType
                (Log4NetViewModel.DocumentKey,
                 Log4NetViewModel.Description,
                 Log4NetViewModel.FileFilterName,
                 Log4NetViewModel.DefaultFilter,
                 Log4NetViewModel.LoadFile,
                 null,                          // Log4Net Grid Viewer is a readonly viewer
                 typeof(Log4NetViewModel), 40);

            if (docType != null)
            {
                var t = docType.CreateItem("log4net XML output", new List<string>() { "log4j", "log", "txt", "xml" }, 35);
                docType.RegisterFileTypeItem(t);
            }
        }

        /// <summary>
        /// Register viewmodel types with <seealso cref="DataTemplate"/> for a view
        /// and return all definitions with a <seealso cref="PanesTemplateSelector"/> instance.
        /// </summary>
        /// <param name="paneSel"></param>
        /// <returns></returns>
        private PanesTemplateSelector RegisterDataTemplates(PanesTemplateSelector paneSel)
        {
            // Register Log4Net DataTemplates
            var template = ResourceLocator.GetResource<DataTemplate>(
                                    Assembly.GetAssembly(typeof(Log4NetViewModel)).GetName().Name,
                                    "DataTemplates/Log4NetViewDataTemplate.xaml",
                                    "Log4NetDocViewDataTemplate") as DataTemplate;

            paneSel.RegisterDataTemplate(typeof(Log4NetViewModel), template);

            template = ResourceLocator.GetResource<DataTemplate>(
                                    Assembly.GetAssembly(typeof(Log4NetMessageToolViewModel)).GetName().Name,
                                    "DataTemplates/Log4NetViewDataTemplate.xaml",
                                    "Log4NetMessageViewDataTemplate") as DataTemplate;

            paneSel.RegisterDataTemplate(typeof(Log4NetMessageToolViewModel), template);

            template = ResourceLocator.GetResource<DataTemplate>(
                                    Assembly.GetAssembly(typeof(Log4NetToolViewModel)).GetName().Name,
                                    "DataTemplates/Log4NetViewDataTemplate.xaml",
                                    "Log4NetToolViewDataTemplate") as DataTemplate;

            paneSel.RegisterDataTemplate(typeof(Log4NetToolViewModel), template);

            return paneSel;
        }

        private PanesStyleSelector RegisterStyles(PanesStyleSelector selectPanesStyle)
        {
            var newStyle = ResourceLocator.GetResource<Style>(
                                    "Log4NetTools", "Styles/AvalonDockStyles.xaml", "Log4NetStyle") as Style;

            selectPanesStyle.RegisterStyle(typeof(Log4NetViewModel), newStyle);

            return selectPanesStyle;
        }
        #endregion methods
    }
}
