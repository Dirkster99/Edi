namespace Edi
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Castle.Windsor.Installer;
    using Edi.Apps.Interfaces;
    using Edi.Apps.ViewModels;
    using Edi.Apps.Views.Shell;
    using Edi.Core.Interfaces;
    using Edi.Core.Interfaces.DocumentTypes;
    using Edi.Core.Models.DocumentTypes;
    using MRULib.MRU.Interfaces;
    using MsgBox;
    using System;
    using System.Diagnostics;

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
        /// Implements the <see cref="IWindsorInstaller"/> interface to
        /// performs the installation in the <see cref="IWindsorContainer"/>
        /// (performed automatically when Castle scans the containing assembly).
        /// </summary>
        /// <param name="container"></param>
        /// <param name="store"></param>
        public void Install(IWindsorContainer container,
                            IConfigurationStore store)
        {

            try
            {
                string fullPath = System.Reflection.Assembly.GetAssembly(typeof(Installers)).Location;
                string dir = System.IO.Path.GetDirectoryName(fullPath);

                // Do a Build Solution/Rebuild Solution to ensure that all modules have been build
                // if you are running into this assertion being raised. You should do:
                // Solution > Clean Solution
                // Solution > Rebuild Solution to fix this issue
                Debug.Assert(System.IO.File.Exists(System.IO.Path.Combine(dir, "Output.dll")));

                container.Install(FromAssembly.Named(System.IO.Path.Combine(dir, "Output.dll")));
                container.Install(FromAssembly.Named(System.IO.Path.Combine(dir, "Files.dll")));
                container.Install(FromAssembly.Named(System.IO.Path.Combine(dir, "Edi.Documents.dll")));
                container.Install(FromAssembly.Named(System.IO.Path.Combine(dir, @"Plugins\Log4NetTools\Log4NetTools.dll")));
            }
            catch (Exception exp)
            {
                Debug.WriteLine("A Core loader error occurred {0}", exp.Message);
                Debug.WriteLine("Stacktrace {0}", exp.StackTrace);
                Logger.Error(exp);
            }

            // Register shell to have a MainWindow to start up with
            container
                .Register(Component.For<Edi.Apps.IShell<MainWindow>>()
                .ImplementedBy<Edi.Apps.Shell>().LifestyleTransient());

            // Register MainWindow to help castle satisfy Shell dependencies on it
            container.Register(Component.For<MainWindow>().LifestyleTransient());
        }

        /// <summary>
        /// Installs the core modules of this application into <see cref="IWindsorContainer"/>
        /// and returns it to continue initialization/start-up using the core modules.
        /// </summary>
        /// <param name="container"></param>
        internal static void InstallWindsorCore(IWindsorContainer container)
        {
            // Register Messagebox service to help castle satisfy dependencies on it
            container
                .Register(Component.For<IMessageBoxService>()
                .ImplementedBy<MessageBoxService>().LifestyleSingleton());

            try
            {
                string fullPath = System.Reflection.Assembly.GetAssembly(typeof(Installers)).Location;
                string dir = System.IO.Path.GetDirectoryName(fullPath);

                container.Install(FromAssembly.Named(System.IO.Path.Combine(dir, "Edi.Core.dll")));
                container.Install(FromAssembly.Named(System.IO.Path.Combine(dir, "Edi.Themes.dll")));
                container.Install(FromAssembly.Named(System.IO.Path.Combine(dir, "Edi.Settings.dll")));
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
            }

            container.Register(Component.For<IMRUListViewModel>().Instance(MRULib.MRU_Service.Create_List()).LifestyleSingleton());

            container
                .Register(Component.For<IAvalonDockLayoutViewModel>()
                .ImplementedBy<AvalonDockLayoutViewModel>().LifestyleSingleton());

            container
                .Register(Component.For<IDocumentTypeManager>()
                .ImplementedBy<DocumentTypeManager>().LifestyleSingleton());

            container
                .Register(Component.For<IFileOpenService, IApplicationViewModel>()
                .ImplementedBy<ApplicationViewModel>().LifestyleSingleton());
        }
    }
}