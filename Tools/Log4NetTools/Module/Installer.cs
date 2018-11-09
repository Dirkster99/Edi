namespace Log4NetTools.Module
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Edi.Core.Interfaces;
    using Edi.Core.Interfaces.DocumentTypes;
    using Edi.Core.Resources;
    using Edi.Core.View.Pane;
    using Log4NetTools.ViewModels;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows;

    /// <summary>
    /// This class gets picked up by from Castle.Windsor because
    /// it implements the <see cref="IWindsorInstaller"/> interface.
    /// 
    /// The <see cref="IWindsorInstaller"/> interface is used by the
    /// container to resolve installers when calling
    /// <see cref="IWindsorContainer"/>.Install(FromAssembly.This()); 
    /// </summary>
    public class Installers : IWindsorInstaller
    {
        #region fields
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion fields

        /// <summary>
        /// Performs the installation in the Castle.Windsor.IWindsorContainer.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="store"></param>
        void IWindsorInstaller.Install(IWindsorContainer container,
                                       IConfigurationStore store)
        {
            Logger.InfoFormat("Registering Log4NetTools.Module");

            try
            {
                var avLayout = container.Resolve<IAvalonDockLayoutViewModel>();
                var toolRegistry = container.Resolve<IToolWindowRegistry>();
                var documentTypeManager = container.Resolve<IDocumentTypeManager>();

                Initialize(avLayout, toolRegistry, documentTypeManager);
            }
            catch (System.Exception exp)
            {
                Logger.Error(exp);
            }
        }

        #region registering methods
        /// <summary>
        /// Initialize this module via standard PRISM MEF procedure
        /// </summary>
        private void Initialize(IAvalonDockLayoutViewModel avLayout,
                                IToolWindowRegistry toolRegistry,
                                IDocumentTypeManager documentTypeManager)
        {
            this.RegisterDataTemplates(avLayout.ViewProperties.SelectPanesTemplate);
            this.RegisterStyles(avLayout.ViewProperties.SelectPanesStyle);

            toolRegistry.RegisterTool(new Log4NetToolViewModel());
            toolRegistry.RegisterTool(new Log4NetMessageToolViewModel());

            var docType = documentTypeManager.RegisterDocumentType
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
        #endregion registering methods
    }
}
