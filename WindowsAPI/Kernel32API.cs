using System;
using System.Runtime.InteropServices;
using System.Text;
using static SuperFramework.WindowsAPI.APIStruct;

namespace SuperFramework.WindowsAPI
{
    /// <summary>
    /// <para>作者：不良帥</para>
    /// <para>日期：2016-08-19</para>
    /// <para>说明：Windows9x/Me中非常重要的32位动态链接库文件，属于内核级文件。它控制着系统的内存管理、数据的输入输出操作和中断处理，当Windows启动时，kernel32.dll就驻留在内存中特定的写保护区域，使别的程序无法占用这个内存区域。</para>
    /// </summary>
    public static class Kernel32API
    {
        #region 检测文件是否被占用

        /// <summary>
        /// 以二进制模式打开指定的文件
        /// </summary>
        /// <param name="lpPathName">欲打开文件的路径 </param>
        /// <param name="iReadWrite">访问模式和共享模式常数的一个组合</param>
        /// <returns>如执行成功，返回打开文件的句柄。HFILE_ERROR表示出错。会设置GetLastError </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr _lopen(string lpPathName, int iReadWrite);

        #endregion

        /// <summary>
        /// 关闭一个指定的指针对象指向的设备。。
        /// </summary>
        /// <param name="hObject">要关闭的句柄 <see cref="IntPtr"/> 对象。</param>
        /// <returns>成功返回 <b>0</b> ，不成功返回非零值。</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CloseHandle(IntPtr hObject);

