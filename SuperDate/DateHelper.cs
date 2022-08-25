using System;
using System.Globalization;

namespace SuperFramework.SuperDate
{

    /// <summary>
    /// 日 期:2017-11-30
    /// 作 者:不良帥
    /// 描 述:日期时间辅助方法类
    /// </summary>
    public class DateHelper
    {
        #region Unix与DateTime
        /// <summary>
        /// 将Unix时间戳转换为DateTime类型时间
        /// </summary>
        /// <param name="time">数字时间</param>
        /// <returns>DateTime格式时间</returns>
        public static DateTime ConvertInt2DatTime(double time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime dtime = startTime.AddSeconds(time);
            return dtime;
        }
        /// <summary>
        /// 将DateTime类型时间转换为Unix时间戳
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>double 格式时间</returns>
        public static double ConvertDatTime2Int(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            double dtime = (time - startTime).TotalSeconds;
            return dtime;
        }
        #endregion

        #region  将时间格式化成 年月日 的形式,如果时间为null，返回当前系统时间 
        /// <summary>
        /// 将时间格式化成 年月日 的形式,如果时间为null，返回当前系统时间
        /// </summary>
        /// <param name="dt">年月日分隔符</param>
        /// <param name="separator"></param>
        /// <returns>日期字符串</returns>
        public string GetFormatDate(DateTime dt, char separator)
        {
            if (dt != null && !dt.Equals(DBNull.Value))
            {
                string tem = string.Format("yyyy{0}MM{1}dd", separator, separator);
                return dt.ToString(tem);
            }
            else
            {
                return GetFormatDate(DateTime.Now, separator);
            }
        }
        #endregion

        #region  将时间格式化成 时分秒 的形式,如果时间为null，返回当前系统时间 
        /// <summary>
        /// 将时间格式化成 时分秒 的形式,如果时间为null，返回当前系统时间
        /// </summary>
        /// <param name="dt">时间</param>
        /// <param name="separator">分隔符</param>
        /// <returns>时间字符串</returns>
        public string GetFormatTime(DateTime dt, char separator)
        {
            if (dt != null && !dt.Equals(DBNull.Value))
            {
                string tem = string.Format("hh{0}mm{1}ss", separator, separator);
                return dt.ToString(tem);
            }
            else
            {
                return GetFormatTime(DateTime.Now, separator);
            }
        }
        #endregion

        #region  带毫秒的字符转换成时间（DateTime）格式 
        /// <summary>
        /// 带毫秒的字符转换成时间（DateTime）格式
        /// 可处理格式：[2014-10-10 10:10:10,666 或 2014-10-10 10:10:10 666]
        /// </summary>
        public static DateTime GetFormatDateTime(string dateTime)
        {
            string[] strArr = dateTime.Split(new char[] { '-', ' ', ':', ',' });
            DateTime dt = new DateTime(int.Parse(strArr[0]),
                int.Parse(strArr[1]),
                int.Parse(strArr[2]),
                int.Parse(strArr[3]),
                int.Parse(strArr[4]),
                int.Parse(strArr[5]),
                int.Parse(strArr[6]));
            return dt;
        }
        #endregion

        #region  将日期时间格式化成 年月日时分秒 的形式,如果时间为null，返回当前系统时间 
        /// <summary>
        /// 将日期时间格式化成 年月日时分秒 的形式,如果时间为null，返回当前系统时间
        /// </summary>
        /// <param name="dt">日期时间</param>
        /// <param name="separator1">日期分隔符</param>
        /// <param name="separator2">时间分隔符</param>
        /// <returns>日期时间字符串</returns>
        public string GetFormatDateTime(DateTime dt, char separator1, char separator2)
        {
            if (dt != null && !dt.Equals(DBNull.Value))
            {
                string tem = string.Format("yyyy{0}MM{1}dd hh{2}mm{3}ss", separator1, separator1, separator2, separator2);
                return dt.ToString(tem);
            }
            else
            {
                return GetFormatDateTime(DateTime.Now, separator1, separator2);
            }
        }
        #endregion

