namespace Edi.Dialogs.FindReplace
{
	using System.Windows.Input;

	/// <summary>
	/// Interaction logic for FindReplaceDialog.xaml
	/// </summary>
	public partial class FindReplaceDialog : FirstFloor.ModernUI.Windows.Controls.ModernWindow
	{
		public FindReplaceDialog()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// Close the dialog when the user hits escape
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
				Close();
		}
	}
}
