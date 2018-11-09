namespace Edi.Core.Module
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Edi.Core.Interfaces;
    using Edi.Core.Models;
    using Edi.Interfaces.App;
    using Edi.Interfaces.MessageManager;

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
        void IWindsorInstaller.Install(IWindsorContainer container,
                                       IConfigurationStore store)
        {
            Logger.InfoFormat("Registering Edi.Core.Module");

            try
            {
                // Register Application Helper class to help castle satisfy dependencies on it
                container
                    .Register(Component.For<IAppCore>()
                    .ImplementedBy<AppCore>().LifestyleSingleton());

                container
                    .Register(Component.For<IToolWindowRegistry>()
                    .ImplementedBy<ToolWindowRegistry>().LifestyleSingleton());

                container
                    .Register(Component.For<IMessageManager>()
                    .ImplementedBy<MessageManager>().LifestyleSingleton());
            }
            catch (System.Exception exp)
            {
                Logger.Error(exp);
            }
        }
    }
}