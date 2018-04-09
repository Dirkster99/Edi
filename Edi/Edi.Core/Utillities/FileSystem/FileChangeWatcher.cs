namespace Edi.Core.Models.Utillities.FileSystem
{
    using System;
    using System.IO;
    using Edi.Core.Interfaces.Documents;

    // Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
    // 
    // Permission is hereby granted, free of charge, to any person obtaining a copy of this
    // software and associated documentation files (the "Software"), to deal in the Software
    // without restriction, including without limitation the rights to use, copy, modify, merge,
    // publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
    // to whom the Software is furnished to do so, subject to the following conditions:
    // 
    // The above copyright notice and this permission notice shall be included in all copies or
    // substantial portions of the Software.
    // 
    // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
    // INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
    // PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
    // FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
    // OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
    // DEALINGS IN THE SOFTWARE.
    /// <summary>
    /// Based on:
    /// https://github.com/icsharpcode/SharpDevelop/blob/master/src/Main/Base/Project/Workbench/FileChangeWatcher.cs
    /// </summary>
    public sealed class FileChangeWatcher : IDisposable
	{
		#region fields
		////static HashSet<FileChangeWatcher> activeWatchers = new HashSet<FileChangeWatcher>();
		////static int globalDisableCount;

		private FileSystemWatcher _mWatcher;
		private bool _mEnabled;

		private IDocumentModel _mFile;
		#endregion fields

		#region constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="file"></param>
		public FileChangeWatcher(IDocumentModel file)
		{
			_mFile = file ?? throw new ArgumentNullException(nameof(file));

			////SD.Workbench.MainWindow.Activated += MainForm_Activated;

			_mFile.FileNameChanged += file_FileNameChanged;
			////FileChangeWatcher.activeWatchers.Add(this);
			
			////Bugfix: Watching files by default can cause application file load deadlock situations(?)
			////this.SetWatcher();
		}
		#endregion constructors

		#region properties
		public static bool DetectExternalChangesOption
		{
			get => true;

			set
			{
				// Activate/deactivate file watchers when application setting is changed
				//// SD.MainThread.VerifyAccess();
				//// PropertyService.Set("SharpDevelop.FileChangeWatcher.DetectExternalChanges", value);
				//// foreach (FileChangeWatcher watcher in activeWatchers)
				//// {
				//// 	watcher.SetWatcher();
				//// }
			}
		}

		public bool Enabled
		{
			get => _mEnabled;
			set
			{
				_mEnabled = value;
				SetWatcher();
			}
		}

		////public static bool AllChangeWatchersDisabled
		////{
		////	get { return globalDisableCount > 0; }
		////}

		public bool WasChangedExternally { get; set; }

		#endregion properties

		#region methods

		////public static void DisableAllChangeWatchers()
		////{
		////	////SD.MainThread.VerifyAccess();
		////	globalDisableCount++;
		////	
		////	foreach (FileChangeWatcher w in activeWatchers)
		////		w.SetWatcher();
		////
		////	////Project.ProjectChangeWatcher.OnAllChangeWatchersDisabledChanged();
		////}

		////public static void EnableAllChangeWatchers()
		////{
		////	////SD.MainThread.VerifyAccess();
		////
		////	if (globalDisableCount == 0)
		////		throw new InvalidOperationException();
		////
		////	globalDisableCount--;
		////
		////	foreach (FileChangeWatcher w in activeWatchers)
		////		w.SetWatcher();
		////
		////	////Project.ProjectChangeWatcher.OnAllChangeWatchersDisabledChanged();
		////}

		public void Dispose()
		{
			////SD.MainThread.VerifyAccess();
			////activeWatchers.Remove(this);

			if (_mFile != null)
			{
				////SD.Workbench.MainWindow.Activated -= MainForm_Activated;
				_mFile.FileNameChanged -= file_FileNameChanged;
				_mFile = null;
			}

			if (_mWatcher != null)
			{
				_mWatcher.Changed -= OnFileChangedEvent;
				_mWatcher.Created -= OnFileChangedEvent;
				_mWatcher.Renamed -= OnFileChangedEvent;

				_mWatcher.Dispose();
				_mWatcher = null;
			}
		}

		private void SetWatcher()
		{
			////SD.MainThread.VerifyAccess();

			if (_mWatcher != null)
			{
				_mWatcher.EnableRaisingEvents = false;
			}

			if (_mEnabled == false)
				return;

			////if (globalDisableCount > 0)
			////	return;

			if (DetectExternalChangesOption == false)
				return;

			string fileName = _mFile.FileNamePath;
			if (string.IsNullOrEmpty(fileName))
				return;

			if (_mFile.IsReal == false)
				return;

			if (!Path.IsPathRooted(fileName))
				return;

			try
			{
				if (_mWatcher == null)
				{
					_mWatcher = new FileSystemWatcher();

					////watcher.SynchronizingObject = SD.MainThread.SynchronizingObject;

					_mWatcher.Changed += OnFileChangedEvent;
					_mWatcher.Created += OnFileChangedEvent;
					_mWatcher.Renamed += OnFileChangedEvent;
				}

				_mWatcher.Path = Path.GetDirectoryName(fileName);
				_mWatcher.Filter = Path.GetFileName(fileName);
				_mWatcher.EnableRaisingEvents = true;
			}
			catch (PlatformNotSupportedException)
			{
				if (_mWatcher != null)
					_mWatcher.Dispose();

				_mWatcher = null;
			}
			catch (FileNotFoundException)
			{
				// can occur if directory was deleted externally
				if (_mWatcher != null)
					_mWatcher.Dispose();

				_mWatcher = null;
			}
			catch (ArgumentException)
			{
				// can occur if parent directory was deleted externally
				if (_mWatcher != null)
					_mWatcher.Dispose();

				_mWatcher = null;
			}
		}

		private void file_FileNameChanged(object sender, EventArgs e)
		{
			SetWatcher();
		}
		
		/// <summary>
		/// Method executes if/when the sub-system registers a change for this file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnFileChangedEvent(object sender, FileSystemEventArgs e)
		{
			if (_mFile == null)
				return;

			////LoggingService.Debug("File " + file.FileName + " was changed externally: " + e.ChangeType);

			if (WasChangedExternally == false)
			{
				WasChangedExternally = true;

				//// if (SD.Workbench.IsActiveWindow)
				//// {
				//// 	// delay reloading message a bit, prevents showing two messages
				//// 	// when the file changes twice in quick succession; and prevents
				//// 	// trying to reload the file while it is still being written
				//// 	SD.MainThread.CallLater(
				//// 		TimeSpan.FromSeconds(0.5),
				//// 		delegate { MainForm_Activated(this, EventArgs.Empty); });
				//// }
			}
		}
		#endregion methods
	}
}