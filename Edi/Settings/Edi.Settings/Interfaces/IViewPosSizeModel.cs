namespace Edi.Settings.Interfaces
{

    /// <summary>
    /// Simple API for allowing windows to persist their
    /// position, height, and width between user sessions in Properties.Default...
    /// </summary>
    public interface IViewPosSizeModel
    {
        /// <summary>
        /// Gets whether this object was created through the default constructor or not
        /// (default data values can be easily overwritten by actual data).
        /// </summary>
        bool DefaultConstruct { get; }

        /// <summary>
        /// Get/set whether view is amximized or not
        /// </summary>
        bool IsMaximized { get; set; }

        /// <summary>
        /// Get/set height of control
        /// </summary>
        double Height { get; set; }

        /// <summary>
        /// Get/set width of control
        /// </summary>
        double Width { get; set; }

        /// <summary>
        /// Get/set X coordinate of control
        /// </summary>
        double X { get; set; }

        /// <summary>
        /// Get/set Y coordinate of control
        /// </summary>
        double Y { get; set; }

        /// <summary>
        /// Convinience function to set the position of a view to a valid position
        /// </summary>
        void SetValidPos(double SystemParameters_VirtualScreenLeft, double SystemParameters_VirtualScreenTop);
    }
}