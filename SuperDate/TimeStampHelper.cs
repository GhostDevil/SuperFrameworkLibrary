using System;

namespace SuperFramework.SuperDate
{
    /// <summary>
    /// 时间戳
    /// </summary>
    public static class TimeStampHelper
    {
        /// <summary>
        /// 获取秒级时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStampSeconds()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        /// <summary>
        /// 获取毫秒级时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStampMilliseconds()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp(DateTime time, int length = 13)
        {
            long ts = ConvertDateTimeToInt(time);
            return ts.ToString().Substring(0, length);
        }
        /// <summary> 
        /// 将c# DateTime时间格式转换为Unix时间戳格式 
        /// </summary> 
        /// <param name="time">时间</param> 
        /// <returns>long 13位</returns> 
        public static long ConvertDateTimeToInt(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;  //除10000调整为13位  ~ /~ 相差毫秒数 
            return t;
        }
        /// <summary>    
        /// 时间戳转为时间    
        /// </summary>    
        /// <param name="timeStamp"></param>    
        /// <returns></returns>    
        public static DateTime ConvertStringToDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        /// <summary>
        /// 时间戳转为时间10位
        /// </summary>
        /// <param name="curSeconds">Unix时间戳格式</param>
        /// <returns>C#格式时间</returns>
        public static DateTime GetDateTimeFrom1970Ticks(long curSeconds)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return dtStart.AddSeconds(curSeconds);
        }

        /// <summary>
        /// 验证时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <param name="interval">差值（分钟）</param>
        /// <returns></returns>
        public static bool IsTime(long time, double interval)
        {
            DateTime dt = GetDateTimeFrom1970Ticks(time);
            //取现在时间
            DateTime dt1 = DateTime.Now.AddMinutes(interval);
            DateTime dt2 = DateTime.Now.AddMinutes(interval * -1);
            if (dt > dt2 && dt < dt1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断时间戳是否正确（验证前8位）
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool IsTime(string time)
        {
            string str = GetTimeStamp(DateTime.Now, 8);
            if (str.Equals(time))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
