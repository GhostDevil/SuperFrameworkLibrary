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
    /// ִ����Ҫ���� <b>Win32</b> API �Ĳ��������ࡣ
    /// </summary>
    [SuppressUnmanagedCodeSecurity()]
    public static partial class Win32
    {
        #region ����

        /// <summary>
        /// ִ�л�ȡ��ǰ���еĲ���ϵͳ�汾��
        /// </summary>
        /// <returns><see cref="Platform"/> ��ֵ֮һ������ʾ��ǰ���еĲ���ϵͳ�汾��</returns>
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
        /// ��ʾ����ϵͳƽ̨��
        /// </summary>
        public enum Platform : byte
        {
            /// <summary>
            /// Windows 95 ����ϵͳ.
            /// </summary>
            Windows95,
            /// <summary>
            /// Windows 98 ����ϵͳ.
            /// </summary>
            Windows98,
            /// <summary>
            /// Windows 98 �ڶ������ϵͳ.
            /// </summary>
            Windows982ndEdition,
            /// <summary>
            /// Windows ME ����ϵͳ.
            /// </summary>
            WindowsME,
            /// <summary>
            /// Windows NT 3.51 ����ϵͳ.
            /// </summary>
            WindowsNT351,
            /// <summary>
            /// Windows NT 4.0 ����ϵͳ.
            /// </summary>
            WindowsNT40,
            /// <summary>
            /// Windows 2000 ����ϵͳ.
            /// </summary>
            Windows2000,
            /// <summary>
            /// Windows XP ����ϵͳ.
            /// </summary>
            WindowsXP,
            /// <summary>
            /// Windows 2003 ����ϵͳ.
            /// </summary>
            Windows2003,
            /// <summary>
            /// Windows Vista ����ϵͳ.
            /// </summary>
            WindowsVista,
            /// <summary>
            /// Windows CE ����ϵͳ.
            /// </summary>
            WindowsCE,
            /// <summary>
            /// ����ϵͳ�汾δ֪��
            /// </summary>
            UnKnown
        }

        /// <summary>
        /// ��ʾIDE�豸����״̬����ĳ�������ֵ�Ķ�Ӧ��
        /// </summary>
        /// <remarks>����ֵ�볣�������� <b>WinIoCtl.h</b> �ļ��С�</remarks>
        private enum DriverError : byte
        {
            /// <summary>
            /// �豸�޴���
            /// </summary>
            SMART_NO_ERROR = 0, // No error
            /// <summary>
            /// �豸IDE����������
            /// </summary>
            SMART_IDE_ERROR = 1, // Error from IDE controller
            /// <summary>
            /// ��Ч�������ǡ�
            /// </summary>
            SMART_INVALID_FLAG = 2, // Invalid command flag
            /// <summary>
            /// ��Ч���������ݡ�
            /// </summary>
            SMART_INVALID_COMMAND = 3, // Invalid command byte
            /// <summary>
            /// ��������Ч���绺����Ϊ�ջ��ַ���󣩡�
            /// </summary>
            SMART_INVALID_BUFFER = 4, // Bad buffer (null, invalid addr..)
            /// <summary>
            /// �豸��Ŵ���
            /// </summary>
            SMART_INVALID_DRIVE = 5, // Drive number not valid
            /// <summary>
            /// IOCTL����
            /// </summary>
            SMART_INVALID_IOCTL = 6, // Invalid IOCTL
            /// <summary>
            /// �޷������û��Ļ�������
            /// </summary>
            SMART_ERROR_NO_MEM = 7, // Could not lock user's buffer
            /// <summary>
            /// ��Ч��IDEע�����
            /// </summary>
            SMART_INVALID_REGISTER = 8, // Some IDE Register not valid
            /// <summary>
            /// ��Ч���������á�
            /// </summary>
            SMART_NOT_SUPPORTED = 9, // Invalid cmd flag set
            /// <summary>
            /// ָ��Ҫ���ҵ������������Ч��
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
        /// ����ָ�����豸��Ϣ�����豸����ϸ��Ϣ��
        /// </summary>
        /// <param name="phdinfo">һ�� <see cref="IdSector"/></param>
        /// <returns></returns>
        internal static HDiskInfo GetHardDiskInfo(IdSector phdinfo)
        {
            HDiskInfo hdd = new() { ModuleNumber = Encoding.ASCII.GetString(phdinfo.sModelNumber).Trim(), Firmware = Encoding.ASCII.GetString(phdinfo.sFirmwareRev).Trim(), SerialNumber = Encoding.ASCII.GetString(phdinfo.sSerialNumber).Trim(), Capacity = phdinfo.ulTotalAddressableSectors / 2 / 1024, BufferSize = phdinfo.wBufferSize / 1024 };
            return hdd;
        }

        /// <summary>
        /// ��ȡ��NTƽ̨��ָ�����кŵ�Ӳ����Ϣ��
        /// </summary>
        /// <param name="driveIndex">������̵�������</param>
        /// <returns></returns>
        internal static HDiskInfo GetHddInfoNT(byte driveIndex)
        {
            GetVersionOutParams vers = new();
            SendCmdInParams inParam = new();
            SendCmdOutParams outParam = new();
            uint bytesReturned = 0;

            // ʹ�� Win2000 �� Xp�µķ�����ȡӲ����Ϣ

            // ��ȡ�豸�ľ����
            IntPtr hDevice =
                CreateFile(string.Format(@"\\.\PhysicalDrive{0}", driveIndex), GENERIC_READ | GENERIC_WRITE,
                           FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

            // ��ʼ���
            if(hDevice == IntPtr.Zero)
                throw new UnauthorizedAccessException("ִ�� Win32 API ���� CreateFile ʧ�ܡ�");
            if(0 == DeviceIoControl(hDevice, SMART_GET_VERSION, IntPtr.Zero, 0, ref vers,
                (uint)Marshal.SizeOf(vers),
                ref bytesReturned,
                IntPtr.Zero))
            {
                CloseHandle(hDevice);
                throw new IOException(string.Format(ResourcesApi.Win32_DeviceIoControlErr, "SMART_GET_VERSION"));
            }
            // ���IDE���������Ƿ�֧��
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
                throw new UnauthorizedAccessException("�� smartvsd.vxd �ļ�ʧ�ܡ�");
            if(0 == DeviceIoControl(hDevice, SMART_GET_VERSION, 
                IntPtr.Zero, 0, 
                ref vers, (uint)Marshal.SizeOf(vers), ref bytesReturned, IntPtr.Zero))
            {
                CloseHandle(hDevice);
                throw new IOException(string.Format(ResourcesApi.Win32_DeviceIoControlErr, "SMART_GET_VERSION"));
            }
            // ��� IDE �ļ��������ʶ���ʧ��
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
                // ����IDEΪATAPI���ͣ��޷�����
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
            // �ر��ļ����
            CloseHandle(hDevice);
            ChangeByteOrder(outParam.bBuffer.sModelNumber);
            ChangeByteOrder(outParam.bBuffer.sSerialNumber);
            ChangeByteOrder(outParam.bBuffer.sFirmwareRev);
            return GetHardDiskInfo(outParam.bBuffer);
        }

        #endregion

        #region ����

        /// <summary>
        /// Win32 API ������ָʾ��ʹ�� <see cref="RemoveMenu"/> ����ʱָ��ʹ��������������ʹ��ID��
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