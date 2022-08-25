using System;
using System.Runtime.InteropServices;

namespace SuperFramework.WindowsAPI
{
    /// <summary>
    /// 日 期:2015-09-08
    /// 作 者:不良帥
    /// 描 述:API操作结构
    /// </summary>
    public static class APIStruct
    {
        #region 系统结构
        /// <summary>
        /// 文件操作信息
        /// </summary>
        public struct SHFILEOPSTRUCT
        {
            /// <summary>
            /// 父窗口句柄
            /// </summary>
            public IntPtr hwnd;         //父窗口句柄 
            /// <summary>
            /// 要执行的动作
            /// </summary>
            public wFunc wFunc;         //要执行的动作 
            /// <summary>
            /// 源文件路径，可以是多个文件，以结尾符号"\0"结束
            /// </summary>
            public string pFrom;        //源文件路径，可以是多个文件，以结尾符号"\0"结束
            /// <summary>
            /// 目标路径，可以是路径或文件名 
            /// </summary>
            public string pTo;          //目标路径，可以是路径或文件名 
            /// <summary>
            /// 标志，附加选项 
            /// </summary>
            public FILEOP_FLAGS fFlags;             //标志，附加选项 
            /// <summary>
            /// 是否可被中断 
            /// </summary>
            public bool fAnyOperationsAborted;      //是否可被中断 
            /// <summary>
            /// 文件映射名字，可在其它 Shell 函数中使用 
            /// </summary>
            public IntPtr hNameMappings;            //文件映射名字，可在其它 Shell 函数中使用 
            /// <summary>
            ///  只在 FOF_SIMPLEPROGRESS 时，指定对话框的标题
            /// </summary>
            public string lpszProgressTitle;        // 只在 FOF_SIMPLEPROGRESS 时，指定对话框的标题。
        }

        public enum wFunc
        {
            /// <summary>
            /// 移动文件
            /// </summary>
            FO_MOVE = 0x0001,   //移动文件
            /// <summary>
            /// 复制文件
            /// </summary>
            FO_COPY = 0x0002,   //复制文件
            /// <summary>
            /// 删除文件，只是用pFrom
            /// </summary>
            FO_DELETE = 0x0003, //删除文件，只是用pFrom
            /// <summary>
            /// 文件重命名
            /// </summary>
            FO_RENAME = 0x0004  //文件重命名
        }

        public enum FILEOP_FLAGS
        {
            /// <summary>
            /// pTo 指定了多个目标文件，而不是单个目录
            /// </summary>
            FOF_MULTIDESTFILES = 0x0001,    //pTo 指定了多个目标文件，而不是单个目录
            /// <summary>
            /// 不显示一个进度对话框
            /// </summary>
            FOF_CONFIRMMOUSE = 0x0002,
            /// <summary>
            /// 碰到有抵触的名字时，自动分配前缀
            /// </summary>
            FOF_SILENT = 0x0044,            // 不显示一个进度对话框
            /// <summary>
            /// 碰到有抵触的名字时，自动分配前缀
            /// </summary>
            FOF_RENAMEONCOLLISION = 0x0008, // 碰到有抵触的名字时，自动分配前缀
            /// <summary>
            /// 不对用户显示提示
            /// </summary>
            FOF_NOCONFIRMATION = 0x10,      // 不对用户显示提示
            /// <summary>
            /// 填充 hNameMappings 字段，必须使用 SHFreeNameMappings 释放
            /// </summary>
            FOF_WANTMAPPINGHANDLE = 0x0020, // 填充 hNameMappings 字段，必须使用 SHFreeNameMappings 释放
            /// <summary>
            /// 允许撤销
            /// </summary>
            FOF_ALLOWUNDO = 0x40,           // 允许撤销
            /// <summary>
            /// 使用 *.* 时, 只对文件操作
            /// </summary>
            FOF_FILESONLY = 0x0080,         // 使用 *.* 时, 只对文件操作
            /// <summary>
            /// 简单进度条，意味者不显示文件名。
            /// </summary>
            FOF_SIMPLEPROGRESS = 0x0100,    // 简单进度条，意味者不显示文件名。
            /// <summary>
            /// 建新目录时不需要用户确定
            /// </summary>
            FOF_NOCONFIRMMKDIR = 0x0200,    // 建新目录时不需要用户确定
            /// <summary>
            ///  不显示出错用户界面
            /// </summary>
            FOF_NOERRORUI = 0x0400,         // 不显示出错用户界面
            /// <summary>
            /// 不复制 NT 文件的安全属性
            /// </summary>
            FOF_NOCOPYSECURITYATTRIBS = 0x0800,     // 不复制 NT 文件的安全属性
            /// <summary>
            /// 不递归目录
            /// </summary>
            FOF_NORECURSION = 0x1000        // 不递归目录
        }
        #endregion

