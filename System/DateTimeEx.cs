namespace System
{
    public static class DateTimeEx
    {

        ////public static DateTime ToDateTime(this string timeStamp)// 时间戳Timestamp转换成日期
        ////{
        ////    DateTime dtStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
        ////    long lTime = long.Parse(timeStamp + "0000000");
        ////    TimeSpan toNow = new TimeSpan(lTime);
        ////    DateTime targetDt = dtStart.Add(toNow);
        ////    return dtStart.Add(toNow);
        ////}
        ///// <summary>
        ///// 【毫秒级】获取时间（格林威治时间）
        ///// </summary>
        ///// <param name="timestamp">13位时间戳</param>
        //public static DateTime ToUnixDateTimeMilliseconds(long timestamp)
        //{
        //    long begtime = timestamp * 10000;
        //    DateTime dt_1970 = new(1970, 1, 1, 0, 0, 0);
        //    long tricks_1970 = dt_1970.Ticks;//1970年1月1日刻度
        //    long time_tricks = tricks_1970 + begtime;//日志日期刻度
        //    DateTime dt = new(time_tricks);//转化为DateTime
        //    return dt;
        //}
        ///// <summary>
        ///// 【毫秒级】生成13位时间戳（格林威治时间）
        ///// </summary>
        ///// <param name="dt">时间</param>
        //public static long ToUnixTimeStampMilliseconds(DateTime dt)
        //{
        //    DateTime dateStart = new(1970, 1, 1, 0, 0, 0);
        //    return Convert.ToInt64((dt - dateStart).TotalMilliseconds);
        //}
        //#region 13位时间戳转换（毫秒级）
        ///// <summary>
        ///// 【毫秒级】获取时间（北京时间）
        ///// </summary>
        ///// <param name="timestamp">13位时间戳</param>
        //static DateTime ToDateTimeMilliseconds(long timestamp)
        //{
        //    long begtime = timestamp * 10000;
        //    DateTime dt_1970 = new(1970, 1, 1, 8, 0, 0);
        //    long tricks_1970 = dt_1970.Ticks;//1970年1月1日刻度
        //    long time_tricks = tricks_1970 + begtime;//日志日期刻度
        //    DateTime dt = new(time_tricks);//转化为DateTime
        //    return dt;
        //}
        ///// <summary>
        ///// 【毫秒级】生成13位时间戳（北京时间）
        ///// </summary>
        ///// <param name="dt">时间</param>
        //static long ToTimeStampMilliseconds(DateTime dt)
        //{
        //    DateTime dateStart = new(1970, 1, 1, 8, 0, 0);
        //    return Convert.ToInt64((dt - dateStart).TotalMilliseconds);
        //}
        //#endregion

        #region 时间戳
        /// <summary>
        /// 时间戳转为时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="isMS">是否毫秒级</param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this long timeStamp, bool isMS = true)//时间戳Timestamp转换成日期
        {
            if (timeStamp < 0)
                return null;
            DateTime time;
            System.DateTime startTime = TimeZoneInfo.ConvertTime(new System.DateTime(1970, 1, 1), TimeZoneInfo.Local); // 当地时区
            if (isMS)
            {
                time = startTime.AddMilliseconds(timeStamp);
            }
            else
            {
                time = startTime.AddSeconds(timeStamp);
            }
            return time;
        }
        /// <summary>    
        /// 时间戳转为时间    
        /// </summary>    
        /// <param name="timeStamp"></param>    
        /// <param name="isMS">是否毫秒级</param>
        /// <returns></returns>    
        public static DateTime? ToDateTimeByTimeStamp(this string timeStamp, bool isMS = true)
        {
            if (string.IsNullOrWhiteSpace(timeStamp))
                return null;
            return long.Parse(timeStamp).ToDateTime(isMS);
        }
        /// <summary>    
        /// 时间字符串转为时间    
        /// </summary>    
        /// <param name="timeStamp"></param> 
        /// <returns></returns>    
        public static DateTime? ToDateTime(this string time)
        {
            if (string.IsNullOrWhiteSpace(time))
                return null;
            bool b = DateTime.TryParse(time, out DateTime ti);
            if (b)
                return ti;
            else
                return null;
        }
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="isMS">是否毫秒级</param>
        /// <returns></returns>
        public static long ToTimeStamp(this string dt, bool isMS = true)
        {
            DateTime dts = DateTime.Parse(dt);
            return dts.ToTimeStamp(isMS);
        }
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="isMS">是否毫秒级</param>
        /// <returns></returns>
        public static long ToTimeStamp(this DateTime dt, bool isMS = true)
        {
            DateTime dateStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            long timeStamp;
            if (isMS)
                timeStamp = Convert.ToInt64((dt - dateStart).TotalMilliseconds);
            else
                timeStamp = Convert.ToInt64((dt - dateStart).TotalSeconds);
            return timeStamp;
        }

        #endregion

        #region 格式

        #region  GMT时间转本地时间 
        /// <summary>  
        /// GMT时间转成本地时间  
        /// </summary>  
        /// <param name="gmtDate">字符串形式的GMT时间</param>  
        /// <returns>返回本地时间</returns>  
        public static DateTime GMTToLocal(this string gmtDate)
        {
            DateTime dt = DateTime.MinValue;
            try
            {
                string pattern = "";
                if (gmtDate.IndexOf("+0") != -1)
                {
                    gmtDate = gmtDate.Replace("GMT", "");
                    pattern = "ddd, dd-MMM-yyyy HH':'mm':'ss zzz";
                }
                if (gmtDate.ToUpper().IndexOf("GMT") != -1)
                {
                    pattern = "ddd, dd-MMM-yyyy HH':'mm':'ss 'GMT'";
                }
                if (pattern != "")
                {
                    dt = DateTime.ParseExact(gmtDate, pattern, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal);
                    dt = dt.ToLocalTime();
                }
                else
                {
                    dt = Convert.ToDateTime(gmtDate);
                }
            }
            catch
            { }
            return dt;
        }
        #endregion

        /// <summary>
        /// 获取时间字符串
        /// </summary>
        /// <param name="str">24小时制 数字日期字符串</param>
        /// <returns>返回时间格式字符串</returns>
        public static string FormatTimeString(this string str)
        {
            if (str.Length < 14)
                return "";
            return $"{str[..4]}-{str.Substring(4, 2)}-{str.Substring(6, 2)} {str.Substring(8, 2)}:{str.Substring(10, 2)}:{str.Substring(12, 2)}";
        }
        /// <summary>
        /// 格式化日期时间
        /// </summary>
        /// <param name="dateTime">日期时间</param>
        /// <param name="dateMode">显示模式</param>
        /// <returns>格式化后的日期</returns>
        public static string FormatDate(this DateTime dateTime, DateFormat dateMode)
        {
            return dateMode switch
            {
                DateFormat.d => string.Format("{0:d}", dateTime),
                DateFormat.D => string.Format("{0:D}", dateTime),
                DateFormat.f => string.Format("{0:f}", dateTime),
                DateFormat.F => string.Format("{0:F}", dateTime),
                DateFormat.g => string.Format("{0:g}", dateTime),
                DateFormat.G => string.Format("{0:G}", dateTime),
                DateFormat.M => string.Format("{0:M}", dateTime),
                DateFormat.R => string.Format("{0:R}", dateTime),
                DateFormat.s => string.Format("{0:s}", dateTime),
                DateFormat.t => string.Format("{0:t}", dateTime),
                DateFormat.T => string.Format("{0:T}", dateTime),
                DateFormat.u => string.Format("{0:u}", dateTime),
                DateFormat.U => string.Format("{0:U}", dateTime),
                DateFormat.Y => string.Format("{0:Y}", dateTime),
                DateFormat.o => string.Format("{0}", dateTime),
                DateFormat.a => string.Format("{0:yyyyMMddHHmmss}", dateTime),
                DateFormat.b => string.Format("{0:yyyy-MM-dd HH:mm:ss}", dateTime),
                DateFormat.c => string.Format("{0:yyyy-MM-dd}", dateTime),
                DateFormat.s0 => dateTime.GetDateTimeFormats('s')[0].ToString(),
                DateFormat.t0 => dateTime.GetDateTimeFormats('t')[0].ToString(),
                DateFormat.y0 => dateTime.GetDateTimeFormats('y')[0].ToString(),
                DateFormat.D0 => dateTime.GetDateTimeFormats('D')[0].ToString(),
                DateFormat.D1 => dateTime.GetDateTimeFormats('D')[1].ToString(),
                DateFormat.D2 => dateTime.GetDateTimeFormats('D')[2].ToString(),
                DateFormat.D3 => dateTime.GetDateTimeFormats('D')[3].ToString(),
                DateFormat.M0 => dateTime.GetDateTimeFormats('M')[0].ToString(),
                DateFormat.f0 => dateTime.GetDateTimeFormats('f')[0].ToString(),
                DateFormat.g0 => dateTime.GetDateTimeFormats('g')[0].ToString(),
                DateFormat.r0 => dateTime.GetDateTimeFormats('r')[0].ToString(),
                _ => dateTime.ToString(),
            };
        }
        /// <summary>
        /// 格式化时间模式
        /// </summary>
        public enum DateFormat
        {
            /// <summary>
            /// 2005-11-5
            /// </summary>
            d,
            /// <summary>
            /// 2005年11月5日
            /// </summary>
            D,
            /// <summary>
            /// 2005年11月5日 14:23
            /// </summary>
            f,
            /// <summary>
            /// 2005年11月5日 14:23:23
            /// </summary>
            F,
            /// <summary>
            /// 2005-11-5 14:23
            /// </summary>
            g,
            /// <summary>
            /// 2005-11-5 14:23:23
            /// </summary>
            G,
            /// <summary>
            /// 11月5日
            /// </summary>
            M,
            /// <summary>
            /// Sat, 05 Nov 2005 14:23:23 GMT
            /// </summary>
            R,
            /// <summary>
            /// 2005-11-05T14:23:23
            /// </summary>
            s,
            /// <summary>
            /// 14:23
            /// </summary>
            t,
            /// <summary>
            /// 14:23:23
            /// </summary>
            T,
            /// <summary>
            /// 2005-11-05 14:23:23Z
            /// </summary>
            u,
            /// <summary>
            /// 2005年11月5日 6:23:23
            /// </summary>
            U,
            /// <summary>
            /// 2005年11月
            /// </summary>
            Y,
            /// <summary>
            /// 2005-11-5 14:23:23
            /// </summary>
            o,
            /// <summary>
            /// 20051105142323(2005-11-5 14:23:23)
            /// </summary>
            a,
            /// <summary>
            /// 2005-11-05 14:23:23(2005-11-5 14:23:23)
            /// </summary>
            b,
            /// <summary>
            /// 2005-11-05
            /// </summary>
            c,
            /// <summary>
            /// 2005-11-05T14:06:25
            /// </summary>
            s0,
            /// <summary>
            /// 14:06
            /// </summary>
            t0,
            /// <summary>
            /// 2005年11月
            /// </summary>
            y0,
            /// <summary>
            /// 2005年11月5日
            /// </summary>
            D0,
            /// <summary>
            /// 2005 11 05
            /// </summary>
            D1,
            /// <summary>
            /// 星期六 2005 11 05
            /// </summary>
            D2,
            /// <summary>
            /// 星期六 2005年11月5日
            /// </summary>
            D3,
            /// <summary>
            /// 11月5日
            /// </summary>
            M0,
            /// <summary>
            /// 2005年11月5日 14:06
            /// </summary>
            f0,
            /// <summary>
            /// 2005-11-5 14:06
            /// </summary>
            g0,
            /// <summary>
            /// Sat, 05 Nov 2005 14:06:25 GMT
            /// </summary>
            r0
        }
        #endregion

        #region 当前时间是否处于时间范围内 
        /// <summary>
        /// 当前时间是否处于时间范围内
        /// </summary>
        /// <param name="startShortTime">开始时间（HH:mm:ss）</param>
        /// <param name="endshortTime">结束时间（HH:mm:ss）</param>
        /// <returns>范围内返回true，否则返回false</returns>
        public static bool IsInTime(this DateTime nowTime, string startShortTime, string endshortTime)
        {
            if (string.IsNullOrWhiteSpace(startShortTime) || string.IsNullOrWhiteSpace(endshortTime))
                return false;
            DateTime StartTime = DateTime.Parse(startShortTime);
            DateTime EndTime = DateTime.Parse(endshortTime);
            bool b = int.Parse(StartTime.ToString("HHmmss")) > int.Parse(EndTime.ToString("HHmmss"))
                ? nowTime >= StartTime && nowTime <= EndTime.AddDays(1)
                : nowTime >= StartTime && nowTime <= EndTime;
            return b;
        }
        /// <summary>
        /// 当前时间是否处于时间范围内
        /// </summary>
        /// <param name="startShortTime">开始时间（HH:mm:ss）</param>
        /// <param name="endshortTime">结束时间（HH:mm:ss）</param>
        /// <returns>范围内返回true，否则返回false</returns>
        public static bool IsInTime(this DateTime nowTime, DateTime StartTime, DateTime EndTime)
        {
            if (StartTime.Year < EndTime.Year && StartTime.Year > nowTime.Year)
                return false;
            bool b = false;
            if (StartTime.Year <= DateTime.Now.Year && DateTime.Now.Year <= EndTime.Year)
            {
                b = int.Parse(StartTime.ToString("HHmmss")) > int.Parse(EndTime.ToString("HHmmss"))
                    ? nowTime >= StartTime && nowTime <= EndTime.AddDays(1)
                    : nowTime >= StartTime && nowTime <= EndTime;
            }
            return b;
        }
        #endregion

        #region  判断时间段是否属于同一天 
        /// <summary>
        /// 一个时间段是否在同一天内
        /// </summary>
        /// <param name="startShortTime">开始时间（HH:mm:ss）</param>
        /// <param name="endshortTime">结束时间（HH:mm:ss）</param>
        /// <returns>在同一天范围内返回true,否则返回false,参数为null返回false</returns>
        public static bool IsInTheDay(this string startShortTime, string endshortTime)
        {
            if (string.IsNullOrWhiteSpace(startShortTime) || string.IsNullOrWhiteSpace(endshortTime))
                return false;
            startShortTime = startShortTime.Replace(":", "");
            endshortTime = endshortTime.Replace(":", "");
            double x = double.Parse(startShortTime);
            double y = double.Parse(endshortTime);
            if (x > y)
                return false;
            else
                return true;
        }

        #endregion

        /// <summary>
        /// 根据年月日计算星期几
        /// </summary>
        /// <returns></returns>
        public static string WeekDay(this DateTime time)
        {
            string[] weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            string week = weekdays[Convert.ToInt32(time.DayOfWeek)];

            return week;
        }

        /// <summary>
        /// 获得指定日期所在周的第一天
        /// </summary>
        /// <param name="datetime">日期</param>
        /// <returns>返回指定日期第一天日期</returns>
        public static DateTime GetFirstDayOfWeek(this DateTime datetime)
        {
            int weeknow = Convert.ToInt32(datetime.DayOfWeek);
            int daydiff = (-1) * weeknow + 1;
            //本周第一天
            return datetime.AddDays(daydiff);
        }
        /// <summary>
        /// 获得指定日期所在周的最后一天
        /// </summary>
        /// <param name="datetime">日期</param>
        /// <returns>返回指定日期最后一天日期</returns>
        public static DateTime GetLastDayOfWeek(this DateTime datetime)
        {
            int weeknow = Convert.ToInt32(datetime.DayOfWeek);
            int dayadd = 7 - weeknow;
            //本周最后一天
            return datetime.AddDays(dayadd);
        }

    }
}
