namespace FolderBrowser.ViewModels
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using FolderBrowser.ViewModels.Interfaces;

  /// <summary>
  /// Class a dialog viewmodel MVVM style...
  /// </summary>
  public class DialogViewModel : Base.ViewModelBase
  {
    #region fields
    private bool? mDialogCloseResult;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    public DialogViewModel(IBrowserViewModel browserViewModel = null)
    {
      if (browserViewModel == null)
        this.TreeBrowser = browserViewModel;
      else
        this.TreeBrowser = new BrowserViewModel();

      this.TreeBrowser.FinalPathSelectionEvent += this.browser_PathSelectionEvent;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Gets the viewmodel that drives the folder picker control.
    /// </summary>
    public IBrowserViewModel TreeBrowser { get; private set; }

    /// <summary>
    /// This can be used to close the attached view via ViewModel
    /// 
    /// Source: http://stackoverflow.com/questions/501886/wpf-mvvm-newbie-how-should-the-viewmodel-close-the-form
    /// </summary>
    public bool? DialogCloseResult
    {
      get
      {
        return this.mDialogCloseResult;
      }

      private set
      {
        if (this.mDialogCloseResult != value)
        {
          this.mDialogCloseResult = value;
          this.RaisePropertyChanged(() => this.DialogCloseResult);
        }
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Is invoked when this path is selected as dialog result by the user.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void browser_PathSelectionEvent(object sender, FileSystemModels.Events.FolderChangedEventArgs e)
    {
      this.DialogCloseResult = true;
    }
    #endregion methods
  }
}
