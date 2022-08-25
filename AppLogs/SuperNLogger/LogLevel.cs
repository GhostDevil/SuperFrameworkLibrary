/******************************************
 * 
 * 模块名称：日志级别
 * 当前版本：1.0
 * 开发人员：不良帥
 * 开发时间：2011-06-30
 * 版本历史：
 * 
 * Copyright (C) 不良帥 2011-2015  
 * 
 ******************************************/

namespace SuperFramework.SuperNLogger
{
    /// <summary>
    /// 日志级别  从低到高依次顺序：DEBUG|INFO|WARN|ERROR|FATAL
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// All
        /// </summary>
        All=0,
        /// <summary>
        /// DEBUG
        /// </summary>
        DEBUG = 1,
        /// <summary>
        /// INFO
        /// </summary>
        INFO = 2,
        /// <summary>
        /// WARN
        /// </summary>
        WARN = 3,
        /// <summary>
        /// ERROR
        /// </summary>
        ERROR = 4,
        /// <summary>
        /// FATAL
        /// </summary>
        FATAL = 5
    }

    /// <summary>
    /// 日志级别与字符串的相互转换
    /// </summary>
    internal class LogLevelHelper
    {
        /// <summary>
        /// 日志级别字符串转换为日志级别
        /// </summary>
        /// <param name="strLogLevel">日志级别字符串</param>
        /// <returns>日志级别，默认为DEBUG</returns>
        internal static LogLevel StrToLogLevel(string strLogLevel)
        {
            LogLevel _loglevel = LogLevel.DEBUG;
            if (string.IsNullOrEmpty(strLogLevel))
            {
                return _loglevel;
            }
            string level = strLogLevel.ToUpper();
            
            switch (level)
            {
                case "All":
                    _loglevel = LogLevel.All;
                    break;
                case "DEBUG":
                    _loglevel = LogLevel.DEBUG;
                    break;
                case "INFO":
                    _loglevel = LogLevel.INFO;
                    break;
                case "WARN":
                    _loglevel = LogLevel.WARN;
                    break;
                case "ERROR":
                    _loglevel = LogLevel.ERROR;
                    break;
                case "FATAL":
                    _loglevel = LogLevel.FATAL;
                    break;
            }
            return _loglevel;
        }
    }
}
