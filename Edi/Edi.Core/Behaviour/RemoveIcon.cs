namespace Edi.Core.Behaviour
{
	using System.Windows;
	using System.Runtime.InteropServices;
	using System;
	using System.Windows.Interop;

	/// <summary>
	/// Remove window icon from window chrome/title bar
	/// </summary>
	public static class RemoveIcon
	{
		/// <summary>
		/// Dependency property for attached behaviour in NON-dialog windows.
		/// This can be is used to close a NON-dialog window via ViewModel.
		/// </summary>
		public static readonly DependencyProperty RemoveProperty =
				DependencyProperty.RegisterAttached(
						"Remove",
						typeof(bool?),
						typeof(RemoveIcon),
						new PropertyMetadata(RemoveChanged));

		/// <summary>
		/// Setter of corresponding dependency property
		/// </summary>
		/// <param name="target"></param>
		/// <param name="value"></param>
		public static void SetRemove(Window target, bool? value)
		{
			target.SetValue(RemoveProperty, value);
		}

		private static void RemoveChanged(DependencyObject d,
																						DependencyPropertyChangedEventArgs e)
		{
			var window = d as Window;

			if (e != null)                  // The bound value can be set to null in the ViewModel
			{                              // If a shutdown request was cancelled.
				if (e.NewValue == null)     // Do not react on this ([re-]initialization) event.
					return;
			}

			if (window != null)
			{
				try
				{
					window.SourceInitialized += new System.EventHandler(window_SourceInitialized);
				}
				catch
				{
				}
			}
		}

		private static void window_SourceInitialized(object sender, System.EventArgs e)
		{
			if (sender != null)
			{

                if (sender is Window)
                {
                    Window win = sender as Window;
                    IconHelper.RemoveIcon(win);
                }
            }
		}
	}

	/// <summary>
	/// Remove an icon from a window's title bar:
	/// Source: http://wpftutorial.net/RemoveIcon.html
	/// </summary>
	internal static class IconHelper
	{
		#region const
		private const int GwlEXSTYLE = -20;
		private const int WsExDlgModalFrame = 0x0001;
		private const int SwpNOSIZE = 0x0001;
		private const int SwpNOMOVE = 0x0002;
		private const int SwpNOZORDER = 0x0004;
		private const int SwpFRAMECHANGED = 0x0020;
		private const uint WmSETICON = 0x0080;
		#endregion const

		#region Methods
		/// <summary>
		/// Remove an icon from a window's title bar:
		/// Source: http://wpftutorial.net/RemoveIcon.html
		/// </summary>
		/// <param name="window"></param>
		public static void RemoveIcon(Window window)
		{
			// Get this window's handle
			IntPtr hwnd = new WindowInteropHelper(window).Handle;

			// Change the extended window style to not show a window icon
			int extendedStyle = GetWindowLong(hwnd, GwlEXSTYLE);
			SetWindowLong(hwnd, GwlEXSTYLE, extendedStyle | WsExDlgModalFrame);

			// Update the window's non-client area to reflect the changes
			SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SwpNOMOVE |
									 SwpNOSIZE | SwpNOZORDER | SwpFRAMECHANGED);
		}

		#region External Methods
		[DllImport("user32.dll")]
		private static extern int GetWindowLong(IntPtr hwnd, int index);

		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

		[DllImport("user32.dll")]
		private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter,
																						int x, int y, int width, int height, uint flags);

		[DllImport("user32.dll")]
		private static extern IntPtr SendMessage(IntPtr hwnd, uint msg,
																						 IntPtr wParam, IntPtr lParam);
		#endregion External Methods
		#endregion Methods
	}
}
