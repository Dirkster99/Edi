namespace FolderBrowser.Views
{
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Interaction logic for DropDownView.xaml
    /// </summary>
    public partial class DropDownView : UserControl
    {
        public DropDownView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Make the drop down element resizeable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResizeGripThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Thumb MyThumb = sender as Thumb;

            // Set the new Width and Height fo Grid, Popup they will inherit
            double yAdjust = this.Height + e.VerticalChange;
            double xAdjust = this.Width + e.HorizontalChange;

            // Set new Height and Width
            if ((xAdjust >= 0) && (yAdjust >= 0))
            {
                this.Width = xAdjust;
                this.Height = yAdjust;
            }
        }
    }
}
