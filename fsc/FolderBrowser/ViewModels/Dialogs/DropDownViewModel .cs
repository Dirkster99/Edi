namespace FolderBrowser.Dialogs.ViewModels
{
    using FolderBrowser.Dialogs.Interfaces;
    using FolderBrowser.Interfaces;
    using FileSystemModels.ViewModels.Base;
    using System.Windows.Input;
    using FileSystemModels.Interfaces.Bookmark;

    /// <summary>
    /// A viewmodel in MVVM style to drive a folder browser
    /// or folder picker view that is display in a WPF drop
    /// down button view.
    /// </summary>
    internal class DropDownViewModel : DialogBaseViewModel, IDropDownViewModel
    {
        #region fields
        private double mWidth = 600;
        private double mHeight = 400;
        private bool mIsOpen = false;

        private RelayCommand<object> mOKCommand;
        private RelayCommand<object> mCancelCommand;
        private string mButtonLabel = "Select a Folder";
        private UpdateCurrentPath mUpdateInitialPath;
        private UpdateBookmarks mUpdateInitialBookmarks;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public DropDownViewModel(
            IBrowserViewModel treeBrowser,
            IBookmarksViewModel recentLocations,
            DropDownClosedResult resultCallback)
            : base (treeBrowser, recentLocations)
        {
            ResultCallback = resultCallback;
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Gets/sets a property that can be used to specify a delegete method that is called upon
        /// close of the drop down button element.
        /// </summary>
        public DropDownClosedResult ResultCallback { get; private set; }

        /// <summary>
        /// Gets/sets the width of the drop down control.
        /// </summary>
        public double Width
        {
            get
            {
                return mWidth;
            }

            set
            {
                if (mWidth != value)
                {
                    mWidth = value;
                    RaisePropertyChanged(() => Width);
                }
            }
        }

        /// <summary>
        /// Gets/sets the height of the drop down control.
        /// </summary>
        public double Height
        {
            get
            {
                return mHeight;
            }

            set
            {
                if (mHeight != value)
                {
                    mHeight = value;
                    RaisePropertyChanged(() => Height);
                }
            }
        }

        /// <summary>
        /// Gets the selected item of the DropDown viewmodel.
        /// </summary>
        public string ButtonLabel
        {
            get
            {
                return mButtonLabel;
            }

            set
            {
                if (mButtonLabel != value)
                {
                    mButtonLabel = value;
                    RaisePropertyChanged(() => ButtonLabel);
                }
            }
        }

        /// <summary>
        /// Gets/sets bound property to determine whether
        /// drop-down/pop-up element is open or not.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return this.mIsOpen;
            }

            set
            {
                if (this.mIsOpen != value)
                {
                    if (this.mIsOpen == false && value == true)
                    {
                        if (UpdateInitialPath != null)
                            TreeBrowser.InitialPath = UpdateInitialPath();

                        if (UpdateInitialBookmarks != null)
                            base.ResetBookmarks(UpdateInitialBookmarks());
                    }

                    this.mIsOpen = value;
                    this.RaisePropertyChanged(() => this.IsOpen);
                }
            }
        }

        public UpdateCurrentPath UpdateInitialPath
        {
            get
            {
                return this.mUpdateInitialPath;
            }

            set
            {
                if (this.mUpdateInitialPath != value)
                    this.mUpdateInitialPath = value;
            }
        }

        public UpdateBookmarks UpdateInitialBookmarks
        {
            get
            {
                return this.mUpdateInitialBookmarks;
            }

            set
            {
                if (this.mUpdateInitialBookmarks != value)
                    this.mUpdateInitialBookmarks = value;
            }
        }
        

        /// <summary>
        /// Gets a command to implement the Cancel (button) click command.
        /// </summary>
        public ICommand CancelCommand
        {
            get
            {
                if (this.mCancelCommand == null)
                    this.mCancelCommand = new RelayCommand<object>((p) =>
                    {
                        if (ResultCallback != null)
                            ResultCallback(BookmarkedLocations, TreeBrowser.SelectedFolder, Result.Cancel);

                        this.IsOpen = false;
                    });

                return this.mCancelCommand;
            }
        }

        /// <summary>
        /// Gets a command to implement the OK (button) click command.
        /// </summary>
        public ICommand OKCommand
        {
            get
            {
                if (this.mOKCommand == null)
                    this.mOKCommand = new RelayCommand<object>((p) =>
                    {
                        if (ResultCallback != null)
                            ResultCallback(BookmarkedLocations, TreeBrowser.SelectedFolder, Result.OK);

                        this.IsOpen = false;
                    });

                return this.mOKCommand;
            }
        }
        #endregion properties
    }
}
