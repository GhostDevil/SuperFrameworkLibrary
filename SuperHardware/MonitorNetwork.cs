using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Timers;

namespace SuperFramework.SuperHardware
{
    public class MonitorNetwork
    {
        public class StatisticsInfo
        {
            /// <summary>
            /// 上传速度
            /// </summary>
            public string UpSpeed { get; set; }
            /// <summary>
            /// 下载速度
            /// </summary>
            public string DownSpeed { get; set; }
            /// <summary>
            /// 流量总计 发送+接收
            /// </summary>
            public string AllTraffic { get; set; }
            /// <summary>
            /// 统计开始时间
            /// </summary>
            public DateTime TrafficTime { get; set; }
            /// <summary>
            /// 流量总计 发送
            /// </summary>
            public string AllTrafficSend { get; set; }
            /// <summary>
            /// 流量总计 接收
            /// </summary>
            public string AllTrafficRecived { get; set; }
            /// <summary>
            /// 发送总量
            /// </summary>
            public long SentBytes { get; set; }
            /// <summary>
            /// 接收总量
            /// </summary>
            public long RecivedBytes { get; set; }
        }
        /// <summary>
        /// 
        /// </summary>
        public StatisticsInfo statisticsInfo = new();
        /// <summary>
        /// ipV4
        /// </summary>
        public StatisticsInfo statisticsInfoV4 = new();
        /// <summary>
        /// ipV6
        /// </summary>
        public StatisticsInfo statisticsInfoV6 = new();

        private string NetCardDescription { get; set; }
        //建立连接时上传下载临时的数据量
        private long BaseTraffic { get; set; }
        private long BaseTraffic4 { get; set; }
        private long OldUpFrist { get; set; }
        private long OldDownFrist { get; set; }
        private long OldUp4Frist{ get; set; }
        private long OldDown4Frist { get; set; }
        private long OldUp { get; set; }
        private long OldDown { get; set; }
        private long OldUp4 { get; set; }
        private long OldDown4 { get; set; }

       /// <summary>
       /// 网络接口统计对象
       /// </summary>
        private List<NetworkInterface> networkInterface = new();
        /// <summary>
        /// 计时器
        /// </summary>
        private Timer timer = new() { Interval = 1000 };
        /// <summary>
        /// 关闭监视
        /// </summary>
        public void Close()
        {
            timer?.Stop();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="netCardDescription">描述</param>
        public MonitorNetwork(string netCardDescription)
        {
            timer.Elapsed += Timer_Elapsed;
            NetCardDescription = netCardDescription;
            timer.Interval = 1000;
        }
        /// <summary>
        /// 
        /// </summary>
        public MonitorNetwork()
        {
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 1000;
        }
        /// <summary>
        /// 开启监视
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            networkInterface = null;
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            if (!string.IsNullOrWhiteSpace(NetCardDescription))
            {

                foreach (var var in nics)
                {

                    if (var.Description.Contains(NetCardDescription) && var.OperationalStatus== OperationalStatus.Up)
                    {
                        networkInterface = new List<NetworkInterface>
                        {
                            var
                        };
                        break;
                    }
                }
            }
            else
            {
                foreach (var var in nics)
                {

                    if (var.OperationalStatus== OperationalStatus.Up && !var.Name.ToLower().Contains("loopback"))
                    {
                        if(networkInterface==null)
                            networkInterface = new List<NetworkInterface>();
                        networkInterface.Add(var);
                    }
                }
            }
            if (networkInterface == null)
            {
                return false;
            }
            else
            {
                statisticsInfo.TrafficTime = statisticsInfoV4.TrafficTime = statisticsInfoV6.TrafficTime = DateTime.Now;
                for (int i = 0; i < networkInterface.Count; i++)
                {
                    BaseTraffic += networkInterface[i].GetIPStatistics().BytesSent +
                              networkInterface[i].GetIPStatistics().BytesReceived;
                    BaseTraffic4 += networkInterface[i].GetIPv4Statistics().BytesSent +
                              networkInterface[i].GetIPv4Statistics().BytesReceived;

                    OldUpFrist += networkInterface[i].GetIPStatistics().BytesSent;
                    OldUp += networkInterface[i].GetIPStatistics().BytesSent;
                    OldUp4Frist += networkInterface[i].GetIPv4Statistics().BytesSent;
                    OldUp4 += networkInterface[i].GetIPv4Statistics().BytesSent;

                    OldDownFrist += networkInterface[i].GetIPStatistics().BytesReceived;
                    OldDown += networkInterface[i].GetIPStatistics().BytesReceived;
                    OldDown4Frist += networkInterface[i].GetIPv4Statistics().BytesReceived;
                    OldDown4 += networkInterface[i].GetIPv4Statistics().BytesReceived;
                }
                timer.Start();
                return true;
            }

        }

