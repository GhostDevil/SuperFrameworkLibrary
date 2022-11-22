using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;

namespace SuperFramework.SuperHardware
{
    /// <summary>
    /// <para>日期:2016-09-12</para>
    /// <para>作者:不良帥</para>
    /// <para>描述:硬件资源使用率辅助类</para>
    /// </summary>
    public  class HardwareUseInfo
    {
        public string NodeName { get; set; }
        public float CPUProcessorTime { get; set; }
        public float CPUPrivilegedTime { get; set; }
        public float CPUInterruptTime { get; set; }
        public float CPUDPCTime { get; set; }
        public float MEMAvailable { get; set; }
        public float MEMCommited { get; set; }
        public float MEMCommitLimit { get; set; }
        public float MEMCommitedPerc { get; set; }
        public float MEMPoolPaged { get; set; }
        public float MEMPoolNonPaged { get; set; }
        public float MEMCached { get; set; }
        public float PageFile { get; set; }
        public float ProcessorQueueLengh { get; set; }
        public float DISCQueueLengh { get; set; }
        public float DISKRead { get; set; }
        public float DISKWrite { get; set; }
        public float DISKAveraGetimeRead { get; set; }
        public float DISKAveraGetimeWrite { get; set; }
        /// <summary>
        /// 硬盘利用率
        /// </summary>
        public float DISKTime { get; set; }
        public float HANDLECountCounter { get; set; }
        public float THREADCount { get; set; }
        public int CONTENTSwitches { get; set; }
        public int SYSTEMCalls { get; set; }
        public float NetTrafficSend { get; set; }
        public float NetTrafficReceive { get; set; }
        public DateTime SamplingTime { get; set; }
        private PerformanceCounter cpuProcessorTime = new("Processor", "% Processor Time", "_Total");
        private PerformanceCounter cpuPrivilegedTime = new("Processor", "% Privileged Time", "_Total");
        private PerformanceCounter cpuInterruptTime = new("Processor", "% Interrupt Time", "_Total");
        private PerformanceCounter cpuDPCTime = new("Processor", "% DPC Time", "_Total");
        private PerformanceCounter memAvailable = new("Memory", "Available MBytes", null);
        private PerformanceCounter memCommited = new("Memory", "Committed Bytes", null);
        private PerformanceCounter memCommitLimit = new("Memory", "Commit Limit", null);
        private PerformanceCounter memCommitedPerc = new("Memory", "% Committed Bytes In Use", null);
        private PerformanceCounter memPollPaged = new("Memory", "Pool Paged Bytes", null);
        private PerformanceCounter memPollNonPaged = new("Memory", "Pool Nonpaged Bytes", null);
        private PerformanceCounter memCached = new("Memory", "Cache Bytes", null);
        private PerformanceCounter pageFile = new("Paging File", "% Usage", "_Total");
        private PerformanceCounter processorQueueLengh = new("System", "Processor Queue Length", null);
        private PerformanceCounter diskQueueLengh = new("PhysicalDisk", "Avg. Disk Queue Length", "_Total");
        private PerformanceCounter diskRead = new("PhysicalDisk", "Disk Read Bytes/sec", "_Total");
        private PerformanceCounter diskWrite = new("PhysicalDisk", "Disk Write Bytes/sec", "_Total");
        private PerformanceCounter diskAveraGetimeRead = new("PhysicalDisk", "Avg. Disk sec/Read", "_Total");
        private PerformanceCounter diskAveraGetimeWrite = new("PhysicalDisk", "Avg. Disk sec/Write", "_Total");
        private PerformanceCounter diskTime = new("PhysicalDisk", "% Disk Time", "_Total");
        private PerformanceCounter handleCountCounter = new("Process", "Handle Count", "_Total");
        private PerformanceCounter threadCount = new("Process", "Thread Count", "_Total");
        private PerformanceCounter contentSwitches = new("System", "Context Switches/sec", null);
        private PerformanceCounter systemCalls = new("System", "System Calls/sec", null);
        private PerformanceCounterCategory performanceNetCounterCategory;
        private PerformanceCounter[] trafficSentCounters;
        private PerformanceCounter[] trafficReceivedCounters;
        private string[] interfaces = null;
        public void InitNetCounters()
        {
            // PerformanceCounter(CategoryName,CounterName,InstanceName)
            performanceNetCounterCategory = new PerformanceCounterCategory("Network Interface");
            interfaces = performanceNetCounterCategory.GetInstanceNames();
            int length = interfaces.Length;
            if (length > 0)
            {
                trafficSentCounters = new PerformanceCounter[length];
                trafficReceivedCounters = new PerformanceCounter[length];
            }
            for (int i = 0; i < length; i++)
            {
                // Initializes a new, read-only instance of the PerformanceCounter class.
                //   1st paramenter: "categoryName"-The name of the performance counter category (performance object) with which this performance counter is associated.
                //   2nd paramenter: "CounterName" -The name of the performance counter.
                //   3rd paramenter: "instanceName" -The name of the performance counter category instance, or an empty string (""), if the category contains a single instance.
                trafficReceivedCounters[i] = new PerformanceCounter("Network Interface", "Bytes Received/sec", interfaces[i]);
                trafficSentCounters[i] = new PerformanceCounter("Network Interface", "Bytes Sent/sec", interfaces[i]);
            }
            // List of all names of the network interfaces
            for (int i = 0; i < length; i++)
            {
                Console.WriteLine("Name netInterface: {0}", performanceNetCounterCategory.GetInstanceNames()[i]);
            }
        }
        public void GetProcessorCpuTime()
        {
            float tmp = cpuProcessorTime.NextValue();
            CPUProcessorTime = (float)(Math.Round(tmp, 1));
            // Environment.ProcessorCount: return the total number of cores
        }
        public void GetCpuPrivilegedTime()
        {
            float tmp = cpuPrivilegedTime.NextValue();
            CPUPrivilegedTime = (float)(Math.Round(tmp, 1));
        }
        public void GetCpuinterruptTime()
        {
            float tmp = cpuInterruptTime.NextValue();
            CPUInterruptTime = (float)(Math.Round(tmp, 1));
        }
        public void GetcpuDPCTime()
        {
            float tmp = cpuDPCTime.NextValue();
            CPUDPCTime = (float)(Math.Round(tmp, 1));
        }
        public void GetPageFile()
        {
            PageFile = pageFile.NextValue();
        }
        public void GetProcessorQueueLengh()
        {
            ProcessorQueueLengh = processorQueueLengh.NextValue();
        }
        public void GetMemAvailable()
        {
            MEMAvailable = memAvailable.NextValue();
        }
        public void GetMemCommited()
        {
            MEMCommited = memCommited.NextValue() / (1024 * 1024);
        }
        public void GetMemCommitLimit()
        {
            MEMCommitLimit = memCommitLimit.NextValue() / (1024 * 1024);
        }
        public void GetMemCommitedPerc()
        {
            float tmp = memCommitedPerc.NextValue();
            // return the value of Memory Commit Limit
            MEMCommitedPerc = (float)(Math.Round(tmp, 1));
        }
        public void GetMemPoolPaged()
        {
            float tmp = memPollPaged.NextValue() / (1024 * 1024);
            MEMPoolPaged = (float)(Math.Round(tmp, 1));
        }
        public void GetMemPoolNonPaged()
        {
            float tmp = memPollNonPaged.NextValue() / (1024 * 1024);
            MEMPoolNonPaged = (float)(Math.Round(tmp, 1));
        }
        public void GetMemCachedBytes()
        {
            // return the value of Memory Cached in MBytes
            MEMCached = memCached.NextValue() / (1024 * 1024);
        }
        public void GetDiskQueueLengh()
        {
            DISCQueueLengh = diskQueueLengh.NextValue();
        }
        public void GetDiskRead()
        {
            float tmp = diskRead.NextValue() / 1024;
            DISKRead = (float)(Math.Round(tmp, 1));
        }
        public void GetDiskWrite()
        {
            float tmp = diskWrite.NextValue() / 1024;
            DISKWrite = (float)(Math.Round(tmp, 1)); // round 1 digit decimal
        }
        public void GetDiskAveraGetimeRead()
        {
            float tmp = diskAveraGetimeRead.NextValue() * 1000;
            DISKAveraGetimeRead = (float)(Math.Round(tmp, 1)); // round 1 digit decimal
        }
        public void GetDiskAveraGetimeWrite()
        {
            float tmp = diskAveraGetimeWrite.NextValue() * 1000;
            DISKAveraGetimeWrite = (float)(Math.Round(tmp, 1)); // round 1 digit decimal
        }
        public void GetDiskTime()
        {
            float tmp = diskTime.NextValue();
            DISKTime = (float)(Math.Round(tmp, 1));
        }
        public void GetHandleCountCounter()
        {
            HANDLECountCounter = handleCountCounter.NextValue();
        }
        public void GetThreadCount()
        {
            THREADCount = threadCount.NextValue();
        }
        public void GetContentSwitches()
        {
            CONTENTSwitches = (int)Math.Ceiling(contentSwitches.NextValue());
        }
        public void GetsystemCalls()
        {
            SYSTEMCalls = (int)Math.Ceiling(systemCalls.NextValue());
        }
        public void GetCurretTrafficSent()
        {
            int length = interfaces.Length;
            float sendSum = 0.0F;
            for (int i = 0; i < length; i++)
            {
                sendSum += trafficSentCounters[i].NextValue();
            }
            float tmp = 8 * (sendSum / 1024);
            NetTrafficSend = (float)(Math.Round(tmp, 1));
        }
        public void GetCurretTrafficReceived()
        {
            int length = interfaces.Length;
            float receiveSum = 0.0F;
            for (int i = 0; i < length; i++)
            {
                receiveSum += trafficReceivedCounters[i].NextValue();
            }
            float tmp = 8 * (receiveSum / 1024);
            NetTrafficReceive = (float)(Math.Round(tmp, 1));
        }
        public void GetSampleTime()
        {
            SamplingTime = DateTime.Now;
        }


