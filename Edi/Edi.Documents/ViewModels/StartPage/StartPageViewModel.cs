namespace Edi.Documents.ViewModels.StartPage
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Windows.Input;
    using Core.ViewModels.Command;
    using MsgBox;
    using MRULib.MRU.Interfaces;

    public class StartPageViewModel : Core.ViewModels.FileBaseViewModel
    {
        #region fields
        public const string StartPageContentId = ">StartPage<";

        private ICommand _closeCommand;
        private ICommand _copyFullPathtoClipboard;
        private ICommand _openContainingFolderCommand;
        private readonly IMRUListViewModel _MruList;
        #endregion fields

        #region constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public StartPageViewModel(IMRUListViewModel MruList)
        {
            _MruList = MruList;
            Title = Util.Local.Strings.STR_STARTPAGE_TITLE;
            StartPageTip = Util.Local.Strings.STR_STARTPAGE_WELCOME_TT;
            ContentId = StartPageContentId;
        }
        #endregion constructor

        #region properties
        public override ICommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new RelayCommand<object>(
                    (p) => OnClose(),
                    (p) => CanClose()));
            }
        }

        /// <summary>
        /// Get open containing folder command which will open
        /// the folder containing the executable in windows explorer
        /// and select the file.
        /// </summary>
        public new ICommand OpenContainingFolderCommand
        {
            get
            {
                return _openContainingFolderCommand ?? (_openContainingFolderCommand =
                           new RelayCommand<object>((p) => OnOpenContainingFolderCommand()));
            }
        }

        /// <summary>
        /// Get CopyFullPathtoClipboard command which will copy
        /// the path of the executable into the windows clipboard.
        /// </summary>
        public new ICommand CopyFullPathtoClipboard
        {
            get
            {
                return _copyFullPathtoClipboard ?? (_copyFullPathtoClipboard =
                           new RelayCommand<object>((p) => OnCopyFullPathtoClipboardCommand()));
            }
        }

        /// <summary>
        /// Gets the Uri of the Icon that AvalonDock should display for the StartPage document.
        /// </summary>
        public override Uri IconSource
        {
            get
            {
                return new Uri("pack://application:,,,/Edi.Themes;component/Images/Documents/document.png", UriKind.RelativeOrAbsolute);
            }
        }

        /// <summary>
        /// Gets the MRU List viewmodel for display and editing within the Startpage.
        /// </summary>
        public IMRUListViewModel MruList { get { return _MruList; } }

        /// <summary>
        /// Gets a tool tip describing the startpage in a short textual form to the user.
        /// </summary>
        public string StartPageTip { get; set; }

        /// <summary>
        /// Gets whether the StartPage is Dirty (has changed data that needs persistence) or not.
        /// </summary>
        public override bool IsDirty
        {
            get { return false; }

            set
            {
                throw new NotSupportedException("Start page cannot be saved therfore setting dirty cannot be useful.");
            }
        }

        /// <summary>
        /// Gets whether edited data can be saved or not.
        /// 
        /// This type of document does not have a save
        /// data implementation if this property returns false.
        /// (this is document specific and should always be overriden by descendents)
        /// </summary>
        public override bool CanSaveData { get { return false; } }

        /// <summary>
        /// Gets a ContentId or actual path into the file system of this document.
        /// </summary>
        public override string FilePath
        {
            get { return ContentId; }

            protected set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the file name (document can be persisted) or
        /// an empty string (document cannot be saved in file system).
        /// </summary>
        public override string FileName { get { return string.Empty; } }
        #endregion properties

        #region methods
        /// <summary>
        /// Get a path that does not represent this document that indicates
        /// a useful alternative representation (eg: StartPage -> Assembly Path).
        /// </summary>
        /// <returns></returns>
        public new string GetAlternativePath()
        {
            return Assembly.GetEntryAssembly().Location;
        }

        public override bool CanSave() { return false; }

        public override bool CanSaveAs() { return false; }

        public override bool SaveFile(string filePath)
        {
            throw new NotImplementedException();
        }

        public override string GetFilePath()
        {
            throw new NotSupportedException("Start Page does not have a valid file path.");
        }

        /// <summary>
        /// Implements the <see cref="OpenContainingFolderCommand"/> which starts the Windows Explorer
        /// and highlights the current document within its folder.
        /// </summary>
        private void OnOpenContainingFolderCommand()
        {
            try
            {
                // combine the arguments together it doesn't matter if there is a space after ','
                string argument = @"/select, " + GetAlternativePath();

                System.Diagnostics.Process.Start("explorer.exe", argument);
            }
            catch (Exception ex)
            {
                _MsgBox.Show(string.Format(CultureInfo.CurrentCulture, "{0}\n'{1}'.", ex.Message, (FilePath == null ? string.Empty : FilePath)),
                                           Util.Local.Strings.STR_FILE_FINDING_CAPTION,
                                           MsgBoxButtons.OK, MsgBoxImage.Error);
            }
        }

        /// <summary>
        /// Implements the <see cref="CopyFullPathtoClipboard"/> command.
        /// </summary>
        private void OnCopyFullPathtoClipboardCommand()
        {
            try
            {
                System.Windows.Clipboard.SetText(GetAlternativePath());
            }
            catch
            {
                // ignored
            }
        }
        #endregion methods
    }
}
