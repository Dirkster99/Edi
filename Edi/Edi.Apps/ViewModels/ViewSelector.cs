namespace Edi.Apps.ViewModels
{
	using System;
	using System.Windows;

	/// <summary>
	/// This class matches a viewmodel type with its corresponding dialog view.
	/// </summary>
	public class ViewSelector
	{
		/// <summary>
		/// Return a new view object for a matching type of ViewModel object
		/// (this limits the dependencies between view and viewmodel down to one)
		/// </summary>
		public static Window GetDialogView(object viewModel, Window parent = null)
		{
			if (viewModel == null)
				throw new Exception("The viewModel parameter cannot be null.");

			Window win = null;

			if (viewModel is SettingsView.Config.ViewModels.ConfigViewModel) // Return programm settings dialog instance
				win = new SettingsView.Config.ConfigDlg() { Title = "Settings..." };
			else
				if (viewModel is EdiDialogs.About.AboutViewModel)             // Return about programm dialog instance
					win = new EdiDialogs.About.AboutDlg();
				else
					if (viewModel is EdiDialogs.GotoLine.GotoLineViewModel)       // Return goto line dialog instance
						win = new EdiDialogs.GotoLine.GotoLineDlg();
					else
						if (viewModel is EdiDialogs.FindReplace.ViewModel.FindReplaceViewModel) // Return find replace dialog instance
							win = new EdiDialogs.FindReplace.FindReplaceDialog();

			if (win != null)
			{
				win.Owner = parent;
				win.DataContext = viewModel;

				return win;
			}

			throw new NotSupportedException(viewModel.GetType().ToString());
		}
	}
}