        #region  GMT时间转本地时间 
        /// <summary>  
        /// GMT时间转成本地时间  
        /// </summary>  
        /// <param name="gmt">字符串形式的GMT时间</param>  
        /// <returns>返回本地时间</returns>  
        public static DateTime GMTToLocal(string gmt)
        {
            DateTime dt = DateTime.MinValue;
            try
            {
                string pattern = "";
                if (gmt.IndexOf("+0") != -1)
                {
                    gmt = gmt.Replace("GMT", "");
                    pattern = "ddd, dd-MMM-yyyy HH':'mm':'ss zzz";
                }
                if (gmt.ToUpper().IndexOf("GMT") != -1)
                {
                    pattern = "ddd, dd-MMM-yyyy HH':'mm':'ss 'GMT'";
                }
                if (pattern != "")
                {
                    dt = DateTime.ParseExact(gmt, pattern, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal);
                    dt = dt.ToLocalTime();
                }
                else
                {
                    dt = Convert.ToDateTime(gmt);
                }
            }
            catch
            { }
            return dt;
        }
        #endregion

        #region  把秒转换成分钟 
        /// <summary>
        /// 把秒转换成分钟
        /// </summary>
        /// <param name="second">秒数</param>
        /// <returns>分钟数</returns>
        public static int SecondToMinute(int second)
        {
            decimal mm = (decimal)((decimal)second / (decimal)60);
            return Convert.ToInt32(Math.Ceiling(mm));
        }
        #endregion

        #region  返回某年某月最后一天 
        /// <summary>
        /// 返回某年某月最后一天
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>日</returns>
        public static int GetMonthLastDate(int year, int month)
        {
            DateTime lastDay = new DateTime(year, month, new GregorianCalendar().GetDaysInMonth(year, month));
            int Day = lastDay.Day;
            return Day;
        }
        #endregion

        #region  获得两个时间差 
        /// <summary>
        /// 获得两个时间差
        /// </summary>
        /// <param name="DateTime1">日期一</param>
        /// <param name="DateTime2">日期二</param>
        /// <returns>返回日期间隔的天数</returns>
        public static double DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
           // string dateDiff = null;
            try
            {
                TimeSpan ts = DateTime2 - DateTime1;
                return ts.TotalDays;
                //if (ts.Days >= 1)
                //{
                //    dateDiff = string.Format("{0} 年 {1} 月 {2} 天 ", DateTime1.Year, DateTime1.Month, DateTime1.Day);
                //}
                //else
                //{
                //    if (ts.Hours > 1)
                //    {
                //        dateDiff = ts.Hours + " 小时前";
                //    }
                //    else
                //    {
                //        dateDiff = ts.Minutes + " 分钟前";
                //    }
                //}
            }
            catch(Exception)
            { }
            return 0;
        }
        /// <summary>
        /// 计算2个时间差
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="mindTime">提醒时间,到了返回1,否则返回0</param>
        /// <returns>返回时间差</returns>
        public static string GetDiffTime(DateTime beginTime, DateTime endTime, ref int mindTime)
        {
            string strResout = string.Empty;
            //获得2时间的时间间隔秒计算
            //TimeSpan span = endTime - beginTime;
            TimeSpan span = endTime.Subtract(beginTime);

            int iTatol = Convert.ToInt32(span.TotalSeconds);
            int iMinutes = 1 * 60;
            int iHours = iMinutes * 60;
            int iDay = iHours * 24;
            int iMonth = iDay * 30;
            int iYear = iMonth * 12;

            //提醒时间,到了返回1,否则返回0
            if (mindTime > iTatol && iTatol > 0)
            {
                mindTime = 1;
            }
            else
            {
                mindTime = 0;
            }

            if (iTatol > iYear)
            {
                strResout += iTatol / iYear + " 年 ";
                iTatol %= iYear;//剩余
            }
            if (iTatol > iMonth)
            {
                strResout += iTatol / iMonth + " 月 ";
                iTatol %= iMonth;
            }
            if (iTatol > iDay)
            {
                strResout += iTatol / iDay + " 天 ";
                iTatol %= iDay;

            }
            if (iTatol > iHours)
            {
                strResout += iTatol / iHours + " 小时 ";
                iTatol %= iHours;
            }
            if (iTatol > iMinutes)
            {
                strResout += iTatol / iMinutes + " 分 ";
                iTatol %= iMinutes;
            }
            strResout += iTatol + " 秒 ";

            return strResout;
        }

