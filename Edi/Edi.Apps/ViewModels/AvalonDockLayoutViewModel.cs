using Edi.Apps.Behaviors;

namespace Edi.Apps.ViewModels
{
	using System;
	using System.ComponentModel.Composition;
	using System.IO;
	using System.Text;
	using System.Threading.Tasks;
	using System.Windows.Input;
	using Core.Interfaces;
	using Core.Models.Enums;
	using Core.ViewModels;
	using Core.ViewModels.Command;
	using Events;
	using Settings.Interfaces;
	using Xceed.Wpf.AvalonDock;

	/// <summary>
	/// Class implements a viewmodel to support the
	/// <seealso cref="AvalonDockLayoutSerializer"/>
	/// attached behavior which is used to implement
	/// load/save of layout information on application
	/// start and shut-down.
	/// </summary>
	[Export(typeof(IAvalonDockLayoutViewModel))]
    public class AvalonDockLayoutViewModel : IAvalonDockLayoutViewModel
    {
        #region fields
        private RelayCommand<object> _mLoadLayoutCommand;
        private RelayCommand<object> _mSaveLayoutCommand;

	    private readonly string _mLayoutFileName;
        private readonly string _mAppDir;

	    private readonly IMessageManager _mMessageManager;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        [ImportingConstructor]
        public AvalonDockLayoutViewModel(ISettingsManager programSettings,
                                         IMessageManager messageManager)
        {
	        LayoutSoure = LayoutLoaded.FromDefault;

            var mProgramSettings = programSettings;
            _mMessageManager = messageManager;

            _mAppDir = mProgramSettings.AppDir;
            _mLayoutFileName = mProgramSettings.LayoutFileName;

            LayoutID = Guid.NewGuid();
            ViewProperties = new AvalonDockViewProperties();
            ViewProperties.InitialzeInstance();
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets the layout id for the AvalonDock Layout that is associated with this viewmodel.
        /// This layout id is a form of identification between viewmodel and view to identify whether
        /// a given event aggregated message is for a given recipient or not.
        /// </summary>
        public Guid LayoutID { get; }

	    public AvalonDockViewProperties ViewProperties { get; set; }

        public LayoutLoaded LayoutSoure { get; private set; }

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
	            return _mLoadLayoutCommand ?? (_mLoadLayoutCommand = new RelayCommand<object>((p) =>
	            {
		            try
		            {
			            if (!(p is DockingManager docManager))
				            return;

			            _mMessageManager.Output.AppendLine("Loading document and tool window layout...");
			            LoadDockingManagerLayout();
		            }
		            catch (Exception exp)
		            {
			            var wrt = _mMessageManager.Output.Writer;
			            wrt.WriteLine("Error when loading layout in AvalonDockLayoutViewModel:");
			            wrt.WriteLine(exp.Message);
		            }
	            }));
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
	            return _mSaveLayoutCommand ?? (_mSaveLayoutCommand = new RelayCommand<object>((p) =>
	            {
		            if (!(p is string xmlLayout))
			            return;

		            SaveDockingManagerLayout(xmlLayout);
	            }));
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
	    private void LoadDockingManagerLayout()
        {
            LoadDockingManagerLayout(LayoutID);
        }

        /// <summary>
        /// Loads the layout of a particular docking manager instance from persistence
        /// and checks whether a file should really be reloaded (some files may no longer
        /// be available).
        /// </summary>
        private void LoadDockingManagerLayout(Guid layoutId)
        {
            try
            {
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
				                _mMessageManager.Output.AppendLine($" from file: '{layoutFileName}'");

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
			                LayoutSoure = LayoutLoaded.FromStorage;
			                LoadLayoutEvent.Instance.Publish(new LoadLayoutEventArgs(xmlWorkspaces, layoutId));
		                }
	                }
	                catch (OperationCanceledException exp)
	                {
		                throw exp;
	                }
	                catch (Exception except)
	                {
		                throw except;
	                }
	                finally
	                {
		                _mMessageManager.Output.AppendLine("Loading layout done.\n");
	                }

	                return xmlWorkspaces;                     // End of async task

                }, null);
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

	    /// <summary>
	    /// Source: http://stackoverflow.com/questions/2820384/reading-embedded-xml-file-c-sharps
	    /// </summary>
	    /// <param name="assemblyNameSpace"></param>
	    /// <param name="filename"></param>
	    /// <returns></returns>
	    public string GetResourceTextFile(string assemblyNameSpace, string filename)
        {
            string result = string.Empty;
            _mMessageManager.Output.AppendLine($"Layout from Resource '{assemblyNameSpace}', '{filename}'");

            ////using (Stream stream = this.GetType().Assembly.
            ////			 GetManifestResourceStream(assemblyNameSpace + filename))
            ////{
            ////	using (StreamReader sr = new StreamReader(stream))
            ////	{
            ////		result = sr.ReadToEnd();
            ////	}
            ////}

            return result;
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
