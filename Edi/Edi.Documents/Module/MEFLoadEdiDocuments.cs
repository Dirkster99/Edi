namespace Edi.Documents.Module
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Reflection;
    using System.Windows;
    using Core.Interfaces;
    using Core.Interfaces.DocumentTypes;
    using Core.Resources;
    using Core.View.Pane;
    using ViewModels.EdiDoc;
    using ViewModels.MiniUml;
    using ViewModels.StartPage;
    using Prism.Mef.Modularity;
    using Prism.Modularity;
    using Settings.Interfaces;

    /// <summary>
    /// PRISM MEF Loader/Initializer class
    /// Relevante services are injected in constructor.
    /// 
    /// Requires the following XML in App.config to enable PRISM MEF to pick-up contained definitions.
    /// 
    /// &lt;modules>
    /// &lt;!-- Edi.Tools assembly from plugins folder and initialize it if it was present -->
    /// &lt;module assemblyFile="EdiTools.dll"
    /// 				moduleType="EdiTools.Module.Loader, EdiTools.Module.Loader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
    /// 				moduleName="Loader"
    /// 				startupLoaded="true" />
    /// &lt;/modules>
    /// </summary>
    [ModuleExport(typeof(MEFLoadEdiDocuments))]
    public class MEFLoadEdiDocuments : IModule
    {
        #region fields
        private readonly IAvalonDockLayoutViewModel mAvLayout;
        private readonly IToolWindowRegistry mToolRegistry;
        private readonly IDocumentTypeManager mDocumentTypeManager;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="avLayout"></param>
        [ImportingConstructor]
        public MEFLoadEdiDocuments(IAvalonDockLayoutViewModel avLayout,
                                   IToolWindowRegistry toolRegistry,
                                   ISettingsManager settingsManager,
                                   IDocumentTypeManager documentTypeManager)
        {
            mAvLayout = avLayout;
            mToolRegistry = toolRegistry;
            mDocumentTypeManager = documentTypeManager;
        }
        #endregion constructors

        #region methods
        /// <summary>
        /// Initialize this module via standard PRISM MEF procedure
        /// </summary>
        void IModule.Initialize()
        {
            RegisterDataTemplates(mAvLayout.ViewProperties.SelectPanesTemplate);
            RegisterStyles(mAvLayout.ViewProperties.SelectPanesStyle);

            RegisterEdiTextEditor(mDocumentTypeManager);
            RegisterMiniUml(mDocumentTypeManager);
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