        /// <summary>
        /// 计算2个时间差
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>返回时间差</returns>
        public static string GetDiffTime(DateTime beginTime, DateTime endTime)
        {
            string strResout = string.Empty;
            //获得2时间的时间间隔秒计算
            //TimeSpan span = endTime - beginTime;
            TimeSpan span = endTime.Subtract(beginTime);

            int iTatol = Convert.ToInt32(span.TotalSeconds);
            int iMinutes = 1 * 60;
            int iHours = iMinutes * 60;
            int iDay = iHours * 24;
            int iMonth = iDay * 30;
            int iYear = iMonth * 12;
            if (iTatol > iYear)
            {
                strResout += iTatol / iYear + " 年 ";
                iTatol %= iYear;//剩余
            }
            if (iTatol > iMonth)
            {
                strResout += iTatol / iMonth + " 月 ";
                iTatol %= iMonth;
            }
            if (iTatol > iDay)
            {
                strResout += iTatol / iDay + " 天 ";
                iTatol %= iDay;

            }
            if (iTatol > iHours)
            {
                strResout += iTatol / iHours + " 小时 ";
                iTatol %= iHours;
            }
            if (iTatol > iMinutes)
            {
                strResout += iTatol / iMinutes + " 分 ";
                iTatol %= iMinutes;
            }
            strResout += iTatol + " 秒 ";

            return strResout;
        }
        #endregion

        #region  获得两个日期的间隔 
        /// <summary>
        /// 获得两个日期的间隔
        /// </summary>
        /// <param name="DateTime1">日期一</param>
        /// <param name="DateTime2">日期二</param>
        /// <returns>日期间隔TimeSpan。</returns>
        public static TimeSpan DateDiff2(DateTime DateTime1, DateTime DateTime2)
        {
            TimeSpan ts = new TimeSpan();
            if (DateTime1 != null && DateTime2 != null)
            {
                TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                ts = ts1.Subtract(ts2).Duration();
            }
            return ts;
        }
        #endregion

