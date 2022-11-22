#if NET461_OR_GREATER || NET5_0_OR_GREATER
using System;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SuperFramework.SuperHardware
{
    /// <summary>
    /// 日期:2017-09-23
    /// 作者:不良帥
    /// 描述:系统信息获取辅助类
    /// </summary>
    public class ComputerInfo
    {
        public string CpuID;
        public string MacAddress;
        public string DiskID;
        public string IpAddress;
        public string LoginUserName;
        public string ComputerName;
        public string SystemType;
        public string TotalPhysicalMemory; //单位：M   
        private static ComputerInfo _instance;
        public static ComputerInfo Instance()
        {
            if (_instance == null)
                _instance = new ComputerInfo();
            return _instance;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ComputerInfo()
        {
            CpuID = GetCpuID();
            MacAddress = GetMacAddress();
            DiskID = GetDiskID();
            IpAddress = GetIPAddress();
            LoginUserName = GetUserName();
            SystemType = GetSystemType();
            TotalPhysicalMemory = GetTotalPhysicalMemory();
            ComputerName = GetComputerName();
        }
        #region 获取系统日志
        /// <summary>
        /// 获取系统日志
        /// </summary>
        /// <returns>返回系统日志</returns>
        public StringBuilder GetSystemLog()
        {
            StringBuilder sb = new();
            EventLog mylog = new()
            {
                Log = "Application"  // System[系统日志] | Application[应用日志] | Security[安全日志]
            };
            EventLogEntryCollection myCollection = mylog.Entries;
            for (int i = 0; i < myCollection.Count; i++)
            {
                sb.Append("Time:" + myCollection[i].TimeGenerated + " " +
                    myCollection[i].Message + "<br />");
            }
            return sb;
        }
        #endregion

        #region 获得开机运行时长
        /// <summary>
        /// 获得本计算机开机后运行时长
        /// </summary>
        /// <returns>返回开机时间与现在时间的时间间隔</returns>
        public static TimeSpan GetComputerRunTime()
        {
            int tickSecond = Environment.TickCount / 60000;
            int tickDay = tickSecond / 60 / 24;
            int tickHour = (tickSecond / 60) % 24;
            int tickMinute = tickSecond % 60;
            TimeSpan ts = new(tickDay, tickHour, tickMinute, tickMinute, tickSecond);
            //String runTime = String.Format("{0} 天 {1} 小时 {2} 分", tickDay, tickHour, tickMinute);
            return ts;
        }
        /// <summary>
        /// 获得本计算机开机后运行时长
        /// </summary>
        /// <returns>返回开机时间与现在时间的时间间隔</returns>
        public static string GetComputerRunTimeStr()
        {
            int tickSecond = Environment.TickCount / 60000;
            int tickDay = tickSecond / 60 / 24;
            int tickHour = (tickSecond / 60) % 24;
            int tickMinute = tickSecond % 60;
            TimeSpan ts = new(tickDay, tickHour, tickMinute, tickMinute, tickSecond);
            return string.Format("{0} 天 {1} 小时 {2} 分", tickDay, tickHour, tickMinute);
          
        }
        #endregion

        #region 获得计算机开机时间
        /// <summary>
        /// 获得本计算机开机时间
        /// </summary>
        /// <returns>返回开机时间</returns>
        public static DateTime GetComputerStartTime()
        {
            return DateTime.Now.AddMilliseconds(-Environment.TickCount);
        }
        #endregion

        #region 获得当前登录用户名
        /// <summary>
        /// 获得计算机当前登录用户
        /// </summary>
        /// <returns>返回当前用户名</returns>
        public static string GetLoginUserName()
        {
            return Environment.UserName;
        }
        #endregion

        #region 获取cpu序列号
        /// <summary>
        /// 获取cpu序列号
        /// </summary>
        /// <returns></returns>
        public string GetCpuID()
        {
            try
            {
                //获取CPU序列号代码   
                string cpuInfo = "";//cpu序列号   
                ManagementClass mc = new("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                }
                moc = null;
                mc = null;
                return cpuInfo;
            }
            catch
            {
                return "unknow";
            }
        }
        #endregion

        #region 获取网卡硬件地址
        /// <summary>
        /// 获取网卡硬件地址  
        /// </summary>
        /// <returns></returns>
        public string GetMacAddress()
        {
            try
            {
                //获取网卡硬件地址   
                string mac = "";
                ManagementClass mc = new("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        mac = mo["MacAddress"].ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return mac;
            }
            catch
            {
                return "unknow";
            }
        }
        #endregion

        #region 获取IP地址
        /// <summary>
        /// 获取IP地址（IPv4）
        /// </summary>
        /// <returns>IpV4地址</returns>
        public static string GetIPV4Address()
        {
            try
            {
                IPAddress[] arrIPAddresses = Dns.GetHostAddresses(Dns.GetHostName());
                foreach (IPAddress ip in arrIPAddresses)
                {
                    if (ip.AddressFamily.Equals(AddressFamily.InterNetwork))//IPv4
                    {
                        return ip.ToString();
                    }
                }
                return "unknow";
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }
        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <returns></returns>
        public string GetIPAddress()
        {
            try
            {
                //获取IP地址   
                string st = "";
                ManagementClass mc = new("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        //st=mo["IpAddress"].ToString();   
                        Array ar = (Array)(mo.Properties["IpAddress"].Value);
                        st = ar.GetValue(0).ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
        }
        #endregion

        #region 获取硬盘ID
        /// <summary>
        /// 获取硬盘ID 
        /// </summary>
        /// <returns></returns>
        public string GetDiskID()
        {
            try
            {
                //获取硬盘ID   
                string HDid = "";
                ManagementClass mc = new("Win32_DiskDrive");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    HDid = (string)mo.Properties["Model"].Value;
                }
                moc = null;
                mc = null;
                return HDid;
            }
            catch
            {
                return "unknow";
            }
        }
        #endregion

        #region 操作系统的登录用户名
        /// <summary>   
        /// 操作系统的登录用户名   
        /// </summary>   
        /// <returns></returns>   
        public string GetUserName()
        {
            try
            {
                string st = "";
                ManagementClass mc = new("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    st = mo["UserName"].ToString();
                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
        }
        #endregion

        #region 获取PC类型
        /// <summary>   
        /// 获取PC类型   
        /// </summary>   
        /// <returns></returns>   
        public string GetSystemType()
        {
            try
            {
                string st = "";
                ManagementClass mc = new("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    st = mo["SystemType"].ToString();
                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
        }
        #endregion

        #region 获取物理内存
        /// <summary>   
        /// 获取物理内存   
        /// </summary>   
        /// <returns></returns>   
        public string GetTotalPhysicalMemory()
        {
            try
            {
                string st = "";
                ManagementClass mc = new("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    st = mo["TotalPhysicalMemory"].ToString();
                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
        }
        #endregion

        #region 获取电脑名称
        /// <summary>   
        /// 获取电脑名称
        /// </summary>   
        /// <returns></returns>   
        public string GetComputerName()
        {
            try
            {
                return System.Environment.GetEnvironmentVariable("ComputerName");
            }
            catch
            {
                return "unknow";
            }
        }
        #endregion

    }
}
#endif