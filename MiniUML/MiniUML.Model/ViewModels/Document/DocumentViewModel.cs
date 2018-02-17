namespace MiniUML.Model.ViewModels.Document
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;
    using Framework;
    using Model;
    using Shapes;

    public partial class DocumentViewModel : AbstractDocumentViewModel
    {
        #region fields
        private readonly DocumentDataModel mDataModel;

        private string mFileName = Framework.Local.Strings.STR_Default_FileName;
        private string mFilePath = null;
        private FrameworkElement mDesignSurface;
        private CanvasViewModel mCanvasViewModel = null;
        private CommandUtility mCommandUtility;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public DocumentViewModel(string pluginModelName)
        {
            // Create and initialize the data model.
            mDataModel = new DocumentDataModel(pluginModelName);

            // TODO XXX _dataModel.New((Size)SettingsManager.Settings["DefaultPageSize"], (Thickness)SettingsManager.Settings["DefaultPageMargins"]);
            mDataModel.New(new PageViewModelBase());

            mDataModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                // Suggest to WPF to refresh commands when the DocumentDataModel changes state.
                if (e.PropertyName == "State")
                    CommandManager.InvalidateRequerySuggested();
            };

            // Create the commands in this view model.
            mCommandUtility = new CommandUtility(this);

            // Create the view models.
            mCanvasViewModel = new CanvasViewModel(this);
            vm_XmlViewModel = new XmlViewModel(this);
        }
        #endregion constructor

        #region properties
        public string prop_DocumentFilePath
        {
            get
            {
                return mFilePath;
            }

            private set
            {
                mFilePath = value;

                // If the current document is associated with a file, show the file name as part of the window title.
                if (!string.IsNullOrEmpty(mFilePath))
                    mFileName = System.IO.Path.GetFileName(mFilePath);
                else
                    mFileName = Framework.Local.Strings.STR_Default_FileName;

                NotifyPropertyChanged(() => prop_DocumentFilePath);
                NotifyPropertyChanged(() => prop_DocumentFileName);
            }
        }

        public string prop_DocumentFileName
        {
            get { return mFileName; }
        }

        public override FrameworkElement v_CanvasView
        {
            get
            {
                return mDesignSurface;
            }

            set
            {
                mDesignSurface = value;
                NotifyPropertyChanged(() => v_CanvasView);
            }
        }

        #region Data models
        public override DocumentDataModel dm_DocumentDataModel
        {
            get { return mDataModel; }
        }
        #endregion

        #region View models
        public override CanvasViewModel vm_CanvasViewModel
        {
            get
            {
                return mCanvasViewModel;
            }
        }

        public XmlViewModel vm_XmlViewModel { get; private set; }
        #endregion

        #region Commands
        // Command properties
        public CommandModel cmd_New { get; private set; }

        public CommandModel cmd_Open { get; private set; }

        public CommandModel cmd_Save { get; private set; }

        public CommandModel cmd_SaveAs { get; private set; }

        public CommandModel cmd_Export { get; private set; }

        public CommandModel cmd_Print { get; private set; }

        public CommandModel cmd_Undo { get; private set; }

        public CommandModel cmd_Redo { get; private set; }
        #endregion
        #endregion properties

        #region methods
        /// <summary>
        /// Process the event in which the user drags the grid splitter between
        /// canvas view (that displays the actual shapes) and XML editor.
        /// </summary>
        /// <param name="verticalChange"></param>
        /// <param name="canvasActualHeight"></param>
        public void GridSplitter_DragDelta(double verticalChange, double canvasActualHeight)
        {
            if (vm_XmlViewModel != null)
                vm_XmlViewModel.GridSplitter_DragDelta(verticalChange, canvasActualHeight);
        }

        /// <summary>
        /// Queries the user to save unsaved changes
        /// </summary>
        /// <returns>False on cancel, otherwise true.</returns>
        public bool QuerySaveChanges()
        {
            if (dm_DocumentDataModel.HasUnsavedData == false)
                return true;

            MessageBoxResult result = MessageBox.Show(string.Format(Framework.Local.Strings.STR_QUERY_SAVE_CHANGES, prop_DocumentFileName),
                                                                                                Framework.Local.Strings.STR_QUERY_SAVE_CHANGES_CAPTION,
                                                                                                MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    return ((SaveCommandModel)cmd_Save).Execute();

                case MessageBoxResult.No:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Load the contents of a file from the windows file system into memory and display it.
        /// </summary>
        /// <param name="filename"></param>
        public override void LoadFile(string filename)
        {
            // Look-up plugin model
            string plugin = dm_DocumentDataModel.PluginModelName;
            PluginModelBase m = PluginManager.GetPluginModel(plugin);

            // Look-up shape converter
            UmlTypeToStringConverterBase conv = null;
            conv = m.ShapeConverter;

            // Convert Xml document into a list of shapes and page definition
            List<ShapeViewModelBase> coll;
            PageViewModelBase page = conv.LoadDocument(filename, mCanvasViewModel, out coll);

            // Apply new page and shape definitions to data model
            mDataModel.LoadFileFromCollection(page, coll);

            prop_DocumentFilePath = filename;
            vm_CanvasViewModel.SelectedItem.Clear();
        }

        #region Command implementations

        /// <summary>
        /// Save a document into the file system.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public override bool ExecuteSave(string filePath)
        {
            prop_DocumentFilePath = filePath;

            try
            {
                // Save document to the existing file.
                mDataModel.Save(filePath);
                return true;
            }
            catch (Exception ex)
            {
                var msgBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
                msgBox.Show(ex, string.Format(Framework.Local.Strings.STR_SaveFILE_MSG, filePath),
                            Framework.Local.Strings.STR_SaveFILE_MSG_CAPTION);
            }

            return false;
        }

        /// <summary>
        /// Save a document into the file system.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public override bool ExecuteExport(object sender, ExecutedRoutedEventArgs e, string defaultFileName)
        {
            ExportCommandModel.ExportUMLToImage(this, defaultFileName);

            return true;
        }
        #endregion

        #endregion methods
    }
}
