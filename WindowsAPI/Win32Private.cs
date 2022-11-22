using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using static SuperFramework.WindowsAPI.APIStruct;
using static SuperFramework.WindowsAPI.Kernel32API;

namespace SuperFramework.WindowsAPI
{
    /// <summary>
    /// 执行需要调用 <b>Win32</b> API 的操作辅助类。
    /// </summary>
    [SuppressUnmanagedCodeSecurity()]
    public static partial class Win32
    {
        #region 方法

        /// <summary>
        /// 执行获取当前运行的操作系统版本。
        /// </summary>
        /// <returns><see cref="Platform"/> 的值之一，他表示当前运行的操作系统版本。</returns>
        public static Platform GetCurrentPlatform()
        {
            OperatingSystem os = Environment.OSVersion;
            Platform pt;
            switch(os.Platform)
            {
                case (PlatformID.Win32Windows): // Win95, Win98 or Me
                    switch(os.Version.Minor)
                    {
                        case (0): // 95
                            pt = Platform.Windows95;
                            break;
                        case (10): // 98
                            if(os.Version.Revision.ToString() == "2222A")
                                pt = Platform.Windows982ndEdition;
                            else
                                pt = Platform.Windows98;
                            break;
                        case (90): // winme
                            pt = Platform.WindowsME;
                            break;
                        default: // Unknown
                            pt = Platform.UnKnown;
                            break;
                    }
                    break;
                case (PlatformID.Win32NT): //Win2k or Xp or 2003
                    switch(os.Version.Major)
                    {
                        case (3):
                            pt = Platform.WindowsNT351;
                            break;
                        case (4):
                            pt = Platform.WindowsNT40;
                            break;
                        case (5):
                            if(os.Version.Minor == 0)
                                pt = Platform.Windows2000;
                            else if(os.Version.Minor == 1)
                                pt = Platform.WindowsXP;
                            else if(os.Version.Minor == 2)
                                pt = Platform.Windows2003;
                            else
                                pt = Platform.UnKnown;
                            break;
                        case (6):
                            pt = Platform.WindowsVista;
                            break;
                        default:
                            pt = Platform.UnKnown;
                            break;
                    }
                    break;
                case (PlatformID.WinCE): // WinCE
                    pt = Platform.WindowsCE;
                    break;
                case (PlatformID.Win32S):
                case (PlatformID.Unix):
                default:
                    pt = Platform.UnKnown;
                    break;
            }
            return pt;
        }

        /// <summary>
        /// 表示操作系统平台。
        /// </summary>
        public enum Platform : byte
        {
            /// <summary>
            /// Windows 95 操作系统.
            /// </summary>
            Windows95,
            /// <summary>
            /// Windows 98 操作系统.
            /// </summary>
            Windows98,
            /// <summary>
            /// Windows 98 第二版操作系统.
            /// </summary>
            Windows982ndEdition,
            /// <summary>
            /// Windows ME 操作系统.
            /// </summary>
            WindowsME,
            /// <summary>
            /// Windows NT 3.51 操作系统.
            /// </summary>
            WindowsNT351,
            /// <summary>
            /// Windows NT 4.0 操作系统.
            /// </summary>
            WindowsNT40,
            /// <summary>
            /// Windows 2000 操作系统.
            /// </summary>
            Windows2000,
            /// <summary>
            /// Windows XP 操作系统.
            /// </summary>
            WindowsXP,
            /// <summary>
            /// Windows 2003 操作系统.
            /// </summary>
            Windows2003,
            /// <summary>
            /// Windows Vista 操作系统.
            /// </summary>
            WindowsVista,
            /// <summary>
            /// Windows CE 操作系统.
            /// </summary>
            WindowsCE,
            /// <summary>
            /// 操作系统版本未知。
            /// </summary>
            UnKnown
        }

