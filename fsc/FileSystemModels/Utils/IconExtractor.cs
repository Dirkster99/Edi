namespace FileSystemModels.Utils
{
  using System;
  using System.Drawing;
  using System.Runtime.InteropServices;

  /// <summary>
  /// Helper class to retrieve icons for file of folder
  /// representation from Windows file system sub-system.
  /// </summary>
  public class IconExtractor
  {
    #region fields
    private const uint SHGFI_ICON = 0x100;
    private const uint SHGFI_LARGEICON = 0x0;
    private const uint SHGFI_SMALLICON = 0x1;
    private const uint SHGFI_OPENICON = 0x2;
    #endregion fields

    #region methods
    /// <summary>
    /// Gets an icon that is extracted right out of the given file.
    /// </summary>
    /// <param name="cFile"></param>
    public static Icon GetFileIcon(string cFile)
    {
      // return Icon.ExtractAssociatedIcon(cFile).;
      SHFILEINFO shi = new SHFILEINFO();
      IntPtr hIcon = SHGetFileInfo(cFile, 0, ref shi, (uint)(Marshal.SizeOf(shi)), SHGFI_SMALLICON | SHGFI_ICON);

      if (shi.HIcon != IntPtr.Zero)
      {
        Icon ret = (Icon)Icon.FromHandle(shi.HIcon).Clone();
        DestroyIcon(shi.HIcon);
        return ret;
      }
      else
        return null;
    }

    /// <summary>
    /// Gets an icon that represents the corresponding folder.
    /// </summary>
    /// <param name="cFolder"></param>
    /// <param name="bOpen"></param>
    public static Icon GetFolderIcon(string cFolder, bool bOpen = false)
    {
      SHFILEINFO shi = new SHFILEINFO();
      uint uFlags = SHGFI_SMALLICON | SHGFI_ICON;

      if (bOpen == true)
        uFlags |= SHGFI_OPENICON;

      IntPtr hIcon = SHGetFileInfo(cFolder, 0, ref shi, (uint)Marshal.SizeOf(shi), uFlags);

      if (shi.HIcon != IntPtr.Zero)
      {
        Icon ret = (Icon)Icon.FromHandle(shi.HIcon).Clone();
        bool l = DestroyIcon(shi.HIcon);
        return ret;
      }
      else
        return null;
    }

    [DllImport("shell32.dll")]
    private static extern IntPtr SHGetFileInfo(string pszPath,
                                uint dwFileAttributes,
                                ref SHFILEINFO psfi,
                                uint cbSizeFileInfo,
                                uint uFlags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyIcon(IntPtr hIcon);
    #endregion methods

    [StructLayout(LayoutKind.Sequential)]
    private struct SHFILEINFO
    {
      public IntPtr HIcon;
      public IntPtr IIcon;
      public uint DwAttributes;

      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
      public string SzDisplayName;

      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
      public string SzTypeName;
    }
  }
}
