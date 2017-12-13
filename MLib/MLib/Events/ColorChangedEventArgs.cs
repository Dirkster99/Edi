namespace MLib.Events
{
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Represents the method that will handle the
    /// <see cref="ColorChangedEventArgs"/> routed event.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data (new color).</param>
    public delegate void ColorChangedEventHandler(object sender, ColorChangedEventArgs e);

    public class ColorChangedEventArgs : RoutedEventArgs
    {
        #region constructors
        public ColorChangedEventArgs()
        {
            this.NewColor = Color.FromRgb(0, 0, 0);
        }

        public ColorChangedEventArgs(Color newColor)
        {
            this.NewColor = Color.FromRgb(newColor.R, newColor.G, newColor.B);
        }
        #endregion constructors

        #region properties
        public Color NewColor { get; private set; }
        #endregion properties
    }
}
