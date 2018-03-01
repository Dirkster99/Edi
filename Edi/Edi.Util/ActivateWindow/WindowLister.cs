namespace Edi.Util.ActivateWindow
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Runtime.InteropServices;

	/// <summary>
	/// This class manages basic information about a process running on the computer.
	/// 
	/// Source: http://www.xtremevbtalk.com/showthread.php?t=318187
	/// </summary>
	public class WindowInfo
	{
		public IntPtr Handle { get; set; }
		public string Title { get; set; }
		public string ProcessName { get; set; }
	}

	/// <summary>
	/// Source: http://www.xtremevbtalk.com/showthread.php?t=318187
	/// </summary>
	public class WindowLister
	{
		#region fields
		protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		#endregion fields

		#region methods
		/// <summary>
		/// Receive a list of named (or all) processs' running at present on this computer.
		/// </summary>
		/// <param name="processName">Name of process to list or null (returns all processs')</param>
		/// <returns></returns>
		public WindowInfo[] GetWindows(string processName = null)
		{
			List<WindowInfo> windows = new List<WindowInfo>();

		    var allProcesses = processName == null ? Process.GetProcesses() : Process.GetProcessesByName(processName);

			foreach (Process process in allProcesses)
			{
				if (process.MainWindowHandle != IntPtr.Zero)
				{
					windows.Add(new WindowInfo()
					{
						Handle = process.MainWindowHandle,
						ProcessName = process.ProcessName,
						Title = process.MainWindowTitle
					});
				}
			}

			return windows.ToArray();
		}

		/// <summary>
		/// Activate a window that belongs to a process with the given name.
		/// The Window is given input focus and displayed in the foreground
		/// (if it is not minimized).
		/// </summary>
		/// <param name="procName">Name of process who's Window is to be activated.</param>
		public static void ActivateMainWindow(string procName)
		{
			WindowLister l = new WindowLister();
			WindowInfo[] w = l.GetWindows(procName);

			if (w == null)
			{
				logger.Warn("--> Failed to activate Window (w is null).");
				return;
			}

			if (w.Length == 0)
			{
				logger.Warn("--> Failed to activate Window (w is zero).");
				return;
			}

			foreach (WindowInfo wi in w)
			{
				if (wi.ProcessName == procName)
				{
					bool success = NativeMethods.SetForegroundWindow(wi);

					if (success == false)
						logger.Warn("--> Failed to activate Window (success = false).");
				}
			}
		}
		#endregion methods
	}

	/// <summary>
	/// Wrapper class for the native user32.dll SetForegroundWindow method.
	/// </summary>
	public class NativeMethods
	{
		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow(IntPtr hWnd);

		/// <summary>
		/// Wrapper function for the SetForegroundWindow method in the User32.dll.
		/// </summary>
		/// <param name="window"></param>
		/// <returns></returns>
		public static bool SetForegroundWindow(WindowInfo window)
		{
			return SetForegroundWindow(window.Handle);
		}
	}
}
