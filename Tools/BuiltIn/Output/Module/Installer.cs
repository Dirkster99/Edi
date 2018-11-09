namespace Output.Module
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Edi.Core.Interfaces;
    using Edi.Core.Resources;
    using Edi.Core.View.Pane;
    using Edi.Core.ViewModels;
    using Edi.Interfaces.MessageManager;
    using Output.ViewModels;
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
    public class Installer : IWindsorInstaller
    {
        #region fields
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion fields

        /// <summary>
        /// Performs the installation in the Castle.Windsor.IWindsorContainer.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="store"></param>
        public void Install(IWindsorContainer container,
                            IConfigurationStore store)
        {
            try
            {
                container
                    .Register(Component.For<IOutput>()
                    .ImplementedBy<OutputTWViewModel>().LifestyleSingleton());

                var avLayout = container.Resolve<IAvalonDockLayoutViewModel>();
                var toolRegistry = container.Resolve<IToolWindowRegistry>();
                var messageManager = container.Resolve<IMessageManager>();

                if (avLayout != null)
                    this.RegisterDataTemplates(avLayout.ViewProperties.SelectPanesTemplate);

                if (toolRegistry != null && messageManager != null)
                {
                    var toolVM = container.Resolve<IOutput>();

                    messageManager.RegisterOutputStream(toolVM);
                    toolRegistry.RegisterTool(toolVM as ToolViewModel);
                }
            }
            catch (System.Exception exp)
            {
                Logger.Error(exp);
            }
        }

        #region registering methods
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
                                    Assembly.GetAssembly(typeof(OutputTWViewModel)).GetName().Name,
                                    "DataTemplates/OutputViewDataTemplate.xaml",
                                    "OutputViewDataTemplate") as DataTemplate;

            paneSel.RegisterDataTemplate(typeof(OutputTWViewModel), template);

            return paneSel;
        }
        #endregion registering methods
    }
}