        private string[] units = new string[] { "KB/s", "MB/s", "GB/s" };

        private void CalcUpSpeed()
        {
            long nowValue = 0;
            long nowValue4 = 0;//networkInterface.GetIPStatistics().BytesSent;
            for (int i = 0; i < networkInterface.Count; i++)
            {
                nowValue += networkInterface[i].GetIPStatistics().BytesSent;
                nowValue4 += networkInterface[i].GetIPv4Statistics().BytesSent;
            }
            statisticsInfo.SentBytes = nowValue;
            statisticsInfoV4.SentBytes = nowValue4;
            int num = 0;
            //获取发送速率
            double value = (nowValue - OldUp) / 1024.0;
            while (value > 1023)
            {
                value /= 1024.0;
                num++;
            }
            statisticsInfo.UpSpeed = value.ToString("0.0") + units[num];
            OldUp = nowValue;
            double valueSend = (OldUp - OldUpFrist) / 1024.0;
            while (valueSend > 1023)
            {
                valueSend /= 1024.0;
                num++;
            }
            statisticsInfo.AllTrafficSend = valueSend.ToString("0.0") + unitAlls[num];

            num = 0;
            double value4 = (nowValue4 - OldUp4) / 1024.0;
            while (value4 > 1023)
            {
                value4 /= 1024.0;
                num++;
            }
            statisticsInfoV4.UpSpeed = value4.ToString("0.0") + units[num];
            OldUp4 = nowValue4;
            num = 0;
            double valueSend4 = (OldUp4 - OldUp4Frist) / 1024.0;
            while (valueSend4 > 1023)
            {
                valueSend4 /= 1024.0;
                num++;
            }
            statisticsInfoV4.AllTrafficSend = valueSend4.ToString("0.0") + unitAlls[num];
        }

        private void CalcDownSpeed()
        {
            long nowValue = 0;
            long nowValue4 = 0;// networkInterface.GetIPStatistics().BytesReceived;
            for (int i = 0; i < networkInterface.Count; i++)
            {
                nowValue += networkInterface[i].GetIPStatistics().BytesReceived;
                nowValue4 += networkInterface[i].GetIPv4Statistics().BytesReceived;
            }
            statisticsInfo.RecivedBytes = nowValue;
            statisticsInfoV4.RecivedBytes = nowValue4;
            int num = 0;
            //获取下载速率
            double value = (nowValue - OldDown) / 1024.0;
            while (value > 1023)
            {
                value /= 1024.0;
                num++;
            }
            statisticsInfo.DownSpeed = value.ToString("0.0") + units[num];
            OldDown = nowValue;
            //获取接收数据量
            double valueDown = (  OldDown-OldDownFrist) / 1024.0;
            while (valueDown > 1023)
            {
                valueDown /= 1024.0;
                num++;
            }
            statisticsInfo.AllTrafficRecived = valueDown.ToString("0.0") + unitAlls[num];

            
            num = 0;
            double value4 = (nowValue4 - OldDown4) / 1024.0;
            while (value4 > 1023)
            {
                value4 /= 1024.0;
                num++;
            }
            statisticsInfoV4.DownSpeed = value4.ToString("0.0") + units[num];
            OldDown4 = nowValue4;
            //获取接收数据量
            double valueDown4 = (OldDown4 - OldDown4Frist) / 1024.0;
            while (valueDown4 > 1023)
            {
                valueDown4 /= 1024.0;
                num++;
            }
            statisticsInfoV4.AllTrafficRecived = valueDown4.ToString("0.0") + unitAlls[num];
        }
        public string ConvertUnit(long value)
        {
            int num = 0;
            double value1 = value / 1024.0;
            while (value1 > 1023)
            {
                value1 /= 1024;
                num++;
            }
            return value1.ToString("0.0") + unitAlls[num];
        }
        private string[] unitAlls = new string[] { "KB", "MB", "GB", "TB" };

        private void CalcAllTraffic()
        {
            long nowValue = OldDown + OldUp;
            int num = 0;
            double value = (nowValue - BaseTraffic) / 1024.0;
            while (value > 1023)
            {
                value /= 1024.0;
                num++;
            }
            statisticsInfo.AllTraffic = value.ToString("0.0") + unitAlls[num];
            
            num = 0;
            long nowValue4 = OldDown4 + OldUp4;
            double value4 = (nowValue4 - BaseTraffic4) / 1024.0;
            while (value4 > 1023)
            {
                value4 /= 1024.0;
                num++;
            }
            statisticsInfoV4.AllTraffic = value4.ToString("0.0") + unitAlls[num];
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CalcUpSpeed();
            CalcDownSpeed();
            CalcAllTraffic();
        }
    }
}
