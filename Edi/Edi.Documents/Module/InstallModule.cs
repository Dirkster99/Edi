namespace Edi.Documents.Module
{
    using Edi.Core.Interfaces;
    using Edi.Core.Interfaces.DocumentTypes;
    using Edi.Core.Resources;
    using Edi.Core.View.Pane;
    using Edi.Documents.ViewModels.EdiDoc;
    using Edi.Documents.ViewModels.MiniUml;
    using Edi.Documents.ViewModels.StartPage;
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
        private readonly IDocumentTypeManager _documentTypeManager;
        #endregion fields

        #region ctors
        /// <summary>
        /// Parameterized PUBLIC class constructor
        /// </summary>
        /// <param name="avLayout"></param>
        /// <param name="documentTypeManager"></param>
        public InstallModule(IAvalonDockLayoutViewModel avLayout,
                             IDocumentTypeManager documentTypeManager)
            : this()
        {
            _avLayout = avLayout;
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
            RegisterDataTemplates(_avLayout.ViewProperties.SelectPanesTemplate);
            RegisterStyles(_avLayout.ViewProperties.SelectPanesStyle);

            RegisterEdiTextEditor(_documentTypeManager);
            RegisterMiniUml(_documentTypeManager);
        }

        private void RegisterEdiTextEditor(IDocumentTypeManager documentTypeManager)
        {
            // Register these patterns for the build in AvalonEdit text editor
            // All Files (*.*)|*.*
            var docType = documentTypeManager.RegisterDocumentType(EdiViewModel.DocumentKey,
                                                                   EdiViewModel.Description,
                                                                   EdiViewModel.FileFilterName,
                                                                   EdiViewModel.DefaultFilter,
                                                                   EdiViewModel.LoadFile,
                                                                   EdiViewModel.CreateNewDocument,
                                                                   typeof(EdiViewModel),
                                                                   10);

            if (docType != null) // Lets register some sub-types for editing with Edi's text editor
            {
                // Text Files (*.txt)|*.txt
                // C# Files (*.cs)|*.cs
                // HTML Files (*.htm,*.html,*.css,*.js)|*.htm;*.html;*.css;*.js
                // Structured Query Language (*.sql) |*.sql
                var t = docType.CreateItem("Text Files", new List<string>() { "txt" }, 12);
                docType.RegisterFileTypeItem(t);

                t = docType.CreateItem("C# Files", new List<string>() { "cs", "xaml", "config" }, 14);
                docType.RegisterFileTypeItem(t);

                t = docType.CreateItem("HTML Files", new List<string>() { "htm", "html", "css", "js" }, 16);
                docType.RegisterFileTypeItem(t);

                t = docType.CreateItem("Structured Query Language", new List<string>() { "sql" }, 18);
                docType.RegisterFileTypeItem(t);
            }
        }

        private void RegisterMiniUml(IDocumentTypeManager documentTypeManager)
        {
            // Unified Modeling Language (*.uml,*.xml)|*.uml;*.xml
            var docType = documentTypeManager.RegisterDocumentType(MiniUmlViewModel.DocumentKey,
                                                                   MiniUmlViewModel.Description,
                                                                   MiniUmlViewModel.FileFilterName,
                                                                   MiniUmlViewModel.DefaultFilter,
                                                                   MiniUmlViewModel.LoadFile,
                                                                   null,
                                                                   typeof(MiniUmlViewModel),
                                                                   90);

            if (docType != null) // Lets register some sub-types for editing with Edi's text editor
            {
                var t = docType.CreateItem("UML Files", new List<string>() { "uml", "xml" }, 92);
                docType.RegisterFileTypeItem(t);
            }
        }

        /// <summary>
        /// Register viewmodel types with <seealso cref="DataTemplate"/> for a view
        /// and return all definitions with a <seealso cref="PanesTemplateSelector"/> instance.
        /// </summary>
        /// <param name="paneSel"></param>
        /// <returns></returns>
        private void RegisterDataTemplates(PanesTemplateSelector paneSel)
        {
            // StartPageView
            var template = ResourceLocator.GetResource<DataTemplate>(
                                    Assembly.GetAssembly(typeof(StartPageViewModel)).GetName().Name,
                                    "DataTemplates/StartPageViewDataTemplate.xaml",
                                    "StartPageViewDataTemplate") as DataTemplate;

            paneSel.RegisterDataTemplate(typeof(StartPageViewModel), template);

            //EdiView
            template = ResourceLocator.GetResource<DataTemplate>(
                                    Assembly.GetAssembly(typeof(EdiViewModel)).GetName().Name,
                                    "DataTemplates/EdiViewDataTemplate.xaml",
                                    "EdiViewDataTemplate") as DataTemplate;

            paneSel.RegisterDataTemplate(typeof(EdiViewModel), template);

            // MiniUml
            template = ResourceLocator.GetResource<DataTemplate>(
                                    Assembly.GetAssembly(typeof(MiniUmlViewModel)).GetName().Name,
                                    "DataTemplates/MiniUMLViewDataTemplate.xaml",
                                    "MiniUMLViewDataTemplate") as DataTemplate;

            paneSel.RegisterDataTemplate(typeof(MiniUmlViewModel), template);
        }

        private void RegisterStyles(PanesStyleSelector selectPanesStyle)
        {
            var newStyle = ResourceLocator.GetResource<Style>(
                "Edi.Apps",
                "Resources/Styles/AvalonDockStyles.xaml",
                "StartPageStyle");

            selectPanesStyle.RegisterStyle(typeof(StartPageViewModel), newStyle);
        }
        #endregion methods
    }
}
