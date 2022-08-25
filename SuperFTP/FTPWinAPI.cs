using System;
using System.Runtime.InteropServices;

namespace SuperFramework.SuperFTP
{
    /// <summary>
    /// 日期:2014-09-10
    /// 作者:不良帥
    /// 说明:FTP操作WinAPI
    /// </summary>
    public class FTPWinAPI
    {
        public const uint SHGFI_ICON = 0x100;
        /// <summary>
        /// 大图标
        /// </summary>
        public const uint SHGFI_LARGEICON = 0x0; // 'Large icon
        /// <summary>
        /// 小图标
        /// </summary>
        public const uint SHGFI_SMALLICON = 0x1; // 'Small icon
        public const uint SHGFI_USEFILEATTRIBUTES = 0x10;

        /// <summary>
        /// 获取一个手柄图标从指定的可执行文件，dll，或者图标文件。
        /// </summary>
        /// <param name="hInst">处理调用函数的应用程序的实例。</param>
        /// <param name="lpszExeFileName">指定一个可执行文件，该名字的指针，或者图标文件。</param>
        /// <param name="nIndex">指定要检索的图标的零基础索引。例如，如果这个值是0，该函数将返回一个句柄到指定文件中的第一个图标。
        ///如果这个值为1，该函数返回指定文件中的图标总数。如果文件是可执行文件或动态链接库，返回值是rt_group_icon资源数量。如果文件是一个。ico文件，返回值是1。
        ///如果这个值是一个负数不等于–1，该函数返回一个句柄指定文件中的标识符等于niconindex绝对价值的资源图标。例如，你应该使用3来提取资源标识符为3。提取图标标识符为1的资源，使用extracticonex功能。</param>
        /// <returns>返回值是一个图标的句柄。如果指定的文件不是一个可执行文件，dll，或者图标文件，返回的是1。如果在该文件中发现没有图标，返回值是空的。</returns>
        [DllImport("shell32.dll", EntryPoint = "ExtractIcon")]
        public static extern int ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIndex);
        /// <summary>
        /// 检索文件系统中的对象的信息，如文件、文件夹、目录或驱动器根目录。
        /// </summary>
        /// <param name="pszPath">绝对或相对路径</param>
        /// <param name="dwFileAttribute">一个或多个文件属性标志</param>
        /// <param name="psfi">shfileinfo结构指针接收文件信息</param>
        /// <param name="cbSizeFileInfo">shfileinfo参数指向的结构的字节大小</param>
        /// <param name="uFlags">指定文件信息检索的标志</param>
        /// <returns>返回一个值，它的意义取决于uFlags参数。</returns>
        [DllImport("shell32.dll", EntryPoint = "SHGetFileInfo")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttribute, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="hIcon">需要处理掉的图标指针</param>
        /// <returns>非零表示成功。零指示故障。为了获得更多的错误信息，调用GetLastError。</returns>
        /// <remarks>图标句柄无效后调用</remarks>
        [DllImport("User32.dll", EntryPoint = "DestroyIcon")]
        public static extern int DestroyIcon(IntPtr hIcon);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpszFile">指定要提取图标的可执行文件或dll文件的名称</param>
        /// <param name="nIconIndex">指定用于提取的第一个图标的零基础索引。例如，如果这个值为零，则该函数提取指定文件中的第一个图标。</param>
        /// <param name="phiconLarge">指向一个数组的指针来接收从文件中提取的大图标的句柄。如果这个参数是空的，则没有大的图标是从文件中提取的。</param>
        /// <param name="phiconSmall">指向一个数组的指针来接收从文件中提取的小图标的句柄。如果这个参数是空的，则没有小的图标是从文件中提取的。</param>
        /// <param name="nIcons">指定从文件中提取的图标的数目。Windows CE 2.10和后来的nicons，参数必须是1。</param>
        /// <returns>对于窗口2.10和后，这个函数返回到第一个图标在检索图标阵列的句柄。如果phiconlarge和phiconsmall不空，返回值默认为第一大图标。
        ///    Windows CE 1到2.10，这个函数返回一个数据类型。如果niconindex–参数是1，这phiconlarge参数为空，和phiconsmall参数为空，返回值为指定文件中包含多个图标。否则，返回值是成功从文件中提取的图标的数目。
        /// </returns>
        [DllImport("shell32.dll")]
        public static extern uint ExtractIconEx(string lpszFile, int nIconIndex, int[] phiconLarge, int[] phiconSmall, uint nIcons);

        #region  文件信息结构 
        /// <summary>
        /// 文件信息结构
        /// </summary>
        public struct FileStruct
        {
            public string Flags;
            public string Owner;
            public string Group;
            /// <summary>
            /// 是否文件夹
            /// </summary>
            public bool IsDirectory;
            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime CreateTime;
            /// <summary>
            /// 名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 大小-字节
            /// </summary>
            public long FileSize;
        }
        #endregion

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }
    }
}