        /// <summary>
        /// 表示IDE设备错误状态代码的常量与数值的对应。
        /// </summary>
        /// <remarks>其数值与常量定义在 <b>WinIoCtl.h</b> 文件中。</remarks>
        private enum DriverError : byte
        {
            /// <summary>
            /// 设备无错误。
            /// </summary>
            SMART_NO_ERROR = 0, // No error
            /// <summary>
            /// 设备IDE控制器错误。
            /// </summary>
            SMART_IDE_ERROR = 1, // Error from IDE controller
            /// <summary>
            /// 无效的命令标记。
            /// </summary>
            SMART_INVALID_FLAG = 2, // Invalid command flag
            /// <summary>
            /// 无效的命令数据。
            /// </summary>
            SMART_INVALID_COMMAND = 3, // Invalid command byte
            /// <summary>
            /// 缓冲区无效（如缓冲区为空或地址错误）。
            /// </summary>
            SMART_INVALID_BUFFER = 4, // Bad buffer (null, invalid addr..)
            /// <summary>
            /// 设备编号错误。
            /// </summary>
            SMART_INVALID_DRIVE = 5, // Drive number not valid
            /// <summary>
            /// IOCTL错误。
            /// </summary>
            SMART_INVALID_IOCTL = 6, // Invalid IOCTL
            /// <summary>
            /// 无法锁定用户的缓冲区。
            /// </summary>
            SMART_ERROR_NO_MEM = 7, // Could not lock user's buffer
            /// <summary>
            /// 无效的IDE注册命令。
            /// </summary>
            SMART_INVALID_REGISTER = 8, // Some IDE Register not valid
            /// <summary>
            /// 无效的命令设置。
            /// </summary>
            SMART_NOT_SUPPORTED = 9, // Invalid cmd flag set
            /// <summary>
            /// 指定要查找的设别索引号无效。
            /// </summary>
            SMART_NO_IDE_DEVICE = 10
        }

        public static void ChangeByteOrder(byte[] charArray)
        {
            byte temp;
            for(int i = 0; i < charArray.Length; i += 2)
            {
                temp = charArray[i];
                charArray[i] = charArray[i + 1];
                charArray[i + 1] = temp;
            }
        }

        /// <summary>
        /// 根据指定的设备信息生成设备的详细信息。
        /// </summary>
        /// <param name="phdinfo">一个 <see cref="IdSector"/></param>
        /// <returns></returns>
        internal static HDiskInfo GetHardDiskInfo(IdSector phdinfo)
        {
            HDiskInfo hdd = new() { ModuleNumber = Encoding.ASCII.GetString(phdinfo.sModelNumber).Trim(), Firmware = Encoding.ASCII.GetString(phdinfo.sFirmwareRev).Trim(), SerialNumber = Encoding.ASCII.GetString(phdinfo.sSerialNumber).Trim(), Capacity = phdinfo.ulTotalAddressableSectors / 2 / 1024, BufferSize = phdinfo.wBufferSize / 1024 };
            return hdd;
        }

