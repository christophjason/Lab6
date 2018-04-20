using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace Lab6_1820151020
{
    public enum IconSize
    {
        Small,
        Large
    }
    class IconImport
    {
        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_LARGEICON = 0x0;
        private const uint SHGFI_SMALLICON = 0x1;

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, 
            uint cbSizeFileInfo, uint uFlags);

        public static System.Drawing.Icon Import(string File, IconSize Size)
        {
            IntPtr hIcon;
            SHFILEINFO shinfo = new SHFILEINFO();

            if (Size == IconSize.Large)
            {
                hIcon = SHGetFileInfo(File, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_LARGEICON);
            }
            else
            {
                hIcon = SHGetFileInfo(File, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_SMALLICON);
            }
            return System.Drawing.Icon.FromHandle(shinfo.hIcon);
        }
        public static System.Drawing.Icon Import(string File)
        {
            return Import(File, IconSize.Small);
        }
    }
}
