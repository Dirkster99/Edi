namespace MLib
{
    using MLib.Interfaces;
    using MLib.Internal;

    /// <summary>
    /// Helper class to initialize an
    /// <see cref="IAppearanceManager"/> service interface.
    /// </summary>
    public sealed class AppearanceManager
    {
        /// <summary>
        /// Hidden default constructor.
        /// </summary>
        private AppearanceManager()
        {
        }

        /// <summary>
        /// Gets an instance of an object that implements the
        /// <see cref="IAppearanceManager"/> interface.
        /// </summary>
        /// <returns></returns>
        public static IAppearanceManager GetInstance()
        {
            return new AppearanceManagerImpl();
        }
    }
}