        [StructLayout(LayoutKind.Sequential)]
        public struct TagRECT
        {

            /// LONG->int
            public int left;

            /// LONG->int
            public int top;

            /// LONG->int
            public int right;

            /// LONG->int
            public int bottom;
        }
        /// <summary>
        /// Windows在通过WM_COPYDATA消息传递期间，不提供继承同步方式。
        /// 其中,WM_COPYDATA对应的十六进制数为0x004A
        /// </summary>
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        #region 图标结构
        /// <summary>
        /// 图标结构
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            /// <summary>
            /// 文件的图标句柄
            /// </summary>
            public IntPtr hIcon;//文件的图标句柄
            /// <summary>
            /// 图标的系统索引号
            /// </summary>
            public IntPtr iIcon;//图标的系统索引号
            /// <summary>
            /// 文件的属性值
            /// </summary>
            public uint dwAttributes;//文件的属性值
            /// <summary>
            /// 文件的显示名
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;//文件的显示名
            /// <summary>
            /// 文件的类型名
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;//文件的类型名
        }
        #endregion
        /// <summary>
        /// 定义了辅助键的名称（将数字转变为字符以便于记忆，也可去除此枚举而直接使用数值）
        /// </summary>
        [Flags()]
        public enum KeyModifiers
        {
            None = 0,
            Alt = 1,
            Ctrl = 2,
            Shift = 4,
            WindowsKey = 8
        }


