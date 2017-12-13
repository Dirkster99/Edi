namespace MWindowInterfacesLib.Interfaces
{
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Defines the base interface items that should be implemented by any
    /// view that supports a content dialog of any type.
    /// </summary>
    public interface IBaseMetroDialogFrame
    {
        #region properties
        /// <summary>
        /// Gets the standard dialog settings for this dialog.
        /// </summary>
        IMetroDialogFrameSettings DialogSettings { get; }

        /// <summary>
        /// Gets/sets the dialog's title.
        /// </summary>
        string Title { get; set; }

        //
        // Summary:
        //     Gets or sets the maximum height constraint of the element.
        //
        // Returns:
        //     The maximum height of the element, in device-independent units (1/96th inch per
        //     unit). The default value is System.Double.PositiveInfinity. This value can be
        //     any value equal to or greater than 0.0. System.Double.PositiveInfinity is also
        //     valid.
        [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
        [TypeConverter(typeof(LengthConverter))]
        double MaxHeight { get; set; }

        //
        // Summary:
        //     Gets or sets the minimum height constraint of the element.
        //
        // Returns:
        //     The minimum height of the element, in device-independent units (1/96th inch per
        //     unit). The default value is 0.0. This value can be any value equal to or greater
        //     than 0.0. However, System.Double.PositiveInfinity is NOT valid, nor is System.Double.NaN.
        [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
        [TypeConverter(typeof(LengthConverter))]
        double MinHeight { get; set; }

        //
        // Summary:
        //     Gets or sets the data context for an element when it participates in data binding.
        //
        // Returns:
        //     The object to use as data context.
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Localizability(LocalizationCategory.NeverLocalize)]
        object DataContext { get; set; }
        #endregion properties

        #region events
        /// <summary>
        /// Gets/sets an event handler that is invoked when the <seealso cref="IMetroWindow"/>
        /// has chnaged its size. The event coupling is necessary to have the content dialog
        /// change its size accordingly.
        /// </summary>
        SizeChangedEventHandler SizeChangedHandler { get; set; }
        #endregion events

        #region methods
        /// <summary>
        /// Waits for the dialog to become ready for interaction.
        /// </summary>
        /// <returns>A task that represents the operation and it's status.</returns>
        Task WaitForLoadAsync();

        /// <summary>
        /// Waits until this dialog gets unloaded.
        /// </summary>
        /// <returns></returns>
        Task WaitUntilUnloadedAsync();

        /// <summary>
        /// Waits until this dialog is closed
        /// (Storyboard animation may require some extra time etc.).
        /// </summary>
        /// <returns></returns>
        Task _WaitForCloseAsync();

        /// <summary>
        /// Method is called upon closing of the window to resources
        /// should there be any to clean-up.
        /// </summary>
        void OnClose();

        /// <summary>
        /// Method is called between dialog load and show to allocate
        /// extra resources should there be any need for that.
        /// </summary>
        void OnShown();

        /// <summary>
        /// Set the ZIndex value for this <seealso cref="BaseMetroDialog"/>.
        /// This method can make sure that a given dialog is visible when more
        /// than one dialog is open.
        /// </summary>
        /// <param name="newPanelIndex"></param>
        void SetZIndex(int newPanelIndex);
        #endregion methods
    }
}