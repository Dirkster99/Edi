namespace Log4NetTools.Module
{
    using Edi.Core.Interfaces;
    using Edi.Core.Interfaces.DocumentTypes;
    using Edi.Core.Resources;
    using Edi.Core.View.Pane;
    using Edi.Settings.Interfaces;
    using Log4NetTools.ViewModels;
    using Prism.Mef.Modularity;
    using Prism.Modularity;
    using System.Collections.Generic;
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
    [ModuleExport(typeof(MEFLoadLog4NetTools))]
	public class MEFLoadLog4NetTools : IModule
	{
		#region fields
		private readonly IAvalonDockLayoutViewModel mAvLayout;
		private readonly IToolWindowRegistry mToolRegistry;
		private readonly ISettingsManager mSettingsManager;
		private readonly IDocumentTypeManager mDocumentTypeManager;
		#endregion fields

		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="avLayout"></param>
		[ImportingConstructor]
		public MEFLoadLog4NetTools(IAvalonDockLayoutViewModel avLayout,
															 IToolWindowRegistry toolRegistry,
															 ISettingsManager settingsManager,
															 IDocumentTypeManager documentTypeManager)
		{
			this.mAvLayout = avLayout;
			this.mToolRegistry = toolRegistry;
			this.mSettingsManager = settingsManager;
			this.mDocumentTypeManager = documentTypeManager;
		}

		#region methods
		/// <summary>
		/// Initialize this module via standard PRISM MEF procedure
		/// </summary>
		void IModule.Initialize()
		{
			this.RegisterDataTemplates(this.mAvLayout.ViewProperties.SelectPanesTemplate);
			this.RegisterStyles(this.mAvLayout.ViewProperties.SelectPanesStyle);

			this.mToolRegistry.RegisterTool(new Log4NetToolViewModel());
			this.mToolRegistry.RegisterTool(new Log4NetMessageToolViewModel());

			var docType = this.mDocumentTypeManager.RegisterDocumentType(Log4NetViewModel.DocumentKey,
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
