﻿namespace Output.Module
{
    using Edi.Core.Interfaces;
    using Edi.Core.Resources;
    using Edi.Core.View.Pane;
    using ViewModels;
    using System.ComponentModel.Composition;
    using System.Reflection;
    using System.Windows;

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
    [ModuleExport(typeof(MEFLoadOutputTools))]
    public class MEFLoadOutputTools : IModule
    {
        #region fields
        private readonly IAvalonDockLayoutViewModel mAvLayout;
        private readonly IToolWindowRegistry mToolRegistry;
        private readonly IMessageManager mMessageManager;
        #endregion fields

        static MEFLoadOutputTools()
        {
            
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="avLayout"></param>
        [ImportingConstructor]
        public MEFLoadOutputTools(IAvalonDockLayoutViewModel avLayout,
                                  IToolWindowRegistry toolRegistry,
                                  IMessageManager messageManager)
        {
            mAvLayout = avLayout;
            mToolRegistry = toolRegistry;
            mMessageManager = messageManager;
        }

        #region methods
        /// <summary>
        /// Initialize this module via standard PRISM MEF procedure
        /// </summary>
        void IModule.Initialize()
        {
            if (mAvLayout != null)
            {
                RegisterDataTemplates(mAvLayout.ViewProperties.SelectPanesTemplate);
            }

            if (mToolRegistry != null && mMessageManager != null)
            {
                var toolVM = new OutputViewModel();

                mMessageManager.RegisterOutputStream(toolVM);
                mToolRegistry.RegisterTool(toolVM);
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
                                    Assembly.GetAssembly(typeof(OutputViewModel)).GetName().Name,
                                    "DataTemplates/OutputViewDataTemplate.xaml",
                                    "OutputViewDataTemplate") as DataTemplate;

            paneSel.RegisterDataTemplate(typeof(OutputViewModel), template);

            return paneSel;
        }
        #endregion methods
    }
}