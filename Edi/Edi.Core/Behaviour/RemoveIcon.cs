using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Edi.Core.Behaviour
{
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

			
			// If a shutdown request was cancelled.
			if (e.NewValue == null)     // Do not react on this ([re-]initialization) event.
				return;

			if (window != null)
			{
				try
				{
					window.SourceInitialized += window_SourceInitialized;
				}
				catch
				{
					// ignored
				}
			}
		}

		private static void window_SourceInitialized(object sender, EventArgs e)
		{
			if (sender is Window win)
			{
				IconHelper.RemoveIcon(win);
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
		private const int GwlExstyle = -20;
		private const int WsExDlgModalFrame = 0x0001;
		private const int SwpNosize = 0x0001;
		private const int SwpNomove = 0x0002;
		private const int SwpNozorder = 0x0004;
		private const int SwpFramechanged = 0x0020;
		private const uint WmSeticon = 0x0080;
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
			int extendedStyle = GetWindowLong(hwnd, GwlExstyle);
			SetWindowLong(hwnd, GwlExstyle, extendedStyle | WsExDlgModalFrame);

			// Update the window's non-client area to reflect the changes
			SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SwpNomove |
									 SwpNosize | SwpNozorder | SwpFramechanged);
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