namespace Edi.Core.Interfaces
{
    using System;
    using System.Windows;

    /// <summary>
    /// Defines the methods and properties of a view that can handle AvalonDock layouts
    /// and is positionable on the screen.
    /// </summary>
    public interface ILayoutableWindow
	{
		/// <summary>
		/// Standard Closed Window event.
		/// </summary>
		event EventHandler Closed;

        #region properties
        /// <summary>
        /// Gets the current AvalonDockManager Xml layout and returns it as a string.
        /// </summary>
        string CurrentAdLayout { get; }

        /// <summary>
        /// Gets or sets a value that indicates whether a window is restored, minimized, or maximized.
        /// </summary>
        WindowState WindowState { get; set; }

        /// <summary>
        /// Gets or sets the position of the window's top edge, in relation to the desktop.
        /// 
        /// Returns:
        ///     The position of the window's top, in logical units (1/96").
        /// </summary>
        double Top { get; set; }

        /// <summary>
        /// Gets or sets the position of the window's left edge, in relation to the desktop.
        /// Returns:
        ///     The position of the window's left edge, in logical units (1/96th of an inch).
        /// </summary>
        double Left { get; set; }

        /// <summary>
        /// Gets or sets the width of the element.
        /// Returns:
        ///     The width of the element, in device-independent units (1/96th inch per unit).
        ///     The default value is System.Double.NaN. This value must be equal to or greater
        ///     than 0.0. See Remarks for upper bound information.
        /// </summary>
        double Width { get; set; }

        /// <summary>
        /// Gets or sets the suggested height of the element.
        /// 
        /// Returns:
        ///     The height of the element, in device-independent units (1/96th inch per unit).
        ///     The default value is System.Double.NaN. This value must be equal to or greater
        ///     than 0.0. See Remarks for upper bound information.
        /// </summary>
        double Height { get; set; }
        #endregion properties

        #region methods
        /// <summary>
        /// Call this method to clean-up resource references on exit.
        /// </summary>
        void ReleaseResources();
        #endregion methods
    }
}