        #region  获取某月份中最早的一天 
        /// <summary>
        /// 获取某月份中最早的一天
        /// </summary>
        /// <param name="Year">年份</param>
        /// <param name="Month">月份</param>
        /// <returns>返回月份中最早一天的日期</returns>
        private DateTime GetFirstDayOfMonth(int Year, int Month)
        {
            //你见过不是从1号开始的月份么？没有
            //那么，直接返回给调用者吧！
            //良好的一个编程习惯就是你的代码让人家看了简单易懂
            return Convert.ToDateTime(Year.ToString() + "-" + Month.ToString() + "-1");
        }
        /// <summary>
        /// 取得某月的第一天
        /// </summary>
        /// <param name="datetime">要取得月份第一天的时间</param>
        /// <returns>返回月份中最早一天的日期</returns>
        private DateTime FirstDayOfMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day);
        }
        #endregion

        #region  获取某月份中最后的一天 
        /// <summary>
        /// 获取某月份中最后的一天
        /// </summary>
        /// <param name="Year">年份</param>
        /// <param name="Month">月份</param>
        /// <returns>返回月份中最后一天的日期</returns>
        private DateTime GetLastDayOfMonth(int Year, int Month)
        {
            //这里的关键就是 DateTime.DaysInMonth 获得一个月中的天数
            int Days = DateTime.DaysInMonth(Year, Month);
            return Convert.ToDateTime(Year.ToString() + "-" + Month.ToString() + "-" + Days.ToString());

        }
        /// <summary>
        /// 取得某月的最后一天
        /// </summary>
        /// <param name="datetime">要取得月份最后一天的时间</param>
        /// <returns>返回月份中最后一天的日期</returns>
        private DateTime LastDayOfMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddMonths(1).AddDays(-1);
        }
        #endregion

        #region  获得当前日期所在周的第一天 
        /// <summary>
        /// 获得当前日期所在周的第一天
        /// </summary>
        /// <returns>返回当前周第一天日期</returns>
        public static DateTime GetFirstDayOfWeek()
        {
            int weeknow = Convert.ToInt32(DateTime.Now.DayOfWeek);
            int daydiff = (-1) * weeknow + 1;
            //本周第一天
            return System.DateTime.Now.AddDays(daydiff);
        }
        #endregion

        #region  获得当前日期所在周的最后一天 
        /// <summary>
        /// 获得当前日期所在周的最后一天
        /// </summary>
        /// <returns>返回当前周最后一天日期</returns>
        public static DateTime GetLastDayOfWeek()
        {
            int weeknow = Convert.ToInt32(DateTime.Now.DayOfWeek);
            int dayadd = 7 - weeknow;
            //本周最后一天
            return System.DateTime.Now.AddDays(dayadd);
        }
        #endregion

        #region  获得指定日期所在周的第一天 
        /// <summary>
        /// 获得指定日期所在周的第一天
        /// </summary>
        /// <param name="datetime">日期</param>
        /// <returns>返回指定日期第一天日期</returns>
        public static DateTime GetFirstDayOfWeek(DateTime datetime)
        {
            int weeknow = Convert.ToInt32(datetime.DayOfWeek);
            int daydiff = (-1) * weeknow + 1;
            //本周第一天
            return datetime.AddDays(daydiff);
        }
        #endregion

        #region  获得指定日期所在周的最后一天 
        /// <summary>
        /// 获得指定日期所在周的最后一天
        /// </summary>
        /// <param name="datetime">日期</param>
        /// <returns>返回指定日期最后一天日期</returns>
        public static DateTime GetLastDayOfWeek(DateTime datetime)
        {
            int weeknow = Convert.ToInt32(datetime.DayOfWeek);
            int dayadd = 7 - weeknow;
            //本周最后一天
            return datetime.AddDays(dayadd);
        }
        #endregion

        #region  取得指定日期上个月第一天 
        /// <summary>
        /// 取得指定日期上个月第一天
        /// </summary>
        /// <param name="datetime">指定日期</param>
        /// <returns>返回指定日期上月份中最早一天的日期</returns>
        public static DateTime FirstDayOfPreviousMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddMonths(-1);
        }
        #endregion

        #region  取得指定日期上个月的最后一天 
        /// <summary>
        /// 取得指定日期上个月的最后一天
        /// </summary>
        /// <param name="datetime">指定日期</param>
        /// <returns>返回指定日期上月份中最后一天的日期</returns>
        public static DateTime LastDayOfPrdviousMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddDays(-1);
        }
        #endregion

        #region  判断指定年份是否闰年 
        /// <summary>
        /// 判断指定年份是否闰年
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>闰年返回true;非闰年返回false</returns>
        public static bool IsLeap(int year)
        {
            return DateTime.IsLeapYear(year);
        }
        #endregion

        #region  根据年月日计算星期几 
        /// <summary>
        /// 根据年月日计算星期几
        /// </summary>
        /// <param name="y">年份</param>
        /// <param name="m">月份</param>
        /// <param name="d">日</param>
        /// <returns></returns>
        public static string CaculateWeekDay(int y, int m, int d)
        {
            if (m == 1) m = 13;
            if (m == 2) m = 14;
            int week = (d + 2 * m + 3 * (m + 1) / 5 + y + y / 4 - y / 100 + y / 400) % 7 + 1;
            string weekstr = "";
            switch (week)
            {
                case 1: weekstr = "星期一"; break;
                case 2: weekstr = "星期二"; break;
                case 3: weekstr = "星期三"; break;
                case 4: weekstr = "星期四"; break;
                case 5: weekstr = "星期五"; break;
                case 6: weekstr = "星期六"; break;
                case 7: weekstr = "星期日"; break;
            }

            return weekstr;
        }
        #endregion

        #region  得到随机日期 
        /// <summary>
        /// 得到随机日期
        /// </summary>
        /// <param name="datetime1">起始日期</param>
        /// <param name="datetime2">结束日期</param>
        /// <returns>间隔日期之间的 随机日期</returns>
        public static DateTime GetRandomTime(DateTime datetime1, DateTime datetime2)
        {
            Random random = new Random();
            DateTime minTime = new DateTime();
            DateTime maxTime = new DateTime();

            TimeSpan ts = new TimeSpan(datetime1.Ticks - datetime2.Ticks);

            // 获取两个时间相隔的秒数
            double dTotalSecontds = ts.TotalSeconds;
            int iTotalSecontds;
            if (dTotalSecontds > int.MaxValue)
            {
                iTotalSecontds = int.MaxValue;
            }
            else if (dTotalSecontds < int.MinValue)
            {
                iTotalSecontds = int.MinValue;
            }
            else
            {
                iTotalSecontds = (int)dTotalSecontds;
            }


            if (iTotalSecontds > 0)
            {
                minTime = datetime2;
            }
            else if (iTotalSecontds < 0)
            {
                minTime = datetime1;
            }
            else
            {
                return datetime1;
            }

            int maxValue = iTotalSecontds;

            if (iTotalSecontds <= int.MinValue)
                maxValue = int.MinValue + 1;

            int i = random.Next(System.Math.Abs(maxValue));

            return minTime.AddSeconds(i);
        }
        #endregion

        #region  获取台湾指定日期是本年的第几周 
        /// <summary>
        /// 获取台湾指定日期是本年的第几周
        /// </summary>
        /// <param name="datetime">日期</param>
        /// <returns>周数</returns>
        public static int GetWeekOfYearByTaiwan(DateTime datetime)
        {
            TaiwanCalendar tc = new TaiwanCalendar();
            return tc.GetWeekOfYear(datetime, CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday);//获取当前日期是本年的第几周
        }
        #endregion

        #region  获取台湾指定日期是本年的多少天 
        /// <summary>
        /// 获取台湾指定日期是本年的多少天
        /// </summary>
        /// <param name="datetime">日期</param>
        /// <returns>本年第几天</returns>
        public static int GetDayOfYearByTaiwan(DateTime datetime)
        {
            TaiwanCalendar tc = new TaiwanCalendar();
            return tc.GetDayOfYear(datetime);
        }
        #endregion

        #region  获取台湾指定年份的天数 
        /// <summary>
        /// 获取台湾指定年份的天数
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>年份天数</returns>
        public static int GetDaysInYearByTaiwan(int year)
        {
            TaiwanCalendar tc = new TaiwanCalendar();
            return tc.GetDaysInYear(year);
        }
        #endregion

        #region  获取台湾指定年月的天数 
        /// <summary>
        /// 获取台湾指定年月的天数
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>月份天数</returns>
        public static int GetDaysInMonthByTaiwan(int year, int month)
        {
            TaiwanCalendar tc = new TaiwanCalendar();

            return tc.GetDaysInMonth(year, month);
        }
        #endregion

        #region  获取指定年的闰月 
        /// <summary>
        /// 获取指定年的闰月
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>月份</returns>
        public static int GetLeapMonth(int year)
        {
            TaiwanCalendar tc = new TaiwanCalendar();
            return tc.GetLeapMonth(year);
        }
        #endregion

        #region  格式化时间模式 
        /// <summary>
        /// 格式化时间模式
        /// </summary>
        public enum DateFormt
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

        #region  格式化日期时间 

        /// <summary>
        /// 格式化日期时间
        /// </summary>
        /// <param name="dateTime">日期时间</param>
        /// <param name="dateMode">显示模式</param>
        /// <returns>0-9种模式的日期</returns>
        public static string FormatDate(DateTime dateTime, string dateMode)
        {
            switch (dateMode)
            {
                case "0":
                    return dateTime.ToString("yyyy-MM-dd");
                case "1":
                    return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                case "2":
                    return dateTime.ToString("yyyy/MM/dd");
                case "3":
                    return dateTime.ToString("yyyy年MM月dd日");
                case "4":
                    return dateTime.ToString("MM-dd");
                case "5":
                    return dateTime.ToString("MM/dd");
                case "6":
                    return dateTime.ToString("MM月dd日");
                case "7":
                    return dateTime.ToString("yyyy-MM");
                case "8":
                    return dateTime.ToString("yyyy/MM");
                case "9":
                    return dateTime.ToString("yyyy年MM月");
                default:
                    return dateTime.ToString();
            }
        }
        /// <summary>
        /// 格式化日期时间
        /// </summary>
        /// <param name="dateTime">日期时间</param>
        /// <param name="dateMode">显示模式</param>
        /// <returns>格式化后的日期</returns>
        public static string FormatDate(DateTime dateTime, DateFormt dateMode)
        {
            switch (dateMode)
            {
                case DateFormt.d:
                    return string.Format("{0:d}", dateTime);
                case DateFormt.D:
                    return string.Format("{0:D}", dateTime);
                case DateFormt.f:
                    return string.Format("{0:f}", dateTime);
                case DateFormt.F:
                    return string.Format("{0:F}", dateTime);
                case DateFormt.g:
                    return string.Format("{0:g}", dateTime);
                case DateFormt.G:
                    return string.Format("{0:G}", dateTime);
                case DateFormt.M:
                    return string.Format("{0:M}", dateTime);
                case DateFormt.R:
                    return string.Format("{0:R}", dateTime);
                case DateFormt.s:
                    return string.Format("{0:s}", dateTime);
                case DateFormt.t:
                    return string.Format("{0:t}", dateTime);
                case DateFormt.T:
                    return string.Format("{0:T}", dateTime);
                case DateFormt.u:
                    return string.Format("{0:u}", dateTime);
                case DateFormt.U:
                    return string.Format("{0:U}", dateTime);
                case DateFormt.Y:
                    return string.Format("{0:Y}", dateTime);
                case DateFormt.o:
                    return string.Format("{0}", dateTime);
                case DateFormt.a:
                    return string.Format("{0:yyyyMMddHHmmss}", dateTime);
                case DateFormt.b:
                    return string.Format("{0:yyyy-MM-dd HH:mm:ss}", dateTime);
                case DateFormt.c:
                    return string.Format("{0:yyyy-MM-dd}", dateTime);
                case DateFormt.s0:
                    return dateTime.GetDateTimeFormats('s')[0].ToString();
                case DateFormt.t0:
                    return dateTime.GetDateTimeFormats('t')[0].ToString();
                case DateFormt.y0:
                    return dateTime.GetDateTimeFormats('y')[0].ToString();
                case DateFormt.D0:
                    return dateTime.GetDateTimeFormats('D')[0].ToString();
                case DateFormt.D1:
                    return dateTime.GetDateTimeFormats('D')[1].ToString();
                case DateFormt.D2:
                    return dateTime.GetDateTimeFormats('D')[2].ToString();
                case DateFormt.D3:
                    return dateTime.GetDateTimeFormats('D')[3].ToString();
                case DateFormt.M0:
                    return dateTime.GetDateTimeFormats('M')[0].ToString();
                case DateFormt.f0:
                    return dateTime.GetDateTimeFormats('f')[0].ToString();
                case DateFormt.g0:
                    return dateTime.GetDateTimeFormats('g')[0].ToString();
                case DateFormt.r0:
                    return dateTime.GetDateTimeFormats('r')[0].ToString();
                default:
                    return dateTime.ToString();
            }
        }
        #endregion

        #region  获取指定周数的开始日期和结束日期，开始日期为周日 
        /// <summary>
        /// 获取指定周数的开始日期和结束日期，开始日期为周日
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="index">周数</param>
        /// <param name="first">当此方法返回时，则包含参数 year 和 index 指定的周的开始日期的 System.DateTime 值；如果失败，则为 System.DateTime.MinValue。</param>
        /// <param name="last">当此方法返回时，则包含参数 year 和 index 指定的周的结束日期的 System.DateTime 值；如果失败，则为 System.DateTime.MinValue。</param>
        /// <returns></returns>
        public static bool GetDaysOfWeeks(int year, int index, out DateTime first, out DateTime last)
        {
            first = DateTime.MinValue;
            last = DateTime.MinValue;
            if (year < 1700 || year > 9999)
            {
                //"年份超限"
                return false;
            }
            if (index < 1 || index > 53)
            {
                //"周数错误"
                return false;
            }
            DateTime startDay = new DateTime(year, 1, 1);  //该年第一天
            DateTime endDay = new DateTime(year + 1, 1, 1).AddMilliseconds(-1);
            int dayOfWeek = 0;
            if (Convert.ToInt32(startDay.DayOfWeek.ToString("d")) > 0)
                dayOfWeek = Convert.ToInt32(startDay.DayOfWeek.ToString("d"));  //该年第一天为星期几
            if (dayOfWeek == 7) { dayOfWeek = 0; }
            if (index == 1)
            {
                first = startDay;
                if (dayOfWeek == 6)
                {
                    last = first;
                }
                else
                {
                    last = startDay.AddDays((6 - dayOfWeek));
                }
            }
            else
            {
                first = startDay.AddDays((7 - dayOfWeek) + (index - 2) * 7); //index周的起始日期
                last = first.AddDays(6);
                if (last > endDay)
                {
                    last = endDay;
                }
            }
            if (first > endDay)  //startDayOfWeeks不在该年范围内
            {
                //"输入周数大于本年最大周数";
                return false;
            }
            return true;
        }
        #endregion 

        #region  判断时间段是否属于同一天 
        /// <summary>
        /// 一个时间段是否在同一天内
        /// </summary>
        /// <param name="startShortTime">开始时间（HH:mm:ss）</param>
        /// <param name="endshortTime">结束时间（HH:mm:ss）</param>
        /// <returns>在同一天范围内返回true,否则返回false,参数为null返回false</returns>
        public static bool IsInTheDay(string startShortTime, string endshortTime)
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
        /// <summary>
        /// 一个日期时间段是否在同一天内
        /// </summary>
        /// <param name="startTime">开始日期</param>
        /// <param name="endTime">结束日期</param>
        /// <returns>在同一天范围内返回true,否则返回false,参数为null返回false</returns>
        public static bool IsInTheDay(DateTime startTime, DateTime endTime)
        {
            if (startTime == null || endTime == null)
                return false;
            if (startTime < endTime)
                return false;
            else
                return true;
        }
        #endregion

        #region  当前时间是否处于时间范围内 
        /// <summary>
        /// 当前时间是否处于时间范围内
        /// </summary>
        /// <param name="startShortTime">开始时间（HH:mm:ss）</param>
        /// <param name="endshortTime">结束时间（HH:mm:ss）</param>
        /// <returns>范围内返回true，否则返回false</returns>
        public static bool IsInTime(string startShortTime, string endshortTime)
        {
            if (string.IsNullOrWhiteSpace(startShortTime) || string.IsNullOrWhiteSpace(endshortTime))
                return false;
            DateTime StartTime = DateTime.Parse(startShortTime);
            DateTime EndTime = DateTime.Parse(endshortTime);
            bool b;
            if (int.Parse(StartTime.ToString("HHmmss")) > int.Parse(EndTime.ToString("HHmmss")))
            {
                if (DateTime.Now >= StartTime && DateTime.Now <= EndTime.AddDays(1))
                    b = true;
                else
                    b = false;
            }
            else
            {
                if (DateTime.Now >= StartTime && DateTime.Now <= EndTime)
                    b = true;
                else
                    b = false;
            }
            return b;
        }
        /// <summary>
        /// 当前时间是否处于时间范围内
        /// </summary>
        /// <param name="startShortTime">开始时间（HH:mm:ss）</param>
        /// <param name="endshortTime">结束时间（HH:mm:ss）</param>
        /// <returns>范围内返回true，否则返回false</returns>
        public static bool IsInTime(DateTime StartTime, DateTime EndTime)
        {
            if (StartTime == null || EndTime == null)
                return false;
            if (StartTime.Year < EndTime.Year && StartTime.Year>DateTime.Now.Year)
                return false;
            bool b = false;
            if (StartTime.Year <= DateTime.Now.Year && DateTime.Now.Year <= EndTime.Year)
            {
                if (int.Parse(StartTime.ToString("HHmmss")) > int.Parse(EndTime.ToString("HHmmss")))
                {
                    if (DateTime.Now >= StartTime && DateTime.Now <= EndTime.AddDays(1))
                        b = true;
                    else
                        b = false;
                }
                else
                {
                    if (DateTime.Now >= StartTime && DateTime.Now <= EndTime)
                        b = true;
                    else
                        b = false;
                }
            }
            return b;
        }
        #endregion

        #region 获取时间字符串
        /// <summary>
        /// 获取时间字符串
        /// </summary>
        /// <param name="str">24小时制 数字日期字符串</param>
        /// <returns>返回时间格式字符串</returns>
        public static string GetDateTimeString(string str)
        {
            if (str.Length < 14)
                return "";
            return string.Format("{0}-{1}-{2} {3}:{4}:{5}", str.Substring(0, 4), str.Substring(4, 2), str.Substring(6, 2), str.Substring(8, 2), str.Substring(10, 2), str.Substring(12, 2));
        }
        #endregion

        #region 返回每月的第一天和最后一天
        /// <summary>
        /// 返回每月的第一天和最后一天
        /// </summary>
        /// <param name="month"></param>
        /// <param name="firstDay"></param>
        /// <param name="lastDay"></param>
        public static void ReturnDateFormat(int month, out string firstDay, out string lastDay)
        {
            int year = DateTime.Now.Year + month / 12;
            if (month != 12)
            {
                month %= 12;
            }
            switch (month)
            {
                case 1:
                    firstDay = DateTime.Now.ToString(year + "-0" + month + "-01");
                    lastDay = DateTime.Now.ToString(year + "-0" + month + "-31");
                    break;
                case 2:
                    firstDay = DateTime.Now.ToString(year + "-0" + month + "-01");
                    if (DateTime.IsLeapYear(DateTime.Now.Year))
                        lastDay = DateTime.Now.ToString(year + "-0" + month + "-29");
                    else
                        lastDay = DateTime.Now.ToString(year + "-0" + month + "-28");
                    break;
                case 3:
                    firstDay = DateTime.Now.ToString(year + "-0" + month + "-01");
                    lastDay = DateTime.Now.ToString("yyyy-0" + month + "-31");
                    break;
                case 4:
                    firstDay = DateTime.Now.ToString(year + "-0" + month + "-01");
                    lastDay = DateTime.Now.ToString(year + "-0" + month + "-30");
                    break;
                case 5:
                    firstDay = DateTime.Now.ToString(year + "-0" + month + "-01");
                    lastDay = DateTime.Now.ToString(year + "-0" + month + "-31");
                    break;
                case 6:
                    firstDay = DateTime.Now.ToString(year + "-0" + month + "-01");
                    lastDay = DateTime.Now.ToString(year + "-0" + month + "-30");
                    break;
                case 7:
                    firstDay = DateTime.Now.ToString(year + "-0" + month + "-01");
                    lastDay = DateTime.Now.ToString(year + "-0" + month + "-31");
                    break;
                case 8:
                    firstDay = DateTime.Now.ToString(year + "-0" + month + "-01");
                    lastDay = DateTime.Now.ToString(year + "-0" + month + "-31");
                    break;
                case 9:
                    firstDay = DateTime.Now.ToString(year + "-0" + month + "-01");
                    lastDay = DateTime.Now.ToString(year + "-0" + month + "-30");
                    break;
                case 10:
                    firstDay = DateTime.Now.ToString(year + "-" + month + "-01");
                    lastDay = DateTime.Now.ToString(year + "-" + month + "-31");
                    break;
                case 11:
                    firstDay = DateTime.Now.ToString(year + "-" + month + "-01");
                    lastDay = DateTime.Now.ToString(year + "-" + month + "-30");
                    break;
                default:
                    firstDay = DateTime.Now.ToString(year + "-" + month + "-01");
                    lastDay = DateTime.Now.ToString(year + "-" + month + "-31");
                    break;
            }
        }
        #endregion

        #region 时间戳
        /// <summary>
        /// DateTime转换为时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long GetTimeSpan(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            return (long)(time - startTime).TotalSeconds;
        }
        /// <summary>
        /// 时间戳转换为DateTime
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        public static DateTime TimeSpanToDateTime(long span)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            DateTime time = startTime.AddSeconds(span);
            return time;

        }
        #endregion
    }
}