        /// <summary>
        /// ComBox信息
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct COMBOBOXINFO
        {
            public int cbSize;
            public RECT rcItem;
            public RECT rcButton;
            public IntPtr stateButton;
            public IntPtr hwndCombo;
            public IntPtr hwndItem;
            public IntPtr hwndList;
        }
        /// <summary>
        /// 控件区域
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        /// <summary>
        /// 键盘消息
        /// </summary>
        public struct KeyboardMSG
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
            public int VK_CONTROL;
            public int VK_MENU;
            public int VK_DELETE;
        }
        /// <summary>
        /// An LUID is a 64-bit value guaranteed to be unique only on the system on which it was generated. The uniqueness of a locally unique identifier (LUID) is guaranteed only until the system is restarted.
        /// 本地唯一标志是一个64位的数值，它被保证在产生它的系统上唯一！LUID的在机器被重启前都是唯一的
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LUID
        {
            /// <summary>
            /// The low order part of the 64 bit value.
            /// 本地唯一标志的低32位
            /// </summary>
            public int LowPart;
            /// <summary>
            /// The high order part of the 64 bit value.
            /// 本地唯一标志的高32位
            /// </summary>
            public int HighPart;
        }
        /// <summary>
        /// The LUID_AND_ATTRIBUTES structure represents a locally unique identifier (LUID) and its attributes.
        /// LUID_AND_ATTRIBUTES 结构呈现了本地唯一标志和它的属性
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LUID_AND_ATTRIBUTES
        {
            /// <summary>
            /// Specifies an LUID value.
            /// </summary>
            public LUID pLuid;
            /// <summary>
            /// Specifies attributes of the LUID. This value contains up to 32 one-bit flags. Its meaning is dependent on the definition and use of the LUID.
            /// 指定了LUID的属性，其值可以是一个32位大小的bit 标志，具体含义根据LUID的定义和使用来看
            /// </summary>
            public int Attributes;
        }
        /// <summary>
        /// The TOKEN_PRIVILEGES structure contains information about a set of privileges for an access token.
        /// TOKEN_PRIVILEGES 结构包含了一个访问令牌的一组权限信息：即该访问令牌具备的权限
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TOKEN_PRIVILEGES
        {
            /// <summary>
            /// Specifies the number of entries in the Privileges array.
            /// 指定了权限数组的容量
            /// </summary>
            public int PrivilegeCount;
            /// <summary>
            /// Specifies an array of LUID_AND_ATTRIBUTES structures. Each structure contains the LUID and attributes of a privilege.
            /// 指定一组的LUID_AND_ATTRIBUTES 结构，每个结构包含了LUID和权限的属性
            /// </summary>
            public LUID_AND_ATTRIBUTES Privileges;
        }

        /// <summary>
        /// 保存当前计算机 IDE 设备（硬盘）的硬件信息的结构。
        /// </summary>
        [Serializable]
        public struct HDiskInfo
        {
            /// <summary>
            /// 硬盘型号。
            /// </summary>
            public string ModuleNumber;

            /// <summary>
            /// 硬盘的固件版本。
            /// </summary>
            public string Firmware;

            /// <summary>
            /// 硬盘序列号。
            /// </summary>
            public string SerialNumber;

            /// <summary>
            /// 硬盘容量，以M为单位。
            /// </summary>
            public uint Capacity;

            /// <summary>
            /// 设备缓存大小（以M为单位）。
            /// </summary>
            public int BufferSize;
        }

        /// <summary>
        /// 表示使用 <b>DeviceIoControl</b> 函数时保存返回的驱动器硬件信息的结构
        /// </summary>
        /// <remarks>>此数据结构定义在 <b>WinIoCtl.h</b> 文件名为 <b>_GETVERSIONINPARAMS</b> 结构中。</remarks>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct GetVersionOutParams
        {
            /// <summary>
            /// IDE设备的二进制硬件版本。
            /// </summary>
            public byte bVersion;

            /// <summary>
            /// IDE设备的二进制修订版本。
            /// </summary>
            public byte bRevision;

            /// <summary>
            /// 此值操作系统没有使用，使用此数据结构时被设置为 <b>0</b> 。
            /// </summary>
            public byte bReserved;

            /// <summary>
            /// IDE设备的二进制映射。
            /// </summary>
            public byte bIDEDeviceMap;

            /// <summary>
            /// IDE设备的二进制容量数据。
            /// </summary>
            public uint fCapabilities;

            /// <summary>
            /// 保留内容，不使用。
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] dwReserved; // For future use.
        }

        /// <summary>
        /// 一个数据结构，表示使用 <b>DeviceIoControl</b> 函数时发送到操作系统中的命令数据结构 <b>SendCmdInParams</b> 的成员结构。
        /// 它表示要获取磁盘设备性能参数的具体定义规则。
        /// </summary>
        /// <seealso cref="SendCmdInParams"/>
        /// <remarks>此数据结构定义在 <b>WinIoCtl.h</b> 文件名为 <b>_IDEREGS</b> 中。</remarks>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IdeRegs
        {
            /// <summary>
            /// 发送到操作系统的注册命令，此为系统的 <b>SMART Command</b> 。
            /// </summary>
            public byte bFeaturesReg;

            /// <summary>
            /// 获取IDE设备扇区数。
            /// </summary>
            public byte bSectorCountReg;

            /// <summary>
            /// 获取IDE设备编号。
            /// </summary>
            public byte bSectorNumberReg;

            /// <summary>
            /// 获取IDE设备低端柱面值。
            /// </summary>
            public byte bCylLowReg;

            /// <summary>
            /// 获取IDE设备高端柱面值。
            /// </summary>
            public byte bCylHighReg;

            /// <summary>
            /// 获取IDE设备的头信息。
            /// </summary>
            public byte bDriveHeadReg;

            /// <summary>
            /// 获取IDE设备的真正命令。
            /// </summary>
            public byte bCommandReg;

            /// <summary>
            /// 保留内容，此值应设置为 <b>0</b> 。
            /// </summary>
            public byte bReserved;
        }

        /// <summary>
        /// 保存执行 <b>DeviceIoControl</b> 函数时向系统提交的执行操作命令。
        /// </summary>
        /// <seealso cref="SendCmdInParams"/>
        /// <remarks>此数据结构定义在 <b>WinIoCtl.h</b> 文件名为 <b>_SENDCMDINPARAMS</b> 中。</remarks>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SendCmdInParams
        {
            /// <summary>
            /// 输出的数据缓冲大小。
            /// </summary>
            public uint cBufferSize;

            /// <summary>
            /// 保存向系统发送的磁盘设备命令的数据结构。
            /// </summary>
            public IdeRegs irDriveRegs;

            /// <summary>
            /// 希望系统控制的物理磁盘的编号。
            /// </summary>
            public byte bDriveNumber;

            /// <summary>
            /// 保留的数据，不使用。
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] bReserved;

            /// <summary>
            /// 保留的数据，不使用。
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] dwReserved;

            /// <summary>
            /// 保存当前 <b>SendCmdInParams</b> 结构填充数据后的大小。
            /// </summary>
            public byte bBuffer;
        }

        /// <summary>
        /// 当执行 <b>DeviceIoControl</b> 函数后系统返回的 <b>SendCmdOutParams</b> 结构中
        /// 保存磁盘设备当前错误信息的数据结构。
        /// </summary>
        /// <seealso cref="SendCmdInParams"/>
        /// <remarks>
        /// 此数据结构定义在 <b>WinIoCtl.h</b> 文件名为 <b>_DRIVERSTATUS</b> 中。
        /// <para>
        /// 错误代码如下表：<br />
        /// <list type="table">
        /// <listheader>
        /// <term>名称</term>
        /// <description>说明</description>
        /// <item><term>SMART_NO_ERROR = 0</term>
        /// <description>没有错误。</description></item>
        /// <item><term>SMART_IDE_ERROR = 1</term>
        /// <description>IDE控制器错误</description>。</item>
        /// <item><term>SMART_INVALID_FLAG = 2</term>
        /// <description>发送的命令标记无效。</description></item>
        /// <item><term>SMART_INVALID_COMMAND = 3</term>
        /// <description>发送的二进制命令无效。</description></item>
        /// <item><term>SMART_INVALID_BUFFER = 4</term>
        /// <description>二进制缓存无效（缓存为空或者无效地址）。</description></item>
        /// <item><term>SMART_INVALID_DRIVE = 5</term>
        /// <description>物理驱动器编号无效。</description></item>
        /// <item><term>SMART_INVALID_IOCTL = 6</term>
        /// <description>无效的IOCTL。</description></item>
        /// <item><term>SMART_ERROR_NO_MEM =  7</term>
        /// <description>使用的缓冲区无法锁定。</description></item>
        /// <item><term>SMART_INVALID_REGISTER = 8</term>
        /// <description>IDE注册命令无效。</description></item>
        /// <item><term>SMART_NOT_SUPPORTED = 9</term>
        /// <description>命令标记设置无效。</description></item>
        /// <item><term>SMART_NO_IDE_DEVICE = 10</term>
        /// <description>发送的物理驱动器索引超过限制。</description></item>
        /// </listheader>
        /// </list>
        /// </para>
        /// </remarks>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DriverStatus
        {
            /// <summary>
            /// 如果检查的IDE设备发生错误，保存的错误代码，<b>0</b> 表示没有错误。
            /// </summary>
            public byte bDriverError;

            /// <summary>
            /// IDE设备被注册的错误内容。
            /// </summary>
            public byte bIDEStatus;

            /// <summary>
            /// 保留的数据，不使用。
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] bReserved;

            /// <summary>
            /// 保留的数据，不使用。
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public uint[] dwReserved;
        }

        /// <summary>
        /// 表示当执行 <b>DeviceIoControl</b> 函数后保存系统根据查询命令返回的磁盘设备信息的数据结构。
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SendCmdOutParams
        {
            /// <summary>
            /// 表示所有二进制信息的缓存大小。
            /// </summary>
            public uint cBufferSize;

            /// <summary>
            /// 表示查询到设备的错误信息状态。
            /// </summary>
            public DriverStatus DriverStatus;

            /// <summary>
            /// 表示系统返回的设备硬件信息的二进制数据结构。
            /// </summary>
            public IdSector bBuffer;
        }

        /// <summary>
        /// 当执行 <b>DeviceIoControl</b> 函数后系统返回的 <b>SendCmdOutParams</b> 结构中
        /// 保存磁盘设备的硬件信息的数据结构。
        /// </summary>
        /// <seealso cref="SendCmdInParams"/>
        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 512)]
        public struct IdSector
        {
            /// <summary>
            /// 设备通用配置信息。
            /// </summary>
            public ushort wGenConfig;

            /// <summary>
            /// 设备的柱面数。
            /// </summary>
            public ushort wNumCyls;

            /// <summary>
            /// 保留内容，不使用。
            /// </summary>
            public ushort wReserved;

            /// <summary>
            /// 设备的磁头数目。
            /// </summary>
            public ushort wNumHeads;

            /// <summary>
            /// 设备的磁道数目。
            /// </summary>
            public ushort wBytesPerTrack;

            /// <summary>
            /// 设备的扇区数目。
            /// </summary>
            public ushort wBytesPerSector;

            /// <summary>
            /// 设备厂商设定的扇区磁道数目。
            /// </summary>
            public ushort wSectorsPerTrack;

            /// <summary>
            /// 设备的出品厂商名称。
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public ushort[] wVendorUnique;

            /// <summary>
            /// 设备出品厂商的全球唯一编码。
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] sSerialNumber;

            /// <summary>
            /// 设备的缓存类型。
            /// </summary>
            public ushort wBufferType;

            /// <summary>
            /// 设备缓存容量（单位是byte）。
            /// </summary>
            public ushort wBufferSize;

            /// <summary>
            /// 设备的错误检查和纠正（ECC）数据的大小。
            /// </summary>
            public ushort wECCSize;

            /// <summary>
            /// 设备的固件版本。
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] sFirmwareRev;

            /// <summary>
            /// 设备的型号。
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
            public byte[] sModelNumber;

            /// <summary>
            /// 设备厂商名称的扩展内容（如果有）。
            /// </summary>
            public ushort wMoreVendorUnique;

            /// <summary>
            /// 设备双指令输入输出模式。
            /// </summary>
            public ushort wDoubleWordIO;

            /// <summary>
            /// 设备的容量大小（单位Byte）。
            /// </summary>
            public ushort wCapabilities;

            /// <summary>
            /// 第一个保留的内容，不使用。
            /// </summary>
            public ushort wReserved1;

            /// <summary>
            /// 设备的PIO模式巡道时间。
            /// </summary>
            public ushort wPIOTiming;

            /// <summary>
            /// 设备DMA 模式巡道时间。
            /// </summary>
            public ushort wDMATiming;

            /// <summary>
            /// 设备的总线类型，如SCSI,IDE等。
            /// </summary>
            public ushort wBS;

            /// <summary>
            /// 设备的当前柱面数量。
            /// </summary>
            public ushort wNumCurrentCyls;

            /// <summary>
            /// 设备当前磁头数量。
            /// </summary>
            public ushort wNumCurrentHeads;

            /// <summary>
            /// 设备的当前扇区的磁道数量。
            /// </summary>
            public ushort wNumCurrentSectorsPerTrack;

            /// <summary>
            /// 设备的当前扇区容量（单位byte）。
            /// </summary>
            public uint ulCurrentSectorCapacity;

            /// <summary>
            /// 多扇区读写模式支持。
            /// </summary>
            public ushort wMultSectorStuff;

            /// <summary>
            /// 用户是否可自定义扇区地址(LBA模式）支持。
            /// </summary>
            public uint ulTotalAddressableSectors;

            /// <summary>
            /// 单指令DMA模式。
            /// </summary>
            public ushort wSingleWordDMA;

            /// <summary>
            /// 多指令DMA模式。
            /// </summary>
            public ushort wMultiWordDMA;

            /// <summary>
            /// 保留内容，不使用。
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public byte[] bReserved;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //[StructLayout(LayoutKind.Sequential)]
        //public struct RECT
        //{
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <param name="left"></param>
        //    /// <param name="top"></param>
        //    /// <param name="right"></param>
        //    /// <param name="bottom"></param>
        //    public RECT(int left, int top, int right, int bottom)
        //    {
        //        this.left = left;
        //        this.top = top;
        //        this.right = right;
        //        this.bottom = bottom;
        //    }

        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <param name="r"></param>
        //    public RECT(Rectangle r)
        //    {
        //        left = r.Left;
        //        top = r.Top;
        //        right = r.Right;
        //        bottom = r.Bottom;
        //    }
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    public int left;
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    public int top;
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    public int right;
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    public int bottom;

        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <param name="x"></param>
        //    /// <param name="y"></param>
        //    /// <param name="width"></param>
        //    /// <param name="height"></param>
        //    /// <returns></returns>
        //    public static RECT FromXYWH(int x, int y, int width, int height)
        //    {
        //        return new RECT(x, y, x + width, y + height);
        //    }

        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    public Size Size
        //    {
        //        get { return new Size(right - left, bottom - top); }
        //    }
        //}
        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public sealed class SCROLLINFO
        {
            public SCROLLINFO()
            {
                cbSize = Marshal.SizeOf(typeof(SCROLLINFO));
            }

            public SCROLLINFO(int mask, int min, int max, int page, int pos)
            {
                cbSize = Marshal.SizeOf(typeof(SCROLLINFO));
                fMask = mask;
                nMin = min;
                nMax = max;
                nPage = page;
                nPos = pos;
            }

            public int cbSize;
            public int fMask;
            public int nMin;
            public int nMax;
            public int nPage;
            public int nPos;
            public int nTrackPos;
        }
        #region CPU信息结构
        /// <summary>
        /// CPU信息结构
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct CPU_INFO
        {
            /// <summary>
            /// CPU OEM id
            /// </summary>
            public uint dwOemId;

            /// <summary>
            /// 体系结构
            /// </summary>
            public Architecture architecture;
            /// <summary>
            /// 分页大小
            /// </summary>
            public uint dwPageSize;
            /// <summary>
            /// 最小寻址空间
            /// </summary>
            public uint lpMinimumApplicationAddress;
            /// <summary>
            /// 最大寻址空间
            /// </summary>
            public uint lpMaximumApplicationAddress;
            /// <summary>
            /// 处理器掩码; 0..31 表示不同的处理器
            /// </summary>
            public uint dwActiveProcessorMask;
            /// <summary>
            /// CPU总数
            /// </summary>
            public uint dwNumberOfProcessors;
            /// <summary>
            /// CPU类型
            /// </summary>
            public uint dwProcessorType;
            /// <summary>
            /// 虚拟内存空间的粒度
            /// </summary>
            public uint dwAllocationGranularity;
            /// <summary>
            /// CPU等级
            /// </summary>
            public uint dwProcessorLevel;
            /// <summary>
            /// CPU版本
            /// </summary>
            public uint dwProcessorRevision;
        }
        #endregion

        #region CPU信息结构
        /// <summary>
        /// CPU信息结构
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Architecture
        {
            /// <summary>
            /// 处理器的体系结构
            /// </summary>
            public uint wProcessorArchitecture;
            /// <summary>
            /// 保留
            /// </summary>
            public uint wReserved;
        }
        #endregion

        #region 内存信息
        /// <summary>
        /// 内存信息
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_INFO
        {
            /// <summary>
            /// 结构的大小，在调GlobalMemoryStatus函数前用sizeof()函数求得，用来供函数检测结构的版本。
            /// </summary>
            public uint dwLength;
            /// <summary>
            /// 返回一个介于0～100之间的值，用来指示当前系统内存的使用率。
            /// </summary>
            public uint dwMemoryLoad;
            /// <summary>
            /// 返回总的物理内存大小，以字节(byte)为单位。
            /// </summary>
            public uint dwTotalPhys;
            /// <summary>
            /// 返回可用的物理内存大小，以字节(byte)为单位。
            /// </summary>
            public uint dwAvailPhys;
            /// <summary>
            /// 显示可以存在页面文件中的字节数。注意这个数值并不表示在页面文件在磁盘上的真实物理大小。
            /// </summary>
            public uint dwTotalPageFile;
            /// <summary>
            /// 返回可用的页面文件大小，以字节(byte)为单位。
            /// </summary>
            public uint dwAvailPageFile;
            /// <summary>
            /// 返回调用进程的用户模式部分的全部可用虚拟地址空间，以字节(byte)为单位。
            /// </summary>
            public uint dwTotalVirtual;
            /// <summary>
            /// 返回调用进程的用户模式部分的实际自由可用的虚拟地址空间，以字节(byte)为单位。
            /// </summary>
            public uint dwAvailVirtual;
        }
        #endregion

        #region 系统时间
        /// <summary>
        /// 系统时间
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME_INFO
        {
            /// <summary>
            /// 年
            /// </summary>
            public ushort wYear;
            /// <summary>
            /// 月
            /// </summary>
            public ushort wMonth;
            /// <summary>
            ///  星期，0=星期日，1=星期一
            /// </summary>
            public ushort wDayOfWeek;
            /// <summary>
            /// 日
            /// </summary>
            public ushort wDay;
            /// <summary>
            /// 时
            /// </summary>
            public ushort wHour;
            /// <summary>
            /// 分
            /// </summary>
            public ushort wMinute;
            /// <summary>
            /// 秒
            /// </summary>
            public ushort wSecond;
            /// <summary>
            /// 毫秒
            /// </summary>
            public ushort wMilliseconds;
        }
        #endregion
    }
}
