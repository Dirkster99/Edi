namespace Files.Module
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

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
            Logger.InfoFormat("Registering Files.Module");

            try
            {
                container.Register(Component.For<InstallModule>());
                var modul = container.Resolve<InstallModule>();
                modul.Initialize();
            }
            catch (System.Exception exp)
            {
                Logger.Error(exp);
            }
        }
    }
}