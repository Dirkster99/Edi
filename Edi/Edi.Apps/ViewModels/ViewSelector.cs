using System;
using System.Windows;
using Edi.Dialogs.About;
using Edi.Dialogs.FindReplace;
using Edi.Dialogs.FindReplace.ViewModel;
using Edi.Dialogs.GotoLine;
using Edi.SettingsView.Config;
using Edi.SettingsView.Config.ViewModels;

namespace Edi.Apps.ViewModels
{
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

			if (viewModel is ConfigViewModel) // Return programm settings dialog instance
				win = new ConfigDlg { Title = "Settings..." };
			else
				if (viewModel is AboutViewModel)             // Return about programm dialog instance
					win = new AboutDlg();
				else
					if (viewModel is GotoLineViewModel)       // Return goto line dialog instance
						win = new GotoLineDlg();
					else
						if (viewModel is FindReplaceViewModel) // Return find replace dialog instance
							win = new FindReplaceDialog();

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
