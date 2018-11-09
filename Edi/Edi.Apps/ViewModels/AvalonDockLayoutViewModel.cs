namespace Edi.Apps.ViewModels
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Core.Interfaces;
    using Core.Models.Enums;
    using Core.ViewModels;
    using Core.ViewModels.Command;
    using Edi.Interfaces.Events;
    using Edi.Interfaces.MessageManager;
    using Settings.Interfaces;
    using Xceed.Wpf.AvalonDock;

    /// <summary>
    /// Class implements a viewmodel to support the
    /// <seealso cref="AvalonDockLayoutSerializer"/>
    /// attached behavior which is used to implement
    /// load/save of layout information on application
    /// start and shut-down.
    /// 
    /// Installs in Edi's main project installer
    /// </summary>
    public class AvalonDockLayoutViewModel : IAvalonDockLayoutViewModel
    {
        #region fields
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ICommand _LoadLayoutCommand;
        private ICommand _mSaveLayoutCommand;

	    private readonly string _mLayoutFileName;
        private readonly string _mAppDir;
        private readonly IToolWindowRegistry _ToolWindowRegistry;
        private readonly IMessageManager _MessageManager;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public AvalonDockLayoutViewModel(ISettingsManager programSettings,
                                         IMessageManager messageManager,
                                         IToolWindowRegistry toolWindowRegistry)
            : this()
        {
            _ToolWindowRegistry = toolWindowRegistry;
            _MessageManager = messageManager;

            _mAppDir = programSettings.AppDir;
            _mLayoutFileName = programSettings.LayoutFileName;

            LayoutId = Guid.NewGuid();
            ViewProperties.InitialzeInstance();
        }

        /// <summary>
        /// Hidden standard constructor
        /// </summary>
        protected AvalonDockLayoutViewModel()
        {
            LayoutSource = LayoutLoaded.FromDefault;
            ViewProperties = new AvalonDockViewProperties();
        }
        #endregion constructors

        /// <summary>
        /// Defines an event that is send from the viewmodel to the view to have the
        /// view load a new layout when it is available on start-up.
        /// </summary>
        public event EventHandler<LoadLayoutEventArgs> LoadLayout;

        #region properties
        /// <summary>
        /// Gets the layout id for the AvalonDock Layout that is associated with this viewmodel.
        /// This layout id is a form of identification between viewmodel and view to identify whether
        /// a given event aggregated message is for a given recipient or not.
        /// </summary>
        public Guid LayoutId { get; }

        /// <summary>
        /// Gets properties that are relevant to viewing, styling and templating
        /// of document and tool winodw items managed by AvalonDock.
        /// </summary>
        public AvalonDockViewProperties ViewProperties { get; }

        /// <summary>
        /// Determines the source of the layout (e.g. from storage or from default).
        /// </summary>
        public LayoutLoaded LayoutSource { get; private set; }

        #region command properties
        /// <summary>
        /// Implement a command to load the layout of an AvalonDock-DockingManager instance.
        /// This layout defines the position and shape of each document and tool window
        /// displayed in the application.
        /// 
        /// Parameter:
        /// The command expects a reference to a <seealso cref="DockingManager"/> instance to
        /// work correctly. Not supplying that reference results in not loading a layout (silent return).
        /// </summary>
        public ICommand LoadLayoutCommand
        {
            get
            {
                if (_LoadLayoutCommand == null)
                {
                    _LoadLayoutCommand = new RelayCommand<object>((p) =>
                    {
                        try
                        {
                            DockingManager docManager = p as DockingManager;

                            if (docManager == null)
                                return;

                            _MessageManager.Output.AppendLine("Loading document and tool window layout...");
                            LoadDockingManagerLayout(LayoutId);
                        }
                        catch (Exception exp)
                        {
                            Logger.Error("Error when loading layout", exp);
                        }
                    });
                }

                return _LoadLayoutCommand;
            }
        }

        /// <summary>
        /// Implements a command to save the layout of an AvalonDock-DockingManager instance.
        /// This command can be executed in an OnClosed Window event.
        /// 
        /// This AvalonDock layout defines the position and shape of each document and tool window
        /// displayed in the application.
        /// 
        /// Parameter:
        /// The command expects a reference to a <seealso cref="string"/> instance to
        /// work correctly. The string is supposed to contain the XML layout persisted
        /// from the DockingManager instance. Not supplying that reference to the string
        /// results in not saving a layout (silent return).
        /// </summary>
        public ICommand SaveLayoutCommand
        {
            get
            {
                if (_mSaveLayoutCommand == null)
                {
                    _mSaveLayoutCommand = new RelayCommand<object>((p) =>
                    {
                        string xmlLayout = p as string;

                        if (xmlLayout == null)
                            return;

                        this.SaveDockingManagerLayout(xmlLayout);
                    });
                }

                return _mSaveLayoutCommand;
            }
        }
        #endregion command properties
        #endregion properties

        #region methods
        #region LoadLayout
        /// <summary>
        /// Loads the layout of a particular docking manager instance from persistence
        /// and checks whether a file should really be reloaded (some files may no longer
        /// be available).
        /// </summary>
        private void LoadDockingManagerLayout(Guid layoutId)
        {
            try
            {
                _ToolWindowRegistry.PublishTools();

                Task.Factory.StartNew((stateObj) =>
                {
                    // Begin Aysnc Task
                    string layoutFileName = Path.Combine(_mAppDir, _mLayoutFileName);
	                string xmlWorkspaces = string.Empty;

	                try
	                {
		                try
		                {
			                if (File.Exists(layoutFileName))
			                {
				                _MessageManager.Output.AppendLine($" from file: '{layoutFileName}'");

				                using (FileStream fs = new FileStream(layoutFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
				                {
					                using (StreamReader reader = ICSharpCode.AvalonEdit.Utils.FileReader.OpenStream(fs, Encoding.Default))
					                {
						                xmlWorkspaces = reader.ReadToEnd();
					                }
				                }
			                }
		                }
		                catch
		                {
			                // ignored
		                }

		                if (string.IsNullOrEmpty(xmlWorkspaces) == false)
		                {
			                LayoutSource = LayoutLoaded.FromStorage;

                            LoadLayout?.Invoke(this, new LoadLayoutEventArgs(xmlWorkspaces, layoutId));
		                }
	                }
	                catch (OperationCanceledException exp)
	                {
                        _MessageManager._MsgBox.Show(exp);
                    }
                    catch (Exception exp)
	                {
                        _MessageManager._MsgBox.Show(exp);
	                }
	                finally
	                {
		                _MessageManager.Output.AppendLine("Loading layout done.\n");
	                }

	                return xmlWorkspaces;                     // End of async task

                }, null);
            }
            catch (Exception exp)
            {
                _MessageManager._MsgBox.Show(exp);
            }
        }
        #endregion LoadLayout

        #region SaveLayout
        private void SaveDockingManagerLayout(string xmlLayout)
        {
            // Create XML Layout file on close application (for re-load on application re-start)
            if (xmlLayout == null)
                return;

            string fileName = Path.Combine(_mAppDir, _mLayoutFileName);

            File.WriteAllText(fileName, xmlLayout);
        }
        #endregion SaveLayout
        #endregion methods
    }
}