        /// <summary>
        /// 获取计算机名称
        /// </summary>
        /// <param name="IpBuffer"></param>
        /// <param name="nSize"></param>
        /// <returns></returns>
        [DllImport("kernel32", EntryPoint = "GetComputerName", ExactSpelling = false, SetLastError = true)]//计算机名称
        public static extern bool GetComputerName([MarshalAs(UnmanagedType.LPArray)]byte[] IpBuffer, [MarshalAs(UnmanagedType.LPArray)] int[] nSize);
        /// <summary>
        /// 获取Windows目录的完整路径名
        /// </summary>
        /// <param name="WinDir">指定一个字串缓冲区，用于装载Windows目录名。除非是根目录，否则目录中不会有一个中止用的“\”字符</param>
        /// <param name="count">lpBuffer字串的最大长度</param>
        [DllImport("kernel32")]
        public static extern void GetWindowsDirectory(StringBuilder WinDir, int count);
        /// <summary>
        /// 取得Windows系统目录（System目录）的完整路径名
        /// </summary>
        /// <param name="SysDir">用于装载系统目录路径名的一个字串缓冲区。它应事先初始化成“路径”字符串的长度+1。通常至少要为这个缓冲区分配MAX_PATH个字符的长度</param>
        /// <param name="count">lpBuffer字串的最大长度</param>
        [DllImport("kernel32")]
        public static extern void GetSystemDirectory(StringBuilder SysDir, int count);
        /// <summary>
        /// 查询当前系统的信息
        /// </summary>
        /// <param name="cpuinfo">SYSTEM_INFO结构体</param>
        [DllImport("kernel32")]
        public static extern void GetSystemInfo(ref CPU_INFO cpuinfo);
        /// <summary>
        /// 获得当前可用的物理和虚拟内存信息
        /// </summary>
        /// <param name="meminfo">MEMORYSTATUS的结构的指针。函数的返回信息会被存储在MEMORYSTATUS结构中。</param>
        [DllImport("kernel32")]
        public static extern void GlobalMemoryStatus(ref MEMORY_INFO meminfo);
        /// <summary>
        /// 获取当前系统时间，这个时间采用的是“协同世界时间”（即UTC，也叫做GMT）格式。
        /// </summary>
        /// <param name="stinfo"></param>
        [DllImport("kernel32")]
        public static extern void GetSystemTime(ref SYSTEMTIME_INFO stinfo);
        /// <summary>
        /// LoadLibrary函数将指定的可执行模块映射到调用进程的地址空间
        /// </summary>
        /// <param name="lpLibFileName">可执行模块(dll or exe)的名字：以null结束的字符串指针. 
        /// 该名称是可执行模块的文件名，与模块本身存储的,用关键字LIBRARY在模块定义文件(.def)中指定的名称无关,</param>
        /// <returns>如果执行成功, 返回模块的句柄如果执行失败, 返回 NULL. 如果要获取更多的错误信息, 请调用Marshal.GetLastWin32Error.</returns>
        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryA", CharSet = CharSet.Ansi)]
        public static extern IntPtr LoadLibrary(string lpLibFileName);
        /// <summary>
        /// FreeLibrary函数将装载的dll引用计数器减一，当引用计数器的值为0后，模块将从调用进程的地址空间退出，模块的句柄将不可再用
        /// </summary>
        /// <param name="hLibModule">dll模块的句柄. LoadLibrary 或者 GetModuleHandle 函数返回该句柄</param>
        /// <returns>如果执行成功, 返回值为非0,如果失败，返回值为0. 如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error.</returns>
        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary", CharSet = CharSet.Ansi)]
        public static extern int FreeLibrary(IntPtr hLibModule);
        /// <summary>
        /// GetProcAddress 函数获取外部函数的入口地址，或者从指定的DLL获取变量信息
        /// </summary>
        /// <param name="hModule">Dll的句柄，包含了函数或者变量，LoadLibrary 或 GetModuleHandle 函数返回该句柄 </param>
        /// <param name="lpProcName">以null结束的字符串指针，包含函数或者变量名，或者函数的顺序值，如果该参数是一个顺序值，
        /// 它必须是低序字be in the low-order word,高序字(the high-order)必须为0</param>
        /// <returns>如果执行成功, 返回值为外部函数或变量的地址如果执行失败，返回值为NULL. 
        /// 如果要获取更多错误信息，请调用Marshal.GetLastWin32Error.</returns>
        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", CharSet = CharSet.Ansi)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
        /// <summary>
        /// 执行打开/建立资源的功能。
        /// </summary>
        /// <param name="lpFileName">指定要打开的设备或文件的名称。</param>
        /// <param name="dwDesiredAccess">
        /// <para>Win32 常量，用于控制对设备的读访问、写访问或读/写访问的常数。内容如下表：
        /// <p><list type="table">
        /// <listheader>
        /// <term>名称</term>
        /// <description>说明</description>
        /// </listheader>
        /// <item>
        /// <term>GENERIC_READ</term><description>指定对设备进行读取访问。</description>
        /// </item>
        /// <item>
        /// <term>GENERIC_WRITE</term><description>指定对设备进行写访问。</description>
        /// </item>
        /// <item><term><b>0</b></term><description>如果值为零，则表示只允许获取与一个设备有关的信息。</description></item>
        /// </list></p>
        /// </para>
        /// </param>
        /// <param name="dwShareMode">指定打开设备时的文件共享模式</param>
        /// <param name="lpSecurityAttributes"></param>
        /// <param name="dwCreationDisposition">Win32 常量，指定操作系统打开文件的方式。内容如下表：
        /// <para><p>
        /// <list type="table">
        /// <listheader><term>名称</term><description>说明</description></listheader>
        /// <item>
        /// <term>CREATE_NEW</term>
        /// <description>指定操作系统应创建新文件。如果文件存在，则抛出 <see cref="IOException"/> 异常。</description>
        /// </item>
        /// <item><term>CREATE_ALWAYS</term><description>指定操作系统应创建新文件。如果文件已存在，它将被改写。</description></item>
        /// </list>
        /// </p></para>
        /// </param>
        /// <param name="dwFlagsAndAttributes"></param>
        /// <param name="hTemplateFile"></param>
        /// <returns>使用函数打开的设备的句柄。</returns>
        /// <remarks>
        /// 本函数可以执行打开或建立文件、文件流、目录/文件夹、物理磁盘、卷、系统控制的缓冲区、磁带设备、
        /// 通信资源、邮件系统和命名管道。
        /// </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode,
                                                IntPtr lpSecurityAttributes, uint dwCreationDisposition,
                                                uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        /// <summary>
        /// 对设备执行指定的操作。
        /// </summary>
        /// <param name="hDevice">要执行操作的设备句柄。</param>
        /// <param name="dwIoControlCode">Win32 API 常数，输入的是以 <b>FSCTL_</b> 为前缀的常数，定义在 
        /// <b>WinIoCtl.h</b> 文件内，执行此重载方法必须输入 <b>SMART_GET_VERSION</b> 。</param>
        /// <param name="lpInBuffer">当参数为指针时，默认的输入值是 <b>0</b> 。</param>
        /// <param name="nInBufferSize">输入缓冲区的字节数量。</param>
        /// <param name="lpOutBuffer">一个 <b>GetVersionOutParams</b> ，表示执行函数后输出的设备检查。</param>
        /// <param name="nOutBufferSize">输出缓冲区的字节数量。</param>
        /// <param name="lpBytesReturned">实际装载到输出缓冲区的字节数量。</param>
        /// <param name="lpOverlapped">同步操作控制，一般不使用，默认值为 <b>0</b> 。</param>
        /// <returns>非零表示成功，零表示失败。</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int DeviceIoControl(IntPtr hDevice, uint dwIoControlCode, IntPtr lpInBuffer,
                                                  uint nInBufferSize, ref GetVersionOutParams lpOutBuffer,
                                                  uint nOutBufferSize, ref uint lpBytesReturned,
                                                  [Out] IntPtr lpOverlapped);

