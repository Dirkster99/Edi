namespace Edi.Apps.ViewModels
{
	using System;
	using System.ComponentModel.Composition;
	using System.IO;
	using System.Text;
	using System.Threading.Tasks;
	using System.Windows.Input;
	using Edi.Core.Interfaces;
	using Edi.Core.Models.Enums;
	using Edi.Core.ViewModels;
	using Edi.Core.ViewModels.Command;
	using Edi.Apps.Events;
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
		private RelayCommand<object> mLoadLayoutCommand;
		private RelayCommand<object> mSaveLayoutCommand;

		private readonly Guid mLayoutID;
		private string mLayoutFileName;
		private string mAppDir;

		private readonly ISettingsManager mProgramSettings;
		private IMessageManager mMessageManager;
		#endregion fields

		#region constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		[ImportingConstructor]
		public AvalonDockLayoutViewModel(ISettingsManager programSettings,
																		 IMessageManager messageManager)
		{
			this.LayoutSoure = LayoutLoaded.FromDefault;

			this.mProgramSettings = programSettings;
			this.mMessageManager = messageManager;

			this.mAppDir = this.mProgramSettings.AppDir;
			this.mLayoutFileName = this.mProgramSettings.LayoutFileName;

			this.mLayoutID = Guid.NewGuid();
			this.ViewProperties = new AvalonDockViewProperties();
			this.ViewProperties.InitialzeInstance();
		}
		#endregion constructors

		#region properties
		/// <summary>
		/// Gets the layout id for the AvalonDock Layout that is associated with this viewmodel.
		/// This layout id is a form of identification between viewmodel and view to identify whether
		/// a given event aggregated message is for a given recipient or not.
		/// </summary>
		public Guid LayoutID
		{
			get
			{
				return this.mLayoutID;
			}
		}

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
				if (this.mLoadLayoutCommand == null)
				{
					this.mLoadLayoutCommand = new RelayCommand<object>((p) =>
					{
						DockingManager docManager = p as DockingManager;

						if (docManager == null)
							return;

						this.mMessageManager.Output.AppendLine("Loading document and tool window layout...");
						this.LoadDockingManagerLayout(docManager);
					});
				}

				return this.mLoadLayoutCommand;
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
				if (this.mSaveLayoutCommand == null)
				{
					this.mSaveLayoutCommand = new RelayCommand<object>((p) =>
					{
						string xmlLayout = p as string;

						if (xmlLayout == null)
							return;

						this.SaveDockingManagerLayout(xmlLayout);
					});
				}

				return this.mSaveLayoutCommand;
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
		/// <param name="docManager"></param>
		private void LoadDockingManagerLayout(DockingManager docManager)
		{
			this.LoadDockingManagerLayout(this.LayoutID);
		}

		/// <summary>
		/// Loads the layout of a particular docking manager instance from persistence
		/// and checks whether a file should really be reloaded (some files may no longer
		/// be available).
		/// </summary>
		private void LoadDockingManagerLayout(Guid layoutID)
		{
			try
			{
				string sTaskError = string.Empty;

				Task taskToProcess = null;
				taskToProcess = Task.Factory.StartNew<string>((stateObj) =>
				{
					// Begin Aysnc Task
					string layoutFileName = System.IO.Path.Combine(this.mAppDir, this.mLayoutFileName);
					string xmlWorkspaces = string.Empty;

					try
					{
						try
						{
							if (System.IO.File.Exists(layoutFileName) == true)
							{
								this.mMessageManager.Output.AppendLine(string.Format(" from file: '{0}'", layoutFileName));

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
						}

						if (string.IsNullOrEmpty(xmlWorkspaces) == false)
						{
							this.LayoutSoure = LayoutLoaded.FromStorage;
							LoadLayoutEvent.Instance.Publish(new LoadLayoutEventArgs(xmlWorkspaces, layoutID));
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
						this.mMessageManager.Output.AppendLine("Loading layout done.\n");
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
		/// <param name="filename"></param>
		/// <returns></returns>
		public string GetResourceTextFile(string assemblyNameSpace, string filename)
		{
			string result = string.Empty;
			this.mMessageManager.Output.AppendLine(string.Format("Layout from Resource '{0}', '{1}'", assemblyNameSpace, filename));

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

			string fileName = System.IO.Path.Combine(this.mAppDir, this.mLayoutFileName);

			File.WriteAllText(fileName, xmlLayout);
		}
		#endregion SaveLayout
		#endregion methods
	}
}
