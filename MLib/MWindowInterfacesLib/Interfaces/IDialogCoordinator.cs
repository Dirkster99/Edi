namespace MWindowInterfacesLib.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// Supports coordination of content dialogs from within
    /// a viewmodel that is attached to a window view.
    /// 
    /// The relevant methods contain a parameter called context to support
    /// this use case. The context is either:
    /// 
    /// 1) An implementation of <seealso cref="IMetroWindow"/> or
    /// 
    /// 2) A ViewModel that is bound to an <seealso cref="IMetroWindow"/> implementation
    ///    and registered via <seealso cref="DialogParticipation"/>.
    /// </summary>
    public interface IDialogCoordinator
    {
        /// <summary>
        /// Gets the current shown dialog.
        /// </summary>
        /// <param name="context">Typically this should be the view model, which you register in XAML using <see cref="DialogParticipation.SetRegister"/>.</param>
        Task<TDialog> GetCurrentDialogAsync<TDialog>(object context) where TDialog : IBaseMetroDialogFrame;

        /// <summary>
        /// Adds a Metro Dialog instance to the specified window and makes it visible asynchronously.        
        /// <para>You have to close the resulting dialog yourself with <see cref="HideMetroDialogAsync"/>.</para>
        /// </summary>
        /// <param name="context">Typically this should be the view model, which you register in XAML using <see cref="DialogParticipation.SetRegister"/>.</param>
        /// <param name="dialog">The dialog instance itself.</param>
        /// <param name="settings">An optional pre-defined settings instance.</param>
        /// <returns>A task representing the operation.</returns>
        /// <exception cref="InvalidOperationException">The <paramref name="dialog"/> is already visible in the window.</exception>
        Task ShowMetroDialogAsync(object context, IBaseMetroDialogFrame dialog,
            IMetroDialogFrameSettings settings = null);

        Task<int> ShowMetroDialogAsync(object context
                                       , IMsgBoxDialogFrame<int> dialog
                                       , IMetroDialogFrameSettings settings = null);

        /// <summary>
        /// Hides a visible Metro Dialog instance.
        /// </summary>
        /// <param name="context">Typically this should be the view model, which you register in XAML using <see cref="DialogParticipation.SetRegister"/>.</param>
        /// <param name="dialog">The dialog instance to hide.</param>
        /// <param name="settings">An optional pre-defined settings instance.</param>
        /// <returns>A task representing the operation.</returns>
        /// <exception cref="InvalidOperationException">
        /// The <paramref name="dialog"/> is not visible in the window.
        /// This happens if <see cref="ShowMetroDialogAsync"/> hasn't been called before.
        /// </exception>
        Task HideMetroDialogAsync(object context, IBaseMetroDialogFrame dialog
                                , IMetroDialogFrameSettings settings = null);

        /// <summary>
        /// Attempts to find the MetroWindow that should show the ContentDialog
        /// by searching the context object in the <seealso cref="DialogParticipation"/>
        /// object.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IMetroWindow GetMetroWindow(object context);
    }
}