        #region 获取cpu使用率
        private static string GetInstanceName(string categoryName, string counterName, Process p)
        {
            try
            {
                PerformanceCounterCategory processcounter = new(categoryName);
                string[] instances = processcounter.GetInstanceNames();
                foreach (string instance in instances)
                {
                    PerformanceCounter counter = new(categoryName, counterName, instance);
                    //Logger.Info("对比in mothod GetInstanceName，" + counter.NextValue() + "：" + p.Id);
                    if (counter.NextValue() == p.Id)
                    {
                        return instance;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }
        /// <summary>
        /// 上次记录的CPU时间
        /// </summary>
        static TimeSpan prevCpuTime = TimeSpan.Zero;
        /// <summary>
        /// 获取进程的第一个实例cpu使用率
        /// </summary>
        /// <param name="pname">进程名</param>
        /// <param name="interval">间隔时间（毫秒）</param>
        /// <returns>返回使用百分比数</returns>
        /// <exception cref="Win32Exception 访问被拒绝">访问被拒绝</exception>
        public static double UsingProcess(string pname, int interval= 1000)
        {
            using (var pro = Process.GetProcessesByName(pname)[0])
            {
               
                //while (true)
                //{
                    //当前时间
                    var curTime = pro.TotalProcessorTime;
                    //间隔时间内的CPU运行时间除以逻辑CPU数量
                    var value = (curTime - prevCpuTime).TotalMilliseconds / interval / Environment.ProcessorCount * 100;
                    prevCpuTime = curTime;
                //输出
                return Math.Round(value, 2, MidpointRounding.AwayFromZero);

                   // Thread.Sleep(interval);
                //}
            }
        }
        /// <summary>
        /// 获取cpu使用率
        /// </summary>
        /// <param name="pName">指定进程名,默认全局使用率</param>
        /// <returns>返回使用率百分比数</returns>
        public static double GetCpuUseRateByProcess(string pName)//= "_Total"
        {

            //string instance1 = GetInstanceName("Process", "ID Process", pName);
            //if (instance1 != null)
            //{
            //    PerformanceCounter cpucounter = new PerformanceCounter("Process", "% Processor Time", instance1);
            //    if (cpucounter != null)
            //    {
            //        float f=cpucounter.NextValue();
            //        Thread.Sleep(200); //等200ms(是测出能换取下个样本的最小时间间隔)，让后系统获取下一个样本,因为第一个样本无效
            //        return f;
            //    }

            //}
            //return 0;

            ////创建性能计数器
            //using (var p1 = new PerformanceCounter("Process", "% Processor Time", pName))
            //{

            //        //注意除以CPU数量
            //       float f= p1.NextValue() / Environment.ProcessorCount;
            //        Thread.Sleep(1000);
            //    return f;
            //}
          
            try
            {
                double b = GetCounterValue(_cpuCounter, "Process", "% Processor Time", pName);
                //b = b / Environment.ProcessorCount; curpcp.NextValue() / 1024,curtime.NextValue() / Environment.ProcessorCount
                b /= Environment.ProcessorCount;
                return Math.Round(b, 2, MidpointRounding.AwayFromZero);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        #endregion

        #region 硬盘速度
        /// <summary>
        /// 获取硬盘读取速度
        /// </summary>
        /// <param name="pName">指定进程名</param>
        /// <returns>读取速度 Bytes/sec</returns>
        public static double GetPhysicalDiskReadUseRate(string pName = "_Total")
        {
            try
            {
                return GetCounterValue(_diskReadCounter, "PhysicalDisk", "Disk Read Bytes/sec", pName);
             
            }
            catch (Exception)
            {
                return 0;
            }
        }
        /// <summary>
        /// 获取硬盘写入速度
        /// </summary>
        /// <param name="pName">指定进程名</param>
        /// <returns>写入速度 Bytes/sec</returns>
        public static double GetPhysicalDiskWriteUseRate(string pName = "_Total")
        {
            try
            {
               return GetCounterValue(_diskWriteCounter, "PhysicalDisk", "Disk Write Bytes/sec", pName);
          
            }
            catch (Exception)
            {
                return 0;
            }
        }
      
        #endregion

        #region 获取内存使用率
        /// <summary>
        /// 获取内存空闲率
        /// </summary>
        /// <returns>返回百分比数</returns>
        public static double GetMemoryFreeRate()
        {
            double x= HardwareInfo.GetFreePhysicalMemory() / HardwareInfo.GetPhysicalMemory() * 100;
            return double.Parse(x.ToString("#0.00"));
        }
        /// <summary>
        /// 获取内存使用率
        /// </summary>
        /// <returns>返回百分比数</returns>
        public static double GetMemoryUseRate()
        {
            double x = (HardwareInfo.GetPhysicalMemory()-HardwareInfo.GetFreePhysicalMemory()) / HardwareInfo.GetPhysicalMemory() * 100;
            return double.Parse(x.ToString("#0.00"));
        }
        #endregion

        #region 获取硬盘空间使用率
        /// <summary>
        /// 获取硬盘空间使用率
        /// </summary>
        /// <returns>返回硬盘空间使用率百分比数</returns>
        public static double GetHDUseRate()
        {
            return double.Parse((double.Parse(HardwareInfo.GetHDSpace().HdTotalSpace)-(double.Parse(HardwareInfo.GetHDSpace().HdSurplusSpace)) / double.Parse(HardwareInfo.GetHDSpace().HdTotalSpace)).ToString("#0.00"))*100;
        }
        /// <summary>
        /// 获取硬盘空间空闲率
        /// </summary>
        /// <returns>返回硬盘空间空闲百分比数</returns>
        public static double GetHDFreeRate()
        {
            return double.Parse((double.Parse(HardwareInfo.GetHDSpace().HdSurplusSpace) / double.Parse(HardwareInfo.GetHDSpace().HdTotalSpace)).ToString("#0.00")) * 100;
        }
        #endregion

        #region 获取虚拟内存使用情况
        /// <summary>
        /// 获取虚拟内存使用情况
        /// </summary>
        /// <returns>返回结果字符串</returns>
        public static string GetMemoryVDataByUse()
        {
            string str;
            double d = GetCounterValue(_memoryCounter, "Memory", "% Committed Bytes In Use", null);
            str = d.ToString("F") + "% (";

            d = GetCounterValue(_memoryCounter, "Memory", "Committed Bytes", null);
            str += FormatBytes(d) + " / ";

            d = GetCounterValue(_memoryCounter, "Memory", "Commit Limit", null);
            return string.Format("{0}{1}) ", str, FormatBytes(d));
        }
        /// <summary>
        /// 获取虚拟内存空闲情况
        /// </summary>
        /// <returns>返回结果字符串</returns>
        public static string GetMemoryVDataByFree()
        {
            string str;
            double d = GetCounterValue(_memoryCounter, "Memory", "% Committed Bytes In Use", null);

            str = (100 - d).ToString("F") + "% (";

            d = GetCounterValue(_memoryCounter, "Memory", "Committed Bytes", null);
            str += FormatBytes(d) + " / ";

            d = GetCounterValue(_memoryCounter, "Memory", "Commit Limit", null);
            return string.Format("{0}{1}) ", str, FormatBytes(d));
        }
        /// <summary>
        /// 获取物理内存使用情况
        /// </summary>
        /// <returns>返回结果字符串</returns>
        public static string GetMemoryPDataByUse()
        {
            string s = QueryComputerSystem("totalphysicalmemory");
            double totalphysicalmemory = Convert.ToDouble(s);

            double d = GetCounterValue(_memoryCounter, "Memory", "Available Bytes", null);
            d = totalphysicalmemory - d;

            s = _compactFormat ? "%" : string.Format("% ({0} / {1})", FormatBytes(d), FormatBytes(totalphysicalmemory));
            d /= totalphysicalmemory;
            d *= 100;
            return _compactFormat ? (int)d + s : d.ToString("F") + s;
        }
        /// <summary>
        /// 获取物理内存空闲情况
        /// </summary>
        /// <returns>返回结果字符串</returns>
        public static string GetMemoryPDataByFree()
        {
            string s = QueryComputerSystem("totalphysicalmemory");
            double totalphysicalmemory = Convert.ToDouble(s);

            double d = GetCounterValue(_memoryCounter, "Memory", "Available Bytes", null);
            d = totalphysicalmemory - d;

            s = _compactFormat ? "%" : string.Format("% ({0} / {1})", FormatBytes(d), FormatBytes(totalphysicalmemory));
            d /= totalphysicalmemory;
            d *= 100;
            return _compactFormat ? (int)d + s : d.ToString("F") + s;
        }
        #endregion

        #region 获取网络使用情况
        /// <summary>
        /// 网络检测类型
        /// </summary>
        public enum NetData 
        {
            ReceivedAndSent, 
            Received, 
            Sent
        };
        /// <summary>
        /// 获取网络使用情况
        /// </summary>
        /// <param name="nd">获取类型</param>
        /// <returns>返回使用情况字符串</returns>
        public static float GetNetData(NetData nd)
        {
            double receiveSum = 0;
            PerformanceCounterCategory category = new("Network Interface");

            foreach (string name in category.GetInstanceNames())
            {
                if (name == "MS TCP Loopback interface" || name.Contains("isatap") || name.Contains("Interface"))
                    continue;
                try 
                {
                    receiveSum += nd == NetData.Received ?
                                       GetCounterValue(_netRecvCounters, "Network Interface", "Bytes Received/sec", name) :
                                  nd == NetData.Sent ?
                                       GetCounterValue(_netSentCounters, "Network Interface", "Bytes Sent/sec", name) :
                                  nd == NetData.ReceivedAndSent ?
                                       GetCounterValue(_netRecvCounters, "Network Interface", "Bytes Received/sec", name) +
                                       GetCounterValue(_netSentCounters, "Network Interface", "Bytes Sent/sec", name) :
                                  0;
                }
                catch { }
            }
            float tmp = 8 * ((float)receiveSum / 1024);
            receiveSum = (float)(Math.Round(tmp, 1));


            return (float)receiveSum;
        }
        public static void GetNetWorkBytes(ref long recivedBytes,ref long sentBytes)
        {
             sentBytes = 0;
             recivedBytes = 0;
            List<NetworkInterface> _operationalNiCs = new();
            foreach (NetworkInterface t in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (t.OperationalStatus.ToString() == "Up")
                    if (t.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                        _operationalNiCs.Add(t);
            }
            foreach (NetworkInterface nic in _operationalNiCs)
            {
                IPInterfaceStatistics interfaceStats = nic.GetIPStatistics();
                sentBytes += interfaceStats.BytesSent;
                recivedBytes += interfaceStats.BytesReceived;
            }
        }
        public static string[] GetAdapter()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            string[] name = new string[adapters.Length];
            int index = 0;
            foreach (NetworkInterface ni in adapters)
            {
                name[index] = ni.Description;
                index++;
            }
            return name;
        }
        #endregion

        #region 获取本地磁盘剩余空间
        /// <summary>
        /// 获取本地磁盘剩余空间
        /// </summary>
        /// <returns>返回分区空间剩余量</returns>
        public static string LogicalDisk()
        {
            string diskSpace = string.Empty;
            object device, space;
            ManagementObjectSearcher objCS = new("SELECT * FROM Win32_LogicalDisk");
            foreach (ManagementObject objMgmt in objCS.Get())
            {
                device = objMgmt["DeviceID"];       // C:
                if (null != device)
                {
                    space = objMgmt["FreeSpace"];   // C:10.32 GB, D:5.87GB
                    if (null != space)
                        diskSpace += string.Format("{0}{1}, ", device, FormatBytes(double.Parse(space.ToString())));
                }
            }

            diskSpace = diskSpace.Substring(0, diskSpace.Length - 2);
            return diskSpace;
        }

        #endregion

        #region 单位转换
        /// <summary>
        /// 单位
        /// </summary>
        enum Unit { B, KB, MB, GB, ER }
        /// <summary>
        /// 单位转换
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        internal static string FormatBytes(double bytes)
        {
            int unit = 0;
            while (bytes > 1024)
            {
                bytes /= 1024;
                ++unit;
            }

            string s = _compactFormat ? ((int)bytes).ToString() : bytes.ToString("F") + " ";
            return s + ((Unit)unit).ToString();
        }
        #endregion

        #region 内部方法
         
        private static string QueryComputerSystem(string type)
        {
            string str = null;
            ManagementObjectSearcher objCS = new("SELECT * FROM Win32_ComputerSystem");
            foreach (ManagementObject objMgmt in objCS.Get())
            {
                str = objMgmt[type].ToString();
            }
            return str;
        }

        static double GetCounterValue(PerformanceCounter pc, string categoryName, string counterName, string instanceName)
        {
            pc.CategoryName = categoryName;
            pc.CounterName = counterName;
            pc.InstanceName = instanceName;
            
            float f= pc.NextValue();
            //Thread.Sleep(1000);
            return f;
        }

        #endregion

        #region 内部对象
        static bool _compactFormat;

        static PerformanceCounter _memoryCounter = new();
        static PerformanceCounter _cpuCounter = new();
        static PerformanceCounter _diskReadCounter = new();
        static PerformanceCounter _diskWriteCounter = new();

        static string[] _instanceNames;
        static PerformanceCounter _netRecvCounters=new();
        static PerformanceCounter _netSentCounters=new();

        #endregion

    }
}
