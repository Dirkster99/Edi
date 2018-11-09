namespace MiniUML.Model.ViewModels.Document
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;
    using MiniUML.Framework;
    using MiniUML.Model.Model;
    using MiniUML.Model.ViewModels.Shapes;
    using MsgBox;

    public partial class DocumentViewModel : AbstractDocumentViewModel
    {
        #region fields
        private string _FileName = MiniUML.Framework.Local.Strings.STR_Default_FileName;
        private string _FilePath = null;
        private FrameworkElement _DesignSurface;
        private CanvasViewModel _CanvasViewModel = null;
        private CommandUtility _CommandUtility;

        private readonly DocumentDataModel _DataModel;
        private readonly IMessageBoxService _MsgBox;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public DocumentViewModel(string pluginModelName, IMessageBoxService msgBox)
        {
            _MsgBox = msgBox;

            // Create and initialize the data model.
            _DataModel = new DocumentDataModel(pluginModelName);

            // TODO XXX _dataModel.New((Size)SettingsManager.Settings["DefaultPageSize"], (Thickness)SettingsManager.Settings["DefaultPageMargins"]);
            _DataModel.New(new PageViewModelBase());

            _DataModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                // Suggest to WPF to refresh commands when the DocumentDataModel changes state.
                if (e.PropertyName == "State")
                    CommandManager.InvalidateRequerySuggested();
            };

            // Create the commands in this view model.
            _CommandUtility = new CommandUtility(this, msgBox);

            // Create the view models.
            _CanvasViewModel = new CanvasViewModel(this, msgBox);
            this.vm_XmlViewModel = new XmlViewModel(this);
        }
        #endregion constructor

        #region properties
        public string prop_DocumentFilePath
        {
            get
            {
                return _FilePath;
            }

            private set
            {
                _FilePath = value;

                // If the current document is associated with a file, show the file name as part of the window title.
                if (!string.IsNullOrEmpty(_FilePath))
                    _FileName = System.IO.Path.GetFileName(_FilePath);
                else
                    _FileName = MiniUML.Framework.Local.Strings.STR_Default_FileName;

                this.NotifyPropertyChanged(() => this.prop_DocumentFilePath);
                this.NotifyPropertyChanged(() => this.prop_DocumentFileName);
            }
        }

        public string prop_DocumentFileName
        {
            get { return _FileName; }
        }

        public override FrameworkElement v_CanvasView
        {
            get
            {
                return _DesignSurface;
            }

            set
            {
                _DesignSurface = value;
                this.NotifyPropertyChanged(() => this.v_CanvasView);
            }
        }

        #region Data models
        public override DocumentDataModel dm_DocumentDataModel
        {
            get { return _DataModel; }
        }
        #endregion

        #region View models
        public override CanvasViewModel vm_CanvasViewModel
        {
            get
            {
                return _CanvasViewModel;
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
        /// Get region of visible content
        /// </summary>
        /// <param name="margin"></param>
        /// <returns></returns>
        public Rect GetMaxBounds(Rect margin = default(Rect))
        {
            return _DataModel.GetMaxBounds(margin);
        }

        /// <summary>
        /// Process the event in which the user drags the grid splitter between
        /// canvas view (that displays the actual shapes) and XML editor.
        /// </summary>
        /// <param name="verticalChange"></param>
        /// <param name="canvasActualHeight"></param>
        public void GridSplitter_DragDelta(double verticalChange, double canvasActualHeight)
        {
            if (this.vm_XmlViewModel != null)
                this.vm_XmlViewModel.GridSplitter_DragDelta(verticalChange, canvasActualHeight);
        }

        /// <summary>
        /// Queries the user to save unsaved changes
        /// </summary>
        /// <returns>False on cancel, otherwise true.</returns>
        public bool QuerySaveChanges()
        {
            if (this.dm_DocumentDataModel.HasUnsavedData == false)
                return true;

            MessageBoxResult result = MessageBox.Show(string.Format(MiniUML.Framework.Local.Strings.STR_QUERY_SAVE_CHANGES, this.prop_DocumentFileName),
                                                      MiniUML.Framework.Local.Strings.STR_QUERY_SAVE_CHANGES_CAPTION,
                                                      MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    return ((SaveCommandModel)this.cmd_Save).Execute();

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
            string plugin = this.dm_DocumentDataModel.PluginModelName;
            PluginModelBase m = PluginManager.GetPluginModel(plugin);

            // Look-up shape converter
            UmlTypeToStringConverterBase conv = null;
            conv = m.ShapeConverter;

            // Convert Xml document into a list of shapes and page definition
            List<ShapeViewModelBase> coll;
            PageViewModelBase page = conv.LoadDocument(filename, _CanvasViewModel, out coll);

            // Apply new page and shape definitions to data model
            _DataModel.LoadFileFromCollection(page, coll);

            this.prop_DocumentFilePath = filename;
            this.vm_CanvasViewModel.SelectedItem.Clear();
        }
        #region Command implementations

        /// <summary>
        /// Save a document into the file system.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public override bool ExecuteSave(string filePath)
        {
            this.prop_DocumentFilePath = filePath;

            try
            {
                // Save document to the existing file.
                _DataModel.Save(filePath);
                return true;
            }
            catch (Exception ex)
            {
                _MsgBox.Show(ex, string.Format(MiniUML.Framework.Local.Strings.STR_SaveFILE_MSG, filePath),
                            MiniUML.Framework.Local.Strings.STR_SaveFILE_MSG_CAPTION);
            }

            return false;
        }

        /// <summary>
        /// Save an exported image document (eg: png) into the file system.
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
