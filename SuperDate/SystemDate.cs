﻿using System;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SuperFramework.SuperDate
{
    /// <summary>
    /// 类 名:SystemDate
    /// 版 本:Release
    /// 日 期:2015-06-17
    /// 作 者:不良帥
    /// 描 述:系统时间辅助类
    /// </summary>
    public class SystemDate
    {
        #region  获取标准北京时间
        /// <summary>  
        /// 获取标准北京时间，读取http://www.beijing-time.org/time.asp。  
        /// </summary>  
        /// <returns>返回网络时间</returns>  
        public async static Task<DateTime> GetBeijingTime()
        {

            DateTime dt;
            HttpClient httpClient = new HttpClient();
            try
            {
                HttpResponseMessage wrt = await httpClient.GetAsync("https://www.beijing-time.org/");
                string html = await wrt.Content.ReadAsStringAsync();


                //using (Stream stream = wrp.GetResponseStream())
                //{
                //    StreamReader sr = new StreamReader(stream, Encoding.UTF8);
                //    html = sr.ReadToEnd();
                //}

                string[] tempArray = html.Split(';');
                for (int i = 0; i < tempArray.Length; i++)
                {
                    tempArray[i] = tempArray[i].Replace("\r\n", "");
                }

                string year = tempArray[1].Split('=')[1];
                string month = tempArray[2].Split('=')[1];
                string day = tempArray[3].Split('=')[1];
                string hour = tempArray[5].Split('=')[1];
                string minite = tempArray[6].Split('=')[1];
                string second = tempArray[7].Split('=')[1];

                dt = DateTime.Parse(year + "-" + month + "-" + day + " " + hour + ":" + minite + ":" + second);
            }
            catch (WebException)
            {
                return DateTime.Parse("2011-1-1");
            }
            catch (Exception)
            {
                return DateTime.Parse("2011-1-1");
            }
            finally
            {

                httpClient.Dispose();
            }
            return dt;
        }

        #endregion

        #region  设置系统时间的API函数 
        //设置系统时间的API函数
        [DllImport("kernel32.dll")]
        private static extern bool SetLocalTime(ref SYSTEMTIME time);
        [StructLayout(LayoutKind.Sequential)]
        #endregion

        #region  系统时间结构 
        private struct SYSTEMTIME
        {
            public short year;
            public short month;
            public short dayOfWeek;
            public short day;
            public short hour;
            public short minute;
            public short second;
            public short milliseconds;
        }
        #endregion

        #region  同步本地系统时间 
        /// <summary>
        /// 同步系统时间
        /// </summary>
        /// <param name="dt">需要设置的时间</param>
        /// <returns>返回系统时间设置状态，true为成功，false为失败</returns>
        public static bool SetLocalDate(DateTime dt)
        {
            SYSTEMTIME st;
            st.year = (short)dt.Year;
            st.month = (short)dt.Month;
            st.dayOfWeek = (short)dt.DayOfWeek;
            st.day = (short)dt.Day;
            st.hour = (short)dt.Hour;
            st.minute = (short)dt.Minute;
            st.second = (short)dt.Second;
            st.milliseconds = (short)dt.Millisecond;
            bool rt = SetLocalTime(ref st);
            return rt;
        }
        #endregion

        #region  获取标准北京时间,多地址搜索防止获取不到时间。 
        ///<summary>
        /// 获取标准北京时间,多地址搜索防止获取不到时间。
        ///</summary>
        ///<returns></returns>
        public static DateTime GetDataStandardTime()
        {
            DateTime dt;
            //返回国际标准时间
            //只使用的时间服务器的IP地址，未使用域名
            try
            {
                string[,] 时间服务器 = new string[14, 2];
                int[] 搜索顺序 = new int[] { 3, 2, 4, 8, 9, 6, 11, 5, 10, 0, 1, 7, 12 };
                时间服务器[0, 0] = "time-a.nist.gov";
                时间服务器[0, 1] = "129.6.15.28";
                时间服务器[1, 0] = "time-b.nist.gov";
                时间服务器[1, 1] = "129.6.15.29";
                时间服务器[2, 0] = "time-a.timefreq.bldrdoc.gov";
                时间服务器[2, 1] = "132.163.4.101";
                时间服务器[3, 0] = "time-b.timefreq.bldrdoc.gov";
                时间服务器[3, 1] = "132.163.4.102";
                时间服务器[4, 0] = "time-c.timefreq.bldrdoc.gov";
                时间服务器[4, 1] = "132.163.4.103";
                时间服务器[5, 0] = "utcnist.colorado.edu";
                时间服务器[5, 1] = "128.138.140.44";
                时间服务器[6, 0] = "time.nist.gov";
                时间服务器[6, 1] = "192.43.244.18";
                时间服务器[7, 0] = "time-nw.nist.gov";
                时间服务器[7, 1] = "131.107.1.10";
                时间服务器[8, 0] = "nist1.symmetricom.com";
                时间服务器[8, 1] = "69.25.96.13";
                时间服务器[9, 0] = "nist1-dc.glassey.com";
                时间服务器[9, 1] = "216.200.93.8";
                时间服务器[10, 0] = "nist1-ny.glassey.com";
                时间服务器[10, 1] = "208.184.49.9";
                时间服务器[11, 0] = "nist1-sj.glassey.com";
                时间服务器[11, 1] = "207.126.98.204";
                时间服务器[12, 0] = "nist1.aol-ca.truetime.com";
                时间服务器[12, 1] = "207.200.81.113";
                时间服务器[13, 0] = "nist1.aol-va.truetime.com";
                时间服务器[13, 1] = "64.236.96.53";
                int portNum = 13;
                string hostName;
                byte[] bytes = new byte[1024];
                int bytesRead = 0;
                System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
                for (int i = 0; i < 13; i++)
                {
                    hostName = 时间服务器[搜索顺序[i], 1];
                    try
                    {
                        client.Connect(hostName, portNum);
                        System.Net.Sockets.NetworkStream ns = client.GetStream();
                        bytesRead = ns.Read(bytes, 0, bytes.Length);
                        client.Close();
                        break;
                    }
                    catch (Exception)
                    {
                    }
                }
                char[] sp = new char[1];
                sp[0] = ' ';
                dt = new DateTime();
                string str1;
                str1 = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRead);

                string[] s;
                s = str1.Split(sp);
                if (s.Length >= 2)
                {
                    dt = DateTime.Parse(s[1] + "" + s[2]);//得到标准时间
                    dt = dt.AddHours(8);//得到北京时间*/
                }
                else
                {
                    dt = DateTime.Parse("2014-1-1");
                }
            }
            catch (Exception)
            {
                dt = DateTime.Parse("2014-1-1");
            }
            return dt;
        }
        #endregion
    }
}