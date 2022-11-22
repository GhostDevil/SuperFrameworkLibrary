using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Net;
using static SuperFramework.WindowsAPI.APIStruct;
using static SuperFramework.WindowsAPI.Win32;
using SuperFramework.WindowsAPI;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace SuperFramework.SuperHardware
{
    /// <summary>
    /// 日期:2016-01-07
    /// 作者:不良帥
    /// 描述:硬件属性辅助类
    /// </summary>
    public static class HardwareInfo
    {
        /// <summary>
        /// Windows路径信息
        /// </summary>
        /// <param name="WinDir"></param>
        /// <param name="count"></param>
        [DllImport("kernel32")]
        public static extern void GetWindowsDirectory(StringBuilder WinDir, int count);
        /// <summary>
        /// 系统路径信息
        /// </summary>
        /// <param name="SysDir"></param>
        /// <param name="count"></param>
        [DllImport("kernel32")]
        public static extern void GetSystemDirectory(StringBuilder SysDir, int count);
       /// <summary>
       /// 系统信息
       /// </summary>
       /// <param name="cpuinfo"></param>
        [DllImport("kernel32")]
        public static extern void GetSystemInfo(ref CPU_INFO cpuinfo);
        /// <summary>
        /// 内存信息 
        /// </summary>
        /// <param name="meminfo"></param>
        [DllImport("kernel32")]
        public static extern void GlobalMemoryStatus(ref MEMORY_INFO meminfo);
        /// <summary>
        /// 系统时间信息 
        /// </summary>
        /// <param name="stinfo"></param>
        [DllImport("kernel32")]
        public static extern void GetSystemTime(ref SYSTEMTIME_INFO stinfo);

        
        #region 获取当前用户物理磁盘的性能信息
        /// <summary>
        /// 获取当前用户物理磁盘的性能信息
        /// </summary>
        /// <returns>一个 <see cref="HDiskInfo"/> 结构，它保存了物理硬盘的性能数据。</returns>
        public static HDiskInfo GetHddInformation()
        {
            currentOs = GetCurrentPlatform();
            switch (currentOs)
            {
                case (Platform.Windows95):
                case (Platform.Windows98):
                case (Platform.WindowsCE):
                case (Platform.WindowsNT351):
                case (Platform.WindowsNT40):
                default:
                    throw new PlatformNotSupportedException(string.Format(ResourcesApi.Win32_CurrentPlatformNotSupport, currentOs.ToString()));
                case (Platform.UnKnown):
                    throw new PlatformNotSupportedException(ResourcesApi.Win32_CurrentPlatformUnknown);
                case (Platform.Windows982ndEdition):
                case (Platform.WindowsME):
                    return GetHddInfo9X(0);
                case (Platform.Windows2000):
                case (Platform.WindowsXP):
                case (Platform.Windows2003):
                case (Platform.WindowsVista):
                    return GetHddInfoNT(0);
            }
        }
        #endregion

        #region 计算机种类
        /// <summary>
        /// 计算机种类
        /// </summary>
        public enum ChassisTypesValues : int
        {
            Other = 1,
            Unknown = 2,
            Desktop = 3,
            Low_Profile_Desktop = 4,
            Pizza_Box = 5,
            Mini_Tower = 6,
            Tower = 7,
            Portable = 8,
            Laptop = 9,
            Notebook = 10,
            Hand_Held = 11,
            Docking_Station = 12,
            All_in_One = 13,
            Sub_Notebook = 14,
            Space_Saving = 15,
            Lunch_Box = 16,
            Main_System_Chassis = 17,
            Expansion_Chassis = 18,
            SubChassis = 19,
            Bus_Expansion_Chassis = 20,
            Peripheral_Chassis = 21,
            Storage_Chassis = 22,
            Rack_Mount_Chassis = 23,
            Sealed_Case_PC = 24,
            INVALID_ENUM_VALUE = 0,
        }
        #endregion

        #region 获得本地计算机名称
        /// <summary>
        /// 得到本地计算机的名称
        /// </summary>
        /// <returns>返回计算机名称</returns>
        public static string GetHostName()
        {
            return Dns.GetHostName();
        }
        #endregion

        #region 获得CPU总数
        /// <summary>
        /// 获得CUP总数
        /// </summary>
        /// <returns>返回CPU个数</returns>
        public static int GetCpuCount()
        {
            return Environment.ProcessorCount;
        }
        #endregion

        #region 获得计算机种类
        /// <summary>
        /// 获得计算机种类
        /// </summary>
        /// <returns>计算机种类:台式机-Desktop，笔记本-NoteBook</returns>
        public static string GetComputerType()
        {
            ManagementClass mc = new("Win32_SystemEnclosure");
            string type = "";
            foreach (ManagementObject obj in mc.GetInstances())
            {
                foreach (ChassisTypesValues i in (short[])(obj["ChassisTypes"]))
                {
                    switch (i)
                    {
                        case ChassisTypesValues.Desktop:
                        case ChassisTypesValues.Low_Profile_Desktop:
                            type = "Desktop";
                            break;
                        case ChassisTypesValues.Portable:
                        case ChassisTypesValues.Laptop:
                        case ChassisTypesValues.Notebook:
                            type = "NoteBook";
                            break;
                    }
                }
            }
            mc.Dispose();
            return type;
        }
        #endregion

        #region 获得MAC地址
        /// <summary>
        /// 获得mac地址
        /// </summary>
        /// <returns>返回mac地址</returns>
        public static string GetMaxAddress()
        {
            try
            {
                ManagementClass mc = new("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                string mac = "";
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        mac = mo["MacAddress"].ToString();
                        break;
                    }
                }
                mc.Dispose();
                moc.Dispose();
                mc = null;
                moc = null;
                return mac;
            }
            catch { return "unkonw"; }
        }
        #endregion

        #region 获得Ip地址
        /// <summary>
        /// 获得Ip地址
        /// </summary>
        /// <returns>返回本机Ip地址</returns>
        public static string GetIpAddress()
        {
            try
            {
                ManagementClass mc = new("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                string ip = "";
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        Array ar = (Array)(mo.Properties["IpAddress"].Value);
                        ip = ar.GetValue(0).ToString();
                        break;
                    }
                }
                mc.Dispose();
                moc.Dispose();
                mc = null;
                moc = null;
                return ip;
            }
            catch { return "unknow"; }
        }
        #endregion

        #region 获的PC类型
        /// <summary>
        /// 获得pc类型
        /// </summary>
        /// <returns></returns>
        public static string GetSystemType()
        {
            try
            {
                ManagementClass mc = new("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                string type = "";
                foreach (ManagementObject mo in moc)
                    type = mo["SystemType"].ToString();
                mc.Dispose();
                moc.Dispose();
                mc = null;
                moc = null;
                return type;
            }
            catch { return "unknow"; }
        }
        #endregion

        #region 获得内存
        /// <summary>
        /// 获得物理内存（MB）
        /// </summary>
        /// <returns>返回物理内存大小（MB）</returns>
        public static double GetPhysicalMemory()
        {
            try
            {
                //ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                //ManagementObjectCollection moc = mc.GetInstances();
                //string memory = "";
                //foreach (ManagementObject mo in moc)
                //    memory = mo["TotalPhysicalMemory"].ToString();
                //mc.Dispose();
                //moc.Dispose();
                //mc = null;
                //moc = null;
                Microsoft.VisualBasic.Devices.ComputerInfo p = new();
                return double.Parse((p.TotalPhysicalMemory / 1024 / 1024).ToString("#0.00"));
            }
            catch (Exception) { return 0; }
        }
        /// <summary>
        /// 获得虚拟内存（MB）
        /// </summary>
        /// <returns>返回虚拟内存大小（MB）</returns>
        public static double GetVMemory()
        {
            try
            {
                //ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                //ManagementObjectCollection moc = mc.GetInstances();
                //string memory = "";
                //foreach (ManagementObject mo in moc)
                //{
                //    string str = "";
                //    foreach (var item in mo.Properties)
                //    {
                //        str += item.Name + ":" + item.Value + Environment.NewLine;
                //    }
                //    memory = mo["TotalMemory"].ToString();
                //}
                //mc.Dispose();
                //moc.Dispose();
                //mc = null;
                //moc = null;
                Microsoft.VisualBasic.Devices.ComputerInfo p = new();
             //   Console.WriteLine("全部物理内存：{0}",
             //ByteChangeToM(p.TotalPhysicalMemory) + "M");
             //   Console.WriteLine("全部虚拟内存：{0}",
             //       ByteChangeToM(p.TotalVirtualMemory) + "M");
                //Console.WriteLine("可用物理内存：{0}",
                //    ByteChangeToM(p.AvailablePhysicalMemory) + "M");
                //Console.WriteLine("可用虚拟内存：{0}",
                //    ByteChangeToM(p.AvailableVirtualMemory) + "M");
                return double.Parse((p.TotalVirtualMemory / 1024 / 1024).ToString("#0.00"));
            }
            catch (Exception) { return 0; }
        }
        #endregion

        #region 可用内存 
        /// <summary>
        /// 获取可用物理内存（MB）
        /// </summary>
        /// <returns>返回可用物理内存量</returns>

        public static double GetFreePhysicalMemory()
        {
            double availablebytes = 0;

            ManagementClass mc = new("Win32_OperatingSystem");
            foreach (ManagementObject mo in mc.GetInstances())
            {
                if (mo["FreePhysicalMemory"] != null)
                {
                    availablebytes = double.Parse((double.Parse(mo["FreePhysicalMemory"].ToString()) / 1024).ToString("#0.00"));
                }
            }
            mc.Dispose();
            return availablebytes;
        }
        #endregion

        #region 获得主机板属性
        /// <summary>
        /// 获得主机板属性
        /// </summary>
        /// <returns>主机板对象</returns>
        public static HardwareStruct.BaseBoardInfo GetBaseBoardInfos()
        {
            ManagementObjectSearcher mos = new("Select * From Win32_BaseBoard");
            HardwareStruct.BaseBoardInfo motherBoard = new();
            
            foreach (ManagementObject mo in mos.Get())
            {
                try
                {
                    string str = "";
                    foreach (var item in mo.Properties)
                    {
                        str += item.Name + ":" + item.Value + Environment.NewLine;
                    }
                    motherBoard.Manufacturer = mo.Properties["Manufacturer"].Value?.ToString();
                    motherBoard.Product = mo.Properties["Product"].Value?.ToString();
                    motherBoard.SerialNumber = mo.Properties["SerialNumber"].Value?.ToString();
                    motherBoard.StrInfo = str;
                    motherBoard.PoweredOn = bool.Parse(mo.Properties["PoweredOn"].Value.ToString());
                    motherBoard.HotSwappable = bool.Parse(mo.Properties["HotSwappable"].Value.ToString());
                    motherBoard.HostingBoard = bool.Parse(mo.Properties["HostingBoard"].Value.ToString());
                    motherBoard.Name = mo.Properties["Name"].Value?.ToString();
                    motherBoard.Description= mo.Properties["Description"].Value?.ToString();
                    motherBoard.Version= mo.Properties["Version"].Value?.ToString();
                    motherBoard.Removable = bool.Parse(mo.Properties["Removable"].Value.ToString());
                    motherBoard.Replaceable = bool.Parse(mo.Properties["Replaceable"].Value.ToString());
                     
                }
                catch (Exception) { }
                break;
            }
            mos.Dispose();
            return motherBoard;
        }
        #endregion

        #region 获得电脑存储容量
        /// <summary>
        /// 获得电脑存储容量
        /// </summary>
        /// <returns>返回电脑存储容量对象</returns>
        public static HardwareStruct.HDSpaceInfo GetHDSpace()
        {
            DriveInfo[] drivers = DriveInfo.GetDrives();
            double freeSpace = 0, Space = 0;
            foreach (DriveInfo temp in drivers)
            {
                //驱动器类型非光驱和移动设备
                if (temp.DriveType != DriveType.CDRom && temp.DriveType != DriveType.Removable)
                {
                    //得到硬盘总空间
                    freeSpace += double.Parse(temp.TotalSize.ToString()) / 1024 / 1024 / 1024;
                    //得到空用空间
                    Space += double.Parse(temp.TotalFreeSpace.ToString()) / 1024 / 1024 / 1024;
                }
            }
            return new HardwareStruct.HDSpaceInfo() { HdSurplusSpace = Space.ToString("#0.00"), HdTotalSpace = freeSpace.ToString("#0.00") };
        }
        #endregion

        #region 获得显示器
        /// <summary>
        /// 获取显示器显示信息
        /// </summary>
        /// <returns>返回p.x为宽度，p.y为高度</returns>
        public static List<HardwareStruct.ScreenInfo> GetScreenInfos()
        {
            System.Windows.Forms.Screen[] screens = System.Windows.Forms.Screen.AllScreens;
            List<HardwareStruct.ScreenInfo> screen = new();            
            foreach (var item in screens)
            {
                screen.Add(new HardwareStruct.ScreenInfo()
                {
                    BitsPerPixel = item.BitsPerPixel,
                    Bounds = item.Bounds,
                    DeviceName = item.DeviceName,
                    Primary = item.Primary,
                    WorkingArea = item.WorkingArea
                });
            }
            
            return screen;
        }
        /// <summary>
        /// 获得监视器属性
        /// </summary>
        /// <returns>监视器对象</returns>
        public static List<HardwareStruct.DesktopMonitorInfo> GetDesktopMonitorInfos()
        {
            ManagementClass mc = new("Win32_DesktopMonitor");
            ManagementObjectCollection moc = mc.GetInstances();
            List<HardwareStruct.DesktopMonitorInfo> monitorInfos = new();
            foreach (ManagementObject mo in mc.GetInstances())
            {
                try
                {
                    string str = "";
                    foreach (var item in mo.Properties)
                    {
                        str += item.Name + ":" + item.Value + Environment.NewLine;
                    }
                    float f= MonitorScaler(GetMonitorPhysicalSize(mo.Properties["PNPDeviceID"].Value?.ToString()));
                    monitorInfos.Add(new HardwareStruct.DesktopMonitorInfo()
                    {
                        Availability = mo.Properties["Availability"].Value?.ToString(),
                        Caption = mo.Properties["Caption"].Value?.ToString(),
                        Description = mo.Properties["Description"].Value?.ToString(),
                        DeviceID = mo.Properties["DeviceID"].Value?.ToString(),
                        Name = mo.Properties["Name"].Value?.ToString(),
                        Status = mo.Properties["Status"].Value?.ToString(),
                        SystemName = mo.Properties["SystemName"].Value?.ToString(),
                        Manufacturer = mo.Properties["MonitorManufacturer"].Value?.ToString(),
                        StrInfo = str,
                        PNPDeviceID = mo.Properties["PNPDeviceID"].Value?.ToString(),
                        Size = f
                    }) ;
                }
                catch (Exception) { }
                break;
            }
            moc.Dispose();
            mc.Dispose();
            return monitorInfos;
        }
        /// <summary>
        /// 获取监视器PnpDeviceId
        /// </summary>
        /// <returns></returns>
        public static  List<string> GetMonitorPnpDeviceId()
        {
            List<string> rt = new();
            using (ManagementClass mc = new("Win32_DesktopMonitor"))
            {
                using (ManagementObjectCollection moc = mc.GetInstances())
                {
                    foreach (var o in moc)
                    {
                        var each = (ManagementObject)o;
                        object obj = each.Properties["PNPDeviceID"].Value;
                        if (obj == null)
                            continue;

                        rt.Add(each.Properties["PNPDeviceID"].Value.ToString());
                    }
                }
            }

            return rt;
        }
        /// <summary>
        /// 获取edid数据
        /// </summary>
        /// <param name="monitorPnpDevId"></param>
        /// <returns></returns>
        public static  byte[] GetMonitorEdid(string monitorPnpDevId)
        {
            return (byte[])Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Enum\" + monitorPnpDevId + @"\Device Parameters", "EDID", Array.Empty<byte>());
        }

        /// <summary>
        /// 获取显示器物理尺寸(cm)
        /// </summary>
        /// <param name="monitorPnpDevId"></param>
        /// <returns></returns>
        public static System.Drawing.SizeF GetMonitorPhysicalSize(string monitorPnpDevId)
        {
            byte[] edid = GetMonitorEdid(monitorPnpDevId);
            if (edid.Length < 23)
                return System.Drawing.SizeF.Empty;

            return new System.Drawing.SizeF(edid[21], edid[22]);
        }

        /// <summary>
        /// 通过屏显示器理尺寸转换为显示器大小(inch)
        /// </summary>
        /// <param name="moniPhySize"></param>
        /// <returns></returns>
        public static float MonitorScaler(System.Drawing.SizeF moniPhySize)
        {
            double mDSize = Math.Sqrt(Math.Pow(moniPhySize.Width, 2) + Math.Pow(moniPhySize.Height, 2)) / 2.54d;
            return (float)Math.Round(mDSize, 1);
        }
        #endregion

        #region 获得基本属性
        /// <summary>
        /// 获得基本属性
        /// </summary>
        public static HardwareStruct.SystemInfo GetSystemInfos()
        {
            ManagementClass mc = new("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {


                string str = "";
                foreach (var item in mo.Properties)
                {
                    str += item.Name + ":" + item.Value + Environment.NewLine;
                }
               
            }

            mc.Dispose();
            moc.Dispose();
            return new HardwareStruct.SystemInfo()
            {
                ComputerName = GetHostName(),
                CpuInfos = GetProcessorInfos(),
                IpAddress = GetIpAddress(),
                MaxAddress = GetMaxAddress(),
                OSVersion = Environment.OSVersion.ToString(),
                Space = GetHDSpace(),
                SystemType = GetSystemType(),
                TotalPhysicalMemory = GetPhysicalMemory(),
                UserName = Environment.UserName,
                UserDomainName = Environment.UserDomainName,
                MachineName = Environment.MachineName,
                LogicalDrives = Environment.GetLogicalDrives(),
                Is64BitOperatingSystem = Environment.Is64BitOperatingSystem,
                ProcessorCount = Environment.ProcessorCount,
                SysStartTime = ComputerInfo.GetComputerStartTime(),
                SysRunTime = ComputerInfo.GetComputerRunTime(),
                SystemPageSize = Environment.SystemPageSize,
                WorkingSet = Environment.WorkingSet,
                CurrentDirectory = Environment.CurrentDirectory,
                SystemDirectory = Environment.SystemDirectory,
                ComputerType = GetComputerType(),
                Is64BitProcess = Environment.Is64BitProcess,
                TotalVMemory = GetVMemory()


            };
        }
        #endregion

        #region 获得BIOS属性
        /// <summary>
        /// 获得BIOS属性
        /// </summary>
        /// <returns>bios对象集合</returns>
        public static List<HardwareStruct.BIOSInfo> GetBIOSInfos()
        {
            ManagementClass mc = new("Win32_BIOS");
            ManagementObjectCollection moc = mc.GetInstances();
            List<HardwareStruct.BIOSInfo> biosInfos = new();
            foreach (ManagementObject mo in moc)
            {
                string str = "";
                foreach (var item in mo.Properties)
                {
                    str += item.Name + ":" + item.Value + Environment.NewLine;
                }
                biosInfos.Add(new HardwareStruct.BIOSInfo()
                {
                    Manufacturer = mo.Properties["Manufacturer"].Value?.ToString(),
                    Version = mo.Properties["Version"].Value?.ToString(),
                    Description = mo.Properties["Description"].Value?.ToString(),
                    SerialNumber = mo.Properties["SerialNumber"].Value?.ToString(),
                    SMBIOSBIOSVersion = mo.Properties["SMBIOSBIOSVersion"].Value?.ToString(),
                    CurrentLanguage = mo.Properties["CurrentLanguage"].Value?.ToString(),
                    ReleaseDate = mo.Properties["ReleaseDate"].Value?.ToString(),
                    StrInfo = str,
                    Caption= mo.Properties["Caption"].Value?.ToString(),
                });

            }
            mc.Dispose();
            moc.Dispose();
            return biosInfos;
        }

        #endregion

        #region 获得CPU属性
        /// <summary>
        /// 获得处理器属性的方法
        /// </summary>
        public static List<HardwareStruct.CPUInfo> GetProcessorInfos()
        {
            ManagementClass mc = new("Win32_Processor");
            ManagementObjectCollection moc = mc.GetInstances();
            List<HardwareStruct.CPUInfo> cpus = new();
            foreach (ManagementObject mo in moc)
            {
                string str = "";
                foreach (var item in mo.Properties)
                {
                    str += item.Name + ":" + item.Value + Environment.NewLine;
                }
                cpus.Add(new HardwareStruct.CPUInfo()
                {
                    AddressWidth = mo.Properties["AddressWidth"].Value?.ToString(),
                    CurrentClockSpeed = int.Parse(mo.Properties["currentClockSpeed"].Value.ToString()),
                    Description = mo.Properties["Description"].Value?.ToString(),
                    ExtClock = mo.Properties["ExtClock"].Value?.ToString(),
                    Manufacturer = mo.Properties["Manufacturer"].Value?.ToString(),
                    ProcessorId = mo.Properties["ProcessorID"].Value?.ToString(),
                    Caption = mo.Properties["Caption"].Value?.ToString(),
                    DeviceID = mo.Properties["DeviceID"].Value?.ToString(),
                    Name = mo.Properties["Name"].Value?.ToString(),
                    ThreadCount = int.Parse(mo.Properties["ThreadCount"].Value.ToString()),
                    MaxClockSpeed = int.Parse(mo.Properties["MaxClockSpeed"].Value.ToString()),
                    NumberOfCores = int.Parse(mo.Properties["NumberOfCores"].Value.ToString()),
                    StrInfo = str,
                    Architecture = mo.Properties["Name"].Value?.ToString(),
                    DataWidth = int.Parse(mo.Properties["DataWidth"].Value.ToString()),
                    SystemName = mo.Properties["SystemName"].Value?.ToString(),
                    Availability = mo.Properties["Availability"].Value?.ToString(),
                    SerialNumber = mo.Properties["SerialNumber"].Value?.ToString(),
                    Status = mo.Properties["SerialNumber"].Value?.ToString(),
                });
            }
            mc.Dispose();
            moc.Dispose();
            return cpus;
        }
        #endregion

        #region 获得物理媒体属性
        /// <summary>
        /// 获得物理媒体属性方法
        /// </summary>
        public static List<HardwareStruct.PhysicalMediaInfos> GetPhysicalMediaInfos()
        {
            ManagementClass mc = new("Win32_PhysicalMedia");
            ManagementObjectCollection moc = mc.GetInstances();
            List<HardwareStruct.PhysicalMediaInfos> info = new();
            foreach (ManagementObject mo in moc)
            {
                string str = "";
                foreach (var item in mo.Properties)
                {
                    str += item.Name + ":" + item.Value + Environment.NewLine;
                }
                info.Add(new HardwareStruct.PhysicalMediaInfos()
                {
                    
                    Description = mo.Properties["Description"].Value?.ToString(),
                    Manufacturer = mo.Properties["Manufacturer"].Value?.ToString(),
                    Caption = mo.Properties["Caption"].Value?.ToString(),
                    Name = mo.Properties["Name"].Value?.ToString(),
                    StrInfo = str,                    
                    Status = mo.Properties["Status"].Value?.ToString(),                   
                    HotSwappable = mo.Properties["HotSwappable"].Value,
                    OtherIdentifyingInfo = mo.Properties["OtherIdentifyingInfo"].Value,
                    PartNumber = mo.Properties["PartNumber"].Value,
                    PoweredOn = mo.Properties["PoweredOn"].Value,
                    Removable = mo.Properties["Removable"].Value,
                    Replaceable = mo.Properties["Replaceable"].Value,
                    Version = mo.Properties["Version"].Value,
                });
            }
            mc.Dispose();
            moc.Dispose();
            return info;
        }
        #endregion
        #region 获取声卡属性
        /// <summary>
        /// 获得声卡属性的方法
        /// </summary>
        public static List<HardwareStruct.SoundInfo> GetSoundInfos()
        {
            ManagementClass mc = new("Win32_SoundDevice");
            ManagementObjectCollection moc = mc.GetInstances();
            List<HardwareStruct.SoundInfo> infos = new();
            foreach (ManagementObject mo in moc)
            {
                string str = "";
                foreach (var item in mo.Properties)
                {
                    str += item.Name + ":" + item.Value + Environment.NewLine;
                }
                infos.Add(new HardwareStruct.SoundInfo()
                {
                    ProductName = mo.Properties["ProductName"].Value?.ToString(),
                    Status = mo.Properties["Status"].Value?.ToString(),
                    Description = mo.Properties["Description"].Value?.ToString(),
                    PNPDeviceID= mo.Properties["PNPDeviceID"].Value?.ToString(),                     
                    Manufacturer = mo.Properties["Manufacturer"].Value?.ToString(),                    
                    Caption = mo.Properties["Caption"].Value?.ToString(),
                    DeviceID = mo.Properties["DeviceID"].Value?.ToString(),
                    Name = mo.Properties["Name"].Value?.ToString(),                    
                    StrInfo = str, 
                    SystemName= mo.Properties["SystemName"].Value?.ToString(),
                });
            }
            mc.Dispose();
            moc.Dispose();
            return infos;
        }
        #endregion

        #region 获取网卡属性
        /// <summary>
        /// 获得网卡属性的方法
        /// </summary>
        public static List<HardwareStruct.NetworkAdapterInfo> GetNetworkAdapterInfos()
        {
            ManagementClass mc = new("Win32_NetworkAdapter");
            ManagementObjectCollection moc = mc.GetInstances();
            List<HardwareStruct.NetworkAdapterInfo> infos = new();
            foreach (ManagementObject mo in moc)
            {
                string str = "";
                foreach (var item in mo.Properties)
                {
                    str += item.Name + ":" + item.Value + Environment.NewLine;
                }
                infos.Add(new HardwareStruct.NetworkAdapterInfo()
                {
                    ProductName = mo.Properties["ProductName"].Value?.ToString(),
                    Description = mo.Properties["Description"].Value?.ToString(),
                    PNPDeviceID = mo.Properties["PNPDeviceID"].Value?.ToString(),
                    Manufacturer = mo.Properties["Manufacturer"].Value?.ToString(),
                    Caption = mo.Properties["Caption"].Value?.ToString(),
                    DeviceID = mo.Properties["DeviceID"].Value?.ToString(),
                    Name = mo.Properties["Name"].Value?.ToString(),
                    StrInfo = str,
                     Availability= mo.Properties["Availability"].Value?.ToString(),
                     Installed = mo.Properties["Installed"].Value?.ToString(),
                     InterfaceIndex = mo.Properties["InterfaceIndex"].Value?.ToString(),
                     PhysicalAdapter = mo.Properties["PhysicalAdapter"].Value?.ToString(),
                     ServiceName = mo.Properties["ServiceName"].Value?.ToString(),
                     SystemName = mo.Properties["SystemName"].Value?.ToString(),
                });
            }
            mc.Dispose();
            moc.Dispose();
            return infos;
        }
        #endregion

        #region 获取基本属性
        /// <summary>
        /// 获得基本属性的方法
        /// </summary>
        public static List<HardwareStruct.DeviceBaseInfos> GetDeviceBaseInfos(string type)
        {
            ManagementClass mc = new(type);
            ManagementObjectCollection moc = mc.GetInstances();
            List<HardwareStruct.DeviceBaseInfos> infos = new();
            foreach (ManagementObject mo in moc)
            {
                string str = "";
                foreach (var item in mo.Properties)
                {
                    str += item.Name + ":" + item.Value + Environment.NewLine;
                }
                infos.Add(new HardwareStruct.DeviceBaseInfos()
                {                   
                    Status = mo.Properties["Status"].Value?.ToString(),
                    Description = mo.Properties["Description"].Value?.ToString(), 
                    Manufacturer = mo.Properties["Manufacturer"].Value?.ToString(),
                    Caption = mo.Properties["Caption"].Value?.ToString(),
                    DeviceID = mo.Properties["DeviceID"].Value?.ToString(),
                    Name = mo.Properties["Name"].Value?.ToString(),
                    StrInfo = str,
                    Availability = mo.Properties["Availability"].Value?.ToString(), 
                    SystemName = mo.Properties["SystemName"].Value?.ToString(),
                });
            }
            mc.Dispose();
            moc.Dispose();
            return infos;
        }
        #endregion

        #region 获得硬盘属性
        /// <summary>
        /// 获得硬盘属性的方法
        /// </summary>
        public static List<HardwareStruct.HDInfo> GetHDInfos()
        {
            ManagementClass mc = new("Win32_DiskDrive");
            ManagementObjectCollection moc = mc.GetInstances();
            List<HardwareStruct.HDInfo> hdinfos = new();
            foreach (ManagementObject mo in moc)
            {
                string str = "";
                foreach (var item in mo.Properties)
                {
                    str += item.Name + ":" + item.Value + Environment.NewLine;
                }
                hdinfos.Add(new HardwareStruct.HDInfo()
                {
                    BytesPerSector = mo.Properties["BytesPerSector"].Value?.ToString(),
                    Manufacturer = mo.Properties["Manufacturer"].Value?.ToString(),
                    Model = mo.Properties["Model"].Value?.ToString(),
                    Partitions = mo.Properties["Partitions"].Value?.ToString(),
                    PNPDeviceID = mo.Properties["PNPDeviceID"].Value?.ToString(),
                    SectorsPerTrack = mo.Properties["SectorsPerTrack"].Value?.ToString(),
                    Size = mo.Properties["Size"].Value?.ToString(),
                    TotalCylinders = mo.Properties["TotalCylinders"].Value?.ToString(),
                    TracksPerCylinder = mo.Properties["TracksPerCylinder"].Value?.ToString(),
                    TotalHeads = mo.Properties["TotalHeads"].Value?.ToString(),
                    TotalSectors = mo.Properties["TotalSectors"].Value?.ToString(),
                    TotalTracks = mo.Properties["TotalTracks"].Value?.ToString(),
                    Caption = mo.Properties["Caption"].Value?.ToString(),
                    Description = mo.Properties["Description"].Value?.ToString(),
                    DeviceID = mo.Properties["DeviceID"].Value?.ToString(),
                    Index = int.Parse(mo.Properties["Index"].Value.ToString()),
                    FirmwareRevision = mo.Properties["FirmwareRevision"].Value?.ToString(),
                    Name = mo.Properties["Name"].Value?.ToString(),
                    SerialNumber = mo.Properties["SerialNumber"].Value?.ToString(),
                    InterfaceType = mo.Properties["InterfaceType"].Value?.ToString(),
                    Signature = mo.Properties["Signature"].Value?.ToString(),
                    SystemName = mo.Properties["SystemName"].Value?.ToString(),
                    Status = mo.Properties["Status"].Value?.ToString(),
                    StrInfo = str,
                     Capabilities= mo.Properties["Capabilities"].Value,
                     CapabilityDescriptions = mo.Properties["CapabilityDescriptions"].Value,
                }) ;
            }
            mc.Dispose();
            moc.Dispose();
            return hdinfos;
        }
        #endregion

        #region 获得视频控制器属性
        /// <summary>
        /// 获得视频控制器属性的方法
        /// </summary>
        public static List<HardwareStruct.VideoControllerInfo> GetVideoControllerInfos()
        {
            ManagementClass mc = new("Win32_VideoController");
            ManagementObjectCollection moc = mc.GetInstances();
            List<HardwareStruct.VideoControllerInfo> videocards = new();
            foreach (ManagementObject mo in moc)
            {

                string str = "";
                foreach (var item in mo.Properties)
                {
                    str += item.Name + ":" + item.Value + Environment.NewLine;
                }
                videocards.Add(new HardwareStruct.VideoControllerInfo()
                {
                    Name = mo.Properties["Name"].Value?.ToString(),
                    Caption = mo.Properties["Caption"].Value?.ToString(),
                    Description = mo.Properties["Description"].Value?.ToString(),
                    StrInfo = str,
                    AdapterCompatibility = mo.Properties["AdapterCompatibility"].Value?.ToString(),
                    AdapterDACType = mo.Properties["AdapterDACType"].Value?.ToString(),
                    Availability = mo.Properties["Availability"].Value?.ToString(),
                    DeviceID = mo.Properties["DeviceID"].Value?.ToString(),
                    DriverVersion = mo.Properties["DriverVersion"].Value?.ToString(),
                    Monochrome = mo.Properties["Monochrome"].Value?.ToString(),
                    VideoProcessor = mo.Properties["VideoProcessor"].Value?.ToString(),
                    SystemName = mo.Properties["SystemName"].Value?.ToString(),
                    VideoModeDescription = mo.Properties["VideoModeDescription"].Value?.ToString(),
                    Status = mo.Properties["Status"].Value?.ToString(),
                    CurrentBitsPerPixel = int.Parse(mo.Properties["CurrentBitsPerPixel"].Value.ToString()),
                    CurrentHorizontalResolution = int.Parse(mo.Properties["CurrentHorizontalResolution"].Value.ToString()),
                    CurrentRefreshRate = int.Parse(mo.Properties["CurrentRefreshRate"].Value.ToString()),
                    CurrentVerticalResolution = int.Parse(mo.Properties["CurrentVerticalResolution"].Value.ToString()),
                    MaxRefreshRate = int.Parse(mo.Properties["MaxRefreshRate"].Value.ToString()),
                    MinRefreshRate = int.Parse(mo.Properties["MinRefreshRate"].Value.ToString()),
                    AdapterRAM = long.Parse(mo.Properties["AdapterRAM"].Value.ToString()),
                    DriverDate = mo.Properties["DriverDate"].Value?.ToString(),
                    PNPDeviceID = mo.Properties["PNPDeviceID"].Value?.ToString(),
                });
            }
            mc.Dispose();
            moc.Dispose();
            return videocards;
        }
        #endregion

        #region 获得内存属性
        /// <summary>
        /// 获得内存属性
        /// </summary>
        public static List<HardwareStruct.PhysicalMemoryInfo> GetPhysicalMemoryInfos()
        {
            ManagementClass mc = new("Win32_PhysicalMemory");
            ManagementObjectCollection moc = mc.GetInstances();
            List<HardwareStruct.PhysicalMemoryInfo> memorys = new();//
            try
            {
                foreach (ManagementObject mo in moc)
                {
                    string str = "";
                    foreach (var item in mo.Properties)
                    {
                        str += item.Name + ":" + item.Value + Environment.NewLine;
                    }
                    memorys.Add(new HardwareStruct.PhysicalMemoryInfo()
                    {
                        Name = mo.Properties["Name"].Value?.ToString(),
                        PartNumber = mo.Properties["PartNumber"].Value?.ToString(),
                        TotalWidth = mo.Properties["TotalWidth"].Value?.ToString(),
                        Capacity = int.Parse((Convert.ToDouble(mo.Properties["Capacity"].Value) / 1024 / 1024 / 1024).ToString("#")),
                        Caption = mo.Properties["Caption"].Value?.ToString(),
                        Description = mo.Properties["Description"].Value?.ToString(),
                        PositionInRow = mo.Properties["PositionInRow"].Value?.ToString(),
                        SMBIOSMemoryType = mo.Properties["SMBIOSMemoryType"].Value?.ToString(),
                        Manufacturer = mo.Properties["Manufacturer"].Value?.ToString(),
                        SerialNumber = mo.Properties["SerialNumber"].Value?.ToString(),
                        DataWidth = int.Parse(mo.Properties["DataWidth"].Value.ToString()),
                        Speed = long.Parse(mo.Properties["Speed"].Value.ToString()),
                        TypeDetail = long.Parse(mo.Properties["TypeDetail"].Value.ToString()),
                        StrInfo = str,
                    });
                }
            }
            catch (Exception) { }
            mc.Dispose();
            moc.Dispose();
            return memorys;
        }
        #endregion

        #region 获得本地电脑上的usb盘盘符及卷标
        /// <summary>
        /// 获得本地电脑上的usb盘盘符及卷标
        /// </summary>
        /// <returns>返回USB盘的盘符及卷标集合</returns>
        public static List<string> GetUsbDisk()
        {
            List<string> usbDiskNameList = new();
            try
            {

                ManagementObjectCollection drives = new ManagementObjectSearcher("select * from Win32_DiskDrive where interfaceType = 'USB'").Get();

                foreach (ManagementObject drive in drives)
                {
                    ManagementObjectCollection partitions = new ManagementObjectSearcher("ASSOCIATORS OF {Win32_DiskDrive.DeviceID='" + drive["DeviceID"] + "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition").Get();

                    foreach (ManagementObject partition in partitions)
                    {
                        ManagementObjectCollection volumeLetters = new ManagementObjectSearcher("ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" + partition["DeviceID"] + "'} WHERE AssocClass = Win32_LogicalDiskToPartition").Get();

                        foreach (ManagementObject volumeLette in volumeLetters)
                        {
                            string usbDiskName = string.Format(@"{0}\{1}", volumeLette["Name"].ToString(), new DriveInfo(volumeLette["Name"].ToString()).VolumeLabel);

                            usbDiskNameList.Add(usbDiskName);
                        }
                        volumeLetters.Dispose();
                    }
                    partitions.Dispose();
                }
                drives.Dispose();
            }
            catch (Exception)
            {
            }

            return usbDiskNameList;
        }
        #endregion

        #region 得到所有磁盘盘符及卷标
        /// <summary>
        /// 得到所有磁盘盘符及卷标
        /// </summary>
        /// <returns>返回所有盘符及卷标集合</returns>
        public static List<string> GetPartitionsPath()
        {
            DriveInfo[] drivers = DriveInfo.GetDrives();
            List<string> path = new();
            foreach (DriveInfo temp in drivers)
            {
                DirectoryInfo dif = new(temp.Name);
                string s;
                if (temp.DriveType != DriveType.CDRom)
                    //卷标-VolumeLabel TotalFreeSpace驱动器的剩余空间，TotalSize驱动器的总空间
                    s = temp.VolumeLabel + "(" + temp.DriveFormat + "," + (double.Parse(temp.TotalFreeSpace.ToString()) / 1024 / 1024 / 1024).ToString("#0.00") + "GB/"
                        + (double.Parse(temp.TotalSize.ToString()) / 1024 / 1024 / 1024).ToString("#0.00") + "GB)";
                else
                    s = temp.Name;
                path.Add(s);
            }
            return path;
        }
        #endregion

        #region 查询语句
        /// <summary>
        /// 查询语句
        /// </summary>
        enum Informations
        {
            Win32_1394Controller,
            Win32_1394ControllerDevice,
            Win32_AccountSID,
            Win32_ActionCheck,
            Win32_ActiveRoute,
            Win32_AllocatedResource,
            Win32_ApplicationCommandLine,
            Win32_ApplicationService,
            Win32_AssociatedBattery,
            Win32_AssociatedProcessorMemory,
            Win32_AutochkSetting,
            Win32_BaseBoard,
            Win32_Battery,
            Win32_Binary,
            Win32_BindImageAction,
            Win32_BIOS,
            Win32_BootConfiguration,
            Win32_Bus,
            Win32_CacheMemory,
            Win32_CDROMDrive,
            Win32_CheckCheck,
            Win32_CIMLogicalDeviceCIMDataFile,
            Win32_ClassicCOMApplicationClasses,
            Win32_ClassicCOMClass,
            Win32_ClassicCOMClassSetting,
            Win32_ClassicCOMClassSettings,
            Win32_ClassInforAction,
            Win32_ClientApplicationSetting,
            Win32_CodecFile,
            Win32_COMApplicationSettings,
            Win32_COMClassAutoEmulator,
            Win32_ComClassEmulator,
            Win32_CommandLineAccess,
            Win32_ComponentCategory,
            Win32_ComputerSystem,
            Win32_ComputerSystemProcessor,
            Win32_ComputerSystemProduct,
            Win32_ComputerSystemWindowsProductActivationSetting,
            Win32_Condition,
            Win32_ConnectionShare,
            Win32_ControllerHastHub,
            Win32_CreateFolderAction,
            Win32_CurrentProbe,
            Win32_DCOMApplication,
            Win32_DCOMApplicationAccessAllowedSetting,
            Win32_DCOMApplicationLaunchAllowedSetting,
            Win32_DCOMApplicationSetting,
            Win32_DependentService,
            Win32_Desktop,
            Win32_DesktopMonitor,
            Win32_DeviceBus,
            Win32_DeviceMemoryAddress,
            Win32_Directory,
            Win32_DirectorySpecification,
            Win32_DiskDrive,
            Win32_DiskDrivePhysicalMedia,
            Win32_DiskDriveToDiskPartition,
            Win32_DiskPartition,
            Win32_DiskQuota,
            Win32_DisplayConfiguration,
            Win32_DisplayControllerConfiguration,
            Win32_DMAChanner,
            Win32_DriverForDevice,
            Win32_DriverVXD,
            Win32_DuplicateFileAction,
            Win32_Environment,
            Win32_EnvironmentSpecification,
            Win32_ExtensionInfoAction,
            Win32_Fan,
            Win32_FileSpecification,
            Win32_FloppyController,
            Win32_FloppyDrive,
            Win32_FontInfoAction,
            Win32_Group,
            Win32_GroupDomain,
            Win32_GroupUser,
            Win32_HeatPipe,
            Win32_IDEController,
            Win32_IDEControllerDevice,
            Win32_ImplementedCategory,
            Win32_InfraredDevice,
            Win32_IniFileSpecification,
            Win32_InstalledSoftwareElement,
            Win32_IP4PersistedRouteTable,
            Win32_IP4RouteTable,
            Win32_IRQResource,
            Win32_Keyboard,
            Win32_LaunchCondition,
            Win32_LoadOrderGroup,
            Win32_LoadOrderGroupServiceDependencies,
            Win32_LoadOrderGroupServiceMembers,
            Win32_LocalTime,
            Win32_LoggedOnUser,
            Win32_LogicalDisk,
            Win32_LogicalDiskRootDirectory,
            Win32_LogicalDiskToPartition,
            Win32_LogicalFileAccess,
            Win32_LogicalFileAuditing,
            Win32_LogicalFileGroup,
            Win32_LogicalFileOwner,
            Win32_LogicalFileSecuritySetting,
            Win32_LogicalMemoryConfiguration,
            Win32_LogicalProgramGroup,
            Win32_LogicalProgramGroupDirectory,
            Win32_LogicalProgramGroupItem,
            Win32_LogicalProgramGroupItemDataFile,
            Win32_LogicalShareAccess,
            Win32_LogicalShareAuditing,
            Win32_LogicalShareSecuritySetting,
            Win32_LogonSession,
            Win32_LogonSessionMappedDisk,
            Win32_MappedLogicalDisk,
            Win32_MemoryArray,
            Win32_MemoryArrayLocation,
            Win32_MemoryDevice,
            Win32_MemoryDeviceArray,
            Win32_MemoryDeviceLocation,
            Win32_MIMEInfoAction,
            Win32_MotherboardDevice,
            Win32_MoveFileAction,
            Win32_NamedJobObject,
            Win32_NamedJobObjectActgInfo,
            Win32_NamedJobObjectLimit,
            Win32_NamedJobObjectLimitSetting,
            Win32_NamedJobObjectProcess,
            Win32_NamedJobObjectSecLimit,
            Win32_NamedJobObjectSecLimitSetting,
            Win32_NamedJobObjectStatistics,
            Win32_NetworkAdapter,
            Win32_NetworkAdapterConfiguration,
            Win32_NetworkAdapterSetting,
            Win32_NetworkClient,
            Win32_NetworkConnection,
            Win32_NetworkLoginProfile,
            Win32_NetworkProtocol,
            Win32_NTDomain,
            Win32_NTEventlogFile,
            Win32_NTLogEvent,
            Win32_NTLogEventComputer,
            Win32_NTLogEvnetLog,
            Win32_NTLogEventUser,
            Win32_ODBCAttribute,
            Win32_ODBCDataSourceAttribute,
            Win32_ODBCDataSourceSpecification,
            Win32_ODBCDriverAttribute,
            Win32_ODBCDriverSoftwareElement,
            Win32_ODBCDriverSpecification,
            Win32_ODBCSourceAttribute,
            Win32_ODBCTranslatorSpecification,
            Win32_OnBoardDevice,
            Win32_OperatingSystem,
            Win32_OperatingSystemAutochkSetting,
            Win32_OperatingSystemQFE,
            Win32_OSRecoveryConfiguración,
            Win32_PageFile,
            Win32_PageFileElementSetting,
            Win32_PageFileSetting,
            Win32_PageFileUsage,
            Win32_ParallelPort,
            Win32_Patch,
            Win32_PatchFile,
            Win32_PatchPackage,
            Win32_PCMCIAControler,
            Win32_PerfFormattedData_ASP_ActiveServerPages,
            Win32_PerfFormattedData_ASPNET_114322_ASPNETAppsv114322,
            Win32_PerfFormattedData_ASPNET_114322_ASPNETv114322,
            Win32_PerfFormattedData_ASPNET_2040607_ASPNETAppsv2040607,
            Win32_PerfFormattedData_ASPNET_2040607_ASPNETv2040607,
            Win32_PerfFormattedData_ASPNET_ASPNET,
            Win32_PerfFormattedData_ASPNET_ASPNETApplications,
            Win32_PerfFormattedData_aspnet_state_ASPNETStateService,
            Win32_PerfFormattedData_ContentFilter_IndexingServiceFilter,
            Win32_PerfFormattedData_ContentIndex_IndexingService,
            Win32_PerfFormattedData_DTSPipeline_SQLServerDTSPipeline,
            Win32_PerfFormattedData_Fax_FaxServices,
            Win32_PerfFormattedData_InetInfo_InternetInformationServicesGlobal,
            Win32_PerfFormattedData_ISAPISearch_HttpIndexingService,
            Win32_PerfFormattedData_MSDTC_DistributedTransactionCoordinator,
            Win32_PerfFormattedData_NETCLRData_NETCLRData,
            Win32_PerfFormattedData_NETCLRNetworking_NETCLRNetworking,
            Win32_PerfFormattedData_NETDataProviderforOracle_NETCLRData,
            Win32_PerfFormattedData_NETDataProviderforSqlServer_NETDataProviderforSqlServer,
            Win32_PerfFormattedData_NETFramework_NETCLRExceptions,
            Win32_PerfFormattedData_NETFramework_NETCLRInterop,
            Win32_PerfFormattedData_NETFramework_NETCLRJit,
            Win32_PerfFormattedData_NETFramework_NETCLRLoading,
            Win32_PerfFormattedData_NETFramework_NETCLRLocksAndThreads,
            Win32_PerfFormattedData_NETFramework_NETCLRMemory,
            Win32_PerfFormattedData_NETFramework_NETCLRRemoting,
            Win32_PerfFormattedData_NETFramework_NETCLRSecurity,
            Win32_PerfFormattedData_NTFSDRV_ControladordealmacenamientoNTFSdeSMTP,
            Win32_PerfFormattedData_Outlook_Outlook,
            Win32_PerfFormattedData_PerfDisk_LogicalDisk,
            Win32_PerfFormattedData_PerfDisk_PhysicalDisk,
            Win32_PerfFormattedData_PerfNet_Browser,
            Win32_PerfFormattedData_PerfNet_Redirector,
            Win32_PerfFormattedData_PerfNet_Server,
            Win32_PerfFormattedData_PerfNet_ServerWorkQueues,
            Win32_PerfFormattedData_PerfOS_Cache,
            Win32_PerfFormattedData_PerfOS_Memory,
            Win32_PerfFormattedData_PerfOS_Objects,
            Win32_PerfFormattedData_PerfOS_PagingFile,
            Win32_PerfFormattedData_PerfOS_Processor,
            Win32_PerfFormattedData_PerfOS_System,
            Win32_PerfFormattedData_PerfProc_FullImage_Costly,
            Win32_PerfFormattedData_PerfProc_Image_Costly,
            Win32_PerfFormattedData_PerfProc_JobObject,
            Win32_PerfFormattedData_PerfProc_JobObjectDetails,
            Win32_PerfFormattedData_PerfProc_Process,
            Win32_PerfFormattedData_PerfProc_ProcessAddressSpace_Costly,
            Win32_PerfFormattedData_PerfProc_Thread,
            Win32_PerfFormattedData_PerfProc_ThreadDetails_Costly,
            Win32_PerfFormattedData_RemoteAccess_RASPort,
            Win32_PerfFormattedData_RemoteAccess_RASTotal,
            Win32_PerfFormattedData_RSVP_RSVPInterfaces,
            Win32_PerfFormattedData_RSVP_RSVPService,
            Win32_PerfFormattedData_Spooler_PrintQueue,
            Win32_PerfFormattedData_TapiSrv_Telephony,
            Win32_PerfFormattedData_Tcpip_ICMP,
            Win32_PerfFormattedData_Tcpip_IP,
            Win32_PerfFormattedData_Tcpip_NBTConnection,
            Win32_PerfFormattedData_Tcpip_NetworkInterface,
            Win32_PerfFormattedData_Tcpip_TCP,
            Win32_PerfFormattedData_Tcpip_UDP,
            Win32_PerfFormattedData_TermService_TerminalServices,
            Win32_PerfFormattedData_TermService_TerminalServicesSession,
            Win32_PerfFormattedData_W3SVC_WebService,
            Win32_PerfRawData_ASP_ActiveServerPages,
            Win32_PerfRawData_ASPNET_114322_ASPNETAppsv114322,
            Win32_PerfRawData_ASPNET_114322_ASPNETv114322,
            Win32_PerfRawData_ASPNET_2040607_ASPNETAppsv2040607,
            Win32_PerfRawData_ASPNET_2040607_ASPNETv2040607,
            Win32_PerfRawData_ASPNET_ASPNET,
            Win32_PerfRawData_ASPNET_ASPNETApplications,
            Win32_PerfRawData_aspnet_state_ASPNETStateService,
            Win32_PerfRawData_ContentFilter_IndexingServiceFilter,
            Win32_PerfRawData_ContentIndex_IndexingService,
            Win32_PerfRawData_DTSPipeline_SQLServerDTSPipeline,
            Win32_PerfRawData_Fax_FaxServices,
            Win32_PerfRawData_InetInfo_InternetInformationServicesGlobal,
            Win32_PerfRawData_ISAPISearch_HttpIndexingService,
            Win32_PerfRawData_MSDTC_DistributedTransactionCoordinator,
            Win32_PerfRawData_NETCLRData_NETCLRData,
            Win32_PerfRawData_NETCLRNetworking_NETCLRNetworking,
            Win32_PerfRawData_NETDataProviderforOracle_NETCLRData,
            Win32_PerfRawData_NETDataProviderforSqlServer_NETDataProviderforSqlServer,
            Win32_PerfRawData_NETFramework_NETCLRExceptions,
            Win32_PerfRawData_NETFramework_NETCLRInterop,
            Win32_PerfRawData_NETFramework_NETCLRJit,
            Win32_PerfRawData_NETFramework_NETCLRLoading,
            Win32_PerfRawData_NETFramework_NETCLRLocksAndThreads,
            Win32_PerfRawData_NETFramework_NETCLRMemory,
            Win32_PerfRawData_NETFramework_NETCLRRemoting,
            Win32_PerfRawData_NETFramework_NETCLRSecurity,
            Win32_PerfRawData_NTFSDRV_ControladordealmacenamientoNTFSdeSMTP,
            Win32_PerfRawData_Outlook_Outlook,
            Win32_PerfRawData_PerfDisk_LogicalDisk,
            Win32_PerfRawData_PerfDisk_PhysicalDisk,
            Win32_PerfRawData_PerfNet_Browser,
            Win32_PerfRawData_PerfNet_Redirector,
            Win32_PerfRawData_PerfNet_Server,
            Win32_PerfRawData_PerfNet_ServerWorkQueues,
            Win32_PerfRawData_PerfOS_Cache,
            Win32_PerfRawData_PerfOS_Memory,
            Win32_PerfRawData_PerfOS_Objects,
            Win32_PerfRawData_PerfOS_PagingFile,
            Win32_PerfRawData_PerfOS_Processor,
            Win32_PerfRawData_PerfOS_System,
            Win32_PerfRawData_PerfProc_FullImage_Costly,
            Win32_PerfRawData_PerfProc_Image_Costly,
            Win32_PerfRawData_PerfProc_JobObject,
            Win32_PerfRawData_PerfProc_JobObjectDetails,
            Win32_PerfRawData_PerfProc_Process,
            Win32_PerfRawData_PerfProc_ProcessAddressSpace_Costly,
            Win32_PerfRawData_PerfProc_Thread,
            Win32_PerfRawData_PerfProc_ThreadDetails_Costly,
            Win32_PerfRawData_RemoteAccess_RASPort,
            Win32_PerfRawData_RemoteAccess_RASTotal,
            Win32_PerfRawData_RSVP_RSVPInterfaces,
            Win32_PerfRawData_RSVP_RSVPService,
            Win32_PerfRawData_Spooler_PrintQueue,
            Win32_PerfRawData_TapiSrv_Telephony,
            Win32_PerfRawData_Tcpip_ICMP,
            Win32_PerfRawData_Tcpip_IP,
            Win32_PerfRawData_Tcpip_NBTConnection,
            Win32_PerfRawData_Tcpip_NetworkInterface,
            Win32_PerfRawData_Tcpip_TCP,
            Win32_PerfRawData_Tcpip_UDP,
            Win32_PerfRawData_TermService_TerminalServices,
            Win32_PerfRawData_TermService_TerminalServicesSession,
            Win32_PerfRawData_W3SVC_WebService,
            Win32_PhysicalMedia,
            Win32_PhysicalMemory,
            Win32_PhysicalMemoryArray,
            Win32_PhysicalMemoryLocation,
            Win32_PingStatus,
            Win32_PNPAllocatedResource,
            Win32_PnPDevice,
            Win32_PnPEntity,
            Win32_PnPSignedDriver,
            Win32_PnPSignedDriverCIMDataFile,
            Win32_PointingDevice,
            Win32_PortableBattery,
            Win32_PortConnector,
            Win32_PortResource,
            Win32_POTSModem,
            Win32_POTSModemToSerialPort,
            Win32_Printer,
            Win32_PrinterConfiguration,
            Win32_PrinterController,
            Win32_PrinterDriver,
            Win32_PrinterDriverDll,
            Win32_PrinterSetting,
            Win32_PrinterShare,
            Win32_PrintJob,
            Win32_Process,
            Win32_Processor,
            Win32_Product,
            Win32_ProductCheck,
            Win32_ProductResource,
            Win32_ProductSoftwareFeatures,
            Win32_ProgIDSpecification,
            Win32_ProgramGroup,
            Win32_ProgramGroupContents,
            Win32_Property,
            Win32_ProtocolBinding,
            Win32_Proxy,
            Win32_PublishComponentAction,
            Win32_QuickFixEngineering,
            Win32_QuotaSetting,
            Win32_Refrigeration,
            Win32_Registry,
            Win32_RegistryAction,
            Win32_RemoveFileAction,
            Win32_RemoveIniAction,
            Win32_ReserveCost,
            Win32_ScheduledJob,
            Win32_SCSIController,
            Win32_SCSIControllerDevice,
            Win32_SecuritySettingOfLogicalFile,
            Win32_SecuritySettingOfLogicalShare,
            Win32_SelfRegModuleAction,
            Win32_SerialPort,
            Win32_SerialPortConfiguration,
            Win32_SerialPortSetting,
            Win32_ServerConnection,
            Win32_ServerSession,
            Win32_Service,
            Win32_ServiceControl,
            Win32_ServiceSpecification,
            Win32_ServiceSpecificationService,
            Win32_SessionConnection,
            Win32_SessionProcess,
            Win32_Share,
            Win32_ShareToDirectory,
            Win32_ShortcutAction,
            Win32_ShortcutFile,
            Win32_ShortcutSAP,
            Win32_SID,
            Win32_SoftwareElement,
            Win32_SoftwareElementAction,
            Win32_SoftwareElementCheck,
            Win32_SoftwareElementCondition,
            Win32_SoftwareElementResource,
            Win32_SoftwareFeature,
            Win32_SoftwareFeatureAction,
            Win32_SoftwareFeatureCheck,
            Win32_SoftwareFeatureParent,
            Win32_SoftwareFeatureSoftwareElements,
            Win32_SoundDevice,
            Win32_StartupCommand,
            Win32_SubDirectory,
            Win32_SystemAccount,
            Win32_SystemBIOS,
            Win32_SystemBootConfiguration,
            Win32_SystemDesktop,
            Win32_SystemDevices,
            Win32_SystemDriver,
            Win32_SystemDriverPNPEntity,
            Win32_SystemEnclosure,
            Win32_SystemLoadOrderGroups,
            Win32_SystemLogicalMemoryConfiguration,
            Win32_SystemNetworkConnections,
            Win32_SystemOperatingSystem,
            Win32_SystemPartitions,
            Win32_SystemProcesses,
            Win32_SystemProgramGroups,
            Win32_SystemResources,
            Win32_SystemServices,
            Win32_SystemSlot,
            Win32_SystemSystemDriver,
            Win32_SystemTimeZone,
            Win32_SystemUsers,
            Win32_TapeDrive,
            Win32_TCPIPPrinterPort,
            Win32_TemperatureProbe,
            Win32_Terminal,
            Win32_TerminalService,
            Win32_TerminalServiceSetting,
            Win32_TerminalServiceToSetting,
            Win32_TerminalTerminalSetting,
            Win32_Thread,
            Win32_TimeZone,
            Win32_TSAccount,
            Win32_TSClientSetting,
            Win32_TSEnvironmentSetting,
            Win32_TSGeneralSetting,
            Win32_TSLogonSetting,
            Win32_TSNetworkAdapterListSetting,
            Win32_TSNetworkAdapterSetting,
            Win32_TSPermissionsSetting,
            Win32_TSRemoteControlSetting,
            Win32_TSSessionDirectory,
            Win32_TSSessionDirectorySetting,
            Win32_TSSessionSetting,
            Win32_TypeLibraryAction,
            Win32_UninterruptiblePowerSupply,
            Win32_USBController,
            Win32_USBControllerDevice,
            Win32_USBHub,
            Win32_UserAccount,
            Win32_UserDesktop,
            Win32_UserInDomain,
            Win32_UTCTime,
            Win32_VideoController,
            Win32_VideoSettings,
            Win32_VoltageProbe,
            Win32_VolumeQuotaSetting,
            Win32_WindowsProductActivation,
            Win32_WMIElementSetting,
            Win32_WMISetting
        }
        #endregion


        public class HardwareStruct
        {
            #region 内存对象
            /// <summary>
            /// 内存对象
            /// </summary>
            public class PhysicalMemoryInfo:DeviceBaseInfos
            {
                
                /// <summary>
                /// 内存编号
                /// </summary>
                public string PartNumber { get; set; }
                /// <summary>
                /// 内存大小（GB）
                /// </summary>
                public int Capacity { get; set; }
                /// <summary>
                /// 总宽
                /// </summary>
                public string TotalWidth { get; set; }
                /// <summary>
                /// 序列号
                /// </summary>
                public string SerialNumber { get; set; }
                /// <summary>
                /// SMBIOS内存类型
                /// </summary>
                public string SMBIOSMemoryType { get; set; }
                /// <summary>
                /// 速率
                /// </summary>
                public long Speed { get; set; }
                /// <summary>
                /// 类型详细信息
                /// </summary>
                public long TypeDetail { get; set; }
                /// <summary>
                /// 行位置
                /// </summary>
                public string PositionInRow { get; set; }
                /// <summary>
                /// 内存带宽
                /// </summary>
                public int DataWidth { get; set; }
            }
            #endregion

            #region USB对象
            /// <summary>
            /// USB对象
            /// </summary>
            public struct USBInfo
            {
                /// <summary>
                /// 设备名称
                /// </summary>
                public string DeviceName { get; set; }
                /// <summary>
                /// USB控制器名称
                /// </summary>
                public string Name { get; set; }
                /// <summary>
                /// USB厂商
                /// </summary>
                public string Manufacturer { get; set; }
                /// <summary>
                /// USB状态
                /// </summary>
                public string Status { get; set; }
                /// <summary>
                /// USB容量
                /// </summary>
                public string Size { get; set; }
                /// <summary>
                /// USB控制器ID
                /// </summary>
                public string PNPDeviceID { get; set; }
                /// <summary>
                /// 设备版本号
                /// </summary>
                public string VersionId { get; set; }
                /// <summary>
                /// 设备序列号
                /// </summary>
                public string SerialId { get; set; }
                /// <summary>
                /// 制造商ID
                /// </summary>
                public string ManufacturerId { get; set; }

            }
            #endregion

            #region 显卡对象
            /// <summary>
            /// 显卡对象
            /// </summary>
            public class VideoControllerInfo : DeviceBaseInfos
            {
                /// <summary>
                /// 用于此控制器与系统比较兼容性一般芯片组
                /// </summary>
                public string AdapterCompatibility { get; set; }
                /// <summary>
                /// 姓名或数字 - 模拟转换器（DAC）芯片的标识符
                /// </summary>
                public string AdapterDACType { get; set; }
                /// <summary>
                /// 视频适配器的内存大小
                /// </summary>
                public long AdapterRAM { get; set; }
                /// <summary>
                /// 显卡序列号
                /// </summary>
                public string PNPDeviceID { get; set; }
                /// <summary>
                /// 使用的比特数以显示每个像素
                /// </summary>
                public int CurrentBitsPerPixel { get; set; }
                /// <summary>
                /// 水平像素的当前数量
                /// </summary>
                public int CurrentHorizontalResolution { get; set; }
                /// <summary>
                /// 当前垂直像素数量
                /// </summary>
                public int CurrentVerticalResolution { get; set; }
                /// <summary>
                /// 频率在该视频控制器刷新监视器的图像
                /// </summary>
                public int CurrentRefreshRate { get; set; }
                /// <summary>
                /// 当前已安装的视频驱动程序的最后修改日期和时间
                /// </summary>
                public string DriverDate { get; set; }
                /// <summary>
                /// 视频驱动程序的版本号
                /// </summary>
                public string DriverVersion { get; set; }
                /// <summary>
                /// 当前的分辨率，颜色和视频控制器的扫描模式设置
                /// </summary>
                public string VideoModeDescription { get; set; }
                /// <summary>
                /// 无格式的字符串描述视频处理器
                /// </summary>
                public string VideoProcessor { get; set; }
                /// <summary>
                /// 在赫兹视频控制器的最大刷新率
                /// </summary>
                public int MaxRefreshRate {get;set;}
                /// <summary>
                /// 在赫兹视频控制器的最小刷新率
                /// </summary>
                public int MinRefreshRate { get; set; }
                /// <summary>
                /// 如果是TRUE，灰阶用于显示图像。
                /// </summary>
                public string Monochrome { get; set; }
            }
            #endregion

            #region 硬盘对象
            /// <summary>
            /// 硬盘对象
            /// </summary>
            public class HDInfo:DeviceBaseInfos
            {                
                /// <summary>
                /// 硬盘型号
                /// </summary>
                public string Model { get; set; }
                /// <summary>
                /// 硬盘容量
                /// </summary>
                public string Size { get; set; }
                /// <summary>
                /// 硬盘PNPDeviceID
                /// </summary>
                public string PNPDeviceID { get; set; }
                /// <summary>
                /// 硬盘分区数
                /// </summary>
                public string Partitions { get; set; }
                /// <summary>
                /// 硬盘 字节/扇
                /// </summary>
                public string BytesPerSector { get; set; }
                /// <summary>
                /// 硬盘 扇区/道
                /// </summary>
                public string SectorsPerTrack { get; set; }
                /// <summary>
                /// 硬盘 磁道/族。该值可能不准确。
                /// </summary>
                public string TracksPerCylinder { get; set; }
                /// <summary>
                /// 硬盘总扇区。该值可能不准确。
                /// </summary>
                public string TotalSectors { get; set; }
                /// <summary>
                /// 硬盘总磁道。该值可能不准确。
                /// </summary>
                public string TotalTracks { get; set; }
                /// <summary>
                /// 硬盘总族数。该值可能不准确。
                /// </summary>
                public string TotalCylinders { get; set; }
                /// <summary>
                /// 硬盘总磁头。该值可能不准确。
                /// </summary>
                public string TotalHeads { get; set; }
                /// <summary>
                /// 物理磁盘驱动器的类型 （IDE、sata）
                /// </summary>
                public string InterfaceType { get; set; }
                /// <summary>
                /// 由制造商分配的号来识别物理介质
                /// </summary>
                public string SerialNumber { get; set; }
                /// <summary>
                /// 修订制造商分配的磁盘驱动器固件
                /// </summary>
                public string FirmwareRevision { get; set; }
                /// <summary>
                /// 给定的驱动器的物理驱动器号
                /// </summary>
                public int Index { get; set; }
                /// <summary>
                /// 磁盘识别。该属性可以被用于识别一个共享资源。
                /// </summary>
                public string Signature { get; set; }
                /// <summary>
                /// 媒体访问设备的能力阵列
                /// </summary>
                public object Capabilities { get; set; }
                /// <summary>
                /// 更详细的解释为任何在功能阵列表示的访问设备的功能的列表
                /// </summary>
                public object CapabilityDescriptions { get; set; }
            }
            #endregion

            #region CPU对象
            /// <summary>
            /// CPU对象
            /// </summary>
            public class CPUInfo :DeviceBaseInfos
            {
                /// <summary>
                /// cpu频率
                /// </summary>
                public int CurrentClockSpeed { get; set; }
                /// <summary>
                /// cpu序号
                /// </summary>
                public string ProcessorId { get; set; }
                /// <summary>
                /// cpu主频
                /// </summary>
                public string ExtClock { get; set; }
                /// <summary>
                /// 在32位操作系统，该值是32，在64位操作系统是64。
                /// </summary>
                public string AddressWidth { get; set; }
                /// <summary>
                /// 所使用的平台的处理器架构
                /// </summary>
                public string Architecture { get; set; }
                /// <summary>
                /// 核心数 芯为处理器的当前实例的数目。核心是在集成电路上的物理处理器
                /// </summary>
                public int NumberOfCores { get; set; }
                /// <summary>
                /// 线程数
                /// </summary>
                public int ThreadCount { get; set; }
                /// <summary>
                /// 最大频率
                /// </summary>
                public int MaxClockSpeed { get; set; }
                /// <summary>
                /// 在32位处理器，该值是32，在64位处理器是64
                /// </summary>
                public int DataWidth { get; set;}
                /// <summary>
                /// 处理器的序列号
                /// </summary>
                public string SerialNumber { get; set; }
            }
            #endregion

            #region BIOS对象
            /// <summary>
            /// BIOS对象
            /// </summary>
            public struct BIOSInfo
            {
                /// <summary>
                /// BIOS厂商
                /// </summary>
                public string Manufacturer { get; set; }
                /// <summary>
                /// BIOS版本
                /// </summary>
                public string Version { get; set; }
                /// <summary>
                /// BIOS版本
                /// </summary>
                public string SMBIOSBIOSVersion { get; set; }
                /// <summary>
                /// 序列号
                /// </summary>
                public string SerialNumber { get; set; }
                /// <summary>
                /// 描述
                /// </summary>
                public string Description { get; set; }
                /// <summary>
                /// 当前语言
                /// </summary>
                public string CurrentLanguage { get; set; }
                /// <summary>
                /// 生产日期
                /// </summary>
                public string ReleaseDate { get; set; }
                /// <summary>
                /// 字符串信息
                /// </summary>
                public string StrInfo { get; set; }
                public string Caption { get; set; }

            }
            #endregion

            #region 基本信息对象
            /// <summary>
            /// 基本信息对象
            /// </summary>
            public struct SystemInfo
            {
                /// <summary>
                /// 计算机名称
                /// </summary>
                public string ComputerName { get; set; }
                /// <summary>
                /// 当前用户名称
                /// </summary>
                public string UserName { get; set; }
                /// <summary>
                /// 操作系统版本
                /// </summary>
                public string OSVersion { get; set; }
                /// <summary>
                /// 获得与当前用户关联的网络域名
                /// </summary>
                public string UserDomainName { get; set; }
                /// <summary>
                /// 获取操作系统页面文件的内存量(MB)
                /// </summary>
                public int SystemPageSize { get; set; }
                /// <summary>
                /// 获取映射到系统上下文的物理内存量
                /// </summary>
                public long WorkingSet { get; set; }
                /// <summary>
                /// 获取系统目录的完全限定路径
                /// </summary>
                public string SystemDirectory { get; set; }
                /// <summary>
                /// 获取本地计算机的NetBios名称
                /// </summary>
                public string MachineName { get; set; }
                /// <summary>
                /// 获取当前进程是否为64位进程
                /// </summary>
                public bool Is64BitProcess { get; set; }
                /// <summary>
                /// 确定当前系统是否为64位系统
                /// </summary>
                public bool Is64BitOperatingSystem { get; set; }
                /// <summary>
                /// 获取当前计算机的逻辑驱动器的名称
                /// </summary>
                public string[] LogicalDrives { get; set; }
                /// <summary>
                /// 获取或设置当前工作目录的完全限定路径
                /// </summary>
                public string CurrentDirectory { get; set; }
                /// <summary>
                /// 系统开机时间
                /// </summary>
                public DateTime SysStartTime { get; set; }
                /// <summary>
                /// 系统运行时间
                /// </summary>
                public TimeSpan SysRunTime { get; set; }
                /// <summary>
                /// 网卡MAC地址
                /// </summary>
                public string MaxAddress { get; set; }
                /// <summary>
                /// IP地址
                /// </summary>
                public string IpAddress { get; set; }
                /// <summary>
                /// cpu信息
                /// </summary>
                public List<CPUInfo> CpuInfos { get; set; }
                /// <summary>
                /// 物理内存大小(MB)
                /// </summary>
                public double TotalPhysicalMemory { get; set; }
                /// <summary>
                /// 虚拟内存大小(MB)
                /// </summary>
                public double TotalVMemory { get; set; }
                /// <summary>
                /// 获取计算机cpu的数量
                /// </summary>
                public int ProcessorCount { get; set; }
                /// <summary>
                /// pc类型
                /// </summary>
                public string SystemType { get; set; }
                /// <summary>
                /// 硬盘空间
                /// </summary>
                public HDSpaceInfo Space { get; set; }
                /// <summary>
                /// 计算机种类
                /// </summary>
                public string ComputerType { get; set; }
            }
            #endregion

            #region 磁盘空间对象
            /// <summary>
            /// 硬盘空间对象
            /// </summary>
            public struct HDSpaceInfo
            {
                /// <summary>
                /// 硬盘总空间（GB）
                /// </summary>
                public string HdTotalSpace { get; set; }
                /// <summary>
                /// 硬盘剩余空间（GB）
                /// </summary>
                public string HdSurplusSpace { get; set; }
            }
            #endregion

            #region 主机板对象
            /// <summary>
            /// 主机板对象
            /// </summary>
            public struct BaseBoardInfo
            {
                /// <summary>
                /// 制造商
                /// </summary>
                public string Manufacturer; // mo["Manufacturer"]，制造商，如“On-data”（昂达） 
                /// <summary>
                /// 型号
                /// </summary>
                public string Product; // mo["Product"]，型号，如“KT400A-8235” 
                /// <summary>
                /// 序列号
                /// </summary>
                public string SerialNumber; // mo["SerialNumber"]，序列号 
                /// <summary>
                /// 是否接通电源
                /// </summary>
                public bool PoweredOn;
                /// <summary>
                /// 版本
                /// </summary>
                public string Version;
                /// <summary>
                /// 描述
                /// </summary>
                public string Description;
                /// <summary>
                /// 信息字符串
                /// </summary>
                public string StrInfo;
                /// <summary>
                /// 主板，或在一个机箱中的基板
                /// </summary>
                public bool HostingBoard;
                /// <summary>
                /// 是否支持热插拔
                /// </summary>
                public bool HotSwappable;
                /// <summary>
                /// 名称
                /// </summary>
                public string Name;
                /// <summary>
                /// 可移动
                /// </summary>
                public bool Removable;
                /// <summary>
                /// 可替换
                /// </summary>
                public bool Replaceable;
            }
            #endregion

            #region 显示器对象
            /// <summary>
            /// 显示器对象
            /// </summary>
            public struct ScreenInfo
            {
                /// <summary>
                /// 显示器大小
                /// </summary>
                public int Size { get; set; }
                /// <summary>
                /// 是否为主显示器
                /// </summary>
                public bool Primary { get; set; }
                /// <summary>
                /// 获取与数据的一个像素相关联的内存位数
                /// </summary>
                public int BitsPerPixel { get; set; }
                /// <summary>
                /// 获取显示边界
                /// </summary>
                public System.Drawing.Rectangle Bounds { get; set; }
                /// <summary>
                /// 获得与显示关联的设备名称
                /// </summary>
                public string DeviceName { get; set; }
                /// <summary>
                /// 获得显示器的工作区域
                /// </summary>
                public System.Drawing.Rectangle WorkingArea { get; set; }
            }
            /// <summary>
            /// 监视器对象
            /// </summary>
            public class DesktopMonitorInfo:DeviceBaseInfos
            {
                /// <summary>
                /// 即插即用逻辑设备的播放装置识别符
                /// </summary>
                public string PNPDeviceID { get; set; }
            }
            #endregion


            #region 声卡对象
            /// <summary>
            /// 声卡对象
            /// </summary>
            public class SoundInfo : DeviceBaseInfos
            {
                /// <summary>
                /// 序列号
                /// </summary>
                public string SerialNumber { get; set; }                
                /// <summary>
                /// 即插即用逻辑设备的播放设备标识符
                /// </summary>
                public string PNPDeviceID { get; set; }
                /// <summary>
                /// 产品名字
                /// </summary>
                public string ProductName { get; set;}

            }
            #endregion

            #region 网卡对象
            /// <summary>
            /// 网卡对象
            /// </summary>
            public class NetworkAdapterInfo : DeviceBaseInfos
            {
                ///// <summary>
                ///// 序列号
                ///// </summary>
                //public string SerialNumber { get; set; }
                /// <summary>
                /// 即插即用逻辑设备的播放设备标识符
                /// </summary>
                public string PNPDeviceID { get; set; }
                /// <summary>
                /// 产品名字
                /// </summary>
                public string ProductName { get; set; }
                /// <summary>
                /// 是否安装
                /// </summary>
                public string Installed { get; set; }
                /// <summary>
                /// 索引值唯一标识本地网络接口
                /// </summary>
                public string InterfaceIndex { get; set; }
                /// <summary>
                /// 指明适配器是否是物理或逻辑适配器。如果为True，适配器是物理
                /// </summary>
                public string PhysicalAdapter { get; set; }
                /// <summary>
                /// 网络适配器的服务名
                /// </summary>
                public string ServiceName { get; set; }
                ///// <summary>
                /////  最大速度，以每秒位数，为网络适配器
                ///// </summary>
                //public string MaxSpeed { get; set; }

            }
            #endregion

            #region 物理媒体对象
            /// <summary>
            /// 物理媒体对象
            /// </summary>
            public class PhysicalMediaInfos : DeviceBaseInfos
            {
                /// <summary>
                /// 版本信息
                /// </summary>
                public object Version { get; set; }
                /// <summary>
                /// 零件编号
                /// </summary>
                public object PartNumber { get; set; }
                /// <summary>
                /// 其他识别信息
                /// </summary>
                public object OtherIdentifyingInfo { get; set; }
                /// <summary>
                /// 是否接通电源
                /// </summary>
                public object PoweredOn { get; set; }
                /// <summary>
                /// 是否可拆卸
                /// </summary>
                public object Removable { get; set; }
                /// <summary>
                /// 是否可更换
                /// </summary>
                public object Replaceable { get; set; }
                /// <summary>
                /// 是否支持热插拔
                /// </summary>
                public object HotSwappable { get; set; }
            }
            #endregion
            /// <summary>
            /// 基本信息
            /// </summary>
            public class DeviceBaseInfos
            {
                /// <summary>
                /// 名称
                /// </summary>
                public string Name { get; set; }
                /// <summary>
                /// 可用性和设备的状态
                /// </summary>
                public string Availability { get; set; }
                /// <summary>
                /// 厂商
                /// </summary>
                public string Manufacturer { get; set; }
                /// <summary>
                /// 对象与系统中的其他设备的唯一标识符
                /// </summary>
                public string DeviceID { get; set; }                
                /// <summary>
                /// 描述
                /// </summary>
                public string Description { get; set; }                
                /// <summary>
                /// 简短描述
                /// </summary>
                public string Caption { get; set; }
                /// <summary>
                /// 对象的当前状态
                /// </summary>
                public string Status { get; set; }
                /// <summary>
                /// 系统名称
                /// </summary>
                public string SystemName { get; set; }
                /// <summary>
                /// 信息字符串
                /// </summary>
                public string StrInfo { get; set; }
                /// <summary>
                /// 尺寸
                /// </summary>
                public float Size { get; set; }
            }

            /// <summary>
            /// CPU的信息结构
            /// </summary>        
            [StructLayout(LayoutKind.Sequential)]
            public struct CPU_INFO
            {
                public uint dwOemId;
                public uint dwPageSize;
                public uint lpMinimumApplicationAddress;
                public uint lpMaximumApplicationAddress;
                public uint dwActiveProcessorMask;
                public uint dwNumberOfProcessors;
                public uint dwProcessorType;
                public uint dwAllocationGranularity;
                public uint dwProcessorLevel;
                public uint dwProcessorRevision;
            }

            /// <summary>
            /// 内存的信息结构
            /// </summary>        
            [StructLayout(LayoutKind.Sequential)]
            public struct MEMORY_INFO
            {
                public uint dwLength;
                /// <summary>
                /// 正在使用内存比
                /// </summary>
                public uint dwMemoryLoad;
                /// <summary>
                /// 物理内存共有
                /// </summary>
                public uint dwTotalPhys;
                /// <summary>
                /// 可使用的物理内存
                /// </summary>
                public uint dwAvailPhys;
                /// <summary>
                /// 交换文件总大小
                /// </summary>
                public uint dwTotalPageFile;
                /// <summary>
                /// 尚可交换文件大小
                /// </summary>
                public uint dwAvailPageFile;
                /// <summary>
                /// 总虚拟内存
                /// </summary>
                public uint dwTotalVirtual;
                /// <summary>
                /// 未用虚拟内存
                /// </summary>
                public uint dwAvailVirtual;
            }

            //定义系统时间的信息结构 
            [StructLayout(LayoutKind.Sequential)]
            public struct SYSTEMTIME_INFO
            {
                public ushort wYear;
                public ushort wMonth;
                public ushort wDayOfWeek;
                public ushort wDay;
                public ushort wHour;
                public ushort wMinute;
                public ushort wSecond;
                public ushort wMilliseconds;
            }
        }
    }
}
