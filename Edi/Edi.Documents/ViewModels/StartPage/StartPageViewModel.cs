namespace Edi.Documents.ViewModels.StartPage
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Windows.Input;
    using Core.ViewModels.Command;
    using MsgBox;
    using MRULib.MRU.Interfaces;
    using CommonServiceLocator;

    public class StartPageViewModel : Core.ViewModels.FileBaseViewModel
    {
        #region fields
        public const string StartPageContentId = ">StartPage<";
        #endregion fields

        #region constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public StartPageViewModel()
        {
            Title = Util.Local.Strings.STR_STARTPAGE_TITLE;
            StartPageTip = Util.Local.Strings.STR_STARTPAGE_WELCOME_TT;
            ContentId = StartPageContentId;
        }
        #endregion constructor

        #region properties
        #region CloseCommand

        private RelayCommand<object> _closeCommand;
        public override ICommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new RelayCommand<object>((p) => OnClose(),
                           (p) => CanClose()));
            }
        }
        #endregion

        #region OpenContainingFolder
        private RelayCommand<object> _openContainingFolderCommand;

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
                var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
                msgBox.Show(string.Format(CultureInfo.CurrentCulture, "{0}\n'{1}'.", ex.Message, (FilePath == null ? string.Empty : FilePath)),
                                            Util.Local.Strings.STR_FILE_FINDING_CAPTION,
                                            MsgBoxButtons.OK, MsgBoxImage.Error);
            }
        }
        #endregion OpenContainingFolder

        #region CopyFullPathtoClipboard
        private RelayCommand<object> _copyFullPathtoClipboard;

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
        #endregion CopyFullPathtoClipboard

        public override Uri IconSource => new Uri("pack://application:,,,/Edi.Themes;component/Images/Documents/document.png", UriKind.RelativeOrAbsolute);

        public IMRUListViewModel MruList => ServiceLocator.Current.GetInstance<IMRUListViewModel>();

        public string StartPageTip { get; set; }

        public override bool IsDirty
        {
            get { return false; }

            set
            {
                throw new NotSupportedException("Start page cannot be saved therfore setting dirty cannot be useful.");
            }
        }

        /// <summary>
        /// Get whether edited data can be saved or not.
        /// This type of document does not have a save
        /// data implementation if this property returns false.
        /// (this is document specific and should always be overriden by descendents)
        /// </summary>
        public override bool CanSaveData => false;

        public override string FilePath
        {
            get { return ContentId; }

            protected set { throw new NotSupportedException(); }
        }

        public override string FileName => string.Empty;

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
        #endregion methods
    }
}