        /// <summary>
        /// 获取在NT平台下指定序列号的硬盘信息。
        /// </summary>
        /// <param name="driveIndex">物理磁盘的数量。</param>
        /// <returns></returns>
        internal static HDiskInfo GetHddInfoNT(byte driveIndex)
        {
            GetVersionOutParams vers = new();
            SendCmdInParams inParam = new();
            SendCmdOutParams outParam = new();
            uint bytesReturned = 0;

            // 使用 Win2000 或 Xp下的方法获取硬件信息

            // 获取设备的句柄。
            IntPtr hDevice =
                CreateFile(string.Format(@"\\.\PhysicalDrive{0}", driveIndex), GENERIC_READ | GENERIC_WRITE,
                           FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

            // 开始检查
            if(hDevice == IntPtr.Zero)
                throw new UnauthorizedAccessException("执行 Win32 API 函数 CreateFile 失败。");
            if(0 == DeviceIoControl(hDevice, SMART_GET_VERSION, IntPtr.Zero, 0, ref vers,
                (uint)Marshal.SizeOf(vers),
                ref bytesReturned,
                IntPtr.Zero))
            {
                CloseHandle(hDevice);
                throw new IOException(string.Format(ResourcesApi.Win32_DeviceIoControlErr, "SMART_GET_VERSION"));
            }
            // 检测IDE控制命令是否支持
            if(0 == (vers.fCapabilities & 1))
            {
                CloseHandle(hDevice);
                throw new IOException(ResourcesApi.Win32_DeviceIoControlNotSupport);
            }
            // Identify the IDE drives
            if(0 != (driveIndex & 1))
                inParam.irDriveRegs.bDriveHeadReg = 0xb0;
            else
                inParam.irDriveRegs.bDriveHeadReg = 0xa0;
            if(0 != (vers.fCapabilities & (16 >> driveIndex)))
            {
                // We don't detect a ATAPI device.
                CloseHandle(hDevice);
                throw new IOException(ResourcesApi.Win32_DeviceIoControlNotSupport);
            }
            else
                inParam.irDriveRegs.bCommandReg = 0xec;
            inParam.bDriveNumber = driveIndex;
            inParam.irDriveRegs.bSectorCountReg = 1;
            inParam.irDriveRegs.bSectorNumberReg = 1;
            inParam.cBufferSize = 512;

            if(0 == DeviceIoControl(
                hDevice,
                SMART_RCV_DRIVE_DATA,
                ref inParam,
                (uint)Marshal.SizeOf(inParam),
                ref outParam,
                (uint)Marshal.SizeOf(outParam),
                ref bytesReturned,
                IntPtr.Zero))
            {
                CloseHandle(hDevice);
                throw new IOException(
                        string.Format(ResourcesApi.Win32_DeviceIoControlErr, "SMART_RCV_DRIVE_DATA"));
            }
            CloseHandle(hDevice);
            ChangeByteOrder(outParam.bBuffer.sModelNumber);
            ChangeByteOrder(outParam.bBuffer.sSerialNumber);
            ChangeByteOrder(outParam.bBuffer.sFirmwareRev);
            return GetHardDiskInfo(outParam.bBuffer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driveIndex"></param>
        /// <returns></returns>
        internal static HDiskInfo GetHddInfo9X(byte driveIndex)
        {
            GetVersionOutParams vers = new();
            SendCmdInParams inParam = new();
            SendCmdOutParams outParam = new();
            uint bytesReturned = 0;
            IntPtr hDevice = CreateFile(@"\\.\Smartvsd", 0, 0, IntPtr.Zero, CREATE_NEW, 0, IntPtr.Zero);
            if(hDevice == IntPtr.Zero)
                throw new UnauthorizedAccessException("打开 smartvsd.vxd 文件失败。");
            if(0 == DeviceIoControl(hDevice, SMART_GET_VERSION, 
                IntPtr.Zero, 0, 
                ref vers, (uint)Marshal.SizeOf(vers), ref bytesReturned, IntPtr.Zero))
            {
                CloseHandle(hDevice);
                throw new IOException(string.Format(ResourcesApi.Win32_DeviceIoControlErr, "SMART_GET_VERSION"));
            }
            // 如果 IDE 的鉴定命令不被识别或失败
            if(0 == (vers.fCapabilities & 1))
            {
                CloseHandle(hDevice);
                throw new IOException(ResourcesApi.Win32_DeviceIoControlNotSupport);
            }
            if(0 != (driveIndex & 1))
                inParam.irDriveRegs.bDriveHeadReg = 0xb0;
            else
                inParam.irDriveRegs.bDriveHeadReg = 0xa0;
            if(0 != (vers.fCapabilities & (16 >> driveIndex)))
            {
                // 检测出IDE为ATAPI类型，无法处理
                CloseHandle(hDevice);
                throw new IOException(ResourcesApi.Win32_DeviceIoControlNotSupport);
            }
            else
                inParam.irDriveRegs.bCommandReg = 0xec;
            inParam.bDriveNumber = driveIndex;
            inParam.irDriveRegs.bSectorCountReg = 1;
            inParam.irDriveRegs.bSectorNumberReg = 1;
            inParam.cBufferSize = BUFFER_SIZE;
            if(0 == DeviceIoControl(hDevice, SMART_RCV_DRIVE_DATA, ref inParam, (uint)Marshal.SizeOf(inParam), ref outParam, (uint)Marshal.SizeOf(outParam), ref bytesReturned, IntPtr.Zero))
            {
                CloseHandle(hDevice);
                throw new IOException(string.Format(ResourcesApi.Win32_DeviceIoControlErr, "SMART_RCV_DRIVE_DATA"));
            }
            // 关闭文件句柄
            CloseHandle(hDevice);
            ChangeByteOrder(outParam.bBuffer.sModelNumber);
            ChangeByteOrder(outParam.bBuffer.sSerialNumber);
            ChangeByteOrder(outParam.bBuffer.sFirmwareRev);
            return GetHardDiskInfo(outParam.bBuffer);
        }

        #endregion

        #region 常量

        /// <summary>
        /// Win32 API 常数，指示在使用 <see cref="RemoveMenu"/> 函数时指定使用索引数而不是使用ID。
        /// </summary>
        private const int MF_BYPOSITION = 0x00000400;
        private const uint FILE_SHARE_READ = 0x00000001;
        private const uint FILE_SHARE_WRITE = 0x00000002;
        private const uint FILE_SHARE_DELETE = 0x00000004;
        private const uint SMART_GET_VERSION = 0x00074080; // SMART_GET_VERSION
        private const uint SMART_SEND_DRIVE_COMMAND = 0x0007c084; // SMART_SEND_DRIVE_COMMAND
        private const uint SMART_RCV_DRIVE_DATA = 0x0007c088; // SMART_RCV_DRIVE_DATA
        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint CREATE_NEW = 1;
        private const uint OPEN_EXISTING = 3;
        private const uint BUFFER_SIZE = 512;
        public static Platform currentOs;

        #endregion
    }
}