        /// <summary>
        /// 对设备执行指定的操作。
        /// </summary>
        /// <param name="hDevice">要执行操作的设备句柄。</param>
        /// <param name="dwIoControlCode">Win32 API 常数，输入的是以 <b>FSCTL_</b> 为前缀的常数，定义在 
        /// <b>WinIoCtl.h</b> 文件内，执行此重载方法必须输入 <b>SMART_SEND_DRIVE_COMMAND</b> 或 <b>SMART_RCV_DRIVE_DATA</b> 。</param>
        /// <param name="lpInBuffer">一个 <b>SendCmdInParams</b> 结构，它保存向系统发送的查询要求具体命令的数据结构。</param>
        /// <param name="nInBufferSize">输入缓冲区的字节数量。</param>
        /// <param name="lpOutBuffer">一个 <b>SendCmdOutParams</b> 结构，它保存系统根据命令返回的设备相信信息二进制数据。</param>
        /// <param name="nOutBufferSize">输出缓冲区的字节数量。</param>
        /// <param name="lpBytesReturned">实际装载到输出缓冲区的字节数量。</param>
        /// <param name="lpOverlapped">同步操作控制，一般不使用，默认值为 <b>0</b> 。</param>
        /// <returns>非零表示成功，零表示失败。</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int DeviceIoControl(IntPtr hDevice, uint dwIoControlCode, ref SendCmdInParams lpInBuffer,
                                                  uint nInBufferSize, ref SendCmdOutParams lpOutBuffer,
                                                  uint nOutBufferSize, ref uint lpBytesReturned,
                                                  [Out] IntPtr lpOverlapped);

        [DllImport("kernel32")]
        public static extern int GetCurrentThreadId();

        /// <summary>
        /// 向全局原子表添加一个字符串，并返回这个字符串的唯一标识符,成功则返回值为新创建的原子ID,失败返回0
        /// </summary>
        /// <param name="lpString"></param>
        /// <returns></returns>
        [DllImport("kernel32", SetLastError = true)]
        public static extern short GlobalAddAtom(string lpString);

        [DllImport("kernel32", SetLastError = true)]
        public static extern short GlobalDeleteAtom(short nAtom);

        /// <summary>
        /// 获取伪句柄
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll ", ExactSpelling = true)]
        public static extern IntPtr GetCurrentProcess();

    }
}
