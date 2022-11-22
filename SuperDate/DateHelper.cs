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

        #region  获得两个时间差 
        /// <summary>
        /// 获得两个时间差
        /// </summary>
        /// <param name="DateTime1">日期一</param>
        /// <param name="DateTime2">日期二</param>
        /// <returns>返回日期间隔的天数</returns>
        public static double DateDiff(DateTime DateTime1, DateTime DateTime2)
        {

            TimeSpan ts = DateTime2 - DateTime1;
            return ts.TotalDays;
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
            TimeSpan ts = new();

            TimeSpan ts1 = new(DateTime1.Ticks);
            TimeSpan ts2 = new(DateTime2.Ticks);
            ts = ts1.Subtract(ts2).Duration();

            return ts;
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
            Random random = new();
            DateTime minTime = new();
            DateTime maxTime = new();

            TimeSpan ts = new(datetime1.Ticks - datetime2.Ticks);

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
            TaiwanCalendar tc = new();
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
            TaiwanCalendar tc = new();
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
            TaiwanCalendar tc = new();
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
            TaiwanCalendar tc = new();

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
            TaiwanCalendar tc = new();
            return tc.GetLeapMonth(year);
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
            DateTime startDay = new(year, 1, 1);  //该年第一天
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

        /// <summary>
        /// 返回某年某月最后一天
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>日</returns>
        public static int GetMonthLastDate(int year, int month)
        {
            DateTime lastDay = new(year, month, new GregorianCalendar().GetDaysInMonth(year, month));
            int Day = lastDay.Day;
            return Day;
        }
    }
}
