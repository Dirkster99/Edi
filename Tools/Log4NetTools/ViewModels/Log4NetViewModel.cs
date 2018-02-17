namespace Log4NetTools.ViewModels
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Windows.Input;
    using Edi.Core.Interfaces.Documents;
    using Edi.Core.ViewModels.Command;

    public class Log4NetViewModel : Edi.Core.ViewModels.FileBaseViewModel
    {
        #region fields
        public const string DocumentKey = "Log4NetView";
        public const string Description = "Log4Net";
        public const string FileFilterName = "Log4Net";
        public const string DefaultFilter = "log4j";

        private static int iNewFileCounter = 1;
        private string defaultFileType = "log4j";
        private readonly static string defaultFileName = Edi.Util.Local.Strings.STR_FILE_DEFAULTNAME;

        private YalvLib.ViewModel.YalvViewModel mYalvVM = null;
        #endregion fields

        #region constructor
        /// <summary>
        /// ViewModel Class Constructor
        /// </summary>
        public Log4NetViewModel()
            : base(DocumentKey)
        {
            ScreenTip = Edi.Util.Local.Strings.STR_LOG4NET_DOCUMENTTAB_TT;
            ContentId = string.Empty;
            IsReadOnlyReason = Edi.Util.Local.Strings.STR_LOG4NET_READONY_REASON;

            FilePath = string.Format(CultureInfo.InvariantCulture, "{0} {1}.{2}",
                                          defaultFileName,
                                          iNewFileCounter++,
                                          defaultFileType);

            mYalvVM = new YalvLib.ViewModel.YalvViewModel();
        }
        #endregion constructor

        #region properties
        public string ScreenTip { get; set; }

        public string IsReadOnlyReason { get; set; }

        #region FilePath
        private string mFilePath = null;

        /// <summary>
        /// Get/set complete path including file name to where this stored.
        /// This string is never null or empty.
        /// </summary>
        override public string FilePath
        {
            get
            {
                if (mFilePath == null || mFilePath == String.Empty)
                    return string.Format(CultureInfo.CurrentCulture, "{0}.{1}",
                                                             defaultFileName, defaultFileType);

                return mFilePath;
            }

            protected set
            {
                if (mFilePath != value)
                {
                    mFilePath = value;

                    RaisePropertyChanged(() => FilePath);
                    RaisePropertyChanged(() => FileName);
                    RaisePropertyChanged(() => Title);
                }
            }
        }
        #endregion

        #region Title
        /// <summary>
        /// Title is the string that is usually displayed - with or without dirty mark '*' - in the docking environment
        /// </summary>
        public override string Title
        {
            get
            {
                return FileName + (IsDirty ? "*" : string.Empty);
            }
        }
        #endregion

        #region FileName
        /// <summary>
        /// FileName is the string that is displayed whenever the application refers to this file, as in:
        /// string.Format(CultureInfo.CurrentCulture, "Would you like to save the '{0}' file", FileName)
        /// 
        /// Note the absense of the dirty mark '*'. Use the Title property if you want to display the file
        /// name with or without dirty mark when the user has edited content.
        /// </summary>
        public override string FileName
        {
            get
            {
                // This option should never happen - its an emergency break for those cases that never occur
                if (FilePath == null || FilePath == String.Empty)
                    return string.Format(CultureInfo.InvariantCulture, "{0}.{1}",
                                                             defaultFileName,
                                                             defaultFileType);

                return Path.GetFileName(FilePath);
            }
        }

        public override Uri IconSource
        {
            get
            {
                // This icon is visible in AvalonDock's Document Navigator window
                return new Uri("pack://application:,,,/Edi.Themes;component/Images/Documents/Log4net.png", UriKind.RelativeOrAbsolute);
            }
        }
        #endregion FileName

        #region DocumentCommands
        /// <summary>
        /// IsDirty indicates whether the file currently loaded
        /// in the editor was modified by the user or not
        /// (this should always be false since log4net documents cannot be edit and saved).
        /// </summary>
        override public bool IsDirty
        {
            get
            {
                return false;
            }

            set
            {
                throw new NotSupportedException("Log4Net documents cannot be saved therfore setting dirty cannot be useful.");
            }
        }

        /// <summary>
        /// Get whether edited data can be saved or not.
        /// This type of document does not have a save
        /// data implementation if this property returns false.
        /// (this is document specific and should always be overriden by descendents)
        /// </summary>
        override public bool CanSaveData
        {
            get
            {
                return false;
            }
        }

        override public bool CanSave() { return false; }

        override public bool CanSaveAs() { return false; }

        override public bool SaveFile(string filePath)
        {
            throw new NotImplementedException();
        }

        #region CloseCommand
        RelayCommand<object> _closeCommand = null;
        public override ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand<object>((p) => OnClose(),
                                                                                                     (p) => CanClose());
                }

                return _closeCommand;
            }
        }
        #endregion
        #endregion DocumentCommands

        public YalvLib.ViewModel.YalvViewModel Yalv
        {
            get
            {
                return mYalvVM;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Get the path of the file or empty string if file does not exists on disk.
        /// </summary>
        /// <returns></returns>
        override public string GetFilePath()
        {
            try
            {
                if (File.Exists(FilePath))
                    return Path.GetDirectoryName(FilePath);
            }
            catch
            {
            }

            return string.Empty;
        }

        public static Log4NetViewModel LoadFile(IDocumentModel dm, object o)
        {
            return LoadFile(dm.FileNamePath);
        }

        /// <summary>
        /// Load a log4net file and return the corresponding viewmodel representation for it.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static Log4NetViewModel LoadFile(string filePath)
        {
            bool IsFilePathReal = false;

            try
            {
                IsFilePathReal = File.Exists(filePath);
            }
            catch
            {
            }

            if (IsFilePathReal == false)
                return null;

            Log4NetViewModel vm = new Log4NetViewModel();

            if (vm.OpenFile(filePath))
                return vm;

            return null;
        }

        /// <summary>
        /// Attempt to open a file and load it into the viewmodel if it exists.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>True if file exists and was succesfully loaded. Otherwise false.</returns>
        protected bool OpenFile(string filePath)
        {
            try
            {
                var isReal = File.Exists(filePath);

                if (isReal)
                {
                    mDocumentModel.SetFileNamePath(filePath, isReal);
                    FilePath = filePath;
                    ContentId = mFilePath;

                    // File may be blocked by another process
                    // Try read-only shared method and set file access to read-only
                    try
                    {
                        // XXX TODO Extend log4net FileOpen method to support base.FireFileProcessingResultEvent(...);
                        mYalvVM.LoadFile(filePath);
                    }
                    catch (Exception ex)
                    {
                        var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
                        msgBox.Show(ex.Message, Edi.Util.Local.Strings.STR_FILE_OPEN_ERROR_MSG_CAPTION, MsgBoxButtons.OK);

                        return false;
                    }
                }
                else
                    return false;
            }
            catch (Exception exp)
            {
                var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
                msgBox.Show(exp.Message, Edi.Util.Local.Strings.STR_FILE_OPEN_ERROR_MSG_CAPTION, MsgBoxButtons.OK);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Reloads/Refresh's the current document content with the content
        /// of the from disc.
        /// </summary>
        public override void ReOpen()
        {
            try
            {
                base.ReOpen();

                OpenFile(FilePath);
            }
            catch (Exception exp)
            {
                var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
                msgBox.Show(exp.Message, Edi.Util.Local.Strings.STR_FILE_OPEN_ERROR_MSG_CAPTION, MsgBoxButtons.OK);
            }
        }
        #endregion
    }
}
